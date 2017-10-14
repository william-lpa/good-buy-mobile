using GoodBuy.Service;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using GoodBuy.Models;
using GoodBuy.Log;

namespace GoodBuy.ViewModels
{
    public class HistoricosOfertaPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly OfertasService ofertasService;
        private decimal monitorarOfertaValor;
        private MonitoramentoOferta alerta;
        public Command PrimaryAction { get; }
        public bool ExistsAlert { get; set; }

        public decimal MonitorarOfertaValor
        {
            get { return monitorarOfertaValor; }
            set { SetProperty(ref monitorarOfertaValor, value); }
        }
        public HistoricosOfertaPageViewModel(AzureService azureService, OfertasService ofertasService)
        {
            this.ofertasService = ofertasService;
            PrimaryAction = new Command(ExecuteCriarOferta);
            Init();
        }
        protected override async void Init(Dictionary<string, string> parameters = null)
        {
            while (OfertasTabDetailPageViewModel.Oferta == null) await System.Threading.Tasks.Task.Delay(55);
            alerta = await ofertasService.TryLoadAlerta(OfertasTabDetailPageViewModel.Oferta.Id);
            MonitorarOfertaValor = Math.Round(OfertasTabDetailPageViewModel.Oferta.PrecoAtual * 0.9M, 2);
            Adapter();
        }

        private void Adapter()
        {
            if (alerta != null)
            {
                ExistsAlert = true;
                OnPropertyChanged(nameof(ExistsAlert));
                MonitorarOfertaValor = alerta.PrecoAlvo;
            }
            else
                ExistsAlert = false;
        }

        private async void ExecuteCriarOferta()
        {
            var mensagem = string.Empty;
            Action action = null;
            if (alerta == null)
            {
                mensagem = "Você tem certeza de quer criar um alerta para monitorar o valor desta oferta?";
                action = () => ofertasService.CriarNovoAlerta(MonitorarOfertaValor);
            }
            else if (ExistsAlert && MonitorarOfertaValor != alerta.PrecoAlvo)
            {
                mensagem = "Você tem certeza de quer alterar o valor do alerta criado anteriormente?";
                action = () => ofertasService.AtualizarAlerta(alerta, MonitorarOfertaValor);
            }
            else if (!ExistsAlert)
            {
                mensagem = "Você tem certeza de quer excluir o alerta  de preço criado para esta oferta?";
                action = () => ofertasService.RemoverAlerta(alerta);
            }

            if (action != null && await MessageDisplayer.Instance.ShowAsk("Monitorar oferta", mensagem, "Sim", "Não"))
            {
                action.Invoke();
                await PopAsync<OfertasPageViewModel>();
            }
        }
    }
}
