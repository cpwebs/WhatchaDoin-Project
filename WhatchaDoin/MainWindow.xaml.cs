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
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using ListViewItem = System.Windows.Controls.ListViewItem;
using ListView = System.Windows.Controls.ListView;

namespace WhatchaDoin
{

    public partial class MainWindow : Window
    {
        //future dates of events
        private List<DateTime> nextDates = new List<DateTime>();

        //all event dates which are highlighted on calendar
        private List<DateTime> highlightedDates = new List<DateTime>();

        //all events of user
        private List<String> allEvents = new List<String>();

        //all friends of user
        private List<String> allFriends = new List<String>();

        //all following of user
        private List<String> allFollowing = new List<String>();

        //all followers of user
        private List<String> allFollowers = new List<String>();

        //username of user currently using application
        private string username;

        //number of events that have been completed
        private int completed = 0;

        //SQL Server connection string
        string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;";

        public MainWindow(string user)
        {
            username = user;
            InitializeComponent();
            retrieveDatesToDisplay();
            retrieveTotalEvents();
            retrieveTotalFriends();
            retrieveFollowers();
            retrieveFollowing();
            setLabels();
        }

        /*
         * Overrides label text so values could be included in label text
         */
        private void setLabels()
        {
            lblEvents.Content = "Total Events: " + allEvents.Count;
            lblCompleted.Content = "Completed: " + completed;
            lblAdventure.Content = "Next Adventure: " + findUpComingDate() + " Days";
            lblFriends.Content = "Friends: " + allFriends.Count;
            lblFollowers.Content = "Followers: " + allFollowers.Count;
            lblFollowing.Content = "Following: " + allFollowing.Count;
        }

        /*
         * Finds all followers a user has
         */
        private void retrieveFollowers()
        {
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                SqlCommand c = new SqlCommand("SELECT DISTINCT Follower FROM Friends WHERE UserName=@username AND Follower IS NOT NULL", cnn);
                c.Parameters.AddWithValue("@username", username);

                using (SqlDataReader dr = c.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        allFollowers.Add(dr[0].ToString());
                    }
                }
            }
        }

        /*
         * Finds all following persons that a user follows
         */
        private void retrieveFollowing()
        {
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                SqlCommand c = new SqlCommand("SELECT DISTINCT Following FROM Friends WHERE UserName=@username AND Following IS NOT NULL", cnn);
                c.Parameters.AddWithValue("@username", username);

                using (SqlDataReader dr = c.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        allFollowing.Add(dr[0].ToString());
                    }
                }
            }
        }

        /*
         * Finds all friends that all a user has
         */
        private void retrieveTotalFriends()
        {
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                SqlCommand c = new SqlCommand("SELECT DISTINCT Friends FROM Friends WHERE UserName=@username AND Friends IS NOT NULL", cnn);
                c.Parameters.AddWithValue("@username", username);

                using (SqlDataReader dr = c.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        allFriends.Add(dr[0].ToString());
                    }
                }
            }
        }

        /*
         * Finds all events that a user has made past, present, and future
         */
        private void retrieveTotalEvents()
        {
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                SqlCommand c = new SqlCommand("SELECT DISTINCT Activity,Date FROM BucketList WHERE UserName=@username", cnn);
                c.Parameters.AddWithValue("@username", username);

                using (SqlDataReader dr = c.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            allEvents.Add(dr[0].ToString());

                            DateTime dt1 = Convert.ToDateTime(dr[1]);
                            DateTime dt2 = DateTime.Now;

                            if (dt1.Date >= dt2.Date) {
                                nextDates.Add(dt1);
                            }
                            else
                            {
                                completed++;
                            }
                        }
                    }
            }
        }

        /*
         * Finds the next day (in numeric) when an event is planned
         */
        private int findUpComingDate()
        {
            int ret = int.MaxValue;
            foreach(DateTime d in nextDates)
            {
                DateTime dt1 = d;
                DateTime dt2 = DateTime.Now;

                int sub = (int)dt1.Subtract(dt2).TotalDays;

                if(ret > sub)
                {
                    ret = sub; 
                }
            }

            if(ret==int.MaxValue)
            {
                return 0;
            }

            return ret + 1;
        }

        /*
         * Gets dates from DB that are in the future of the user
         */
        private void retrieveDatesToDisplay()
        {
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                SqlCommand c = new SqlCommand("SELECT DISTINCT Activity,Date FROM BucketList WHERE UserName=@username", cnn);
                c.Parameters.AddWithValue("@username", username);

                    using (SqlDataReader dr = c.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                           highlightedDates.Add(Convert.ToDateTime(dr[1]));
                        }
                    }
               
            }
        }

        /*
         * Shows and displays the current information for the event when clicked
         */
        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            DateTime dateSelected = Convert.ToDateTime(calendar.SelectedDate);
            if (highlightedDates.Contains(dateSelected))
            {
                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    cnn.Open();
                    SqlCommand c = new SqlCommand("WITH cte AS (SELECT *, ROW_NUMBER() OVER(PARTITION BY Activity ORDER BY Date DESC) AS rn FROM BucketList) SELECT * FROM cte WHERE rn = 1 AND UserName=@username AND Date=@dateSelected", cnn);
                    c.Parameters.AddWithValue("@username", username);
                    c.Parameters.AddWithValue("@dateSelected", dateSelected);
                    using (SqlDataReader dr = c.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            dateTxtBox.Text = calendar.SelectedDate.Value.ToShortDateString();
                        }
                    }
                    SqlDataAdapter sda = new SqlDataAdapter(c);
                    DataTable dt = new DataTable("activities");
                    sda.Fill(dt);
                    grdActivity.ItemsSource = dt.DefaultView;
                    grdLocation.ItemsSource = dt.DefaultView;
                    grdTimeSet.ItemsSource = dt.DefaultView;
                    grdBudget.ItemsSource = dt.DefaultView;
                    grdReservation.ItemsSource = dt.DefaultView;
                }

                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    //"WITH cte AS (SELECT *, ROW_NUMBER() OVER(PARTITION BY Activity ORDER BY Date DESC) AS rn FROM BucketList) SELECT * FROM cte WHERE rn = 1 AND UserName=@username AND Date=@dateSelected"
                    cnn.Open();
                    SqlCommand c = new SqlCommand("SELECT DISTINCT Supplies FROM BucketList WHERE UserName=@username AND Date=@dateSelected AND Supplies IS NOT NULL AND Supplies != ''", cnn);
                    c.Parameters.AddWithValue("@username", username);
                    c.Parameters.AddWithValue("@dateSelected", dateSelected);
                    SqlDataAdapter sda = new SqlDataAdapter(c);
                    DataTable dt = new DataTable("activities");
                    sda.Fill(dt);
                    grdSupplies.ItemsSource = dt.DefaultView;
                }

                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    //"WITH cte AS (SELECT *, ROW_NUMBER() OVER(PARTITION BY Activity ORDER BY Date DESC) AS rn FROM BucketList) SELECT * FROM cte WHERE rn = 1 AND UserName=@username AND Date=@dateSelected"
                    cnn.Open();
                    SqlCommand c = new SqlCommand("SELECT DISTINCT Friends FROM BucketList WHERE UserName=@username AND Date=@dateSelected AND Friends IS NOT NULL AND Friends != ''", cnn);
                    c.Parameters.AddWithValue("@username", username);
                    c.Parameters.AddWithValue("@dateSelected", dateSelected);
                    SqlDataAdapter sda = new SqlDataAdapter(c);
                    DataTable dt = new DataTable("activities");
                    sda.Fill(dt);
                    grdFriends.ItemsSource = dt.DefaultView;
                }
            }
            else
            {
                grdActivity.ItemsSource = null;
                dateTxtBox.Text = null;
                grdLocation.ItemsSource = null;
                grdTimeSet.ItemsSource = null;
                grdBudget.ItemsSource = null;
                grdReservation.ItemsSource = null;
                grdSupplies.ItemsSource = null;
                grdFriends.ItemsSource = null;
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
    }
}
