#!/bin/bash
set -euo pipefail

BUILD_DIR="${BUILD_DIR:-$(pwd)/out/linux}"
ARTIFACTS_DIR="${ARTIFACTS_DIR:-$(pwd)/artifacts/linux}"
SKIP_SYNC=${SKIP_SYNC:-0}   # ← skip sync step

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/build_libwebrtc_common.sh"

if [[ "${SKIP_SYNC}" == 1 ]]; then
    log_info "SKIP_SYNC is set. Skipping gclient sync."
else
    setup_webrtc_env webrtc

    # add jsoncpp
    patch -N "$WEBRTC_DIR/src/BUILD.gn" < "$SCRIPT_DIR/patches/add_jsoncpp.patch"

    # Fix SetRawImagePlanes() in LibvpxVp8Encoder
    patch -N "$WEBRTC_DIR/src/modules/video_coding/codecs/vp8/libvpx_vp8_encoder.cc" < "$SCRIPT_DIR/patches/libvpx_vp8_encoder.patch"

    # Fix ExpectationToString() in SequenceCheckerImpl
    patch -N "$WEBRTC_DIR/src/rtc_base/synchronization/sequence_checker_internal.cc" < "$SCRIPT_DIR/patches/fix_sequence_check.patch"
fi


flavor=release

lib_name=$([ "$flavor" == "debug" ] && echo "libwebrtcd.a" || echo "libwebrtc.a")

build_dir="$BUILD_DIR/linux_x64_${flavor}"

if [[ "$flavor" == debug ]]; then
  build_webrtc    \
    --flavor "$flavor"  \
    --extra-args "rtc_use_x11=false is_asan=true is_lsan=true"    \
    "$build_dir" linux x64
else
  build_webrtc  \
    --flavor "$flavor"  \
    --extra-args "rtc_use_x11=false"    \
    "$build_dir" linux x64
fi

mkdir -p "$ARTIFACTS_DIR/lib/"
cp "$build_dir/obj/$lib_name" "$ARTIFACTS_DIR/lib/"


headers_dir="$ARTIFACTS_DIR/include/"
mkdir -p "$headers_dir"
copy_headers "$headers_dir"

echo "✅ Done!"