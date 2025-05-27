#!/bin/bash
set -euo pipefail

# export LIBWEBRTC_DOWNLOAD_URL=https://github.com/Unity-Technologies/com.unity.webrtc/releases/download/M116/webrtc-mac.zip
export SOLUTION_DIR=$(pwd)/Plugin~
export DYLIB_FILE=$(pwd)/Runtime/Plugins/macOS/libwebrtc.dylib

# Install cmake
# export HOMEBREW_NO_AUTO_UPDATE=1
# brew install cmake

# Download LibWebRTC 
# curl -L $LIBWEBRTC_DOWNLOAD_URL > webrtc.zip
# unzip -d $SOLUTION_DIR/webrtc webrtc.zip 


# Remove old build files to avoid cmake error:
# ---------------------------------------------
# CreateBuildOperation

# error: Could not delete `/.../Runtime/Plugins/macOS` because it was not created by the build system.
#   note: To mark this directory as deletable by the build system,
#   run `xattr -w com.apple.xcode.CreatedByBuildSystem true /.../Runtime/Plugins/macOS` when it is created.
# ** CLEAN FAILED **
# rm -rf "$(pwd)/Runtime/Plugins/macOS" || true

cd "$SOLUTION_DIR"

cmake --preset=x64-windows-msvc
cmake --build --preset=release-windows-msvc --target=WebRTCPlugin -- /verbosity:quiet

