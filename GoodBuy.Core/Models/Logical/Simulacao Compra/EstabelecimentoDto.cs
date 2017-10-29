using GoodBuy.Models.Logical;
using System.Collections.Generic;

namespace GoodBuy.Models.Logical
{
    public class EstabelecimentoDto
    {
        public float TotalSimulacao { get; set; }
        public string NomeEstabelecimento { get; set; }
        public IEnumerable<ProdutoDto> ProdutosCompra { get; set; }

        public EstabelecimentoDto(string nome)
        {
            NomeEstabelecimento = nome;
        }
    }
}