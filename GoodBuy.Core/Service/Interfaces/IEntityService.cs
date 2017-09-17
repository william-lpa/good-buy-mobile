using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoodBuy.Service.Interfaces
{
    public interface IGenericRepository<TModel> : ICrudOperation<TModel> where TModel : IEntity
    {
        IDictionary<string, TModel> Cache { get; }
        IMobileServiceClient Client { get; }
        IMobileServiceSyncTable<TModel> SyncTableModel { get; }
        Task SyncDataBase(DateTime? createdOrChangedAfter = null);
        Task PullUpdates();
        Task<List<TModel>> GetEntities(int currentPage = 0, int pageSize = 200, DateTime? createdOrChangedAfter = null);
        Task<TModel> GetById(string id);
        Task<IEnumerable<TModel>> GetByIds(string[] id);
    }
}
