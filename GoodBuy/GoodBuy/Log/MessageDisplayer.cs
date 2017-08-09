using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodBuy.Log
{
    class MessageDisplayer
    {
        private static MessageDisplayer _instance;
        public static MessageDisplayer Instance => _instance ?? (_instance = new MessageDisplayer());

        private MessageDisplayer() { }

        private NavigationPage Navigation => (Application.Current.MainPage as NavigationPage);

        public async Task ShowMessage(string title, string message, string button)
        {
            await Navigation.DisplayAlert(title, message, button);
        }

        public async Task<bool> ShowAsk(string title, string message, string buttonYes, string buttonNo)
        {
            return await Navigation.DisplayAlert(title, message, buttonYes, buttonNo);
        }

        public async Task<string> ShowOptions(string title, string cancel, string destruction, string[] buttons)
        {
            return await Navigation.DisplayActionSheet(title, cancel, destruction, buttons);
        }
    }
}
