using GoodBuy.Model;
using System.Threading.Tasks;

namespace GoodBuy.Service.Interfaces
{
    public interface ICrudOperation<TModel> where TModel : IEntity
    {
        Task<string> CreateEntityAsync(TModel entidade, bool forceSync = false);
        Task DeleteEntityAsync(TModel entidade);
        Task UpdateEntityAsync(TModel entidade);
    }
}