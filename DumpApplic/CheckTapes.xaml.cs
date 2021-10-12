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
    /// Interaction logic for CheckTapes.xaml
    /// </summary>
    public partial class CheckTapes : Window
    {
        SqlConnection sqlConnection;
        public CheckTapes()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //string connectionString = ConfigurationManager.ConnectionStrings["DumpApplic.Properties.Settings.DumpAppConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(Helper.CnnValue(Helper.database));//setting sqlconnection using the connection string
            InitializeComponent();
        }

        private void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> labelsList = new List<string>();//labels contains the barcode of the tapes
            List<TextBox> tbList = new List<TextBox>();//creating a list with all the textboxes we use to check the tapes
            foreach (UIElement element in TapesToCheck.Children)
            {
                if (element is TextBox)
                {
                    tbList.Add(element as TextBox);
                }
            };
            foreach (UIElement element in TapesToCheck.Children)
            {
                if (element is Label)
                {
                    if ((element as Label).Content.ToString() != "")
                    {
                        labelsList.Add((element as Label).Content.ToString());
                    }
                }
            };
            int goodtapes = 1;//assuming we checked the correct tapes
            foreach (TextBox item in tbList)
            {
                if (labelsList[tbList.IndexOf(item)] != item.Text)
                {
                    goodtapes = 0;//if one of the tapes is not checked correctly we break
                    break;
                }              
            }
            if (goodtapes == 0)//checking if tapes are not correct
                MessageBox.Show("Wrong scanned tapes!");
            else
            {
                foreach (TextBox item in tbList)
                {
                    try
                    {
                        string query = "select Details from Tapes where BarCode=@tape";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlCommand.Parameters.AddWithValue("@tape", item.Text);
                        sqlConnection.Open();
                        string detail = sqlCommand.ExecuteScalar().ToString();
                        //for every checked tape we get the details and insert them in history table with current date and status checked 1
                        query = "insert into History values (@code,@details,@date,@time,@checked)";
                        sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlCommand.Parameters.AddWithValue("@code", item.Text);
                        sqlCommand.Parameters.AddWithValue("@details", detail);
                        sqlCommand.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                        sqlCommand.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                        sqlCommand.Parameters.AddWithValue("@checked", 1);
                        sqlCommand.ExecuteScalar();
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
                MessageBox.Show("Tapes checked successfully!");
                this.Close();
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();         
        }
    }
}
