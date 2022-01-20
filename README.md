# Jacdac for .NET

**Jacdac** is a plug-and-play hardware/software stack 
for **microcontrollers** and their peripherals (sensors/actuators), 
with applications to rapid prototyping, making, and physical computing. 

This repository contains **.NET Core (5+)**, **.NET nanoframework** and **TinyCLR** client libraries for the [Jacdac](https://aka.ms/jacdac) protocol,
as well as transports over USB, SPI, WebSockets.

* **[Jacdac .NET Documentation](https://microsoft.github.io/jacdac-docs/clients/dotnet)**
* **[Jacdac Documentation](https://aka.ms/jacdac/)**
* **[NuGet packages](https://www.nuget.org/profiles/Jacdac)**
* Discussions at https://github.com/microsoft/jacdac/discussions
* Issues are tracked on https://github.com/microsoft/jacdac/issues

## Assemblies

The Jacdac project contains C# sources of the Jacdac protocol for various .NET runtime, including desktop or TinyClR.
To avoid mscorlib issues, each platform recompiles these sources into its own assembly where C# files are simply shared as links.
As a result, the C# used in the Jacdac project is .NET 5+/.NET nanoframework/TinyCLR compatible (and also inherits limitations thereof).

### [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/introduction) and [.NET IoT Core](https://dotnet.microsoft.com/en-us/apps/iot)

  - `Jacdac.NET`, core runtime
  - `Jacdac.NET.Clients`, service clients
  - `Jacdac.NET.Logging`, a logging provider that allows to send log messages over the Jacdac bus
  - `Jacdac.NET.Transports.Spi`, SPI transport layer for SPI Jacdapter using .NET IoT
  - `Jacdac.NET.Transports.WebSockets`, WebSocket transport
  - `Jacdac.NET.Transports.Hf2`, HF2 protocol layer, .NET5
  - `Jacdac.NET.Transports.LibUsb`, Usb transport based on [libusb](https://libusb.info/), .NET5 **(not functional, experimental)**
  - `Jacdac.NET.Servers.SoundPlayer`, .NET sound player server implementation

### [TinyCLR](https://www.ghielectronics.com/tinyclr/)

  - `Jacdac.TinyCLR`, mirror of `Jacdac` library and UART transport
  - `Jacdac.TinyCLR.Clients`, service clients
  - `Jacdac.TinyCLR.Storage`, SD card storage support

### [.NET nanoframework](https://www.nanoframework.net/)

**Under development**: we need help to either write a native single wire serial transport... or build a SPI HAT for Feather S2.

  - `Jacdac.Nano`, mirror of `Jacdac` library and UART transport
  - `Jacdac.Nano.Transports.Spi`, SPI transport layer for SPI Jacdapter using .NET nanoframework
  - `Jacdac.Nano.Clients`, service clients

### Sample sources

The samples are mostly foud in `./Jacdac/Samples` and cross-compiled into these projects:
- `Jacdac.NET.Playground`, change the command line argument to match the sample type name
- `Jacdac.TinyCLR.Playground`, change the sample type name in `Program.cs`

### Misc:

  - `Jacdac.DevTools`, a .NET Core web site/proxy to help with developing Jacdac applications
  - `Jacdac.Tests`, unit tests, .NET6
  - `Jacdac`, C# Jacdac sources. This package serves as a placeholder for C# files and
    and is not referenced anywhere. **For development purposes only**.

### DevTools

Launch Jacdac.DevTools and use the `WebSocket` transport to connect your Jacdac bus to a local dashboard.
This allows you to test your code against simulators and physical devices, and leverage the tooling
available in the Jacdac network.

To installm,
```
dotnet tool install jacdac.devtools
```

To run,
```
jacdac.devtools
```

Add `spi` on Raspberry Pi to also connect to the JacHAT

## Developer setup

The rest of this page is for developers of the jacdac-dotnet library. For user documentation, go to https://microsoft.github.io/jacdac-docs/clients/dotnet.

* clone this repository and pull all submodules
```
git clone https://github.com/microsoft/jacdac-dotnet
git submodule update --init --recursive
git pull
```

* Restore Nuget packages. (Either in your preferred IDE/Editor or using `dotnet restore`).
* Execute the desired tool or build the core library using your IDE or `dotnet build`/`dotnet run`

## Testing with .NET and Jacdac development server

* install NodeJS 14+
* install Jacdac cli
```
npm install -g jacdac-cli
```

* launch Jacdac dev tools
```
jacdac devtools
```

* start running or debugging Jacdac.NET.Playground. The webdashboard will serve as a connector to the hardware.

### .NET IoT Raspberry Pi

* Open bash

```
sh ./publish.sh
```

* From a SSH session
```
cd dotnet
dotnet Jacdac.NET.Playground spi
```

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
