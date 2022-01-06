/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients {

    /// <summary>
    /// Support for sending and receiving packets using the [Bit Radio protocol](https://github.com/microsoft/pxt-common-packages/blob/master/libs/radio/docs/reference/radio.md), typically used between micro:bit devices.
    /// Implements a client for the bit:radio service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/bitradio/" />
    public partial class BitRadioClient : Client
    {
        public BitRadioClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.BitRadio)
        {
        }

        /// <summary>
        /// Turns on/off the radio antenna., 
        /// </summary>
        public bool Enabled
        {
            get
            {
                return (bool)this.GetRegisterValue((ushort)BitRadioReg.Enabled, BitRadioRegPack.Enabled);
            }
            set
            {
                
                this.SetRegisterValue((ushort)BitRadioReg.Enabled, BitRadioRegPack.Enabled, value);
            }

        }

        /// <summary>
        /// Group used to filter packets, 
        /// </summary>
        public uint Group
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)BitRadioReg.Group, BitRadioRegPack.Group);
            }
            set
            {
                
                this.SetRegisterValue((ushort)BitRadioReg.Group, BitRadioRegPack.Group, value);
            }

        }

        /// <summary>
        /// Antenna power to increase or decrease range., 
        /// </summary>
        public uint TransmissionPower
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)BitRadioReg.TransmissionPower, BitRadioRegPack.TransmissionPower);
            }
            set
            {
                
                this.SetRegisterValue((ushort)BitRadioReg.TransmissionPower, BitRadioRegPack.TransmissionPower, value);
            }

        }

        /// <summary>
        /// Change the transmission and reception band of the radio to the given channel., 
        /// </summary>
        public uint FrequencyBand
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)BitRadioReg.FrequencyBand, BitRadioRegPack.FrequencyBand);
            }
            set
            {
                
                this.SetRegisterValue((ushort)BitRadioReg.FrequencyBand, BitRadioRegPack.FrequencyBand, value);
            }

        }


        
        /// <summary>
        /// Sends a string payload as a radio message, maximum 18 characters.
        /// </summary>
        public void SendString(string message)
        {
            this.SendCmdPacked((ushort)BitRadioCmd.SendString, BitRadioCmdPack.SendString, new object[] { message });
        }

        
        /// <summary>
        /// Sends a double precision number payload as a radio message
        /// </summary>
        public void SendNumber(float value)
        {
            this.SendCmdPacked((ushort)BitRadioCmd.SendNumber, BitRadioCmdPack.SendNumber, new object[] { value });
        }

        
        /// <summary>
        /// Sends a double precision number and a name payload as a radio message
        /// </summary>
        public void SendValue(float value, string name)
        {
            this.SendCmdPacked((ushort)BitRadioCmd.SendValue, BitRadioCmdPack.SendValue, new object[] { value, name });
        }

        
        /// <summary>
        /// Sends a payload of bytes as a radio message
        /// </summary>
        public void SendBuffer(byte[] data)
        {
            this.SendCmdPacked((ushort)BitRadioCmd.SendBuffer, BitRadioCmdPack.SendBuffer, new object[] { data });
        }

    }
}