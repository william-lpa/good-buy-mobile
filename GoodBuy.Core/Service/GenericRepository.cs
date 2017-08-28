using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoodBuy.Model;
using GoodBuy.Service.Interfaces;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq.Expressions;
using System.Linq;
using GoodBuy.Models;

namespace GoodBuy.Service
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class, IEntity
    {
        public IMobileServiceSyncTable<TModel> SyncTableModel { get; }
        public IMobileServiceClient Client { get; }
        public static bool IsFetching { get; private set; }

        public GenericRepository(AzureService azureService)
        {
            Client = azureService.Client;
            SyncTableModel = azureService.GetTable<TModel>();
        }
        public async Task<string> CreateEntity(TModel entidade)
        {
            try
            {
                entidade.Id = Guid.NewGuid().ToString();
                await SyncTableModel.InsertAsync(entidade);
                return entidade.Id;
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return null;
            }
        }
        public async Task DeleteEntity(TModel entidade)
        {
            try
            {
                await SyncTableModel.DeleteAsync(entidade);
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
        public async Task<TModel> GetById(string id)
        {
            try
            {
                await SyncDataBase();
                return (await SyncTableModel.Where(x => x.Id == id).ToListAsync()).First();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return default(TModel);
            }
        }
        public async Task<IList<TModel>> GetEntities(int currentPage = 0, int pageSize = 200)
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
        public async Task PullUpdates()
        {
            try
            {
                await SyncTableModel.PullAsync(typeof(TModel).Name, SyncTableModel.CreateQuery());
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
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

        public async Task UpdateEntity(TModel entidade)
        {
            try
            {
                await SyncTableModel.UpdateAsync(entidade);
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        public async Task<IEnumerable<TModel>> GetByIds(string[] id)
        {
            try
            {
                await SyncDataBase();
                return (await SyncTableModel.Where(x => id.Contains(x.Id)).ToListAsync());
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return null;
            }
        }
    }
}
