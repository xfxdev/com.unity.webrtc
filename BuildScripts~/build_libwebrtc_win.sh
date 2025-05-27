#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

BUILD_DIR="${BUILD_DIR:-$SCRIPT_DIR/out/win}"
ARTIFACTS_DIR="${ARTIFACTS_DIR:-$SCRIPT_DIR/artifacts/win}"
SKIP_SYNC=${SKIP_SYNC:-0}   # ← skip sync step

source "$SCRIPT_DIR/build_libwebrtc_common.sh"

log_info "BUILD_DIR: '${BUILD_DIR}'"
log_info "ARTIFACTS_DIR: '${ARTIFACTS_DIR}'"

# !important
# tells depot_tools to use your locally installed version of Visual Studio.
# (by default, depot_tools will try to use a google-internal version).
export DEPOT_TOOLS_WIN_TOOLCHAIN=0 

if [[ "${SKIP_SYNC}" == 1 ]]; then
    log_info "SKIP_SYNC is set. Skipping gclient sync."
else
    setup_webrtc_env webrtc


    # add jsoncpp
    patch --binary -N "$WEBRTC_DIR/src/BUILD.gn" < "$SCRIPT_DIR/patches/add_jsoncpp.patch"

    # disable GCD taskqueue, use stdlib taskqueue instead
    # This is because GCD cannot measure with UnityProfiler
    patch --binary -N "$WEBRTC_DIR/src/api/task_queue/BUILD.gn" < "$SCRIPT_DIR/patches/disable_task_queue_gcd.patch"

    # Fix SetRawImagePlanes() in LibvpxVp8Encoder
    patch --binary -N "$WEBRTC_DIR/src/modules/video_coding/codecs/vp8/libvpx_vp8_encoder.cc" < "$SCRIPT_DIR/patches/libvpx_vp8_encoder.patch"

    # Fix ExpectationToString() in SequenceCheckerImpl
    patch --binary -N "$WEBRTC_DIR/src/rtc_base/synchronization/sequence_checker_internal.cc" < "$SCRIPT_DIR/patches/fix_sequence_check.patch"
fi


flavor=release

lib_name=$([ "$flavor" == "debug" ] && echo "webrtcd.lib" || echo "webrtc.lib")

build_dir="$BUILD_DIR/win_x64_${flavor}"
build_webrtc  \
    --flavor "$flavor"  \
    "$build_dir" win x64

mkdir -p "$ARTIFACTS_DIR/lib/"
cp "$build_dir/obj/$lib_name" "$ARTIFACTS_DIR/lib/"


headers_dir="$ARTIFACTS_DIR/include/"
mkdir -p "$headers_dir"
copy_headers "$headers_dir"

echo "✅ Done!"