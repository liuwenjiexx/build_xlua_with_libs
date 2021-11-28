@REM for /f %%a in ('dir /a:d /b %ANDROID_SDK%\cmake\') do set cmake_version=%%a
@REM set cmake_bin=%ANDROID_SDK%\cmake\%cmake_version%\bin\cmake.exe
@REM set ninja_bin=%ANDROID_SDK%\cmake\%cmake_version%\bin\ninja.exe

@REM mkdir build_v7a
@REM %cmake_bin% -H.\ -B.\build_v7a "-GAndroid Gradle - Ninja" -DANDROID_ABI=armeabi-v7a -DANDROID_NDK=%ANDROID_NDK% -DCMAKE_BUILD_TYPE=Relase -DCMAKE_MAKE_PROGRAM=%ninja_bin% -DCMAKE_TOOLCHAIN_FILE=.\cmake\android.windows.toolchain.cmake "-DCMAKE_CXX_FLAGS=-std=c++11 -fexceptions"
@REM %ninja_bin% -C .\build_v7a
@REM mkdir .\plugin_lua53\Plugins\Android\Libs\armeabi-v7a
@REM move .\build_v7a\libxlua.so .\plugin_lua53\Plugins\Android\Libs\armeabi-v7a\libxlua.so

@REM mkdir build_android_x86
@REM %cmake_bin% -H.\ -B.\build_android_x86 "-GAndroid Gradle - Ninja" -DANDROID_ABI=x86 -DANDROID_NDK=%ANDROID_NDK% -DCMAKE_BUILD_TYPE=Relase -DCMAKE_MAKE_PROGRAM=%ninja_bin% -DCMAKE_TOOLCHAIN_FILE=.\cmake\android.windows.toolchain.cmake "-DCMAKE_CXX_FLAGS=-std=c++11 -fexceptions"
@REM %ninja_bin% -C .\build_android_x86
@REM mkdir .\plugin_lua53\Plugins\Android\Libs\x86
@REM move .\build_android_x86\libxlua.so .\plugin_lua53\Plugins\Android\Libs\x86\libxlua.so

set ANDROID_ABI=armeabi-v7a
set OUTPUT=build\android\%ANDROID_ABI%
set LUA_VERSION=5.3.5
set LUA_VERSION_Name=53

call windows_build_android.bat armeabi-v7a,%LUA_VERSION%
call windows_build_android.bat arm64-v8a,%LUA_VERSION%
call windows_build_android.bat x86,%LUA_VERSION%

echo "compile success"
pause