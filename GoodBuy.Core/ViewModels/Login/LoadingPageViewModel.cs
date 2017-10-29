using GoodBuy.Service;

namespace GoodBuy.ViewModels
{
    public enum Operation { Login, SyncInitalDataBase, SimulatingList }
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
                case ViewModels.Operation.SimulatingList:
                    Operation = "Simulando compras";
                    break;
                default:
                    break;
            }
        }
    }
}
