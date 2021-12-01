using GreedyDelete.Classes;
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
        public IWpfTextView TextView { get; set; }

        public bool IsHandlingChange { get; set; }
        public bool FoundEmptyLineInPreviousChange { get; set; }
        public int EmptyLineIndexInPreviousChange { get; set; }
    }
}
