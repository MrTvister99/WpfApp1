using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        public bool found = false;
        public string login;
        public string password;
        public static string FIO1;
        string server = "DESKTOP-9MU0DUB";
        string database = "practic_work";
        string username = "MrTv";
        string passwordDB = "1";
        public Registration()
        {
            InitializeComponent();
        }
        private void Enter_Click1(object sender, RoutedEventArgs e)
        {
            podkl();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            podkl1();


        }
        private void podkl()
        {


            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                login = email.Text;
                password = Password.Text;
                

                connection.Open();



                string sql = $"INSERT INTO clients (email,password,Role_client)  VALUES (@email,@password,@Role_client)";
                if (Password.Text != "" && email.Text != "")
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", $"{login}");
                        
                        command.Parameters.AddWithValue("@password", $"{password}");
                       
                        command.Parameters.AddWithValue("@Role_client",$"1");
                        NavigationService.Navigate(new AddPage());

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    //frame.Navigate(new Menu());
                }
                else
                {
                    MessageBox.Show($"Поля не могут быть пустыми");

                }
            }

        }
        private void podkl1()
        {
            NavigationService.Navigate(new RegistrEnter());

        }
    }
}

