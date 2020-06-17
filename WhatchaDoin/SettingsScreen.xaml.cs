using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.MessageBox;

namespace WhatchaDoin
{
    /// <summary>
    /// Interaction logic for SettingsScreen.xaml
    /// </summary>
    public partial class SettingsScreen : Window
    {
        //username of user currently using application
        private string username;

        //SQL Server connection string
        string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;";

        //helps load values from SQL
        DataSet ds;

        //current Password of user
        string currentPassword;

        //current privacy of user
        string privacy;

        //current date of user
        DateTime currentDate;

        //friend code of user
        int currentFriendCode;


        public SettingsScreen(string user)
        {
            username = user;
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;
            loadLabelValues();
            savedPrivacy();
        }

        /*
         * sets all user statistics
         */
        public void loadLabelValues()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();

                    string CmdString = "SELECT * FROM Users WHERE UserName = @username";

                    SqlCommand cmd = new SqlCommand(CmdString, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        ds = new DataSet("myDataSet");
                        adapter.Fill(ds);
                        DataTable dt = ds.Tables[0];

                        foreach (DataRow dr in dt.Rows)
                        {
                            lblUsername.Content = "Username: " + dr[0].ToString();

                            int lengthPassword = dr[1].ToString().Length;
                            currentPassword = dr[1].ToString();
                            string password = "";
                            for(int i = 0; i < lengthPassword; i++)
                            {
                                password += "*";
                            }
                            lblPassword.Content = "Password: " + password;
                            lblFriendCode.Content = "Friend Code: " + dr[2].ToString();
                            currentFriendCode = Convert.ToInt32(dr[2]);
                            lblJoined.Content = "Joined: " + Convert.ToDateTime(dr[3]).ToShortDateString();
                            currentDate = Convert.ToDateTime(dr[3]);
                            lblPrivacy.Content = "Privacy: " + dr[4].ToString();
                            privacy = dr[4].ToString();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
         * saves and updates new password in account and DB
         */
        private void savePassword(object sender, RoutedEventArgs e)
        {
            if(newPassword.Password.Equals(repeatPassword.Password))
            {
                if(currentPassword.Equals(oldPassword.Password))
                {
                    SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                    sqlCon.Open();

                    String insStmt = "UPDATE Users SET UserName = @user, Password = @password, FriendCode = @friendCode, Joined = @date, Privacy = @privacy WHERE UserName = @user";

                    using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                    {
                        cnn.Open();
                        SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                        // use sqlParameters to prevent sql injection!
                        // values are retrieve from variables defined in program
                        insCmd.Parameters.AddWithValue("@user", username);
                        insCmd.Parameters.AddWithValue("@password", newPassword.Password);
                        insCmd.Parameters.AddWithValue("@friendCode", currentFriendCode);
                        insCmd.Parameters.AddWithValue("@date", currentDate);
                        insCmd.Parameters.AddWithValue("@privacy", privacy);

                        insCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Password Successfully Changed");
                }
            }
            else
            {
                MessageBox.Show("New Passwords Don't Match");
            }
        }

        /*
         * clears password fields
         */
        private void cancelPassword(object sender, RoutedEventArgs e)
        {
            oldPassword.Password = "";
            newPassword.Password = "";
            repeatPassword.Password = "";
        }

        /*
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void oldPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(oldPassword.Password))
            {
                oldPassword.Visibility = Visibility.Collapsed;
                oldPasswordWatermark.Visibility = Visibility.Visible;
            }
        }

        /*
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void oldPasswordWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            oldPasswordWatermark.Visibility = Visibility.Collapsed;
            oldPassword.Visibility = Visibility.Visible;
            oldPassword.Focus();
        }

        /*
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void newPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(newPassword.Password))
            {
                newPassword.Visibility = Visibility.Collapsed;
                newPasswordWatermark.Visibility = Visibility.Visible;
            }
        }

        /*
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void newPasswordWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            newPasswordWatermark.Visibility = Visibility.Collapsed;
            newPassword.Visibility = Visibility.Visible;
            newPassword.Focus();
        }

        /*
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void repeatPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(repeatPassword.Password))
            {
                repeatPassword.Visibility = Visibility.Collapsed;
                repeatPasswordWatermark.Visibility = Visibility.Visible;
            }
        }

        /*
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void repeatPasswordWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            repeatPasswordWatermark.Visibility = Visibility.Collapsed;
            repeatPassword.Visibility = Visibility.Visible;
            repeatPassword.Focus();
        }

        /*
         * loads current privacy as the selected value
         */
        public void savedPrivacy()
        {
            if(privacy.Equals(firstRadio.Content))
            {
                firstRadio.IsChecked = true;
            }

            else if (privacy.Equals(secondRadio.Content))
            {
                secondRadio.IsChecked = true;
            }

            else if (privacy.Equals(thirdRadio.Content))
            {
                thirdRadio.IsChecked = true;
            }
        }

        /*
         * updates new privacy to DB
         */
        private void savePrivacy(object sender, RoutedEventArgs e)
        {
            if(firstRadio.IsChecked==true)
            {
                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "UPDATE Users SET Privacy = 'Private' WHERE Username = @user";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    insCmd.Parameters.AddWithValue("@user", username);

                    insCmd.ExecuteNonQuery();
                }
                firstRadio.IsChecked = true;
                MessageBox.Show("Privacy Saved");
            }

            else if(secondRadio.IsChecked == true)
            {
                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "UPDATE Users SET Privacy = 'Friends' WHERE Username = @user";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    insCmd.Parameters.AddWithValue("@user", username);

                    insCmd.ExecuteNonQuery();
                }
                secondRadio.IsChecked = true;
                MessageBox.Show("Privacy Saved");
            }

            else if(thirdRadio.IsChecked == true)
            {
                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                String insStmt = "UPDATE Users SET Privacy = 'Public' WHERE Username = @user";

                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                    insCmd.Parameters.AddWithValue("@user", username);

                    insCmd.ExecuteNonQuery();
                }
                thirdRadio.IsChecked = true;
                MessageBox.Show("Privacy Saved");
            }
        }

        /*
         * deletes account and all everything associated with it
         */
        private void deleteAccount(object sender, RoutedEventArgs e)
        {
            if(currentPassword.Equals(confirmPassword.Password) && username.Equals(confirmUsername.Text))
            {
                String CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    CmdString = "DELETE FROM Users WHERE UserName = @username";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    // values are retrieve from variables defined in program
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }

                CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    CmdString = "DELETE FROM BucketList WHERE UserName = @username";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    // values are retrieve from variables defined in program
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }

                CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    CmdString = "DELETE FROM Friends WHERE UserName = @username";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    // values are retrieve from variables defined in program
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }

                CmdString = string.Empty;
                using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    cnn.Open();
                    CmdString = "DELETE FROM Image WHERE UserName = @username";

                    SqlCommand cmd = new SqlCommand(CmdString, cnn);
                    // values are retrieve from variables defined in program
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }

                LoginScreen login = new LoginScreen();
                login.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Credentials Incorrect");
            }

        }

        /*
         * clears delete account fields
         */
        private void cancelDeleteAccount(object sender, RoutedEventArgs e)
        {
            confirmUsername.Text = "";
            confirmPassword.Password = "";
        }

        /*
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void confirmUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(confirmUsername.Text))
            {
                confirmUsername.Visibility = Visibility.Collapsed;
                confirmUsernameWatermark.Visibility = Visibility.Visible;
            }
        }

        /*
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void confirmUsernameWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            confirmUsernameWatermark.Visibility = Visibility.Collapsed;
            confirmUsername.Visibility = Visibility.Visible;
            confirmUsername.Focus();
        }

        /*
         * reinstates watermark when textbox is empty or displays textbox when value is entered
         */
        private void confirmPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(confirmPassword.Password))
            {
                confirmPassword.Visibility = Visibility.Collapsed;
                confirmPasswordWatermark.Visibility = Visibility.Visible;
            }
        }

        /*
         * focuses and makes the textbox visible when watermark box is clicked
         */
        private void confirmPasswordWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            confirmPasswordWatermark.Visibility = Visibility.Collapsed;
            confirmPassword.Visibility = Visibility.Visible;
            confirmPassword.Focus();
        }
    }
}
