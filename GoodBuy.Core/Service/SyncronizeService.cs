using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace GoodBuy.Service
{
    public class SyncronizedAccessService
    {
        public IGenericRepository<Oferta> OfertaRepository { get; }
        public IGenericRepository<HistoricoOferta> HistoricoOfertaRepository { get; }
        public IGenericRepository<CarteiraProduto> CarteiraProdutoRepository { get; }
        public IGenericRepository<Estabelecimento> EstabelecimentoRepository { get; }
        public IGenericRepository<Marca> MarcaRepository { get; }
        public IGenericRepository<UnidadeMedida> UnidadeMedidaRepository { get; }
        public IGenericRepository<Sabor> SaborRepository { get; }
        public IGenericRepository<Produto> ProdutoRepository { get; }
        public IGenericRepository<GrupoOferta> GrupoOfertaRepository { get; }
        public IGenericRepository<ParticipanteGrupo> ParticipanteGrupoRepository { get; }
        public IGenericRepository<User> UserRepository { get; }
        public IGenericRepository<Categoria> CategoriaRepository { get; set; }
        public IGenericRepository<MonitoramentoOferta> MonitoramentoOfertaRepository { get; set; }


        private readonly AzureService azureService;

        public SyncronizedAccessService(AzureService azureService)
        {
            this.azureService = azureService;
            OfertaRepository = new GenericRepository<Oferta>(azureService);
            CarteiraProdutoRepository = new GenericRepository<CarteiraProduto>(azureService);
            EstabelecimentoRepository = new GenericRepository<Estabelecimento>(azureService);
            MarcaRepository = new GenericRepository<Marca>(azureService);
            UnidadeMedidaRepository = new GenericRepository<UnidadeMedida>(azureService);
            ProdutoRepository = new GenericRepository<Produto>(azureService);
            SaborRepository = new GenericRepository<Sabor>(azureService);
            GrupoOfertaRepository = new GenericRepository<GrupoOferta>(azureService);
            HistoricoOfertaRepository = new GenericRepository<HistoricoOferta>(azureService);
            ParticipanteGrupoRepository = new GenericRepository<ParticipanteGrupo>(azureService);
            CategoriaRepository = new GenericRepository<Categoria>(azureService);
            UserRepository = new GenericRepository<User>(azureService);
            MonitoramentoOfertaRepository = new GenericRepository<MonitoramentoOferta>(azureService);
        }

        public async Task<bool> FirstUsageAsync() => (await OfertaRepository.GetEntitiesAsync(0, 1)).Count == 0;


        public async void SyncronizeFirstUseAsync()
        {
            await SyncronizeData();
        }
        public async void SyncronizeAsync(DateTime fromDate)
        {
            await SyncronizeData(fromDate);
        }

        private async Task SyncronizeData(DateTime? date = null)
        {
            await Task.WhenAll(new Task[]
           {
                     Task.Run(() => OfertaRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => CarteiraProdutoRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => ProdutoRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => SaborRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => MarcaRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => EstabelecimentoRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => UnidadeMedidaRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => GrupoOfertaRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => HistoricoOfertaRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => ParticipanteGrupoRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => UserRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => CategoriaRepository.SyncDataBaseAsync(date)),
                     Task.Run(() => MonitoramentoOfertaRepository.SyncDataBaseAsync(date)),
           });
        }
    }
}

