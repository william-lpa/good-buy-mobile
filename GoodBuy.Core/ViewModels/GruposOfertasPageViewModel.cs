﻿using GoodBuy.Models;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using GoodBuy.Log;

namespace GoodBuy.ViewModels
{
    public class GruposOfertasPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly GrupoOfertaService grupoOfertaService;
        public Command NovoGrupoCommand { get; }
        public Command EditGroupCommand { get; }
        public Command SearchPublicGroup { get; }
        public ObservableCollection<GrupoOferta> GruposOfertasUsuario { get; private set; }
        public ObservableCollection<GrupoOferta> CachedList { get; set; }
        private bool notShareMode;

        public bool NotSharing
        {
            get { return notShareMode; }
            set { SetProperty(ref notShareMode, value); }
        }

        public GruposOfertasPageViewModel(AzureService azureService, GrupoOfertaService service)
        {
            NotSharing = true;
            this.azureService = azureService;
            grupoOfertaService = service;
            NovoGrupoCommand = new Command(ExecuteCriarNovoGrupoOferta);
            EditGroupCommand = new Command<GrupoOferta>(ExecuteEditarGroupoOfertas);
            SearchPublicGroup = new Command<string>(ExecuteSearchPublicGroup);
            GruposOfertasUsuario = new ObservableCollection<GrupoOferta>();
            CachedList = new ObservableCollection<GrupoOferta>();
        }

        private async void ExecuteSearchPublicGroup(string expression)
        {
            if (expression?.Length == 1 && CachedList.Count == 0)
            {
                CachedList = new ObservableCollection<GrupoOferta>(GruposOfertasUsuario);
            }
            GruposOfertasUsuario.Clear();

            if (string.IsNullOrEmpty(expression))
            {
                foreach (var item in CachedList)
                {
                    GruposOfertasUsuario.Add(item);
                }
                CachedList.Clear();
                return;
            }

            var result = await grupoOfertaService.LocalizarGruposOfertaPublicos(expression);
            foreach (var item in result)
            {
                GruposOfertasUsuario.Add(item);
            }
        }

        private async void ExecuteEditarGroupoOfertas(GrupoOferta grupoOferta)
        {
            if (NotSharing)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("ID", grupoOferta.Id);
                await PushAsync<NovoGrupoOfertaPageViewModel>(false, parameters);
            }
            else
            {
                if (await MessageDisplayer.Instance.ShowAsk("Compartilhar Oferta", $"Você tem certeza que deseja compartilhar a oferta selecionada com o grupo {grupoOferta.Name} ?", "Sim", "Não"))
                {
                    //azureService.Client.InvokeApiAsync();
                    //await PopAsync<GruposOfertasPageViewModel>();
                }

            }
        }

        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            GruposOfertasUsuario.Clear();
            var grupos = await grupoOfertaService.CarregarGrupoDeOfertasUsuarioLogado();
            foreach (var grupo in grupos)
            {
                GruposOfertasUsuario.Add(grupo);
            }
            var newGroups = await grupoOfertaService.CarregarGrupoDeOfertasUsuarioLogadoSync(grupos.Select(x => x.Id));
            if (newGroups != null)
            {
                foreach (var grupo in grupos)
                {
                    GruposOfertasUsuario.Add(grupo);
                }
            }
        }

        private async void ExecuteCriarNovoGrupoOferta()
        {
            await PushAsync<NovoGrupoOfertaPageViewModel>();
        }
    }
}
