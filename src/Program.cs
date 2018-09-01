using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace EveClipboardWatcher
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                mutex.ReleaseMutex();
            }
            else
            {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                if (Uri.TryCreate(args[0], UriKind.Absolute, out var uri) &&
                    string.Equals(uri.Scheme, MainForm.UriScheme, StringComparison.OrdinalIgnoreCase))
                {
                    /*NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);*/
                    int hwnd = 0;
                    foreach (Process pList in Process.GetProcesses())
                    {
                        if (pList.MainWindowTitle.Contains("Eve Clipboardwatcher"))
                        {
                            hwnd = (int) pList.MainWindowHandle;
                        }
                    }

                    MessageHelper msgHelper = new MessageHelper();
                    string xUri = uri.ToString();
                    msgHelper.sendWindowsStringMessage(hwnd, 0, xUri);
                }
            }
        }
    }
}
