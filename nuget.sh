dotnet clean Jacdac.sln
dotnet build Jacdac.sln -c Release
dotnet pack Jacdac.sln -c Release
dotnet nuget push packages/*.nupkg -k $NUGET_TOKEN -s $NUGET_PUSH_URL
