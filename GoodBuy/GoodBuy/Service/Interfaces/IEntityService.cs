using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodBuy.Service.Interfaces
{
    public interface IEntityService : ICrudOperation
    {
        IMobileServiceClient Client { get; }
        IMobileServiceSyncTable<IEntity> SyncTableModel { get; }
        Task SyncDataBase();
        Task<IList<IEntity>> GetEntites(int currentPage = 0, int pageSize = 200);
        Task<IEntity> GetById(string id);
        Task<IEnumerable<IEntity>> GetByIds(string[] id);
    }
}
