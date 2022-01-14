/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients 
{
    /// <summary>
    /// A screen that displays characters, typically a LCD/OLED character screen.
    /// Implements a client for the Character Screen service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/characterscreen/" />
    public partial class CharacterScreenClient : Client
    {
        public CharacterScreenClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.CharacterScreen)
        {
        }

        /// <summary>
        /// Reads the <c>message</c> register value.
        /// Text to show. Use `\n` to break lines., 
        /// </summary>
        public string Message
        {
            get
            {
                return (string)this.GetRegisterValue((ushort)CharacterScreenReg.Message, CharacterScreenRegPack.Message);
            }
            set
            {
                
                this.SetRegisterValue((ushort)CharacterScreenReg.Message, CharacterScreenRegPack.Message, value);
            }

        }

        /// <summary>
        /// Tries to read the <c>brightness</c> register value.
        /// Brightness of the screen. `0` means off., _: /
        /// </summary>
        bool TryGetBrightness(out float value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)CharacterScreenReg.Brightness, CharacterScreenRegPack.Brightness, out values)) 
            {
                value = (float)values[0];
                return true;
            }
            else
            {
                value = default(float);
                return false;
            }
        }
        
        /// <summary>
        /// Sets the brightness value
        /// </summary>
        public void SetBrightness(float value)
        {
            this.SetRegisterValue((ushort)CharacterScreenReg.Brightness, CharacterScreenRegPack.Brightness, value);
        }


        /// <summary>
        /// Tries to read the <c>variant</c> register value.
        /// Describes the type of character LED screen., 
        /// </summary>
        bool TryGetVariant(out CharacterScreenVariant value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)CharacterScreenReg.Variant, CharacterScreenRegPack.Variant, out values)) 
            {
                value = (CharacterScreenVariant)values[0];
                return true;
            }
            else
            {
                value = default(CharacterScreenVariant);
                return false;
            }
        }

        /// <summary>
        /// Tries to read the <c>text_direction</c> register value.
        /// Specifies the RTL or LTR direction of the text., 
        /// </summary>
        bool TryGetTextDirection(out CharacterScreenTextDirection value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)CharacterScreenReg.TextDirection, CharacterScreenRegPack.TextDirection, out values)) 
            {
                value = (CharacterScreenTextDirection)values[0];
                return true;
            }
            else
            {
                value = default(CharacterScreenTextDirection);
                return false;
            }
        }
        
        /// <summary>
        /// Sets the text_direction value
        /// </summary>
        public void SetTextDirection(CharacterScreenTextDirection value)
        {
            this.SetRegisterValue((ushort)CharacterScreenReg.TextDirection, CharacterScreenRegPack.TextDirection, value);
        }


        /// <summary>
        /// Reads the <c>rows</c> register value.
        /// Gets the number of rows., _: #
        /// </summary>
        public uint Rows
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)CharacterScreenReg.Rows, CharacterScreenRegPack.Rows);
            }
        }

        /// <summary>
        /// Reads the <c>columns</c> register value.
        /// Gets the number of columns., _: #
        /// </summary>
        public uint Columns
        {
            get
            {
                return (uint)this.GetRegisterValue((ushort)CharacterScreenReg.Columns, CharacterScreenRegPack.Columns);
            }
        }


    }
}