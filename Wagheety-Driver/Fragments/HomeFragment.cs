using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using Android.Util;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;


namespace Wagheety_Driver.Fragments
{
    public class HomeFragment : Android.Support.V4.App.Fragment, ILocationListener, IOnMapReadyCallback
    {
        static readonly string TAG = "X:" + typeof(MainActivity).Name;

        public GoogleMap mainMap;

        double _Latitude;
        double _Longitude;
        Location _currentLocation;
        LocationManager _locationManager;
        LatLng latLng;
        string _locationProvider;
        double _locationLatitude;
        double _locationLongitude;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.home, container, false);

            SupportMapFragment MapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            MapFragment.GetMapAsync(this);
            InitializeLocationManager();
            return view;
        }

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;

            _locationLatitude = _currentLocation.Latitude;
            _locationLongitude = _currentLocation.Longitude;

            LatLng latLng = new LatLng(_locationLatitude, _locationLongitude);

            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latLng, 15);
            mainMap.MoveCamera(camera);

            //ICON Image     

            MarkerOptions options = new MarkerOptions()
            .SetIcon(null)

            .SetPosition(latLng)
            .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.CarIcon))
            .Draggable(true);

            mainMap.AddMarker(options);
        }
      
        public void OnMapReady(GoogleMap googleMap)
        {
            mainMap = googleMap;
            mainMap.MapType = GoogleMap.MapTypeNormal;
            _Latitude = 29.990004;
            _Longitude = 31.150507;
            LatLng latLng = new LatLng(_Latitude, _Longitude);
            GetmyCurruntLocation(latLng);
        }

        public LatLng GetMyLang()
        {
            _Latitude = 29.990004;
            _Longitude = 31.150507;
            latLng = new LatLng(_Latitude, _Longitude);
            return latLng;
        }

        // OnLocationChanged 
        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)Activity.GetSystemService(Context.LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        public void GetmyCurruntLocation(LatLng MLocation)
        {
            mainMap.MapType = GoogleMap.MapTypeNormal;
            _Latitude = MLocation.Latitude;
            _Longitude = MLocation.Longitude;

            LatLng latLng = new LatLng(_Latitude, _Longitude);

            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latLng, 15);
            mainMap.MoveCamera(camera);

            //ICON Image     

            MarkerOptions options = new MarkerOptions()
            .SetIcon(null)

            .SetPosition(latLng)
            .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.CarIcon))
            .Draggable(true);

            mainMap.AddMarker(options);
        }
    }
}