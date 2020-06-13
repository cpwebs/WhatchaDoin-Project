using System;
using System.Collections.Generic;
using System.Data;
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
using Microsoft.Win32;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Navigation;

namespace WhatchaDoin
{
    /// <summary>
    /// Interaction logic for DiscoverScreen.xaml
    /// </summary>
    public partial class MemoriesScreen : Window
    {
        public string username;
        private List<DateTime> highlightedDates = new List<DateTime>();
        DataSet ds;
        string strName, imageName;
        string constr = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;";

        private List<String> selectedDayEvents = new List<String>();

        public MemoriesScreen(string user)
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

        private void calendarButton_Loaded(object sender, EventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            HighlightDay(button, date);
            button.DataContextChanged += new DependencyPropertyChangedEventHandler(calendarButton_DataContextChanged);
        }

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

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif)|*.jpg;*.bmp;*.gif";
                fldlg.ShowDialog();
                {
                    strName = fldlg.SafeFileName;
                    imageName = fldlg.FileName;
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            insertImageData();
        }

        private void insertImageData()
        {

            string activityName = activity.Text;
            string memories = memoriesBlock.Text;
            string askedAgain = again.Text;
            int eventRating = rating.Value;
            string recommending = recommend.Text;
            string askFeedback = feedback.Text;


            if (activityName.Length.Equals("") || activityName==null)
            {
                MessageBox.Show("Activity Not Entered");
            }

            else
            {

                try
                {
                    if (imageName != "")
                    {
                        //Initialize a file stream to read the image file
                        FileStream fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);

                        //Initialize a byte array with size of stream
                        byte[] imgByteArr = new byte[fs.Length];

                        //Read data from the file stream and put into the byte array
                        fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

                        //Close a file stream
                        fs.Close();

                        using (SqlConnection conn = new SqlConnection(constr))
                        {
                            conn.Open();
                            string sql = "INSERT into Image(id,img,UserName,Activity) VALUES('" + strName + "',@img, @user, @activity)";
                            using (SqlCommand cmd = new SqlCommand(sql, conn))
                            {
                                //Pass byte array into database
                                cmd.Parameters.Add(new SqlParameter("img", imgByteArr));
                                cmd.Parameters.AddWithValue("@user", username);
                                cmd.Parameters.AddWithValue("@activity", activityName);
                                /*
                                cmd.Parameters.AddWithValue("@memories", memories);
                                cmd.Parameters.AddWithValue("@askedAgain", askedAgain);
                                cmd.Parameters.AddWithValue("@eventRating", eventRating);
                                cmd.Parameters.AddWithValue("@recommending", recommending);
                                cmd.Parameters.AddWithValue("@askFeedback", askFeedback);
                                */
                                int result = cmd.ExecuteNonQuery();
                                if (result == 1)
                                {
                                    MessageBox.Show("Image added successfully.");
                                    BindImageList();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("Image extension is not supported!");
                }
            }
        }

        private void cbImages_DropDownOpened(object sender, EventArgs e)
        {
            BindImageList();
        }

        private void cbImages_DropDownClosed(object sender, EventArgs e)
        {
            DataTable dataTable = ds.Tables[0];

            foreach (DataRow row in dataTable.Rows)
            {
                if (row[0].ToString() == cbImages.SelectedItem.ToString())
                {
                    //Store binary data read from the database in a byte array
                    byte[] blob = (byte[])row[1];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;

                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();

                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    image2.Source = bi;

                }
            }
        }

        private void memoriesBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(memoriesBlock.Text))
            {
                memoriesBlock.Visibility = Visibility.Collapsed;
                memoriesBlockWatermark.Visibility = Visibility.Visible;
            }
        }

        private void memoriesBlockWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            memoriesBlockWatermark.Visibility = Visibility.Collapsed;
            memoriesBlock.Visibility = Visibility.Visible;
            memoriesBlock.Focus();
        }

        private void again_DropDownOpened(object sender, EventArgs e)
        {
            again.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void feedback_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(feedback.Text))
            {
                feedback.Visibility = Visibility.Collapsed;
                feedbackWatermark.Visibility = Visibility.Visible;
            }
        }

        private void feedbackWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            feedbackWatermark.Visibility = Visibility.Collapsed;
            feedback.Visibility = Visibility.Visible;
            feedback.Focus();
        }

        private void activity_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(activity.Text))
            {
                activity.Visibility = Visibility.Collapsed;
                activityWatermark.Visibility = Visibility.Visible;
            }
        }

        private void activityWatermark_GotFocus(object sender, RoutedEventArgs e)
        {
            activityWatermark.Visibility = Visibility.Collapsed;
            activity.Visibility = Visibility.Visible;
            activity.Focus();
        }

        private void loadMemories(object sender, RoutedEventArgs e)
        {
            string activityName = activity.Text;

            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    string CmdString = "SELECT * FROM Image WHERE UserName = @username AND Activity = @activity";

                    SqlCommand cmd = new SqlCommand(CmdString, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@activity", activityName);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        ds = new DataSet("myDataSet");
                        adapter.Fill(ds);
                        DataTable dt = ds.Tables[0];

                        foreach (DataRow dr in dt.Rows)
                        {
                            memoriesBlock.Text = dr[4].ToString();
                            again.Text = dr[5].ToString();
                            rating.Value = Convert.ToInt32(dr[6]);
                            recommend.Text = dr[7].ToString();
                            feedback.Text = dr[8].ToString();
                        }

                        activityWatermark_GotFocus(this, new RoutedEventArgs());
                        feedbackWatermark_GotFocus(this, new RoutedEventArgs());
                        memoriesBlockWatermark_GotFocus(this, new RoutedEventArgs());

                        again.Foreground = new SolidColorBrush(Colors.Black);

                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void recommend_DropDownOpened(object sender, EventArgs e)
        {
            recommend.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(cbImages.Text.Equals("Please Select"))
            {
                MessageBox.Show("Image Not Selected");
            }
            else if (cbImages.Text.Equals(""))
            {
                MessageBox.Show("Image Does Not Exist");
            }
            else
            {
                string imageID = cbImages.Text;
                string eventName = activity.Text;

                String CmdString = string.Empty;

                try
                {
                    using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                    {
                        CmdString = "DELETE FROM Image WHERE UserName = @username AND Activity = @eventName AND id = @imageID";

                        SqlCommand cmd = new SqlCommand(CmdString, cnn);
                        // values are retrieve from variables defined in program
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@eventName", eventName);
                        cmd.Parameters.AddWithValue("@imageID", imageID);

                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable("Activity");
                        sda.Fill(dt);

                        cbImages_DropDownClosed(this, new EventArgs());
                        cbImages_DropDownOpened(this, new EventArgs());

                        image2.Source = null;

                        MessageBox.Show("Photo Successfully Deleted");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void addMemory(object sender, RoutedEventArgs e)
        {

            string activityName = activity.Text;
            string memories = memoriesBlock.Text;
            string askedAgain = again.Text;
            int eventRating = rating.Value;
            string recommending = recommend.Text;
            string askFeedback = feedback.Text;

            SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
            sqlCon.Open();

            String insStmt = "INSERT INTO Image (UserName,Activity,SpecificMemories,Again,Rating, Recommendation, Feedback) VALUES( @user, @activity, @memories, @askedAgain, @eventRating, @recommending, @askFeedback);";

            using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
            {
                cnn.Open();
                SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                // use sqlParameters to prevent sql injection!
                // values are retrieve from variables defined in program
                insCmd.Parameters.AddWithValue("@user", username);
                insCmd.Parameters.AddWithValue("@activity", activityName);
                insCmd.Parameters.AddWithValue("@memories", memories);
                insCmd.Parameters.AddWithValue("@askedAgain", askedAgain);
                insCmd.Parameters.AddWithValue("@eventRating", eventRating);
                insCmd.Parameters.AddWithValue("@recommending", recommending);
                insCmd.Parameters.AddWithValue("@askFeedback", askFeedback);

                insCmd.ExecuteNonQuery();
                MessageBox.Show("Memory successfully created!");
            }
        }

        private void clearFields(object sender, RoutedEventArgs e)
        {
            memoriesBlock.Text = "";
            again.Text = "";
            rating.Value = 0;
            recommend.Text = "";
            feedback.Text = "";
        }

        private void BindImageList()
        {
            string activityName = activity.Text;

            if (activityName.Equals("") || activityName == null)
            {
                MessageBox.Show("Activity Not Entered");
            }

            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();

                        string CmdString = "SELECT * FROM Image WHERE UserName = @username AND Activity = @activity";

                        SqlCommand cmd = new SqlCommand(CmdString, conn);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@activity", activityName);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            ds = new DataSet("myDataSet");
                            adapter.Fill(ds);
                            DataTable dt = ds.Tables[0];

                            cbImages.Items.Clear();

                            foreach (DataRow dr in dt.Rows)
                                cbImages.Items.Add(dr["id"].ToString());

                            cbImages.SelectedIndex = 0;
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}
