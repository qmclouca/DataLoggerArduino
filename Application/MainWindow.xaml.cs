using DataLoggerArduino.Domain.Entities;
using DataLoggerArduino.Infrastructure.Services;
using System.Collections.Generic;
using System.Windows;

namespace DataLoggerArduino
{
    public partial class MainWindow : Window
    {
        public static List<ArduinoDevices> _ArduinoDevicesConnected = new List<ArduinoDevices>();
        public MainWindow()
        {
            _ArduinoDevicesConnected = SerialCommunications.AutodetectArduinoPort();
            InitializeComponent();
        }
    }
}
