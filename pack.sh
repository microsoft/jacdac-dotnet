# Jacdac.TinyCLR
cd Jacdac.TinyCLR
msbuild.exe -t:clean Jacdac.TinyCLR.csproj
nuget.exe restore Jacdac.TinyCLR.csproj -OutputDirectory packages -Verbosity detailed
ls -R
msbuild.exe -t:build -p:Configuration=Release Jacdac.TinyCLR.csproj -v:d
nuget.exe pack Jacdac.TinyCLR.csproj -Prop Configuration=Release -OutputDirectory ../newpackages -Verbosity detailed

#msbuild.exe -t:clean Jacdac.sln
#nuget.exe restore Jacdac.sln
#msbuild.exe -t:build -p:Configuration=Release Jacdac.sln
#dotnet pack Jacdac.sln -c Release -o newpackages
