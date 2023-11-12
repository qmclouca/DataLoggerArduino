using DataLoggerArduino.Domain.Entities;
using DataLoggerArduino.Domain.Enumerables;
using DataLoggerArduino.Infrastructure.Services;
using DataLoggerArduino.Presentation;
using HelixToolkit.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
        public static StringBuilder data = new StringBuilder();
        public static List<Point3D> points = new List<Point3D>();
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
            List<Point3D> points = new List<Point3D>();
            MeshBuilder meshBuilder = new MeshBuilder(false, false);
            Model3DGroup modelGroup = new Model3DGroup();

            await Task.Run(async () =>
            {
                while (true)
                {
                    string input = ReadIncomeDataDevice(serialPort);
                    if (serialPort != null && !string.IsNullOrWhiteSpace(input))
                    {
                        // Limpa o input antes de processá-lo
                        string inputCleaned = input.Trim(new char[] { '\r', '\n' });

                        DataFromArduino data = null;
                        try
                        {
                            // Deserializa fora do Dispatcher para evitar operações desnecessárias na UI thread
                            data = JsonConvert.DeserializeObject<DataFromArduino>(inputCleaned);
                        }
                        catch (Exception ex)
                        {
                            // Lida com a exceção
                            Debug.WriteLine("Exception occurred: " + ex.Message);
                        }

                        if (data != null)
                        {
                            // Realizando a conversão de esféricas para cartesianas
                            double angulo1Rad = data.angulo1 * (Math.PI / 180);
                            double angulo2Rad = data.angulo2 * (Math.PI / 180);
                            double x = data.distancia * Math.Sin(angulo2Rad) * Math.Cos(angulo1Rad);
                            double y = data.distancia * Math.Sin(angulo2Rad) * Math.Sin(angulo1Rad);
                            double z = data.distancia * Math.Cos(angulo2Rad);

                            Point3D point3D = new Point3D(x, y, z);

                            this.Dispatcher.Invoke(() =>
                            {
                                // Atualiza a UI com os dados recebidos
                                IncomeData.AppendText(input + "\n");
                                IncomeData.ScrollToEnd(); // Isso vai rolar o texto para o final para mostrar os dados mais recentes

                                points.Add(point3D);

                                if (points.Count >= 10)
                                {
                                    meshBuilder = new MeshBuilder(false, false);
                                    foreach (Point3D point in points)
                                    {
                                        meshBuilder.Positions.Add(point);
                                    }
                                    for (int i=0; i<points.Count-2; i++)
                                    {
                                        meshBuilder.AddTriangle(points[i], points[i + 1], points[i + 2]);
                                    }
                                    meshBuilder.ComputeNormalsAndTangents(MeshFaces.Default, false);

                                    // Aqui vamos adicionar a lógica para construir a malha
                                    // ...

                                    var mesh = meshBuilder.ToMesh(true);
                                    var geometryModel = new GeometryModel3D
                                    {
                                        Geometry = mesh,
                                        Material = MaterialHelper.CreateMaterial(Brushes.White),
                                        BackMaterial = MaterialHelper.CreateMaterial(Brushes.White)
                                    };
                                    var modelVisual3D = new ModelVisual3D
                                    {
                                        Content = geometryModel
                                    };
                                    
                                    Model3D.ViewPort3D.Children.Add(modelVisual3D);

                                    points.Clear();
                                }
                            });
                        }
                    }
                    await Task.Delay(500); // Await é importante aqui para evitar bloquear a thread
                }
            });
        }

        private void OnClickResetCoordinates(object sender, RoutedEventArgs e)
        {
            SerialCommunications.SendData(serialPort, "Z");
            IncomeData.Text = string.Empty;
            foreach (var visual in Model3D.ViewPort3D.Children.OfType<SphereVisual3D>().ToList())
            {
                Model3D.ViewPort3D.Children.Remove(visual);
            }
        }

        private void OnClickResetArduino(object sender, RoutedEventArgs e)
        {
            SerialCommunications.SendData(serialPort, "R");
            IncomeData.Text = string.Empty; 
            foreach (var visual in Model3D.ViewPort3D.Children.OfType<SphereVisual3D>().ToList())
            {
                Model3D.ViewPort3D.Children.Remove(visual);
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

        private void SaveDataFile(object sender, RoutedEventArgs e)
        {
            string pathToSaveCsv = GenerateUniqueFileName();
            File.WriteAllText(GenerateUniqueFileName(), data.ToString());
        }

        private string GenerateUniqueFileName()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;  // Diretório do aplicativo, você pode ajustar isso conforme necessário.
            string datePart = DateTime.Now.ToString("ddMMyyyy");
            int counter = 1;

            string fullFileName = "";

            do
            {
                fullFileName = Path.Combine(basePath, $"ArduinoLogger_{datePart}_{counter}.csv");
                counter++;
            }
            while (File.Exists(fullFileName));

            return fullFileName;
        }
    }
}