# clean
msbuild.exe -t:clean Jacdac.sln

# Jacdac.TinyCLR
cd Jacdac.TinyCLR
nuget.exe restore Jacdac.TinyCLR.csproj -OutputDirectory packages -Verbosity detailed
ls -R
msbuild.exe -t:build -p:Configuration=Release Jacdac.TinyCLR.csproj -v:d
nuget.exe pack Jacdac.TinyCLR.csproj -Prop Configuration=Release -OutputDirectory ../newpackages -Verbosity detailed -PackagesDirectory ./packages
cd ..

# Jacdac.TinyCLR.Clients
cd Jacdac.TinyCLR.Clients
nuget.exe restore Jacdac.TinyCLR.Clients.csproj -OutputDirectory packages -Verbosity detailed
ls -R
msbuild.exe -t:build -p:Configuration=Release Jacdac.TinyCLR.Clients.csproj -v:d
nuget.exe pack Jacdac.TinyCLR.Clients.csproj -Prop Configuration=Release -OutputDirectory ../newpackages -Verbosity detailed -PackagesDirectory ./packages
cd ..

# Jacdac.TinyCLR.Storage
cd Jacdac.TinyCLR.Storage
nuget.exe restore Jacdac.TinyCLR.Storage.csproj -OutputDirectory packages -Verbosity detailed
ls -R
msbuild.exe -t:build -p:Configuration=Release Jacdac.TinyCLR.Storage.csproj -v:d
nuget.exe pack Jacdac.TinyCLR.Storage.csproj -Prop Configuration=Release -OutputDirectory ../newpackages -Verbosity detailed -PackagesDirectory ./packages
cd ..

# .NET assemblies
nuget.exe restore Jacdac.sln
msbuild.exe -t:build -p:Configuration=Release Jacdac.sln
dotnet pack Jacdac.sln -c Release -o newpackages
