using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodBuy.ViewModels
{
    public class OfertasTabDetailPageViewModel : BaseViewModel
    {
        private readonly OfertasService ofertasService;
        public static Oferta Oferta { get; set; }

        public OfertasTabDetailPageViewModel(OfertasService ofertasService)
        {
            this.ofertasService = ofertasService;
        }

        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            base.Init(parameters);
            if (parameters != null && parameters.ContainsKey("ID"))
            {
                await LoadOferta(parameters["ID"]);
            }
        }

        private async Task LoadOferta(string idOFerta) => Oferta = await ofertasService.ObterOfertaCompleta(idOFerta);

    }
}
