using System;
using System.Collections.Generic;
using System.Text;

namespace JZXY.Duoke.Interface
{
    public interface IDocumentViewer
    {
        void ShowDocumentFile(string filepath, string mimeType);

        string GetRootPath();
    }
}
