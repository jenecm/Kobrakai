using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Views;

namespace Glados.Droid.Views
{
	[Activity(Label = "Log")]
	public class Log : MvxActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Log);

			Button backButton = FindViewById<Button>(Resource.Id.button1);
			backButton.Click += delegate
			{
				StartActivity(typeof(FirstView));
			};
		}
	}
}

