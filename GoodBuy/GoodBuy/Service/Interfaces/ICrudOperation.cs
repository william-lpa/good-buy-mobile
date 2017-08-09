using GoodBuy.Model;
using System.Threading.Tasks;

namespace GoodBuy.Service.Interfaces
{
    public interface ICrudOperation
    {
        Task CreateEntity(IEntity entidade);
        Task DeleteEntity(IEntity entidade);
        Task UpdateEntity(IEntity entidade);        
    }
}