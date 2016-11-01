using System;
using System.Collections.Generic;
using System.Linq;

namespace Glados.Droid
{
		public static class locations
		{
			static List<locationsDDB> locationsList = new List<locationsDDB>();

			public static List<locationsDDB> getLocationsList()
			{
				return locationsList;
			}

			public static void addLocationToList(locationsDDB location)
			{
				bool containsItem = locationsList.Any(locationsDDB => locationsDDB.id == location.id);
				if (containsItem)
				{
					return;
				}
				locationsList.Add(location);
			}

			public static void setLocationsList(List<locationsDDB> locationsList)
			{
				foreach (locationsDDB a_location in locationsList)
				{
					bool containsItem = locationsList.Any(locationsDDB => locationsDDB.id == a_location.id);
					if (containsItem)
					{
						return;
					}
					locationsList.Add(a_location);
				}
			}

			


		}
}


