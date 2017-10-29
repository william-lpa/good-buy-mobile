using GoodBuy.Models.Many_to_Many;

namespace GoodBuy.Models.Logical
{
    public class ProdutoDto
    {
        public ProdutoDto(Oferta oferta)
        {
            Valor = (float)oferta.PrecoAtual;
            ProdutoDescricao = oferta.CarteiraProduto.Produto.Nome + (oferta.CarteiraProduto.Produto.IdTipo != null ? " - " + oferta.CarteiraProduto.Produto.Tipo.Nome : "");
        }

        public string ProdutoDescricao { get; set; }
        public float Valor { get; set; }
        public string EstabelecimentoMaisBarato { get; set; }
        public float EstabelecimentoMaisBaratoValor { get; set; }
        public string EstabelecimentoMaisCaro { get; set; }
        public float EstabelecimentoMaisCaroValor { get; set; }
    }
}