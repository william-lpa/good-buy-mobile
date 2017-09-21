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
            Task.WaitAll(new Task[] { Task.Run(() => participantesRepository.SyncDataBase()), Task.Run(() => participantesRepository.SyncDataBase()) });
            Dictionary<string, GrupoOferta> grupos = (await grupoRepository.GetEntities()).ToDictionary(key => key.Id, value => value);
            var retorno = (await participantesRepository.GetEntities()).Where(x => x.IdUser == azureService.CurrentUser.User.Id).Select(x => grupos[x.IdGrupoOferta]).OrderBy(x => x.Name);
            return retorno.ToArray();
        }

        public async Task CadastrarNovoGrupoUsuario(GrupoOferta grupoOferta)
        {
            var idOferta = await grupoRepository.CreateEntity(grupoOferta, true);
            foreach (var participante in grupoOferta?.Participantes)
            {
                participante.IdGrupoOferta = idOferta;//confirmar
                await participantesRepository.CreateEntity(participante);
            }
        }

        public async Task AtualizarNovoGrupoUsuario(GrupoOferta grupoOferta)
        {
            await grupoRepository.UpdateEntity(grupoOferta);

            foreach (var participante in grupoOferta?.Participantes)
            {
                if (participante.IdGrupoOferta == null) //novo participante
                {
                    participante.IdGrupoOferta = grupoOferta.Id;
                    await participantesRepository.CreateEntity(participante);
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
                await participantesRepository.SyncTableModel.DeleteAsync(participante);
            }
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
    }
}
