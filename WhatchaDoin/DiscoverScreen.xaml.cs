using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Windows.Navigation;
using CefSharp;
using System.Data;

namespace WhatchaDoin
{
    /// <summary>
    /// Interaction logic for DiscoverScreen.xaml
    /// </summary>
    public partial class DiscoverScreen : Window
    {
        //all friends of user
        private List<String> friendList = new List<String>();

        //all event dates which are highlighted on calendar
        private List<DateTime> highlightedDates = new List<DateTime>();

        //username of user currently using application
        private string username;

        //SQL Server connection string
        string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;";

        //lists events on that selected day
        private List<String> selectedDayEvents = new List<String>();

        public DiscoverScreen(string user)
        {
            username = user;
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;
            retrieveDatesToDisplay();
        }

        /*
         * determines if user and searched user is friends and populates friendlist
         */
        private bool privacySettingFriends()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    cnn.Open();
                    using (SqlCommand c = new SqlCommand("SELECT Friends from Friends WHERE UserName = @user", cnn))
                    {
                        c.Parameters.AddWithValue("@user", txtSearch.Text);

                        using (SqlDataReader dr = c.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                friendList.Add(dr[0].ToString());
                            }
                            if(friendList.Contains(username))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /*
         * retreives privacy of searched user
         */
        private string privacySetting()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    cnn.Open();
                    using (SqlCommand c = new SqlCommand("SELECT Privacy from Users WHERE UserName = @user", cnn))
                    {
                        c.Parameters.AddWithValue("@user", txtSearch.Text);

                        using (SqlDataReader dr = c.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                return dr[0].ToString();
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                return "";
            }
            return "";
        }

        /*
         * Gets dates from DB that are in the future of the user
         */
        private void retrieveDatesToDisplay()
        {
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                using (SqlCommand c = new SqlCommand("SELECT Date from BucketList WHERE UserName = @user", cnn))
                {
                    c.Parameters.AddWithValue("@user", username);

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

        /*
         * Shows and displays events of selected day when clicked on calendar
         */
        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            String dis = "";
            DateTime dateSelected = Convert.ToDateTime(calendar.SelectedDate);
            if (highlightedDates.Contains(dateSelected))
            {
                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
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

        /*
         * Various funtions allow closing, maximizing, etc. capabilities of window
         */
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

        /*
         * Navigation links and keeps window size consistent among windows
         */
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

        /*
         * calls functions to display highlighted events on calendar
         */
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

        /*
         * sets web browser zoom size 
         */
        private void ChromiumWebBrowser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            ChromiumWebBrowser.SetZoomLevel(-15.0);
        }

        /*
         * sets web browser zoom size 
         */
        private void ChromiumWebBrowser1_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            ChromiumWebBrowser1.SetZoomLevel(-15.0);
        }

        /*
         * sets web browser zoom size 
         */
        private void ChromiumWebBrowser2_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            ChromiumWebBrowser2.SetZoomLevel(-10.0);
        }

        /*
         * sets web browser zoom size 
         */
        private void ChromiumWebBrowser3_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            ChromiumWebBrowser3.SetZoomLevel(-10.0);
        }

        /*
         * determines if grid values will be displayed based on privacy settings
         */
        private void showEvents(object sender, RoutedEventArgs e)
        {
            if (privacySetting().Equals("Public"))
            {
                fillEvents();
            }
            else if (privacySetting().Equals("Friends"))
            {
                if (txtSearch.Text == username)
                {
                    fillEvents();
                }
                else if (privacySettingFriends() == true)
                {
                    fillEvents();
                }
                else
                {
                    MessageBox.Show("Aren't friends");
                }
            }
            else if (privacySetting().Equals("Private") && txtSearch.Text == username)
            {
                fillEvents();
            }
            else
            {
                MessageBox.Show("Privacy Restricted");
            }
        }

        /*
         * fills events grid
         */
        private void fillEvents()
        {
            grdFollower.Visibility = Visibility.Collapsed;
            grdBucketlist.Visibility = Visibility.Visible;
            grdFollowing.Visibility = Visibility.Collapsed;

            string searchedUser = txtSearch.Text;

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                /*
                 Limits distinct values as compared to 
                 "SELECT Activity,Date,TimeSet,Budget,Reservation,Supplies FROM BucketList WHERE UserName=@username ";
                 Insures that rows that are repeated with events and such are displayed due to design of DB
                */
                CmdString = "WITH cte AS (SELECT *, ROW_NUMBER() OVER(PARTITION BY Activity ORDER BY Date DESC) AS rn FROM BucketList) SELECT * FROM cte WHERE rn = 1 AND UserName=@username";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", searchedUser);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Activity");
                sda.Fill(dt);
                grdBucketlist.ItemsSource = dt.DefaultView;

            }
        }

        /*
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                txtSearch.Visibility = Visibility.Collapsed;
                txtSearchWatermark.Visibility = Visibility.Visible;
            }
        }

        /*
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void txtSearchWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearchWatermark.Visibility = Visibility.Collapsed;
            txtSearch.Visibility = Visibility.Visible;
            txtSearch.Focus();
        }

        /*
         * determines if grid values will be displayed based on privacy settings
         */
        private void showFollowers(object sender, RoutedEventArgs e)
        {
            if (privacySetting().Equals("Public"))
            {
                fillFollowers();
            }
            else if (privacySetting().Equals("Friends"))
            {
                if (txtSearch.Text == username)
                {
                    fillFollowers();
                }
                else if (privacySettingFriends() == true)
                {
                    fillFollowers();
                }
                else
                {
                    MessageBox.Show("Aren't friends");
                }
            }
            else if (privacySetting().Equals("Private") && txtSearch.Text == username)
            {
                fillFollowers();
            }
            else
            {
                MessageBox.Show("Privacy Restricted");
            }
        }

        /*
         * fills followers grid
         */
        private void fillFollowers()
        {
            grdFollower.Visibility = Visibility.Visible;
            grdBucketlist.Visibility = Visibility.Collapsed;
            grdFollowing.Visibility = Visibility.Collapsed;

            string searchedUser = txtSearch.Text;

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                CmdString = "SELECT DISTINCT Follower FROM Friends WHERE UserName=@username AND Follower IS NOT NULL";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", searchedUser);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Activity");
                sda.Fill(dt);
                grdFollower.ItemsSource = dt.DefaultView;

            }
        }

        /*
        * fills following grid
        */
        private void fillFollowing()
        {
            grdFollower.Visibility = Visibility.Collapsed;
            grdBucketlist.Visibility = Visibility.Collapsed;
            grdFollowing.Visibility = Visibility.Visible;

            string searchedUser = txtSearch.Text;

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                CmdString = "SELECT DISTINCT Following FROM Friends WHERE UserName=@username AND Following IS NOT NULL";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", searchedUser);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Activity");
                sda.Fill(dt);
                grdFollowing.ItemsSource = dt.DefaultView;

            }
        }

        /*
         * determines if grid values will be displayed based on privacy settings
         */
        private void showFollowing(object sender, RoutedEventArgs e)
        {
            if (privacySetting().Equals("Public"))
            {
                fillFollowing();
            }
            else if (privacySetting().Equals("Friends"))
            {
                if (txtSearch.Text == username)
                {
                    fillFollowing();
                }
                else if (privacySettingFriends() == true)
                {
                    fillFollowing();
                }
                else
                {
                    MessageBox.Show("Aren't friends");
                }
            }
            else if (privacySetting().Equals("Private") && txtSearch.Text == username)
            {
                fillFollowing();
            }
            else
            {
                MessageBox.Show("Privacy Restricted");
            }
        }
    }
}
