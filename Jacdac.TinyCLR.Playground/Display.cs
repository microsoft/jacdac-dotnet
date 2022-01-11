using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;
using GHIElectronics.TinyCLR.Pins;
using Jacdac.TinyCLR.Playground.Properties;
using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Threading;

namespace Jacdac.Playground
{
    internal class Display
    {
        static ST7735Controller st7735;
        static int Width => 160;
        static int Height => 128;

        static string[] internalMsg = new string[12];
        static Font font;
        static Graphics gfx;
        static SolidBrush colorWhite = new SolidBrush(Color.White);
        public static void Enable()
        {
            var spiController = SpiController.FromName(FEZBit.SpiBus.Display);
            var gpioController = GpioController.GetDefault();

            st7735 = new ST7735Controller(
                spiController.GetDevice(ST7735Controller.GetConnectionSettings(SpiChipSelectType.Gpio, GpioController.GetDefault().OpenPin(FEZBit.GpioPin.DisplayChipselect))), // ChipSelect 
                gpioController.OpenPin(FEZBit.GpioPin.DisplayRs), // Pin RS
                gpioController.OpenPin(FEZBit.GpioPin.DisplayReset) // Pin RESET

            );

            var backlight = gpioController.OpenPin(FEZBit.GpioPin.Backlight); // back light

            backlight.Write(GpioPinValue.High);
            backlight.SetDriveMode(GpioPinDriveMode.Output);


            st7735.SetDataAccessControl(true, true, false, false); // rotate the screen
            st7735.SetDrawWindow(0, 0, Width, Height);
            st7735.Enable();

            Graphics.OnFlushEvent += (sender, data, x, y, width, heigh, originalWidth) =>
            {
                if (st7735 != null)
                    st7735.DrawBuffer(data);
            };

            gfx = Graphics.FromImage(new Bitmap(Width, Height));
            font = Resources.GetFont(Resources.FontResources.small);

            for (var i = 0; i < 12; i++)
                internalMsg[i] = "";
        }

        public static void WriteLine(string text)
        {
            Array.Copy(internalMsg, 1, internalMsg, 0, internalMsg.Length - 1);

            internalMsg[11] = text;

            if (gfx != null)
            {
                gfx.Clear();

                for (var i = 0; i < 12; i++)
                    gfx.DrawString(internalMsg[i], font, colorWhite, 1, i * 10);

                gfx.Flush();
            }


        }
    }
}
