using System;

namespace WpfWindowProcedureHook
{
    public interface IUsbNotifier
    {
        event EventHandler UsbDeviceAttached;
        event EventHandler UsbDeviceDetached;
    }
}
