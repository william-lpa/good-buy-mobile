using System.Collections.Generic;
using GoodBuy.ViewModels;

namespace GoodBuy.ViewModels
{
    public class MonitorarOfertaPageViewModel : BaseViewModel
    {
        public MonitorarOfertaPageViewModel()
        {

        }

        protected override void Init(Dictionary<string, string> parameters = null)
        {
            if (parameters != null && parameters.ContainsKey("ID"))
            {
                var idOferta = parameters["ID"];
            }
        }
    }
}
