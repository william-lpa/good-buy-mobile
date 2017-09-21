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


        public void SyncronizeFirstUse()
        {
            Task.WaitAll(new Task[]
            {
                        Task.Run(() => OfertaRepository.SyncDataBase()),
                        Task.Run(() => CarteiraProdutoRepository.SyncDataBase()),
                        Task.Run(() => ProdutoRepository.SyncDataBase()),
                        Task.Run(() => SaborRepository.SyncDataBase()),
                        Task.Run(() => MarcaRepository.SyncDataBase()),
                        Task.Run(() => EstabelecimentoRepository.SyncDataBase()),
                        Task.Run(() => UnidadeMedidaRepository.SyncDataBase()),
                        Task.Run(() => GrupoOfertaRepository.SyncDataBase()),
                        Task.Run(() => ParticipanteGrupoRepository.SyncDataBase()),
                        Task.Run(() => UserRepository.SyncDataBase()),
            });
        }
    }
}
