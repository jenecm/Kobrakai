using System;
using System.Linq;
using Android.App;
using Android.OS;
using System.Collections.Generic;
using MvvmCross.Droid.Views;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using Glados.Core.Helpers;
using Java.Security;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Views;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MvvmCross.Binding.ExtensionMethods;

namespace Glados.Droid.Views
{
    [Activity(Label = "Home", Icon = "@mipmap/icon")]
    public class FirstView : MvxActivity
    {
        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private LinearLayout _toolbar;
        private Button _menuButton;

        private ListView _menuListView;
        private MenuListAdapterClass _objAdapterMenu;
        private int _intDisplayWidth;
        private bool _isSingleTapFired = false;

        private ListView _listView;
        private AutoCompleteTextView _actv;
        private List<string> _items;
        private List<string> _rooms;

        private EditText searchView;
        private List<usersDDB> searchedItems;
        private string searched;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Window.RequestFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.FirstView);

            var checkInDialog = new AlertDialog.Builder(this);

            var locationDialog = new AlertDialog.Builder(this);

            locationsID.getListFromDDB();

            // set the id of the user to the saved one on the device or create a new id and save it to the device
            SaveId.Saveid();

            // search the AWS DDB database for the user information associated with the id saved on the device
            // and update any fields in the user class that need to be updated from the AWS DDB
            GetUserFromDdb(User.GetId());

            FnInitialization();
            TapEvent();
            FnBindMenu();

            Button profileButton = FindViewById<Button>(Resource.Id.log);
            profileButton.Click += delegate
            {
                StartActivity(typeof(Log));
            };
            Button checkinButton = FindViewById<Button>(Resource.Id.checkin);
            checkinButton.Click += delegate
            {
                checkInDialog.SetMessage("Scan");
                checkInDialog.SetNegativeButton("Done", delegate { });
                checkInDialog.Show();
            };
        }

        private void TapEvent()
        {
            //title bar menu icon
            _menuButton.Click += delegate(object sender, EventArgs e)
            {
                if (!_isSingleTapFired)
                {
                    FnToggleMenu();
                    _isSingleTapFired = false;
                }
            };
        }

        private void FnInitialization()
        {
            _gestureListener = new GestureListener();
            _gestureListener.LeftEvent += GestureLeft; //find definition in below steps
            _gestureListener.RightEvent += GestureRight;
            _gestureListener.SingleTapEvent += SingleTap;
            _gestureDetector = new GestureDetector(this, _gestureListener);

            var headerbar = FindViewById<LinearLayout>(Resource.Id.headerbar);
            TextView headertext = (TextView) headerbar.GetChildAt(0);
            headertext.Text = "Home";
            _toolbar = FindViewById<LinearLayout>(Resource.Id.toolbar);
            var toolText = (TextView) _toolbar.GetChildAt(1);
            toolText.Text = StorageHelper.GetValue("name") ?? User.GetName();
            _menuButton = (Button) _toolbar.GetChildAt(0);
            _menuListView = FindViewById<ListView>(Resource.Id.menuListView);

            _listView = FindViewById<ListView>(Resource.Id.notifications);


            Button searchButton = FindViewById<Button>(Resource.Id.searchButton);
            searchView = FindViewById<EditText>(Resource.Id.editSearch);
            searchButton.Click += (object sender, EventArgs e) =>
            {
                searched = searchView.Text;
                List<string> user_s = new List<string>();

                if (!string.IsNullOrEmpty(searched))
                {
                    var aList = users.getUsersByName(searched);

                    foreach (usersDDB aUser in aList)
                    {
                        user_s.Add(aUser.name);
                    }

                    ArrayAdapter<string> adapterThree = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, user_s);
                    _listView.Adapter = adapterThree;

                    // IMPORTANT change to try and avoid out of range exception
                    if (aList.Any())
                    {
                        usersDDB searched_user = (Glados.Droid.usersDDB)aList.ElementAt(0);
                        searedProfile.setID(searched_user.id);
                        searedProfile.setName(searched_user.name);
                        searedProfile.setPosition(searched_user.position);
                        searedProfile.setLocation(searched_user.location);
                        StartActivity(typeof(SearchProfile));
                    }
                    else
                    {
                        //Show a toast where someMessage is a string ie, string someMessage = "this is a message";
                        RunOnUiThread(() => Toast.MakeText(this, "The name you entered may not exist. Please try again.", ToastLength.Long).Show());
                    }

                }
            };

            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
            //    _items);

            //_listView.Adapter = adapter;
            _actv = FindViewById<AutoCompleteTextView>(Resource.Id.room);

            ArrayAdapter<string> adapterTwo = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleDropDownItem1Line, _rooms);

            _actv.Adapter = adapterTwo;

            Display display = this.WindowManager.DefaultDisplay;
            var point = new Point();
            display.GetSize(point);
            _intDisplayWidth = point.X;
            _intDisplayWidth = _intDisplayWidth - (_intDisplayWidth/3);
            using (var layoutParams = (RelativeLayout.LayoutParams) _menuListView.LayoutParameters)
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

        public async Task GetUserFromDdb(string id)
        {
            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
                "us-west-2:d17455cb-c093-403a-a797-d8b01906f7b2", // Identity Pool 

                RegionEndpoint.USWest2 // Regio

            );
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USWest2);
            // Pass the client to the DynamoDBCont
            DynamoDBContext context = new DynamoDBContext(client);

            Table users = Table.LoadTable(client, "kobrakaiUsers");

            var theUser = await users.GetItemAsync(id);

            if (
                !((string.IsNullOrEmpty(theUser["name"])) && (string.IsNullOrEmpty(theUser["location"])) &&
                  (string.IsNullOrEmpty(theUser["position"]))))
            {
                User.SetName(theUser["name"]);
                User.SetLocation(theUser["location"]);
                User.SetPosition(theUser["position"]);
            }

        }

        protected override void OnResume()
        {
            base.OnResume();

            //create a variable and assign it to the TextView called aToolBar that shows the users nam
            TextView toolBarText = FindViewById<TextView>(Resource.Id.aToolBar);
            //set the text of the TextView, called aToolBar, to show the name stored in the static class user
            toolBarText.Text = User.GetName();

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
}
