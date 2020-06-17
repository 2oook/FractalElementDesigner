using RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor
{
    public class WindowsClipboard : IClipboard
    {
        public bool ContainsText()
        {
            return Clipboard.ContainsText();
        }

        public string GetText()
        {
            return Clipboard.GetText();
        }

        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    } 
}
