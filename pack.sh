msbuild -t:clean Jacdac.sln
msbuild -t:build -p:Configuration=Release Jacdac.sln
dotnet pack Jacdac.sln -c Release -o newpackages
nuget.exe pack ./Jacdac.TinyCLR/Jacdac.TinyCLR.csproj -Prop Configuration=Release -OutputDirectory ./newpackages