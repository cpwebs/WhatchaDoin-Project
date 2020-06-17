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
using System.Windows.Controls.Primitives;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using ListViewItem = System.Windows.Controls.ListViewItem;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Diagnostics;

namespace WhatchaDoin
{

    public partial class FriendScreen : Window

    {
        //all event dates which are highlighted on calendar
        private List<DateTime> highlightedDates = new List<DateTime>();

        //username of user currently using application
        private string username;

        //SQL Server connection string
        string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;";

        //lists events on that selected day
        private List<String> selectedDayEvents = new List<String>();

        public FriendScreen(string user)
        {
            username = user;
            InitializeComponent();
            FillDataGrid();
            retrieveDatesToDisplay();
            StateChanged += MainWindowStateChangeRaised;
            viewMessages();
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
         * Fills friend grid with friends of user
         */
        private void FillDataGrid()
        {
            String CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                CmdString = "SELECT DISTINCT Friends FROM Friends WHERE UserName=@username AND Friends IS NOT NULL";
                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Friends");
                sda.Fill(dt);
                grdEmployee.ItemsSource = dt.DefaultView;
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
                    if (this.WindowState == WindowState.Maximized)
                    {
                        main.WindowState = WindowState.Maximized;
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
                    if (this.WindowState == WindowState.Maximized)
                    {
                        events.WindowState = WindowState.Maximized;
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
                    if (this.WindowState == WindowState.Maximized)
                    {
                        friend.WindowState = WindowState.Maximized;
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
                    if (this.WindowState == WindowState.Maximized)
                    {
                        discover.WindowState = WindowState.Maximized;
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
                    if (this.WindowState == WindowState.Maximized)
                    {
                        settings.WindowState = WindowState.Maximized;
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
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void txtFriendWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            txtFriendWatermark.Visibility = Visibility.Collapsed;
            txtFriendCode.Visibility = Visibility.Visible;
            txtFriendCode.Focus();
        }

        /*
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void txtFriendCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtFriendCode.Text))
            {
                txtFriendCode.Visibility = Visibility.Collapsed;
                txtFriendWatermark.Visibility = Visibility.Visible;
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
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void recipient_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(recipient.Text))
            {
                recipient.Visibility = Visibility.Collapsed;
                recipientWatermark.Visibility = Visibility.Visible;
            }
        }

        /*
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void recipientWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            recipientWatermark.Visibility = Visibility.Collapsed;
            recipient.Visibility = Visibility.Visible;
            recipient.Focus();
        }

        /*
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void message_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(message.Text))
            {
                message.Visibility = Visibility.Collapsed;
                messageWatermark.Visibility = Visibility.Visible;
            }
        }

        /*
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void messageWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            messageWatermark.Visibility = Visibility.Collapsed;
            message.Visibility = Visibility.Visible;
            message.Focus();
        }

        /*
         * adds friend to DB if credentials match
         */
        private void addFriend(object sender, RoutedEventArgs e)
        {
          if(isValid())
            {
                if(isFriendCode())
                {
                    string insStmt = "INSERT INTO Friends (UserName, Friends) VALUES(@user, @friends);";

                    using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                    {
                        cnn.Open();
                        SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                        insCmd.Parameters.AddWithValue("@user", username);
                        insCmd.Parameters.AddWithValue("@friends", txtSearch.Text);
                        insCmd.ExecuteNonQuery();
                    }

                    insStmt = "INSERT INTO Friends (UserName, Friends) VALUES(@user, @friends);";

                    using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                    {
                        cnn.Open();
                        SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                        // use sqlParameters to prevent sql injection!
                        // values are retrieve from variables defined in program
                        insCmd.Parameters.AddWithValue("@user", txtSearch.Text);
                        insCmd.Parameters.AddWithValue("@friends", username);
                        insCmd.ExecuteNonQuery();
                    }

                    FillDataGrid();
                    txtSearch.Text = "";
                    txtFriendCode.Text = "";
                    MessageBox.Show("Now Friends!");
                }
                else
                {
                    MessageBox.Show("Wrong Friend Code");
                }
            }
          else
            {
                MessageBox.Show("User Doesn't Exist");
            }
        }

        /*
         * checks if user exists
         */
        public bool isValid()
        {
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                SqlCommand c = new SqlCommand("SELECT UserName FROM Users WHERE UserName = @user", cnn);
                c.Parameters.AddWithValue("@user", txtSearch.Text);

                using (SqlDataReader dr = c.ExecuteReader())
                {
                    if (dr.Read())
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

        /*
         * checks if friend code matches
         */
        public bool isFriendCode()
        {
            string code = "";

            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                SqlCommand c = new SqlCommand("SELECT FriendCode FROM Users WHERE UserName = @user", cnn);

                c.Parameters.AddWithValue("@user", txtSearch.Text);
                c.Parameters.AddWithValue("@code", txtFriendCode.Text);

                using (SqlDataReader dr = c.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        code = dr[0].ToString();

                        if (code.Equals(txtFriendCode.Text))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("User Doesn't Exist");
                        return false;
                    }
                }
            }
        }

        /*
         * add follow to user
         */
        private void addFollow(object sender, RoutedEventArgs e)
        {
            if (isValid())
            {
                string insStmt = "INSERT INTO Friends (UserName, Following) VALUES(@user, @following);";

                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    // values are retrieve from variables defined in program
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@following", txtSearch.Text);
                    insCmd.ExecuteNonQuery();
                }

                insStmt = "INSERT INTO Friends (UserName, Follower) VALUES(@user, @follower);";

                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    // values are retrieve from variables defined in program
                    insCmd.Parameters.AddWithValue("@user", txtSearch.Text);
                    insCmd.Parameters.AddWithValue("@follower", username);
                    insCmd.ExecuteNonQuery();
                }

                txtSearch.Text = "";
                MessageBox.Show("Now Following!");
            }
            else {
                MessageBox.Show("User Doesn't Exist");
            }
        }

        /*
         * sends message to user
         */
        private void sendMessage(object sender, RoutedEventArgs e)
        {
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                cnn.Open();
                SqlCommand c = new SqlCommand("SELECT UserName FROM Users WHERE UserName = @user", cnn);
                c.Parameters.AddWithValue("@user", recipient.Text);

                using (SqlDataReader dr = c.ExecuteReader())
                {
                    if (dr.Read())
                    {

                        string insStmt = "INSERT INTO Friends (UserName, Message, Recipient, ReadMessage, TimeSent) VALUES(@user, @message, @recipient, 'Sent', @time);";

                        using (SqlConnection cnn2 = new SqlConnection(sqlConnectionString))
                        {
                            cnn2.Open();
                            SqlCommand insCmd = new SqlCommand(insStmt, cnn2);
                            insCmd.Parameters.AddWithValue("@user", username);
                            insCmd.Parameters.AddWithValue("@message", message.Text);
                            insCmd.Parameters.AddWithValue("@recipient", recipient.Text);
                            insCmd.Parameters.AddWithValue("@time", DateTime.Now);
                            insCmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Message Successfully Sent");

                    }
                    else
                    {
                        MessageBox.Show("User Doesn't Exist");
                    }
                }
            }
        }

        /*
         * clear message fields
         */
        private void clearFields(object sender, RoutedEventArgs e)
        {
            message.Text = "";
            recipient.Text = "";
        }

        /*
         * view messages sent to user
         */
        public void viewMessages()
        {
            string CmdString = string.Empty;
            using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
            {
                CmdString = "SELECT * FROM Friends WHERE Recipient = @username AND ReadMessage = 'Sent'";

                SqlCommand cmd = new SqlCommand(CmdString, cnn);
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Activity");
                sda.Fill(dt);
                grdMessages.ItemsSource = dt.DefaultView;
            }
        }

        /*
         * saved messages by user
         */
        private void saveMessages(object sender, RoutedEventArgs e)
        {
            DataGrid grid = grdMessages as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

            if (dgr != null)
            {

                DataRowView dr = (DataRowView)dgr.Item;

                string message = dr[4].ToString();
                string sendUser = dr[0].ToString();

                String insStmt = "UPDATE Friends SET ReadMessage = 'Saved' WHERE Recipient = @user AND Message = @message AND UserName = @sendUser";

                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    // values are retrieve from variables defined in program
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@message", message);
                    insCmd.Parameters.AddWithValue("@sendUser", sendUser);

                    insCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Message Saved");
            }
        }

        /*
         * view saved messages by user
         */
        private void archivedMessages(object sender, RoutedEventArgs e)
        {
            if (archived.Content.Equals("Archived"))
            {
                string CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    CmdString = "SELECT * FROM Friends WHERE Recipient = @username AND ReadMessage = 'Saved'";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Activity");
                    sda.Fill(dt);
                    grdMessages.ItemsSource = dt.DefaultView;
                }
                archived.Content = "Inbox";
            }
            else if(archived.Content.Equals("Inbox"))
            {
                viewMessages();
                archived.Content = "Archived";
            }
        }

        /*
         * view messages details from message
         */
        private void showMessageDetails(object sender, RoutedEventArgs e)
        {
            DataGrid grid = grdMessages as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

            if (dgr != null)
            {
                DataRowView dr = (DataRowView)dgr.Item;
                MessageBox.Show("From: " + dr[0].ToString() + "\n" + "Date: " + Convert.ToDateTime(dr[7]).ToLongDateString() + "\n" + "Time: " + Convert.ToDateTime(dr[7]).ToShortTimeString());
            }
        }

        /*
         * deletes messages by user
         */
        private void deleteMessage(object sender, RoutedEventArgs e)
        {
            DataGrid grid = grdMessages as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

            if (dgr != null)
            {
                DataRowView dr = (DataRowView)dgr.Item;

                string message = dr[4].ToString();
                string sendUser = dr[0].ToString();

                String insStmt = "UPDATE Friends SET ReadMessage = 'Deleted' WHERE Recipient = @user AND Message = @message AND UserName = @sendUser"; ;

                using (SqlConnection cnn = new SqlConnection(sqlConnectionString))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    // use sqlParameters to prevent sql injection!
                    // values are retrieve from variables defined in program
                    insCmd.Parameters.AddWithValue("@user", username);
                    insCmd.Parameters.AddWithValue("@message", message);
                    insCmd.Parameters.AddWithValue("@sendUser", sendUser);

                    insCmd.ExecuteNonQuery();
                }

                viewMessages();
            }
        }
    }
}
