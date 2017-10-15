using GoodBuy.Service;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using GoodBuy.Log;
using Microcharts;
using SkiaSharp;
using System.Linq;
using GoodBuy.Models.Many_to_Many;

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
            PrimaryAction = new Command(ExecuteCriarOfertaAsync);
            Init();
        }
        protected override async void Init(Dictionary<string, string> parameters = null)
        {
            while (OfertasTabDetailPageViewModel.Oferta == null) await System.Threading.Tasks.Task.Delay(55);
            alerta = await ofertasService.TryLoadAlertaAsync(OfertasTabDetailPageViewModel.Oferta.Id);
            MonitorarOfertaValor = Math.Round(OfertasTabDetailPageViewModel.Oferta.PrecoAtual * 0.9M, 2);
            Adapter();
            var chart = HistoricoOfertaChart;
            OnPropertyChanged(nameof(HistoricoOfertaChart));
        }
        public Chart HistoricoOfertaChart => OfertasTabDetailPageViewModel.Oferta != null ? new LineChart()
        {
            Entries = GetEntries(),
            LineSize = 20,
            PointSize = 35,
            LineAreaAlpha = 10,
            PointAreaAlpha = 5,
            LineMode = LineMode.Straight,
            PointMode = PointMode.Circle,
            LabelTextSize = 20,
            BackgroundColor = SKColors.Transparent
        } : null;

        private IEnumerable<Microcharts.Entry> GetEntries()
        {
            var currentTime = DateTime.Now;

            var entries = OfertasTabDetailPageViewModel.Oferta?.OfertasAnteriores?
                .Select(x =>
                {
                    return new Microcharts.Entry((float)x.Preco)
                    {
                        Label = ObterDescriptionDateTime(x.UpdatedAt), // "January",
                        ValueLabel = $"R$ {x.Preco}",//"102",
                        Color = SKColor.Parse("#4e6cab"),
                        TextColor = SKColor.Parse("#4e6cab"),
                    };
                });
            return entries;

            string ObterDescriptionDateTime(DateTime date)
            {
                var difference = (currentTime - date);
                if (difference.TotalDays < 1)
                {
                    if (difference.TotalHours < 1)
                    {
                        var minutos = Math.Round(difference.TotalMinutes, 0);
                        var minutoString = minutos > 1 ? "minutos" : "minuto";
                        return $"Há {minutos} {minutoString}";
                    }
                    else
                    {
                        var horas = Math.Round(difference.TotalHours, 0);
                        var horasString = horas > 1 ? "horas" : "hora";
                        return $"Há {horas} {horasString}";
                    }
                }
                else
                {
                    var dias = Math.Round(difference.TotalDays, 0);
                    var diasString = dias > 1 ? "dias" : "dia";
                    return $"Há {dias} {diasString}";
                }
            }
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

        private async void ExecuteCriarOfertaAsync()
        {
            var mensagem = string.Empty;
            Action action = null;
            if (alerta == null)
            {
                mensagem = "Você tem certeza de quer criar um alerta para monitorar o valor desta oferta?";
                action = () => ofertasService.CriarNovoAlertaAsync(MonitorarOfertaValor);
            }
            else if (ExistsAlert && MonitorarOfertaValor != alerta.PrecoAlvo)
            {
                mensagem = "Você tem certeza de quer alterar o valor do alerta criado anteriormente?";
                action = () => ofertasService.AtualizarAlertaAsync(alerta, MonitorarOfertaValor);
            }
            else if (!ExistsAlert)
            {
                mensagem = "Você tem certeza de quer excluir o alerta  de preço criado para esta oferta?";
                action = () => ofertasService.RemoverAlertaAsync(alerta);
            }

            if (action != null && await MessageDisplayer.Instance.ShowAskAsync("Monitorar oferta", mensagem, "Sim", "Não"))
            {
                action.Invoke();
                await PopAsync<OfertasPageViewModel>();
            }
        }
    }
}
