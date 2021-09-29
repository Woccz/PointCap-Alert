using System.IO;
using Pipliz;
using Chatting;
using Assets.ColonyPointUpgrades;
using Assets.ColonyPointUpgrades.Implementations;

namespace PointCapAlert {

	public static class PointCapAlert
	{
		public const string NAMESPACE = "PointCapAlert";
		public const float CHECK_INTERVAL = 12.5f;
		public static string MOD_DIRECTORY;
		public static InterfaceManager interfaceClass = new InterfaceManager();
		private static UpgradeKey keyCapacity, keyEfficiency;
		private static ColonyPointCapacityUpgrade upgradeCapacity;
		private static ColonyPointMultiplierUpgrade upgradeEfficiency;

		public static void AssemblyLoaded(string path)
		{
			MOD_DIRECTORY = Path.GetDirectoryName(path);
			Log.Write("Loaded PointCapAlert");
		}

		public static void WorldStarted()
		{
			IUpgrade iUpgradeCapacity, iUpgradeEfficiency;
			ServerManager.UpgradeManager.TryGetKeyUpgrade("pipliz.colonypointcap", out keyCapacity, out iUpgradeCapacity);
			upgradeCapacity = (ColonyPointCapacityUpgrade) iUpgradeCapacity;

			ThreadManager.InvokeOnMainThread(delegate() {
				CheckColonies();
			}, CHECK_INTERVAL);
		}

		// check all colonies every 30 seconds
		public static void CheckColonies()
		{
			foreach (Colony colony in ServerManager.ColonyTracker.ColoniesByID.Values) {
				if (colony.Banners.Length == 0 || colony.Owners.Length == 0) {
					continue;
				}

				CheckCapacity(colony);
			}

			// queue self again
			ThreadManager.InvokeOnMainThread(delegate() {
				CheckColonies();
			}, CHECK_INTERVAL);
		}

		public static void CheckCapacity(Colony colony)
		{
			int lvlCapacity = colony.UpgradeState.GetUnlockedLevels(keyCapacity);
			if (lvlCapacity < upgradeCapacity.LevelCount) {
				long costCapacity = upgradeCapacity.GetUpgradeCost(lvlCapacity);
				if (colony.ColonyPoints >= costCapacity) {
					foreach (Players.Player owner in colony.Owners) {
						if (owner.ConnectionState == Players.EConnectionState.Connected) {
							Chat.Send("{colony.Name} has reached maximum point capacity of {colony.ColonyPoints}.");
						}
					}	
				}
				while (colony.ColonyPoints >= costCapacity) {
					Sleep((Int32)(CHECK_INTERVAL*1000))	// Spin while at max point capacity.
				}
			}
		}

	} // class

} // namespace

