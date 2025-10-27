using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace Planora.ViewModels.ViewModels
{
    public class GroupManagementViewModel : ViewModelBase
    {
        public GroupManagementViewModel()
        {
            Groups = new ObservableCollection<GroupItem>
            {
                new GroupItem { Name = "КН-101", Faculty = "Комп'ютерні науки", Course = 1, StudentCount = 30 },
                new GroupItem { Name = "КН-202", Faculty = "Комп'ютерні науки", Course = 2, StudentCount = 25 }
            };

            AddGroupCommand = new RelayCommand(ExecuteAddGroup);
            EditGroupCommand = new RelayCommand(ExecuteEditGroup);
            DeleteGroupCommand = new RelayCommand(ExecuteDeleteGroup);
        }

        public ObservableCollection<GroupItem> Groups { get; }
        public RelayCommand AddGroupCommand { get; }
        public RelayCommand EditGroupCommand { get; }
        public RelayCommand DeleteGroupCommand { get; }

        private void ExecuteAddGroup(object parameter) => System.Diagnostics.Debug.WriteLine("Додати групу");
        private void ExecuteEditGroup(object parameter) => System.Diagnostics.Debug.WriteLine("Редагувати групу");
        private void ExecuteDeleteGroup(object parameter) => System.Diagnostics.Debug.WriteLine("Видалити групу");
    }

    public class GroupItem : ViewModelBase
    {
        public string Name { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public int Course { get; set; }
        public int StudentCount { get; set; }
    }
}