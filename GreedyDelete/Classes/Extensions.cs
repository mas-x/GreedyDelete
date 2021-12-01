using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace GreedyDelete.Classes
{
    public static class Extensions
    {
        public static void HandleTextChange(this WpfTextViewHandler wpfTextViewHandler)
        {
            if (wpfTextViewHandler == null || wpfTextViewHandler.IsHandlingChange)
                return;

            ITextViewLine currentLine = wpfTextViewHandler.TextView.Caret.ContainingTextViewLine;
            bool isCurrentLineEmpty = string.IsNullOrWhiteSpace(currentLine.Extent.GetText());
            int currentLineIndex = wpfTextViewHandler.TextView.TextViewLines.IndexOf(wpfTextViewHandler.TextView.Caret.ContainingTextViewLine);

            if (wpfTextViewHandler.FoundEmptyLineInPreviousChange)
            {
                if (wpfTextViewHandler.EmptyLineIndexInPreviousChange != currentLineIndex || !isCurrentLineEmpty)
                {
                    wpfTextViewHandler.FoundEmptyLineInPreviousChange = false;
                    wpfTextViewHandler.EmptyLineIndexInPreviousChange = -1;
                    return;
                }
            }

            if (isCurrentLineEmpty)
            {
                if (wpfTextViewHandler.EmptyLineIndexInPreviousChange != currentLineIndex || !wpfTextViewHandler.FoundEmptyLineInPreviousChange)
                {
                    wpfTextViewHandler.EmptyLineIndexInPreviousChange = currentLineIndex;
                    wpfTextViewHandler.FoundEmptyLineInPreviousChange = true;
                    return;
                }

                wpfTextViewHandler.IsHandlingChange = true;

                RemoveLine(currentLine, wpfTextViewHandler.TextView);

                ITextViewLine lineToMoveCursorTo = wpfTextViewHandler.TextView.TextViewLines[wpfTextViewHandler.TextView.TextViewLines.IndexOf(wpfTextViewHandler.TextView.Caret.ContainingTextViewLine) - 1];
                while (string.IsNullOrWhiteSpace(lineToMoveCursorTo.Extent.GetText()))
                {
                    ITextViewLine tempCopy = lineToMoveCursorTo;

                    lineToMoveCursorTo = wpfTextViewHandler.TextView.TextViewLines[wpfTextViewHandler.TextView.TextViewLines.IndexOf(tempCopy) - 1];

                    RemoveLine(tempCopy, wpfTextViewHandler.TextView);
                }

                wpfTextViewHandler.TextView.Caret.MoveTo(lineToMoveCursorTo, lineToMoveCursorTo.Right);

                wpfTextViewHandler.FoundEmptyLineInPreviousChange = false;
                wpfTextViewHandler.EmptyLineIndexInPreviousChange = -1;

                wpfTextViewHandler.IsHandlingChange = false;
            }
        }

        private static void RemoveLine(ITextViewLine lineToRemove, IWpfTextView textView)
        {
            if (textView == null || lineToRemove == null)
                return;

            ITextEdit textEdit = textView.TextBuffer.CreateEdit();
            textEdit.Delete(lineToRemove.Start, lineToRemove.LengthIncludingLineBreak);
            textEdit.Apply();
        }
    }
}
