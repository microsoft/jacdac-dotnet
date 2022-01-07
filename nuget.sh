dotnet clean Jacdac.sln
dotnet build Jacdac.sln -c Release
dotnet pack Jacdac.sln -c Release -o packages
dotnet nuget push packages/*.nupkg -k $NUGET_TOKEN -s $NUGET_PUSH_URL
