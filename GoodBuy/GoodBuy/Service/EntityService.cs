using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoodBuy.Model;
using GoodBuy.Service.Interfaces;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq.Expressions;
using System.Linq;

namespace GoodBuy.Service
{
    public class EntityService<TModel> : IEntityService where TModel : IEntity
    {
        public IMobileServiceSyncTable<IEntity> SyncTableModel { get; }

        public IMobileServiceClient Client { get; }

        public EntityService(IMobileServiceClient client, IMobileServiceSyncTable<IEntity> syncTable)
        {
            SyncTableModel = syncTable;
            Client = client;
        }

        public async Task CreateEntity(IEntity entidade)
        {
            try
            {
                await SyncTableModel.InsertAsync(entidade);
                await Client.SyncContext.PushAsync();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
        public async Task DeleteEntity(IEntity entidade)
        {
            try
            {
                await SyncTableModel.DeleteAsync(entidade);
                await SyncDataBase();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
        public async Task<IEntity> GetById(string id)
        {
            try
            {
                await SyncDataBase();
                return (await SyncTableModel.Where(x => x.Id == id).ToListAsync()).First();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return null;
            }
        }
        public async Task<IList<IEntity>> GetEntites(int currentPage = 0, int pageSize = 15)
        {
            try
            {
                await SyncDataBase();
                return await SyncTableModel.Skip(currentPage * pageSize).Take(pageSize).ToListAsync();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return null;
            }
        }
        public async Task SyncDataBase()
        {
            try
            {
                await Client.SyncContext.PushAsync();
                await SyncTableModel.PullAsync(typeof(TModel).Name, SyncTableModel.CreateQuery());
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        public async Task UpdateEntity(IEntity entidade)
        {
            try
            {
                await SyncTableModel.UpdateAsync(entidade);
                await SyncDataBase();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
    }
}
