/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients 
{
    /// <summary>
    /// A satellite-based navigation system like GPS, Gallileo, ...
    /// Implements a client for the Satellite Navigation System service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/satelittenavigationsystem/" />
    public partial class SatNavClient : SensorClient
    {
        public SatNavClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.SatNav)
        {
        }

        /// <summary>
        /// Reads the <c>position</c> register value.
        /// Reported coordinates, geometric altitude and time of position. Altitude accuracy is 0 if not available., timestamp: ms,latitude: lat,longitude: lon,accuracy: m,altitude: m,altitudeAccuracy: m
        /// </summary>
        public object[] /*(uint, float, float, float, float, float)*/ Position
        {
            get
            {
                return (object[] /*(uint, float, float, float, float, float)*/)this.GetRegisterValues((ushort)SatNavReg.Position, SatNavRegPack.Position);
            }
        }

        /// <summary>
        /// Reads the <c>enabled</c> register value.
        /// Enables or disables the GPS module, 
        /// </summary>
        public bool Enabled
        {
            get
            {
                return (bool)this.GetRegisterValueAsBool((ushort)SatNavReg.Enabled, SatNavRegPack.Enabled);
            }
            set
            {
                
                this.SetRegisterValue((ushort)SatNavReg.Enabled, SatNavRegPack.Enabled, value);
            }

        }

        /// <summary>
        /// The module is disabled or lost connection with satellites.
        /// </summary>
        public event ClientEventHandler Inactive
        {
            add
            {
                this.AddEvent((ushort)SatNavEvent.Inactive, value);
            }
            remove
            {
                this.RemoveEvent((ushort)SatNavEvent.Inactive, value);
            }
        }


    }
}