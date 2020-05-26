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

        private void btnSubmitRegister_Click(object sender, RoutedEventArgs e)
        {
            String user = txtUsername.Text;
            String password = txtPassword.Password;
            String passwordRepeated = txtPasswordRepeat.Password;

            if (password == passwordRepeated)
            {
                SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;");
                sqlCon.Open();

                SqlCommand cmd = new SqlCommand("SELECT * from Users where UserName=@User", sqlCon);
                cmd.Parameters.AddWithValue("@User", txtUsername.Text);
                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.HasRows)
                {
                    String insStmt = "INSERT INTO Users (UserName, Password) VALUES( @user, @password);";

                    using (SqlConnection cnn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LoginDB; Integrated Security=True;"))
                    {
                        cnn.Open();
                        SqlCommand insCmd = new SqlCommand(insStmt, cnn);
                        // use sqlParameters to prevent sql injection!
                        insCmd.Parameters.AddWithValue("@user", txtUsername.Text);
                        insCmd.Parameters.AddWithValue("@password", txtPassword.Password);
                        insCmd.ExecuteNonQuery();
                        MessageBox.Show("Account successfully created!");
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

        private void btnSubmitCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginScreen login = new LoginScreen();
            login.Show();
            this.Close();
        }


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
