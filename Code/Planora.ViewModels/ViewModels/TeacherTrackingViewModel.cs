using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Planora.ViewModels.ViewModels
{
    public class TeacherDto
    {
        public string Name { get; set; } = "";
        public string Department { get; set; } = ""; // Кафедра
        public string CurrentStatus { get; set; } = ""; // "Вільний" або "Пара: Математика"
        public string Location { get; set; } = ""; // "Кафедра" або "101-A"
        public bool IsBusy { get; set; } // Для кольору (червоний/зелений)
        public string NextLessonTime { get; set; } = ""; // "12:30"
    }

    public class TeacherTrackingViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private List<TeacherDto> _allTeachers = new();

        public TeacherTrackingViewModel()
        {
            Teachers = new ObservableCollection<TeacherDto>();
            InitializeData();
            FilterTeachers();
        }

        public string SearchText
        {
            get => _searchText;
            set 
            { 
                SetProperty(ref _searchText, value); 
                FilterTeachers(); 
            }
        }

        public ObservableCollection<TeacherDto> Teachers { get; }

        private void InitializeData()
        {
            _allTeachers = new List<TeacherDto>
            {
                new TeacherDto { Name = "Малюга Іван Іванович", Department = "КН", CurrentStatus = "Лекція: Математика", Location = "101-A", IsBusy = true, NextLessonTime = "12:15" },
                new TeacherDto { Name = "Петрова Марія Василівна", Department = "КН", CurrentStatus = "Практика: Програмування", Location = "201-B", IsBusy = true, NextLessonTime = "14:00" },
                new TeacherDto { Name = "Стерненко Олег Петрович", Department = "ПЗ", CurrentStatus = "Вільний", Location = "Кафедра 305", IsBusy = false, NextLessonTime = "10:45" },
                new TeacherDto { Name = "Ковальчук Тетяна Сергіївна", Department = "Фізика", CurrentStatus = "Вільна", Location = "Кафедра 102", IsBusy = false, NextLessonTime = "Завтра" },
                new TeacherDto { Name = "Гончарук Василь Андрійович", Department = "Історія", CurrentStatus = "Семінар", Location = "303-C", IsBusy = true, NextLessonTime = "13:00" }
            };
        }

        private void FilterTeachers()
        {
            Teachers.Clear();
            var filtered = _allTeachers.Where(t => 
                string.IsNullOrWhiteSpace(SearchText) || 
                t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                t.Department.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var teacher in filtered)
            {
                Teachers.Add(teacher);
            }
        }
    }
}