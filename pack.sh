msbuild.exe -t:clean Jacdac.sln
msbuild.exe -t:build -p:Configuration=Release Jacdac.sln
dotnet pack Jacdac.sln -c Release -o newpackages
msbuild -t:restore ./Jacdac.TinyCLR/Jacdac.TinyCLR.csproj
msbuild -t:build ./Jacdac.TinyCLR/Jacdac.TinyCLR.csproj -p:Configuration=Release
nuget.exe pack ./Jacdac.TinyCLR/Jacdac.TinyCLR.csproj -Prop Configuration=Release -OutputDirectory ./newpackages