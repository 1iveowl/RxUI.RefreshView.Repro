using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using ReactiveUI;

namespace RefreshViewDemo
{
    public class MainPageViewModel : ReactiveObject, INotifyPropertyChanged
    {
        const int RefreshDuration = 2;
        int itemNumber = 1;
        readonly Random random;
        bool isRefreshing;

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Item> Items { get; private set; }

        
        // Replaced:
        //public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        // Replacement:
        private ReactiveCommand<Unit, Unit> _refreshCommand;

        public ReactiveCommand<Unit, Unit> RefreshCommand
        {
            get => _refreshCommand;
            set => this.RaiseAndSetIfChanged(ref _refreshCommand, value);
        }


        public MainPageViewModel()
        {
            random = new Random();
            Items = new ObservableCollection<Item>();
            AddItems();

            // Added
            DefineRefreshCommandCommand();
        }

        void AddItems()
        {
            for (int i = 0; i < 50; i++)
            {
                Items.Add(new Item
                {
                    Color = Color.FromRgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
                    Name = $"Item {itemNumber++}"
                });
            }
        }


        //Added
        private void DefineRefreshCommandCommand()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask<Unit>(async x =>
            {
                await RefreshItemsAsync();
            });

            RefreshCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message));
        }

        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            AddItems();
            IsRefreshing = false;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
