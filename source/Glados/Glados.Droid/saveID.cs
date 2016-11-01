using System;
using Android.App;
using Android.Content;

namespace Glados.Droid
{
	public static class SaveId
	{
		public static void Saveid()
		{

			var prefs = Application.Context.GetSharedPreferences("Glados", FileCreationMode.Private);
			var userId = prefs.GetString("id", null);

			if (string.IsNullOrWhiteSpace(userId))
			{
				string guid = Guid.NewGuid().ToString();

				var preference = Application.Context.GetSharedPreferences("Glados", FileCreationMode.Private);
				var prefEditor = preference.Edit();
				prefEditor.PutString("id", guid);
				prefEditor.Commit();
				User.SetId(guid);

			}
			else {
				User.SetId(userId);
			}
		}
	}
}
