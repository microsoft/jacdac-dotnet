/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients 
{
    /// <summary>
    /// A switching relay.
     /// 
     /// The contacts should be labelled `NO` (normally open), `COM` (common), and `NC` (normally closed).
     /// When relay is energized it connects `NO` and `COM`.
     /// When relay is not energized it connects `NC` and `COM`.
     /// Some relays may be missing `NO` or `NC` contacts.
     /// When relay module is not powered, or is in bootloader mode, it is not energized (connects `NC` and `COM`).
    /// Implements a client for the Relay service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/relay/" />
    public partial class RelayClient : Client
    {
        public RelayClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.Relay)
        {
        }

        /// <summary>
        /// Reads the <c>active</c> register value.
        /// Indicates whether the relay circuit is currently energized or not., 
        /// </summary>
        public bool Active
        {
            get
            {
                return (bool)this.GetRegisterValueAsBool((ushort)RelayReg.Active, RelayRegPack.Active);
            }
            set
            {
                
                this.SetRegisterValue((ushort)RelayReg.Active, RelayRegPack.Active, value);
            }

        }

        /// <summary>
        /// Tries to read the <c>variant</c> register value.
        /// Describes the type of relay used., 
        /// </summary>
        bool TryGetVariant(out RelayVariant value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)RelayReg.Variant, RelayRegPack.Variant, out values)) 
            {
                value = (RelayVariant)values[0];
                return true;
            }
            else
            {
                value = default(RelayVariant);
                return false;
            }
        }

        /// <summary>
        /// Tries to read the <c>max_switching_current</c> register value.
        /// Maximum switching current for a resistive load., _: mA
        /// </summary>
        bool TryGetMaxSwitchingCurrent(out uint value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)RelayReg.MaxSwitchingCurrent, RelayRegPack.MaxSwitchingCurrent, out values)) 
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


    }
}