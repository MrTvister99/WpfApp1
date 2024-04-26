using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для AddPage.xaml
    /// </summary>
    public partial class AddPage : Page
    {
        public int i;
        private string usersFio;
        
        public string Name1;
        string server = "DESKTOP-9MU0DUB";
        string database = "practic_work";
        string username = "MrTv";
        string passwordDB = "1";
        public AddPage()
        {
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Telephone.Text != "" && DescriptionProblemTextBox.Text != "" && ClientNameTextBox.Text != "") {
                using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
                {

                    connection.Open();

                    string sql = "INSERT INTO Orders (Telephone,Date_add, Product, typeFault, OverviewProblem, NameClient, Status) VALUES (@Telephone,@Date_add, @equipment, @type_of_fault, @Problem, @Users_FIO, @Status)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Telephone", Telephone.Text);
                        command.Parameters.AddWithValue("@Date_add", DateTime.Now);
                        command.Parameters.AddWithValue("@equipment", ProductBox.Text);
                        command.Parameters.AddWithValue("@type_of_fault", typeFault.Text);
                        command.Parameters.AddWithValue("@Problem", DescriptionProblemTextBox.Text);
                        command.Parameters.AddWithValue("@Users_FIO", ClientNameTextBox.Text);
                        command.Parameters.AddWithValue("@Status", "В работе");

                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                MessageBox.Show($"Поля не должны быть пустыми");
            }
           
        }
       
        }
    }

