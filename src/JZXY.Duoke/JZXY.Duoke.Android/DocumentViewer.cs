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
                //var uri = Android.Net.Uri.FromFile(new Java.IO.File(filepath));Forms.Context.ApplicationContext.PackageName
                //var uri = FileProvider.GetUriForFile(MainActivity.AppContext, MainActivity.AppContext.PackageName+ ".provider", new Java.IO.File(filepath));
                //Java.IO.File file = new Java.IO.File(filepath);
                //var uri = Android.Net.Uri.Parse( "https://us.v-cdn.net/5019960/uploads/userpics/171/pQQ7DQ5VAJMJV.jpg");
                //var uri = Android.Net.Uri.Parse("content://" + file.AbsolutePath);                
                var uri = Android.Net.Uri.Parse("file://" + filepath);
                //Device.OpenUri(new Uri(filepath));
                var ps = GetActualPathFromFile(uri);
                var intent = new Intent(Intent.ActionView);
                intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.N)
                {
                    //uri = FileProvider.GetUriForFile(MainActivity.AppContext, MainActivity.AppContext.PackageName + ".provider", new Java.IO.File(filepath));
                }

                intent.SetDataAndType(uri, mimeType);

                Forms.Context.StartActivity(Intent.CreateChooser(intent, "Select App"));
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

        private string GetActualPathFromFile(Android.Net.Uri uri)
        {
            bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

            var context = Android.App.Application.Context;
            if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (isExternalStorageDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }
                }
                // DownloadsProvider
                else if (isDownloadsDocument(uri))
                {
                    string id = DocumentsContract.GetDocumentId(uri);

                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                                    Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                    //System.Diagnostics.Debug.WriteLine(contentUri.ToString());

                    return getDataColumn(context, contentUri, null, null);
                }
                // MediaProvider
                else if (isMediaDocument(uri))
                {
                    String docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    String[] split = docId.Split(chars);

                    String type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    String selection = "_id=?";
                    String[] selectionArgs = new String[]
                    {
                split[1]
                    };

                    return getDataColumn(context, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {

                // Return the remote address
                if (isGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                return getDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        public static String getDataColumn(Context context, Android.Net.Uri uri, String selection, String[] selectionArgs)
        {
            ICursor cursor = null;
            String column = "_data";
            String[] projection =
            {
        column
    };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }

        //Whether the Uri authority is ExternalStorageProvider.
        public static bool isExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is DownloadsProvider.
        public static bool isDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is MediaProvider.
        public static bool isMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is Google Photos.
        public static bool isGooglePhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority);
        }
    }    
}