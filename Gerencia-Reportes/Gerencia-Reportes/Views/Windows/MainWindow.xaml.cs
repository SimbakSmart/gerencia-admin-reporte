using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Gerencia_Reportes.Views.Windows
{
  
    public partial class MainWindow : Window
    {

        private const int SPI_GETSCREENSAVERRUNNING = 114;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(int uiAction, int uiParam, ref bool pvParam, int fWinIni);
        public MainWindow()
        {
            InitializeComponent();
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           Application.Current.Shutdown();
        }
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool IsScreenSaverRunning()
        {
            bool isRunning = false;
            SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0);
            return isRunning;
        }

        protected override void OnClosed(EventArgs e)
        {
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            base.OnClosed(e);
        }
    }
}
