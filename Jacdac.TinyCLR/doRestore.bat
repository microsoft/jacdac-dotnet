@echo off

REM make sure have nuget.exe installed.
REM May change packages.config before run the command
copy .\packages.restore .\packages.config
nuget.exe restore .\Jacdac.TinyCLR.csproj -OutputDirectory .\packages
del /q .\packages.config