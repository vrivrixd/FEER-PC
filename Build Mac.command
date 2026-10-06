#!/bin/bash
# Double-click this in Finder to build the Mac game. See tools/build_macos.sh.
cd "$(dirname "$0")" || exit 1
./tools/build_macos.sh
