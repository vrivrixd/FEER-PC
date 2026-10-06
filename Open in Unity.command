#!/bin/bash
# Double-click this in Finder to open the project in the editor Unity asks for.
cd "$(dirname "$0")" || exit 1
source ./tools/unity.sh
UNITY="$(port_require_unity)"
exec "$UNITY/Contents/MacOS/Unity" -projectPath "$PWD"
