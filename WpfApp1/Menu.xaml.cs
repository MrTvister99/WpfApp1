using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using System.Xml.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class Menu : System.Windows.Controls.Page
    {
        public int i;
        private string usersFio;
        private ObservableCollection<application> TowarList = new ObservableCollection<application>();
        public string Name1;
        string server = "DESKTOP-9MU0DUB";
        string database = "practic_work";
        string username = "MrTv";
        string passwordDB = "1";
        List<string> values;
        public Menu()
        {
            InitializeComponent();
            podkl();
            myTextBox1.TextChanged += myTextBox1_TextChanged;
            //ComboB();






        }
        public void ExportToExcel(object sender, RoutedEventArgs e)
        {
           

            // Создание нового Excel-файла
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;

                // Заголовки столбцов
                worksheet.Cell(currentRow, 1).Value = "Telephone";
                worksheet.Cell(currentRow, 2).Value = "Date_add";
                worksheet.Cell(currentRow, 3).Value = "equipment";
                worksheet.Cell(currentRow, 4).Value = "type_of_fault";
                worksheet.Cell(currentRow, 5).Value = "Problem";
                worksheet.Cell(currentRow, 6).Value = "Users_FIO";
                worksheet.Cell(currentRow, 7).Value = "Status";

                foreach (var item in TowarList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Telephone;
                    worksheet.Cell(currentRow, 2).Value = item.Date_add;
                    worksheet.Cell(currentRow, 3).Value = item.equipment;
                    worksheet.Cell(currentRow, 4).Value = item.type_of_fault;
                    worksheet.Cell(currentRow, 5).Value = item.Problem;
                    worksheet.Cell(currentRow, 6).Value = item.Users_FIO;
                    worksheet.Cell(currentRow, 7).Value = item.Status;
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string filePath = System.IO.Path.Combine(desktopPath, "Orders.xlsx");
                workbook.SaveAs(filePath);
                Console.WriteLine($"Данные экспортированы в файл: {filePath}");
            }
        }
    
    private void podkl()
        {

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "SELECT * FROM Orders";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TowarList.Add(new application
                            {
                                Telephone = reader["Telephone"].ToString(),
                                Date_add = DateTime.Parse(reader["Date_add"].ToString()).ToString("dd.MM.yyyy"),
                            equipment = reader["Product"].ToString(),
                                type_of_fault = reader["typeFault"].ToString(),
                                Problem = reader["OverviewProblem"].ToString(),
                                Users_FIO = reader["NameClient"].ToString(),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            TowarListView.ItemsSource = TowarList;

            // Получение уникальных значений equipment
            var uniqueEquipments = TowarList.Select(t => t.equipment).Distinct().ToList();
            uniqueEquipments.Insert(0, "Все");
            // Установка источника данных для ComboBox
            comboBoxEquipment.ItemsSource = uniqueEquipments;

            // Обработчик события SelectionChanged для ComboBox
            comboBoxEquipment.SelectionChanged += ComboBoxEquipment_SelectionChanged;
        }
        private void ComboBoxEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получение выбранного значения из ComboBox
            string selectedEquipment = comboBoxEquipment.SelectedItem as string;

            // Если выбран "Все", возвращаем полный список
            if (selectedEquipment == "Все")
            {
                TowarListView.ItemsSource = TowarList;
            }
            else
            {
                // Фильтрация списка по выбранному оборудованию
                var filteredList = TowarList.Where(t => t.equipment == selectedEquipment).ToList();

                // Обновление источника данных для ListView
                TowarListView.ItemsSource = filteredList;
            }
        }
        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный элемент из списка
            application selectedApplication = TowarListView.SelectedItem as application;

            if (selectedApplication != null)
            {
                // Обновляем статус в базе данных
                UpdateStatusInDatabase(selectedApplication);

                // Обновляем статус в списке
                selectedApplication.Status = "Выполненно"; // Замените "Новый статус" на желаемый статус

                // Обновляем источник данных для обновления UI
                TowarListView.Items.Refresh();
            }
        }
        private void UpdateStatusInDatabase(application selectedApplication)
        {
            string newStatus = "Выполненно";

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "UPDATE Orders SET Status = @Status WHERE NameClient = @NameClient";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Status", newStatus);
                    command.Parameters.AddWithValue("@NameClient", selectedApplication.Users_FIO);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void AddApplication(application newApplication)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                
                connection.Open();

                string sql = "INSERT INTO application (Date_add, equipment, type_of_fault, Problem, Users_FIO, Status) VALUES (@Date_add, @equipment, @type_of_fault, @Problem, @Users_FIO, @Status)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Date_add", newApplication.Date_add);
                    command.Parameters.AddWithValue("@equipment", newApplication.equipment);
                    command.Parameters.AddWithValue("@type_of_fault", newApplication.type_of_fault);
                    command.Parameters.AddWithValue("@Problem", newApplication.Problem);
                    command.Parameters.AddWithValue("@Users_FIO", newApplication.Users_FIO);
                    command.Parameters.AddWithValue("@Status", newApplication.Status);

                    command.ExecuteNonQuery();
                }
            }
            TowarListView.ItemsSource = TowarList;
        }
        private void OldCheckBox_Checked(object sender, RoutedEventArgs e)
        {
           
            ApplyFilter(true);
        }

        private void OldCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyFilter(false);
        }

        private void NewCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
            ApplyFilter(false);
        }

        private void NewCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyFilter(true);
        }

        private void ApplyFilter(bool isOld)
        {
            if (isOld)
            {
                var sortedList = new ObservableCollection<application>(TowarList.OrderByDescending(a => DateTime.Parse(a.Date_add)));
                TowarListView.ItemsSource = sortedList;
            }
            else
            {
                var sortedList = new ObservableCollection<application>(TowarList.OrderBy(a => DateTime.Parse(a.Date_add)));
                TowarListView.ItemsSource = sortedList;
            }
        }
        public void OpenApplicationWindowAndProcessResult(object sender, RoutedEventArgs e)
        {

            WindowAdd applicationWindow = new WindowAdd();

            // Отображаем окно
            if (applicationWindow.ShowDialog() == applicationWindow.DialogResult)
            {
                {

                    application newApplication = new application
                    {
                        Date_add = DateTime.Now.ToString(),
                        Users_FIO = applicationWindow.FIO.Text,
                        equipment = applicationWindow.Name_Product.Text,
                        type_of_fault = applicationWindow.Fault.Text,
                        Problem = applicationWindow.Problem.Text,
                        Status="В работе"
                    };

                   
                    AddApplication(newApplication);
                }
            }
        }
        private void Button_Click4(object sender, RoutedEventArgs e)
        {


            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                string selectedItemText = TowarListView.SelectedItem as string;
                connection.Open();

                if (!string.IsNullOrEmpty(selectedItemText))
                {
                    string sql = $"INSERT INTO Korzina (product)  VALUES (@product)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@product", $"{selectedItemText}");
                        command.ExecuteNonQuery();
                        Button button = sender as Button;
                        button.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    MessageBox.Show("Выбранный товар не может быть пустым");
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            // frame.Navigate(new Korzina());
        }


       
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
            if (TowarListView.SelectedItem is application selectedApplication)
            {
              
                if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Удаляем запись из базы данных и из списка
                    RemoveApplication(selectedApplication);
                }
            }
        }

        private void RemoveApplication(application app)
        {
            // Удаляем запись из базы данных
            DeleteApplicationFromDatabase(app);

            // Удаляем запись из списка
            TowarList.Remove(app);

            // Обновляем источник данных для обновления списка в интерфейсе
            TowarListView.ItemsSource = null;
            TowarListView.ItemsSource = TowarList;
        }

        private void DeleteApplicationFromDatabase(application app)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                // Предполагается, что в базе данных есть поле, которое можно использовать для идентификации записи
                // Например, если у вас есть поле "Id" в таблице "application", то вы можете использовать его для удаления
                string sql = "DELETE FROM Orders WHERE NameClient  = @FIO";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FIO", app.Users_FIO); 
                    command.ExecuteNonQuery();
                }
            }
        }
        private void myTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = myTextBox1.Text;

            var filteredItems = TowarList.Where(item => item.Users_FIO.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            TowarListView.ItemsSource = filteredItems;
        }
        private void ComboB()
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "SELECT * FROM Group1";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        List<string> values = new List<string>();
                        ComboBoxData.ClearItems(values);
                        while (reader.Read())
                        {
                            string Name2 = reader["Predmet"].ToString();
                            if (!values.Contains(Name2))
                            {
                                values.Add(Name2);
                                ComboBoxData.AddItems(values);
                            }
                        }
                    }
                    connection.Close();
                }
            }
        }
        private void comboBox1_SelectionChanged(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                var comboBox = (ComboBox)sender;
                var selectedItem = comboBox.SelectedItem;
                connection.Open();



                string sql = $"INSERT INTO Group1 (Predmet,Name)  VALUES (@Predmet,@Name)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    ComboBox comboBox1 = (ComboBox)sender;
                    int index = TowarListView.Items.IndexOf(comboBox.DataContext);
                    string listName = comboBox.DataContext.ToString();

                    MessageBox.Show($"Selected item: {selectedItem}, index: {listName}");
                    if (selectedItem != null)
                    {
                        command.Parameters.AddWithValue("@Predmet", $"{selectedItem}");
                        command.Parameters.AddWithValue("@Name", $"{listName}");



                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
                //frame.Navigate(new Menu());




            }
        }

        private void TowarListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //  ComboBox comboBox = (ComboBox)e.AddedItems[0].FindName("Pre");
            // ...
        }
        // ...
        public class application
        {
            public string Telephone { get; set; }
            public string Date_add { get; set; }
            public string equipment { get; set; }
            public string type_of_fault { get; set; }
            public string Problem { get; set; }
            public string Users_FIO { get; set; }
            public string Status { get; set; }

        }
        public static class ComboBoxData
        {

            public static ObservableCollection<string> Items { get; } = new ObservableCollection<string>
            {

            };

            public static void AddItems(List<string> items)
            {
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }

            public static void ClearItems(List<string> items)
            {
                Items.Clear();
            }

        }
    }
}




