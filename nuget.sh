dotnet clean Jacdac.sln
dotnet build Jacdac.sln -c Release
dotnet pack Jacdac.sln -c Release
dotnet nuget push ./bin/Release/packages/Jacdac.NET.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push ./bin/Release/packages/Jacdac.DevTools*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push ./bin/Release/packages/Jacdac.TinyCLR.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push ./bin/Release/packages/Jacdac.Nano.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
