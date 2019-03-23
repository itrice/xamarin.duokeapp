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

[assembly:Xamarin.Forms.Dependency(typeof(JZXY.Duoke.Droid.Messager))]
namespace JZXY.Duoke.Droid
{
    public class Messager : JZXY.Duoke.Interface.IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();            
        }
    }
}