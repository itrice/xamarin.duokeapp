using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Xamarin.Forms;

[assembly:Xamarin.Forms.Dependency(typeof(JZXY.Duoke.Droid.DocumentViewer))]
namespace JZXY.Duoke.Droid
{
    public class DocumentViewer:JZXY.Duoke.Interface.IDocumentViewer
    {
        [Obsolete]
        public void ShowDocumentFile(string filepath, string mimeType)
        {
            var uri = Android.Net.Uri.Parse("file://" + filepath);
            var intent = new Intent(Intent.ActionView);
            //mimeType = GetMimeType(uri);
            intent.SetDataAndType(uri, mimeType);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
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