using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V4.View;
using Com.Ittianyu.Bottomnavigationviewex;
using System;
using Wagheety_Driver.Adapter;
using Wagheety_Driver.Fragments;
using Android.Graphics;
using Android;
using Android.Support.V4.App;
using Android.Views;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using Wagheety_Driver.EventListener;
using Wagheety_Driver.Data_Model;
using System.Collections.Generic;
using Android.Media;
using IO.Github.Krtkush.Lineartimer;
using Android.Content;
using System.Threading;

namespace Wagheety_Driver
{
    [Activity(Label = "@string/app_name", Theme = "@style/DriverTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        // Rider Data
        string RiderName, RiderPhone, DestinationAddress, pickup_address, Paymentmethod;
        double PickupLat, PickupLng, DestinationLat, DestinationLng;
        MediaPlayer Player;

        ViewPager viewPager;
        BottomNavigationViewEx bnve;

        //Listnener
        RequestListener requestlistener = new RequestListener();

        List<Receiving_NewTripDetails> receiving_NewTripDetails;


        //images
        ImageView CallRider;
        ImageView RiderLocation;
        Android.Support.V7.Widget.Toolbar appmainToolbarRiderinfo;
        Android.Support.V7.Widget.Toolbar appmainToolbar;

        //Fragments;
        HomeFragment HomeFragment = new HomeFragment();
        RatingsFragment RatingsFragment = new RatingsFragment();
        AccountFragment AccountFragment = new AccountFragment();
        EarningsFragment EarningsFragment = new EarningsFragment();
        // Buttons
        Button Online;
        Button Offline;
        Button GoToRider;
        Button StartTrip;
        // Text View For Delog
        TextView RiderPickupAddresstxt;
        TextView RiderDestinationtxt;
        TextView RiderNametxt;
        TextView ridernameInHidenDelog;
        TextView Paymentmethodtxt;
        TextView cancel_Trip;

        // Alert Dialog
        public Android.Support.V7.App.AlertDialog.Builder alert;
        public Android.Support.V7.App.AlertDialog alertDialog;

        //LinearTimer
        LinearTimer linearTimer;
        LinearTimerView linearTimerView;

        //PermissionRequest
        const int requestId = 0;
        readonly string[] permissionGroup =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource

            //Call Another XML VIEW
            //View view = View.Inflate(this, Resource.Layout.progress, null);
            //loading_msg = (TextView)view.FindViewById(Resource.Id.loading_msg);

            SetContentView(Resource.Layout.activity_main);
            connectViews();
            CheckSpecialPermission();
        }
        
        void connectViews()
        {
            bnve = (BottomNavigationViewEx)FindViewById(Resource.Id.bnve);
            bnve.EnableItemShiftingMode(false);
            bnve.EnableShiftingMode(false);

            bnve.NavigationItemSelected += Bnve_NavigationItemSelected;

            BnveToAccentColor(0);

            viewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
            viewPager.OffscreenPageLimit = 3;
            viewPager.BeginFakeDrag();

            SetupViewpager();

            // App Toolbar appmainToolbar
            appmainToolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.appmainToolbar);
            appmainToolbarRiderinfo = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.appmainToolbarRiderinfo);

            // buttons
            Online = (Button)FindViewById(Resource.Id.Onlinebtn);
            Offline = (Button)FindViewById(Resource.Id.Offlinebtn);

            Online.Click += Online_Click;
            Offline.Click += Offline_Click;

            //Text View
            ridernameInHidenDelog = (TextView)FindViewById(Resource.Id.ridernameInHidenDelog);
            cancel_Trip = (TextView)FindViewById(Resource.Id.cancel_Trip);
            cancel_Trip.Click += Cancel_Trip_Click;

            //Imgaes
            CallRider = (ImageView)FindViewById(Resource.Id.CallRider);
            CallRider.Click += CallRider_Click;

            RiderLocation = (ImageView)FindViewById(Resource.Id.RiderLocation);
            RiderLocation.Click += RiderLocation_Click;

            StartTrip = (Button)FindViewById(Resource.Id.StartTrip);

            GoToRider = (Button)FindViewById(Resource.Id.GoToRider);
            GoToRider.Click += GoToRider_Click;
        }

        private void GoToRider_Click(object sender, EventArgs e)
        {
            GoToRider.Visibility = ViewStates.Gone;
            StartTrip.Visibility = ViewStates.Visible;
        }
    
        private void RiderLocation_Click(object sender, EventArgs e)
        {
            String uri = "http://maps.google.com/maps?saddr=" + 29.990004 + "," + 31.150507 + "&daddr=" + PickupLat + "," + PickupLng;
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(uri));
            StartActivity(intent);
        }

        private void CallRider_Click(object sender, EventArgs e)
        {
            var url = Android.Net.Uri.Parse("tel:"+ RiderPhone);
            var intent = new Intent(Intent.ActionDial, url);
            StartActivity(intent);
        }

        private void Online_Click(object sender, EventArgs e)
        {
            Online.Visibility = ViewStates.Gone;
            Offline.Visibility = ViewStates.Visible;

            requestlistener.GetRequstes();
            if (requestlistener != null)
            {
                requestlistener.DataRetrived += Requestlistener_DataRetrived;
            }
        }

        private void Offline_Click(object sender, EventArgs e)
        {
            Online.Visibility = ViewStates.Visible;
            Offline.Visibility = ViewStates.Gone;

            //HomeFragment homeFragment = new HomeFragment();
            //var LatLng = homeFragment.GetMyLang();
            //Toast.MakeText(this, LatLng.Latitude + " " + LatLng.Longitude, ToastLength.Short).Show();
            Toast.MakeText(this,"You Are OffLine Now", ToastLength.Long).Show();

        }

        // get Rider Requstes Data From DataBase
        private void Requestlistener_DataRetrived(object sender, RequestListener.DataEventArgs e)
        {
            receiving_NewTripDetails = e.Receiving_NewTrip;
            foreach (var item in receiving_NewTripDetails)
            {
                RiderName = item.rider_name;
                RiderPhone = item.rider_phone;

                pickup_address = item.PickupAddress;
                DestinationAddress = item.DestinationAddress;
                Paymentmethod = item.Paymentmethod;

                // Pickup Location Lang
                PickupLat = item.PickupLat;
                PickupLng = item.PickupLng;

                // Destenation  Lang
                DestinationLat = item.DestinationLat;
                DestinationLng = item.DestinationLng;
            }

            ShowNewTripDialog(RiderName, pickup_address, DestinationAddress, Paymentmethod);
        }

        // Alert Dialog Section
        void ShowNewTripDialog(string RiderName, string PickupAddress , string DestinationAddress, string Paymentmethod)
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.NewTripRequest);
            alert.SetCancelable(false);
            alertDialog = alert.Show();

            // linearTimer
            linearTimerView = (LinearTimerView)alertDialog.FindViewById(Resource.Id.linearTimer);
            linearTimerView.Click += LinearTimerView_Click;

            linearTimer = new LinearTimer(linearTimerView);
            linearTimer.StartTimer(360, 120 * 100);

            RiderPickupAddresstxt = (TextView)alertDialog.FindViewById(Resource.Id.RiderPickupAddresstxt);
            RiderDestinationtxt = (TextView)alertDialog.FindViewById(Resource.Id.RiderDestinationtxt);
            RiderNametxt = (TextView)alertDialog.FindViewById(Resource.Id.RiderNametxt);
            Paymentmethodtxt = (TextView)alertDialog.FindViewById(Resource.Id.Paymentmethodtxt);

            // Cancel Trip Before Accept
            TextView cancel = (TextView)alertDialog.FindViewById(Resource.Id.cancelTrip);
            cancel.Click += Cancel_Click;

            RiderPickupAddresstxt.Text = PickupAddress;
            RiderDestinationtxt.Text = DestinationAddress;
            RiderNametxt.Text = RiderName;
            Paymentmethodtxt.Text = Paymentmethod;

            // Start miusc
            string filePath = "http://server6.mp3quran.net/thubti/001.mp3";
            StartPlayer(filePath);
        }

        // Cancel Trip befor Accept It
        private void Cancel_Click(object sender, EventArgs e)
        {
            CancelTrips();
        }

        // Cancel Trip After Accept it
        private void Cancel_Trip_Click(object sender, EventArgs e)
        {
            CancelTrips();
        }


        // When Driver Accept The Trip
        private void LinearTimerView_Click(object sender, EventArgs e)
        {
            CloseProgressDialog();
            appmainToolbar.Visibility = ViewStates.Gone;
            appmainToolbarRiderinfo.Visibility = ViewStates.Visible;
            GoToRider.Visibility = ViewStates.Visible;
            
            // Show Ridere Data After Accept Trip
            ridernameInHidenDelog.Text = RiderName;
        }

        private void Bnve_NavigationItemSelected(object sender, Android.Support.Design.Widget.BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.action_earning)
            {
                viewPager.SetCurrentItem(1, true);
                BnveToAccentColor(1);
            }
            else if(e.Item.ItemId == Resource.Id.action_home)
            {
                viewPager.SetCurrentItem(0, true);
                BnveToAccentColor(0);
            }
            else if (e.Item.ItemId == Resource.Id.action_rating)
            {
                viewPager.SetCurrentItem(2, true);
                BnveToAccentColor(2);

            }
            else if (e.Item.ItemId == Resource.Id.action_account)
            {
                viewPager.SetCurrentItem(3, true);
                BnveToAccentColor(3);
            }
        }

        void BnveToAccentColor(int index)
        {
            //Set all to white
         
            var img = bnve.GetIconAt(1);
            var txt = bnve.GetLargeLabelAt(1);
            img.SetColorFilter(Color.Rgb(255, 255, 255));
            txt.SetTextColor(Color.Rgb(255, 255, 255));
            
            var img0 = bnve.GetIconAt(0);
            var txt0 = bnve.GetLargeLabelAt(0);
            img0.SetColorFilter(Color.Rgb(255, 255, 255));
            txt0.SetTextColor(Color.Rgb(255, 255, 255));

            var img2 = bnve.GetIconAt(2);
            var txt2 = bnve.GetLargeLabelAt(2);
            img2.SetColorFilter(Color.Rgb(255, 255, 255));
            txt2.SetTextColor(Color.Rgb(255, 255, 255));

            var img3 = bnve.GetIconAt(3);
            var txt3 = bnve.GetLargeLabelAt(3);
            img3.SetColorFilter(Color.Rgb(255, 255, 255));
            txt3.SetTextColor(Color.Rgb(255, 255, 255));

            //Sets Accent Color
            var imgindex = bnve.GetIconAt(index);
            var textindex = bnve.GetLargeLabelAt(index);
            imgindex.SetColorFilter(Color.Rgb(24, 191, 242));
            textindex.SetTextColor(Color.Rgb(24, 191, 242));
        }

        private void SetupViewpager()
        {
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragemts(HomeFragment, "Home");
            adapter.AddFragemts(EarningsFragment, "Earning");
            adapter.AddFragemts(RatingsFragment, "Rating");
            adapter.AddFragemts(AccountFragment, "Account");
            viewPager.Adapter = adapter;
        }

        bool CheckSpecialPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(permissionGroup, requestId);
            }
            else
            {
                permissionGranted = true;
            }
            return permissionGranted;
        }


        //@"C:\Users\Ahmed-PC\Desktop\Wagheety-Driver\Wagheety-Driver\Resources\Raw\Request.mp3"
        //readonly string filePath = "http://server6.mp3quran.net/thubti/001.mp3";

        protected MediaPlayer player;
        public void StartPlayer(String filePath)
        {
            if (player == null)
            {
                player = MediaPlayer.Create(this, Resource.Raw.Request);
                player.Start();
            }
            else
            {
                //player.Reset();
                //player.SetDataSource(filePath);
                //player.Prepare();
                player.Start();
            }
        }

        void CloseProgressDialog()
        {
            if (alert != null)
            {
                alertDialog.Dismiss();
                alert.Dispose();
                alertDialog = null;
                alert = null;
                player.Stop();
                player.Dispose();
                player = null;
            }
        }


        // Cnacel Trips
        void CancelTrips()
        {
            CloseProgressDialog();
            appmainToolbar.Visibility = ViewStates.Visible;
            appmainToolbarRiderinfo.Visibility = ViewStates.Gone;
            StartTrip.Visibility = ViewStates.Gone;
            GoToRider.Visibility = ViewStates.Gone;

            // Time Between Every Trip 3 Secound
            Thread.Sleep(3000);

            // Find Anther Trip
            requestlistener.GetRequstes();
            if (requestlistener != null)
            {
                requestlistener.DataRetrived += Requestlistener_DataRetrived;
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}