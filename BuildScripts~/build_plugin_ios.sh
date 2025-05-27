#!/bin/bash
set -euo pipefail

export LIBWEBRTC_DOWNLOAD_URL=https://github.com/Unity-Technologies/com.unity.webrtc/releases/download/M116/webrtc-ios.zip
export SOLUTION_DIR=$(pwd)/Plugin~
export WEBRTC_FRAMEWORK_DIR=$(pwd)/Runtime/Plugins/iOS

# Install cmake
# export HOMEBREW_NO_AUTO_UPDATE=1
# brew install cmake

# Download webrtc 
# curl -L $LIBWEBRTC_DOWNLOAD_URL > webrtc.zip
# unzip -d $SOLUTION_DIR/webrtc webrtc.zip 

# rm -rf "$SOLUTION_DIR/out"

cd "$SOLUTION_DIR"

cmake --preset ios-device
cmake --build --preset ios-device-release --target WebRTCPlugin -- -quiet

cmake --preset ios-simulator
cmake --build --preset ios-simulator-release --target WebRTCPlugin -- -quiet

rm -rf "$WEBRTC_FRAMEWORK_DIR/webrtc.xcframework"

echo "📦 Creating XCFramework..."
xcodebuild -create-xcframework \
  -framework "$SOLUTION_DIR/out/build/ios-device/WebRTCPlugin/release-iphoneos/webrtc.framework" \
  -framework "$SOLUTION_DIR/out/build/ios-simulator/WebRTCPlugin/release-iphonesimulator/webrtc.framework" \
  -output "$WEBRTC_FRAMEWORK_DIR/webrtc.xcframework"

echo "✅ Done!"