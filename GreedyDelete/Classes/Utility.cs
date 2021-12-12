using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
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
        public const int VSMinimumSpacesInLine = 8;

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

        public static bool IsWhiteSpaceChange(IWpfTextView textView, TextContentChangedEventArgs eventArgs)
        {
            if (textView == null || eventArgs == null || eventArgs.Changes.Count == 0)
                return false;

            int indexOfCurrentLine = GetCurrentLineIndex(textView);

            string lineTextBeforeChange = textView.Caret.ContainingTextViewLine.Extent.GetText();
            string lineTextAfterChange = eventArgs.After.GetLineFromLineNumber(indexOfCurrentLine).GetText();

            if (!string.IsNullOrWhiteSpace(lineTextBeforeChange) || !string.IsNullOrWhiteSpace(lineTextAfterChange))
                return false;

            if (lineTextAfterChange.Length == VSMinimumSpacesInLine) /* Temporary Hardcode */
                return false;

            return lineTextAfterChange.Length > lineTextBeforeChange.Length;
        }

        public static int GetCurrentLineIndex(IWpfTextView textView)
        {
            if (textView == null)
                return -1;

            ITextViewLine currentLine = textView.Caret.ContainingTextViewLine;

            return textView.TextViewLines.GetIndexOfTextLine(currentLine);
        }
    }
}
