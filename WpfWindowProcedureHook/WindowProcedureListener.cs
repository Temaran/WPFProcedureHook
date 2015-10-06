using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Common;

namespace WpfWindowProcedureHook
{
	public class WindowProcedureListener : DisposableBase, IUsbNotifier
	{
		public event EventHandler UsbDeviceAttached = delegate { };
		public event EventHandler UsbDeviceDetached = delegate { };

		private readonly Guid _guidDevinterfaceUsbDevice;

		private IntPtr _windowHandle;
		private IntPtr _notificationHandle;

		private const int DbtDevtypDeviceinterface = 5;

		public WindowProcedureListener()
		{
			_guidDevinterfaceUsbDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
		}

		public void Initialize(IntPtr hwnd)
		{
			if (_windowHandle != IntPtr.Zero)
			{
				Console.WriteLine("Usb notification already initialized! Reinitializing...");
				Uninitialize();
			}

			var source = HwndSource.FromHwnd(hwnd);
			if (source == null)
				return;

			source.AddHook(WindowProcedureHook);

			_windowHandle = hwnd;
			RegisterUsbDeviceNotification();
		}

		public void Uninitialize()
		{
			if (_windowHandle == IntPtr.Zero)
			{
				return;
			}

			if (_notificationHandle != IntPtr.Zero)
			{
				NativeMethods.UnregisterDeviceNotification(_notificationHandle);
			}

			_windowHandle = IntPtr.Zero;
			_notificationHandle = IntPtr.Zero;
		}

		protected override void DisposeNativeResources()
		{
			base.DisposeNativeResources();

			Uninitialize();
		}

		public IntPtr WindowProcedureHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			switch ((WindowsMessage)msg)
			{
				case WindowsMessage.DeviceChange:
					{
						switch ((DeviceChangeCode)wparam)
						{
							case DeviceChangeCode.DeviceArrival:
								UsbDeviceAttached(this, null);
								break;
							case DeviceChangeCode.DeviceRemoveComplete:
								UsbDeviceDetached(this, null);
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
