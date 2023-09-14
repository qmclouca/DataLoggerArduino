using DataLoggerArduino.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Windows;

namespace DataLoggerArduino.Infrastructure.Services
{
    public class SerialCommunications
    {   
        private SerialCommunications()
        {
            
        }

        public static SerialPort? ConnectArduino(string comPort, int baudRate)
        {
            try
            {
                SerialPort serialPort = new SerialPort();
                serialPort.PortName = comPort;
                serialPort.BaudRate = baudRate;
                serialPort.Open();                
                return serialPort;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static List<ArduinoDevices> AutodetectArduinoPort()
        {
            List<ArduinoDevices> arduinoDevices = new List<ArduinoDevices>();
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);
            
            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    ArduinoDevices device = new ArduinoDevices();
                    device.description = item["Description"].ToString();
                    device.deviceId = item["DeviceID"].ToString();
                    device.name = item["Name"].ToString();
                    device.caption = item["Caption"].ToString();
                    device.status = item["Status"].ToString();
                    device.systemCreationClassName = item["SystemCreationClassName"].ToString();
                    device.systemName = item["SystemName"].ToString();
                    device.providerType = item["ProviderType"].ToString();
                    device.pnpDeviceId = item["PNPDeviceID"].ToString();
                    device.osAutoDiscovered = Convert.ToBoolean(item["OSAutoDiscovered"]);
                    device.powerManagementSupported = Convert.ToBoolean(item["PowerManagementSupported"]);
                    device.powerManagementCapabilities = item["PowerManagementCapabilities"].ToString();
                    device.availability = Convert.ToInt32(item["Availability"]);
                    device.statusInfo = Convert.ToInt32(item["StatusInfo"]);
                    device.binary = Convert.ToBoolean(item["Binary"]);
                    device.maxBaudRate = Convert.ToInt32(item["MaxBaudRate"]);
                    device.settableBaudRate = Convert.ToBoolean(item["SettableBaudRate"]);
                    device.settableDataBits = Convert.ToBoolean(item["SettableDataBits"]);
                    device.settableFlowControl = Convert.ToBoolean(item["SettableFlowControl"]);
                    device.settableParity = Convert.ToBoolean(item["SettableParity"]);
                    device.settableParityCheck = Convert.ToBoolean(item["SettableParityCheck"]);
                    device.settableRLSD = Convert.ToBoolean(item["SettableRLSD"]);
                    device.settableStopBits = Convert.ToBoolean(item["SettableStopBits"]);
                    device.supports16BitMode = Convert.ToBoolean(item["Supports16BitMode"]);
                    device.supportsDTRDSR = Convert.ToBoolean(item["SupportsDTRDSR"]);
                    device.supportsElapsedTimeouts = Convert.ToBoolean(item["SupportsElapsedTimeouts"]);
                    device.supportsIntTimeouts = Convert.ToBoolean(item["SupportsIntTimeouts"]);
                    device.supportsParityCheck = Convert.ToBoolean(item["SupportsParityCheck"]);
                    device.supportsRLSD = Convert.ToBoolean(item["SupportsRLSD"]);
                    device.supportsRTSCTS = Convert.ToBoolean(item["SupportsRTSCTS"]);
                    device.supportsSpecialCharacters = Convert.ToBoolean(item["SupportsSpecialCharacters"]);
                    device.supportsXOnXOff = Convert.ToBoolean(item["SupportsXOnXOff"]);
                    device.supportsXOnXOffSet = Convert.ToBoolean(item["SupportsXOnXOffSet"]);                    
                    
                    if (device.description != null && device.description.Contains("Arduino"))
                    {
                        arduinoDevices.Add(device);
                    }
                    return arduinoDevices;
                }
            }
            catch (ManagementException)
            {
                MessageBox.Show("Nenhum dispositivo de aquisição de dados localizado no sistema.", "Detecção de dispositivos", MessageBoxButton.OK);
            }
            return arduinoDevices;
        }

        public static string ReadData(SerialPort serialPort)
        {
            if(serialPort != null)
            {
                string data = serialPort.ReadLine();
                return data;
            }
            return string.Empty;
            
        }
    }
}
