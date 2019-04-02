using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(JZXY.Duoke.Droid.DocumentViewer))]
namespace JZXY.Duoke.Droid
{
    public class DocumentViewer : JZXY.Duoke.Interface.IDocumentViewer
    {
        [Obsolete]
        public void ShowDocumentFile(string filepath, string mimeType)
        {
            var rs = System.IO.File.Exists(filepath);
            if (rs)
            {
                var context = Android.App.Application.Context;
                var file = new Java.IO.File(filepath);

                //var uri = FileProvider.GetUriForFile(context, "com.jzxy.duoke.provider", new Java.IO.File(filepath));
                //var uri = FileProvider.GetUriForFile(MainActivity.AppContext, MainActivity.AppContext.PackageName+ ".provider", new Java.IO.File(filepath));
                var uri = Android.Net.Uri.FromFile(file);
                try
                {
                    var intent = new Intent(Intent.ActionView);
                    intent.AddCategory(Intent.CategoryDefault);
                    intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                    intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                    intent.SetDataAndType(uri, mimeType);
                    Forms.Context.StartActivity(Intent.CreateChooser(intent, "选择打开的程序"));
                }
                catch (Exception e)
                {
                    Toast.MakeText(context, e.Message, ToastLength.Long).Show();
                }
            }
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

        public string GetRootPath()
        {
            var path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            return Path.Combine(path.ToString());
        }
    }
}