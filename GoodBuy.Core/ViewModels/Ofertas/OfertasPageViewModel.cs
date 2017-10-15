using GoodBuy.Service;
using System.Collections.ObjectModel;
using GoodBuy.Models.Logical;
using Xamarin.Forms;
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
            OfertaDetailCommand = new Command<OfertaDto>(ExecuteDetailOfertaAsync);

            FillListViewAsync();
        }

        private async void ExecuteDetailOfertaAsync(OfertaDto oferta)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", oferta.idOFerta);
            await PushAsync<OfertasTabDetailPageViewModel>(false, parameters);
        }

        private async void FillListViewAsync()
        {
            var ofertas = await ofertasService.ObterOfertasAsync();

            foreach (var oferta in ofertas)
            {
                Ofertas.Add(oferta);
            }

        }
    }
}

