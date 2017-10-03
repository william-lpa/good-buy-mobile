using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoodBuy.Service
{
    public class SyncronizedAccessService
    {
        public IGenericRepository<Oferta> OfertaRepository { get; }
        public IGenericRepository<CarteiraProduto> CarteiraProdutoRepository { get; }
        public IGenericRepository<Estabelecimento> EstabelecimentoRepository { get; }
        public IGenericRepository<Marca> MarcaRepository { get; }
        public IGenericRepository<UnidadeMedida> UnidadeMedidaRepository { get; }
        public IGenericRepository<Sabor> SaborRepository { get; }
        public IGenericRepository<Produto> ProdutoRepository { get; }
        public IGenericRepository<GrupoOferta> GrupoOfertaRepository { get; }
        public IGenericRepository<ParticipanteGrupo> ParticipanteGrupoRepository { get; }
        public IGenericRepository<User> UserRepository { get; }
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
            ParticipanteGrupoRepository = new GenericRepository<ParticipanteGrupo>(azureService);
            UserRepository = new GenericRepository<User>(azureService);
        }

        public async Task<bool> FirstUsage() => (await OfertaRepository.GetEntities(0, 1)).Count == 0;


        public async void SyncronizeFirstUse()
        {
            await SyncronizeData();
        }
        public async void Syncronize(DateTime fromDate)
        {
            await SyncronizeData(fromDate);
        }

        private async Task SyncronizeData(DateTime? date = null)
        {
            await Task.WhenAll(new Task[]
           {
                     Task.Run(() => OfertaRepository.SyncDataBase(date)),
                     Task.Run(() => CarteiraProdutoRepository.SyncDataBase(date)),
                     Task.Run(() => ProdutoRepository.SyncDataBase(date)),
                     Task.Run(() => SaborRepository.SyncDataBase(date)),
                     Task.Run(() => MarcaRepository.SyncDataBase(date)),
                     Task.Run(() => EstabelecimentoRepository.SyncDataBase(date)),
                     Task.Run(() => UnidadeMedidaRepository.SyncDataBase(date)),
                     Task.Run(() => GrupoOfertaRepository.SyncDataBase(date)),
                     Task.Run(() => ParticipanteGrupoRepository.SyncDataBase(date)),
                     Task.Run(() => UserRepository.SyncDataBase(date)),
           });
        }
    }
}
