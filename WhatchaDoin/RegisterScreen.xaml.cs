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
using System.Data.SqlClient;
using System.Data;

namespace WhatchaDoin
{
    /// <summary>
    /// Interaction logic for RegisterScreen.xaml
    /// </summary>
    public partial class RegisterScreen : Window
    {
        public RegisterScreen()
        {
            InitializeComponent();
        }

        /*
         * registers user to DB and application and checks to make sure credentials are valid
         */
        private void btnSubmitRegister_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text;
            string password = txtPassword.Password;
            string passwordRepeated = txtPasswordRepeat.Password;
            DateTime today = DateTime.Today;
            string privacy = "Public";

            if (password == passwordRepeated)
            {
                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                SqlCommand cmd = new SqlCommand("SELECT * from Users where UserName=@User", sqlCon);
                cmd.Parameters.AddWithValue("@User", txtUsername.Text);
                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.HasRows)
                {
                    String insStmt = "INSERT INTO Users (UserName, Password, FriendCode, Joined, Privacy) VALUES( @user, @password, ROUND(RAND() * 1000, 0), @joined, @privacy);";

                    using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                    {
                        cnn.Open();
                        SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                        // use sqlParameters to prevent sql injection!
                        insCmd.Parameters.AddWithValue("@user", user);
                        insCmd.Parameters.AddWithValue("@password", password);
                        insCmd.Parameters.AddWithValue("@joined", today);
                        insCmd.Parameters.AddWithValue("@privacy", privacy);
                        insCmd.ExecuteNonQuery();
                        MessageBox.Show("Account successfully created!");
                        LoginScreen login = new LoginScreen();
                        login.Show();
                        this.Close();
                    }
                }

                else
                {
                    MessageBox.Show("Username has been taken!");
                }
            }

            else
            {
                MessageBox.Show("Passwords don't match!");
            }
        }

        /*
         * cancels registeration and directs to login page
         */
        private void btnSubmitCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginScreen login = new LoginScreen();
            login.Show();
            this.Close();
        }

        /*
         * Various funtions allow closing, maximizing, etc. capabilities of window
         */
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
            }
        }
    }
}
