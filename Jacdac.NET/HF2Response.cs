using System;
using System.Linq;

namespace Jacdac.Transports
{
    internal struct HF2Response
    {
        public enum HF2CommandResponseStatus
        {
            Success = 0,
            InvalidCommand = 1,
            ExecutionError = 2,
            Event = 128
        }

        public short Tag { get; private set; }

        public HF2CommandResponseStatus Status { get; private set; }

        public byte StatusValue { get; private set; }

        public byte StatusInfo { get; private set; }

        public byte[] Payload { get; private set; }

        public bool IsComplete { get; private set; }

        public static HF2Response Parse(byte[] data)
        {
            if (data.Length == 0)
                throw new ArgumentException("No data supplied");

            return new HF2Response
            {
                Tag = (short)(data[0] | (data[1] << 8)),
                Status = (HF2CommandResponseStatus)data[2],
                StatusValue = data[2],
                StatusInfo = data[3],
                Payload = data.Skip(4).ToArray(),
                IsComplete = false
            };
        }

        public void AppendPayload(byte[] additionalPayload)
        {
            if (IsComplete)
                throw new InvalidOperationException("Response already completed");

            Payload = Payload.Concat(additionalPayload).ToArray();
        }

        public void Complete()
        {
            IsComplete = true;
        }

        public override string ToString()
        {
            return $"[CommandResponse] Tag: {Tag}, Status: {Status.ToString()}, Payload: {Payload.Length} bytes, Complete: {IsComplete}";
        }
    }
}
