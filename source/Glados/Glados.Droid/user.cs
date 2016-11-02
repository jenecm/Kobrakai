using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Preferences;
using Glados.Core.Helpers;

namespace Glados.Droid
{
	
	public static class User
	{
		private static string _name = "Glados";
		private static string _location = "not set";
		private static string _id;
		private static string _position = "not set";

		public static void SetName(string newName)
		{
			_name = newName;
            StorageHelper.StoreValue("name", newName);
		}

		public static string GetName()
		{
			return _name;
		}

		public static void SetLocation(string newLocation)
		{
			_location = newLocation;
		}

		public static string GetLocation()
		{
			return _location;
		}

		public static void SetId(string newId)
		{
			_id = newId;
            StorageHelper.StoreValue("id", newId);
		}

		public static string GetId()
		{
		    if (string.IsNullOrWhiteSpace(StorageHelper.GetValue("id")))
		    {
		        return StorageHelper.GetValue("id");
		    }
			return _id;
		}

		public static void SetPosition(string newPosition)
		{
			_position = newPosition;
		}

		public static string GetPosition()
		{
			return _position;
		}


	}
}
