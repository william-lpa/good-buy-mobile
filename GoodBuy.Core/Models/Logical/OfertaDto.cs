using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodBuy.Models.Logical
{
    public class OfertaDto: INotifyPropertyChanged
    {
        private readonly string IdOFerta;
        private float confiabilidade;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly OfertasService ofertasService;
        public string DescricaoOferta { get; }
        public decimal ValorOferta { get; }
        public float Confiabilidade
        {
            get { return confiabilidade; }
            set
            {
                confiabilidade = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConfiabilidadeNegativa));
            }
        }
        public float ConfiabilidadeNegativa => 100 - Confiabilidade;
        public string Picture { get; }
        public DateTime CreatedAt { get; set; }
        public string Estabelecimento { get; set; }
        public Command AumentarConfiabilidadeCommand { get; }
        public Command DiminuirConfiabilidadeCommand { get; }
        public bool? LikePerformed { get; set; } = null;



        public OfertaDto(Estabelecimento estabelecimento, Oferta oferta, Produto produto, UnidadeMedida unidade, Sabor sabor, Marca marca, OfertasService ofertasService)
        {
            IdOFerta = oferta.Id;
            this.ofertasService = ofertasService;
            CreatedAt = oferta.CreatedAt;
            ValorOferta = oferta.PrecoAtual;
            Confiabilidade = ofertasService.CalculateConfiabilidade(oferta);
            Estabelecimento = estabelecimento.Nome;
            DescricaoOferta = $"{produto.Nome} - {sabor.Nome}, {marca.Nome}, {produto.QuantidadeMensuravel} {unidade.Nome}";
            AumentarConfiabilidadeCommand = new Command(ExecuteAplicarLike);
            DiminuirConfiabilidadeCommand = new Command(ExecuteAplicarDislike);
        }


        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task NeedsReversal()
        {
            if (LikePerformed != null) //ja foi realizado anteriormente
            {
                Confiabilidade = await ofertasService.RevertConfiabilidade(IdOFerta, LikePerformed.Value);
            }
        }
        private async void ExecuteAplicarDislike()
        {
            await NeedsReversal();
            LikePerformed = false;
            Confiabilidade = await ofertasService.ApplyDislike(IdOFerta);
        }

        private async void ExecuteAplicarLike()
        {
            await NeedsReversal();
            LikePerformed = true;
            Confiabilidade = await ofertasService.ApplyLike(IdOFerta);
        }
    }
}
