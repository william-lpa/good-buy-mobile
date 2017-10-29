using System;

namespace GoodBuy.Log
{
    public class Log
    {
        private static Log _instance;
        public static Log Instance => _instance ?? (_instance = new Log());

        private Log() { }

        private async void ShowLogAsync(string text)
        {
            //System.Diagnostics.Debug.WriteLine(text);
            try
            {
                await MessageDisplayer.Instance.ShowMessageAsync("Error!", text, "OK");
            }
            catch (Exception err)
            {
                
            }
        }

        public void AddLog(string text)
        {
            ShowLogAsync(text);
        }

        public void AddLog(Exception e)
        {
            ShowLogAsync(e.Message);
        }
    }
}
