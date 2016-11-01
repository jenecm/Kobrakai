using Android.App;
using Android.OS;
using System.Collections.Generic;
using MvvmCross.Droid.Views;
using Android.Widget;
using EstimoteSdk;
using Android.Content;
using Glados.Core.Models;
using System.Linq;
using System;
using System.Diagnostics;
using System.Threading;

namespace Glados.Droid.Views
{
    class BeaconView : MvxActivity, BeaconManager.IServiceReadyCallback
    {

        private BeaconManager _beaconManager;
        private Glados.Core.ViewModels.FirstViewModel vm;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);
            vm = this.ViewModel as Glados.Core.ViewModels.FirstViewModel;
            _beaconManager = new BeaconManager(this);
            _beaconManager.Eddystone += BeaconManager_Eddystone;
            _beaconManager.Connect(this);
        }

        private void BeaconManager_Eddystone(object sender, BeaconManager.EddystoneEventArgs e)
        {
            vm.EddyStoneList.Clear();
            List<EddyStone> eddys = new List<EddyStone>(e.Eddystones.Count);
            var sortedEddys = eddys.OrderBy(ed => ed.Rssi);
            foreach (var stone in sortedEddys)
            {
                System.Diagnostics.Debug.WriteLine("BeaconFound");
                //Thread.Sleep(10000);
                
                vm.EddyStoneList.Add(new Core.Models.EddyStone
                {
                    CalibratedTxPower = stone.CalibratedTxPower,
                    Instance = stone.Instance,
                    MacAddress = stone.MacAddress.ToString(),
                    Namespace = stone.Namespace,
                    Rssi = stone.Rssi,
                    TelemetryLastSeenMillis = System.Convert.ToInt16(stone.TelemetryLastSeenMillis),
                    Url = stone.Url
                });
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            _beaconManager.Connect(this);
        }

        

        public void OnServiceReady()
        {
            // This method is called when BeaconManager is up and running.
           // _beaconManager.StartMonitoring(_region);
        }

        protected override void OnDestroy()
        {
            // Make sure we disconnect from the Estimote.
            _beaconManager.Disconnect();
            base.OnDestroy();
        }



    }
}