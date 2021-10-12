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
using System.Data;

namespace DumpApplic
{
    /// <summary>
    /// Interaction logic for AddTape.xaml
    /// </summary>
    public partial class AddTape : Window
    {
        SqlConnection sqlConnection;
        public AddTape()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //string connectionString = ConfigurationManager.ConnectionStrings["DumpApplic.Properties.Settings.DumpAppConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(Helper.CnnValue(Helper.database));
            InitializeCB();
        }

        public void InitializeCB()
        {
            try
            {
                //initializing combox boxes values with info from database
                string query = "select Name from Systems";
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    string system = reader.GetString(0);
                    SystemValuesCB.Items.Add(system);
                    
                }
                reader.Close();
                query = "select Name from Types";
                sqlCommand= new SqlCommand(query, sqlConnection);
                reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    string type = reader.GetString(0);
                    BTypeValuesCB.Items.Add(type);
                }
                reader.Close();

            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }

        }

        private void CancelAddTape_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddTape_Click(object sender, RoutedEventArgs e)
        {
            //check if the tape we want to add is already in the database
            string duplicateTape = "";       
            string duplicatesQuery = "select barCode,System,BackupType,Details from Tapes where barCode=@code";

            SqlCommand command = new SqlCommand(duplicatesQuery, sqlConnection);
            command.Parameters.AddWithValue("@code", barCodeBox.Text);
            sqlConnection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                duplicateTape = reader.GetString(0);                
            }
            reader.Close();
            sqlConnection.Close();

            //if tape does not existing
            if (duplicateTape == "")
            {
                Patterns patterns = new Patterns();
                bool allgood = true;
                if (patterns.matchTape(barCodeBox.Text.ToString()) == false)
                {
                    MessageBox.Show("Tapes barcode must be like 'RS0045L6'");
                    allgood = false;
                }
                else if ((BTypeValuesCB.SelectedItem.ToString() == "Daily" && patterns.matchDailyTape(detailsBox.Text.ToString()) == false))
                {
                    MessageBox.Show("Daily Tapes must have the details like 'DAILY TAPE 7'");
                    allgood = false;
                }
                else if ((BTypeValuesCB.SelectedItem.ToString() == "Weekly" && patterns.matchWeeklyTape(detailsBox.Text.ToString()) == false))
                {
                    MessageBox.Show("Weekly Tapes must have the details like 'WEEK 3 CAD TAPE 6'");
                    allgood = false;
                }
                else if ((BTypeValuesCB.SelectedItem.ToString() == "Monthly" && patterns.matchMonthlyTape(detailsBox.Text.ToString()) == false))
                {
                    MessageBox.Show("Monthly Tapes must have the details like 'MARCH MONTHEND CAD TAPE 6'");
                    allgood = false;
                }
                else if ((BTypeValuesCB.SelectedItem.ToString() == "Yearly" && patterns.matchYearlyTape(detailsBox.Text.ToString()) == false))
                {
                    MessageBox.Show("Yearly Tapes must have the details like 'YEARLY CAD TAPE 6'");
                    allgood = false;
                }
                else
                {
                    MessageBox.Show("Valid data!");
                    allgood = true;
                }

                if (allgood == true)
                {
                    try
                    {
                        //inserting a new tape in Tapes table
                        string query = "insert into Tapes values (@code,@sys,@type,@details)";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@code", barCodeBox.Text);
                        sqlCommand.Parameters.AddWithValue("@sys", SystemValuesCB.SelectedValue);
                        sqlCommand.Parameters.AddWithValue("@type", BTypeValuesCB.SelectedValue);
                        sqlCommand.Parameters.AddWithValue("@details", detailsBox.Text);
                        sqlCommand.ExecuteScalar();

                    }
                    catch (Exception exe)
                    {
                        MessageBox.Show(exe.ToString());
                    }
                    finally
                    {
                        sqlConnection.Close();
                        MessageBox.Show("Tape inserted succesfully!");
                        this.Close();
                    }
                }
                else
                {
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("The tape you want to add already exists!");
            }
            
        }
    }
}
