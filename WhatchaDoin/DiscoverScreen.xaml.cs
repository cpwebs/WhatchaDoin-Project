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
        public string username;
        private List<DateTime> highlightedDates = new List<DateTime>();

        public DiscoverScreen(string user)
        {
            username = user;
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;
            retrieveDatesToDisplay();


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

        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            if (MessageBox.Show("Do you want to add an event?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // add event
            }
            else
            {
                // cancel
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

        private void Browser_OnFrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            ChromiumWebBrowser.SetZoomLevel(-15.0);
            ChromiumWebBrowser1.SetZoomLevel(-15.0);
            ChromiumWebBrowser2.SetZoomLevel(-10.0);
            ChromiumWebBrowser3.SetZoomLevel(-10.0);
        }

        private void showEvents(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }

        private void FillDataGrid()
        {
            grdFollower.Visibility = Visibility.Collapsed;
            grdBucketlist.Visibility = Visibility.Visible;
            grdFollowing.Visibility = Visibility.Collapsed;

            string searchedUser = txtSearch.Text;

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
                cmd.Parameters.AddWithValue("@username", searchedUser);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Activity");
                sda.Fill(dt);
                grdBucketlist.ItemsSource = dt.DefaultView;

            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                txtSearch.Visibility = Visibility.Collapsed;
                txtSearchWatermark.Visibility = Visibility.Visible;
            }
        }

        private void txtSearchWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearchWatermark.Visibility = Visibility.Collapsed;
            txtSearch.Visibility = Visibility.Visible;
            txtSearch.Focus();
        }

        private void showFollowers(object sender, RoutedEventArgs e)
        {
            grdFollower.Visibility = Visibility.Visible;
            grdBucketlist.Visibility = Visibility.Collapsed;
            grdFollowing.Visibility = Visibility.Collapsed;

            string searchedUser = txtSearch.Text;

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
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
        private void showFollowing(object sender, RoutedEventArgs e)
        {
            grdFollower.Visibility = Visibility.Collapsed;
            grdBucketlist.Visibility = Visibility.Collapsed;
            grdFollowing.Visibility = Visibility.Visible;

            string searchedUser = txtSearch.Text;

            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
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
    }
}
