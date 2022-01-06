using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DumpApplic
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        SqlConnection sqlconnection;
        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            AppDomain.CurrentDomain.SetData("DataDirectory", projectDirectory);
            //string connectionString = ConfigurationManager.ConnectionStrings["DumpApplic.Properties.Settings.DumpAppConnectionString"].ConnectionString;
            sqlconnection = new SqlConnection(Helper.CnnValue(Helper.database));

        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Patterns patterns = new Patterns();
            if (patterns.matchUsername(UserTB.Text) && !PasswordTB.Password.Equals(""))
            {
                string query = "select Hash from Users where Name=@user";
                sqlconnection.Open();
                SqlCommand sqlcommand = new SqlCommand(query, sqlconnection);
                sqlcommand.Parameters.AddWithValue("@user", UserTB.Text);
                string pwd = "";
                try
                {
                    pwd = sqlcommand.ExecuteScalar().ToString();

                    if (pwd != "")
                    {
                        if (pwd == ComputeSha256Hash(PasswordTB.Password))
                        {
                            MainWindow mw = new MainWindow();
                            mw.Show();
                            this.Close();
                        }
                        else
                            MessageBox.Show("Wrong password!");
                    }
                    else
                    {
                        MessageBox.Show("Wrong Username!");
                    }
                }
                catch (Exception exe)
                { MessageBox.Show(exe.ToString()); }
                finally { sqlconnection.Close(); }
            }
            else
            {
                MessageBox.Show("Username and password cannot be empty and username must have 5 characters!");
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString().ToUpper();
            }
        }

        private void LoginBtn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginBtn_Click(sender, e);
            }
        }
    }
}
