using Jacdac;
using System;

namespace Jacdac.Clients
{
    public partial class SevenSegmentDisplayClient : Client
    {
        /// <summary>
        /// Shows the number on the screen using the decimal dot if available.
        /// </summary>
        public void SetNumber(float value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shows the text on the screen. The client may decide to scroll the text if too long.
        /// </summary>
        public void SetText(string text)
        {
            throw new NotImplementedException();
        }
    }
}