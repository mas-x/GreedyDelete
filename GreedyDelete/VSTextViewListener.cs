using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using GreedyDelete.Classes;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace GreedyDelete
{
    [ContentType("code")]
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class VSTextViewCreationListener : IWpfTextViewCreationListener
    {
        private List<WpfTextViewHandler> _wpfTextViewHandlers = new List<WpfTextViewHandler>();

        public void TextViewCreated(IWpfTextView textView)
        {
            if (WrapperAlreadyExistsForTextView(textView))
                return;

            textView.TextBuffer.PostChanged += TextBuffer_PostChanged;
            textView.Closed += TextView_Closed;

            _wpfTextViewHandlers.Add(new WpfTextViewHandler()
            {
                TextView = textView
            });
        }

        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
            WpfTextViewHandler handler = GetHandlerForTextBuffer(sender as ITextBuffer);

            if (handler != null)
                handler.HandleTextChange();
        }

        private bool WrapperAlreadyExistsForTextView(IWpfTextView textView)
        {
            return _wpfTextViewHandlers.FindIndex(x => x.TextView == textView) >= 0;
        }

        private WpfTextViewHandler GetHandlerForTextView(IWpfTextView textView)
        {
            return _wpfTextViewHandlers.Find(x => x.TextView == textView);
        }

        private WpfTextViewHandler GetHandlerForTextBuffer(ITextBuffer textBuffer)
        {
            return _wpfTextViewHandlers.Find(x => x.TextView.TextBuffer == textBuffer);
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            WpfTextViewHandler handler = GetHandlerForTextView(sender as IWpfTextView);

            if (handler != null)
            {
                handler.TextView.TextBuffer.PostChanged -= TextBuffer_PostChanged;
                handler.TextView.Closed -= TextView_Closed;

                _wpfTextViewHandlers.Remove(handler);
            }
        }
    }
}
