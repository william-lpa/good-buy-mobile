using GoodBuy.ViewModels;
using System.Collections.Generic;

namespace GoodBuy.ViewModels
{
    public class OfertasTabDetailPageViewModel : BaseViewModel
    {
        public static string IdOferta { get; private set; }

        protected override void Init(Dictionary<string, string> parameters = null)
        {
            base.Init(parameters);
            if (parameters != null && parameters.ContainsKey("ID"))
            {
                IdOferta = parameters["ID"];
            }
        }

    }
}
