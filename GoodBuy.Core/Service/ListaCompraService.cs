using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodBuy.Service
{
    public class ListaCompraService
    {
        private readonly IGenericRepository<ListaCompra> listaCompraRepository;
        private readonly IGenericRepository<ParticipanteListaCompra> participantesRepository;
        private readonly IGenericRepository<User> userRepository;
        private readonly AzureService azureService;

        public ListaCompraService(AzureService azureService, SyncronizedAccessService syncronizedAccessService)
        {
            this.azureService = azureService;
            listaCompraRepository = syncronizedAccessService.ListaCompraRepository;
            participantesRepository = syncronizedAccessService.ParticipanteListaCompraRepository;
            userRepository = syncronizedAccessService.UserRepository;
        }
        public async Task<IEnumerable<ListaCompra>> CarregarGrupoDeOfertasUsuarioLogadoAsync()
        {
            Dictionary<string, ListaCompra> grupos = (await listaCompraRepository.GetEntitiesAsync()).ToDictionary(key => key.Id, value => value);
            var retorno = (await participantesRepository.GetEntitiesAsync()).Where(x => x.IdUser == azureService.CurrentUser.User.Id).Select(x => grupos[x.IdListaCompra]).OrderBy(x => x.Nome);
            return retorno.ToArray();
        }
        public async Task<IEnumerable<ListaCompra>> CarregarGrupoDeOfertasUsuarioLogadoSync(IEnumerable<string> localResult = null)
        {
            var date = azureService.CurrentUser.User.UpdatedAt;
            await participantesRepository.SyncDataBaseAsync(date);
            await listaCompraRepository.SyncDataBaseAsync(date);

            var newResult = await CarregarGrupoDeOfertasUsuarioLogadoAsync();
            if (newResult.Count() > localResult.Count())
            {
                return newResult.Where(x => !localResult.Contains(x.Id)).ToList();
            }
            return null;
        }

        public async Task CadastrarNovaListaDeCompraAsync(ListaCompra listaCompra, List<ParticipanteListaCompra> participantes)
        {
            var idListaCompra = await listaCompraRepository.CreateEntityAsync(listaCompra, true);
            foreach (var participante in participantes)
            {
                participante.IdListaCompra= idListaCompra;
                await participantesRepository.CreateEntityAsync(participante);
            }
        }

        public async Task AtualizarNovoGrupoUsuarioAsync(ListaCompra listaCompra, bool sync)
        {
            await listaCompraRepository.UpdateEntityAsync(listaCompra);

            //foreach (var participante in listaCompra?.Participantes)
            //{
            //    if (participante.IdGrupoOferta == null) //novo participante
            //    {
            //        participante.IdGrupoOferta = listaCompra.Id;
            //        participante.NomeGrupo = listaCompra.Name;
            //        await participantesRepository.CreateEntityAsync(participante);
            //        if (sync)
            //            await participantesRepository.SyncDataBaseAsync();

            //    }
            //}
        }

        public async Task ExcluirGrupoOfertaAsync(ListaCompra listaCompra)
        {
            //if (listaCompra == null)
            //    return;

            //listaCompra.Participantes = await participantesRepository.SyncTableModel.Where(x => x.IdGrupoOferta == listaCompra.Id).ToListAsync();

            //foreach (var participante in listaCompra?.Participantes)
            //{
            //    participante.Delete = true;
            //    await participantesRepository.UpdateEntityAsync(participante); //notify ser and delete on server         
            //}
            //listaCompra.Delete = true;
            //await listaCompraRepository.UpdateEntityAsync(listaCompra);
            //await listaCompraRepository.SyncDataBaseAsync();

            //listaCompra?.Participantes.ToList().ForEach(async x => await participantesRepository.DeleteEntityAsync(x));
            //await listaCompraRepository.DeleteEntityAsync(listaCompra);
        }

        internal async Task<ListaCompra> CarregarGrupoOfertaPorIdAsync(string id)
        {
            return await listaCompraRepository.GetByIdAsync(id);
        }

        internal async Task<IEnumerable<ParticipanteListaCompra>> CarregarParticipantesPorIdGrupoOfertaAsync(string id)
        {
            var participantes = await participantesRepository.SyncTableModel.Where(x => x.IdListaCompra == id).ToListAsync();
            participantes.ForEach(async x => x.User = await userRepository.GetByIdAsync(x.IdUser));
            return participantes;

        }

        internal async Task ExcluirParticipanteGrupoOfertaAsync(ParticipanteGrupo membro)
        {
            //membro.Delete = true;
            //await participantesRepository.UpdateEntityAsync(membro); //notify ser and delete on server            
            //await participantesRepository.SyncDataBaseAsync();
            //await participantesRepository.DeleteEntityAsync(membro);
            //await participantesRepository.SyncDataBaseAsync();
        }


    }
}
