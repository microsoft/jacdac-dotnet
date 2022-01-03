using System.Threading.Tasks;

namespace Jacdac.Transports.Hf2
{
    public interface IHf2Transport
    {
        Task SendData(byte[] buffer);
    }
}
