using MvvmCross.Core.ViewModels;
using System.Collections.ObjectModel;
using Glados.Core.Models;


namespace Glados.Core.ViewModels
{
    public class FirstViewModel 
        : MvxViewModel
    {
        private string _hello = "Glados First view model cs";
        public string Hello
        { 
            get { return _hello; }
            set { SetProperty (ref _hello, value); }
        }

        public ObservableCollection<EddyStone> EddyStoneList
        {
            get { return eddyStoneList; }
            set { SetProperty(ref eddyStoneList, value); }
        }

    }
}
