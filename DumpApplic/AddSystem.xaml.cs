using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace DumpApplic
{
    /// <summary>
    /// Interaction logic for AddSystem.xaml
    /// </summary>
    /// 
    
    public partial class AddSystem : Window
    {
        SqlConnection sqlConnection;

        public AddSystem()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //string connectionString = ConfigurationManager.ConnectionStrings["DumpApplic.Properties.Settings.DumpAppConnectionString"].ConnectionString;
            sqlConnection= new SqlConnection(Helper.CnnValue(Helper.database));
        }

        private void addSystem_Click(object sender, RoutedEventArgs e)
        {
            string querySystem = "select Name from Systems where Name=@sysName";
            string duplicateSystem = "";
            SqlCommand command = new SqlCommand(querySystem, sqlConnection);
            command.Parameters.AddWithValue("@sysName", systemNameBox.Text);
            sqlConnection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                duplicateSystem = reader.GetString(0);
            }
            reader.Close(); sqlConnection.Close();

            if (duplicateSystem == "")//checking if system we want to add already exists
            {
                Patterns patterns = new Patterns();
                bool allgood = true;
                if (patterns.matchTape(systemNameBox.Text.ToString()) == false)//checking text using regular expressions
                {
                    MessageBox.Show("System name should contain only letters and digits!");
                    allgood = false;
                }

                if (allgood == true)
                {
                    try
                    {
                        //inserting a new system in Systems table
                        string query = "insert into Systems values (@name)";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@name", systemNameBox.Text);
                        sqlCommand.ExecuteScalar();
                    }
                    catch (Exception exe)
                    {
                        MessageBox.Show(exe.ToString());
                    }
                    finally
                    {
                        sqlConnection.Close();
                        MessageBox.Show("System inserted succesfully!");
                        this.Close();
                    }
                }

            }
            else
            {
                MessageBox.Show("This system already exists. Change the name or cancel editing!");
            }
        }

        private void CancelAddSystem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
