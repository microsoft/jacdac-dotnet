/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients 
{
    /// <summary>
    /// Allows for deployment and control over DeviceScript virtual machine.
     /// 
     /// Programs start automatically after device restart or uploading of new program.
     /// You can stop programs until next reset by setting the `running` and `autostart` registers to `false`.
    /// Implements a client for the DeviceScript Manager service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/devicescriptmanager/" />
    public partial class DeviceScriptManagerClient : Client
    {
        public DeviceScriptManagerClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.DeviceScriptManager)
        {
        }

        /// <summary>
        /// Reads the <c>running</c> register value.
        /// Indicates if the program is currently running.
        /// To restart the program, stop it (write `0`), read back the register to make sure it's stopped,
        /// start it, and read back., 
        /// </summary>
        public bool Running
        {
            get
            {
                return (bool)this.GetRegisterValueAsBool((ushort)DeviceScriptManagerReg.Running, DeviceScriptManagerRegPack.Running);
            }
            set
            {
                
                this.SetRegisterValue((ushort)DeviceScriptManagerReg.Running, DeviceScriptManagerRegPack.Running, value);
            }

        }

        /// <summary>
        /// Reads the <c>autostart</c> register value.
        /// Indicates wheather the program should be re-started upon `reboot()` or `panic()`.
        /// Defaults to `true`., 
        /// </summary>
        public bool Autostart
        {
            get
            {
                return (bool)this.GetRegisterValueAsBool((ushort)DeviceScriptManagerReg.Autostart, DeviceScriptManagerRegPack.Autostart);
            }
            set
            {
                
                this.SetRegisterValue((ushort)DeviceScriptManagerReg.Autostart, DeviceScriptManagerRegPack.Autostart, value);
            }

        }

        /// <summary>
        /// Reads the <c>program_size</c> register value.
        /// The size of current program., 
        /// </summary>
        public uint ProgramSize
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)DeviceScriptManagerReg.ProgramSize, DeviceScriptManagerRegPack.ProgramSize);
            }
        }

        /// <summary>
        /// Reads the <c>program_hash</c> register value.
        /// Return FNV1A hash of the current bytecode., 
        /// </summary>
        public uint ProgramHash
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)DeviceScriptManagerReg.ProgramHash, DeviceScriptManagerRegPack.ProgramHash);
            }
        }

        /// <summary>
        /// Reads the <c>program_sha256</c> register value.
        /// Return 32-byte long SHA-256 hash of the current bytecode., 
        /// </summary>
        public byte[] ProgramSha256
        {
            get
            {
                return (byte[])this.GetRegisterValue((ushort)DeviceScriptManagerReg.ProgramSha256, DeviceScriptManagerRegPack.ProgramSha256);
            }
        }

        /// <summary>
        /// Tries to read the <c>runtime_version</c> register value.
        /// Returns the runtime version number compatible with [Semver](https://semver.org/).
        /// When read as 32-bit little endian integer a version `7.15.500` would be `0x07_0F_01F4`., 
        /// </summary>
        bool TryGetRuntimeVersion(out object[] /*(uint, uint, uint)*/ value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)DeviceScriptManagerReg.RuntimeVersion, DeviceScriptManagerRegPack.RuntimeVersion, out values)) 
            {
                value = (object[] /*(uint, uint, uint)*/)values[0];
                return true;
            }
            else
            {
                value = default(object[] /*(uint, uint, uint)*/);
                return false;
            }
        }

        /// <summary>
        /// Reads the <c>program_name</c> register value.
        /// The name of currently running program. The compiler takes is from `package.json`., 
        /// </summary>
        public string ProgramName
        {
            get
            {
                return (string)this.GetRegisterValue((ushort)DeviceScriptManagerReg.ProgramName, DeviceScriptManagerRegPack.ProgramName);
            }
        }

        /// <summary>
        /// Reads the <c>program_version</c> register value.
        /// The version number of currently running program. The compiler takes is from `package.json`
        /// and `git`., 
        /// </summary>
        public string ProgramVersion
        {
            get
            {
                return (string)this.GetRegisterValue((ushort)DeviceScriptManagerReg.ProgramVersion, DeviceScriptManagerRegPack.ProgramVersion);
            }
        }

        /// <summary>
        /// Emitted when the program calls `panic(panic_code)` or `reboot()` (`panic_code == 0` in that case).
        /// The byte offset in byte code of the call is given in `program_counter`.
        /// The program will restart immediately when `panic_code == 0` or in a few seconds otherwise.
        /// </summary>
        public event ClientEventHandler ProgramPanic
        {
            add
            {
                this.AddEvent((ushort)DeviceScriptManagerEvent.ProgramPanic, value);
            }
            remove
            {
                this.RemoveEvent((ushort)DeviceScriptManagerEvent.ProgramPanic, value);
            }
        }

        /// <summary>
        /// Emitted after bytecode of the program has changed.
        /// </summary>
        public event ClientEventHandler ProgramChange
        {
            add
            {
                this.AddEvent((ushort)DeviceScriptManagerEvent.ProgramChange, value);
            }
            remove
            {
                this.RemoveEvent((ushort)DeviceScriptManagerEvent.ProgramChange, value);
            }
        }


        
        /// <summary>
        /// Open pipe for streaming in the bytecode of the program. The size of the bytecode has to be declared upfront.
        /// To clear the program, use `bytecode_size == 0`.
        /// The bytecode is streamed over regular pipe data packets.
        /// The bytecode shall be fully written into flash upon closing the pipe.
        /// If `autostart` is true, the program will start after being deployed.
        /// The data payloads, including the last one, should have a size that is a multiple of 32 bytes.
        /// Thus, the initial bytecode_size also needs to be a multiple of 32.
        /// </summary>
        public void DeployBytecode(uint bytecode_size)
        {
            this.SendCmdPacked((ushort)DeviceScriptManagerCmd.DeployBytecode, DeviceScriptManagerCmdPack.DeployBytecode, new object[] { bytecode_size });
        }

    }
}