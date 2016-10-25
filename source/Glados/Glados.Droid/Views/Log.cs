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

            var headerbar = FindViewById<LinearLayout>(Resource.Id.headerbar);

            TextView headertext = (TextView)headerbar.GetChildAt(1);

            headertext.Text = "Log";

            Button backButton = (Button)headerbar.GetChildAt(0);
			backButton.Click += delegate
			{
				StartActivity(typeof(FirstView));
			};

            var toolbar = FindViewById<LinearLayout>(Resource.Id.toolbar);

            

            var toolText = (TextView)toolbar.GetChildAt(1);
            toolText.Text = "Alice";
        }
	}
}

