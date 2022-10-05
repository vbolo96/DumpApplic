using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace DumpApplic
{
    public partial class MainWindow : Window
    {       
        SqlConnection sqlConnection;
        Window checkDailyTapes = new Window();
        public MainWindow()
        {
            InitializeComponent();
            
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //string connectionString = ConfigurationManager.ConnectionStrings["DumpApplic.Properties.Settings.DumpAppConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(Helper.CnnValue(Helper.database));
            showDailyTapes();
            initializeDailyBackupDays();
            initializeWeeklyCB();                    
        }

        #region Daily Tapes
        //main method to display the current day tapes to be checked
        private void showDailyTapes()
        {
            todayTapes.Visibility = Visibility.Visible;
            basicDataPanel.Visibility = Visibility.Collapsed;
            backupPlanPanel.Visibility = Visibility.Collapsed;
            backupSettingsPanel.Visibility = Visibility.Collapsed;
            basicDataSystemsPanel.Visibility = Visibility.Collapsed;
            backupHistoryPanel.Visibility = Visibility.Collapsed;
            try
            {
                List<string> dailyBackupDays = DataLayer.GetDailyBackupDays();//the list with daily backup days
                string weeklyBackupDay = DataLayer.GetWeeklyBackupDay();//string with the weekly backup day
                
                
                string today = DateTime.Now.AddDays(0).DayOfWeek.ToString();// finding which day of the week is today in order to search for the tapes and display them
                List<string> todayTapes = new List<string>();// today tapes list

                //we need 2 lists in order to distinguish the monthend backup day from regular weekly backup day
                List<string> weeklyBackupDates = DataLayer.GetWeeklyBackupDates(DateTime.Now.AddDays(0).Date);
                List<string> MonthEndBackupDate = DataLayer.GetMonthlyBackupDate(weeklyBackupDay);
                DataTable DailyBackupList = new DataTable();// table table with daily tapes list


                if (dailyBackupDays.Contains(today))// if today is marked as a daily backup day
                {
                    //query to extract daily tapes information; Details should be like 'DAILY TAPE 1'
                    string query = "select DISTINCT System,BackupType,Details from Tapes where BackupType='Daily' and Details like 'DAILY%'+@day";
                   
                    DailyBackupList = DataLayer.FillDailyTapesTable(today, query, dailyBackupDays,weeklyBackupDates);
                    
                }
                else if (weeklyBackupDay == today.ToString())//if today is marked as a weekly backup day
                {
                    DateTime weekly = DateTime.Now.AddDays(0).Date;
                                       
                    //if today is a weekly backup date we gather the information needed from Tapes table and add the to the grid view
                    if (weeklyBackupDates.Contains(DateTime.Now.AddDays(0).ToLongDateString()))
                    {
                        string query = "select distinct System,BackupType from Tapes where BackupType='Weekly' and Details like 'WEEK '+@week+'%'";
                        
                        DailyBackupList = DataLayer.FillDailyTapesTable(today, query, dailyBackupDays, weeklyBackupDates);
                      
                    }
                    else if (MonthEndBackupDate.Contains(DateTime.Now.AddDays(0).ToLongDateString()))//if today is monthend backup date
                    {
                        string query = "select DISTINCT System,BackupType from Tapes where BackupType='Monthly' and Details like @month+' MONTHEND%'";
                        DailyBackupList = DataLayer.FillDailyTapesTable(today, query, dailyBackupDays, weeklyBackupDates);
                    }
                }
                GridTodayTapes.ItemsSource = DailyBackupList.DefaultView;
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
        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }//main exit button event which will close the application
        #endregion
        //menu items and their actions
        #region Menu Items 

        private void showDailyTapes(object sender, RoutedEventArgs e)
        {
            showDailyTapes();
        }

        //this event is displaying the tapes inserted by the operator in the database
        private void showBasicData(object sender, RoutedEventArgs e)
        {
            todayTapes.Visibility = Visibility.Collapsed;
            basicDataPanel.Visibility = Visibility.Visible;
            backupPlanPanel.Visibility = Visibility.Collapsed;
            backupSettingsPanel.Visibility = Visibility.Collapsed;
            basicDataSystemsPanel.Visibility = Visibility.Collapsed;
            backupHistoryPanel.Visibility = Visibility.Collapsed;
            DataTable tapesList = new DataTable();
            try
            {
                tapesList = DataLayer.FillBasicDataTable();
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
            basicDataGrid.ItemsSource = tapesList.DefaultView;
        }
        //this windows is displaying the daily and weekly backup days
        private void showBackupSettings(object sender, RoutedEventArgs e)
        {
            todayTapes.Visibility = Visibility.Collapsed;
            basicDataPanel.Visibility = Visibility.Collapsed;
            backupPlanPanel.Visibility = Visibility.Collapsed;
            backupSettingsPanel.Visibility = Visibility.Visible;
            basicDataSystemsPanel.Visibility = Visibility.Collapsed;
            backupHistoryPanel.Visibility = Visibility.Collapsed;
            initializeDailyBackupDays();
            initializeWeeklyCB();
        }
        //backup plan is showing a calendar in which weekly dates, monthend dates and holidays are marked with different colours
        private void showBackupPlan(object sender, RoutedEventArgs e)
        {
            todayTapes.Visibility = Visibility.Collapsed;
            basicDataPanel.Visibility = Visibility.Collapsed;
            backupPlanPanel.Visibility = Visibility.Visible;
            backupSettingsPanel.Visibility = Visibility.Collapsed;
            basicDataSystemsPanel.Visibility = Visibility.Collapsed;
            backupHistoryPanel.Visibility = Visibility.Collapsed;
            
        }
        //backup history is getting all the information from history table which contains all checked tapes 
        private void showBackupHistory(object sender, RoutedEventArgs e)
        {
            todayTapes.Visibility = Visibility.Collapsed;
            basicDataPanel.Visibility = Visibility.Collapsed;
            backupPlanPanel.Visibility = Visibility.Collapsed;
            backupSettingsPanel.Visibility = Visibility.Collapsed;
            basicDataSystemsPanel.Visibility = Visibility.Collapsed;
            backupHistoryPanel.Visibility = Visibility.Visible;
            
            DataTable HistoryList = new DataTable();

            try 
            {
                HistoryList = DataLayer.FillHistoryTable();
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }

            HistoryGrid.ItemsSource = HistoryList.DefaultView;
        }
        #endregion

        #region Basic Data

        private void BackButton_Click(object sender, RoutedEventArgs e)//back button event takes to the todayTapes view
        {
            showDailyTapes();
            basicDataPanel.Visibility = Visibility.Collapsed;
            todayTapes.Visibility = Visibility.Visible;
            backupPlanPanel.Visibility = Visibility.Collapsed;
            backupSettingsPanel.Visibility = Visibility.Collapsed;
            basicDataSystemsPanel.Visibility = Visibility.Collapsed;
            backupHistoryPanel.Visibility = Visibility.Collapsed;
        }

        private void AddTapeButton_Click(object sender, RoutedEventArgs e)
        {
            AddTape AT = new AddTape();//new window will pop up for adding a new tape in the database
            AT.Show();
            refreshGrid();
        }

        private void EditTapeButton_Click(object sender, RoutedEventArgs e)
        {
            EditTape ET = new EditTape();//new edit tape window will pop up
            DataRowView rowview = basicDataGrid.SelectedItem as DataRowView;// selected row that we want to edit
            if (rowview != null)//if we selected any row
            {
                //we initialize the objects of the new window with the information from the row selected
                ET.initializeEditWindow(rowview.Row.ItemArray[0].ToString());
                ET.barCodeBox.Text = rowview.Row.ItemArray[0].ToString();
                ET.SystemValuesCB.SelectedItem = rowview.Row.ItemArray[1].ToString();
                ET.BTypeValuesCB.SelectedItem = rowview.Row.ItemArray[2].ToString();
                ET.detailsBox.Text = rowview.Row.ItemArray[3].ToString();
                ET.Show();
            }
            else
            {
                MessageBox.Show("Please select one tape to edit!");
            }
            refreshGrid();// refreshing the grid after the edit of a row
        }
        private void deleteTapeButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rowview = basicDataGrid.SelectedItem as DataRowView;
            if (rowview != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected tape?",
                "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)//poping out the double check of tape deletion
                {
                    string query = "delete from Tapes where BarCode=@code";
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    try
                    {
                        command.Parameters.AddWithValue("@code", rowview.Row.ItemArray[0].ToString());//the barcode of the tape is the first item of the row we selected
                        command.ExecuteScalar();//we just execute the deletion command from database
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
                refreshGrid();
            }
            else
            {
                MessageBox.Show("Please select one tape to delete!");
            }
        }
        private void RefreshTapeButton_Click(object sender, RoutedEventArgs e)
        {
            basicDataGrid.Columns.Clear();
            
            refreshGrid();
        }
        private void refreshGrid()
        {
            //refresh basic data grid method
            DataTable tapesList = new DataTable();
            //refreshing the basic data grid 
            try
            {
                tapesList = DataLayer.FillBasicDataTable();
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
            basicDataGrid.ItemsSource = tapesList.DefaultView;
        }
        
        #endregion

        #region Basic Data Systems
        
        private void AddSystemButton_Click(object sender, RoutedEventArgs e)
        {
            AddSystem AS = new AddSystem();//pops out a new window for adding a new system name to the database
            AS.Show();
            refreshSystems();//refreshing the systems grid
        }
        private void EditSystemButton_Click(object sender, RoutedEventArgs e)
        {
            EditSystem ES = new EditSystem();
            DataRowView rowview = basicDataSystemsGrid.SelectedItem as DataRowView;
            if (rowview != null)
            {
                ES.getSystemToModify(rowview.Row.ItemArray[0].ToString());//passing the selected name of the system as parameter
                ES.systemNameBox.Text = rowview.Row.ItemArray[0].ToString();//initialize the object in the new edit window
                ES.Show();
                
            }
            else
            {
                MessageBox.Show("Please select one tape to edit!");
            }
            
            refreshSystems();//refreshing the systems grid
        }
        private void deleteSystemButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rowview = basicDataSystemsGrid.SelectedItem as DataRowView;
            if (rowview != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected system?",
                "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string query = "delete from Systems where Name=@name";//deleting the system we selected from Systems table
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    try
                    {
                        command.Parameters.AddWithValue("@name", rowview.Row.ItemArray[0].ToString());
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
                refreshSystems();
            }
            else
            {
                MessageBox.Show("Please select one system to delete!");
            }
        }
        private void RefreshSystemButton_Click(object sender, RoutedEventArgs e)
        {
            refreshSystems();
        }
        private void refreshSystems()
        {
            DataTable systemsList = new DataTable();
            //refreshing systems grid
            try
            {
                systemsList = DataLayer.FillBasicDataSystemsTable();
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
            basicDataSystemsGrid.ItemsSource = null;
            basicDataSystemsGrid.Items.Clear();
            basicDataSystemsGrid.ItemsSource = systemsList.DefaultView;
        }
       
        private void TapesButton_Click(object sender, RoutedEventArgs e)
        {
            showBasicData(sender, e);//showing tapes
        }
        private void SystemsButton_Click(object sender, RoutedEventArgs e)//showing systems
        {
            basicDataPanel.Visibility = Visibility.Collapsed;
            basicDataSystemsPanel.Visibility = Visibility.Visible;
            DataTable systemsList = new DataTable();
            //refreshing systems grid
            try
            {
                systemsList = DataLayer.FillBasicDataSystemsTable();
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
            basicDataSystemsGrid.ItemsSource = systemsList.DefaultView;
        }
        #endregion

        #region Backup Settings
        
        //checking if check boxes status changed and update the database
        private void MondayCB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newval = (mondayCB.IsChecked == true);
            if (newval == true)
            {
                if (checkValidDailySelection("Monday"))//only if this day is not already checked as weekly backup day
                {
                    DataLayer.UpdateDays(1, "Monday");
                }
                else
                {
                    MessageBox.Show("This day is already set as a weekly backup day and cannot be checked as daily backup day!");
                    mondayCB.IsChecked = false;
                }
            }
            else
            {
                DataLayer.UpdateDays(0, "Monday");
            }
        }
        private void TuesdayCB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newval = (tuesdayCB.IsChecked == true);
            if (newval == true)
            {
                if (checkValidDailySelection("Tuesday"))//only if this day is not already checked as weekly backup day
                {
                    DataLayer.UpdateDays(1, "Tuesday");
                }
                else
                {
                    MessageBox.Show("This day is already set as a weekly backup day and cannot be checked as daily backup day!");
                    tuesdayCB.IsChecked = false;
                }
            }
            else
            {
                DataLayer.UpdateDays(0, "Tuesday");
            }
        }
        private void WednesdayCB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newval = (WednesdayCB.IsChecked == true);
            if (newval == true)
            {
                if (checkValidDailySelection("Wednesday"))//only if this day is not already checked as weekly backup day
                {
                    DataLayer.UpdateDays(1, "Wednesday");
                }
                else
                {
                    MessageBox.Show("This day is already set as a weekly backup day and cannot be checked as daily backup day!");
                    WednesdayCB.IsChecked = false;
                }
            }
            else
            {
                DataLayer.UpdateDays(0, "Wednesday");
            }
        }
        private void ThursdayCB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newval = (thursdayCB.IsChecked == true);
            if (newval == true)
            {
                if (checkValidDailySelection("Thursday"))//only if this day is not already checked as weekly backup day
                {
                    DataLayer.UpdateDays(1, "Thursday");
                }
                else
                {
                    MessageBox.Show("This day is already set as a weekly backup day and cannot be checked as daily backup day!");
                    thursdayCB.IsChecked = false;
                }
            }
            else
            {
                DataLayer.UpdateDays(0, "Thursday");
            }
        }

        private void FridayCB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newval = (fridayCB.IsChecked == true);
            if (newval == true)
            {
                if (checkValidDailySelection("Friday"))//only if this day is not already checked as weekly backup day
                {
                    DataLayer.UpdateDays(1, "Friday");
                }
                else
                {
                    MessageBox.Show("This day is already set as a weekly backup day and cannot be checked as daily backup day!");
                    fridayCB.IsChecked = false;
                }
            }
            else
            {
                DataLayer.UpdateDays(0, "Friday");
            }
        }

        private void SaturdayCB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newval = (saturdayCB.IsChecked == true);
            if (newval == true)
            {
                if (checkValidDailySelection("Saturday"))//only if this day is not already checked as weekly backup day
                {
                    DataLayer.UpdateDays(1, "Saturday");
                }
                else
                {
                    MessageBox.Show("This day is already set as a weekly backup day and cannot be checked as daily backup day!");
                    sundayCB.IsChecked = false;
                }
            }
            else
            {
                DataLayer.UpdateDays(0, "Saturday");
            }
        }

        private void SundayCB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newval = (sundayCB.IsChecked == true);
            if (newval == true)
            {
                if (checkValidDailySelection("Sunday"))//only if this day is not already checked as weekly backup day
                {
                    DataLayer.UpdateDays(1, "Sunday");
                }
                else
                {
                    MessageBox.Show("This day is already set as a weekly backup day and cannot be checked as daily backup day!");
                    saturdayCB.IsChecked = false;
                }
            }
            else
            {
                DataLayer.UpdateDays(0, "Sunday");
            }
        }

        //initialize daily check boxes status with the info from the database
        private void initializeDailyBackupDays()
        {
            
            int[] DayStates = DataLayer.GetDaysState();//array used to store the days check state
           
            //if DayStates[i]=0 then the checkbox should be unchecked
            for (int i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 0:
                        if (DayStates[i] == 0)
                            mondayCB.IsChecked = false;
                        else
                            mondayCB.IsChecked = true;
                        break;
                    case 1:
                        if (DayStates[i] == 0)
                            tuesdayCB.IsChecked = false;
                        else
                            tuesdayCB.IsChecked = true;
                        break;
                    case 2:
                        if (DayStates[i] == 0)
                            WednesdayCB.IsChecked = false;
                        else
                            WednesdayCB.IsChecked = true;
                        break;
                    case 3:
                        if (DayStates[i] == 0)
                            thursdayCB.IsChecked = false;
                        else
                            thursdayCB.IsChecked = true;
                        break;
                    case 4:
                        if (DayStates[i] == 0)
                            fridayCB.IsChecked = false;
                        else
                            fridayCB.IsChecked = true;
                        break;
                    case 5:
                        if (DayStates[i] == 0)
                            saturdayCB.IsChecked = false;
                        else
                            saturdayCB.IsChecked = true;
                        break;
                    case 6:
                        if (DayStates[i] == 0)
                            sundayCB.IsChecked = false;
                        else
                            sundayCB.IsChecked = true;
                        break;
                }
            }
        }

        private void WeeklyCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (checkValidWeeklySelection((weeklyCB.SelectedItem as ComboBoxItem).Content.ToString()))
            {
                //if the weekly day changed first we need to make sure that weekly state of all the other days of the week is 0
                string query0 = "update Days set Weekly=0";
                //then we set Weekly to 1 for the new selected day
                string query = "update Days set Weekly=1 where Day=@selectedday";
                
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(query0, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.ExecuteScalar();//running first query
                    sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@selectedday", (weeklyCB.SelectedItem as ComboBoxItem).Content.ToString());
                    sqlCommand.ExecuteScalar();//running second query

                }
                catch (Exception exe) { MessageBox.Show(exe.ToString()); }
                finally
                {
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("The day you want to select is already used as a daily backup day. Please select another day for weekly backup!");
            }


        }
        private bool checkValidDailySelection(string day)
        {
            int weeklyState = 0;
            try
            {
                string query0 = "select Weekly from Days where Day=@day";
                SqlCommand command = new SqlCommand(query0, sqlConnection);
                command.Parameters.AddWithValue("@day", day);
                sqlConnection.Open();
                weeklyState = (Int32)command.ExecuteScalar();
                
            }
            catch (Exception exe) { MessageBox.Show(exe.ToString()); }
            finally {sqlConnection.Close(); }
            //checking if the day we selected for daily backup is already selected as weekly backup day (0 means weekly state is 0)
            if (weeklyState == 0)
                return true;
            else
                return false;
            
        }

        private bool checkValidWeeklySelection(string day)
        {
            int dailyState = 0;
            try
            {
                string query0 = "select State from Days where Day=@day";
                SqlCommand command = new SqlCommand(query0, sqlConnection);
                command.Parameters.AddWithValue("@day", day);
                sqlConnection.Open();
                dailyState = (Int32)command.ExecuteScalar();

            }
            catch (Exception exe) { MessageBox.Show(exe.ToString()); }
            finally { sqlConnection.Close(); }
            //checking if the day we selected for weekly backup is already selected as daily backup day (0 means daily state is 0)
            if (dailyState == 0)
                return true;
            else
                return false;
        }
        private void initializeWeeklyCB()
        {
            //initiliaze the selection of combobox
            string query = "select Day from Days where Weekly=1";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            string weeklybackupday = "";
            try
            {
                sqlConnection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    weeklybackupday = reader.GetString(0);
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
            foreach (ComboBoxItem item in weeklyCB.Items)
            {

                if (item.Content.ToString() == weeklybackupday)
                {
                    weeklyCB.SelectedValue = item;

                }
            }
        }

        #endregion

        #region BackupPlan
        //every time we select another date from the calendar we have to check if it's already a holiday or not in order to hide or unhide the mark holiday button
        private void BackupPlanCalendar_SelectionDatesChanged(object sender, EventArgs e)
        {
            //getting holidays from the table with the same name and mark them as green in the calendar
            List<string> holidays = DataLayer.GetHolidays();
            
            //if selected date is already marked as holiday the mark button becomes hidden and unmark button visible
            if (holidays.Contains(backupPlanCalendar.SelectedDate.Value.ToShortDateString()))
            {
                MarkHoliday.Visibility = Visibility.Hidden;
                UNMarkHoliday.Visibility = Visibility.Visible;
            }
            else
            {
                MarkHoliday.Visibility = Visibility.Visible;
                UNMarkHoliday.Visibility = Visibility.Hidden;
            }
        }
        
        private void BackupPlanCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            monthLabel.Content = DataLayer.GetMonthName(backupPlanCalendar.DisplayDate.Month);// label text is changing with calendar's selected month
            string weeklyBackupDay = DataLayer.GetWeeklyBackupDay();
                       
            //we use dictionaries in order to color different the monthend backup date
            Dictionary<string, Color> MonthEndBackupDate = new Dictionary<string, Color>
            { };
            Dictionary<string, Color> weeklyBackupDates = new Dictionary<string, Color>
            { };

            DateTime minDate = backupPlanCalendar.DisplayDate;// this should be the first day of the month displayed in calendar
            DateTime weekly = minDate;
            for (var d = minDate; d < minDate.AddDays(7); d = d.AddDays(1))
            {
                if (d.DayOfWeek.ToString() == weeklyBackupDay)
                {
                    MonthEndBackupDate.Add(d.ToLongDateString(), Colors.DeepSkyBlue);//adding the monthend backup date to the dictionary
                    weekly = d;
                    break;
                }
            }
            //searching for the rest of the weekly backup dates and add them to their dictionary
            List<string> weeklyBackupDatesList = DataLayer.GetWeeklyBackupDates(weekly);
            foreach (var data in weeklyBackupDatesList)
            {
                weeklyBackupDates.Add(data, Colors.BlueViolet);
            }
           
            monthendTV.Items.Clear();//we clear the tree view for monthend dates
            Style style = new Style(typeof(System.Windows.Controls.Primitives.CalendarDayButton));
            foreach (KeyValuePair<string, Color> item in MonthEndBackupDate)//for every entry from the dictionary
            {
                TreeViewItem Child1Item = new TreeViewItem();//create a new treeview item
                Child1Item.Header = DataLayer.GetMonthName(backupPlanCalendar.DisplayDate.Month - 1) + " Month End Backup: " + item.Key;
                monthendTV.Items.Add(Child1Item);
                DataTrigger trigger = new DataTrigger()//create a new style trigger
                {
                    Value = item.Key,
                    Binding = new Binding("Date")
                };
                //color the background of every date from the dictionary with the value associated
                trigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(item.Value)));
                style.Triggers.Add(trigger);
            }
            weekendsTV.Items.Clear();//preparing to populate the weekly backup dates in the treeview
            int x = 1;//weeks counter
            foreach (KeyValuePair<string, Color> item in weeklyBackupDates)
            {
                TreeViewItem Child1Item = new TreeViewItem();
                Child1Item.Header = "Week " + x.ToString() + ": " + item.Key;
                weekendsTV.Items.Add(Child1Item);
                x++;
                DataTrigger trigger = new DataTrigger()
                {
                    Value = item.Key,
                    Binding = new Binding("Date")
                };
                trigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(item.Value)));
                style.Triggers.Add(trigger);
            }
            x = 1;

            //getting holidays from the table with the same name and mark them as green in the calendar
            List<string> holidays = DataLayer.GetHolidays();

            Dictionary<string, Color> Holidays = new Dictionary<string, Color>
            { };//create holidays dictionary used to color them in green
            DateTime now = backupPlanCalendar.DisplayDate;
            DateTime first = new DateTime(now.Year, now.Month, 1);
            DateTime last = first.AddMonths(1).AddDays(-1);
            for (DateTime day = first; day <= last; day=day.AddDays(1))//searching if there are any marked holidays in the showed month
            {
                if (holidays.Contains(day.ToShortDateString()))
                {
                    Holidays.Add(day.ToLongDateString(), Colors.Green);
                }
            }
            holidaysTV.Items.Clear();
            foreach (KeyValuePair<string, Color> item in Holidays)
            {
                TreeViewItem Child1Item = new TreeViewItem();
                Child1Item.Header = item.Key;
                holidaysTV.Items.Add(Child1Item);             
                DataTrigger trigger = new DataTrigger()
                {
                    Value = item.Key,
                    Binding = new Binding("Date")
                };
                trigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(item.Value)));
                style.Triggers.Add(trigger);
            }

            backupPlanCalendar.CalendarDayButtonStyle = style;//associate the new style created to color the calendar with the triggers we set

        }
        
        private void BackupPlanCalendar_Loaded(object sender, RoutedEventArgs e)
        {
            //coloring the calendar and treeviews at first time loading
            monthLabel.Content = DataLayer.GetMonthName(backupPlanCalendar.DisplayDate.Month);
            string weeklyBackupDay = DataLayer.GetWeeklyBackupDay();           

            Dictionary<string, Color> MonthEndBackupDate = new Dictionary<string, Color>
            { };
            Dictionary<string, Color> weeklyBackupDates = new Dictionary<string, Color>
            { };

            DateTime minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime weekly = minDate;
            for (var d = minDate; d < minDate.AddDays(7); d = d.AddDays(1))
            {
                if (d.DayOfWeek.ToString() == weeklyBackupDay)
                {
                    MonthEndBackupDate.Add(d.ToLongDateString(), Colors.DeepSkyBlue);
                    weekly = d;
                    break;
                }
            }

            //searching for the rest of the weekly backup dates and add them to their dictionary
            List<string> weeklyBackupDatesList = DataLayer.GetWeeklyBackupDates(weekly);
            foreach (var data in weeklyBackupDatesList)
            {
                weeklyBackupDates.Add(data, Colors.BlueViolet);
            }

            Style style = new Style(typeof(System.Windows.Controls.Primitives.CalendarDayButton));
            foreach (KeyValuePair<string, Color> item in MonthEndBackupDate)
            {
                TreeViewItem Child1Item = new TreeViewItem();
                Child1Item.Header = DataLayer.GetMonthName(backupPlanCalendar.DisplayDate.Month - 1) + " Month End Backup: " + item.Key;
                monthendTV.Items.Add(Child1Item);

                DataTrigger trigger = new DataTrigger()
                {
                    Value = item.Key,
                    Binding = new Binding("Date")
                };
                trigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(item.Value)));
                style.Triggers.Add(trigger);
            }
            int x = 1;
            foreach (KeyValuePair<string, Color> item in weeklyBackupDates)
            {
                TreeViewItem Child1Item = new TreeViewItem();
                Child1Item.Header = "Week " + x.ToString() + ": " + item.Key;
                weekendsTV.Items.Add(Child1Item);
                x++;
                DataTrigger trigger = new DataTrigger()
                {
                    Value = item.Key,
                    Binding = new Binding("Date")
                };
                trigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(item.Value)));
                style.Triggers.Add(trigger);
            }
            x = 1;

            //getting holidays from the table with the same name and mark them as green in the calendar
            List<string> holidays = DataLayer.GetHolidays();           
            Dictionary<string, Color> Holidays = new Dictionary<string, Color> { };
            DateTime now = backupPlanCalendar.DisplayDate;
            DateTime first = new DateTime(now.Year, now.Month, 1);
            DateTime last = first.AddMonths(1).AddDays(-1);
            //MessageBox.Show(first.ToShortDateString());
            for (DateTime day = first; day <= last; day=day.AddDays(1))
            {
                if (holidays.Contains(day.ToShortDateString()))
                {
                    Holidays.Add(day.ToLongDateString(), Colors.Green);
                }
            }
            holidaysTV.Items.Clear();
            foreach (KeyValuePair<string, Color> item in Holidays)
            {
                TreeViewItem Child1Item = new TreeViewItem();
                Child1Item.Header = item.Key;
                holidaysTV.Items.Add(Child1Item);
                DataTrigger trigger = new DataTrigger()
                {
                    Value = item.Key,
                    Binding = new Binding("Date")
                };
                trigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(item.Value)));
                style.Triggers.Add(trigger);
            }
            backupPlanCalendar.CalendarDayButtonStyle = style;
        }

        private void MarkHoliday_Click(object sender, RoutedEventArgs e)
        {           
            string selectedDate = backupPlanCalendar.SelectedDate.Value.ToShortDateString();
            DataLayer.MarkOrUnmarkHoliday(1, selectedDate);//1 means we are adding a holiday
            BackupPlanCalendar_DisplayDateChanged(backupPlanCalendar, null);
        }

        private void UNMarkHoliday_Click(object sender, RoutedEventArgs e)
        {
            string selectedDate = backupPlanCalendar.SelectedDate.Value.ToShortDateString();
            DataLayer.MarkOrUnmarkHoliday(0, selectedDate);//0 means we are deleting a holiday
            BackupPlanCalendar_DisplayDateChanged(backupPlanCalendar, null);
        }
        #endregion

        #region Check Daily Tapes
        //this is the double click event of today tapes grid which is used to check the tapes
        private void GridTodayTapes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CheckTapes CT = new CheckTapes();//new check tapes window
            List<string> todayTapes = new List<string>();//creating the list with barcodes of the tapes

            try
            {
                List<string> dailyBackupDays = DataLayer.GetDailyBackupDays();
                string weeklyBackupDay = DataLayer.GetWeeklyBackupDay();                 
                string today = DateTime.Now.AddDays(0).DayOfWeek.ToString();
                int x = 0;// 1 means we search daily tapes, 2 is weekly and 3 is monthly
                if (dailyBackupDays.Contains(today))
                {
                    x = 1;
                    todayTapes = DataLayer.GetTodayTapes(x, (dailyBackupDays.IndexOf(today) + 1).ToString(),"","","");
                    //we need to create dinamically the objects of the new windows because the number depends on the backup type
                    CT=Helper.CreateCheckTapesWindow(todayTapes);                   
                }
                else if (weeklyBackupDay == today)
                {
                    DateTime weekly = DateTime.Now.AddDays(0).Date;
                    List<string> weeklyBackupDates = DataLayer.GetWeeklyBackupDates(weekly);
                    List<string> MonthEndBackupDate = DataLayer.GetMonthlyBackupDate(weeklyBackupDay);
                                      
                    DataRowView rowview = GridTodayTapes.SelectedItem as DataRowView;
                    string system = "";
                    if (rowview != null)
                    {
                       system = rowview.Row.ItemArray[0].ToString();                     
                    }
                    else
                    {
                        MessageBox.Show("Please select one row to check available tapes!");
                    }

                    if (weeklyBackupDates.Contains(DateTime.Now.AddDays(0).ToLongDateString()))
                    {
                        x = 2;
                        todayTapes = DataLayer.GetTodayTapes(x, (dailyBackupDays.IndexOf(today) + 1).ToString(), system, (weeklyBackupDates.IndexOf(DateTime.Now.AddDays(0).ToLongDateString()) + 1).ToString(), "");
                        CT=Helper.CreateCheckTapesWindow(todayTapes);
                    }
                    else if (MonthEndBackupDate.Contains(DateTime.Now.AddDays(0).ToLongDateString()))
                    {
                        x = 3;
                        todayTapes = DataLayer.GetTodayTapes(x, (dailyBackupDays.IndexOf(today) + 1).ToString(), system, "", DataLayer.GetMonthName(DateTime.Now.AddMonths(-1).Month).ToUpper());
                        CT = Helper.CreateCheckTapesWindow(todayTapes);
                    }
                }            
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            //searching the history table to check if the tapes for today were already checked or not
            string TapesAlreadyChecked = "";
            TapesAlreadyChecked = DataLayer.CheckTapesHistory(DateTime.Now.ToShortDateString(),todayTapes[0]);
            if (TapesAlreadyChecked=="")//if daily tapes are not already checked then the new Check tapes window will pop up
                CT.Show();
            else { MessageBox.Show("Tapes already checked!"); }
            sqlConnection.Close();
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }     
    }
    } 
