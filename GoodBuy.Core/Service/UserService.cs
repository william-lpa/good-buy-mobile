using GoodBuy.Models;
using GoodBuy.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Autofac;

namespace GoodBuy.Service
{
    public class UserService
    {
        private readonly IGenericRepository<User> userRepository;
        private readonly AzureService azureService;

        public UserService(AzureService azureService, SyncronizedAccessService syncronizedAccessService)
        {
            this.azureService = azureService;
            userRepository = syncronizedAccessService.UserRepository;
            userRepository.SyncDataBase();
        }

        public async Task<IEnumerable<User>> LocalizarUsuariosPesquisados(string searchTerm)
        {
            try
            {
                searchTerm = searchTerm.ToLower();
                return await userRepository.SyncTableModel.Where(x => x.Id.Contains(searchTerm) || x.Email.Contains(searchTerm) || x.FullName.ToLower().Contains(searchTerm)).ToListAsync();
            }
            catch (System.Exception err)
            {
                Log.Log.Instance.AddLog(err);
                return null;
            }
        }

        internal async void LogarUsuario(User user)
        {
            var existingUser = await userRepository.GetById(user.Id);
            if (existingUser == null)
            {
                await userRepository.CreateEntity(user);
                using (var scope = App.Container.BeginLifetimeScope())
                    scope.Resolve<SyncronizedAccessService>().SyncronizeFirstUse();
            }
            else
            {
                using (var scope = App.Container.BeginLifetimeScope())
                    scope.Resolve<SyncronizedAccessService>().Syncronize(user.UpdatedAt);
                await userRepository.UpdateEntity(user);
            }
        }
    }
}
