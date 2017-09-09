using GoodBuy.Models.Many_to_Many;
using System;

namespace GoodBuy.Models.Logical
{
    public class OfertaDto
    {
        public string DescricaoProduto { get; }
        public decimal ValorOferta { get; }
        public float Confiabilidade { get; set; }
        public string Picture { get; }
        public DateTime CreateAt { get; set; }
        public string Estabelecimento { get; set; }
        public OfertaDto(Estabelecimento estabelecimento, Oferta oferta)
        {
            CreateAt = oferta.CreatedAt;
            ValorOferta = oferta.PrecoAtual;
            Estabelecimento = estabelecimento.Nome;
        }
    }
}
