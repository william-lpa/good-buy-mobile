using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBuy.Log
{
    public class Log
    {
        private static Log _instance;
        public static Log Instance => _instance ?? (_instance = new Log());

        private Log() { }

        private async void ShowLog(string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
            await MessageDisplayer.Instance.ShowMessage("Error!", text, "OK");
        }

        public void AddLog(string text)
        {
            ShowLog(text);
        }

        public void AddLog(Exception e)
        {
            ShowLog(e.Message);
        }
    }
}
