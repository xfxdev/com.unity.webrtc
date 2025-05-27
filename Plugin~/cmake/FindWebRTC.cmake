# Find WebRTC include path

include(FindPackageHandleStandardArgs)

if(iOS)
  set(WEBRTC_DIR "${CMAKE_SOURCE_DIR}/webrtc/iOS")
elseif(macOS)
  set(WEBRTC_DIR "${CMAKE_SOURCE_DIR}/webrtc/macOS")
elseif(Windows)
  set(WEBRTC_DIR "${CMAKE_SOURCE_DIR}/webrtc/win")
elseif(Linux)
  set(WEBRTC_DIR "${CMAKE_SOURCE_DIR}/webrtc/linux")
elseif(Android)
  set(WEBRTC_DIR "${CMAKE_SOURCE_DIR}/webrtc/android")
else()
  message(FATAL_ERROR "Unsupported platform for WebRTC.")
endif()

# Include directories
set(WEBRTC_INCLUDE_DIR
  ${WEBRTC_DIR}/include
  ${WEBRTC_DIR}/include/third_party/abseil-cpp
  ${WEBRTC_DIR}/include/third_party/jsoncpp/source/include
  ${WEBRTC_DIR}/include/third_party/jsoncpp/generated
  ${WEBRTC_DIR}/include/third_party/libyuv/include
)

# Optional Objective-C includes
if(iOS OR macOS)
  list(APPEND WEBRTC_INCLUDE_DIR
    ${WEBRTC_DIR}/include/sdk/objc
    ${WEBRTC_DIR}/include/sdk/objc/base
  )
endif()

# Library directory
set(WEBRTC_LIBRARY_DIR
  ${WEBRTC_DIR}/lib
)

# Adjust library directory based on platform and architecture
# There is only `x64` on Windows, macOS, and Linux
# iOS and macOS use universal binary contains `x64` and `arm64`
if(Windows OR Linux)
  # set(WEBRTC_LIBRARY_DIR "${WEBRTC_LIBRARY_DIR}/x64")
elseif(Android)
  if(CMAKE_ANDROID_ARCH_ABI STREQUAL "x86_64")
    set(_ANDROID_ARCH "x64")
  else()
    set(_ANDROID_ARCH "${CMAKE_ANDROID_ARCH_ABI}")
  endif()
  set(WEBRTC_LIBRARY_DIR ${WEBRTC_LIBRARY_DIR}/${_ANDROID_ARCH})
elseif(iOS)
  # automatically detect whether the iOS build is for simulator or device
  if(CMAKE_OSX_SYSROOT MATCHES "iphonesimulator")
        # message(STATUS "Building for iOS Simulator")
        set(WEBRTC_LIBRARY_DIR "${WEBRTC_LIBRARY_DIR}/simulator")
    elseif(CMAKE_OSX_SYSROOT MATCHES "iphoneos")
        # message(STATUS "Building for iOS Device")
        set(WEBRTC_LIBRARY_DIR "${WEBRTC_LIBRARY_DIR}/device")
    else()
        message(FATAL_ERROR "Unknown iOS SDK: ${CMAKE_OSX_SYSROOT}")
    endif()
endif()

# Find debug and release libraries
find_library(WEBRTC_LIBRARY_DEBUG
  NAMES webrtcd
  PATHS ${WEBRTC_LIBRARY_DIR}
  NO_CMAKE_FIND_ROOT_PATH
)

find_library(WEBRTC_LIBRARY_RELEASE
  NAMES webrtc
  PATHS ${WEBRTC_LIBRARY_DIR}
  NO_CMAKE_FIND_ROOT_PATH
)

# Fallback: use release as debug if debug not found
if(NOT WEBRTC_LIBRARY_DEBUG)
  message(WARNING "Debug WebRTC library not found. Falling back to release: ${WEBRTC_LIBRARY_RELEASE}")
  set(WEBRTC_LIBRARY_DEBUG ${WEBRTC_LIBRARY_RELEASE})
endif()

# Cache the individual paths (optional but useful for GUI/config tools)
set(WEBRTC_LIBRARY_DEBUG "${WEBRTC_LIBRARY_DEBUG}" CACHE FILEPATH "WebRTC debug library")
set(WEBRTC_LIBRARY_RELEASE "${WEBRTC_LIBRARY_RELEASE}" CACHE FILEPATH "WebRTC release library")

# Multi-config link expression for target_link_libraries (not cached — avoids warning)
set(WEBRTC_LIBRARY
  debug ${WEBRTC_LIBRARY_DEBUG} 
  optimized ${WEBRTC_LIBRARY_RELEASE}
)

# Package status evaluation
find_package_handle_standard_args(WebRTC 
  REQUIRED_VARS
    WEBRTC_INCLUDE_DIR
    WEBRTC_LIBRARY_DEBUG
    WEBRTC_LIBRARY_RELEASE 
)

# Modern imported target
if(NOT TARGET WebRTC::WebRTC)
    add_library(WebRTC::WebRTC UNKNOWN IMPORTED)
    set_target_properties(WebRTC::WebRTC PROPERTIES
        INTERFACE_INCLUDE_DIRECTORIES "$<BUILD_INTERFACE:${WEBRTC_INCLUDE_DIR}>"
        IMPORTED_CONFIGURATIONS       "DEBUG;RELEASE"
        IMPORTED_LOCATION_DEBUG       "${WEBRTC_LIBRARY_DEBUG}"
        IMPORTED_LOCATION_RELEASE     "${WEBRTC_LIBRARY_RELEASE}")
endif()
