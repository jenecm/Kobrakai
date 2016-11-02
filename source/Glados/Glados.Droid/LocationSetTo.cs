using System;
namespace Glados.Droid
{
	public static class LocationSetTo
	{
		private static string locationIs = "not set";
		private static string beacon = "not set";
		private static string qr = "not set";

		public static void setLocation(string newLocation)
		{
			locationIs = newLocation;
		}

		public static string getLocation()
		{
			return locationIs;
		}

		public static void setBeacon(string newBeacon)
		{
			beacon = newBeacon;
		}

		public static string getBeacon()
		{
			return beacon;
		}

		public static void setQR(string newQR)
		{
			qr = newQR;
		}

		public static string getQR()
		{
			return qr;
		}
	}
}
