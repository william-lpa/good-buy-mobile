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
        private readonly AzureService azureService;
        private Dictionary<string, GrupoOferta> cacheGrupo;
        private Dictionary<string, ParticipanteGrupo> cacheParticipantes;
        private const int MINUTES_TO_REFRESH = 10;
        private DateTime LastRefresh;

        public GrupoOfertaService(AzureService azureService)
        {
            this.azureService = azureService;
            grupoRepository = new GenericRepository<GrupoOferta>(azureService);
            participantesRepository = new GenericRepository<ParticipanteGrupo>(azureService);
            cacheGrupo = new Dictionary<string, GrupoOferta>();
            cacheParticipantes = new Dictionary<string, ParticipanteGrupo>();
            LastRefresh = DateTime.Now;
        }

        private void ObterGruposDeOfertaUsuarioLogado()
        {
            Task.WaitAll(new Task[] { Task.Run(() => participantesRepository.SyncDataBase()), Task.Run(() => participantesRepository.SyncDataBase()) });
            Task.WaitAll(new Task[]
            {
                Task.Run(async () =>  cacheParticipantes = ((await participantesRepository.SyncTableModel.ToListAsync()).ToDictionary(key => key.Id, value => value))),
                Task.Run(async () => cacheGrupo = ((await grupoRepository.SyncTableModel.ToListAsync()).ToDictionary(key => key.Id, value => value))),
            });
            LastRefresh = DateTime.Now;
        }

        public IEnumerable<GrupoOferta> CarregarGrupoDeOfertasUsuarioLogado()
        {
            if ((LastRefresh - DateTime.Now).TotalMinutes > MINUTES_TO_REFRESH)
            {
                ObterGruposDeOfertaUsuarioLogado();
            }
            return cacheParticipantes.Values.Where(x => x.IdUser == azureService.CurrentUser.User.Id).Select(x => cacheGrupo[x.IdGrupoOferta]).OrderBy(x => x.Name);
        }

        public void CadastrarNovoGrupoUsuario(GrupoOferta grupoOferta)
        {
            foreach (var participante in grupoOferta?.Participantes)
            {
                new ParticipanteGrupo(participante.IdUser, participante.IdGrupoOferta);//confirmar
                participantesRepository.CreateEntity(participante);
                cacheParticipantes.Add(participante.Id, participante);
            }

            grupoRepository.CreateEntity(grupoOferta);
            cacheGrupo.Add(grupoOferta.Id, grupoOferta);
        }
    }
}
