# Starting with CMake 3.28,
# native support for .xcframework bundles has been introduced on Apple platforms
# https://cmake.org/cmake/help/latest/release/3.28.html
cmake_minimum_required(VERSION 3.28)

# Define the project
project(
  webrtc
  VERSION 2.4.0
  LANGUAGES C CXX ASM)

cmake_policy(SET CMP0076 NEW)

# Detect platforms early
if(CMAKE_SYSTEM_NAME STREQUAL "iOS")
  set(iOS TRUE)
elseif(CMAKE_SYSTEM_NAME STREQUAL "Darwin")
  set(macOS TRUE)
elseif(CMAKE_SYSTEM_NAME STREQUAL "Windows")
  set(Windows TRUE)
elseif(CMAKE_SYSTEM_NAME STREQUAL "Linux")
  set(Linux TRUE)
elseif(CMAKE_SYSTEM_NAME STREQUAL "Android")
  set(Android TRUE)
else()
  message(FATAL_ERROR "Unsupported platform: ${CMAKE_SYSTEM_NAME}")
endif()

# Standard configuration
set(CMAKE_C_STANDARD 17)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_C_STANDARD_REQUIRED ON)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Verbose builds
# set(CMAKE_VERBOSE_MAKEFILE ON)

# Thread model
set(CMAKE_THREAD_PREFER_PTHREAD TRUE)
find_package(Threads REQUIRED)

# Xcode compatibility check
if(XCODE)
  if(NOT DEFINED XCODE_VERSION)
    message(WARNING "XCODE_VERSION not defined.")
  elseif(XCODE_VERSION VERSION_LESS 11.0)
    message(FATAL_ERROR "The required Xcode version is 11.0 or higher.")
  endif()
endif()

# Linux-specific flags
# Use lld linker instead of GNU ld.
if(Linux)
  # set in CMakePresets.json instead
  # add_link_options(-fuse-ld=lld-11)
endif()

# set iterator debug level and compiler flags.
if(Windows AND CMAKE_CXX_COMPILER_ID STREQUAL "MSVC")
  set(CMAKE_CXX_FLAGS_DEBUG
      "${CMAKE_CXX_FLAGS_DEBUG} -D_ITERATOR_DEBUG_LEVEL=0 /Zi /Od")
else()
  set(CMAKE_CXX_FLAGS_DEBUG
      "${CMAKE_CXX_FLAGS_DEBUG} -D_ITERATOR_DEBUG_LEVEL=0")
endif()

# Extend module search path
list(APPEND CMAKE_MODULE_PATH "${CMAKE_SOURCE_DIR}/cmake")

# dependencies
find_package(WebRTC REQUIRED)

if(iOS OR macOS)
  find_package(Framework REQUIRED)
endif()

if(Windows OR Linux)
  find_package(Vulkan REQUIRED)

  # https://cmake.org/cmake/help/latest/module/FindCUDA.html#findcuda
  # The FindCUDA module is deprecated.
  # find_package(CUDA REQUIRED)
  find_package(CUDAToolkit REQUIRED)

  # Building CUDA kernel is not supported with Clang compiler on Windows.
  # https://gitlab.kitware.com/cmake/cmake/-/issues/20776
  if((Windows AND CMAKE_CXX_COMPILER_ID STREQUAL "MSVC") OR Linux)

    set(CMAKE_CUDA_FLAGS "${CMAKE_CUDA_FLAGS} -Wno-deprecated-gpu-targets")

    enable_language(CUDA)
  endif()

  add_subdirectory(NvCodec)
endif()

# Enable unit testing
enable_testing()

# Main project modules
add_subdirectory(WebRTCPlugin)
add_subdirectory(WebRTCPluginTest)
