namespace GoodBuy.ViewModels
{
    class MainMenuViewModel : BaseViewModel
    {
        public string Token { get; }
        public MainMenuViewModel(object token)
        {
            Token = token?.ToString();
        }

    }
}
