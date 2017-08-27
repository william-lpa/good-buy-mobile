using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoodBuy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu : ContentPage
    {
        public MainMenu()
        {
            InitializeComponent();

            var uri = new UriImageSource();
            uri.Uri = new System.Uri("https://scontent.xx.fbcdn.net/v/t1.0-1/p50x50/1935044_794747160671958_6623532753384248269_n.jpg?oh=c9bff7e2df158dc76a525d8af326873d&oe=5A24E960");
            image.Source = uri;


        }
    }
}