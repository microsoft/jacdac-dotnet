version=$1

# clean
msbuild.exe -t:clean Jacdac.sln
rm -Rf ./newpackages

# restore
nuget restore Jacdac.sln -PackagesDirectory ./packages

# Jacdac.TinyCLR
cd Jacdac.TinyCLR
msbuild.exe -t:rebuild -p:Configuration=Release Jacdac.TinyCLR.csproj
nuget.exe pack Jacdac.TinyCLR.csproj -Prop Configuration=Release -OutputDirectory ../newpackages -PackagesDirectory ../packages
cd ..

# Jacdac.TinyCLR.Clients
cd Jacdac.TinyCLR.Clients
msbuild.exe -t:rebuild -p:Configuration=Release Jacdac.TinyCLR.Clients.csproj
nuget.exe pack Jacdac.TinyCLR.Clients.csproj -Prop Configuration=Release -OutputDirectory ../newpackages -PackagesDirectory ../packages
cd ..

# Jacdac.TinyCLR.Storage
cd Jacdac.TinyCLR.Storage
msbuild.exe -t:rebuild -p:Configuration=Release Jacdac.TinyCLR.Storage.csproj
nuget.exe pack Jacdac.TinyCLR.Storage.csproj -Prop Configuration=Release -OutputDirectory ../newpackages -PackagesDirectory ../packages
cd ..

# Jacdac.Nano
cd Jacdac.Nano
msbuild.exe -t:rebuild -p:Configuration=Release Jacdac.Nano.nfproj
nuget.exe pack Jacdac.Nano.nuspec -Version=$version -OutputDirectory ../newpackages -PackagesDirectory ../packages
cd ..

# .NET assemblies
msbuild.exe -t:rebuild -p:Configuration=Release Jacdac.sln
dotnet pack Jacdac.DevTools/Jacdac.DevTools.csproj -c Release -o newpackages
dotnet pack Jacdac.NET/Jacdac.NET.csproj -c Release -o newpackages
dotnet pack Jacdac.NET.Clients/Jacdac.NET.Clients.csproj -c Release -o newpackages
dotnet pack Jacdac.NET.Transports.Hf2/Jacdac.NET.Transports.Hf2.csproj -c Release -o newpackages
dotnet pack Jacdac.NET.Transports.Spi/Jacdac.NET.Transports.Spi.csproj -c Release -o newpackages
dotnet pack Jacdac.NET.Transports.WebSockets/Jacdac.NET.Transports.WebSockets.csproj -c Release -o newpackages
dotnet pack Jacdac.NET.Servers.SoundPlayer/Jacdac.NET.Servers.SoundPlayer.csproj -c Release -o newpackages

# let's see  who got built
ls newpackages