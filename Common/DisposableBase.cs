using System;

namespace Common
{
	public abstract class DisposableBase : IDisposable
	{
		protected bool Disposed { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				if (disposing)
				{
					DisposeManagedResources();
				}

				DisposeNativeResources();
			}

			Disposed = true;
		}

		protected virtual void DisposeManagedResources() { }
		protected virtual void DisposeNativeResources() { }
		
		~DisposableBase()
		{
			Dispose(false);
		}
	}
}
