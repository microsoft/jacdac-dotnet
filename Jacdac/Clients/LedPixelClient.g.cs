/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients {

    /// <summary>
    /// A controller for strips of individually controlled RGB LEDs.
    /// Implements a client for the LED Pixel service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/ledpixel/" />
    public partial class LedPixelClient : Client
    {
        public LedPixelClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.LedPixel)
        {
        }

        /// <summary>
        /// Set the luminosity of the strip.
        /// At `0` the power to the strip is completely shut down., _: /
        /// </summary>
        public float Brightness
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)LedPixelReg.Brightness, LedPixelRegPack.Brightness);
            }
            set
            {
                
                this.SetRegisterValue((ushort)LedPixelReg.Brightness, LedPixelRegPack.Brightness, value);
            }

        }

        /// <summary>
        /// This is the luminosity actually applied to the strip.
        /// May be lower than `brightness` if power-limited by the `max_power` register.
        /// It will rise slowly (few seconds) back to `brightness` is limits are no longer required., _: /
        /// </summary>
        public float ActualBrightness
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)LedPixelReg.ActualBrightness, LedPixelRegPack.ActualBrightness);
            }
        }

        /// <summary>
        /// Specifies the type of light strip connected to controller.
        /// Controllers which are sold with lights should default to the correct type
        /// and could not allow change., 
        /// </summary>
        public LedPixelLightType LightType
        {
            get
            {
                return (LedPixelLightType)this.GetRegisterValue((ushort)LedPixelReg.LightType, LedPixelRegPack.LightType);
            }
            set
            {
                
                this.SetRegisterValue((ushort)LedPixelReg.LightType, LedPixelRegPack.LightType, value);
            }

        }

        /// <summary>
        /// Specifies the number of pixels in the strip.
        /// Controllers which are sold with lights should default to the correct length
        /// and could not allow change. Increasing length at runtime leads to ineffective use of memory and may lead to controller reboot., _: #
        /// </summary>
        public uint NumPixels
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)LedPixelReg.NumPixels, LedPixelRegPack.NumPixels);
            }
            set
            {
                
                this.SetRegisterValue((ushort)LedPixelReg.NumPixels, LedPixelRegPack.NumPixels, value);
            }

        }

        /// <summary>
        /// (Optional) If the LED pixel strip is a matrix, specifies the number of columns. Otherwise, a square shape is assumed. Controllers which are sold with lights should default to the correct length
        /// and could not allow change. Increasing length at runtime leads to ineffective use of memory and may lead to controller reboot., _: #
        /// </summary>
        public uint NumColumns
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)LedPixelReg.NumColumns, LedPixelRegPack.NumColumns);
            }
            set
            {
                
                this.SetRegisterValue((ushort)LedPixelReg.NumColumns, LedPixelRegPack.NumColumns, value);
            }

        }

        /// <summary>
        /// Limit the power drawn by the light-strip (and controller)., _: mA
        /// </summary>
        public uint MaxPower
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)LedPixelReg.MaxPower, LedPixelRegPack.MaxPower);
            }
            set
            {
                
                this.SetRegisterValue((ushort)LedPixelReg.MaxPower, LedPixelRegPack.MaxPower, value);
            }

        }

        /// <summary>
        /// The maximum supported number of pixels.
        /// All writes to `num_pixels` are clamped to `max_pixels`., _: #
        /// </summary>
        public uint MaxPixels
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)LedPixelReg.MaxPixels, LedPixelRegPack.MaxPixels);
            }
        }

        /// <summary>
        /// How many times to repeat the program passed in `run` command.
        /// Should be set before the `run` command.
        /// Setting to `0` means to repeat forever., _: #
        /// </summary>
        public uint NumRepeats
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)LedPixelReg.NumRepeats, LedPixelRegPack.NumRepeats);
            }
            set
            {
                
                this.SetRegisterValue((ushort)LedPixelReg.NumRepeats, LedPixelRegPack.NumRepeats, value);
            }

        }

        /// <summary>
        /// (Optional) Specifies the shape of the light strip., 
        /// </summary>
        public LedPixelVariant Variant
        {
            get
            {
                return (LedPixelVariant)this.GetRegisterValue((ushort)LedPixelReg.Variant, LedPixelRegPack.Variant);
            }
        }


        
        /// <summary>
        /// Run the given light "program". See service description for details.
        /// </summary>
        public void Run(byte[] program)
        {
            this.SendCmdPacked((ushort)LedPixelCmd.Run, LedPixelCmdPack.Run, new object[] { program });
        }

    }
}