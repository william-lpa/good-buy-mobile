using GoodBuy.Service;
using System.Collections.ObjectModel;
using GoodBuy.Models.Logical;
using Xamarin.Forms;
using System;
using System.Collections.Generic;

namespace GoodBuy.ViewModels
{
    public class OfertasPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly OfertasService ofertasService;
        public ObservableCollection<OfertaDto> Ofertas { get; set; }
        public Command OfertaDetailCommand { get; set; }

        public OfertasPageViewModel(AzureService service, OfertasService ofertasService)
        {
            azureService = service;
            this.ofertasService = ofertasService;
            Ofertas = new ObservableCollection<OfertaDto>();
            OfertaDetailCommand = new Command<OfertaDto>(ExecuteDetailOferta);

            FillListView();
        }

        private async void ExecuteDetailOferta(OfertaDto oferta)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", oferta.idOFerta);
            await PushAsync<OfertasTabDetailPageViewModel>(false, parameters);
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

