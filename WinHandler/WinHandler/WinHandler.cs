using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WinHandler
{
    public class WinHandler
    {
        #region User32 const

        protected const int WM_KEYDOWN = 0x100;
        protected const int WM_KEYUP = 0x101;

        protected const Int32 WM_CHAR = 0x0102;
        protected const Int32 VK_RETURN = 0x0D;
        protected const int VK_ENTER = 0x0D;

        protected const uint BM_GETCHECK = 0x00F0;
        protected const uint BM_SETCHECK = 0x00F1;

        protected const uint BST_CHECKED = 0x0001;
        protected const uint BST_UNCHECKED = 0x0000;

        protected const uint BM_CLICK = 0x00F5;
        protected const int WM_LBUTTONDOWN = 0x0201;
        protected const int WM_LBUTTONUP = 0x0202;
        protected const int BM_SETSTATE = 0x00F3;

        private const UInt32 WM_CLOSE = 0x0010;

        #endregion
        #region User32 import functions      

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        //Retorna o handle a partir do nome da classe e titulo da pagina
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        protected static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // ativa uma aplicação para uso.
        [DllImport("USER32.DLL")]
        protected static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        protected static extern void SendMessage(IntPtr hWnd, uint msg, int Param, String s);

        [DllImport("user32.dll")]
        protected static extern int SendMessage(IntPtr hWnd, uint msg, uint Param, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


        //Envia ou recebe mensagens de componentes
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern IntPtr SendMessage(IntPtr handleWndow, uint msg, IntPtr wParam, [Out] StringBuilder lParam);

        //Envia ou recebe mensagens de componentes
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern int SendMessage(IntPtr handleWndow, int msg, int wParam, int lParam);

        // Delegata usado para chamar os metodos enumenando as janelas filhas 
        //(nesse caso cada componente de pagina é chamado de janela filha) 
        //retona os processos em memoria.
        private delegate bool EnumWindowProc(IntPtr handleWndow, IntPtr parameter);

        //verifica se um handle existe
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr handleWndow, EnumWindowProc callback, IntPtr i);

        //retorna o caption da janela selecionada pelo handle
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetFocus(IntPtr hWnd);

        protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll")]
        protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);


        #endregion

        public WinHandler(IntPtr hndPointer)
        {
            HandlePointer = hndPointer;
        }

        public WinHandler(string processName, SearchType searchType)
        {
            HandlePointer = GetHandlePointer(processName, searchType);
        }

        public string ClassName
        {
            get
            {
                int nRet;
                // Pre-allocate 256 characters, since this is the maximum class name length.
                StringBuilder className = new StringBuilder(256);
                //Get the window class name
                nRet = GetClassName(HandlePointer, className, className.Capacity);
                if (nRet != 0)
                {
                    return className.ToString();
                }
                else
                {
                    return "";
                }

            }
        }

        public Rectangle Rectangle
        {
            get
            {
                Rectangle rect;
                GetWindowRect(this.HandlePointer, out rect);
                return rect;
            }
        }

        public void SetFocus()
        {
            SetFocus(this.HandlePointer);
        }

        protected static bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
        {
            //  int size = GetWindowTextLength(hWnd);
            //   if (size++ > 0 && IsWindowVisible(hWnd))
            //  {
            //      StringBuilder sb = new StringBuilder(size);
            string text = GetText(hWnd);
            //     Console.WriteLine(sb.ToString());
            // }
            Console.Write(text);
            return true;
        }

        public static void GetWindowsList()
        {
            //  List<string> list = new List<string>();
            EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
        }

        public IntPtr HandlePointer { get; set; }

        public enum SearchType
        {
            ByProcess = 0,
            ContainsTitle,
            ExactTitle

        }

        public IntPtr GetParent()
        {
            return GetParent(this.HandlePointer);
        }


        public void ActiveWindow()
        {
            if (HandlePointer != IntPtr.Zero)
                SetForegroundWindow(HandlePointer);
        }

        public void Close()
        {
            SendMessage(HandlePointer, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        public static WinHandler GetActiveWindow()
        {
            try
            {
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);

                IntPtr handle = GetForegroundWindow();
                return new WinHandler(handle);
            }
            catch (Exception)
            {

            }
            return null;

        }
        public string Text
        {
            get
            {
                if (!this.isNull())
                    return WinHandler.GetText(HandlePointer);
                else
                    return "";
            }
            set
            {
                WinHandler.SetText(HandlePointer, value);
            }
        }

        public bool isNull()
        {
            if (HandlePointer == null || HandlePointer == IntPtr.Zero)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Return the handle pointer to window that meet the search condition
        /// </summary>
        /// <param name="searchName">Process Name ou Window Title</param>
        /// <param name="searchType">Type of search</param>
        /// <returns>Return IntPtr.Zero if not found any matching</returns>
        public static IntPtr GetHandlePointer(string searchName, SearchType searchType)
        {
            IntPtr hndPointer = IntPtr.Zero;
            if (searchType == SearchType.ByProcess)
            {
                Process[] process = Process.GetProcessesByName(searchName);
                try
                {
                    hndPointer = process[0].MainWindowHandle;
                }
                catch
                {
                    hndPointer = IntPtr.Zero;
                }
            }
            else if (searchType == SearchType.ContainsTitle)
            {
                WinHandler desktop = new WinHandler(new IntPtr(0x10010));
                var allwin = WinHandler.GetChildWindows(desktop.HandlePointer);
                hndPointer = allwin.FirstOrDefault(w => GetText(w).Contains(searchName));
                /*  foreach (var w in allwin)
                  {
                      if (GetText(w).Contains(searchName))
                      {
                          hndPointer = w;
                          return hndPointer;
                      }
                  }*/

            }
            else if (searchType == SearchType.ExactTitle)
            {
                hndPointer = FindWindowByCaption(IntPtr.Zero, searchName);
            }
            return hndPointer;
        }
        public static IntPtr GetHandlePointer(string className, string titleName)
        {
            //função precisa ser revisada se é necessária ou não. Ou talvez melhorada
            IntPtr hndPointer = IntPtr.Zero;
            hndPointer = FindWindow(className, titleName);
            return hndPointer;
        }

        public WinHndControl GetControl(int ControlID)
        {
            IEnumerable<IntPtr> components = GetChildWindows(HandlePointer);
            IntPtr hnd;
            if (components.Count() >= ControlID)
            {
                hnd = components.ElementAt(ControlID);
                var tempControl = new WinHndControl(hnd, HandlePointer);
                // tempControl.
                return tempControl;
            }

            else
                return null;
        }

        public WinHndControl GetControl(string controlText)
        {
            IEnumerable<IntPtr> components = GetChildWindows(HandlePointer);
            var comp = components.FirstOrDefault(c => GetText(c).Replace("\r", "").Replace("\n", "") == controlText);
            return new WinHndControl(comp, HandlePointer);

        }
        public WinHndControl GetControl(string controlText, int returnElementCount)
        {
            IEnumerable<IntPtr> components = GetChildWindows(HandlePointer);
            int counter = 0;
            foreach (var c in components)
            {
                var t = GetText(c);
                t = t.Replace("\r", "");
                t = t.Replace("\n", "");
                if (t == controlText && counter == returnElementCount)
                {
                    return new WinHndControl(c, HandlePointer);
                }
                else if (t == controlText)
                {
                    counter++;
                }
            }
            return null;
        }

        public WinHndControl GetControlWith(string containText)
        {
            IEnumerable<IntPtr> components = GetChildWindows(HandlePointer);

            foreach (var c in components)
            {
                var t = GetText(c);
                t = t.Replace("\r", "");
                t = t.Replace("\n", "");
                if (t.Contains(containText))
                {
                    return new WinHndControl(c, HandlePointer);
                }
            }
            return null;

        }

        public bool ExistControl(string controlText)
        {
            IEnumerable<IntPtr> components = GetChildWindows(HandlePointer);
            foreach (var c in components)
            {
                var t = GetText(c);
                t = t.Replace("\r", "");
                t = t.Replace("\n", "");
                if (t == controlText)
                {
                    return true;
                }
            }
            return false;
        }

        //Verifica se o ponteiro do handle retona um objeto valido
        // Cria um objeto GCHandle gerenciado do ponteiro que representa um identificador para a lista criada em GetChildWindows.
        private static bool EnumChildWindowsCallback(IntPtr handle, IntPtr pointer)
        {
            var gcHandle = GCHandle.FromIntPtr(pointer);
            var list = gcHandle.Target as List<IntPtr>;

            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target não pôde ser lançado como Lista <IntPtr>");
            }
            list.Add(handle);

            return true;
        }

        public static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
        {
            var result = new List<IntPtr>();
            var listHandle = GCHandle.Alloc(result);

            try
            {
                EnumChildWindows(parent, EnumChildWindowsCallback, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }

            return result;
        }

        //Obtém texto de um controle por ele é identificador.
        //Retorna o caption o text de um componente pelo handle do componente
        public static string GetText(IntPtr handle)
        {
            const uint WM_GETTEXTLENGTH = 0x000E;
            const uint WM_GETTEXT = 0x000D;

            var length = (int)SendMessage(handle, WM_GETTEXTLENGTH, IntPtr.Zero, null);
            var sb = new StringBuilder(length + 1);
            SendMessage(handle, WM_GETTEXT, (IntPtr)sb.Capacity, sb);

            return sb.ToString();
        }

        public static void SetText(IntPtr handle, string message)
        {
            const uint WM_SETTEXT = 0x000C;
            SendMessage(handle, WM_SETTEXT, 0, message);
        }

        public void SendKey(Keys key)
        {
            KeysConverter convert = new KeysConverter();
            PostMessage(HandlePointer, WM_KEYDOWN, (int)key, 0);
        }

        public void Click()
        {
            SendMessage(this.HandlePointer, BM_CLICK, 0, IntPtr.Zero);     //send change state
        }
    }
}