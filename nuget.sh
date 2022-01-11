dotnet nuget push ./bin/Release/packages/Jacdac.NET.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push ./bin/Release/packages/Jacdac.DevTools.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push ./bin/Release/Jacdac.TinyCLR.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push ./bin/Release/Jacdac.Nano.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
