#!/bin/bash
# PORT: shared helpers for the Unity build scripts. Sourced, not run.
#
# The point of this file is that nobody has to remember which Unity is installed, or that the
# project needs one particular version. It finds the editor, checks it against
# ProjectSettings/ProjectVersion.txt, and says something useful when it cannot.

set -o pipefail

# tools/ -> project root. BASH_SOURCE is unset when this is sourced from zsh rather than bash, and
# dirname of nothing is ".", which silently points the project root at the parent folder - so fall
# back to $0, and check the result actually looks like the project.
PORT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]:-$0}")/.." 2>/dev/null && pwd)"
if [ ! -f "$PORT_ROOT/ProjectSettings/ProjectVersion.txt" ]; then
	echo "These scripts must be run from inside the project (tools/unity.sh could not find" >&2
	echo "ProjectSettings/ProjectVersion.txt above \"$PORT_ROOT\")." >&2
	exit 1
fi
export PORT_ROOT

# Where the built games land. PortBuild writes ../Build relative to the project folder.
PORT_BUILD_DIR="$PORT_ROOT/../Build"
export PORT_BUILD_DIR

PORT_APP="$PORT_BUILD_DIR/Feer.app"
PORT_EXE="$PORT_BUILD_DIR/Feer.exe"

# The release asset the in-game updater looks for, per platform (PortPlatform.ReleaseAssetName).
PORT_MAC_ASSET="FEER-PC-osx.zip"

# The version this project must be opened with, read from the project itself so there is one
# source of truth. This matters more than usual here: the Mac build needs an arm64 editor, and
# Unity 2020.3 has none - see the README.
port_required_unity() {
	sed -n 's/^m_EditorVersion: *//p' "$PORT_ROOT/ProjectSettings/ProjectVersion.txt" | head -1
}

port_required_revision() {
	sed -n 's/^m_EditorVersionWithRevision: *//p' "$PORT_ROOT/ProjectSettings/ProjectVersion.txt" | head -1
}

# Echoes the path to a Unity editor, preferring one whose version matches the project.
#
# Order: $UNITY_PATH, then a Unity.app next to this checkout's siblings, then the Hub's editors,
# then /Applications. Among the candidates an exact version match wins, because having both 2020.3
# and 2022.3 installed is normal and picking the wrong one silently changes the result.
port_find_unity() {
	local required version candidate
	required="$(port_required_unity)"
	[ -z "$required" ] && required="2022.3.62f3"

	local candidates=()
	[ -n "$UNITY_PATH" ] && candidates+=("$UNITY_PATH")
	candidates+=("$HOME/Applications/Unity/Unity.app")
	candidates+=("/Applications/Unity/Unity.app")
	# Unity Hub keeps editors here, newest layout first.
	local d
	for d in "$HOME"/Applications/Unity/Hub/Editor/*/Unity.app \
	         "$HOME"/Library/Unity/Hub/Editor/*/Unity.app \
	         /Applications/Unity/Hub/Editor/*/Unity.app; do
		[ -x "$d/Contents/MacOS/Unity" ] && candidates+=("$d")
	done

	# First pass: exact match.
	for candidate in "${candidates[@]}"; do
		[ -x "$candidate/Contents/MacOS/Unity" ] || continue
		version="$("$candidate/Contents/MacOS/Unity" -version 2>/dev/null | head -1)"
		if [ "$version" = "$required" ]; then
			echo "$candidate"
			return 0
		fi
	done

	# Second pass: any editor, with a warning.
	for candidate in "${candidates[@]}"; do
		[ -x "$candidate/Contents/MacOS/Unity" ] || continue
		echo "$candidate"
		return 0
	done
	return 1
}

# Fails with an explanation rather than letting Unity print something cryptic.
port_require_unity() {
	local unity required found
	required="$(port_required_unity)"
	unity="$(port_find_unity)" || {
		echo "No Unity editor found." >&2
		echo "" >&2
		echo "Install Unity $required, or point UNITY_PATH at it:" >&2
		echo "  export UNITY_PATH=/Applications/Unity/Hub/Editor/$required/Unity.app" >&2
		echo "" >&2
		echo "On the Mac the Apple Silicon editor is the one to install; the Intel one cannot build" >&2
		echo "an arm64 player. The project is also openable from Unity Hub." >&2
		exit 1
	}

	found="$("$unity/Contents/MacOS/Unity" -version 2>/dev/null | head -1)"
	if [ "$found" != "$required" ]; then
		echo "WARNING: the project asks for Unity $required but $unity is $found." >&2
		echo "         Open it once in $required and let it upgrade the project, or the build will be" >&2
		echo "         from the wrong editor." >&2
		echo "" >&2
	fi
	echo "$unity"
}

# Runs Unity headless with the project's log in a predictable place, and returns its exit status.
# -buildTarget is passed by the caller: it has to be applied before the execute method runs.
port_run_unity() {
	local target="$1"
	local method="$2"
	local log="${3:-$PORT_ROOT/../Build/build-$target.log}"
	local unity
	unity="$(port_require_unity)"

	echo "Unity:   $unity ($("$unity/Contents/MacOS/Unity" -version 2>/dev/null | head -1))"
	echo "Target:  $target"
	echo "Method:  $method"
	echo "Log:     $log"
	echo ""

	mkdir -p "$(dirname "$log")"
	"$unity/Contents/MacOS/Unity" \
		-batchmode -quit -nographics \
		-buildTarget "$target" \
		-projectPath "$PORT_ROOT" \
		-executeMethod "$method" \
		-logFile "$log"
	local status=$?

	echo ""
	if [ $status -ne 0 ]; then
		echo "BUILD FAILED (exit $status). Last lines of $log:" >&2
		echo "" >&2
		grep -nE "error CS|PORTBUILD RESULT|Exception|error:" "$log" | tail -20 >&2
		return $status
	fi

	# Do not trust the build's own word for it: grep the log, then look at the artifact.
	if ! grep -q "PORTBUILD RESULT: Succeeded" "$log"; then
		echo "BUILD FAILED: no 'Succeeded' in the log. Last lines:" >&2
		tail -20 "$log" >&2
		return 1
	fi
	grep -E "PORTBUILD RESULT|PortBuild\]" "$log" | sed 's/^/  /'
	return 0
}

# Checks the Mac bundle with lipo, because the build log is not evidence.
port_verify_arm64() {
	local app="${1:-$PORT_APP}"
	if [ ! -d "$app" ]; then
		echo "VERIFY FAILED: no bundle at $app" >&2
		return 1
	fi
	local bad=0 total=0 file archs
	while IFS= read -r file; do
		total=$((total + 1))
		archs="$(lipo -archs "$file" 2>/dev/null)"
		if [ "$archs" != "arm64" ]; then
			echo "  NOT arm64: ${file#$app} = $archs" >&2
			bad=$((bad + 1))
		fi
	done <<EOF
$(find "$app" -type f -perm +111 2>/dev/null | while read -r f; do
	if file -b "$f" | grep -q Mach-O; then echo "$f"; fi
done)
EOF
	if [ "$total" -eq 0 ]; then
		echo "VERIFY FAILED: found no Mach-O binaries in $app" >&2
		return 1
	fi
	if [ "$bad" -ne 0 ]; then
		echo "VERIFY FAILED: $bad of $total binaries are not arm64 only" >&2
		return 1
	fi
	echo "  verified: $total binaries, all arm64 only"
	return 0
}

# True when Finder double-clicked a .command file: then the Terminal window must be held open or
# it closes before the output can be read.
port_pause_if_interactive() {
	if [ -t 1 ] && [ -z "$PORT_NO_PAUSE" ]; then
		echo ""
		read -r -p "Press return to close this window. " _
	fi
}