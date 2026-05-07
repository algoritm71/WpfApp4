using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelection));
            }
        }

        public bool HasSelection => SelectedStudent != null;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Students = new ObservableCollection<Student>
            {
                new Student { FirstName = "Иван", LastName = "Иванов", Age = 20, Group = "ИС-101" },
                new Student { FirstName = "Анна", LastName = "Петрова", Age = 21, Group = "ИС-102" },
                new Student { FirstName = "Сергей", LastName = "Сидоров", Age = 19, Group = "ПР-201" }
            };
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNewName.Text.Trim();
            string surname = txtNewSurname.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
            {
                MessageBox.Show("Введите Имя и Фамилию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Student newStudent = new Student
            {
                FirstName = name,
                LastName = surname,
                Age = 18,
                Group = "Новичок"
            };

            Students.Add(newStudent);
            SelectedStudent = newStudent;

            txtNewName.Clear();
            txtNewSurname.Clear();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Выберите студента для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show($"Удалить студента {SelectedStudent.FullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Students.Remove(SelectedStudent);
                SelectedStudent = null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Student : INotifyPropertyChanged
    {
        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        public string FullName => $"{LastName} {FirstName}";

        private int _age;
        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }

        private string _group = string.Empty;
        public string Group
        {
            get => _group;
            set { _group = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}