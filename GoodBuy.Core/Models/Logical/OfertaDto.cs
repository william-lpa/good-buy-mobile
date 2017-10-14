using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using GoodBuy.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        public DateTime CreatedAt { get; set; }
        public string Estabelecimento { get; set; }
        public Command AumentarConfiabilidadeCommand { get; }
        public Command DiminuirConfiabilidadeCommand { get; }
        public Command ShareOfertaCommand { get; }
        public Command OfertaDetailCommand { get; }
        public bool? LikePerformed { get; set; } = null;

        public OfertaDto(Estabelecimento estabelecimento, Oferta oferta, Produto produto, UnidadeMedida unidade, Sabor sabor, Marca marca, OfertasService ofertasService)
        {
            idOFerta = oferta.Id;
            this.ofertasService = ofertasService;
            CreatedAt = oferta.CreatedAt;
            ValorOferta = oferta.PrecoAtual;
            Confiabilidade = ofertasService.CalculateConfiabilidade(oferta);
            Estabelecimento = estabelecimento.Nome;
            DescricaoOferta = $"{produto.Nome} - {sabor?.Nome}, {marca.Nome}, {produto.QuantidadeMensuravel} {unidade.Nome}";
            AumentarConfiabilidadeCommand = new Command(ExecuteAplicarLike);
            DiminuirConfiabilidadeCommand = new Command(ExecuteAplicarDislike);
            ShareOfertaCommand = new Command(ExecuteCompartilharOferta);
            OfertaDetailCommand = new Command(ExecuteMonitorarOferta);
        }

        private async void ExecuteCompartilharOferta()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", idOFerta);
            await PushAsync<CompartilharOfertasPageViewModel>(false, parameters);
        }
        public async void ExecuteMonitorarOferta()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", idOFerta);
            await PushAsync<OfertasTabDetailPageViewModel>(false, parameters);
        }

        private async Task NeedsReversal()
        {
            if (LikePerformed != null) //ja foi realizado anteriormente
            {
                Confiabilidade = await ofertasService.RevertConfiabilidade(idOFerta, LikePerformed.Value);
            }
        }
        private async void ExecuteAplicarDislike()
        {
            await NeedsReversal();
            LikePerformed = false;
            Confiabilidade = await ofertasService.ApplyDislike(idOFerta);
        }

        private async void ExecuteAplicarLike()
        {
            await NeedsReversal();
            LikePerformed = true;
            Confiabilidade = await ofertasService.ApplyLike(idOFerta);
        }
    }
}
