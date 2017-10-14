using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using GoodBuy.Service.Interfaces;
using System;
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
        public async Task<IEnumerable<GrupoOferta>> CarregarGrupoDeOfertasUsuarioLogado()
        {
            Dictionary<string, GrupoOferta> grupos = (await grupoRepository.GetEntities()).ToDictionary(key => key.Id, value => value);
            var retorno = (await participantesRepository.GetEntities()).Where(x => x.IdUser == azureService.CurrentUser.User.Id).Select(x => grupos[x.IdGrupoOferta]).OrderBy(x => x.Name);
            return retorno.ToArray();
        }
        public async Task<IEnumerable<GrupoOferta>> CarregarGrupoDeOfertasUsuarioLogadoSync(IEnumerable<string> localResult = null)
        {
            var date = azureService.CurrentUser.User.UpdatedAt;
            await participantesRepository.SyncDataBase(date);
            await grupoRepository.SyncDataBase(date);

            var newResult = await CarregarGrupoDeOfertasUsuarioLogado();
            if (newResult.Count() > localResult.Count())
            {
                return newResult.Where(x => !localResult.Contains(x.Id)).ToList();
            }
            return null;
        }


        public async Task<IEnumerable<GrupoOferta>> LocalizarGruposOfertaPublicos(string searchTerm)
        {
            return await grupoRepository.SyncTableModel.Where(x => !x.Private && x.Name.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
        }

        public async Task CadastrarNovoGrupoUsuario(GrupoOferta grupoOferta, List<ParticipanteGrupo> participantes)
        {
            var idOferta = await grupoRepository.CreateEntity(grupoOferta, true);
            foreach (var participante in participantes)
            {
                participante.IdGrupoOferta = idOferta;
                participante.NomeGrupo = grupoOferta.Name;
                await participantesRepository.CreateEntity(participante);
            }
        }

        public async Task ParticiparGrupo(GrupoOferta grupoOferta, ParticipanteGrupo participanteGrupo)
        {
            participanteGrupo.IdGrupoOferta = grupoOferta.Id;
            participanteGrupo.NomeGrupo = grupoOferta.Name;
            await participantesRepository.CreateEntity(participanteGrupo);
            await participantesRepository.SyncDataBase();
        }

        public async Task AtualizarNovoGrupoUsuario(GrupoOferta grupoOferta, bool sync)
        {
            await grupoRepository.UpdateEntity(grupoOferta);

            foreach (var participante in grupoOferta?.Participantes)
            {
                if (participante.IdGrupoOferta == null) //novo participante
                {
                    participante.IdGrupoOferta = grupoOferta.Id;
                    participante.NomeGrupo = grupoOferta.Name;
                    await participantesRepository.CreateEntity(participante);
                    if (sync)
                        await participantesRepository.SyncDataBase();

                }
            }
        }

        public async Task ExcluirGrupoOferta(GrupoOferta grupoOferta)
        {
            if (grupoOferta == null)
                return;

            grupoOferta.Participantes = await participantesRepository.SyncTableModel.Where(x => x.IdGrupoOferta == grupoOferta.Id).ToListAsync();

            foreach (var participante in grupoOferta?.Participantes)
            {
                participante.Delete = true;
                await participantesRepository.UpdateEntity(participante); //notify ser and delete on server         
            }
            grupoOferta.Delete = true;
            await grupoRepository.UpdateEntity(grupoOferta);
            await grupoRepository.SyncDataBase();

            grupoOferta?.Participantes.ToList().ForEach(async x => await participantesRepository.DeleteEntity(x));
            await grupoRepository.DeleteEntity(grupoOferta);
        }

        internal async Task<GrupoOferta> CarregarGrupoOfertaPorId(string id)
        {
            return await grupoRepository.GetById(id);
        }

        internal async Task<IEnumerable<ParticipanteGrupo>> CarregarParticipantesPorIdGrupoOferta(string id)
        {
            var participantes = await participantesRepository.SyncTableModel.Where(x => x.IdGrupoOferta == id).ToListAsync();
            participantes.ForEach(async x => x.User = await userRepository.GetById(x.IdUser));
            return participantes;

        }

        internal async Task ExcluirParticipanteGrupoOferta(ParticipanteGrupo membro)
        {
            membro.Delete = true;
            await participantesRepository.UpdateEntity(membro); //notify ser and delete on server            
            await participantesRepository.SyncDataBase();
            await participantesRepository.DeleteEntity(membro);
            await participantesRepository.SyncDataBase();
        }
    }
}
