using System.Threading.Tasks;

namespace Jacdac.NET
{
    public interface IHF2Transport
    {
        Task SendData(byte[] buffer);
    }
}
