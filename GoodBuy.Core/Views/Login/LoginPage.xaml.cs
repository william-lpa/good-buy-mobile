﻿using System;
using Autofac;
using GoodBuy.ViewModels;
using Xamarin.Forms;

namespace GoodBuy.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage() : this(null)
        {
            lvwOfertas.ItemTapped += ResetarSelecao;
        }

        private void ResetarSelecao(object sender, ItemTappedEventArgs e)
        {
            lvwOfertas.SelectedItem = null;
        }


        public LoginPage(string param = null)
        {
            LoginPageViewModel login;
            InitializeComponent();
            using (var scope = App.Container.BeginLifetimeScope())
            {
                login = scope.Resolve<LoginPageViewModel>();
                BindingContext = login;
            }
            login.TrySSOAsync(param);
        }
    }
}
