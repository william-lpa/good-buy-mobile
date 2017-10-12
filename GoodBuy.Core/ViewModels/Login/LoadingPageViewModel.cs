using GoodBuy.Service;
using GoodBuy.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodBuy.ViewModels
{
    public enum Operation { Login, SyncInitalDataBase, }
    public class LoadingPageViewModel : BaseViewModel
    {
        private string operation;

        public string Operation
        {
            get { return operation; }
            set { SetProperty(ref operation, value); }
        }

        public LoadingPageViewModel(Operation operation, SyncronizedAccessService syncronizeService)
        {
            switch (operation)
            {
                case ViewModels.Operation.Login:
                    Operation = "Carregando";
                    break;
                case ViewModels.Operation.SyncInitalDataBase:
                    Operation = "Sincronizando dados para primeiro uso";
                    break;
                default:
                    break;
            }
        }
    }
}
