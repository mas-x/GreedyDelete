using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreedyDelete.Classes
{
    public class Utility
    {
        public static bool IsXAMLNewLine(TextContentChangedEventArgs eventArgs)
        {
            if (eventArgs == null || eventArgs.Changes.Count == 0)
                return false;

            return string.IsNullOrEmpty(eventArgs.Changes[0].OldText)
                && string.IsNullOrWhiteSpace(eventArgs.Changes[0].NewText)
                && IsContentTypeXaml(eventArgs.After.ContentType);
        }

        public static bool IsContentTypeXaml(IContentType contentType)
        {
            return contentType.DisplayName == "XAML";
        }
    }
}
