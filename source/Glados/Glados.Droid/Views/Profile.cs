
using System;
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
using Glados.Droid.Views;
using MvvmCross.Droid.Views;

namespace Glados.Droid
{
	[Activity(Label = "Profile")]
	public class Profile : MvxActivity
	{
        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private LinearLayout _toolbar;
	    private TextView _toolText;
        private Button _menuButton;

        private ListView _menuListView;
        private MenuListAdapterClass _objAdapterMenu;
        private int _intDisplayWidth;
        private bool _isSingleTapFired = false;
        
        private AutoCompleteTextView actv;
        private int Location = 0;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Profile);

            string username = Intent.GetStringExtra("User") ?? "Error";

            FnInitialization();
            SetProfileText(username);
            TapEvent();
            FnBindMenu();
        }

        private void TapEvent()
        {
            _menuButton.Click += delegate (object sender, EventArgs e)
            {
                if (!_isSingleTapFired)
                {
                    FnToggleMenu(); //find definition in below steps
                    _isSingleTapFired = false;
                }
            };
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

            headertext.Text = "Profile";

            

            _toolbar = FindViewById<LinearLayout>(Resource.Id.toolbar);

            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(_toolbar.LayoutParameters);

            p.TopMargin = 0;

            _toolbar.LayoutParameters = p;

            _menuButton = (Button)_toolbar.GetChildAt(0);
            _menuListView = FindViewById<ListView>(Resource.Id.lvMenu);

            
            _toolText = (TextView)_toolbar.GetChildAt(1);

            var edit_name = FindViewById<EditText>(Resource.Id.editName);
            var edit_position = FindViewById<EditText>(Resource.Id.editPosition);
            var edit_location = FindViewById<EditText>(Resource.Id.editLocation);


            TextView tvPosition = FindViewById<TextView>(Resource.Id.setPosition);
            tvPosition.Text = User.GetPosition();

            TextView tv = FindViewById<TextView>(Resource.Id.setTo);
            tv.Text = User.GetLocation();

            _toolText.Text = User.GetName();

            var updatedDetialsDialog = new AlertDialog.Builder(this);
            var detialsNotUpdatedDialog = new AlertDialog.Builder(this);


            Button updateButton = FindViewById<Button>(Resource.Id.update);
            updateButton.Click += delegate
            {
                if (!String.IsNullOrEmpty(edit_name.Text))
                {
                    User.SetName(edit_name.Text);
                }
                if (!String.IsNullOrEmpty(edit_location.Text))
                {
                    User.SetLocation(edit_location.Text);
                }
                if (!String.IsNullOrEmpty(edit_position.Text))
                {
                    User.SetPosition(edit_position.Text);
                }

                if (!String.IsNullOrEmpty(edit_location.Text))
                {
                    String newID = User.GetLocation();
                    String newBeacon = "not set";
                    String newQR = "not set";
                    if (!locationsID.getLocationsList().Contains(newID))
                    {
                        locationsID.addNewLocationToDDB(newID, newBeacon, newQR);
                        locationsID.addLocationToList(newID);
                    }
                    // change all notifications in the AWS DDB for the device user to not active
                    foreach (notificationsDDB notification in notificationsList.getNotificationsList())
                    {

                        notificationsList.addNotificagtionToDDB(notification.id, notification.lost, notification.searcher, "false");
                        notificationsList.updateNotification(notification.id);

                    }
                }

                if (!((String.IsNullOrEmpty(edit_name.Text)) && (String.IsNullOrEmpty(edit_location.Text)) && (String.IsNullOrEmpty(edit_position.Text))))
                {
                    //update AWS DDB database to match the values stored in the static class user
                    UpdateItem();

                    updatedDetialsDialog.SetMessage("Your details have been updated");
                    updatedDetialsDialog.SetNegativeButton("Done", delegate { });
                    updatedDetialsDialog.Show();

                    //set the text of the TextView, called setTo, to show the location stored in the static class user
                    tv.Text = User.GetLocation();
                    //set the text of the TextView, called setPosition, to show the position stored in the static class user
                    tvPosition.Text = User.GetPosition();
                    //set the text of the TextView, called aToolBar, to show the name stored in the static class user
                    _toolText.Text = User.GetName();


                }
                else
                {
                    detialsNotUpdatedDialog.SetMessage("No details entered, update not performed");
                    detialsNotUpdatedDialog.SetNegativeButton("Done", delegate { });
                    detialsNotUpdatedDialog.Show();
                }



            };


            var locationDialog = new AlertDialog.Builder(this);
            
            actv = FindViewById<AutoCompleteTextView>(Resource.Id.room);
            ArrayAdapter<string> adapterTwo = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, locationsID.getLocationsList());

            actv.Adapter = adapterTwo;

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

                    edit_location.Text = User.GetLocation();

                    // change all notifications in the AWS DDB for this the device user to not active
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

            //changed sliding menu width to 3/4 of screen width 
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

	    private void SetProfileText(string username)
	    {
         //   var toolText = (TextView)_toolbar.GetChildAt(1);
         //   var jobTitle = FindViewById<TextView>(Resource.Id.txtJobTitle);
         //   var bio = FindViewById<TextView>(Resource.Id.txtBio);
	        //if (username == "Self")
	        //{
	        //    toolText.Text = StorageHelper.GetValue("Name");
         //       jobTitle.Text = StorageHelper.GetValue("Job");
         //       bio.Text = StorageHelper.GetValue("Bio");

         //       FindViewById(Resource.Id.btnRequestLocation).Visibility = ViewStates.Gone;
	        //}
         //   else if (username == "Error")
         //   {
         //       StartActivity(typeof(FirstView));
         //   }
	        //else
	        //{
         //       //Get Data from username
	        //    toolText.Text = username;
	        //    jobTitle.Text = username + " Job Title";
	        //    bio.Text = username + " Bio";

	        //    FindViewById(Resource.Id.lblLocation).Visibility = ViewStates.Gone;
	        //    FindViewById(Resource.Id.txtLocation).Visibility = ViewStates.Gone;
         //   }


            
        }

        private void FnBindMenu()
        {
            string[] strMnuText =
            {
                "Home", "Profile", "Log"
            };
            int[] strMnuUrl =
            {
                Resource.Drawable.icon, Resource.Drawable.icon, Resource.Drawable.icon,
                Resource.Drawable.icon, Resource.Drawable.icon
            };
            if (_objAdapterMenu != null)
            {
                _objAdapterMenu.actionMenuSelected -= FnMenuSelected;
                _objAdapterMenu = null;
            }
            _objAdapterMenu = new MenuListAdapterClass(this, strMnuText, strMnuUrl);
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
            // Pass the client to the DynamoDBConte
            DynamoDBContext context = new DynamoDBContext(client);

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

            _toolText.Text = User.GetName();
            _toolText.Touch += delegate
            {
                StartActivity(typeof(Profile));
            };

            TextView tv = FindViewById<TextView>(Resource.Id.setTo);
            tv.Text = User.GetLocation();

            users.setUsersList();

            locationsID.getListFromDDB();

            notificationsList.setNotificationsList();

            var locationRequestDialog = new AlertDialog.Builder(this);

            string provideLocation = "true";

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
            MenuListViewHolderClass objMenuListViewHolderClass;
            View view;
            view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.menu, parent, false);
                objMenuListViewHolderClass = new MenuListViewHolderClass();

                objMenuListViewHolderClass.txtMnuText = view.FindViewById<TextView>(Resource.Id.txtMnuText);
                objMenuListViewHolderClass.ivMenuImg = view.FindViewById<ImageView>(Resource.Id.ivMenuImg);

                objMenuListViewHolderClass.initialize(view);
                view.Tag = objMenuListViewHolderClass;
            }
            else
            {
                objMenuListViewHolderClass = (MenuListViewHolderClass)view.Tag;
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