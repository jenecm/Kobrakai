using System;
namespace Glados.Droid
{
	
		public static class searedProfile
		{
			private static string name = "Glados";
			private static string location = "not set";
			private static string id;
			private static string position = "not set";

			public static void setName(string newName)
			{
				name = newName;
			}

			public static string getName()
			{
				return name;
			}

			public static void setLocation(string newLocation)
			{
				location = newLocation;
			}

			public static string getLocation()
			{
				return location;
			}

			public static void setID(string newID)
			{
				id = newID;
			}

			public static string getID()
			{
				return id;
			}

			public static void setPosition(string newPosition)
			{
				position = newPosition;
			}

			public static string getPosition()
			{
				return position;
			}

		}
}
