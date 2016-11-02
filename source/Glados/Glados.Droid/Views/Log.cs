﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Glados.Core.Helpers;
using Glados.Droid;
using MvvmCross.Droid.Views;

namespace Glados.Droid.Views
{
	[Activity(Label = "Log")]
	public class Log : MvxActivity
	{
        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private LinearLayout _toolbar;
        private Button _menuButton;

        private ListView _menuListView;
        private Droid.MenuListAdapterClass _objAdapterMenu;
        private int _intDisplayWidth;
        private bool _isSingleTapFired = false;

        private AutoCompleteTextView actv;
        private List<string> items;
        private ListView listView;
        private int Location = 0;

        protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Log);

            FnInitialization();
            TapEvent();
            FnBindMenu();

            
        }
        private void TapEvent()
        {
            var toolbar = FindViewById<LinearLayout>(Resource.Id.toolbar);


            //title bar menu icon
            _menuButton.Click += delegate (object sender, EventArgs e)
            {
                if (!_isSingleTapFired)
                {
                    FnToggleMenu(); //find definition in below steps
                    _isSingleTapFired = false;
                }
            };
            //bottom expandable description window
            //btnDescExpander.Click += delegate(object sender, EventArgs e)
            //{
            //    FnDescriptionWindowToggle();
            //};
        }

        private void FnInitialization()
        {
            //gesture initialization
            _gestureListener = new GestureListener();
            _gestureListener.LeftEvent += GestureLeft; //find definition in below steps
            _gestureListener.RightEvent += GestureRight;
            _gestureListener.SingleTapEvent += SingleTap;
            _gestureDetector = new GestureDetector(this, _gestureListener);

            var headerbar = FindViewById<LinearLayout>(Resource.Id.headerbar);

            TextView headertext = (TextView)headerbar.GetChildAt(0);

            headertext.Text = "Log";

            _toolbar = FindViewById<LinearLayout>(Resource.Id.toolbar);

            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(_toolbar.LayoutParameters);

            p.TopMargin = 0;

            _toolbar.LayoutParameters = p;

            var toolText = (TextView)_toolbar.GetChildAt(1);

            toolText.Text = StorageHelper.GetValue("Name");

            _menuButton = (Button)_toolbar.GetChildAt(0);
            _menuListView = FindViewById<ListView>(Resource.Id.menuListView);

            listView = FindViewById<ListView>(Resource.Id.notifications);
            actv = FindViewById<AutoCompleteTextView>(Resource.Id.room);

            var locationDialog = new AlertDialog.Builder(this);

            var updateLocationDialog = new AlertDialog.Builder(this);

            TextView tv = FindViewById<TextView>(Resource.Id.setTo);

            items = new List<string>();


            ArrayAdapter<string> ad = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);

            listView.Adapter = ad;

            ArrayAdapter<string> adapterTwo = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, locationsID.getLocationsList());

            actv.Adapter = adapterTwo;

            Button ignoreButton = FindViewById<Button>(Resource.Id.ignore);
            ignoreButton.Click += (object sender, EventArgs e) =>
            {
                // change all notifications in the AWS DDB for this the device user to not active
                foreach (notificationsDDB notification in notificationsList.getNotificationsList())
                {

                    notificationsList.addNotificagtionToDDB(notification.id, notification.lost, notification.searcher, "false");
                    notificationsList.updateNotification(notification.id);

                }

                items.Clear();

                //Show a toast where someMessage is a string ie, string someMessage = "this is a message";
                RunOnUiThread(() => Toast.MakeText(this, "notifications ignored, you will be notified the next time someone requests your location", ToastLength.Long).Show());
            };

            Button busyButton = FindViewById<Button>(Resource.Id.busy);
            busyButton.Click += (object sender, EventArgs e) =>
            {
                //use the static class user and set the location stored by this class 
                //to the string "Busy"
                User.SetLocation("Busy");

                //update AWS DDB database to match the values stored in the static class user
                UpdateItem();

                // change all notifications in the AWS DDB for the device user to not active
                foreach (notificationsDDB notification in notificationsList.getNotificationsList())
                {
                    notificationsList.addNotificagtionToDDB(notification.id, notification.lost, notification.searcher, "false");
                    notificationsList.updateNotification(notification.id);
                }

                items.Clear();

                //Show a toast where someMessage is a string ie, string someMessage = "this is a message";
                RunOnUiThread(() => Toast.MakeText(this, "Your location has been set to Busy", ToastLength.Long).Show());

                //set the text of the TextView, called setTo, to show the location stored in the static class user
                tv.Text = User.GetLocation();

            };

            Button sendButton = FindViewById<Button>(Resource.Id.send);
            sendButton.Click += delegate
            {
                updateLocationDialog.SetMessage("Your location is set to" + " " + User.GetLocation() + "." + " " + "Please select a new location.");
                updateLocationDialog.SetNegativeButton("Done", delegate { });
                updateLocationDialog.Show();
            };

            tv.Text = User.GetLocation();

            actv.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                if (!String.IsNullOrEmpty(actv.Text))
                {
                    //use the static class user and set the location stored by this class 
                    //to the element in the list named rooms at the position stored in the variable named Location
                    User.SetLocation(locationsID.getMatchingLocation(actv.Text));

                    //update AWS DDB database to match the values stored in the static class user
                    UpdateItem();

                    //use a dialog box to inform the user of the position in the array and the room setting, 
                    //this is for debugging and should be taken out or modified on ship
                    locationDialog.SetMessage("Your location is set to" + " " + User.GetLocation());
                    locationDialog.SetNegativeButton("Done", delegate { });
                    locationDialog.Show();

                    //set the text of the TextView, called setTo, to show the location stored in the static class user
                    tv.Text = User.GetLocation();

                    // change all notifications in the AWS DDB for the device user to not active
                    foreach (notificationsDDB notification in notificationsList.getNotificationsList())
                    {
                        notificationsList.addNotificagtionToDDB(notification.id, notification.lost, notification.searcher, "false");
                        notificationsList.updateNotification(notification.id);
                    }
                }
                else
                {
                    locationDialog.SetMessage("Your location is set to" + " " + User.GetLocation() + "." + " " + "The string entered is null or empty.");
                    locationDialog.SetNegativeButton("Done", delegate { });
                    locationDialog.Show();
                }
            };
            Display display = this.WindowManager.DefaultDisplay;
            
            var point = new Point();
            display.GetSize(point);
            _intDisplayWidth = point.X;
            _intDisplayWidth = _intDisplayWidth - (_intDisplayWidth / 3);
            using (var layoutParams = (LinearLayout.LayoutParams)_menuListView.LayoutParameters)
            {
                layoutParams.Width = _intDisplayWidth;
                layoutParams.Height = ViewGroup.LayoutParams.MatchParent;
                _menuListView.LayoutParameters = layoutParams;
            }
        }

        private void FnBindMenu()
        {
            string[] strMnuText =
            {
                "Home", "Profile", "Log"
            };
            int[] strMnuUrl =
            {
               Resource.Drawable.icon,  Resource.Drawable.icon, Resource.Drawable.icon,
                Resource.Drawable.icon, Resource.Drawable.icon
            };
            if (_objAdapterMenu != null)
            {
                _objAdapterMenu.actionMenuSelected -= FnMenuSelected;
                _objAdapterMenu = null;
            }
            _objAdapterMenu = new Droid.MenuListAdapterClass(this, strMnuText, strMnuUrl);
            _objAdapterMenu.actionMenuSelected += FnMenuSelected;
            _menuListView.Adapter = _objAdapterMenu;
        }

        private void FnMenuSelected(string strMenuText)
        {
            Intent profile = new Intent();
            switch (strMenuText)
            {
                case "Home":
                    StartActivity(typeof(FirstView));
                    break;
                case "Profile":
                    profile = new Intent(this, typeof(Profile));
                    profile.PutExtra("User", "Self");
                    StartActivity(profile);
                    break;
                case "Log":
                    StartActivity(typeof(Log));
                    break;
            }
        }

        private void GestureLeft()
        {
            if (!_menuListView.IsShown)
                FnToggleMenu();
            _isSingleTapFired = false;
        }

        private void GestureRight()
        {
            if (_menuListView.IsShown)
                FnToggleMenu();
            _isSingleTapFired = false;
        }

        private void SingleTap()
        {
            if (_menuListView.IsShown)
            {
                FnToggleMenu();
                _isSingleTapFired = true;
            }
            else
            {
                _isSingleTapFired = false;
            }
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            _gestureDetector.OnTouchEvent(ev);
            return base.DispatchTouchEvent(ev);
        }

        private void FnToggleMenu()
        {
            Console.WriteLine(_menuListView.IsShown);
            if (_menuListView.IsShown)
            {
                _menuListView.Animation = new TranslateAnimation(0f, -_menuListView.MeasuredWidth, 0f, 0f);
                _menuListView.Animation.Duration = 300;
                _menuListView.Visibility = ViewStates.Gone;
            }
            else
            {
                _menuListView.Visibility = ViewStates.Visible;
                _menuListView.RequestFocus();
                _menuListView.Animation = new TranslateAnimation(-_menuListView.MeasuredWidth, 0f, 0f, 0f);
                //starting edge of layout 
                _menuListView.Animation.Duration = 300;
            }
        }
        public async Task UpdateItem()
        {
            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
                               "us-west-2:d17455cb-c093-403a-a797-d8b01906f7b2", // Identity Pool 

                               RegionEndpoint.USWest2 // Regio

                           );
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USWest2);

            var theUser = new Document();

            theUser["id"] = User.GetId();
            theUser["location"] = User.GetLocation();
            theUser["name"] = User.GetName();
            theUser["position"] = User.GetPosition();

            Table users = Table.LoadTable(client, "kobrakaiUsers");

            Document updatedUser = await users.UpdateItemAsync(theUser);
        }

        protected override void OnResume()
        {
            base.OnResume();

            //create a variable and assign it to the TextView called aToolBar that shows the users nam
            TextView toolBarText = FindViewById<TextView>(Resource.Id.aToolBar);
            //set the text of the TextView, called aToolBar, to show the name stored in the static class user
            toolBarText.Text = User.GetName();
            toolBarText.Touch += delegate
            {
                StartActivity(typeof(Profile));
            };

            users.setUsersList();
            locationsID.getListFromDDB();

            notificationsList.setNotificationsList();

            var locationRequestDialog = new AlertDialog.Builder(this);

            string provideLocation = "true";

            items = new List<string>();
            foreach (notificationsDDB tha_notification in notificationsList.getNotificationsList())
            {
                if (tha_notification.active.Equals("true"))
                {
                    items.Add((users.getUser(tha_notification.searcher)).name + " " + "requested your location");
                }
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);

            listView.Adapter = adapter;

            foreach (notificationsDDB the_notification in notificationsList.getNotificationsList())
            {
                if (the_notification.active.Equals(provideLocation))
                {
                    locationRequestDialog.SetMessage("Location request received from " + users.getUser(the_notification.searcher).name);
                    locationRequestDialog.SetNegativeButton("Done", delegate { });
                    locationRequestDialog.Show();
                }
            }
        }
    }

    public class MenuListAdapterClass : BaseAdapter<string>
    {
        private Activity _context;
        private string[] _mnuText;
        private int[] _mnuUrl;
        //action event to pass selected menu item to main activity
        internal event Action<string> actionMenuSelected;
        public MenuListAdapterClass(Activity context, string[] strMnu, int[] intImage)
        {
            _context = context;
            _mnuText = strMnu;
            _mnuUrl = intImage;
        }
        public override string this[int position]
        {
            get { return this._mnuText[position]; }
        }

        public override int Count
        {
            get { return this._mnuText.Length; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Droid.MenuListViewHolderClass objMenuListViewHolderClass;
            View view;
            view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.menu, parent, false);
                objMenuListViewHolderClass = new Droid.MenuListViewHolderClass();

                objMenuListViewHolderClass.txtMnuText = view.FindViewById<TextView>(Resource.Id.txtMnuText);
                objMenuListViewHolderClass.ivMenuImg = view.FindViewById<ImageView>(Resource.Id.ivMenuImg);

                objMenuListViewHolderClass.initialize(view);
                view.Tag = objMenuListViewHolderClass;
            }
            else
            {
                objMenuListViewHolderClass = (Droid.MenuListViewHolderClass)view.Tag;
            }
            objMenuListViewHolderClass.viewClicked = () =>
            {
                if (actionMenuSelected != null)
                {
                    actionMenuSelected(_mnuText[position]);
                }
            };
            objMenuListViewHolderClass.txtMnuText.Text = _mnuText[position];
            objMenuListViewHolderClass.ivMenuImg.SetImageResource(_mnuUrl[position]);
            return view;
        }
    }
    //Viewholder class
    internal class MenuListViewHolderClass : Java.Lang.Object
    {
        internal Action viewClicked { get; set; }
        internal TextView txtMnuText;
        internal ImageView ivMenuImg;
        public void initialize(View view)
        {
            view.Click += delegate
            {
                viewClicked();
            };
        }

    }

}