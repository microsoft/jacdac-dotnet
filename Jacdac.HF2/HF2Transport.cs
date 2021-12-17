namespace Jacdac.Transports
{
    public abstract class HF2Transport : Jacdac.Transport
    {
        protected HF2Transport()
            : base()
        {

        }

        public abstract Task SendData(byte[] buffer);
    }

}
