using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Wagheety_Driver.EventListener;
using Wagheety_Driver.Helpers;

namespace Wagheety_Driver.Activities
{
    [Activity(Label = "loginActivity",MainLauncher =false,Theme = "@style/DriverTheme")]
    public class loginActivity : AppCompatActivity
    {
        Button Loginbtn;
        TextInputLayout Emailtxt;
        TextInputLayout passwordtxt;
        CoordinatorLayout rootview;
        TextView clickTosignUp;

        FirebaseAuth mAuth;
        FirebaseDatabase database;
        FirebaseUser currentuser;

        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            // Create your application here
            Connectviews();
            InitializeFirebase();
        }

        void InitializeFirebase()
        {
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentuser = AppDataHelper.GetCurrentUser();
            database = AppDataHelper.GetDatabase();
        }

        void Connectviews()
        {
            Loginbtn = (Button)FindViewById(Resource.Id.btnLogin);
            Emailtxt = (TextInputLayout)FindViewById(Resource.Id.Email);
            passwordtxt = (TextInputLayout)FindViewById(Resource.Id.Password);
            rootview = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            clickTosignUp = (TextView)FindViewById(Resource.Id.gotoregister);

            Loginbtn.Click += Loginbtn_Click;
            clickTosignUp.Click += ClickTosignUp_Click;
        }

        private void ClickTosignUp_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
            Finish();
        }

        private void Loginbtn_Click(object sender, EventArgs e)
        {
            string email, password;
            email = Emailtxt.EditText.Text;
            password = passwordtxt.EditText.Text;

            ShowProgressDialog();
            
            TaskComletionListener taskComletionListener = new TaskComletionListener();
            taskComletionListener.Success += TaskComletionListener_Success;
            taskComletionListener.Failure += TaskComletionListener_Failure;

            if (!email.Contains("@"))
            {
                CloseProgressDialog();
                Snackbar.Make(rootview, "Please Enter Right Email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password == null)
            {
                CloseProgressDialog();
                Snackbar.Make(rootview, "please Enter password", Snackbar.LengthShort).Show();
                return;
            }


            try
            {
                mAuth.SignInWithEmailAndPassword(email, password)
                       .AddOnSuccessListener(this, taskComletionListener)
                       .AddOnFailureListener(this, taskComletionListener);

            }
            catch (Exception)
            {
                CloseProgressDialog();
                Snackbar.Make(rootview, "Oppse Some thing is Wrong !!", Snackbar.LengthShort).Show();
            }
        }

        private void TaskComletionListener_Failure(object sender, EventArgs e)
        {
            CloseProgressDialog();
            Snackbar.Make(rootview, "Login Failed", Snackbar.LengthShort).Show();
        }

        private void TaskComletionListener_Success(object sender, EventArgs e)
        {
            CloseProgressDialog();
            StartActivity(typeof(MainActivity));
            Finish();
        }

        void ShowProgressDialog()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.progress);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void CloseProgressDialog()
        {
            if (alert != null)
            {
            alertDialog.Dismiss();
            alertDialog = null;
            alert = null;
            }
        }


    }
}