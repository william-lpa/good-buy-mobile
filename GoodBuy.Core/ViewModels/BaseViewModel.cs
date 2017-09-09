using Autofac;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    public class BaseViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected async Task PushAsync<TViewModel>(bool resetNavigation = false, params object[] args) where TViewModel : BaseViewModel
        {
            var viewmodelType = typeof(TViewModel);
            var viewModelTypeName = viewmodelType.Name;
            var viewModelWordLength = "ViewModel".Length;
            var viewTypeName = $"GoodBuy.Views.{viewModelTypeName.Substring(0, viewModelTypeName.Length - viewModelWordLength) }";
            var viewType = Type.GetType(viewTypeName);
            var page = Activator.CreateInstance(viewType) as Page;
            TViewModel viewModel = null;
            using (var scope = App.Container.BeginLifetimeScope())
            { viewModel = scope.Resolve<TViewModel>(); }
            //var viewModel = Activator.CreateInstance(viewmodelType, args);
            if (page != null)
                page.BindingContext = viewModel;
            if (resetNavigation)
            {
                await Application.Current.MainPage.Navigation.PopToRootAsync();
                Application.Current.MainPage = new NavigationPage(page);
            }
            else
                await Application.Current.MainPage.Navigation.PushAsync(page);

        }
        protected async Task PopModalAsync() => await Application.Current.MainPage.Navigation.PopModalAsync(true);

        protected async Task PopAsync() => await Application.Current.MainPage.Navigation.PopAsync(true);

        protected async Task PopToRootAsync() => await Application.Current.MainPage.Navigation.PopToRootAsync(true);

        protected async Task<TViewModel> PushModalAsync<TViewModel>(params object[] args) where TViewModel : BaseViewModel
        {
            var viewmodelType = typeof(TViewModel);
            var viewModelTypeName = viewmodelType.Name;
            var viewModelWordLength = "ViewModel".Length;
            var viewTypeName = $"GoodBuy.Views.{viewModelTypeName.Substring(0, viewModelTypeName.Length - viewModelWordLength) }";
            var viewType = Type.GetType(viewTypeName);
            var page = Activator.CreateInstance(viewType) as Page;
            TViewModel viewModel = null;
            using (var scope = App.Container.BeginLifetimeScope())
            { viewModel = scope.Resolve<TViewModel>(); }
            //var viewModel = Activator.CreateInstance(viewmodelType, args);
            if (page != null)
                page.BindingContext = viewModel;
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
            return (TViewModel)viewModel;
        }
    }
}
