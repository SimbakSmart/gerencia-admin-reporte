

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using System.Windows.Input;

namespace Gerencia_Reportes.ViewModels.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        public bool isDarkMode;

        public bool IsDarkMode
        {
            get { return isDarkMode; }
            set
            {
                isDarkMode = value;
                RaisePropertyChanged(nameof(IsDarkMode));
            }
        }

        public ICommand ToggleThemeCommand { get; private set; }


        public MainWindowViewModel()
        {
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            UpdateTheme();
        }

        private void ToggleTheme()
        {
            IsDarkMode = IsDarkMode ? true : false;
            Properties.Settings.Default.IsDark = IsDarkMode;
            Properties.Settings.Default.Save();
            UpdateTheme();
        }

        private void UpdateTheme()
        {
            if (Properties.Settings.Default.IsDark)
            {
                IsDarkMode = true;
            }

            PaletteHelper paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(IsDarkMode ? BaseTheme.Dark : BaseTheme.Light);
            paletteHelper.SetTheme(theme);
        }
    }
}
