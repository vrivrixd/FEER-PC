#!/bin/bash
# Double-click this in Finder to build and package the Mac release zip.
# Pass a version to set it:  "./Make Mac Release.command" 1.3
cd "$(dirname "$0")" || exit 1
./tools/make_release_macos.sh "$@"
