﻿using System;
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

namespace WhatchaDoin
{
    /// <summary>
    /// Interaction logic for DiscoverScreen.xaml
    /// </summary>
    public partial class DiscoverScreen : Window
    {
        public string username;

        public DiscoverScreen(string user)
        {
            username = user;
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;

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
                    main.Show();
                    this.Close();
                    break;
                case "Events":
                    EventScreen events = new EventScreen(username);
                    events.Show();
                    this.Close();
                    break;
                case "Friends":
                    FriendScreen friend = new FriendScreen(username);
                    friend.Show();
                    this.Close();
                    break;
                case "Discover":
                    DiscoverScreen discover = new DiscoverScreen(username);
                    discover.Show();
                    this.Close();
                    break;
                case "Settings":
                    SettingsScreen settings = new SettingsScreen(username);
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
    }
}