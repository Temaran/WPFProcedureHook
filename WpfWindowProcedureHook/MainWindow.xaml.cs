using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace WpfWindowProcedureHook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IUsbNotifier
    {
        public event EventHandler UsbDeviceAttached = delegate { };
        public event EventHandler UsbDeviceDetached = delegate { };

        private IntPtr _windowHandle;
        private IntPtr _notificationHandle;
        private readonly Guid _guidDevinterfaceUsbDevice;
        
        private const int DbtDevtypDeviceinterface = 5;

        public MainWindow()
        {
            InitializeComponent();

            _guidDevinterfaceUsbDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source == null) 
                return;

            _windowHandle = source.Handle;
            source.AddHook(WndProc);
            RegisterUsbDeviceNotification();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_notificationHandle != IntPtr.Zero)
            {
                NativeMethods.UnregisterDeviceNotification(_notificationHandle);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch ((WindowsMessage)msg)
            {
                case WindowsMessage.DeviceChange:
                {
                    switch ((Dbt)wparam)
                    {
                        case Dbt.DeviceArrival:
                            UsbDeviceAttached(this, null);
                            Debug.WriteLine("Attach");
                            break;
                        case Dbt.DeviceRemoveComplete:
                            UsbDeviceDetached(this, null);
                            Debug.WriteLine("Detach");
                            break;
                    }

                    var htLocation = NativeMethods.DefWindowProc(hwnd, msg, wparam, lparam).ToInt32();
                    return new IntPtr(htLocation);
                }
            }

            return IntPtr.Zero;
        }

        private void RegisterUsbDeviceNotification()
        {
            var dbi = new NativeMethods.DevBroadcastDeviceinterface
            {
                DeviceType = DbtDevtypDeviceinterface,
                Reserved = 0,
                ClassGuid = _guidDevinterfaceUsbDevice,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);
            var buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            _notificationHandle = NativeMethods.RegisterDeviceNotification(_windowHandle, buffer, 0);
        }
    }
}