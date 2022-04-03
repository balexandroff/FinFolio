using System.Threading.Tasks;

namespace FinFolio.Services.Interfaces
{
    public interface IStockService: IService
    {
        Task<string> LoadRemoteStockInfo(string ticker);
    }
}
