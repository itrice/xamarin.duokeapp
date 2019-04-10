using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JZXY.Duoke
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = Source == null ? null : Xamarin.Forms.ImageSource.FromResource(Source, GetType().GetTypeInfo().Assembly);
            return result;
        }
    }

    [ContentProperty("Source")]
    public class ImageMultiResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = Source == null ? null : Forms9Patch.ImageSource.FromMultiResource(Source, GetType().GetTypeInfo().Assembly);
            return result;
        }
    }
}
