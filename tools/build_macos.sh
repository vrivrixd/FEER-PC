#!/bin/bash
# PORT: build the macOS player -> ../Build/Feer.app, then verify it with lipo.
#
# The verify is not decoration. Unity 2022.3 cannot restrict a macOS player to one architecture,
# so PortBuild thins the bundle afterwards, and a thinning step that silently matches no files
# would report success over a universal binary. See the README.

source "$(dirname "${BASH_SOURCE[0]}")/unity.sh"

echo "Building the macOS player..."
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

echo ""
echo "Done: $PORT_APP"
du -sh "$PORT_APP" | sed 's/^/  /'
echo ""
echo "Run it with:  open \"$PORT_APP\""
port_pause_if_interactive