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
using Android.Graphics;
using Android.Views.Animations;

namespace Glados.Droid.Views
{
	[Activity(Label = "SearchProfile")]
	public class SearchProfile : MvxActivity
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

        protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.SearchProfile);

            FnInitialization();
            TapEvent();
            FnBindMenu();

            TextView user_position = FindViewById<TextView>(Resource.Id.userPosition);
			TextView user_location = FindViewById<TextView>(Resource.Id.userLocation);

			user_position.Text = searedProfile.getPosition();
			user_location.Text = searedProfile.getLocation();

			//create a variable and assign it to the TextView called aToolBar that shows the users nam
			TextView toolBarText = FindViewById<TextView>(Resource.Id.aToolBar);
			//set the text of the TextView, called aToolBar, to show the name stored in the static class user
			toolBarText.Text = searedProfile.getName();
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

            //LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(_toolbar.LayoutParameters);

            //p.TopMargin = 0;

            //_toolbar.LayoutParameters = p;

            _menuButton = (Button)_toolbar.GetChildAt(0);
            _menuListView = FindViewById<ListView>(Resource.Id.lvMenu);
            
            _toolText = (TextView)_toolbar.GetChildAt(1);
                      
            //changed sliding menu width to 3/4 of screen width 
            Display display = this.WindowManager.DefaultDisplay;
            var point = new Point();
            display.GetSize(point);
            _intDisplayWidth = point.X;
            _intDisplayWidth = _intDisplayWidth - (_intDisplayWidth / 3);
            using (var layoutParams = (RelativeLayout.LayoutParams)_menuListView.LayoutParameters)
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
    }


}
