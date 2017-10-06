using System.Collections.Generic;

namespace GoodBuy.ViewModels
{
    public class CompartilharOfertasPageViewModel : BaseViewModel
    {
        public static string IdSharingOferta { get; set; }
        public CompartilharOfertasPageViewModel()
        {
        }

        protected override void Init(Dictionary<string, string> parameters = null)
        {
            base.Init(parameters);
            if (parameters != null && parameters.ContainsKey("ID"))
            {
                IdSharingOferta = parameters["ID"];
            }
        }
    }
}
