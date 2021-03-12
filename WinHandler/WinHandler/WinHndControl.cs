using System;
using System.Collections.Generic;

namespace WinHandler
{
    public class WinHndControl : WinHandler
    {

        // private IntPtr controlHandler;
        protected IntPtr mainHandler;

        public int posID
        {
            get
            {
                IEnumerable<IntPtr> components = GetChildWindows(mainHandler);
                int index = 0;
                foreach (var c in components)
                {
                    if (c == HandlePointer)
                        return index;
                    index++;
                }
                return -1;
            }

        }

        public WinHndControl(IntPtr handler, IntPtr mainhandler) : base(handler)
        {
            //HandlePointer = handler;
            this.mainHandler = mainhandler;
        }


        public bool Visible
        {
            get
            {
                return WinHandler.IsWindowVisible(HandlePointer);
            }
        }

        public bool Checked
        {
            get
            {
                int result = SendMessage(this.HandlePointer, BM_GETCHECK, 0, IntPtr.Zero);// 'Send the BM_GETCHECK message
                if (result == BST_CHECKED)
                    return true;
                else
                    return false;
            }
            set
            {
                uint val = (value == true) ? BST_CHECKED : BST_UNCHECKED;
                //  SendMessage(this.HandlePointer, BM_SETCHECK, val, 0);
                SendMessage(this.HandlePointer, BM_SETCHECK, val, IntPtr.Zero);
            }

        }
    }
}
