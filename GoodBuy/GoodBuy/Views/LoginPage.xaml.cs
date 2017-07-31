using GoodBuy.Service;
using GoodBuy.ViewModels;
using System;
using Xamarin.Forms;

namespace GoodBuy
{
    public partial class LoginPage : ContentPage
    {
        private readonly AzureService azureService;
        public LoginPage()
        {
            azureService = new AzureService();
            InitializeComponent();
            BindingContext = new LoginPageViewModel();
        }
    }
}
