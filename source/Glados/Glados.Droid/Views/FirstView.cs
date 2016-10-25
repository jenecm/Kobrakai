using Android.App;
using Android.OS;
using System.Collections.Generic;
using MvvmCross.Droid.Views;
using Android.Widget;
using Android.Content;
using Android.Views;

namespace Glados.Droid.Views
{
    [Activity(Label = "Home", Icon = "@mipmap/icon")]
    public class FirstView : MvxActivity
    {
		private List<string> items;
		private List<string> rooms;
		private ListView listView;
		private AutoCompleteTextView actv;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);

            var headerbar = FindViewById<LinearLayout>(Resource.Id.headerbar);

            TextView headertext = (TextView)headerbar.GetChildAt(1);

            headertext.Text = "Home";

            Button backButton = (Button) headerbar.GetChildAt(0);
            backButton.Visibility = ViewStates.Invisible;

            var toolbar = FindViewById<LinearLayout>(Resource.Id.toolbar);

            var toolText = (TextView)toolbar.GetChildAt(1);
            toolText.Text = "Alice";

            RelativeLayout.LayoutParams p = new RelativeLayout.LayoutParams(toolbar.LayoutParameters);
            
            p.TopMargin = 300;

            toolbar.LayoutParameters = p;



            //listView = FindViewById<ListView>(Resource.Id.notifications);
            //actv = FindViewById<AutoCompleteTextView>(Resource.Id.room);

            //items = new List<string>();
            //items.Add("Dan requested your location");
            //items.Add("Jan is at D101");
            //items.Add("Bob is not available");

            //rooms = new List<string>();
            //rooms.Add("F101");
            //rooms.Add("F102");
            //rooms.Add("F103");

            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);

            //listView.Adapter = adapter;

            //ArrayAdapter<string> adapterTwo = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, rooms);

            //actv.Adapter = adapterTwo;

            Button profileButton = FindViewById<Button>(Resource.Id.log);
            profileButton.Click += delegate
            {
                StartActivity(typeof(Log));
            };
            Button checkinButton = FindViewById<Button>(Resource.Id.checkin);
            checkinButton.Click += delegate
            {
                StartActivity(typeof(Profile));
            };
        }
    }
}
