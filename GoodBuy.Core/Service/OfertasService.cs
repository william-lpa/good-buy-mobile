using GoodBuy.Model;
using GoodBuy.Models;
using GoodBuy.Models.Logical;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IGenericRepository<Tipo> tipoRepository;
        private readonly IGenericRepository<Produto> produtoRepository;
        private readonly IGenericRepository<MonitoramentoOferta> monitoramentoOfertaRepository;
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
            tipoRepository = syncronizedAccessService.TipoRepository;
            categoriaRepository = syncronizedAccessService.CategoriaRepository;
            monitoramentoOfertaRepository = syncronizedAccessService.MonitoramentoOfertaRepository;
        }
        internal float CalculateConfiabilidade(Oferta oferta)
        {
            if (oferta.Likes == 0)
                return 0;
            return (oferta.Likes * BASE_PERCENTUAL) / oferta.Avaliacoes;
        }
        public async Task SyncronizeBaseDeOfertasAsync()
        {
            var t1 = Task.Run(async () => await (tipoRepository.PullUpdatesAsync()));
            var t2 = Task.Run(async () => await (unidadeMedidaRepository.PullUpdatesAsync()));
            var t3 = Task.Run(async () => await (carteiraProdutoRepository.PullUpdatesAsync()));
            await (produtoRepository.PullUpdatesAsync());
            await (marcaRepository.PullUpdatesAsync());
            var t4 = Task.Run(async () => await (estabelecimentoRepository.PullUpdatesAsync()));
            await (carteiraProdutoRepository.PullUpdatesAsync());
            await (ofertaRepository.PullUpdatesAsync());
            await (historicoOfertaRepository.PullUpdatesAsync());
            Task.WaitAll(new Task[] { t1, t2, t3, t4 });
        }

        internal async Task LoadAutoCompleteAsync()
        {
            var Produtos = new List<string>((await produtoRepository.GetEntitiesAsync()).Select(x => x.Nome).Distinct());
            OnCollectionLoaded(nameof(Produtos), Produtos);
            var Tipos = new List<string>((await tipoRepository.GetEntitiesAsync()).Select(x => x.Nome).Distinct());
            OnCollectionLoaded(nameof(Tipos), Tipos);
            var UnidadesMedidas = new List<string>((await unidadeMedidaRepository.GetEntitiesAsync()).Select(x => x.Nome).Distinct());
            OnCollectionLoaded(nameof(UnidadesMedidas), UnidadesMedidas);
            var Marcas = new List<string>((await marcaRepository.GetEntitiesAsync()).Select(x => x.Nome).Distinct());
            OnCollectionLoaded(nameof(Marcas), Marcas);
            var Categorias = new List<string>((await categoriaRepository.GetEntitiesAsync()).Select(x => x.Nome).Distinct());
            OnCollectionLoaded(nameof(Categorias), Categorias);
            var Estabelecimentos = new List<string>((await estabelecimentoRepository.GetEntitiesAsync()).Select(x => x.Nome).Distinct());
            OnCollectionLoaded(nameof(Estabelecimentos), Estabelecimentos);
        }

        internal async Task<float> ApplyLikeAsync(string idOFerta)
        {
            var oferta = await ofertaRepository.GetByIdAsync(idOFerta);
            oferta.Avaliacoes++;
            oferta.Likes++;
            await ofertaRepository.UpdateEntityAsync(oferta);
            return CalculateConfiabilidade(oferta);
        }

        internal async Task<float> ApplyDislikeAsync(string idOFerta)
        {
            var oferta = await ofertaRepository.GetByIdAsync(idOFerta);
            oferta.Avaliacoes++;
            await ofertaRepository.UpdateEntityAsync(oferta);
            return CalculateConfiabilidade(oferta);
        }

        internal async Task<float> RevertConfiabilidadeAsync(string idOFerta, bool revertLike)
        {
            var oferta = await ofertaRepository.GetByIdAsync(idOFerta);
            oferta.Avaliacoes--;
            if (revertLike)
                oferta.Likes--;
            await ofertaRepository.UpdateEntityAsync(oferta);
            return CalculateConfiabilidade(oferta);
        }

        public async Task<IList<OfertaDto>> ObterUltimasTresOfertasFromServerAsync()
        {
            if (azureService.CurrentUser?.User != null)
                return null;
            List<Oferta> ofertas = new List<Oferta>();
            var idCarteiraProduto = new List<CarteiraProduto>();
            var idMarcas = new List<Marca>();
            var idProdutos = new List<Produto>();
            var idTipo = new List<Tipo>();
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

                    Task.Run(async () => idTipo = await azureService.Client.GetTable<Tipo>().ToListAsync()).Wait();
                    Task.Run(async () => idUnidadeMedida = await azureService.Client.GetTable<UnidadeMedida>().ToListAsync()).Wait();
                    Task.Run(async () => idEstabelecimento = await azureService.Client.GetTable<Estabelecimento>().ToListAsync()).Wait();

                    var retorno = (ofertas.Select(oferta =>
                    {
                        var carteiraProduto = idCarteiraProduto.Find(x => x.Id == oferta.IdCarteiraProduto);
                        var produto = idProdutos.Find(x => x.Id == carteiraProduto.IdProduto);

                        return new OfertaDto(idEstabelecimento.Find(x => x.Id == oferta.IdEstabelecimento), oferta, produto,
                                             idUnidadeMedida.Find(x => x.Id == produto.IdUnidadeMedida), idTipo.Find(x => x.Id == produto.IdTipo),
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

        public async Task<IList<OfertaDto>> ObterOfertasAsync(int count = 500)
        {
            var ofertas = (await ofertaRepository.GetEntitiesAsync(0, count));
            await carteiraProdutoRepository.GetEntitiesAsync();
            await produtoRepository.GetEntitiesAsync();
            await marcaRepository.GetEntitiesAsync();

            var retorno = await Task.WhenAll(ofertas.Select(async oferta =>
            {
                var idCarteiraProduto = await carteiraProdutoRepository.GetByIdAsync(oferta.IdCarteiraProduto);
                var produto = await produtoRepository.GetByIdAsync(idCarteiraProduto.IdProduto);

                return new OfertaDto(await estabelecimentoRepository.GetByIdAsync(oferta.IdEstabelecimento), oferta, produto, await unidadeMedidaRepository.GetByIdAsync(produto.IdUnidadeMedida),
                                    await tipoRepository.GetByIdAsync(produto.IdTipo), await marcaRepository.GetByIdAsync(idCarteiraProduto.IdMarca), this);
            }));

            return retorno.OrderByDescending(x => x.UpdatedAt).ToArray();
        }

        public async Task<Oferta> ObterOfertaCompletaAsync(string id)
        {
            var oferta = await ofertaRepository.GetByIdAsync(id);
            oferta.Estabelecimento = await estabelecimentoRepository.GetByIdAsync(oferta.IdEstabelecimento);
            var tasks = new Task[2];
            tasks[0] = Task.Run(async () => oferta.Estabelecimento = await estabelecimentoRepository.GetByIdAsync(oferta.IdEstabelecimento));
            tasks[1] = Task.Run(async () => oferta.CarteiraProduto = await carteiraProdutoRepository.GetByIdAsync(oferta.IdCarteiraProduto));
            Task.WaitAll(tasks);
            tasks = new Task[2];
            tasks[0] = Task.Run(async () => oferta.CarteiraProduto.Marca = await marcaRepository.GetByIdAsync(oferta.CarteiraProduto.IdMarca));
            tasks[1] = Task.Run(async () => oferta.CarteiraProduto.Produto = await produtoRepository.GetByIdAsync(oferta.CarteiraProduto.IdProduto));
            Task.WaitAll(tasks);
            tasks = new Task[3];
            tasks[0] = Task.Run(async () => oferta.CarteiraProduto.Produto.Categoria = await categoriaRepository.GetByIdAsync(oferta.CarteiraProduto.Produto.IdCategoria));
            tasks[1] = Task.Run(async () => oferta.CarteiraProduto.Produto.Tipo = await tipoRepository.GetByIdAsync(oferta.CarteiraProduto.Produto.IdTipo));
            tasks[2] = Task.Run(async () => oferta.CarteiraProduto.Produto.UnidadeMedida = await unidadeMedidaRepository.GetByIdAsync(oferta.CarteiraProduto.Produto.IdUnidadeMedida));
            Task.WaitAll(tasks);
            return oferta;
        }

        public async Task<IEnumerable<HistoricoOferta>> CarregarHistoricoDeOFertaAsync(string idOferta, DateTime fromDate, DateTime toDate)
        {
            return (await historicoOfertaRepository.SyncTableModel.Where(x => x.IdOferta == idOferta && x.CreatedAt >= fromDate && x.CreatedAt <= toDate)
                .OrderByDescending(x => x.CreatedAt).Take(7)
                .ToListAsync())
                    .OrderBy(x => x.UpdatedAt);
        }

        public async void CriarNovaOfertaAsync(Oferta oferta)
        {
            try
            {
                var idEstabelecimento = await CreateOrRetrieveEntityByNameAsync(oferta?.Estabelecimento);
                var idTipo = await CreateOrRetrieveEntityByNameAsync(oferta?.CarteiraProduto?.Produto?.Tipo);
                var idUnidadeMedida = await CreateOrRetrieveEntityByNameAsync(oferta?.CarteiraProduto?.Produto?.UnidadeMedida);
                var idCategoria = await CreateOrRetrieveEntityByNameAsync(oferta?.CarteiraProduto?.Produto?.Categoria);
                var idMarca = await CreateOrRetrieveEntityByNameAsync(oferta?.CarteiraProduto?.Marca) ?? (await marcaRepository.SyncTableModel.Where(x => x.Nome.ToLower() == "sem marca").ToListAsync()).First().Id;
                await azureService.Client.SyncContext.PushAsync(new System.Threading.CancellationToken());
                var idProduto = await CriarProdutoAsync(oferta.CarteiraProduto.Produto.Nome, idUnidadeMedida, idTipo, idCategoria, oferta.CarteiraProduto.Produto.QuantidadeMensuravel);
                await azureService.Client.SyncContext.PushAsync(new System.Threading.CancellationToken());
                var idCarteira = await CriarCarteiraProdutoAsync(idProduto, idMarca);
                await azureService.Client.SyncContext.PushAsync(new System.Threading.CancellationToken());
                await CriarOfertaAsync(idCarteira, idEstabelecimento, oferta.PrecoAtual);
                await SyncronizeBaseDeOfertasAsync();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        private async Task<string> CreateOrRetrieveEntityByNameAsync<TEntity>(TEntity entity) where TEntity : class, IEntity, IName, new()
        {
            var repository = await GetEntityService<TEntity>();
            if (entity?.Nome == null)
                return null;
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome.ToLower() == entity.Nome.ToLower()).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntityAsync(new TEntity() { Nome = entity.Nome });
        }

        private async Task<string> CriarProdutoAsync(string nome, string idUnidadeMedida, string idTipo, string idCategoria, float quantidadeMensuravel)
        {
            var repository = await GetEntityService<Produto>();
            return ((await repository?.SyncTableModel.Where(x => x.IdTipo == idTipo && x.IdUnidadeMedida == idUnidadeMedida && nome.ToLower() == x.Nome.ToLower() && x.IdCategoria == idCategoria).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                       await repository?.CreateEntityAsync(new Produto(nome, idTipo, idUnidadeMedida, idCategoria, quantidadeMensuravel));
        }

        private async Task<string> CriarCarteiraProdutoAsync(string idProduto, string idMarca)
        {

            if (idMarca == null || idProduto == null)
                return null;

            var repository = await GetEntityService<CarteiraProduto>();

            return ((await repository?.SyncTableModel.Where(x => x.IdMarca == idMarca && x.IdProduto == idProduto).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                       await repository?.CreateEntityAsync(new CarteiraProduto(idProduto, idMarca));
        }
        private async Task<GenericRepository<T>> GetEntityService<T>(bool downloadUpdates = false) where T : class, IEntity, new()
        {
            var repository = new GenericRepository<T>(azureService);
            if (downloadUpdates)
                await repository.PullUpdatesAsync();
            return repository;
        }

        internal async void CriarNovoAlertaAsync(decimal valor)
        {
            var alerta = new MonitoramentoOferta(ViewModels.OfertasTabDetailPageViewModel.Oferta.Id, azureService.CurrentUser.User.Id);
            alerta.PrecoAlvo = valor;
            await monitoramentoOfertaRepository.CreateEntityAsync(alerta);
            await monitoramentoOfertaRepository.PullUpdatesAsync();
        }
        internal async Task<MonitoramentoOferta> TryLoadAlertaAsync(string idOferta)
        {
            await monitoramentoOfertaRepository.PullUpdatesAsync();
            return (await monitoramentoOfertaRepository.SyncTableModel.Where(x => x.IdOferta == idOferta && x.IdUser == azureService.CurrentUser.User.Id).ToListAsync()).FirstOrDefault();
        }
        internal async void RemoverAlertaAsync(MonitoramentoOferta alerta)
        {
            var data = DateTime.Today;
            alerta.Delete = true;
            await monitoramentoOfertaRepository.UpdateEntityAsync(alerta); //notify ser and delete on server            
            await monitoramentoOfertaRepository.SyncDataBaseAsync(data);
            await monitoramentoOfertaRepository.DeleteEntityAsync(alerta);
            await monitoramentoOfertaRepository.SyncDataBaseAsync(data);
        }

        internal async void AtualizarAlertaAsync(MonitoramentoOferta alerta, decimal valor)
        {
            alerta.PrecoAlvo = valor;
            await monitoramentoOfertaRepository.UpdateEntityAsync(alerta); //notify ser and delete on server            
            await monitoramentoOfertaRepository.SyncDataBaseAsync(DateTime.Today);
        }

        private async Task CriarOfertaAsync(string idCarteira, string idEstabelecimento, decimal preco)
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
                        await ApplyLikeAsync(ofertaExistente.Id);
                    }
                    else
                    {
                        var historico = CriarHistoricoAPartirDeOfertaAsync(ofertaExistente, preco);
                    }
                }
                else
                    await ofertaRepository?.CreateEntityAsync(new Oferta(idEstabelecimento, idCarteira, preco));
            }
        }

        private async Task<string> CriarHistoricoAPartirDeOfertaAsync(Oferta ofertaAtual, decimal preco)
        {
            var historico = await historicoOfertaRepository.CreateEntityAsync(new HistoricoOferta(ofertaAtual));
            ofertaAtual.PrecoAtual = preco;
            ofertaAtual.Likes = 1;
            ofertaAtual.Avaliacoes = 1;
            await ofertaRepository?.UpdateEntityAsync(ofertaAtual);
            return historico;
        }

    }
}

