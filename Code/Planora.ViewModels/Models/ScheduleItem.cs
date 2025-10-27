using Planora.ViewModels.Base;

namespace Planora.ViewModels.Models
{
  public class ScheduleItem : ViewModelBase
  {
    private string _day = string.Empty;
    private DateTime _date;
    private string _time = string.Empty;
    private string _subject = string.Empty;
    private string _teacher = string.Empty;
    private string _classroom = string.Empty;
    private string _type = string.Empty;
    private string _group = string.Empty;

    public string Day
    {
      get => _day;
      set => SetProperty(ref _day, value);
    }

    public DateTime Date
    {
      get => _date;
      set => SetProperty(ref _date, value);
    }

    public string Time
    {
      get => _time;
      set => SetProperty(ref _time, value);
    }

    public string Subject
    {
      get => _subject;
      set => SetProperty(ref _subject, value);
    }

    public string Teacher
    {
      get => _teacher;
      set => SetProperty(ref _teacher, value);
    }

    public string Classroom
    {
      get => _classroom;
      set => SetProperty(ref _classroom, value);
    }

    public string Type
    {
      get => _type;
      set => SetProperty(ref _type, value);
    }

    public string Group
    {
      get => _group;
      set => SetProperty(ref _group, value);
    }

    public string FullInfo => $"{Subject} ({Type})\n{Teacher}\n{Classroom}\n{Time}";

    public bool HasComputersRequirement => Type == "Лабораторна" || Subject == "Програмування";
    public bool HasProjectorRequirement => Type == "Лекція";
  }
}