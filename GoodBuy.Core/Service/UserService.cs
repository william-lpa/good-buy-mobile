using GoodBuy.Models;
using GoodBuy.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;

namespace GoodBuy.Service
{
    public class UserService
    {
        private readonly IGenericRepository<User> userRepository;
        private readonly AzureService azureService;
        public bool FirstUsage { get; set; }
        public UserService(AzureService azureService)//, SyncronizedAccessService syncronizedAccessService)
        {
            this.azureService = azureService;
            //FirstUsage = syncronizedAccessService.FirstUsage().Result;
            userRepository = new GenericRepository<User>(azureService);
        }

        public async Task<IEnumerable<User>> LocalizarUsuariosPesquisadosAsync(string searchTerm)
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

        internal async void LogarUsuarioAsync(User user)
        {
            var existingUser = await userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                await userRepository.CreateEntityAsync(user);
            else
            {
                await userRepository.UpdateEntityAsync(existingUser);
                using (var scope = App.Container.BeginLifetimeScope())
                    scope.Resolve<SyncronizedAccessService>().SyncronizeAsync(existingUser.UpdatedAt);
            }
        }
    }
}
