
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

namespace Glados.Droid
{
	[Activity(Label = "Profile")]
	public class Profile : Activity
	{
        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private LinearLayout _toolbar;
        private Button _menuButton;

        private ListView _menuListView;
        private MenuListAdapterClass _objAdapterMenu;
        //ImageView menuIconImageView;
        private int _intDisplayWidth;
        private bool _isSingleTapFired = false;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Profile);
            // Create your application here

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

            TextView headertext = (TextView)headerbar.GetChildAt(1);

            headertext.Text = "Profile";

            Button backButton = (Button)headerbar.GetChildAt(0);
            backButton.Click += delegate
            {
                StartActivity(typeof(FirstView));
            };

            _toolbar = FindViewById<LinearLayout>(Resource.Id.toolbar);

            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(_toolbar.LayoutParameters);

            p.TopMargin = 0;

            _toolbar.LayoutParameters = p;

            var toolText = (TextView)_toolbar.GetChildAt(1);

            toolText.Text = StorageHelper.GetValue("Name");

            var jobTitle = FindViewById<TextView>(Resource.Id.JobTitleTitle);
            jobTitle.Text = StorageHelper.GetValue("Job");

            var bio = FindViewById<TextView>(Resource.Id.BioLabel);
            bio.Text = StorageHelper.GetValue("Bio");

            _menuButton = (Button)_toolbar.GetChildAt(0);
            _menuListView = FindViewById<ListView>(Resource.Id.menuListView);

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

        private void FnBindMenu()
        {
            string[] strMnuText =
            {
                "Favourites", "Profile",
                "Settings", "Log"
            };
            int[] strMnuUrl =
            {
                Resource.Drawable.icon, Resource.Drawable.icon,
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
            //_txtActionBarText.Text = strMenuText;
            //_txtPageName.Text = strMenuText;
            //selected action goes here
            switch (strMenuText)
            {
                case "Favourites":
                    break;
                case "Profile":
                    StartActivity(typeof(Profile));
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