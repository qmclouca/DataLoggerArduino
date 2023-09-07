namespace DataLoggerArduino.Domain.Entities
{
    public class ArduinoDevices: BaseEntity
    {
        public int? availability { get; set; }
        public bool? binary { get; set; }
        public string? caption { get; set; }
        public string? description { get; set; }
        public string? deviceId { get; set; }
        public int? maxBaudRate { get; set; }
        public string? name { get; set; }
        public bool? osAutoDiscovered { get; set; }
        public string? pnpDeviceId { get; set; }
        public string? powerManagementCapabilities { get; set; }
        public bool? powerManagementSupported { get; set; }
        public string? providerType { get; set; }
        public bool? settableBaudRate { get; set; }
        public bool? settableDataBits { get; set; }
        public bool? settableFlowControl { get; set; }
        public bool? settableParity { get; set; }
        public bool? settableParityCheck { get; set; }
        public bool? settableRLSD { get; set; }
        public bool? settableStopBits { get; set; }
        public string? status { get; set; }
        public int? statusInfo { get; set; }
        public bool? supports16BitMode { get; set; }
        public bool? supportsDTRDSR { get; set; }
        public bool? supportsElapsedTimeouts { get; set; }
        public bool? supportsIntTimeouts { get; set; }
        public bool? supportsParityCheck { get; set; }
        public bool? supportsRLSD { get; set; }
        public bool? supportsRTSCTS { get; set; }
        public bool? supportsSpecialCharacters { get; set; }
        public bool? supportsXOnXOff { get; set; }
        public bool? supportsXOnXOffSet { get; set; }
        public string? systemCreationClassName { get; set; }
        public string? systemName { get; set; }
        public string? timeOfLastReset { get; set; }
    }
}
