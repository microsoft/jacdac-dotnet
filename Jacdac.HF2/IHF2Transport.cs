namespace Jacdac.Transports
{
    public interface IHF2Transport
    {
        Task SendData(byte[] buffer);
    }
}
