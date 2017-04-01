using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StoredProcedureCaller;

namespace SPCodeGenForm
{
    public class NewTextBox : TextBox
    {
        public delegate void PastedEventDelegateType(string spname);
        public event PastedEventDelegateType PastedEvent;

        private const int WM_PASTE = 0x0302;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PASTE)
            {
                if (Clipboard.ContainsText() == true)
                {
                    string sign = Clipboard.GetText();

                    string signLower = sign.ToLower();

                    int pos = signLower.IndexOf("proc");

                    if (pos == -1)
                        return;

                    bool PosInSpaceBefProcName = false;
                    bool PosProcName = false;
                    string spname = string.Empty;
                    for (int i = pos; i < sign.Length; ++i)
                    {
                        char c = sign[i];

                        if (PosInSpaceBefProcName == false)
                        {
                            if (SPSignature.IsWhitespace(c) == false)
                                continue;
                            else
                            {
                                PosInSpaceBefProcName = true;
                            }
                        }
                        else if (PosInSpaceBefProcName && PosProcName == false)
                        {
                            if (SPSignature.IsWhitespace(c))
                                continue;
                            else
                            {
                                spname += c;
                                PosProcName = true;
                            }
                        }
                        else if (PosProcName)
                        {
                            if (SPSignature.IsWhitespace(c) == false && c != '(')
                                spname += c;
                            else
                            {

                                break;
                            }
                        }
                    }

                    PastedEvent(spname.Trim());
                }
            }
            base.WndProc(ref m);
        }
    }
}
