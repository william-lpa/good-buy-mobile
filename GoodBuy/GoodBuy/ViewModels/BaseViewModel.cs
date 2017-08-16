using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    class BaseViewModel
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

        protected async Task PushAsync<TViewModel>(params object[] args) where TViewModel : BaseViewModel
        {
            var viewmodelType = typeof(TViewModel);
            var viewModelTypeName = viewmodelType.Name;
            var viewModelWordLength = "ViewModel".Length;
            var viewTypeName = $"GoodBuy.Views.{viewModelTypeName.Substring(0, viewModelTypeName.Length - viewModelWordLength) }";
            var viewType = Type.GetType(viewTypeName);
            var page = Activator.CreateInstance(viewType) as Page;
            var viewModel = Activator.CreateInstance(viewmodelType, args);
            if (page != null)
                page.BindingContext = viewModel;
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
        protected async Task PopModalAsync() => await Application.Current.MainPage.Navigation.PopModalAsync(true);

        protected async Task<TViewModel> PushModalAsync<TViewModel>(params object[] args) where TViewModel : BaseViewModel
        {
            var viewmodelType = typeof(TViewModel);
            var viewModelTypeName = viewmodelType.Name;
            var viewModelWordLength = "ViewModel".Length;
            var viewTypeName = $"GoodBuy.Views.{viewModelTypeName.Substring(0, viewModelTypeName.Length - viewModelWordLength) }";
            var viewType = Type.GetType(viewTypeName);
            var page = Activator.CreateInstance(viewType) as Page;
            var viewModel = Activator.CreateInstance(viewmodelType, args);
            if (page != null)
                page.BindingContext = viewModel;
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
            return (TViewModel)viewModel;
        }
    }
}
