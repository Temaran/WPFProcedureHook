namespace WpfWindowProcedureHook
{
    /// <summary>
    /// DBT Event codes.
    /// </summary>
    public enum DeviceChangeCode
    {
        ConfigChangeCanceled = 0x0019,      //A request to change the current configuration (dock or undock) has been canceled.
        ConfigChanged = 0x0018,             //The current configuration has changed, due to a dock or undock.
        CustomEvent = 0x8006,               //A custom event has occurred.
        DeviceArrival = 0x8000,             //A device or piece of media has been inserted and is now available.
        DeviceQueryRemove = 0x8001,         //Permission is requested to remove a device or piece of media. Any application can deny this request and cancel the removal.
        DeviceQueryRemoveFailed = 0x8002,   //A request to remove a device or piece of media has been canceled.
        DeviceRemoveComplete = 0x8004,      //A device or piece of media has been removed.
        DeviceRemovePending = 0x8003,       //A device or piece of media is about to be removed. Cannot be denied.
        DeviceTypeSpecific = 0x8005,        //A device-specific event has occurred.
        DevnodesChanged = 0x0007,           //A device has been added to or removed from the system.
        QueryChangeConfig = 0x0017,         //Permission is requested to change the current configuration (dock or undock).
        DeviceChange = 0x0219               //Generic change
    }
}