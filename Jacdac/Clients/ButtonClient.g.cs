/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients {

    /// <summary>
    /// A push-button, which returns to inactive position when not operated anymore.
    /// Implements a client for the Button service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/button/" />
    public partial class ButtonClient : SensorClient
    {
        public ButtonClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.Button)
        {
        }

        /// <summary>
        /// Indicates the pressure state of the button, where `0` is open., _: /
        /// </summary>
        public float Pressure
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)ButtonReg.Pressure, ButtonRegPack.Pressure);
            }
        }

        /// <summary>
        /// (Optional) Indicates if the button provides analog `pressure` readings., 
        /// </summary>
        public bool Analog
        {
            get
            {
                return (bool)this.GetRegisterValue((ushort)ButtonReg.Analog, ButtonRegPack.Analog);
            }
        }

        /// <summary>
        /// Emitted when button goes from inactive to active.
        /// </summary>
        public event ClientEventHandler Down
        {
            add
            {
                this.AddEvent((ushort)ButtonEvent.Down, value);
            }
            remove
            {
                this.RemoveEvent((ushort)ButtonEvent.Down, value);
            }
        }

        /// <summary>
        /// Emitted when button goes from active to inactive. The 'time' parameter
        /// records the amount of time between the down and up events.
        /// </summary>
        public event ClientEventHandler Up
        {
            add
            {
                this.AddEvent((ushort)ButtonEvent.Up, value);
            }
            remove
            {
                this.RemoveEvent((ushort)ButtonEvent.Up, value);
            }
        }

        /// <summary>
        /// Emitted when the press time is greater than 500ms, and then at least every 500ms
        /// as long as the button remains pressed. The 'time' parameter records the the amount of time
        /// that the button has been held (since the down event).
        /// </summary>
        public event ClientEventHandler Hold
        {
            add
            {
                this.AddEvent((ushort)ButtonEvent.Hold, value);
            }
            remove
            {
                this.RemoveEvent((ushort)ButtonEvent.Hold, value);
            }
        }


    }
}