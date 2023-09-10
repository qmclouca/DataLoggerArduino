using DataLoggerArduino.Domain.Entities;
using DataLoggerArduino.Domain.Enumerables;
using DataLoggerArduino.Infrastructure.Services;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace DataLoggerArduino
{
    public partial class MainWindow : Window
    {
        public static List<ArduinoDevices> _ArduinoDevicesConnected = new List<ArduinoDevices>();
        public static string selectedDevice = string.Empty;
        public static string selectedBaudRate = string.Empty;
        public MainWindow()
        {
            _ArduinoDevicesConnected = SerialCommunications.AutodetectArduinoPort();
            InitializeComponent();
            DeviceModelsPorts.ItemsSource = Devices();
            
        }

        private IEnumerable<string> Devices()
        {
            List<string> deviceNames = new List<string>();
            foreach(var device in _ArduinoDevicesConnected)
            {
                deviceNames.Add(device.name);
            }
            IEnumerable<string> devices = deviceNames;
            return devices;
        }
               

        private void ListBaudRatesAvailable(object sender, SelectionChangedEventArgs e)
        {
            string selectedDevice = string.Empty;
            List<string> allowableBaudRates = new List<string>();
            if (DeviceModelsPorts.SelectedItem != null)
            {
                selectedDevice = DeviceModelsPorts.SelectedItem.ToString();
            }
            ArduinoDevices arduinoSelected = _ArduinoDevicesConnected.FirstOrDefault(x => x.name == selectedDevice);

            BaudRates.ItemsSource = BaudRatesEnum.GetValues(typeof(BaudRatesEnum))
                                                 .Cast<BaudRatesEnum>()
                                                 .Where(b => (int)b <= arduinoSelected.maxBaudRate)
                                                 .Select(b => b.ToString())
                                                 .ToList();
        }

        private void SaveConnectionParameters(object sender, SelectionChangedEventArgs e)
        {
            
            if (DeviceModelsPorts.SelectedItem != null)
            {
                selectedDevice = DeviceModelsPorts.SelectedItem.ToString();
            }
            if (BaudRates.SelectedItem != null)
            {
                if(Enum.TryParse(BaudRates.SelectedItem.ToString(), out BaudRatesEnum br))
                {
                    selectedBaudRate = br.ToString();
                }                
            }                        
        }
        private void OnClickConnectDevice(object sender, RoutedEventArgs e)
        {
            ArduinoDevices arduinoSelected = _ArduinoDevicesConnected.FirstOrDefault(x => x.name == selectedDevice);

            SerialPort? serialPort = SerialCommunications.ConnectArduino(arduinoSelected.deviceId, Convert.ToInt32(Regex.Replace(selectedBaudRate, "[^0-9]","")));
            if(serialPort != null)
            {
                DeviceModelsPorts.IsEnabled = false;
                BaudRates.IsEnabled = false;
                ConnectDevice.Content = "Conectado";
            }
        }
    }
}
