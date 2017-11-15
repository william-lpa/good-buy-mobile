using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using GoodBuy.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodBuy.Models.Logical
{
    public class OfertaDto : BaseViewModel
    {
        public readonly string idOFerta;
        private float confiabilidade;
        private readonly OfertasService ofertasService;
        public string DescricaoOferta { get; }
        public decimal ValorOferta { get; }
        public float Confiabilidade
        {
            get { return confiabilidade; }
            set
            {
                if (SetProperty(ref confiabilidade, value))
                    OnPropertyChanged(nameof(ConfiabilidadeNegativa));
            }
        }
        public float ConfiabilidadeNegativa => 100 - Confiabilidade;
        public string Picture { get; }
        public DateTime UpdatedAt { get; set; }
        public string Estabelecimento { get; set; }
        public Command AumentarConfiabilidadeCommand { get; }
        public Command DiminuirConfiabilidadeCommand { get; }
        public Command ShareOfertaCommand { get; }
        public Command OfertaDetailCommand { get; }
        public bool? LikePerformed { get; set; } = null;

        public OfertaDto(Estabelecimento estabelecimento, Oferta oferta, Produto produto, UnidadeMedida unidade, Tipo tipo, Marca marca, OfertasService ofertasService)
        {
            idOFerta = oferta.Id;
            this.ofertasService = ofertasService;
            UpdatedAt = oferta.UpdatedAt;
            ValorOferta = oferta.PrecoAtual;
            Confiabilidade = ofertasService.CalculateConfiabilidade(oferta);
            Estabelecimento = estabelecimento?.Nome;
            DescricaoOferta = $"{produto.Nome} - {tipo?.Nome}, {marca.Nome}, {produto.QuantidadeMensuravel} {unidade.Nome}";
            AumentarConfiabilidadeCommand = new Command(ExecuteAplicarLikeAsync);
            DiminuirConfiabilidadeCommand = new Command(ExecuteAplicarDislikeAsync);
            ShareOfertaCommand = new Command(ExecuteCompartilharOfertaAsync);
            OfertaDetailCommand = new Command(ExecuteMonitorarOfertaAsync);
        }

        private async void ExecuteCompartilharOfertaAsync()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", idOFerta);
            await PushAsync<CompartilharOfertasPageViewModel>(false, parameters);
        }
        public async void ExecuteMonitorarOfertaAsync()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", idOFerta);
            await PushAsync<OfertasTabDetailPageViewModel>(false, parameters);
        }

        private async Task NeedsReversionAsync()
        {
            if (LikePerformed != null) //ja foi realizado anteriormente
            {
                Confiabilidade = await ofertasService.RevertConfiabilidadeAsync(idOFerta, LikePerformed.Value);
            }
        }
        private async void ExecuteAplicarDislikeAsync()
        {
            await NeedsReversionAsync();
            LikePerformed = false;
            Confiabilidade = await ofertasService.ApplyDislikeAsync(idOFerta);
        }

        private async void ExecuteAplicarLikeAsync()
        {
            await NeedsReversionAsync();
            LikePerformed = true;
            Confiabilidade = await ofertasService.ApplyLikeAsync(idOFerta);
        }
    }
}
