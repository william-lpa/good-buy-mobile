using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodBuy.Service
{
    public class GrupoOfertaService
    {
        private readonly IGenericRepository<GrupoOferta> grupoRepository;
        private readonly IGenericRepository<ParticipanteGrupo> participantesRepository;
        private readonly IGenericRepository<User> userRepository;
        private readonly AzureService azureService;

        public GrupoOfertaService(AzureService azureService, SyncronizedAccessService syncronizedAccessService)
        {
            this.azureService = azureService;
            grupoRepository = syncronizedAccessService.GrupoOfertaRepository;
            participantesRepository = syncronizedAccessService.ParticipanteGrupoRepository;
            userRepository = syncronizedAccessService.UserRepository;
        }
        public async Task<IEnumerable<GrupoOferta>> CarregarGrupoDeOfertasUsuarioLogadoAsync()
        {
            Dictionary<string, GrupoOferta> grupos = (await grupoRepository.GetEntitiesAsync()).ToDictionary(key => key.Id, value => value);
            var retorno = (await participantesRepository.GetEntitiesAsync()).Where(x => x.IdUser == azureService.CurrentUser.User.Id).Select(x => grupos[x.IdGrupoOferta]).OrderBy(x => x.Name);
            return retorno.ToArray();
        }
        public async Task<IEnumerable<GrupoOferta>> CarregarGrupoDeOfertasUsuarioLogadoSync(IEnumerable<string> localResult = null)
        {
            var date = azureService.CurrentUser.User.UpdatedAt;
            await participantesRepository.SyncDataBaseAsync(date);
            await grupoRepository.SyncDataBaseAsync(date);

            var newResult = await CarregarGrupoDeOfertasUsuarioLogadoAsync();
            if (newResult.Count() > localResult.Count())
            {
                return newResult.Where(x => !localResult.Contains(x.Id)).ToList();
            }
            return null;
        }


        public async Task<IEnumerable<GrupoOferta>> LocalizarGruposOfertaPublicosAsync(string searchTerm)
        {
            return await grupoRepository.SyncTableModel.Where(x => !x.Private && x.Name.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
        }

        public async Task CadastrarNovoGrupoUsuarioAsync(GrupoOferta grupoOferta, List<ParticipanteGrupo> participantes)
        {
            var idOferta = await grupoRepository.CreateEntityAsync(grupoOferta, true);
            foreach (var participante in participantes)
            {
                participante.IdGrupoOferta = idOferta;
                participante.NomeGrupo = grupoOferta.Name;
                await participantesRepository.CreateEntityAsync(participante);
            }
        }

        public async Task ParticiparGrupoAsync(GrupoOferta grupoOferta, ParticipanteGrupo participanteGrupo)
        {
            participanteGrupo.IdGrupoOferta = grupoOferta.Id;
            participanteGrupo.NomeGrupo = grupoOferta.Name;
            await participantesRepository.CreateEntityAsync(participanteGrupo);
            await participantesRepository.SyncDataBaseAsync();
        }

        public async Task AtualizarNovoGrupoUsuarioAsync(GrupoOferta grupoOferta, bool sync)
        {
            await grupoRepository.UpdateEntityAsync(grupoOferta);

            foreach (var participante in grupoOferta?.Participantes)
            {
                if (participante.IdGrupoOferta == null) //novo participante
                {
                    participante.IdGrupoOferta = grupoOferta.Id;
                    participante.NomeGrupo = grupoOferta.Name;
                    await participantesRepository.CreateEntityAsync(participante);
                    if (sync)
                        await participantesRepository.SyncDataBaseAsync();

                }
            }
        }

        public async Task ExcluirGrupoOfertaAsync(GrupoOferta grupoOferta)
        {
            if (grupoOferta == null)
                return;

            grupoOferta.Participantes = await participantesRepository.SyncTableModel.Where(x => x.IdGrupoOferta == grupoOferta.Id).ToListAsync();

            foreach (var participante in grupoOferta?.Participantes)
            {
                participante.Delete = true;
                await participantesRepository.UpdateEntityAsync(participante); //notify ser and delete on server         
            }
            grupoOferta.Delete = true;
            await grupoRepository.UpdateEntityAsync(grupoOferta);
            await grupoRepository.SyncDataBaseAsync();

            grupoOferta?.Participantes.ToList().ForEach(async x => await participantesRepository.DeleteEntityAsync(x));
            await grupoRepository.DeleteEntityAsync(grupoOferta);
        }

        internal async Task<GrupoOferta> CarregarGrupoOfertaPorIdAsync(string id)
        {
            return await grupoRepository.GetByIdAsync(id);
        }

        internal async Task<IEnumerable<ParticipanteGrupo>> CarregarParticipantesPorIdGrupoOfertaAsync(string id)
        {
            var participantes = await participantesRepository.SyncTableModel.Where(x => x.IdGrupoOferta == id).ToListAsync();
            participantes.ForEach(async x => x.User = await userRepository.GetByIdAsync(x.IdUser));
            return participantes;

        }

        internal async Task ExcluirParticipanteGrupoOfertaAsync(ParticipanteGrupo membro)
        {
            membro.Delete = true;
            await participantesRepository.UpdateEntityAsync(membro); //notify ser and delete on server            
            await participantesRepository.SyncDataBaseAsync();
            await participantesRepository.DeleteEntityAsync(membro);
            await participantesRepository.SyncDataBaseAsync();
        }
    }
}
