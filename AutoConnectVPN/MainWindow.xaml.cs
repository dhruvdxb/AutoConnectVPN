using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Management;
using System.Windows.Media.Imaging;

namespace AutoConnectVPN
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadAvailableNetworks(); // Load networks on startup
        }

        private void ToggleVPN_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            MessageBox.Show(toggle.IsChecked == true ? "VPN Enabled" : "VPN Disabled");
        }

        private void MarkAsTrusted_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkList.SelectedItem is NetworkItem selectedNetwork)
            {
                MessageBox.Show($"You marked {selectedNetwork.Name} as a trusted network.");
            }
            else
            {
                MessageBox.Show("Please select a network to mark as trusted.");
            }
        }


        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Going back...");
        }

        private void LoadAvailableNetworks()
        {
            List<NetworkItem> networks = GetAvailableNetworks();
            NetworkList.ItemsSource = networks;
        }

        private void RefreshNetworks_Click(object sender, RoutedEventArgs e)
        {
            LoadAvailableNetworks();
        }

        private List<NetworkItem> GetAvailableNetworks()
        {
            List<NetworkItem> networkItems = new List<NetworkItem>();

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

                    string networkName = networkInterface.Name;
                    string imageSource = "pack://application:,,,/Asset/Ethernet.png"; // Default to ethernet image

                    if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        string ssid = GetSSID(networkInterface);
                        networkName = !string.IsNullOrEmpty(ssid) ? ssid : networkName;
                        imageSource = "pack://application:,,,/Asset/WifiConnected.png"; // Use wifi image for wireless networks
                    }

                    networkItems.Add(new NetworkItem { Name = networkName, ImageSource = new BitmapImage(new Uri(imageSource)) });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching networks: " + ex.Message);
            }

            return networkItems;
        }

        private string GetSSID(NetworkInterface networkInterface)
        {
            if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
                return null;

            try
            {
                var query = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSNdis_80211_ServiceSetIdentifier");
                var queryCollection = query.Get();

                foreach (ManagementObject m in queryCollection)
                {
                    var ssid = (byte[])m["Ndis80211SsId"];
                    return System.Text.Encoding.ASCII.GetString(ssid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching SSID: " + ex.Message);
            }

            return null;
        }
    }

    public class NetworkItem
    {
        public string Name { get; set; }
        public BitmapImage ImageSource { get; set; }
    }
}