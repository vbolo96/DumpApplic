using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DumpApplic
{
    class DataLayer
    {
        static SqlConnection sqlConnection = new SqlConnection(Helper.CnnValue(Helper.database));
        public static List<string> GetDailyBackupDays()
        {
            List<string> dailyBackupDays = new List<string>();//the list with daily backup days
  
            sqlConnection.Open();
            //get days for daily backup from database (ex: if Monday has State 1 in database add it to the list)
            string query = "select Day from Days where State=1";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                dailyBackupDays.Add(reader.GetString(0));
            }
            reader.Close();
            sqlConnection.Close();
            return dailyBackupDays;
        }

        public static string GetWeeklyBackupDay()
        {
            string weeklyBackupDay = "";
            sqlConnection.Open();
            string query = "select Day from Days where Weekly=1"; //getting the weekly backup day marked in table Days column Weekly=1
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                weeklyBackupDay = reader.GetString(0);
            }
            reader.Close();
            sqlConnection.Close();
            return weeklyBackupDay;
        }

        public static List<string> GetMonthlyBackupDate(string weeklyBackupDay)
        {
            DateTime weekly = DateTime.Now.AddDays(0).Date;
            DateTime first = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            List<string> MonthEndBackupDate = new List<string>();

            for (var d = first; d < first.AddDays(7); d = d.AddDays(1))
            {
                if (d.DayOfWeek.ToString() == weeklyBackupDay)
                {
                    MonthEndBackupDate.Add(d.ToLongDateString());//we have found the monthend backup day and add it to the list
                    weekly = d;
                    break;
                }
            }
            sqlConnection.Close();
            return MonthEndBackupDate;
        }

        public static List<string> GetWeeklyBackupDates(DateTime start)
        {
            DateTime weekly = start;
            List<string> weeklyBackupDates = new List<string>();
            DateTime nextMonth = weekly.AddMonths(1);//setting the next month

            if (nextMonth.Month != 1)//if the next month is not january
            {
                while (weekly.Month < nextMonth.Month)//while we are still in the current month
                {
                    weekly = weekly.AddDays(7);// getting all weekly backup days from week to week
                    if (weekly.Month < nextMonth.Month)
                        weeklyBackupDates.Add(weekly.ToLongDateString());// adding weekly backup days to the list
                }
            }
            else// the rest of the cases where month is NOT January
            {
                while (weekly.Month > nextMonth.Month)
                {
                    weekly = weekly.AddDays(7);
                    if (weekly.Month > nextMonth.Month)
                        weeklyBackupDates.Add(weekly.ToLongDateString());
                }
            }

            return weeklyBackupDates;
        }

        public static string GetMonthName(int x)//returning month name using month number given as parameter
        {
            string[] months = new string[13];
            months[1] = "January"; months[2] = "February"; months[3] = "March"; months[4] = "April"; months[5] = "May"; months[6] = "June";
            months[7] = "July"; months[8] = "August"; months[9] = "September"; months[10] = "October"; months[11] = "November"; months[12] = "December";
            return months[x];
        }

        public static DataTable FillDailyTapesTable(string today, string Query, List<string> dailyBackupDays, List<string> weeklyBackupDates)
        {
            sqlConnection.Open();
            DataTable DailyBackupList = new DataTable();//creating new data table to fill the grid view with
            string query = Query;
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@day", (dailyBackupDays.IndexOf(today) + 1).ToString());
            sqlCommand.Parameters.AddWithValue("@week", (weeklyBackupDates.IndexOf(DateTime.Now.AddDays(0).ToLongDateString()) + 1).ToString());
            sqlCommand.Parameters.AddWithValue("@month", GetMonthName(DateTime.Now.AddMonths(-1).Month).ToUpper());
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand);
            using (sqlAdapter)
            {               
                sqlAdapter.Fill(DailyBackupList);
            }
            sqlAdapter.Dispose();
            sqlConnection.Close();

            return DailyBackupList;
        }

        public static DataTable FillHistoryTable()
        {
            
            DataTable HistoryList = new DataTable();//creating new data table to fill the grid view with
         
            sqlConnection.Open();
            string query = "select * from History";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand);
          
            using (sqlAdapter)
            {                 
                sqlAdapter.Fill(HistoryList);
            }
            sqlAdapter.Dispose();
            sqlConnection.Close();
            return HistoryList;
        }


        public static DataTable FillBasicDataTable()
        {
            sqlConnection.Open();
            string query = "select BarCode,System,BackupType,Details from Tapes";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand);
            DataTable tapesList = new DataTable();
            using (sqlAdapter)
            {             
                sqlAdapter.Fill(tapesList);
            }
            sqlAdapter.Dispose();
            sqlConnection.Close();
            return tapesList;
        }

        public static DataTable FillBasicDataSystemsTable()
        {
            sqlConnection.Open();
            string query = "select Name from Systems";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand);
            DataTable systemsList = new DataTable();
            using (sqlAdapter)
            {              
                sqlAdapter.Fill(systemsList);             
            }
            sqlAdapter.Dispose();
            sqlConnection.Close();
            return systemsList;
        }

        public static void UpdateDays(int state, string name)
        {
            string query = "update Days set State=@state where Day=@name";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@state", state);
            command.Parameters.AddWithValue("@name", name);
            try
            {
                sqlConnection.Open();
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }

        }

        public static int[] GetDaysState()
        {
            string query = "select State from Days";//we should have 7 values
            int[] DayStates = new int[7];//array used to store the days check state
            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = sqlCommand.ExecuteReader();

                int ct = 0;
                while (reader.Read())
                {
                    DayStates[ct++] = reader.GetInt32(0);//read every of the 7 values from the wuery one by one and add it to the array
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return DayStates;
        }

        public static List<string> GetHolidays()
        {
            //getting holidays from the table with the same name and mark them as green in the calendar
            List<string> holidays = new List<string>();
            try
            {
                string query = "select Date from Holidays";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    holidays.Add(reader.GetString(0));
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
            return holidays;
        }

        public static void MarkOrUnmarkHoliday(int operation, string selectedDate)
        {
            string query = "";
            if (operation == 1)
            {
                query = "insert into Holidays values (@date)";
            }
            else
            {
                query = "delete from Holidays where Date=@date";
            }

            try
            {
                sqlConnection.Open();                
                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@date", selectedDate);
                command.ExecuteScalar();
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
            finally { sqlConnection.Close(); }
        }

        public static List<string> GetTodayTapes(int type, string today,string system, string week,string month)
        {
            List<string> todayTapes = new List<string>();//creating the list with barcodes of the tapes
            if (type == 1)
            {
                //getting the barcodes of the daily tapes (BackupType should be Daily and Details should be DAILY TAPE 1/2/3/4 etc)
                string query = "select BarCode from Tapes where BackupType='Daily' and Details like 'DAILY%'+@day";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@day", today);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    todayTapes.Add(reader.GetString(0));
                }
                reader.Close();
            }
            else if (type == 2)
            {
                //getting the barcode of daily tapes where BackupType is Weekly and Details are like WEEK 1/2/3/4 ETC..
                string query = "select BarCode from Tapes where BackupType='Weekly' and System=@sys and Details like 'WEEK_'+@week+'%'";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@sys", system);
                sqlCommand.Parameters.AddWithValue("@week", week);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    todayTapes.Add(reader.GetString(0));
                }
                reader.Close();
            }
            else if (type == 3)
            {
                //getting the barcodes of daily tapes if BackupType is Monthly and details like 'MARCH MONTHEND....'
                string query = "select BarCode from Tapes where BackupType='Monthly' and System=@sys and Details like @month+' MONTHEND%'";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@sys", system);
                sqlCommand.Parameters.AddWithValue("@month", month);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    todayTapes.Add(reader.GetString(0));
                }
                reader.Close();
            }
            return todayTapes;
        }

        public static string CheckTapesHistory(string date, string tape)
        {
            string query1 = "select distinct Checked from History where Date=@date and Tape=@tape";
            SqlCommand sqlCommand1 = new SqlCommand(query1, sqlConnection);
            sqlCommand1.Parameters.AddWithValue("@date", date);
            sqlCommand1.Parameters.AddWithValue("@tape", tape);
            string TapesAlreadyChecked = "";
            sqlConnection.Open();
            try
            {
                var obj = sqlCommand1.ExecuteScalar();
                TapesAlreadyChecked = (obj == null ? "" : obj.ToString());
            }
            catch (Exception exe) { MessageBox.Show(exe.ToString()); }
            finally {
                sqlConnection.Close();
            }

            return TapesAlreadyChecked;
        }
       
       

    }
}
