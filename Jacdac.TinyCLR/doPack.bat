@echo off

REM make sure have nuget.exe installed.
REM May change packages.config before run the command

NuGet.exe pack .\Jacdac.TinyCLR.csproj -Prop Configuration=Release