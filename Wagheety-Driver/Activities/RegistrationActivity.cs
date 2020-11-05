using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Java.Util;
using Wagheety_Driver.EventListener;
using Wagheety_Driver.Helpers;

namespace Wagheety_Driver.Activities
{
    [Activity(Label = "RegistrationActivity" , MainLauncher = false,Theme = "@style/DriverTheme")]
    public class RegistrationActivity : AppCompatActivity
    {
        TextInputLayout fullnametxt;
        TextInputLayout phonetxt;
        TextInputLayout emailtxt;
        TextInputLayout passwordtxt;
        Button registerbtn;
        CoordinatorLayout rootview;

        FirebaseAuth mAuth;
        FirebaseDatabase database;
        FirebaseUser currentuser;

        TaskComletionListener taskCompletionListener = new TaskComletionListener();

       

        //ISharedPreferences preferences = Application.Context.GetSharedPreferences("Driverinfo", FileCreationMode.Private);
        //ISharedPreferencesEditor editor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);

            ConnectViews();
            SetupFirebase();
            // Create your application here
        }

        void SetupFirebase()
        {
            database = AppDataHelper.GetDatabase();
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentuser = AppDataHelper.GetCurrentUser();
        }

        void ConnectViews()
        {
            fullnametxt = (TextInputLayout)FindViewById(Resource.Id.FullName);
            emailtxt = (TextInputLayout)FindViewById(Resource.Id.Email);
            phonetxt = (TextInputLayout)FindViewById(Resource.Id.PhoneNu);
            passwordtxt = (TextInputLayout)FindViewById(Resource.Id.Password);
            registerbtn = (Button)FindViewById(Resource.Id.btnRegister);
            rootview = (CoordinatorLayout)FindViewById(Resource.Id.rootView);

            registerbtn.Click += Registerbtn_Click;

        }

        private void Registerbtn_Click(object sender, EventArgs e)
        {
            string fullname, email, phone, password;

            fullname = fullnametxt.EditText.Text;
            email = emailtxt.EditText.Text;
            phone = phonetxt.EditText.Text;
            password = passwordtxt.EditText.Text;

            // Check Is Valid Values
            if (fullname.Length < 3)
            {
                Snackbar.Make(rootview, "please put Full Name", Snackbar.LengthShort).Show();
                return;
            }
            else if (phone.Length < 5)
            {
                Snackbar.Make(rootview, "Please Enter a valid Phone Number", Snackbar.LengthShort).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Snackbar.Make(rootview, "Please Enter a valid Email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 4)
            {
                Snackbar.Make(rootview, "Please Enter a password is short than 4 character", Snackbar.LengthShort).Show();
                return;
            }

            mAuth.CreateUserWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this,taskCompletionListener)
                .AddOnFailureListener(this,taskCompletionListener);

            taskCompletionListener.Success += (e, g) =>
            {
                DatabaseReference newDriverRef = database.GetReference("drivers/" + mAuth.CurrentUser.Uid);

                HashMap map = new HashMap();
                map.Put("fullname", fullname);
                map.Put("email", email);
                map.Put("phone", phone);
                map.Put("Create_at", DateTime.Now.ToString());

                newDriverRef.SetValue(map);

                Snackbar.Make(rootview, "Driver was registered successfully", Snackbar.LengthShort).Show();

                StartActivity(typeof(MainActivity));
                Finish();
            };

            taskCompletionListener.Failure += (w, r) =>
            {
                Snackbar.Make(rootview, "Driver could not registered", Snackbar.LengthShort).Show();
            };

        }



    }
}