
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
using Glados.Droid.Views;

namespace Glados.Droid
{
	[Activity(Label = "Profile")]
	public class Profile : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Profile);
            // Create your application here
            var headerbar = FindViewById<LinearLayout>(Resource.Id.headerbar);

            TextView headertext = (TextView)headerbar.GetChildAt(1);

            headertext.Text = "Profile";

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

