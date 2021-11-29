@echo off

set SOURCE=..\LibsTestProj\Assets\Test\TestProtobuf\Proto
set OUTPUT=CSharp/Proto

if not exist "%OUTPUT%" ( mkdir "%OUTPUT%" )
..\Tools\protoc.exe -I %SOURCE% --csharp_out=%OUTPUT% %SOURCE%/CS/LoginRequest.proto
..\Tools\protoc.exe -I %SOURCE% --csharp_out=%OUTPUT% %SOURCE%/SC/LoginResponse.proto

echo Build proto success

pause