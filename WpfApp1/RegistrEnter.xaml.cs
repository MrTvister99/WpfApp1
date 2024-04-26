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
    /// Логика взаимодействия для RegistrEnter.xaml
    /// </summary>
    public partial class RegistrEnter : Page
    {
        string server = "DESKTOP-9MU0DUB";
        string database = "practic_work";
        string username = "MrTv";
        string passwordDB = "1";
        public bool found;


        public string email1;
        public string password1;
        public static string FIO1;
        private string roleClient;
        public RegistrEnter()
        {
            InitializeComponent();
        }
        private void podkl()
        {

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "SELECT * FROM clients";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        bool found = false;
                        while (reader.Read())
                        {
                            email1 = reader["email"].ToString();
                            password1 = reader["password"].ToString();
                           // password1 = reader["FIO"].ToString();
                            password1 = password1.Trim();
                            email1 = email1.Trim();
                            //FIO1 = FIO1.Trim();

                            if (password1 == Password.Text && email.Text == email1)
                            {
                                found = true;
                               roleClient = reader["Role_client"].ToString();
                            }
                        }

                        if (Password.Text != "" && email.Text != "")
                        {
                            if (found)
                            {
                                
                                if (roleClient=="2")
                                {
                                    NavigationService.Navigate(new Menu());
                                }
                                else
                                {
                                    NavigationService.Navigate(new AddPage());
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Неправильный логин или пароль");
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Поля не могут быть пустыми");
                        }
                    }
                }
            }
        }
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            podkl();
        }

       
    }
}
