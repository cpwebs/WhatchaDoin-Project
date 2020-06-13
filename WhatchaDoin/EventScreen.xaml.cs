using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using ListViewItem = System.Windows.Controls.ListViewItem;
using ListView = System.Windows.Controls.ListView;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;

namespace WhatchaDoin
{

    public partial class EventScreen : Window
    {
        private string username;
        private string eventName;
        private string supply;
        private string friend;
        private int rowCount;
        private List<DateTime> highlightedDates = new List<DateTime>();
        private List<String> selectedDayEvents = new List<String>();

        public EventScreen(string user)
        {
            
            username = user;
            InitializeComponent();
            FillDataGrid();
            retrieveDatesToDisplay();
            StateChanged += MainWindowStateChangeRaised;

        }

        private void retrieveDatesToDisplay()
        {
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                cnn.Open();
                using (SqlCommand c = new SqlCommand("SELECT Date from BucketList", cnn))
                {
                    using (SqlDataReader dr = c.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            highlightedDates.Add(Convert.ToDateTime(dr[0]));
                        }
                    }
                }
            }
        }

        private void FillDataGrid()
        {
            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                /*
                 Limits distinct values as compared to 
                 "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
                 Insures that rows that are repeated with events and such are displayed due to design of DB
                */
                CmdString = "WITH cte AS (SELECT *, ROW_NUMBER() OVER(PARTITION BY Activity ORDER BY Date DESC) AS rn FROM BucketList) SELECT * FROM cte WHERE rn = 1 AND UserName=@username";
               
                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Activity");
                sda.Fill(dt);
                grdBucketlist.ItemsSource = dt.DefaultView;
                
            }
        }

        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            String dis = "";
            DateTime dateSelected = Convert.ToDateTime(calendar.SelectedDate);
            if (highlightedDates.Contains(dateSelected))
            {
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand c = new SqlCommand("SELECT DISTINCT Activity FROM BucketList WHERE UserName=@username AND Date=@dateSelected", cnn);
                    c.Parameters.AddWithValue("@username", username);
                    c.Parameters.AddWithValue("@dateSelected", dateSelected);
                    using (SqlDataReader dr = c.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            selectedDayEvents.Add(dr[0].ToString());
                        }
                    }
                }

                for (int i = 0; i < selectedDayEvents.Count; i++)
                {
                    dis += selectedDayEvents[i] + "\n";
                }

                MessageBox.Show("The event(s) for the selected day:" + "\n" + dis, "Information");
                selectedDayEvents.Clear();
                dis = "";
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "Home":
                    MainWindow main = new MainWindow(username);
                    main.Height = this.ActualHeight;
                    main.Width = this.ActualWidth;
                    main.Top = this.Top;
                    main.Left = this.Left;
                    main.WindowStartupLocation = this.WindowStartupLocation;
                    if (this.WindowState == System.Windows.WindowState.Maximized)
                    {
                        main.WindowState = System.Windows.WindowState.Maximized;
                    }
                    main.Show();
                    this.Close();
                    break;
                case "Events":
                    EventScreen events = new EventScreen(username);
                    events.Height = this.ActualHeight;
                    events.Width = this.ActualWidth;
                    events.Top = this.Top;
                    events.Left = this.Left;
                    events.WindowStartupLocation = this.WindowStartupLocation;
                    if (this.WindowState == System.Windows.WindowState.Maximized)
                    {
                        events.WindowState = System.Windows.WindowState.Maximized;
                    }
                    events.Show();
                    this.Close();
                    break;
                case "Friends":
                    FriendScreen friend = new FriendScreen(username);
                    friend.Height = this.ActualHeight;
                    friend.Width = this.ActualWidth;
                    friend.Top = this.Top;
                    friend.Left = this.Left;
                    friend.WindowStartupLocation = this.WindowStartupLocation;
                    if (this.WindowState == System.Windows.WindowState.Maximized)
                    {
                        friend.WindowState = System.Windows.WindowState.Maximized;
                    }
                    friend.Show();
                    this.Close();
                    break;
                case "Memories":
                    MemoriesScreen memories = new MemoriesScreen(username);
                    memories.Height = this.ActualHeight;
                    memories.Width = this.ActualWidth;
                    memories.Top = this.Top;
                    memories.Left = this.Left;
                    memories.WindowStartupLocation = this.WindowStartupLocation;
                    if (this.WindowState == System.Windows.WindowState.Maximized)
                    {
                        memories.WindowState = System.Windows.WindowState.Maximized;
                    }
                    memories.Show();
                    this.Close();
                    break;
                case "Discover":
                    DiscoverScreen discover = new DiscoverScreen(username);
                    discover.Height = this.ActualHeight;
                    discover.Width = this.ActualWidth;
                    discover.Top = this.Top;
                    discover.Left = this.Left;
                    discover.WindowStartupLocation = this.WindowStartupLocation;
                    if (this.WindowState == System.Windows.WindowState.Maximized)
                    {
                        discover.WindowState = System.Windows.WindowState.Maximized;
                    }
                    discover.Show();
                    this.Close();
                    break;
                case "Settings":
                    SettingsScreen settings = new SettingsScreen(username);
                    settings.Height = this.ActualHeight;
                    settings.Width = this.ActualWidth;
                    settings.Top = this.Top;
                    settings.Left = this.Left;
                    settings.WindowStartupLocation = this.WindowStartupLocation;
                    if (this.WindowState == System.Windows.WindowState.Maximized)
                    {
                        settings.WindowState = System.Windows.WindowState.Maximized;
                    }
                    settings.Show();
                    this.Close();
                    break;
                case "Logout":
                    LoginScreen login = new LoginScreen();
                    login.Show();
                    this.Close();
                    break;

                default:
                    break;
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
            DataRowView dr = (DataRowView)dgr.Item;

            txtActivityWatermark_GotFocus(this, new RoutedEventArgs());
            dpWatermark_GotFocus(this, new RoutedEventArgs());
            budgetWatermark_GotFocus(this, new RoutedEventArgs());
            reserve_DropDownOpened(this, new EventArgs());

            this.Focus();

            //array value corresponds to the value found in the select tag from SQL Server
            addUpdateEvent.Content = "Update\nEvent";
            txtActivity.Text = dr[0].ToString();
            txtActivityWatermark.Visibility = System.Windows.Visibility.Collapsed;
            txtActivity.Visibility = System.Windows.Visibility.Visible;
            txtLocation.Visibility = Visibility.Visible;
            txtLocationWatermark.Visibility = Visibility.Collapsed;
            dp.SelectedDate = Convert.ToDateTime(dr[1].ToString());
            timeSet.SelectedTime = Convert.ToDateTime(dr[3].ToString());
            budget.Text = "$" + Math.Round((decimal)dr[4], 2).ToString();
            reserve.Text = dr[5].ToString();
            testS.Text = dr[6].ToString();
            testF.Text = dr[7].ToString();
            txtLocation.Text = dr[8].ToString();

            eventName = dr[0].ToString();
            supply = dr[6].ToString();
            friend = dr[7].ToString();

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                CmdString = "SELECT Supplies FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Supplies IS NOT NULL";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@eventName", eventName);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Supplies");
                sda.Fill(dt);
                grdSupply.ItemsSource = dt.DefaultView;
            }

            CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                CmdString = "SELECT DISTINCT Friends FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Friends IS NOT NULL";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@eventName", eventName);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Supplies");
                sda.Fill(dt);
                grdFriends.ItemsSource = dt.DefaultView;
            }
        }

        private void clear_Fields(object sender, RoutedEventArgs e)
        {
            addUpdateEvent.Content = "Add\nEvent";
            txtActivity.Text = "";
            txtLocation.Text = "";
            dp.SelectedDate = null;
            timeSet.SelectedTime = null;
            timeSet.Text = " ";
            budget.Text = "";
            reserve.Text = "";
            testS.Text = "";
            testF.Text = "";
            grdSupply.ItemsSource = null;
            grdFriends.ItemsSource = null;
        }

        private void addEvent(object sender, RoutedEventArgs e)
        {
            if (addUpdateEvent.Content.ToString().Equals("Add\nEvent"))
            {
                string activity = txtActivity.Text;
                DateTime date = Convert.ToDateTime(dp.SelectedDate);
                DateTime time = Convert.ToDateTime(timeSet.SelectedTime);
                Double money;

                if (budget.Text.Contains("$"))
                {
                    money = Convert.ToDouble(budget.Text.Substring(1));
                }
                else
                {
                    money = Convert.ToDouble(budget.Text);
                }
                string reservation = reserve.Text;
                string supply = testS.Text;
                string friend = testF.Text;
                string location = txtLocation.Text;


                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "INSERT INTO BucketList (Activity, Date,UserName,TimeSet,Budget,Reservation, Supplies, Friends, Location) VALUES( @activity, @date, @user, @time, @money, @reservation, @supply, @friend, @location );";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    // values are retrieve from variables defined in program
                    insCmd.Parameters.AddWithValue("@activity", activity);
                    insCmd.Parameters.AddWithValue("@date", date);
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@time", time);
                    insCmd.Parameters.AddWithValue("@money", money);
                    insCmd.Parameters.AddWithValue("@reservation", reservation);
                    insCmd.Parameters.AddWithValue("@supply", supply);
                    insCmd.Parameters.AddWithValue("@friend", friend);
                    insCmd.Parameters.AddWithValue("@location", location);
                    insCmd.ExecuteNonQuery();
                    MessageBox.Show("Event successfully created!");
                }
            }
            else
            {
                string activity = txtActivity.Text;
                DateTime date = Convert.ToDateTime(dp.SelectedDate);
                DateTime time = Convert.ToDateTime(timeSet.SelectedTime);
                Double money;

                if (budget.Text.Contains("$"))
                {
                    money = Convert.ToDouble(budget.Text.Substring(1));
                }
                else
                {
                    money = Convert.ToDouble(budget.Text);
                }
                string reservation = reserve.Text;
                string supply = testS.Text;
                string friend = testF.Text;
                string location = txtLocation.Text;

                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "UPDATE BucketList SET Activity = @activity, Date = @date, UserName = @user, TimeSet = @time, Budget = @money, Reservation = @reservation, Location = @location WHERE UserName = @user AND Activity = @activity";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    // values are retrieve from variables defined in program
                    insCmd.Parameters.AddWithValue("@activity", activity);
                    insCmd.Parameters.AddWithValue("@date", date);
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@time", time);
                    insCmd.Parameters.AddWithValue("@money", money);
                    insCmd.Parameters.AddWithValue("@reservation", reservation);
                    insCmd.Parameters.AddWithValue("@supply", supply);
                    insCmd.Parameters.AddWithValue("@friend", friend);
                    insCmd.Parameters.AddWithValue("@location", location);
                    insCmd.ExecuteNonQuery();
                    MessageBox.Show("Updated successfully!");
                }
            }
            //populate grid again
            FillDataGrid();

        }

        private void deleteEvent(object sender, RoutedEventArgs e)
        {
            //deletes to and from DB
            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                CmdString = "DELETE FROM BucketList WHERE UserName = @username AND Activity = @eventName";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                // values are retrieve from variables defined in program
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@eventName", eventName);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Activity");
                sda.Fill(dt);
                grdBucketlist.ItemsSource = dt.DefaultView;
            }

            //populate grid again
            FillDataGrid();

            //clear fields
            txtActivity.Text = "";
            txtLocation.Text = "";
            dp.SelectedDate = null;
            timeSet.SelectedTime = null;
            budget.Text = "";
            reserve.Text = "";
            testS.Text = "";
            testF.Text = "";
            grdSupply.ItemsSource = null;
            grdFriends.ItemsSource = null;
        }

        private bool fieldsEmpty()
        {
            if(txtActivity.Text=="" && dp.SelectedDate==null && timeSet.SelectedTime==null && budget.Text=="" && reserve.Text=="" && testS.Text=="" && testF.Text=="" && txtLocation.Text=="")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void addSupply(object sender, RoutedEventArgs e)
        {
            if(fieldsEmpty())
            {
                string activity = txtActivity.Text;
                DateTime date = Convert.ToDateTime(dp.SelectedDate);
                DateTime time = Convert.ToDateTime(timeSet.SelectedTime);
                Double money = Convert.ToDouble(budget.Text.Substring(1));
                string reservation = reserve.Text;
                string supply = testS.Text;
                string location = txtLocation.Text;


                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "INSERT INTO BucketList (Activity, Date,UserName,TimeSet,Budget,Reservation, Supplies, Location) VALUES( @activity, @date, @user, @time, @money, @reservation, @supply, @location);";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    // values are retrieve from variables defined in program
                    insCmd.Parameters.AddWithValue("@activity", activity);
                    insCmd.Parameters.AddWithValue("@date", date);
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@time", time);
                    insCmd.Parameters.AddWithValue("@money", money);
                    insCmd.Parameters.AddWithValue("@reservation", reservation);
                    insCmd.Parameters.AddWithValue("@supply", supply);
                    insCmd.Parameters.AddWithValue("@location", location);
                    insCmd.ExecuteNonQuery();
                }
            }

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                CmdString = "SELECT DISTINCT Supplies FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Supplies IS NOT NULL";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@eventName", eventName);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Supplies");
                sda.Fill(dt);
                grdSupply.ItemsSource = dt.DefaultView;
            }
        }

        private void addFriend(object sender, RoutedEventArgs e)
        {
            if (fieldsEmpty())
            {
                string activity = txtActivity.Text;
                DateTime date = Convert.ToDateTime(dp.SelectedDate);
                DateTime time = Convert.ToDateTime(timeSet.SelectedTime);
                Double money = Convert.ToDouble(budget.Text.Substring(1));
                string reservation = reserve.Text;
                string friend = testF.Text;
                string location = txtLocation.Text;


                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "INSERT INTO BucketList (Activity, Date,UserName,TimeSet,Budget,Reservation, Friends, Location) VALUES( @activity, @date, @user, @time, @money, @reservation, @friend, @location );";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    // values are retrieve from variables defined in program
                    insCmd.Parameters.AddWithValue("@activity", activity);
                    insCmd.Parameters.AddWithValue("@date", date);
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@time", time);
                    insCmd.Parameters.AddWithValue("@money", money);
                    insCmd.Parameters.AddWithValue("@reservation", reservation);
                    insCmd.Parameters.AddWithValue("@friend", friend);
                    insCmd.Parameters.AddWithValue("@location", location);
                    insCmd.ExecuteNonQuery();
                }
            }


            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                CmdString = "SELECT DISTINCT Friends FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Friends IS NOT NULL";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@eventName", eventName);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Friends");
                sda.Fill(dt);
                grdFriends.ItemsSource = dt.DefaultView;
            }
        }

        private void deleteSupply(object sender, RoutedEventArgs e)
        {
            if (rowCount > 1)
            {
                String CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    CmdString = "DELETE FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Supplies = @supply";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@eventName", eventName);
                    cmd.Parameters.AddWithValue("@supply", supply);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Supplies");
                    sda.Fill(dt);
                    grdSupply.ItemsSource = dt.DefaultView;
                }

                CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    CmdString = "SELECT DISTINCT Supplies FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Supplies IS NOT NULL";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@eventName", eventName);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Supplies");
                    sda.Fill(dt);
                    grdSupply.ItemsSource = dt.DefaultView;
                }

            }
        }

        private void deleteFriend(object sender, RoutedEventArgs e)
        {
            if (rowCount > 1)
            {
                String CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    CmdString = "DELETE FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Friends = @friend";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@eventName", eventName);
                    cmd.Parameters.AddWithValue("@friend", friend);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Event");
                    sda.Fill(dt);
                    grdFriends.ItemsSource = dt.DefaultView;
                }

                CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    CmdString = "SELECT DISTINCT Friends FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Friends IS NOT NULL";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@eventName", eventName);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Friend");
                    sda.Fill(dt);
                    grdFriends.ItemsSource = dt.DefaultView;
                }

            }
        }

        private void supplyDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
            DataRowView dr = (DataRowView)dgr.Item;

            rowCount = grid.Items.Count;
            supply = dr[0].ToString();
        }

        private void friendDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
            DataRowView dr = (DataRowView)dgr.Item;

            rowCount = grid.Items.Count;
            friend = dr[0].ToString();
        }

        private void calendarButton_Loaded(object sender, EventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            HighlightDay(button, date);
            button.DataContextChanged += new DependencyPropertyChangedEventHandler(calendarButton_DataContextChanged);
        }

        /*
         * highlights event on calendar
         */
        private void HighlightDay(CalendarDayButton button, DateTime date)
        {
            if (highlightedDates.Contains(date))
                button.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#75B791");
            else
                button.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF1BE");
        }

        /*
         * data context change event on calendar
         */
        private void calendarButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            HighlightDay(button, date);
        }

        private void txtActivity_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtActivity.Text))
            {
                txtActivity.Visibility = Visibility.Collapsed;
                txtActivityWatermark.Visibility = Visibility.Visible;
            }
        }

        private void txtActivityWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            txtActivityWatermark.Visibility = Visibility.Collapsed;
            txtActivity.Visibility = Visibility.Visible;
            txtActivity.Focus();
        }

        private void dpWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            dpWatermark.Visibility = Visibility.Collapsed;
            dp.Focus();
        }

        private void budget_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(budget.Text))
            {
                budget.Visibility = Visibility.Collapsed;
                budgetWatermark.Visibility = Visibility.Visible;
            }
        }

        private void budgetWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            budgetWatermark.Visibility = Visibility.Collapsed;
            budget.Visibility = Visibility.Visible;
            budget.Focus();
        }

        private void reserve_DropDownOpened(object sender, EventArgs e)
        {
            reserve.Foreground= new SolidColorBrush(Colors.Black);
        }

        private void testSWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            testSWatermark.Visibility = Visibility.Collapsed;
            testS.Visibility = Visibility.Visible;
            testS.Focus();
        }

        private void testS_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(testS.Text))
            {
                testS.Visibility = Visibility.Collapsed;
                testSWatermark.Visibility = Visibility.Visible;
            }
        }

        private void testFWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            testFWatermark.Visibility = Visibility.Collapsed;
            testF.Visibility = Visibility.Visible;
            testF.Focus();
        }

        private void testF_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(testF.Text))
            {
                testF.Visibility = Visibility.Collapsed;
                testFWatermark.Visibility = Visibility.Visible;
            }
        }

        private void txtLocation_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLocation.Text))
            {
                txtLocation.Visibility = Visibility.Collapsed;
                txtLocationWatermark.Visibility = Visibility.Visible;
            }
        }

        private void txtLocationWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLocationWatermark.Visibility = Visibility.Collapsed;
            txtLocation.Visibility = Visibility.Visible;
            txtLocation.Focus();
        }
    }
}
