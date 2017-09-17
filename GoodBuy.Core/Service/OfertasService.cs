using GoodBuy.Model;
using GoodBuy.Models;
using GoodBuy.Models.Logical;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBuy.Service
{
    public class OfertasService
    {
        private readonly AzureService azureService;
        private readonly IGenericRepository<Oferta> ofertaRepository;
        private readonly IGenericRepository<CarteiraProduto> carteiraProdutoRepository;
        private readonly IGenericRepository<Estabelecimento> estabelecimentoRepository;
        private readonly IGenericRepository<Marca> marcaRepository;
        private readonly IGenericRepository<UnidadeMedida> unidadeMedidaRepository;
        private readonly IGenericRepository<Sabor> saborRepository;
        private readonly IGenericRepository<Produto> produtoRepository;
        private const int BASE_PERCENTUAL = 100;
        public OfertasService(AzureService azureService, SyncronizedAccessService syncronizedAccessService)
        {
            this.azureService = azureService;
            ofertaRepository = syncronizedAccessService.OfertaRepository;
            carteiraProdutoRepository = syncronizedAccessService.CarteiraProdutoRepository;
            estabelecimentoRepository = syncronizedAccessService.EstabelecimentoRepository;
            marcaRepository = syncronizedAccessService.MarcaRepository;
            unidadeMedidaRepository = syncronizedAccessService.UnidadeMedidaRepository;
            produtoRepository = syncronizedAccessService.ProdutoRepository;
            saborRepository = syncronizedAccessService.SaborRepository;
        }

        internal float CalculateConfiabilidade(Oferta oferta)
        {
            if (oferta.Likes == 0)
                return 0;
            return (oferta.Likes * BASE_PERCENTUAL) / oferta.Avaliacoes;
        }



        internal async Task<float> ApplyLike(string idOFerta)
        {
            var oferta = await ofertaRepository.GetById(idOFerta);
            oferta.Avaliacoes++;
            oferta.Likes++;
            await ofertaRepository.UpdateEntity(oferta);
            return CalculateConfiabilidade(oferta);
        }

        internal async Task<float> ApplyDislike(string idOFerta)
        {
            var oferta = await ofertaRepository.GetById(idOFerta);
            oferta.Avaliacoes++;
            await ofertaRepository.UpdateEntity(oferta);
            return CalculateConfiabilidade(oferta);
        }

        internal async Task<float> RevertConfiabilidade(string idOFerta, bool revertLike)
        {
            var oferta = await ofertaRepository.GetById(idOFerta);
            oferta.Avaliacoes--;
            if (revertLike)
                oferta.Likes--;
            await ofertaRepository.UpdateEntity(oferta);
            return CalculateConfiabilidade(oferta);
        }

        public async Task<IList<OfertaDto>> ObterUltimasTresOfertasFromServer()
        {
            List<Oferta> ofertas = new List<Oferta>();
            var idCarteiraProduto = new List<CarteiraProduto>();
            var idMarcas = new List<Marca>();
            var idProdutos = new List<Produto>();
            var idSabor = new List<Sabor>();
            var idUnidadeMedida = new List<UnidadeMedida>();
            var idEstabelecimento = new List<Estabelecimento>();

            try
            {
                ofertas = await azureService.Client.GetTable<Oferta>().OrderByDescending(x => x.UpdatedAt).Take(3).ToListAsync();
                Task.WaitAll(
                Task.Run(async () => idCarteiraProduto = await azureService.Client.GetTable<CarteiraProduto>().Where(x => x.Id == ofertas[0].IdCarteiraProduto || x.Id == ofertas[1].IdCarteiraProduto || x.Id == ofertas[2].IdCarteiraProduto).ToListAsync()),
                Task.Run(async () => idMarcas = await azureService.Client.GetTable<Marca>().ToListAsync()),
                Task.Run(async () => idProdutos = await azureService.Client.GetTable<Produto>().ToListAsync())
                );

                Task.Run(async () => idSabor = await azureService.Client.GetTable<Sabor>().ToListAsync()).Wait();
                Task.Run(async () => idUnidadeMedida = await azureService.Client.GetTable<UnidadeMedida>().ToListAsync()).Wait();
                Task.Run(async () => idEstabelecimento = await azureService.Client.GetTable<Estabelecimento>().ToListAsync()).Wait();

            }
            catch (Exception)
            {
            }

            var retorno = (ofertas.Select(oferta =>
            {
                var carteiraProduto = idCarteiraProduto.Find(x => x.Id == oferta.IdCarteiraProduto);
                var produto = idProdutos.Find(x => x.Id == carteiraProduto.IdProduto);

                return new OfertaDto(idEstabelecimento.Find(x => x.Id == oferta.IdEstabelecimento), oferta, produto,
                                     idUnidadeMedida.Find(x => x.Id == produto.IdUnidadeMedida), idSabor.Find(x => x.Id == produto.IdSabor),
                                     idMarcas.Find(x => x.Id == carteiraProduto.IdMarca), this);
            })).ToList();

            return retorno;
        }

        public async Task<IList<OfertaDto>> ObterOfertas(int count = 500)
        {
            var ofertas = (await ofertaRepository.GetEntities(0, count));
            await carteiraProdutoRepository.GetEntities();
            await produtoRepository.GetEntities();
            await marcaRepository.GetEntities();

            var retorno = await Task.WhenAll(ofertas.Select(async oferta =>
            {
                var idCarteiraProduto = await carteiraProdutoRepository.GetById(oferta.IdCarteiraProduto);
                var produto = await produtoRepository.GetById(idCarteiraProduto.IdProduto);

                return new OfertaDto(await estabelecimentoRepository.GetById(oferta.IdEstabelecimento), oferta, produto, await unidadeMedidaRepository.GetById(produto.IdUnidadeMedida),
                                    await saborRepository.GetById(produto.IdSabor), await marcaRepository.GetById(idCarteiraProduto.IdMarca), this);
            }));

            return retorno;
        }
    }
}

