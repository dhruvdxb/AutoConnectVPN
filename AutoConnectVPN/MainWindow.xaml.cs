using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;  // Add this namespace for network interfaces
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AutoConnectVPN
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadAvailableNetworks(); // Load networks on startup
        }

       
        //private void ToggleVPN_Click(object sender, RoutedEventArgs e)
        //{
        //    ToggleButton toggle = sender as ToggleButton;
        //    MessageBox.Show(toggle.IsChecked == true ? "VPN Enabled" : "VPN Disabled");
        //}

       
        private void MarkAsTrusted_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            MessageBox.Show($"You marked {button.DataContext} as a trusted network.");
        }

       
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Going back...");
        }

        private void LoadAvailableNetworks()
        {
            List<string> networks = GetAvailableNetworks();
            NetworkList.ItemsSource = networks;
        }

      
      
        private void RefreshNetworks_Click(object sender, RoutedEventArgs e)
        {
            LoadAvailableNetworks();
        }

       
        private List<string> GetAvailableNetworks()
        {
            List<string> networkNames = new List<string>();

            try
            {
                // Use NetworkInterface to get all network interfaces
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var networkInterface in networkInterfaces)
                {
                    // Exclude the loopback and other non-physical interfaces
                    if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel ||
                        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Unknown)
                        continue;

                    string type = networkInterface.NetworkInterfaceType.ToString();
                    string status = networkInterface.OperationalStatus.ToString();
                    networkNames.Add($"{networkInterface.Name} ({type}) - {status}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching networks: " + ex.Message);
            }

            return networkNames;
        }
    }
}
