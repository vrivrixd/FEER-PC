#!/bin/bash
# PORT: build the Mac game and package it as the release zip the in-game updater expects.
#
# The updater (PortUpdater) looks for the asset named FEER-PC-osx.zip on the latest GitHub
# release, unpacks it with ditto and swaps the whole .app. So the zip has to have Feer.app at its
# top level, and it has to be made with ditto: a zip made by anything else loses the symlinks and
# permissions inside the bundle, and macOS then refuses to launch what it just installed.
#
# Usage:
#   tools/make_release_macos.sh            # build and zip, using the version already in the project
#   tools/make_release_macos.sh 1.3        # also write that version into StreamingAssets
#
# After this, commit the version file if it changed, tag the release, and publish:
#   git commit -am "Version 1.3" && git push
#   gh release create v1.3 ../Build/FEER-PC-osx.zip

source "$(dirname "${BASH_SOURCE[0]}")/unity.sh"

VERSION_FILE="$PORT_ROOT/Assets/StreamingAssets/port_version.txt"
WANTED_VERSION="$1"

if [ -n "$WANTED_VERSION" ]; then
	CURRENT="$(tr -d '[:space:]' < "$VERSION_FILE" 2>/dev/null)"
	if [ "$CURRENT" != "$WANTED_VERSION" ]; then
		echo "Setting the bundled version $CURRENT -> $WANTED_VERSION"
		printf '%s' "$WANTED_VERSION" > "$VERSION_FILE"
		echo "  (commit $VERSION_FILE before tagging, or players will be told the wrong version)"
	else
		echo "Version already $WANTED_VERSION"
	fi
	echo ""
fi

echo "Building..."
echo ""
port_run_unity StandaloneOSX PortBuild.BuildMacOS "$PORT_BUILD_DIR/build-macos.log" || {
	echo ""
	port_pause_if_interactive
	exit 1
}

echo ""
echo "Checking the bundle..."
port_verify_arm64 || {
	echo ""
	port_pause_if_interactive
	exit 1
}

# arm64-only means no reason to ship an Intel slice, but make sure no Mac-only junk rides along.
echo ""
echo "Packaging..."
ZIP="$PORT_BUILD_DIR/$PORT_MAC_ASSET"
rm -f "$ZIP"
# --keepParent puts Feer.app at the top level, which is where the updater looks for it.
# --sequesterRsrc and the default keepParent together are what makes the result a real Mac zip.
ditto -c -k --sequesterRsrc --keepParent "$PORT_APP" "$ZIP" || {
	echo "ditto failed" >&2
	port_pause_if_interactive
	exit 1
}

VERSION="$(tr -d '[:space:]' < "$VERSION_FILE" 2>/dev/null)"

echo "  $ZIP"
du -sh "$ZIP" | sed 's/^/  /'
echo ""
echo "Contents at the top level (the updater needs the .app here):"
unzip -l "$ZIP" 2>/dev/null | awk '{print $4}' | grep -v '^$' | cut -d/ -f1 | sort -u | head -5 | sed 's/^/  /'
echo ""
echo "Bundled version: ${VERSION:-unknown}"
echo ""
echo "Next:"
echo "  git commit -am \"Version ${VERSION}\" && git push"
echo "  gh release create v${VERSION} \"$ZIP\""
port_pause_if_interactive