dotnet clean Jacdac.sln
dotnet build Jacdac.sln -c Release
dotnet pack Jacdac.sln -c Release -o packages
dotnet nuget push packages/Jacdac.NET.*.nupkg -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json
