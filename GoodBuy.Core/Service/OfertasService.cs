﻿using GoodBuy.Model;
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
        private readonly IGenericRepository<Categoria> categoriaRepository;
        private readonly IGenericRepository<HistoricoOferta> historicoOfertaRepository;
        private const int BASE_PERCENTUAL = 100;
        public static event Action<string, List<string>> OnCollectionLoaded;

        public OfertasService(AzureService azureService, SyncronizedAccessService syncronizedAccessService)
        {
            this.azureService = azureService;
            ofertaRepository = syncronizedAccessService.OfertaRepository;
            carteiraProdutoRepository = syncronizedAccessService.CarteiraProdutoRepository;
            estabelecimentoRepository = syncronizedAccessService.EstabelecimentoRepository;
            marcaRepository = syncronizedAccessService.MarcaRepository;
            unidadeMedidaRepository = syncronizedAccessService.UnidadeMedidaRepository;
            produtoRepository = syncronizedAccessService.ProdutoRepository;
            historicoOfertaRepository = syncronizedAccessService.HistoricoOfertaRepository;
            saborRepository = syncronizedAccessService.SaborRepository;
            categoriaRepository = syncronizedAccessService.CategoriaRepository;
        }

        internal float CalculateConfiabilidade(Oferta oferta)
        {
            if (oferta.Likes == 0)
                return 0;
            return (oferta.Likes * BASE_PERCENTUAL) / oferta.Avaliacoes;
        }
        public async Task SyncronizeBaseDeOfertas()
        {
            var t1 = Task.Run(async () => await (saborRepository.PullUpdates()));
            var t2 = Task.Run(async () => await (unidadeMedidaRepository.PullUpdates()));
            var t3 = Task.Run(async () => await (carteiraProdutoRepository.PullUpdates()));
            await (produtoRepository.PullUpdates());
            await (marcaRepository.PullUpdates());
            var t4 = Task.Run(async () => await (estabelecimentoRepository.PullUpdates()));
            await (carteiraProdutoRepository.PullUpdates());
            await (ofertaRepository.PullUpdates());
            await (historicoOfertaRepository.PullUpdates());
            Task.WaitAll(new Task[] { t1, t2, t3, t4 });
        }

        internal async Task LoadAutoCompleteAsync()
        {
            var Produtos = new List<string>((await produtoRepository.GetEntities()).Select(x => x.Nome));
            OnCollectionLoaded(nameof(Produtos), Produtos);
            var Sabores = new List<string>((await saborRepository.GetEntities()).Select(x => x.Nome));
            OnCollectionLoaded(nameof(Sabores), Sabores);
            var UnidadesMedidas = new List<string>((await unidadeMedidaRepository.GetEntities()).Select(x => x.Nome));
            OnCollectionLoaded(nameof(UnidadesMedidas), UnidadesMedidas);
            var Marcas = new List<string>((await marcaRepository.GetEntities()).Select(x => x.Nome));
            OnCollectionLoaded(nameof(Marcas), Marcas);
            var Categorias = new List<string>((await categoriaRepository.GetEntities()).Select(x => x.Nome));
            OnCollectionLoaded(nameof(Categorias), Categorias);
            var Estabelecimentos = new List<string>((await estabelecimentoRepository.GetEntities()).Select(x => x.Nome));
            OnCollectionLoaded(nameof(Estabelecimentos), Estabelecimentos);
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
            if (azureService.CurrentUser?.User != null)
                return null;
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
                if (ofertas.Count > 0)
                {
                    Task.WaitAll(
                    Task.Run(async () => idCarteiraProduto = await azureService.Client.GetTable<CarteiraProduto>().Where(x => x.Id == ofertas[0].IdCarteiraProduto || x.Id == ofertas[1].IdCarteiraProduto || x.Id == ofertas[2].IdCarteiraProduto).ToListAsync()),
                    Task.Run(async () => idMarcas = await azureService.Client.GetTable<Marca>().ToListAsync()),
                    Task.Run(async () => idProdutos = await azureService.Client.GetTable<Produto>().ToListAsync())
                    );

                    Task.Run(async () => idSabor = await azureService.Client.GetTable<Sabor>().ToListAsync()).Wait();
                    Task.Run(async () => idUnidadeMedida = await azureService.Client.GetTable<UnidadeMedida>().ToListAsync()).Wait();
                    Task.Run(async () => idEstabelecimento = await azureService.Client.GetTable<Estabelecimento>().ToListAsync()).Wait();

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
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog("Não há ofertas até o momento");
            }
            return null;
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

        public async Task<Oferta> ObterOfertaCompleta(string id)
        {
            var oferta = await ofertaRepository.GetById(id);
            var tasks = new Task[7];
            await Task.WhenAll(
                tasks[0] = Task.Run(async () => oferta.Estabelecimento = await estabelecimentoRepository.GetById(oferta.IdEstabelecimento)),
                tasks[1] = Task.Run(async () => oferta.CarteiraProduto = await carteiraProdutoRepository.GetById(oferta.IdCarteiraProduto))
                ).ContinueWith((action) =>
               {
                   Task.WhenAll(
                       tasks[2] = Task.Run(async () => oferta.CarteiraProduto.Marca = await marcaRepository.GetById(oferta.CarteiraProduto.IdMarca)),
                       tasks[3] = Task.Run(async () => oferta.CarteiraProduto.Produto = await produtoRepository.GetById(oferta.CarteiraProduto.IdProduto))
                       ).ContinueWith((task) =>
                       {
                           Task.WhenAll(
                              tasks[4] = Task.Run(async () => oferta.CarteiraProduto.Produto.Categoria = await categoriaRepository.GetById(oferta.CarteiraProduto.Produto.IdCategoria)),
                              tasks[5] = Task.Run(async () => oferta.CarteiraProduto.Produto.Sabor = await saborRepository.GetById(oferta.CarteiraProduto.Produto.IdSabor)),
                              tasks[6] = Task.Run(async () => oferta.CarteiraProduto.Produto.UnidadeMedida = await unidadeMedidaRepository.GetById(oferta.CarteiraProduto.Produto.IdUnidadeMedida))
                             );
                       });
               });
            Task.WaitAll(tasks);
            return oferta;
        }
        public async void CriarNovaOferta(Oferta oferta)
        {
            var idEstabelecimento = await CreateOrRetrieveEntityByName(oferta?.Estabelecimento);
            await CreateOrRetrieveEntityByName(oferta?.CarteiraProduto?.Produto?.Sabor);
            await CreateOrRetrieveEntityByName(oferta?.CarteiraProduto?.Produto?.UnidadeMedida);
            await CreateOrRetrieveEntityByName(oferta?.CarteiraProduto?.Produto?.Categoria);
            var idMarca = await CreateOrRetrieveEntityByName(oferta?.CarteiraProduto?.Marca);
            await azureService.Client.SyncContext.PushAsync(new System.Threading.CancellationToken());
            var idProduto = await CreateOrRetrieveEntityByName(oferta?.CarteiraProduto?.Produto);
            await SyncronizeBaseDeOfertas();
            var idCarteira = await CriarCarteiraProduto(idProduto, idMarca);
            await SyncronizeBaseDeOfertas();
            await CriarOferta(idEstabelecimento, idCarteira, oferta.PrecoAtual);
            await SyncronizeBaseDeOfertas();
        }

        private async Task<string> CreateOrRetrieveEntityByName<TEntity>(TEntity entity) where TEntity : class, IEntity, IName, new()
        {
            var repository = await GetEntityService<TEntity>();
            if (entity?.Nome == null)
                return null;
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == entity.Nome).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new TEntity() { Nome = entity.Nome });
        }

        private async Task<string> CriarCarteiraProduto(string idProduto, string idMarca)
        {
            var repository = await GetEntityService<CarteiraProduto>();
            var id = ((await repository?.SyncTableModel.Where(x => x.IdMarca == idMarca && x.IdProduto == idProduto).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                       await repository?.CreateEntity(new CarteiraProduto(idProduto, idMarca));
            return id;
        }
        private async Task<GenericRepository<T>> GetEntityService<T>(bool downloadUpdates = false) where T : class, IEntity, new()
        {
            var repository = new GenericRepository<T>(azureService);
            if (downloadUpdates)
                await repository.PullUpdates();
            return repository;
        }

        private async Task CriarOferta(string idCarteira, string idEstabelecimento, decimal preco)
        {
            if (idCarteira == null || preco < 1)
                return;

            if (idEstabelecimento != null)
            {
                var ofertaExistente = (await ofertaRepository.SyncTableModel
                                    .Where(x => x.IdCarteiraProduto == idCarteira && x.IdEstabelecimento == idEstabelecimento)
                                    .OrderByDescending(x => x.UpdatedAt)
                                    .Select(x => x).ToEnumerableAsync()).FirstOrDefault();
                if (ofertaExistente != null)
                {
                    if (ofertaExistente.PrecoAtual == preco)
                    {
                        await ApplyLike(ofertaExistente.Id);
                    }
                    else
                    {
                        var historico = historicoOfertaRepository.CreateEntity(new HistoricoOferta(ofertaExistente));
                        ofertaExistente.PrecoAtual = preco;
                        await ofertaRepository?.UpdateEntity(ofertaExistente);
                    }
                }
                else
                    await ofertaRepository?.CreateEntity(new Oferta(idEstabelecimento, idCarteira, preco));
            }
        }

    }
}

