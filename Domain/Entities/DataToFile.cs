using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerArduino.Domain.Entities
{
    public class DataToFile: BaseEntity
    {
        ArduinoDevices? usedDeviceDetails { get; set; }
        string? data { get; set; }
    }
}
