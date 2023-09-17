﻿using DataLoggerArduino.Domain.Entities;
using DataLoggerArduino.Domain.Enumerables;
using DataLoggerArduino.Infrastructure.Services;
using DataLoggerArduino.Presentation;
using HelixToolkit.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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
                            if (input != null)
                            {
                                try
                                {
                                    Point3DCartesian point = JsonConvert.DeserializeObject<Point3DCartesian>(input);
                                    if (point != null && point.X != null && point.Y != null && point.Z != null)
                                    {
                                        var sphere = new SphereVisual3D
                                        {
                                            Center = new Point3D(point.X, point.Y, point.Z),
                                            Radius = 0.5, // Ajuste o raio conforme necessário
                                            Fill = Brushes.White // A cor da esfera
                                        };
                                        graph3D.ViewPort3D.Children.Add(sphere);
                                    }
                                    else throw new Exception();
                                }
                                catch (Exception)
                                {
                                       
                                }                                
                            }
                           
                        });
                    }
                    
                    Task.Delay(500).Wait();  // Dá uma pequena pausa para não sobrecarregar a CPU.
                }
            });
        }

        private void OnClickResetCoordinates(object sender, RoutedEventArgs e)
        {
            SerialCommunications.SendData(serialPort, "Z");
            IncomeData.Text = string.Empty;
            foreach (var visual in graph3D.ViewPort3D.Children.OfType<SphereVisual3D>().ToList())
            {
                graph3D.ViewPort3D.Children.Remove(visual);
            }
        }

        private void OnClickResetArduino(object sender, RoutedEventArgs e)
        {
            SerialCommunications.SendData(serialPort, "R");
            IncomeData.Text = string.Empty; 
            foreach (var visual in graph3D.ViewPort3D.Children.OfType<SphereVisual3D>().ToList())
            {
                graph3D.ViewPort3D.Children.Remove(visual);
            }
        }

        private void IncreaseReadRate(object sender, RoutedEventArgs e)
        {
            SerialCommunications.SendData(serialPort, "+");
        }

        private void DecreaseReadRate(object sender, RoutedEventArgs e)
        {
            SerialCommunications.SendData(serialPort, "-");
        }

        private void IncomeDataChanged(object sender, TextChangedEventArgs e)
        {
            IncomeData.ScrollToEnd();
        }
    }
}