using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreedyDelete
{
    public class WpfTextViewHandler
    {
        #region Properties

        public IWpfTextView TextView { get; set; }

        public bool IsHandlingChange { get; set; }
        public bool FoundEmptyLineInPreviousChange { get; set; }
        public bool IgnoreChange { get; set; }

        public int EmptyLineIndexInPreviousChange { get; set; }

        private string PreviousLineText { get; set; }

        #endregion

        #region Methods

        public void HandleChange()
        {
            if (IsHandlingChange)
                return;

            ITextViewLine currentLine = TextView.Caret.ContainingTextViewLine;
            string currentLineText = currentLine.Extent.GetText();

            if (IgnoreChange)
            {
                IgnoreChange = false;
                PreviousLineText = string.Empty;
                return;
            }

            if (!string.IsNullOrWhiteSpace(currentLineText) || !string.IsNullOrWhiteSpace(PreviousLineText))
            {
                PreviousLineText = currentLineText;
                return;
            }            

            int currentLineIndex = TextView.TextViewLines.IndexOf(TextView.Caret.ContainingTextViewLine);

            IsHandlingChange = true;

            RemoveLine(currentLine);

            ITextViewLine lineToMoveCursorTo = TextView.TextViewLines[TextView.TextViewLines.IndexOf(TextView.Caret.ContainingTextViewLine) - 1];
            while (string.IsNullOrWhiteSpace(lineToMoveCursorTo.Extent.GetText()))
            {
                ITextViewLine tempCopy = lineToMoveCursorTo;

                lineToMoveCursorTo = TextView.TextViewLines[TextView.TextViewLines.IndexOf(tempCopy) - 1];

                RemoveLine(tempCopy);
            }

            TextView.Caret.MoveTo(lineToMoveCursorTo, lineToMoveCursorTo.Right);

            PreviousLineText = string.Empty;

            IsHandlingChange = false;
        }

        private void RemoveLine(ITextViewLine lineToRemove)
        {
            if (TextView == null || TextView.TextBuffer == null || lineToRemove == null)
                return;

            ITextEdit textEdit = TextView.TextBuffer.CreateEdit();
            textEdit.Delete(lineToRemove.Start, lineToRemove.LengthIncludingLineBreak);
            textEdit.Apply();
        }

        #endregion
    }
}
