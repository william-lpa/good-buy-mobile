using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodBuy.Service.Interfaces
{
    public interface IGenericRepository<TModel> : ICrudOperation<TModel> where TModel : IEntity
    {
        IMobileServiceClient Client { get; }
        IMobileServiceSyncTable<TModel> SyncTableModel { get; }
        Task SyncDataBase();
        Task PullUpdates();
        Task<IList<TModel>> GetEntities(int currentPage = 0, int pageSize = 200);
        Task<TModel> GetById(string id);
        Task<IEnumerable<TModel>> GetByIds(string[] id);
    }
}
