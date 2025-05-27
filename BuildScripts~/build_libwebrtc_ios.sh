#!/bin/bash
set -euo pipefail

BUILD_DIR="${BUILD_DIR:-$(pwd)/out/iOS}"
ARTIFACTS_DIR="${ARTIFACTS_DIR:-$(pwd)/artifacts/iOS}"
SKIP_SYNC=${SKIP_SYNC:-0}   # ← skip sync step

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/build_libwebrtc_common.sh"

if [[ "${SKIP_SYNC}" == 1 ]]; then
    log_info "SKIP_SYNC is set. Skipping gclient sync."
else
    setup_webrtc_env webrtc_ios

    # add jsoncpp
    patch -N "$WEBRTC_DIR/src/BUILD.gn" < "$SCRIPT_DIR/patches/add_jsoncpp.patch"

    # disable GCD taskqueue, use stdlib taskqueue instead
    # This is because GCD cannot measure with UnityProfiler
    patch -N "$WEBRTC_DIR/src/api/task_queue/BUILD.gn" < "$SCRIPT_DIR/patches/disable_task_queue_gcd.patch"

    # add objc library to use videotoolbox
    patch -N "$WEBRTC_DIR/src/sdk/BUILD.gn" < "$SCRIPT_DIR/patches/add_objc_deps.patch"

    # Fix SetRawImagePlanes() in LibvpxVp8Encoder
    patch -N "$WEBRTC_DIR/src/modules/video_coding/codecs/vp8/libvpx_vp8_encoder.cc" < "$SCRIPT_DIR/patches/libvpx_vp8_encoder.patch"

    # Fix ExpectationToString() in SequenceCheckerImpl
    patch -N "$WEBRTC_DIR/src/rtc_base/synchronization/sequence_checker_internal.cc" < "$SCRIPT_DIR/patches/fix_sequence_check.patch"
fi



flavor=release

lib_name=$([ "$flavor" == "debug" ] && echo "libwebrtcd.a" || echo "libwebrtc.a")

# "target_environment= "device", "simulator" or "catalyst". Must be specified.

# note: `treat_warnings_as_errors=false` is for avoiding LLVM warning.
#       https://reviews.llvm.org/D72212
#       See below for details.
#       https://bugs.chromium.org/p/webrtc/issues/detail?id=11729

# device
build_dir="$BUILD_DIR/ios_device_arm64_${flavor}"
build_webrtc    \
    --flavor "$flavor"  \
    --extra-args "target_environment=\"device\" ios_enable_code_signing=false treat_warnings_as_errors=false" \
    $build_dir ios arm64

mkdir -p "$ARTIFACTS_DIR/lib/device"
cp "$build_dir/obj/$lib_name" "$ARTIFACTS_DIR/lib/device/$lib_name"


# simulator
lib_paths=()
for arch in arm64 x64; do
    build_dir="$BUILD_DIR/ios_simulator_${arch}_${flavor}"
    build_webrtc   \
        --flavor "$flavor"  \
        --extra-args "target_environment=\"simulator\" ios_enable_code_signing=false treat_warnings_as_errors=false" \
        $build_dir ios "$arch"
    lib_paths+=("$build_dir/obj/libwebrtc.a")
done

mkdir -p "$ARTIFACTS_DIR/lib/simulator"
lipo -create "${lib_paths[@]}" -output "$ARTIFACTS_DIR/lib/simulator/$lib_name"

headers_dir="$ARTIFACTS_DIR/include/"
mkdir -p "$headers_dir"
copy_headers "$headers_dir"

echo "✅ Done!"