using GoodBuy.Service;
using GoodBuy.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodBuy.Core.ViewModels
{
    class LoadingPageViewModel : BaseViewModel
    {
        public LoadingPageViewModel(AzureService service)
        {
            while (service.CurrentUser != null)
            {
                Task.Delay(200).Wait();
            }
            new Action((async () => await PopAsync())).Invoke();
        }
    }
}
