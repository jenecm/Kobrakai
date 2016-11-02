using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Views;
using System.Threading.Tasks;

namespace Glados.Droid.Views
{
	[Activity(Label = "SearchProfile")]
	public class SearchProfile : MvxActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.SearchProfile);

			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

			Button backButton = FindViewById<Button>(Resource.Id.button1);
			backButton.Click += delegate
			{
				StartActivity(typeof(FirstView));
			};

			TextView user_position = FindViewById<TextView>(Resource.Id.userPosition);
			TextView user_location = FindViewById<TextView>(Resource.Id.userLocation);

			user_position.Text = searedProfile.getPosition();
			user_location.Text = searedProfile.getLocation();

			//Toolbar will now take on default actionbar characteristics
			SetActionBar(toolbar);

			//create a variable and assign it to the TextView called aToolBar that shows the users nam
			TextView toolBarText = FindViewById<TextView>(Resource.Id.aToolBar);
			//set the text of the TextView, called aToolBar, to show the name stored in the static class user
			toolBarText.Text = searedProfile.getName();
		}
	}
}
