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
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using DataLoggerArduino.Presentation;

namespace DataLoggerArduino
{
    public partial class MainWindow : Window
    {
        public static List<ArduinoDevices> _ArduinoDevicesConnected = new List<ArduinoDevices>();
        public static string selectedDevice = string.Empty;
        public static string selectedBaudRate = string.Empty;
        public static SerialPort serialPort = null;
        public static Graph3D graph3D = new Graph3D();
        public MainWindow()
        {
            _ArduinoDevicesConnected = SerialCommunications.AutodetectArduinoPort();
            InitializeComponent();
            DeviceModelsPorts.ItemsSource = Devices();
            graph3D.Show();
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
        private string ReadIncomeDataDevice(SerialPort serialPort)
        {
            string actualData = SerialCommunications.ReadData(serialPort);
            return actualData;            
        }
        private void OnClickConnectDevice(object sender, RoutedEventArgs e)
        {
            ArduinoDevices arduinoSelected = _ArduinoDevicesConnected.FirstOrDefault(x => x.name == selectedDevice);

            serialPort = SerialCommunications.ConnectArduino(arduinoSelected.deviceId, Convert.ToInt32(Regex.Replace(selectedBaudRate, "[^0-9]","")));
            DeviceModelsPorts.IsEnabled = false;
            BaudRates.IsEnabled = false;
            ConnectDevice.Content = "Conectado";
            ConnectDevice.IsEnabled = false;
            Monitorar.IsEnabled = true;            
        }
              

        private async void Monitor(object sender, RoutedEventArgs e)
        {            
            await Task.Run(() =>
            {

                
                while (true)  // Cuidado com loops infinitos, pode ser uma boa ideia adicionar uma condição de saída.
                {
                    string input = ReadIncomeDataDevice(serialPort);
                    if (serialPort != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            IncomeData.Text = IncomeData.Text + "\n" + input;  // Use \n ao invés de /n para nova linha
                        });
                    }   
                    Task.Delay(500).Wait();  // Dá uma pequena pausa para não sobrecarregar a CPU.
                }
            });
        }       
    }
}