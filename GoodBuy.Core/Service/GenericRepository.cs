using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoodBuy.Model;
using GoodBuy.Service.Interfaces;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;
using System.Collections.Concurrent;

namespace GoodBuy.Service
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class, IEntity
    {
        public IMobileServiceSyncTable<TModel> SyncTableModel { get; }
        public IMobileServiceClient Client { get; }
        public ConcurrentDictionary<string, TModel> Cache { get; }
        private const int MINUTES_TO_REFRESH = 10;
        private DateTime LastRefresh;
        public static object pushing = new object();

        public GenericRepository(AzureService azureService)
        {
            Cache = new ConcurrentDictionary<string, TModel>();
            Client = azureService.Client;
            SyncTableModel = azureService.GetTable<TModel>();
            LastRefresh = DateTime.Now;
        }
        public async Task<string> CreateEntityAsync(TModel entidade, bool forceSync = false)
        {
            try
            {
                entidade.Id = entidade.Id ?? Guid.NewGuid().ToString();
                await SyncTableModel.InsertAsync(entidade);
                Cache.AddOrUpdate(entidade.Id, entidade, (key, value) => value);
                await NeedsRefreshAsync();
                if (forceSync)
                    await SyncDataBaseAsync(DateTime.Now);
                return entidade.Id;
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return null;
            }
        }
        public async Task DeleteEntityAsync(TModel entidade)
        {
            try
            {
                await SyncTableModel.DeleteAsync(entidade);
                Cache.TryRemove(entidade.Id, out entidade);
                await NeedsRefreshAsync();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
        public async Task<TModel> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;

                await NeedsRefreshAsync();

                if (!Cache.ContainsKey(id))
                {
                    var entity = await (SyncTableModel.LookupAsync(id));
                    if (entity != null)
                        Cache.AddOrUpdate(entity.Id, entity, (key, value) => value);

                    return entity;
                }
                return Cache[id];
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return default(TModel);
            }
        }
        public async Task<List<TModel>> GetEntitiesAsync(int currentPage = 0, int pageSize = 200, DateTime? createdOrChangedAfter = null)
        {
            try
            {
                await NeedsRefreshAsync();

                List<TModel> entites = new List<TModel>();

                if (createdOrChangedAfter != null)
                    entites = await SyncTableModel.OrderByDescending(x => x.UpdatedAt).Where(x => x.UpdatedAt >= createdOrChangedAfter)
                                 .Skip(currentPage * pageSize).Take(pageSize).ToListAsync();
                else
                    entites = await SyncTableModel.OrderByDescending(x => x.UpdatedAt).Skip(currentPage * pageSize).Take(pageSize).ToListAsync();

                MergeDictionaries(Cache, entites.ToDictionary(key => key.Id, value => value));
                return entites;
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return null;
            }
        }

        public async Task PullUpdatesAsync()
        {
            try
            {
                await SyncTableModel.PullAsync(typeof(TModel).Name, SyncTableModel.CreateQuery());
                MergeDictionaries(Cache, (await SyncTableModel.ToListAsync()).ToDictionary((key) => key.Id, (value) => value));
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
        public async Task SyncDataBaseAsync(DateTime? createdOrChangedAfter = null)
        {
            try
            {
                lock (pushing)
                {
                    Task.Run(() => Client.SyncContext.PushAsync());
                }

                if (createdOrChangedAfter != null)
                    await SyncTableModel.PullAsync(typeof(TModel).Name, SyncTableModel.Where(x => x.UpdatedAt >= createdOrChangedAfter));
                else
                    await SyncTableModel.PullAsync(typeof(TModel).Name, SyncTableModel.CreateQuery());

                var list = (await SyncTableModel.ToListAsync());
                MergeDictionaries(Cache, (list.Count > 0 ? list.ToDictionary((key) => key.Id, (value) => value) : null));
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        public async Task UpdateEntityAsync(TModel entidade)
        {
            try
            {
                await NeedsRefreshAsync();
                await SyncTableModel.UpdateAsync(entidade);
                Cache[entidade.Id] = entidade;
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        private void MergeDictionaries(IDictionary<string, TModel> first, IDictionary<string, TModel> second)
        {
            if (second == null || first == null) return;
            foreach (var item in second)
                if (!first.ContainsKey(item.Key))
                    first.Add(item.Key, item.Value);
        }

        private async Task NeedsRefreshAsync()
        {
            var date = DateTime.Now;
            if ((date - LastRefresh).TotalMinutes > MINUTES_TO_REFRESH)
            {
                await SyncDataBaseAsync(LastRefresh);
                LastRefresh = date;
            }
        }

        public async Task<IEnumerable<TModel>> GetByIdsAsync(string[] id)
        {
            try
            {
                await NeedsRefreshAsync();
                return (await SyncTableModel.Where(x => id.Contains(x.Id)).ToEnumerableAsync());
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return null;
            }
        }


    }
}
