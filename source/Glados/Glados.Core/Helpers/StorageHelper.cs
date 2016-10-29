using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using Android.App;
using Android.Content;

namespace Glados.Core.Helpers
{
    public static class StorageHelper
    {
        public static void StoreValue(string name, string value)
        {
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var contextPref = Application.Context.GetSharedPreferences("Glados", FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutString(name, value);
            contextEdit.Commit();
        }

        public static string GetValue(string name)
        {
            var contextPref = Application.Context.GetSharedPreferences("Glados", FileCreationMode.Private);
            var value = contextPref.GetString(name, null);

            return value;
        }
    }
}
