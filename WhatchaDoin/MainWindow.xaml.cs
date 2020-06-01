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
        private List<DateTime> nextDates = new List<DateTime>();
        private List<DateTime> significantDates = new List<DateTime>();
        private List<String> allEvents = new List<String>();
        private List<String> allFriends = new List<String>();
        private List<String> allFollowing = new List<String>();
        private List<String> allFollowers = new List<String>();
        private string username;
        private int completed = 0;

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

        private void setLabels()
        {
            lblEvents.Content = "Total Events: " + allEvents.Count;
            lblCompleted.Content = "Completed: " + completed;
            lblAdventure.Content = "Next Adventure: " + compareDates() + " Days";
            lblFriends.Content = "Friends: " + allFriends.Count;
            lblFollowers.Content = "Followers: " + allFollowers.Count;
            lblFollowing.Content = "Following: " + allFollowing.Count;
        }

        private void retrieveFollowers()
        {
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
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

        private void retrieveFollowing()
        {
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
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

        private void retrieveTotalFriends()
        {
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
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

        private void retrieveTotalEvents()
        {
            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
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

                            if (dt1.Date > dt2.Date) {
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

        private int compareDates()
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

        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(calendar.SelectedDate.ToString());
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
