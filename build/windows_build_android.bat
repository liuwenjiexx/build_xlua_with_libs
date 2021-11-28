@echo off

set ANDROID_ABI=%1
set LUA_VERSION=%2
set cmake_version=3.6.4111459
set NDK_VERSION=android-ndk-r10e
set BUILD_PATH=build\lua-%LUA_VERSION%\android\%ANDROID_ABI%
set OUTPUT_PATH=.\plugin_lua53\Plugins\Android\libs\%ANDROID_ABI%
set CMAKE_BIN=%ANDROID_SDK%\cmake\%cmake_version%\bin
set CMAKE=%CMAKE_BIN%\cmake.exe
set NINJA=%CMAKE_BIN%\ninja.exe
set PATH=%CMAKE_BIN%;%PATH%
set ANDROID_NDK=%ANDROID_SDK%\ndk\%NDK_VERSION%

echo --- BUILD %LUA_VERSION% %ANDROID_ABI% Options ---
echo NDK: %ANDROID_NDK%
echo CMake: %CMake%
echo ABI: %ANDROID_ABI%
echo Output: %OUTPUT_PATH%
 
if exist "%BUILD_PATH%" ( rmdir /Q/S "%BUILD_PATH%" )
echo 0
mkdir "%BUILD_PATH%"
echo 1
%CMAKE% -H.\ -B.\%BUILD_PATH% "-GAndroid Gradle - Ninja" -DANDROID_ABI=%ANDROID_ABI% -DANDROID_NDK=%ANDROID_NDK% -DCMAKE_BUILD_TYPE=Relase -DCMAKE_MAKE_PROGRAM=%NINJA% -DCMAKE_TOOLCHAIN_FILE=./cmake/android.windows.toolchain.cmake "-DCMAKE_CXX_FLAGS=-std=c++11 -fexceptions"
echo 2
%NINJA% -C .\%BUILD_PATH%

if not exist "%OUTPUT_PATH%" (mkdir "%OUTPUT_PATH%")

move .\%BUILD_PATH%\libxlua.so %OUTPUT_PATH%\libxlua.so
echo --- BUILD %LUA_VERSION% %ANDROID_ABI% End ---
