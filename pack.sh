msbuild.exe -t:clean Jacdac.sln
nuget.exe restore ./Jacdac.TinyCLR/Jacdac.TinyCLR.csproj -OutputDirectory ./Jacdac.TinyCLR/packages -Verbosity detailed
ls -R
nuget.exe pack ./Jacdac.TinyCLR/Jacdac.TinyCLR.csproj -Prop Configuration=Release -OutputDirectory ./newpackages -Verbosity detailed
nuget.exe restore Jacdac.sln
msbuild.exe -t:build -p:Configuration=Release Jacdac.sln
dotnet pack Jacdac.sln -c Release -o newpackages
