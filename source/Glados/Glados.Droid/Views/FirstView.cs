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

            StorageHelper.StoreValue("Name", "Ashleigh");
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
                Intent profile = new Intent(this, typeof(Profile));
                profile.PutExtra("User", "Self");
                StartActivity(profile);
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
            toolText.Text = StorageHelper.GetValue("Name");
            _menuButton = (Button) _toolbar.GetChildAt(0);
            _menuListView = FindViewById<ListView>(Resource.Id.menuListView);

            _listView = FindViewById<ListView>(Resource.Id.notifications);
            _actv = FindViewById<AutoCompleteTextView>(Resource.Id.room);

            _items = new List<string>();
            _items.Add("Dan requested your location");
            _items.Add("Jan is at D101");
            _items.Add("Bob is not available");

            _rooms = new List<string>();
            _rooms.Add("F101");
            _rooms.Add("F102");
            _rooms.Add("F103");

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _items);

            _listView.Adapter = adapter;

            ArrayAdapter<string> adapterTwo = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, _rooms);

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
                    
                    menu.MenuItemClick += (s1, arg1) => {
                        //Console.WriteLine("{0} selected", arg1.Item.TitleFormatted);
                        profile = new Intent(this, typeof(Profile));
                        profile.PutExtra("User", arg1.Item.TitleFormatted);
                        StartActivity(profile);
                    };
                    menu.DismissEvent += (s2, arg2) => {};
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
    }

    //public class MenuListAdapterClass : BaseAdapter<string>
    //{
    //    private Activity _context;
    //    private string[] _mnuText;
    //    private int[] _mnuUrl;
    //    //action event to pass selected menu item to main activity
    //    internal event Action<string> actionMenuSelected;
    //    public MenuListAdapterClass(Activity context, string[] strMnu, int[] intImage)
    //    {
    //        _context = context;
    //        _mnuText = strMnu;
    //        _mnuUrl = intImage;
    //    }
    //    public override string this[int position]
    //    {
    //        get { return this._mnuText[position]; }
    //    }

    //    public override int Count
    //    {
    //        get { return this._mnuText.Length; }
    //    }

    //    public override long GetItemId(int position)
    //    {
    //        return position;
    //    }
    //    public override View GetView(int position, View convertView, ViewGroup parent)
    //    {
    //        MenuListViewHolderClass objMenuListViewHolderClass;
    //        View view;
    //        view = convertView;
    //        if (view == null)
    //        {
    //            view = _context.LayoutInflater.Inflate(Resource.Layout.menu, parent, false);
    //            objMenuListViewHolderClass = new MenuListViewHolderClass();

    //            objMenuListViewHolderClass.txtMnuText = view.FindViewById<TextView>(Resource.Id.txtMnuText);
    //            objMenuListViewHolderClass.ivMenuImg = view.FindViewById<ImageView>(Resource.Id.ivMenuImg);

    //            objMenuListViewHolderClass.initialize(view);
    //            view.Tag = objMenuListViewHolderClass;
    //        }
    //        else
    //        {
    //            objMenuListViewHolderClass = (MenuListViewHolderClass)view.Tag;
    //        }
    //        objMenuListViewHolderClass.viewClicked = () =>
    //        {
    //            if (actionMenuSelected != null)
    //            {
    //                actionMenuSelected(_mnuText[position]);
    //            }
    //        };
    //        objMenuListViewHolderClass.txtMnuText.Text = _mnuText[position];
    //        objMenuListViewHolderClass.ivMenuImg.SetImageResource(_mnuUrl[position]);
    //        return view;
    //    }
    //}
    ////Viewholder class
    //internal class MenuListViewHolderClass : Java.Lang.Object
    //{
    //    internal Action viewClicked { get; set; }
    //    internal TextView txtMnuText;
    //    internal ImageView ivMenuImg;
    //    public void initialize(View view)
    //    {
    //        view.Click += delegate
    //        {
    //            viewClicked();
    //        };
    //    }

    //}
}
