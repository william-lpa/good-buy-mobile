using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodBuy.Service.Interfaces
{
    public interface IGenericRepository<TModel> : ICrudOperation<TModel> where TModel : IEntity
    {
        ConcurrentDictionary<string, TModel> Cache { get; }
        IMobileServiceClient Client { get; }
        IMobileServiceSyncTable<TModel> SyncTableModel { get; }
        Task SyncDataBaseAsync(DateTime? createdOrChangedAfter = null);
        Task PullUpdatesAsync();
        Task<List<TModel>> GetEntitiesAsync(int currentPage = 0, int pageSize = 200, DateTime? createdOrChangedAfter = null);
        Task<TModel> GetByIdAsync(string id);
        Task<IEnumerable<TModel>> GetByIdsAsync(string[] id);
    }
}
