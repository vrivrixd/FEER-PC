#!/bin/bash
# Double-click this in Finder to build the Windows game. See tools/build_windows.sh.
cd "$(dirname "$0")" || exit 1
./tools/build_windows.sh
