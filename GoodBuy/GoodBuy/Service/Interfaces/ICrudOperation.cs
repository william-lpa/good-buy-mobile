using GoodBuy.Model;
using System.Threading.Tasks;

namespace GoodBuy.Service.Interfaces
{
    public interface ICrudOperation<TModel> where TModel : IEntity
    {
        Task<string> CreateEntity(TModel entidade);
        Task DeleteEntity(TModel entidade);
        Task UpdateEntity(TModel entidade);
    }
}