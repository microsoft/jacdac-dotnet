dotnet clean Jacdac.sln
dotnet build Jacdac.sln -c Release
dotnet pack Jacdac.sln -c Release -o newpackages
#rm ./newpackages/Jacdac.*.Playground.exe
#dotnet nuget push ./newpackages/Jacdac.NET.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
#dotnet nuget push ./newpackages/Jacdac.DevTools*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
