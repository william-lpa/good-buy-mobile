using System.Windows.Input;
using Xamarin.Forms;

namespace GoodBuy.Core.Controls
{
    public class CustomListView : ListView
    {
        public static readonly BindableProperty ItemTappedCommandProperty = BindableProperty.Create("ItemTappedCommand", typeof(ICommand), typeof(CustomListView), null);
        public static readonly BindableProperty TextChangedCommandProperty = BindableProperty.Create("TextChangedCommand", typeof(ICommand), typeof(CustomListView), null);

        public ICommand ItemTappedCommand
        {
            get => (ICommand)GetValue(ItemTappedCommandProperty);
            set => SetValue(ItemTappedCommandProperty, value);
        }
        public ICommand TextChangedCommand
        {
            get => (ICommand)GetValue(ItemTappedCommandProperty);
            set => SetValue(ItemTappedCommandProperty, value);
        }
        public CustomListView(ListViewCachingStrategy strategy) : base(strategy)
        {
        }
        public CustomListView() : this(ListViewCachingStrategy.RecycleElement)
        {
            Initialize();
        }
        private void Initialize()
        {
            this.ItemSelected += (sender, e) =>
            {

                if (ItemTappedCommand == null)
                    return;
                if (ItemTappedCommand.CanExecute(e.SelectedItem))
                    ItemTappedCommand.Execute(e.SelectedItem);
            };
        }
    }

}

