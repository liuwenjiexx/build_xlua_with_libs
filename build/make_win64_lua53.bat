call windows.bat
set OUTPUT=build\lua-5.3.5\win\x64

if exist %OUTPUT% ( rmdir /Q/S %OUTPUT% )

mkdir %OUTPUT% & pushd %OUTPUT%
cmake -DPBC=ON -G %CMAKE_G% ../../../../
popd

cmake --build %OUTPUT% --config Release
md plugin_lua53\Plugins\x86_64
copy /Y %OUTPUT%\Release\xlua.dll plugin_lua53\Plugins\x86_64\xlua.dll

pause