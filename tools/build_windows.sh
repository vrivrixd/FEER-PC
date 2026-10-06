#!/bin/bash
# PORT: build the Windows player -> ../Build/Feer.exe.
#
# Usable from the Mac, but only if the Windows Build Support module is installed in the same
# editor. Check in Unity Hub under that editor's Installs > Add modules.

source "$(dirname "${BASH_SOURCE[0]}")/unity.sh"

echo "Building the Windows player..."
echo ""
port_run_unity StandaloneWindows64 PortBuild.BuildWindows "$PORT_BUILD_DIR/build-windows.log" || {
	echo ""
	port_pause_if_interactive
	exit 1
}

echo ""
if [ -f "$PORT_EXE" ]; then
	echo "Done: $PORT_EXE"
	du -sh "$PORT_EXE" | sed 's/^/  /'
else
	echo "The build reported success but $PORT_EXE is not there." >&2
	port_pause_if_interactive
	exit 1
fi
port_pause_if_interactive