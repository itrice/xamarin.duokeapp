using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly:Xamarin.Forms.Dependency(typeof(JZXY.Duoke.Droid.DocumentViewer))]
namespace JZXY.Duoke.Droid
{
    public class DocumentViewer:JZXY.Duoke.Interface.IDocumentViewer
    {
        [Obsolete]
        public void ShowDocumentFile(string filepath, string mimeType)
        {
            var rs = System.IO.File.Exists(filepath);

            //var uri = Android.Net.Uri.FromFile(new Java.IO.File(filepath));Forms.Context.ApplicationContext.PackageName
            //var uri = FileProvider.GetUriForFile(MainActivity.AppContext, MainActivity.AppContext.PackageName+ ".provider", new Java.IO.File(filepath));
            //Java.IO.File file = new Java.IO.File(filepath);
            //var uri = FileProvider.GetUriForFile(Forms.Context.ApplicationContext, BuildConfig.ApplicationId, new Java.IO.File(filepath));            
            //var uri = Android.Net.Uri.Parse( "https://us.v-cdn.net/5019960/uploads/userpics/171/pQQ7DQ5VAJMJV.jpg");
            //var uri = Android.Net.Uri.Parse("content://" + filepath);
            var uri = Android.Net.Uri.Parse("file://" + filepath);
            ////Device.OpenUri(new Uri(filepath));
            var intent = new Intent(Intent.ActionView);
            //var uri = null;

            intent.SetFlags(ActivityFlags.GrantReadUriPermission);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.N)
            {
                //uri = FileProvider.GetUriForFile(MainActivity.AppContext, MainActivity.AppContext.PackageName + ".provider", new Java.IO.File(filepath));
            }

            intent.SetDataAndType(uri, mimeType);

            Forms.Context.StartActivity(Intent.CreateChooser(intent, "Select App"));            
        }

        public DocumentViewer()
        {

        }

        private String GetMimeType(Android.Net.Uri uri)
        {
            String mimeType = null;
            if (uri.Scheme.Equals(ContentResolver.SchemeContent))
            {
                ContentResolver cr = Android.App.Application.Context.ContentResolver;
                mimeType = cr.GetType(uri);
            }
            else
            {
                String fileExtension = MimeTypeMap.GetFileExtensionFromUrl(uri.ToString());
                mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(fileExtension.ToLower());
            }
            return mimeType;
        }
    }

}