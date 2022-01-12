dotnet clean Jacdac.sln
dotnet build Jacdac.sln -c Release
dotnet pack Jacdac.sln -c Release -o newpackages
nuget.exe pack ./Jacdac.TinyCLR/Jacdac.TinyCLR.csproj -Prop Configuration=Release -OutputDirectory ./newpackages