using System.Windows.Input;
using Xamarin.Forms;

namespace GoodBuy.Core.Controls
{
    public class CustomListView : ListView
    {
        public static readonly BindableProperty ItemTappedCommandProperty = BindableProperty.Create("ItemTappedCommand", typeof(ICommand), typeof(CustomListView), null);
        public static readonly BindableProperty TextChangedCommandProperty = BindableProperty.Create("TextChangedCommand", typeof(ICommand), typeof(CustomListView), null);
        public static readonly BindableProperty ResetSelectedProperty = BindableProperty.Create("ResetSelected", typeof(bool), typeof(CustomListView), true, BindingMode.TwoWay);

        public ICommand ItemTappedCommand
        {
            get => (ICommand)GetValue(ItemTappedCommandProperty);
            set => SetValue(ItemTappedCommandProperty, value);
        }

        public bool ResetSelected
        {
            get => (bool)GetValue(ResetSelectedProperty);
            set => SetValue(ResetSelectedProperty, value);
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
            this.ItemTapped += (sender, e) =>
            {

                if (ItemTappedCommand == null)
                    return;
                if (ItemTappedCommand.CanExecute(e.Item))
                    ItemTappedCommand.Execute(e.Item);
                if (ResetSelected)
                    SelectedItem = null;
            };
        }
    }

}

