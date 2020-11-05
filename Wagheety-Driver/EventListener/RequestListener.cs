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
using Firebase.Database;
using Wagheety_Driver.Data_Model;
using Wagheety_Driver.Helpers;

namespace Wagheety_Driver.EventListener
{
    public class RequestListener : Java.Lang.Object, IValueEventListener
    {
        List<Receiving_NewTripDetails> receiving_NewTripsList = new List<Receiving_NewTripDetails>();

        public event EventHandler<DataEventArgs> DataRetrived;

        public class DataEventArgs:EventArgs
        {
            public List<Receiving_NewTripDetails> Receiving_NewTrip { get; set; }
        }


        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }


        public string id = "";
        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>().Where(a => a.Key != id).ToList();
                //receiving_NewTripsList.Clear();

                foreach (var item in child)
                {
                    Receiving_NewTripDetails receiving_NewTrip = new Receiving_NewTripDetails();
                    receiving_NewTrip.RideID = item.Key;
                    receiving_NewTrip.rider_name = item.Child("rider_name").Value.ToString();
                    receiving_NewTrip.rider_phone = item.Child("rider_phone").Value.ToString();
                    receiving_NewTrip.PickupAddress = item.Child("pickup_address").Value.ToString();
                    receiving_NewTrip.DestinationAddress = item.Child("destination_address").Value.ToString();
                    receiving_NewTrip.Paymentmethod = item.Child("payment_method").Value.ToString();

                    receiving_NewTrip.PickupLat = (double) item.Child("location/latitude").Value;
                    receiving_NewTrip.PickupLng = (double) item.Child("location/longitude").Value;

                    // Destination lang
                    receiving_NewTrip.DestinationLat = (double)item.Child("destination/latitude").Value;
                    receiving_NewTrip.DestinationLng = (double)item.Child("destination/longitude").Value;

                    receiving_NewTripsList.Add(receiving_NewTrip);

                    // Make it inside foreach to call Record By Record
                    DataRetrived.Invoke(this, new DataEventArgs { Receiving_NewTrip = receiving_NewTripsList });
                    id = item.Key;
                    return;
                }

                
            }
        }

        public void GetRequstes()
        {
            DatabaseReference reference = AppDataHelper.GetDatabase().GetReference("rideRequest");
            reference.AddValueEventListener(this);
        }
    }
}