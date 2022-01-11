dotnet nuget push packages/Jacdac.NET.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push packages/Jacdac.TinyCLR.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
dotnet nuget push packages/Jacdac.Nano.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
