call windows.bat
set OUTPUT=build64

mkdir %OUTPUT% & pushd %OUTPUT%
cmake -DLUAC_COMPATIBLE_FORMAT=ON -G "Visual Studio 14 2015 Win64" ..
popd
cmake --build %OUTPUT% --config Release
md plugin_lua53\Plugins\x86_64
copy /Y %OUTPUT%\Release\xlua.dll plugin_lua53\Plugins\x86_64\xlua.dll
pause