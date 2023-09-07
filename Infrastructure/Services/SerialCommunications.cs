using System.Collections.Generic;
using System.IO.Ports;
using System.Management;

namespace DataLoggerArduino.Infrastructure.Services
{
    public class SerialCommunications
    {
        SerialPort serialPort = new SerialPort();        
        
        private SerialCommunications()
        {
            serialPort.PortName = "COM3";
            serialPort.BaudRate = 9600;
            serialPort.Open();
        }
        public static string AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

            return null;
        }
    }
}
