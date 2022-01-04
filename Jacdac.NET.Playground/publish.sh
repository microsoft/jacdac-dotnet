dotnet publish -c Debug
scp -r ./bin/Debug/net6.0/publish/* pi@raspberrypi:/home/pi/dotnet/
