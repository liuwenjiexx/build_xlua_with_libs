call ../windows.bat
set OUTPUT=build\lua-5.3.5\x64
set TARGET_DIR=lua-5.3.5\x64

if exist %OUTPUT% ( rmdir /Q/S %OUTPUT% )
mkdir %OUTPUT% & pushd %OUTPUT%

cmake -DLUAC_COMPATIBLE_FORMAT=ON -G %CMAKE_G%  -A x64 ../../../
IF %ERRORLEVEL% NEQ 0 cmake -DLUAC_COMPATIBLE_FORMAT=ON -G "Visual Studio 15 2017 Win64" ..
popd

cmake --build %OUTPUT% --config Release

mkdir %TARGET_DIR%
move .\%OUTPUT%\Release\lua.exe %TARGET_DIR%\lua.exe
move .\%OUTPUT%\Release\luac.exe %TARGET_DIR%\luac.exe

pause