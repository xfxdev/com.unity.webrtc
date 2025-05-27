#!/bin/bash
set -euo pipefail

# Read /etc/os-release to determine distro and version
if [[ -r /etc/os-release ]]; then
  source /etc/os-release
else
  echo "Cannot read /etc/os-release" >&2
  exit 1
fi

# Check for Ubuntu
if [[ "${ID,,}" != "ubuntu" ]]; then
  echo "ERROR: This script supports Ubuntu only (detected: $ID)." >&2
  exit 1
fi

# https://github.com/Unity-Technologies/com.unity.webrtc/blob/main/Plugin~/README.md#ubuntu
sudo apt install -y cmake wget clang-14 lld-14
sudo apt install -y libvulkan1 libvulkan-dev libglfw3-dev ninja-build

sudo apt install python3 python3-pip
pip install glad2

# cuda-toolkit
if [ ! -d "/usr/local/cuda" ]; then
    echo "install cuda-toolkit..."

    # Decide cuda repo_path
    # https://developer.nvidia.com/cuda-downloads?target_os=Linux&target_arch=x86_64
    if uname -r | grep -qi 'microsoft-standard-wsl2'; then
        # WSL2 → OK
        cuda_repo_path="wsl-ubuntu"
    elif uname -r | grep -qi microsoft; then
        # WSL1 → unsupported
        echo "ERROR: Detected WSL1; only WSL2 is supported." >&2
        exit 1
    elif [[ "$VERSION_ID" == "22.04" || "$VERSION_ID" == "24.04" ]]; then
        # Native Ubuntu 22.04 or 24.04
        cuda_repo_path="ubuntu${VERSION_ID//./}"
    else
        # Anything else → unsupported
        echo "ERROR: Unsupported platform (VERSION_ID=$VERSION_ID, kernel=$(uname -r))." >&2
        exit 1
    fi

    wget -nv https://developer.download.nvidia.com/compute/cuda/repos/${cuda_repo_path}/x86_64/cuda-keyring_1.1-1_all.deb

    sudo dpkg -i cuda-keyring_1.1-1_all.deb
    sudo apt-get update
    sudo apt-get -y install cuda-toolkit-12-9
fi

# https://docs.nvidia.com/cuda/cuda-installation-guide-linux/#environment-setup
export PATH="${PATH}":/usr/local/cuda-12.9/bin

# vulkansdk
if [[ ! -r "$HOME/vulkansdk/1.4.313.0/setup-env.sh" ]]; then
    echo "install vulkansdk..."

    # Be aware that using -q will suppresses all output, including error messages.
    # Using --no-verbose (or -nv) will suppress the progress output but keep the error messages.
    wget -nv https://sdk.lunarg.com/sdk/download/1.4.313.0/linux/vulkansdk-linux-x86_64-1.4.313.0.tar.xz
    mkdir $HOME/vulkansdk
    tar -xf vulkansdk-linux-x86_64-1.4.313.0.tar.xz -C $HOME/vulkansdk
fi
source $HOME/vulkansdk/1.4.313.0/setup-env.sh


export SOLUTION_DIR=$(pwd)/Plugin~
export OUTPUT_FILEPATH=$(pwd)/Runtime/Plugins/linux/libwebrtc.so

rm "$OUTPUT_FILEPATH" || true
mkdir -p "$(dirname "$OUTPUT_FILEPATH")" || true

# Build Plugin 
pushd "$SOLUTION_DIR"
cmake --preset x86_64-linux
cmake --build --preset release-linux --target WebRTCPlugin
popd

# strip --strip-unneeded "$OUTPUT_FILEPATH"
llvm-strip-14 --strip-unneeded "$OUTPUT_FILEPATH"