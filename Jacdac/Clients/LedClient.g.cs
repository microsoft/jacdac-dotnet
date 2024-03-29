/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients 
{
    /// <summary>
    /// A controller for displays of individually controlled RGB LEDs.
     /// 
     /// For 64 or less LEDs, the service should support the pack the pixels in the pixels register.
     /// Beyond this size, the register should return an empty payload as the amount of data exceeds
     /// the size of a packet. Typically services that use more than 64 LEDs
     /// will run on the same MCU and will maintain the pixels buffer internally.
    /// Implements a client for the LED service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/led/" />
    public partial class LedClient : Client
    {
        public LedClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.Led)
        {
        }

        /// <summary>
        /// Reads the <c>pixels</c> register value.
        /// A buffer of 24bit RGB color entries for each LED, in R, G, B order.
        /// When writing, if the buffer is too short, the remaining pixels are set to `#000000`;
        /// If the buffer is too long, the write may be ignored, or the additional pixels may be ignored.
        /// If the number of pixels is greater than `max_pixels_length`, the read should return an empty payload., 
        /// </summary>
        public byte[] Pixels
        {
            get
            {
                return (byte[])this.GetRegisterValue((ushort)LedReg.Pixels, LedRegPack.Pixels);
            }
            set
            {
                
                this.SetRegisterValue((ushort)LedReg.Pixels, LedRegPack.Pixels, value);
            }

        }

        /// <summary>
        /// Reads the <c>brightness</c> register value.
        /// Set the luminosity of the strip.
        /// At `0` the power to the strip is completely shut down., _: /
        /// </summary>
        public float Brightness
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)LedReg.Brightness, LedRegPack.Brightness);
            }
            set
            {
                
                this.SetRegisterValue((ushort)LedReg.Brightness, LedRegPack.Brightness, value);
            }

        }

        /// <summary>
        /// Reads the <c>actual_brightness</c> register value.
        /// This is the luminosity actually applied to the strip.
        /// May be lower than `brightness` if power-limited by the `max_power` register.
        /// It will rise slowly (few seconds) back to `brightness` is limits are no longer required., _: /
        /// </summary>
        public float ActualBrightness
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)LedReg.ActualBrightness, LedRegPack.ActualBrightness);
            }
        }

        /// <summary>
        /// Reads the <c>num_pixels</c> register value.
        /// Specifies the number of pixels in the strip., _: #
        /// </summary>
        public uint NumPixels
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)LedReg.NumPixels, LedRegPack.NumPixels);
            }
        }

        /// <summary>
        /// Tries to read the <c>num_columns</c> register value.
        /// If the LED pixel strip is a matrix, specifies the number of columns., _: #
        /// </summary>
        bool TryGetNumColumns(out uint value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)LedReg.NumColumns, LedRegPack.NumColumns, out values)) 
            {
                value = (uint)values[0];
                return true;
            }
            else
            {
                value = default(uint);
                return false;
            }
        }

        /// <summary>
        /// Tries to read the <c>max_power</c> register value.
        /// Limit the power drawn by the light-strip (and controller)., _: mA
        /// </summary>
        bool TryGetMaxPower(out uint value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)LedReg.MaxPower, LedRegPack.MaxPower, out values)) 
            {
                value = (uint)values[0];
                return true;
            }
            else
            {
                value = default(uint);
                return false;
            }
        }
        
        /// <summary>
        /// Sets the max_power value
        /// </summary>
        public void SetMaxPower(uint value)
        {
            this.SetRegisterValue((ushort)LedReg.MaxPower, LedRegPack.MaxPower, value);
        }


        /// <summary>
        /// Tries to read the <c>leds_per_pixel</c> register value.
        /// If known, specifies the number of LEDs in parallel on this device.
        /// The actual number of LEDs is `num_pixels * leds_per_pixel`., _: #
        /// </summary>
        bool TryGetLedsPerPixel(out uint value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)LedReg.LedsPerPixel, LedRegPack.LedsPerPixel, out values)) 
            {
                value = (uint)values[0];
                return true;
            }
            else
            {
                value = default(uint);
                return false;
            }
        }

        /// <summary>
        /// Tries to read the <c>wave_length</c> register value.
        /// If monochrome LED, specifies the wave length of the LED.
        /// Register is missing for RGB LEDs., _: nm
        /// </summary>
        bool TryGetWaveLength(out uint value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)LedReg.WaveLength, LedRegPack.WaveLength, out values)) 
            {
                value = (uint)values[0];
                return true;
            }
            else
            {
                value = default(uint);
                return false;
            }
        }

        /// <summary>
        /// Tries to read the <c>luminous_intensity</c> register value.
        /// The luminous intensity of all the LEDs, at full brightness, in micro candella., _: mcd
        /// </summary>
        bool TryGetLuminousIntensity(out uint value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)LedReg.LuminousIntensity, LedRegPack.LuminousIntensity, out values)) 
            {
                value = (uint)values[0];
                return true;
            }
            else
            {
                value = default(uint);
                return false;
            }
        }

        /// <summary>
        /// Tries to read the <c>variant</c> register value.
        /// Specifies the shape of the light strip., 
        /// </summary>
        bool TryGetVariant(out LedVariant value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)LedReg.Variant, LedRegPack.Variant, out values)) 
            {
                value = (LedVariant)values[0];
                return true;
            }
            else
            {
                value = default(LedVariant);
                return false;
            }
        }


    }
}