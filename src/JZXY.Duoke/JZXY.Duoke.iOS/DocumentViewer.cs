using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using QuickLook;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(JZXY.Duoke.iOS.DocumentViewer))]
namespace JZXY.Duoke.iOS
{
    public class DocumentViewer : JZXY.Duoke.Interface.IDocumentViewer
    {
        public DocumentViewer()
        {

        }

        public void ShowDocumentFile(string filepath, string mimeType)
        {
            var fileinfo = new FileInfo(filepath);
            var previewController = new QLPreviewController
            {
                DataSource = new PreviewControllerDataSource(fileinfo.FullName, fileinfo.Name)
            };

            var controller = FindNavigationController();

            controller?.PresentViewController(previewController, true, null);
        }

        private UINavigationController FindNavigationController()
        {
            foreach (var window in UIApplication.SharedApplication.Windows)
            {
                if (window.RootViewController.NavigationController != null)
                {
                    return window.RootViewController.NavigationController;
                }

                var value = CheckSubs(window.RootViewController.ChildViewControllers);
                if (value != null)
                    return value;
            }

            return null;
        }

        private UINavigationController CheckSubs(UIViewController[] controllers)
        {
            foreach (var controller in controllers)
            {
                if (controller.NavigationController != null)
                {
                    return controller.NavigationController;
                }

                var value = CheckSubs(controller.ChildViewControllers);

                return value;
            }

            return null;
        }
    }

    public class DocumentItem : QLPreviewItem
    {
        private readonly string _uri;

        public DocumentItem(string title, string uri)
        {
            ItemTitle = title;
            _uri = uri;
        }

        public override string ItemTitle { get; }

        public override NSUrl ItemUrl => NSUrl.FromFilename(_uri);
    }

    public class PreviewControllerDataSource : QLPreviewControllerDataSource
    {
        private readonly string _url;
        private readonly string _filename;

        public PreviewControllerDataSource(string url, string filename)
        {
            _url = url;
            _filename = filename;
        }

        public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
        {
            return new DocumentItem(_filename, _url);
        }

        public override nint PreviewItemCount(QLPreviewController controller)
        {
            return 1;
        }
    }
}