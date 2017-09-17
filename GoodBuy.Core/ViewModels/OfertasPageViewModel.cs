using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using GoodBuy.Models.Logical;
using GoodBuy.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;

namespace GoodBuy.ViewModels
{
    public class OfertasPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly OfertasService ofertasService;
        public ObservableCollection<OfertaDto> Ofertas { get; set; }
        public OfertasPageViewModel(AzureService service, OfertasService ofertasService)
        {
            azureService = service;
            this.ofertasService = ofertasService;
            Ofertas = new ObservableCollection<OfertaDto>();

            FillListView();
        }

        private async void FillListView()
        {
            var ofertas = await ofertasService.ObterOfertas();

            foreach (var oferta in ofertas)
            {
                Ofertas.Add(oferta);
            }

        }
    }
}

