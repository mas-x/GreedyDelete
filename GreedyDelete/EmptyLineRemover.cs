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
    public class EmptyLineRemover
    {
        private bool _handlingChange = false;
        private bool _foundEmptyLineInPreviousChange = false;
        private int _emptyLineIndexInPreviousChange = -1;

        private IWpfTextView m_TextView = null;

        public EmptyLineRemover(IWpfTextView textView)
        {
            m_TextView = textView;
        }

        public void HandleTextChange()
        {
            if (_handlingChange)
                return;

            bool bCurrentLineIsEmpty = string.IsNullOrWhiteSpace(m_TextView.Caret.ContainingTextViewLine.Extent.GetText());
            int currentLineIndex = m_TextView.TextViewLines.IndexOf(m_TextView.Caret.ContainingTextViewLine);

            if (_foundEmptyLineInPreviousChange)
            {
                if (_emptyLineIndexInPreviousChange != currentLineIndex || !bCurrentLineIsEmpty)
                {
                    _foundEmptyLineInPreviousChange = false;
                    _emptyLineIndexInPreviousChange = -1;
                    return;
                }
            }

            if (bCurrentLineIsEmpty)
            {
                if (currentLineIndex != _emptyLineIndexInPreviousChange || !_foundEmptyLineInPreviousChange)
                {
                    _emptyLineIndexInPreviousChange = currentLineIndex;
                    _foundEmptyLineInPreviousChange = true;
                    return;
                }

                _handlingChange = true;

                ITextViewLine currentTextViewLine = m_TextView.Caret.ContainingTextViewLine;
                RemoveLine(currentTextViewLine);

                ITextViewLine lineToMoveCursorTo = m_TextView.TextViewLines[m_TextView.TextViewLines.IndexOf(m_TextView.Caret.ContainingTextViewLine) - 1];
                while (string.IsNullOrWhiteSpace(lineToMoveCursorTo.Extent.GetText()))
                {
                    ITextViewLine tempCopy = lineToMoveCursorTo;

                    lineToMoveCursorTo = m_TextView.TextViewLines[m_TextView.TextViewLines.IndexOf(tempCopy) - 1];

                    RemoveLine(tempCopy);
                }

                m_TextView.Caret.MoveTo(lineToMoveCursorTo, lineToMoveCursorTo.Right);

                _emptyLineIndexInPreviousChange = -1;
                _foundEmptyLineInPreviousChange = false;

                _handlingChange = false;
            }
        }

        private void RemoveLine(ITextViewLine lineToRemove)
        {
            if (lineToRemove == null)
                return;

            ITextEdit textEdit = m_TextView.TextBuffer.CreateEdit();
            textEdit.Delete(lineToRemove.Start, lineToRemove.LengthIncludingLineBreak);
            textEdit.Apply();
        }
    }
}
