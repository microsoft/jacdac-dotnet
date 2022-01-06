dotnet publish -c Debug
scp -r ./bin/Debug/net6.0/publish/* pi@192.168.0.131:/home/pi/dotnet/
