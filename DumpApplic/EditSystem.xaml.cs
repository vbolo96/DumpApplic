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
    /// Interaction logic for EditSystem.xaml
    /// </summary>
    public partial class EditSystem : Window
    {
        SqlConnection sqlConnection;
        string systemToModify;
        public EditSystem()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.systemNameBox.Text = systemToModify;
            //string connectionString = ConfigurationManager.ConnectionStrings["DumpApplic.Properties.Settings.DumpAppConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(Helper.CnnValue(Helper.database));
        }

        public void getSystemToModify(string systemName)
        {
            systemToModify = systemName;
        }
        private void editSystem_Click(object sender, RoutedEventArgs e)
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
            reader.Close();sqlConnection.Close();
        
            if (duplicateSystem == "")//checking if system we want to add is the same we are editing
            {
                Patterns patterns = new Patterns();
                bool allgood = true;
                if (patterns.matchSystem(systemNameBox.Text.ToString()) == false)//checking text using regular expressions
                {
                    MessageBox.Show("System name should contain only letters and digits!");
                    allgood = false;
                }

                if (allgood == true)
                {
                    try
                    {
                        string query = "update Systems set Name=@sysName where Name=@modifySystem";
                        command = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        command.Parameters.AddWithValue("@sysName", systemNameBox.Text);
                        command.Parameters.AddWithValue("@modifySystem", systemToModify);
                        command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        sqlConnection.Close();
                        MessageBox.Show("System updated successfully!!");
                        this.Close();
                       
                    }
                }
                
            }else
            {
                MessageBox.Show("This system already exists. Change the name or cancel editing!");
            }
        }

        private void CancelEditSystem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
