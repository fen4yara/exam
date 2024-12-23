using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace exam
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DateTime StartTime { get; private set; }
        public List<int> Temperatures { get; private set; } = new List<int>();
        public string FishType { get; private set; }
        public int MaxTemperature { get; private set; }
        public int MaxDuration { get; private set; }
        public int MinTemperature { get; private set; }
        public int MinDuration { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Length < 2)
                    {
                        throw new FormatException("Файл должен содержать хотя бы две строки: время начала и значения температуры.");
                    }

                    // Парсинг времени начала
                    if (!DateTime.TryParse(lines[0], out DateTime startTime))
                    {
                        throw new FormatException("Неверный формат даты/времени в первой строке.");
                    }
                    StartTime = startTime;

                    // Парсинг температур
                    Temperatures.Clear();
                    var tempValues = lines[1].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var temp in tempValues)
                    {
                        if (int.TryParse(temp, out int temperature))
                        {
                            Temperatures.Add(temperature);
                        }
                        else
                        {
                            throw new FormatException($"Неверное значение температуры: {temp}");
                        }
                    }

                    MessageBox.Show("Данные успешно загружены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AnalyzeData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Temperatures.Count == 0)
                {
                    throw new InvalidOperationException("Нет данных о температуре для анализа.");
                }

                if (!int.TryParse(MaxTempInput.Text, out int maxTemp))
                {
                    throw new FormatException("Неверное значение максимальной температуры.");
                }
                MaxTemperature = maxTemp;

                if (!int.TryParse(MaxDurationInput.Text, out int maxDuration))
                {
                    throw new FormatException("Неверное значение максимальной длительности.");
                }
                MaxDuration = maxDuration;

                if (!int.TryParse(MinTempInput.Text, out int minTemp))
                {
                    throw new FormatException("Неверное значение минимальной температуры.");
                }
                MinTemperature = minTemp;

                if (!int.TryParse(MinDurationInput.Text, out int minDuration))
                {
                    throw new FormatException("Неверное значение минимальной длительности.");
                }
                MinDuration = minDuration;

                var report = new List<string>();
                DateTime currentTime = StartTime;
                int maxTempExceedTime = 0;
                int minTempExceedTime = 0;

                foreach (var temp in Temperatures)
                {
                    if (temp > MaxTemperature)
                    {
                        report.Add($"{currentTime}: Превышена максимальная температура {MaxTemperature}°C. Фактическая: {temp}°C");
                        maxTempExceedTime += 10;
                        if (maxTempExceedTime > MaxDuration)
                        {
                            report.Add($"Порог минимально допустимой температуры превышен на {maxTempExceedTime} минут.");
                        }
                    }
                    else
                    {
                        maxTempExceedTime = 0;
                    }

                    if (temp < MinTemperature)
                    {
                        report.Add($"{currentTime}: Температура опустилась ниже минимальной {MinTemperature}°C. Фактическая: {temp}°C");
                        minTempExceedTime += 10;
                        if (minTempExceedTime > MinDuration)
                        {
                            report.Add($"Порог минимально допустимой температуры превышен на {minTempExceedTime} минут.");
                        }
                    }
                    else
                    {
                        minTempExceedTime = 0;
                    }

                    currentTime = currentTime.AddMinutes(10);
                }

                if (report.Count == 0)
                {
                    MessageBox.Show("Условия хранения соблюдены на протяжении всего транспортировки.", "Результат анализа", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var reportPath = "C:\\Users\\202217\\source\\repos\\exam\\exam\\TemperatureReport.txt";
                    File.WriteAllLines(reportPath, report);
                    MessageBox.Show($"Условия хранения были нарушены. Отчет сохранен в {reportPath}", "Результат анализа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка во время анализа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManualInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FishType = FishTypeInput.Text;
                if (string.IsNullOrWhiteSpace(FishType))
                {
                    throw new FormatException("Тип рыбы не может быть пустым.");
                }

                if (!DateTime.TryParse(StartTimeInput.Text, out DateTime startTime))
                {
                    throw new FormatException("Неверный формат даты/времени.");
                }
                StartTime = startTime;

                var tempValues = TemperaturesInput.Text.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                Temperatures.Clear();
                foreach (var temp in tempValues)
                {
                    if (int.TryParse(temp, out int temperature))
                    {
                        Temperatures.Add(temperature);
                    }
                    else
                    {
                        throw new FormatException($"Неверное значение температуры: {temp}");
                    }
                }

                MessageBox.Show("Данные успешно введены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при ручном вводе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
