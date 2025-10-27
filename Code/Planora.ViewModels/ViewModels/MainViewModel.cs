using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using Planora.ViewModels.ViewModels;

namespace Planora.ViewModels.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private string _title = "Planora - Система управління розкладом";

        public MainViewModel()
        {

            CurrentViewModel = new LoginViewModel();
            
            ShowLoginCommand = new RelayCommand(_ => CurrentViewModel = new LoginViewModel());
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public RelayCommand ShowLoginCommand { get; }
    }
}