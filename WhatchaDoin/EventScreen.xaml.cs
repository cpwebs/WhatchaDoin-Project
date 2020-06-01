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
        private List<DateTime> significantDates = new List<DateTime>();

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
                            significantDates.Add(Convert.ToDateTime(dr[0]));
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
                //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
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
            /*if (MessageBox.Show("Do you want to add an event?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // add event
            }
            else
            {
                // cancel
            }*/
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

            txtActivity.Text = dr[0].ToString();
            dp.SelectedDate = Convert.ToDateTime(dr[1].ToString());
            //timeSet.Text = dr[2].ToString();
            timeSet.SelectedTime = Convert.ToDateTime(dr[3].ToString());
            budget.Text = "$" + Math.Round((decimal)dr[4], 2).ToString();
            reserve.Text = dr[5].ToString();
            testS.Text = dr[6].ToString();
            testF.Text = dr[7].ToString();

            //dp.SelectedDate.Value.Date.ToShortDateString();

            eventName = dr[0].ToString();
            supply = dr[6].ToString();
            friend = dr[7].ToString();

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
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
                //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
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
            txtActivity.Text = "";
            dp.SelectedDate = null;
            //timeSet.Text = dr[2].ToString();
            timeSet.SelectedTime = null;
            budget.Text = "";
            reserve.Text = "";
            testS.Text = "";
            testF.Text = "";
            grdSupply.ItemsSource = null;
            grdFriends.ItemsSource = null;

        }

        private void addEvent(object sender, RoutedEventArgs e)
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


            SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
            sqlCon.Open();

            String insStmt = "INSERT INTO BucketList (Activity, Date,UserName,TimeSet,Budget,Reservation, Supplies, Friends) VALUES( @activity, @date, @user, @time, @money, @reservation, @supply, @friend );";

            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                cnn.Open();
                SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                // use sqlParameters to prevent sql injection!
                insCmd.Parameters.AddWithValue("@activity", activity);
                insCmd.Parameters.AddWithValue("@date", date);
                insCmd.Parameters.AddWithValue("@user", username);
                insCmd.Parameters.AddWithValue("@time", time);
                insCmd.Parameters.AddWithValue("@money", money);
                insCmd.Parameters.AddWithValue("@reservation", reservation);
                insCmd.Parameters.AddWithValue("@supply", supply);
                insCmd.Parameters.AddWithValue("@friend", friend);
                insCmd.ExecuteNonQuery();
                MessageBox.Show("Event successfully created!");
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
                //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
                CmdString = "DELETE FROM BucketList WHERE UserName = @username AND Activity = @eventName";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
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
            if(txtActivity.Text=="" && dp.SelectedDate==null && timeSet.SelectedTime==null && budget.Text=="" && reserve.Text=="" && testS.Text=="" && testF.Text=="")
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


                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "INSERT INTO BucketList (Activity, Date,UserName,TimeSet,Budget,Reservation, Supplies) VALUES( @activity, @date, @user, @time, @money, @reservation, @supply);";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    insCmd.Parameters.AddWithValue("@activity", activity);
                    insCmd.Parameters.AddWithValue("@date", date);
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@time", time);
                    insCmd.Parameters.AddWithValue("@money", money);
                    insCmd.Parameters.AddWithValue("@reservation", reservation);
                    insCmd.Parameters.AddWithValue("@supply", supply);
                    insCmd.ExecuteNonQuery();
                }
            }

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
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


                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "INSERT INTO BucketList (Activity, Date,UserName,TimeSet,Budget,Reservation, Friends) VALUES( @activity, @date, @user, @time, @money, @reservation, @friend );";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    insCmd.Parameters.AddWithValue("@activity", activity);
                    insCmd.Parameters.AddWithValue("@date", date);
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@time", time);
                    insCmd.Parameters.AddWithValue("@money", money);
                    insCmd.Parameters.AddWithValue("@reservation", reservation);
                    insCmd.Parameters.AddWithValue("@friend", friend);
                    insCmd.ExecuteNonQuery();
                }
            }


            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
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
                    //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
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
                    //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
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
                    //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
                    CmdString = "DELETE FROM BucketList WHERE UserName = @username AND Activity = @eventName AND Friends = @friend";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@eventName", eventName);
                    cmd.Parameters.AddWithValue("@friend", friend);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Friend");
                    sda.Fill(dt);
                    grdFriends.ItemsSource = dt.DefaultView;
                }

                CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    //CmdString = "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
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

        private void HighlightDay(CalendarDayButton button, DateTime date)
        {
            if (significantDates.Contains(date))
                button.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#75B791");
            else
                button.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF1BE");
        }

        private void calendarButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            HighlightDay(button, date);
        }
    }
}
