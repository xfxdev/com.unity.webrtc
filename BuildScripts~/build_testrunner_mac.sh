#!/bin/bash
set -euo pipefail

export SOLUTION_DIR=$(pwd)/Plugin~

pushd "$SOLUTION_DIR"

cmake --preset=macos-arm64
cmake --build --preset=macos-arm64-release --target=WebRTCLibTest

popd

# Copy test runner
cp "$SOLUTION_DIR/out/build/macos-arm64/WebRTCPluginTest/Release/WebRTCLibTest" .