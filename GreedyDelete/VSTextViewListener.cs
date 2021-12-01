using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace GreedyDelete
{
    [ContentType("code")]
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class VSTextViewListener : IWpfTextViewCreationListener
    {
        public EmptyLineRemover _emptyLineRemover = null;

        public void TextViewCreated(IWpfTextView textView)
        {
            _emptyLineRemover = new EmptyLineRemover(textView);
            textView.TextBuffer.PostChanged += new EventHandler(TextBuffer_PostChanged);

        }
        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
            _emptyLineRemover.HandleTextChange();
        }
    }
}
