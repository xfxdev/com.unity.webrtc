#!/bin/bash
#
# Shared WebRTC single-slice build helper, to be sourced by per-platform scripts
#
# To source this file, you can use:
#   Absolute path: source /full/path/to/build_libwebrtc_common.sh
#   Relative path: source "$(dirname "${BASH_SOURCE[0]}")/build_libwebrtc_common.sh"

set -euo pipefail

# Default environment variables (can be overridden before sourcing)
DEPOT_TOOLS_DIR="${DEPOT_TOOLS_DIR:-$(pwd)/depot_tools}"
WEBRTC_VERSION="${WEBRTC_VERSION:-7151}"
WEBRTC_DIR="${WEBRTC_DIR:-$(pwd)}"
GIT_CACHE_DIR="${GIT_CACHE_DIR:-}"

WEBRTC_SRC_DIR="${WEBRTC_DIR}/src"
TARGET_REVISION="src@refs/remotes/branch-heads/${WEBRTC_VERSION}"

# Logging functions
log_info() { echo "INFO: $1"; }
log_warning() { echo "WARNING: $1" >&2; }
log_error() { echo "ERROR: $1" >&2; exit 1; }

# Add depot_tools to PATH
export PATH="$DEPOT_TOOLS_DIR:$PATH"
log_info "Added depot_tools to PATH"

# Execute a command with OS-specific handling
run_webrtc_cmd() {
  local cmd="$*"

  case "$(uname -s)" in
    Linux*|Darwin*)
      eval "$cmd" || log_error "Command failed: $cmd"
      ;;
    CYGWIN*|MINGW*|MSYS*)
      cmd.exe "/c $cmd" || log_error "Command failed: $cmd"
      ;;
    *)
      log_error "Unsupported OS: $(uname -s)"
      ;;
  esac
}

setup_webrtc_env() {
  local WEBRTC_SOLUTION_TYPE="${1:-webrtc}"  # $1 = solution type
  log_info "Starting WebRTC environment setup and sync"
  log_info "Version: ${WEBRTC_VERSION}, Solution: ${WEBRTC_SOLUTION_TYPE}"
  log_info "Depot tools directory: ${DEPOT_TOOLS_DIR}"
  log_info "WebRTC Working directory: ${WEBRTC_DIR}"

  # --- Setup depot_tools ---
  if [[ ! -d "${DEPOT_TOOLS_DIR}" ]]; then
    log_info "Cloning depot_tools into ${DEPOT_TOOLS_DIR}"
    git clone --depth 1 \
      https://chromium.googlesource.com/chromium/tools/depot_tools.git \
      "${DEPOT_TOOLS_DIR}" || log_error "Failed to clone depot_tools"
  else
    log_info "Updating existing depot_tools"
    (cd "${DEPOT_TOOLS_DIR}" && git pull --ff-only) || log_warning "Failed to fast-forward depot_tools"
  fi

  mkdir -p "${WEBRTC_DIR}"
  cd "${WEBRTC_DIR}" || log_error "Cannot cd to ${WEBRTC_DIR}"
  if [[ ! -d "${WEBRTC_SRC_DIR}" ]]; then
    rm -f "${WEBRTC_DIR}"/.gcli* || true  # remove .gclient* under ${WEBRTC_DIR} if ${WEBRTC_SRC_DIR} doesn't exist
    log_info "Performing initial fetch for solution type ${WEBRTC_SOLUTION_TYPE}"
    run_webrtc_cmd fetch --nohooks --no-history "${WEBRTC_SOLUTION_TYPE}"
  else
    log_info "Skipping initial fetch; '${WEBRTC_SRC_DIR}' already exists"
  fi

  log_info "Starting gclient sync to ${TARGET_REVISION}"

  if [[ -n "${GIT_CACHE_DIR}" ]]; then
    abspath=$(cd -- ${GIT_CACHE_DIR} && pwd)
    log_info "GIT_CACHE_DIR : ${abspath}"
    mkdir -p "${abspath}" # Ensure git-cache directory exists if specified
    export GIT_CACHE_PATH=${abspath}
  fi

  # Record start time
  local sync_start=${SECONDS}

  run_webrtc_cmd "gclient sync \
    -D \
    --force \
    --reset \
    --no-history \
    --shallow  \
    --revision ${TARGET_REVISION}"

  local sync_total_sec=$((${SECONDS} - sync_start))

  # Format duration into Hh Mm Ss
  local H=$((sync_total_sec / 3600))
  local M=$(((sync_total_sec % 3600) / 60))
  local S=$((sync_total_sec % 60))

  local DURATION_STR=""
  [ ${H} -gt 0 ] && DURATION_STR="${DURATION_STR}${H}h "
  [ ${M} -gt 0 ] && DURATION_STR="${DURATION_STR}${M}m "
  DURATION_STR="${DURATION_STR}${S}s"

  log_info "gclient sync completed in ${DURATION_STR}"

  # Verification
  if [[ ! -d "${WEBRTC_SRC_DIR}" ]]; then
    log_error "${WEBRTC_SRC_DIR} not found after sync."
  fi

  log_info "WebRTC source synchronization complete. source code for ${TARGET_REVISION} is ready at: ${WEBRTC_SRC_DIR}"

  cd - # Return to the original dir
}

# build_webrtc: build one slice
# Usage: build_webrtc [--flavor <debug|release>] [--extra-args "<gn_args>"] <target> <arch>
build_webrtc() {
  local flavor="release" extra_args=""

  # parse flags
  while [[ "$#" -gt 0 ]]; do
    case "$1" in
      --flavor)
        flavor="$2"; shift 2;;
      --extra-args)
        extra_args="$2"; shift 2;;
      --)
        shift; break;;
      -* )
        echo "Unknown option: $1" >&2; return 1;;
      *)
        break;;
    esac
  done

  # require positional args: build_path, target, arch
  if [[ $# -ne 3 ]]; then
    echo "Usage: build_webrtc [--flavor debug|release] [--extra-args '<gn_args>'] <build_dir> <target> <arch>" >&2
    return 1
  fi
  local build_dir="$1" target="$2" arch="$3"

  # determine debug flag
  local is_debug=false
  [[ "$flavor" == "debug" ]] && is_debug=true

  # assemble GN args
  local -a args=(
    "target_os=\"$target\""
    "target_cpu=\"$arch\""
    "is_debug=$is_debug"
    "is_component_build=false"
    "rtc_include_tests=false"
    "rtc_build_examples=false"
    "rtc_use_h264=false"
    "symbol_level=0"
    "use_custom_libcxx=false"
  )
  # append user-provided extras
  if [[ -n "$extra_args" ]]; then
    args+=( $extra_args )
  fi

  mkdir -p "$build_dir"
  echo "🔨 Building: $target / $arch / $flavor "

  local uname="$(uname -s)"
  if [[ "$uname" == CYGWIN* || "$uname" == MINGW* || "$uname" == MSYS* ]]; then

    export webrtc_dir_win=$(cygpath -w "${WEBRTC_DIR}")
    export build_dir_win=$(cygpath -w "${build_dir}")
    
    # generate build args
    echo ${args[*]} > ${build_dir}/args.gn
    cat <<EOF > ./_win_build_cmd.bat
    REM create link for shorter path
    mklink /d .\\_webrtc "${webrtc_dir_win}"

    cd .\\_webrtc

    call gn.bat gen "${build_dir_win}" --root=".\\src"
    call ninja.bat -C "${build_dir_win}" webrtc

    cd ..

    REM remove link
    rmdir .\\_webrtc
EOF
    cmd.exe "/c "_win_build_cmd.bat""
    rm "_win_build_cmd.bat"
  else
    cd "${WEBRTC_DIR}"
    gn gen "$build_dir" --root="$WEBRTC_SRC_DIR" --args="${args[*]}"
    # ninja -C "$build_dir" webrtc --quiet > /dev/null  # suppress normal log (stdout)
    ninja -C "$build_dir" webrtc
    cd -
  fi
}


copy_headers() {
  local target_dir=$1; shift

  # 1. target_dir must not be empty
  if [[ -z "$target_dir" ]]; then
    echo "Usage: copy_headers <target_dir> [--include paths...] [--exclude patterns...]"
    return 1
  fi

  cd $WEBRTC_SRC_DIR  # Change to WebRTC source directory

  # Default include paths
  local include_paths=(
    api
    audio
    call
    rtc_base
    common_audio
    common_video
    logging/rtc_event_log
    media
    modules/include
    modules/rtp_rtcp
    modules/video_coding
    p2p/base
    p2p/dtls
    pc
    sdk
    system_wrappers
    test
    third_party/abseil-cpp
    third_party/jsoncpp/source/include
    third_party/jsoncpp/generated
    third_party/libyuv/include
    video
  )

  # Default exclude patterns
  local exclude_paths=(
    */examples/*
  )

  # Parse extra arguments
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --include)
        shift
        include_paths+=( "$@" )
        break
        ;;
      --exclude)
        shift
        exclude_paths+=( "$@" )
        break
        ;;
      *)
        echo "Unknown argument: $1" >&2
        return 1
        ;;
    esac
  done

  # Find and copy, preserving directory structure
  # The following find command searches for .h and .inc files in the specified include_paths,
  # excluding any paths that match the patterns in exclude_paths.
  # The -print0 option is used to handle filenames with spaces or special characters.
  find "${include_paths[@]}" \
    \( $(printf -- '-path %q -o ' "${exclude_paths[@]}") -false \) -prune -o \
    -type f \( -name "*.h" -o -name "*.inc" \) -print0 \
  | while IFS= read -r -d '' file; do
      rel=${file#./}
      dst="$target_dir/$rel"
      mkdir -p "$(dirname "$dst")" || { echo "mkdir failed" >&2; exit 1; }
      cp "$file" "$dst"           || { echo "cp failed" >&2; exit 1; }
    done
  
  cd -  # Return to the original dir
}