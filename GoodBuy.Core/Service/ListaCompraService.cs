using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using GoodBuy.Models.Logical;
using Autofac;

namespace GoodBuy.Service
{
    public class ListaCompraService
    {
        private readonly IGenericRepository<ListaCompra> listaCompraRepository;
        private readonly IGenericRepository<ProdutoListaCompra> produtoListaCompraRepository;
        private readonly IGenericRepository<ParticipanteLista> participantesRepository;
        private readonly IGenericRepository<User> userRepository;
        private readonly AzureService azureService;
        private readonly OfertasService ofertasService;

        public ListaCompraService(AzureService azureService, OfertasService ofertasService, SyncronizedAccessService syncronizedAccessService)
        {
            this.azureService = azureService;
            this.ofertasService = ofertasService;
            listaCompraRepository = syncronizedAccessService.ListaCompraRepository;
            participantesRepository = syncronizedAccessService.ParticipanteListaCompraRepository;
            userRepository = syncronizedAccessService.UserRepository;
            produtoListaCompraRepository = syncronizedAccessService.ProdutoListaCompraRepository;
        }
        public async Task<IEnumerable<ListaCompra>> CarregarListasDeComprasUsuarioLogadoAsync()
        {
            Dictionary<string, ListaCompra> grupos = (await listaCompraRepository.GetEntitiesAsync()).ToDictionary(key => key.Id, value => value);
            var retorno = (await participantesRepository.GetEntitiesAsync()).Where(x => x.IdUser == azureService.CurrentUser.User.Id).Select(x => grupos[x.IdListaCompra]).OrderBy(x => x.Nome);
            return retorno.ToArray();
        }
        public async Task<IEnumerable<ListaCompra>> CarregarListasDeComprasUsuarioLogadoSync(IEnumerable<string> localResult = null)
        {
            var date = azureService.CurrentUser.User.UpdatedAt;
            await participantesRepository.SyncDataBaseAsync(date);
            await listaCompraRepository.SyncDataBaseAsync(date);
            await produtoListaCompraRepository.SyncDataBaseAsync(date);

            var newResult = await CarregarListasDeComprasUsuarioLogadoAsync();
            if (newResult.Count() > localResult.Count())
            {
                return newResult.Where(x => !localResult.Contains(x.Id)).ToList();
            }
            return null;
        }
        public async Task<string[]> ObterMarcasPorProduto(string idProduto)
        {
            return await ofertasService.ObterMarcasPorProdutoAsync(idProduto);
        }

        public async Task CadastrarNovaListaDeCompraAsync(ListaCompra listaCompra)
        {
            var idListaCompra = await listaCompraRepository.CreateEntityAsync(listaCompra, true);
            foreach (var participante in listaCompra.Participantes)
            {
                participante.IdListaCompra = idListaCompra;
                participante.NomeLista = listaCompra.Nome;
                await participantesRepository.CreateEntityAsync(participante);
            }

            foreach (var produto in listaCompra?.ProdutosListaCompra)
            {
                produto.IdListaCompra = idListaCompra;
                produto.IdMarca = produto.IdMarca ?? await ofertasService.CreateOrRetrieveEntityByNameAsync(produto.Marca);
                produto.IdUnidadeMedida = produto.IdUnidadeMedida ?? await ofertasService.CreateOrRetrieveEntityByNameAsync(produto.UnidadeMedida);
                await produtoListaCompraRepository.CreateEntityAsync(produto);
            }
        }

        internal async Task<List<Produto>> ObterProdutoPorNome(string expression)
        {
            return await ofertasService.ObterProdutoPorNome(expression);
        }

        public async Task AtualizarListaDeComprasAsync(ListaCompra listaCompra, bool sync)
        {
            await listaCompraRepository.UpdateEntityAsync(listaCompra);

            foreach (var participante in listaCompra?.Participantes)
            {
                if (participante.IdListaCompra == null) //novo participante
                {
                    participante.IdListaCompra = listaCompra.Id;
                    participante.NomeLista = listaCompra.Nome;
                    await participantesRepository.CreateEntityAsync(participante);
                }
            }
            if (sync)
                await participantesRepository.SyncDataBaseAsync();

            foreach (var produtoLista in listaCompra?.ProdutosListaCompra)
            {
                if (produtoLista.Id == null)
                {
                    produtoLista.IdListaCompra = listaCompra.Id;
                    await produtoListaCompraRepository.CreateEntityAsync(produtoLista);
                }
                else
                {
                    produtoLista.IdMarca = produtoLista.IdMarca ?? await ofertasService.CreateOrRetrieveEntityByNameAsync(produtoLista.Marca);
                    produtoLista.IdProduto = produtoLista.IdProduto;
                    produtoLista.IdUnidadeMedida = produtoLista.IdUnidadeMedida ?? await ofertasService.CreateOrRetrieveEntityByNameAsync(produtoLista.UnidadeMedida);
                    await produtoListaCompraRepository.UpdateEntityAsync(produtoLista);
                }
            }
            if (sync)
                await participantesRepository.SyncDataBaseAsync();
        }

        internal async Task AdicionarProdutoAListaAsync(ListaCompra editList, ProdutoListaCompra produtoListaCompra)
        {
            produtoListaCompra.IdListaCompra = editList.Id;
            await produtoListaCompraRepository.CreateEntityAsync(produtoListaCompra);
        }

        internal async Task LoadAutoCompleteAsync(string[] idsPedidos)
        {
            foreach (var idPedido in idsPedidos)
            {
                await ofertasService.LoadUnidadeMedidaAutoCompleteAsync(idPedido);
            }
        }


        public async Task ExcluirListaCompraAsync(ListaCompra listaCompra)
        {
            if (listaCompra == null)
                return;

            //listaCompra.Participantes = await participantesRepository.SyncTableModel.Where(x => x.IdListaCompra == listaCompra.Id).ToListAsync();
            foreach (var participante in listaCompra?.Participantes)
            {
                participante.Delete = true;
                await participantesRepository.UpdateEntityAsync(participante); //notify ser and delete on server         
            }
            foreach (var produtoListaCompra in listaCompra?.ProdutosListaCompra)
            {
                produtoListaCompra.Delete = true;
                await produtoListaCompraRepository.UpdateEntityAsync(produtoListaCompra); //notify ser and delete on server         
            }
            listaCompra.Delete = true;
            await listaCompraRepository.UpdateEntityAsync(listaCompra);
            await listaCompraRepository.SyncDataBaseAsync();

            listaCompra?.Participantes.ToList().ForEach(async x => await participantesRepository.DeleteEntityAsync(x));
            listaCompra?.ProdutosListaCompra.ToList().ForEach(async x => await produtoListaCompraRepository.DeleteEntityAsync(x));
            await listaCompraRepository.DeleteEntityAsync(listaCompra);
        }

        internal async Task<ListaCompra> CarregarListaCompraPorIdAsync(string id)
        {
            return await listaCompraRepository.GetByIdAsync(id);
        }

        internal async Task<IEnumerable<ParticipanteLista>> CarregarParticipantesPorIdListaDeCompraAsync(string idListaCompra)
        {
            var participantes = await participantesRepository.SyncTableModel.Where(x => x.IdListaCompra == idListaCompra).ToListAsync();
            participantes.ForEach(async x => x.User = await userRepository.GetByIdAsync(x.IdUser));
            return participantes;
        }

        internal async Task<SimulacaoCompraDto> SimularCompraAsync(ListaCompra listaCompra)
        {
            using (var scope = App.Container.BeginLifetimeScope())
            {
                var filter = scope.Resolve<IFilterSimulacaoCompra>();
                filter.InitialValue = listaCompra.ProdutosListaCompra;
                var teste = await (await (await filter.FiltrarPorProdutoTipoAsync()).FiltrarPorMarcaAsync()).FiltrarPorQuantidadeAsync();
                var teste2 = await teste.GetFilterResult();
                return teste2;
            }
        }

        internal async Task<IEnumerable<ProdutoListaCompra>> CarregarProdutosListaCompraPorIdListaDeCompraAsync(string idListaCompra)
        {
            var produtosListaCompra = await produtoListaCompraRepository.SyncTableModel.Where(x => x.IdListaCompra == idListaCompra).ToListAsync();
            return ofertasService.PreencherInformacoesDeOfertaNoProdutoLista(produtosListaCompra);
        }

        internal async Task ExcluirParticipanteListaCompraAsync(ParticipanteLista membro)
        {
            membro.Delete = true;
            await participantesRepository.UpdateEntityAsync(membro); //notify ser and delete on server            
            await participantesRepository.SyncDataBaseAsync();
            await participantesRepository.DeleteEntityAsync(membro);
            await participantesRepository.SyncDataBaseAsync();
        }
        internal async void ExcluirProdutoListaCompraAsync(ProdutoListaCompra produtoListaCompra)
        {
            produtoListaCompra.Delete = true;
            await produtoListaCompraRepository.UpdateEntityAsync(produtoListaCompra); //notify ser and delete on server            
            await produtoListaCompraRepository.SyncDataBaseAsync();
            await produtoListaCompraRepository.DeleteEntityAsync(produtoListaCompra);
            await produtoListaCompraRepository.SyncDataBaseAsync();
        }


    }
}

