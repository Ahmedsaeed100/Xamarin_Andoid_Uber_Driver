using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Wagheety_Driver.Data_Model
{
   public class Receiving_NewTripDetails
    {
        public string RideID { get; set; }
        public string rider_name { get; set; }
        public string rider_phone { get; set; }
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }

        public string PickupLatStr { get; set; }
        public string PickupLngStr { get; set; } 

        public double DestinationLat { get; set; }
        public double DestinationLng { get; set; }

        public string DestinationLatStr { get; set; }
        public string DestinationLngStr { get; set; }

        public string PickupAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string DistanceString { get; set; }
        public double DistanceValue { get; set; }
        public string DurationString { get; set; }
        public double DurationValue { get; set; }
        public string Paymentmethod { get; set; }
        public DateTime Timestamp { get; set; }
        public string Driver_ID { get; set; }
        public string Driver_Name { get; set; }
        public string DriverPhone { get; set; }
        public double EstimateFare { get; set; }
        public double Driver_Lat { get; set; }
        public double Driver_Lng { get; set; }
    }
}