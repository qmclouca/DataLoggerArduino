using DataLoggerArduino.Infrastructure.Services;
using System.Windows;

namespace DataLoggerArduino
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SerialCommunications.AutodetectArduinoPort();
            InitializeComponent();
        }
    }
}
