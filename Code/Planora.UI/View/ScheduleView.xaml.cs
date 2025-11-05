using System;
using System.Windows;
using System.Windows.Controls;
using Planora.ViewModels.ViewModels;

namespace Planora.UI.View
{
    public partial class ScheduleView : Window
    {
        private ScheduleViewViewModel _viewModel;

        public ScheduleView()
        {
            InitializeComponent();

            // Ініціалізація ViewModel
            _viewModel = new ScheduleViewViewModel();
            DataContext = _viewModel;

            // Коли вікно завантажується — оновлюємо розклад
            this.Loaded += ScheduleView_Loaded;
        }

        private void ScheduleView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.RefreshSchedule();
        }

        // Кнопка закриття
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Кнопка мінімізації
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Якщо в тебе є TextBox для пошуку — обробляємо зміни
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is ScheduleViewViewModel vm)
            {
                vm.SearchText = ((TextBox)sender).Text;
            }
        }

        // Кнопка "Сьогодні"
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScheduleViewViewModel vm)
            {
                vm.TodayCommand.Execute(null);
            }
        }

        // Кнопка "Наступний період"
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScheduleViewViewModel vm)
            {
                vm.NextPeriodCommand.Execute(null);
            }
        }

        // Кнопка "Попередній період"
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScheduleViewViewModel vm)
            {
                vm.PreviousPeriodCommand.Execute(null);
            }
        }

        // Кнопка "Тиждень / День"
        private void btnToggleView_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScheduleViewViewModel vm)
            {
                vm.ToggleViewCommand.Execute(null);
            }
        }

        // Кнопка "Деталі"
        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScheduleViewViewModel vm && vm.ShowDetailsCommand.CanExecute(null))
            {
                vm.ShowDetailsCommand.Execute(null);
            }
        }

        // Кнопка "Оновити розклад"
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScheduleViewViewModel vm)
            {
                vm.RefreshSchedule();
            }
        }
    }
}
