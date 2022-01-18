dotnet nuget push ./newpackages/Jacdac.NET.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push ./newpackages/Jacdac.DevTools.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push ./newpackages/Jacdac.TinyCLR.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
