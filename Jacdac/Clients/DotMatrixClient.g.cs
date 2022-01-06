/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac {

    /// <summary>
    /// A rectangular dot matrix display, made of monochrome LEDs or Braille pins.
    /// Implements a client for the Dot Matrix service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/dotmatrix/" />
    public partial class DotMatrixClient : Client
    {
        public DotMatrixClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.DotMatrix)
        {
        }

        /// <summary>
        /// The state of the screen where dot on/off state is
        /// stored as a bit, column by column. The column should be byte aligned., 
        /// </summary>
        public byte[] Dots
        {
            get
            {
                return (byte[])this.GetRegisterValue((ushort)DotMatrixReg.Dots, DotMatrixRegPack.Dots);
            }
            set
            {
                
                this.SetRegisterValue((ushort)DotMatrixReg.Dots, DotMatrixRegPack.Dots, value);
            }

        }

        /// <summary>
        /// (Optional) Reads the general brightness of the display, brightness for LEDs. `0` when the screen is off., _: /
        /// </summary>
        public float Brightness
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)DotMatrixReg.Brightness, DotMatrixRegPack.Brightness);
            }
            set
            {
                
                this.SetRegisterValue((ushort)DotMatrixReg.Brightness, DotMatrixRegPack.Brightness, value);
            }

        }

        /// <summary>
        /// Number of rows on the screen, _: #
        /// </summary>
        public uint Rows
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)DotMatrixReg.Rows, DotMatrixRegPack.Rows);
            }
        }

        /// <summary>
        /// Number of columns on the screen, _: #
        /// </summary>
        public uint Columns
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)DotMatrixReg.Columns, DotMatrixRegPack.Columns);
            }
        }

        /// <summary>
        /// (Optional) Describes the type of matrix used., 
        /// </summary>
        public DotMatrixVariant Variant
        {
            get
            {
                return (DotMatrixVariant)this.GetRegisterValue((ushort)DotMatrixReg.Variant, DotMatrixRegPack.Variant);
            }
        }


    }
}