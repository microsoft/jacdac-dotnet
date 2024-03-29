/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients 
{
    /// <summary>
    /// Inter-Integrated Circuit (I2C, I²C, IIC) serial communication bus lets you communicate with
     /// many sensors and actuators.
    /// Implements a client for the I2C service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/i2c/" />
    public partial class I2CClient : Client
    {
        public I2CClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.I2C)
        {
        }

        /// <summary>
        /// Reads the <c>ok</c> register value.
        /// Indicates whether the I2C is working., 
        /// </summary>
        public bool Ok
        {
            get
            {
                return (bool)this.GetRegisterValueAsBool((ushort)I2CReg.Ok, I2CRegPack.Ok);
            }
        }


        
        /// <summary>
        /// `address` is 7-bit.
        /// `num_read` can be 0 if nothing needs to be read.
        /// The `write_buf` includes the register address if required (first one or two bytes).
        /// </summary>
        public void Transaction(uint address, uint num_read, byte[] write_buf)
        {
            this.SendCmdPacked((ushort)I2CCmd.Transaction, I2CCmdPack.Transaction, new object[] { address, num_read, write_buf });
        }

    }
}