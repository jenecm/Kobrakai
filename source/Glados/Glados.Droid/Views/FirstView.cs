using System;
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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Window.RequestFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.FirstView);

            var checkInDialog = new AlertDialog.Builder(this);

            var locationDialog = new AlertDialog.Builder(this);

            locationID.getListFromDDB();

            // set the id of the user to the saved one on the device or create a new id and save it to the device
            SaveId.Saveid();

            // search the AWS DDB database for the user information associated with the id saved on the device
            // and update any fields in the user class that need to be updated from the AWS DDB
            GetUserFromDdb(User.GetId());

            StorageHelper.StoreValue("Job", "University Student");
            StorageHelper.StoreValue("Bio", "Is a person. Hates this subject.");

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

            _items = new List<string>();
            _items.Add("Dan requested your location");
            _items.Add("Jan is at D101");
            _items.Add("Bob is not available");

            _rooms = new List<string>();
            _rooms.Add("F101");
            _rooms.Add("F102");
            _rooms.Add("F103");

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
                _items);

            _listView.Adapter = adapter;
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
                "Home", "Favourites", "Profile",
                "Settings", "Log"
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
                case "Favourites":
                    PopupMenu menu = new PopupMenu(this, FindViewById(Resource.Id.headerbar));
                    menu.Inflate(Resource.Layout.favourites);

                    List<string> favourites = new List<string> {"Person1", "Person2", "Person3", "Person4", "Person5"};

                    foreach (var f in favourites)
                    {
                        menu.Menu.Add(f);
                    }

                    menu.MenuItemClick += (s1, arg1) =>
                    {
                        //Console.WriteLine("{0} selected", arg1.Item.TitleFormatted);
                        profile = new Intent(this, typeof(Profile));
                        profile.PutExtra("User", arg1.Item.TitleFormatted);
                        StartActivity(profile);
                    };
                    menu.DismissEvent += (s2, arg2) => { };
                    menu.Show();
                    break;
                case "Profile":
                    profile = new Intent(this, typeof(Profile));
                    profile.PutExtra("User", "Self");
                    StartActivity(profile);
                    break;
                case "Settings":
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


            _toolbar = FindViewById<LinearLayout>(Resource.Id.toolbar);
            var toolText = (TextView)_toolbar.GetChildAt(1);
            toolText.Text = StorageHelper.GetValue("name") ?? User.GetName();

        }
    }
}
