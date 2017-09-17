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
        }

        public async Task<bool> FirstUsage() => (await OfertaRepository.GetEntities(0, 1)).Count == 0;


        public void SyncronizeFirstUse(DateTime? dateTimeOffset = null)
        {
            Task.WaitAll(new Task[]
            {
                        Task.Run(() => OfertaRepository.SyncDataBase(dateTimeOffset)),
                        Task.Run(() => CarteiraProdutoRepository.SyncDataBase(dateTimeOffset)),
                        Task.Run(() => ProdutoRepository.SyncDataBase(dateTimeOffset)),
                        Task.Run(() => SaborRepository.SyncDataBase(dateTimeOffset)),
                        Task.Run(() => MarcaRepository.SyncDataBase(dateTimeOffset)),
                        Task.Run(() => EstabelecimentoRepository.SyncDataBase(dateTimeOffset)),
                        Task.Run(() => UnidadeMedidaRepository.SyncDataBase(dateTimeOffset)),
            });
        }
    }
}
