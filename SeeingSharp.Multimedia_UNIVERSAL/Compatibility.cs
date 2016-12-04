using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia
{
    public delegate void CancelEventHandler(object sender, CancelEventArgs e);

    public class CancelEventArgs : EventArgs
    {
        public CancelEventArgs(bool doCancel = false)
        {
            this.Cancel = doCancel;
        }

        public bool Cancel { get; set; }
    }
}
