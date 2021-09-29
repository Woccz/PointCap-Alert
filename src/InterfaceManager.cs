using ModLoaderInterfaces;

namespace PointCapAlert {

	public class InterfaceManager:
		IOnAssemblyLoaded,
		IAfterWorldLoad
	{
		public void OnAssemblyLoaded(string path)
		{
			PointCapAlert.AssemblyLoaded(path);
		}

		public void AfterWorldLoad()
		{
			PointCapAlert.WorldStarted();
		}

	}

} // namespace

