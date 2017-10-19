using GoodBuy.Service;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using GoodBuy.Log;
using Microcharts;
using SkiaSharp;
using System.Linq;
using GoodBuy.Models.Many_to_Many;
using System.Threading.Tasks;

namespace GoodBuy.ViewModels
{
    public class HistoricosOfertaPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly OfertasService ofertasService;
        private decimal monitorarOfertaValor;
        private DateTime fromDate;
        private DateTime untilDate;
        private LineChart chart;
        public DateTime FromDate
        {
            get { return fromDate; }
            set
            {
                if (value > UntilDate)
                {
                    new Action(async () => await MessageDisplayer.Instance.ShowMessageAsync("Filtro incorreto", "Data inicial não pode ser maior que a data final", "OK")).Invoke();                    
                }
                SetProperty(ref fromDate, value);
                GetChart();
            }
        }
        public DateTime UntilDate
        {
            get { return untilDate; }
            set
            {
                if (value < FromDate)
                {
                    new Action(async () => await MessageDisplayer.Instance.ShowMessageAsync("Filtro incorreto", "Data final não pode ser menor que data inicial", "OK")).Invoke();
                    SetProperty(ref untilDate, untilDate);
                }
                SetProperty(ref untilDate, value);
                GetChart();
            }
        }


        public decimal MonitorarOfertaValor
        {
            get { return monitorarOfertaValor; }
            set { SetProperty(ref monitorarOfertaValor, Math.Round(value, 2)); }
        }

        public decimal MinimumPrice => OfertasTabDetailPageViewModel.Oferta != null ? Math.Round((OfertasTabDetailPageViewModel.Oferta.PrecoAtual) * 0.01M, 2) : 0;
        public decimal MaximumPrice => OfertasTabDetailPageViewModel.Oferta != null ? Math.Round(OfertasTabDetailPageViewModel.Oferta.PrecoAtual, 2) : 100;

        private MonitoramentoOferta alerta;


        public Command PrimaryAction { get; }
        public bool ExistsAlert { get; set; }
        public string ExistsAlertDescription => ExistsAlert ? "Monitorando Oferta" : "Selecione para monitorar ofertas";


        public HistoricosOfertaPageViewModel(AzureService azureService, OfertasService ofertasService)
        {
            this.azureService = azureService;
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
            var min = MinimumPrice;
            var max = MaximumPrice;
            fromDate = DateTime.Today.AddDays(-7);
            untilDate = DateTime.Now;
            GetChart();
            OnPropertyChanged(nameof(HistoricoOfertaChart));
            OnPropertyChanged(nameof(MinimumPrice));
            OnPropertyChanged(nameof(MaximumPrice));
            OnPropertyChanged(nameof(FromDate));
            OnPropertyChanged(nameof(UntilDate));
        }
        public Chart HistoricoOfertaChart => chart ?? null;

        public async void GetChart()
        {
            chart = new LineChart()
            {
                LineSize = 20,
                PointSize = 35,
                LineAreaAlpha = 10,
                PointAreaAlpha = 5,
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                LabelTextSize = 20,
                BackgroundColor = SKColors.Transparent
            };
            chart.Entries = await GetEntriesAsync();
            OnPropertyChanged(nameof(HistoricoOfertaChart));
        }

        private async Task<IEnumerable<Microcharts.Entry>> GetEntriesAsync()
        {
            var currentTime = DateTime.Now;
            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
            if (OfertasTabDetailPageViewModel.Oferta != null)
            {
                var index = 1;
                var historico = await ofertasService.CarregarHistoricoDeOFertaAsync(OfertasTabDetailPageViewModel.Oferta.Id, FromDate, UntilDate);

                historico.Select(x =>
                {
                    if (index == 1 || index == Math.Round(historico.Count() / 2M, 0, MidpointRounding.ToEven) || index == historico.Count())
                        entries.Add(new Microcharts.Entry((float)x.Preco)
                        {
                            Label = ObterDescriptionDateTime(x.UpdatedAt), // "January",
                            ValueLabel = $"R$ {x.Preco}",//"102",
                            Color = SKColor.Parse("#4e6cab"),
                            TextColor = SKColor.Parse("#4e6cab"),
                        });
                    else
                        entries.Add(new Microcharts.Entry((float)x.Preco)
                        {
                            Color = SKColor.Parse("#4e6cab"),
                            TextColor = SKColor.Parse("#4e6cab"),
                            Label = null,
                        });
                    index++;
                    return x;
                }).ToArray();
                return entries;
            }
            return null;
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
                OnPropertyChanged(nameof(ExistsAlertDescription));
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
