using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace WpfWindowProcedureHook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
		private WindowProcedureListener _listener;

        public MainWindow()
        {
            InitializeComponent();

			_listener = new WindowProcedureListener();
			_listener.UsbDeviceAttached += (o, e) => { Debug.WriteLine("USB device attached!"); };
			_listener.UsbDeviceDetached += (o, e) => { Debug.WriteLine("USB device detached!"); };
		}

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

			_listener.Initialize(new WindowInteropHelper(this).Handle);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

			_listener.Uninitialize();
        }
    }
}