using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        private string _dataFile = "students.json";
        public ObservableCollection<Student> Students { get; set; }

        private Student _selectedStudent;
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (_selectedStudent != value)
                {
                    _selectedStudent = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedStudent));
                    UpdateVisibility();
                    UpdateFields(); // Обновляем поля при смене студента
                }
            }
        }

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

            lstStudents.SelectionChanged += (s, e) =>
            {
                if (lstStudents.SelectedItem is Student student)
                {
                    SelectedStudent = student;
                }
            };

            this.Closed += MainWindow_Closed;
        }

        private void UpdateVisibility()
        {
            if (SelectedStudent != null)
            {
                pnlDetails.Visibility = Visibility.Visible;
                txtNoSelection.Visibility = Visibility.Collapsed;
            }
            else
            {
                pnlDetails.Visibility = Visibility.Collapsed;
                txtNoSelection.Visibility = Visibility.Visible;
            }
        }

        // Обновляем поля вручную при смене студента
        private void UpdateFields()
        {
            if (SelectedStudent != null)
            {
                txtFirstName.Text = SelectedStudent.FirstName;
                txtLastName.Text = SelectedStudent.LastName;
                txtAge.Text = SelectedStudent.Age.ToString();
                txtGroup.Text = SelectedStudent.Group;

                txtStatus.Visibility = Visibility.Collapsed;
            }
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            SaveDataToFile();
        }

        // КНОПКА СОХРАНИТЬ ИЗМЕНЕНИЯ
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                return;

            // Обновляем данные из полей
            SelectedStudent.FirstName = txtFirstName.Text;
            SelectedStudent.LastName = txtLastName.Text;

            if (int.TryParse(txtAge.Text, out int age))
            {
                SelectedStudent.Age = age;
            }

            SelectedStudent.Group = txtGroup.Text;

            // Показываем сообщение
            txtStatus.Text = "✓ Изменения сохранены!";
            txtStatus.Visibility = Visibility.Visible;

            // Сохраняем в файл
            SaveDataToFile();

            // Обновляем список
            lstStudents.Items.Refresh();
        }

        private void SaveDataToFile()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Students, Formatting.Indented);
                File.WriteAllText(_dataFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewName.Text) ||
                string.IsNullOrWhiteSpace(txtNewSurname.Text))
            {
                MessageBox.Show("Введите имя и фамилию");
                return;
            }

            var student = new Student
            {
                FirstName = txtNewName.Text,
                LastName = txtNewSurname.Text,
                Age = 18,
                Group = "Новая группа"
            };

            Students.Add(student);
            SelectedStudent = student;

            txtNewName.Clear();
            txtNewSurname.Clear();

            SaveDataToFile();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent != null)
            {
                if (MessageBox.Show($"Удалить {SelectedStudent.FullName}?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Students.Remove(SelectedStudent);
                    SelectedStudent = null;
                    SaveDataToFile();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Student : INotifyPropertyChanged
    {
        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        private string _lastName = "";
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        public string FullName => $"{LastName} {FirstName}";

        public int Age { get; set; }
        public string Group { get; set; } = "";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}