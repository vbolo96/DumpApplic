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
    /// Interaction logic for EditTape.xaml
    /// </summary>
    public partial class EditTape : Window
    {
        SqlConnection sqlConnection;
        string tapeToModify;
        public EditTape()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //string connectionString = ConfigurationManager.ConnectionStrings["DumpApplic.Properties.Settings.DumpAppConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(Helper.CnnValue(Helper.database));
            
        }

        public void initializeEditWindow(string tape)
        {
            try
            {
                //adding comboboxes items
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
                sqlCommand = new SqlCommand(query, sqlConnection);
                reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    string type = reader.GetString(0);
                    BTypeValuesCB.Items.Add(type);
                }
                reader.Close();
                tapeToModify = tape;
                
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
        private void editTape_Click(object sender, RoutedEventArgs e)
        {
           //check if the tape we want to add is already in the database
            string duplicateTape = "";
            string duplicateDetails = "";
            string system = "";
            string backuptype = "";
            string duplicatesQuery = "select barCode,System,BackupType,Details from Tapes where barCode=@code";
           
            SqlCommand command = new SqlCommand(duplicatesQuery, sqlConnection);
            command.Parameters.AddWithValue("@code",barCodeBox.Text);
            sqlConnection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                duplicateTape = reader.GetString(0);
                system = reader.GetString(1);
                backuptype = reader.GetString(2);
                duplicateDetails = reader.GetString(3);
            }
            reader.Close();
            sqlConnection.Close();

            //if tape does not exist or if exists but we want to change other fields 
            if (duplicateTape == "" || ((duplicateDetails!=detailsBox.Text || system!=SystemValuesCB.SelectedItem.ToString() || backuptype!=BTypeValuesCB.SelectedItem.ToString()) && duplicateTape==barCodeBox.Text))
            {

                
                Patterns patterns = new Patterns();//new patterns objects to check if all fields are completed correctly by the operator
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
                        //query to update the selected tape with the new values
                        string query = "update Tapes set barCode=@code,System=@sys,BackupType=@type,Details=@details where barCode=@modifytape";
                        command = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        command.Parameters.AddWithValue("@code", barCodeBox.Text);
                        command.Parameters.AddWithValue("@sys", SystemValuesCB.SelectedItem);
                        command.Parameters.AddWithValue("@type", BTypeValuesCB.SelectedItem);
                        command.Parameters.AddWithValue("@details", detailsBox.Text);
                        command.Parameters.AddWithValue("@modifytape", tapeToModify);
                        command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        sqlConnection.Close();
                        MessageBox.Show("Tape updated successfully!!");
                        this.Close();

                    }
                }
                else
                {
                    //MessageBox.Show("BarCode pattern or details wrong!");
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("The tape you want to add already exists!");
            }

        }
        private void CancelEditTape_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
