using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MonoGame.Interop.Wpf
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.MAIN_GAME.Dispose();
            base.OnClosed(e);
        }

        private void OPEN_WIN_Click(Object sender, RoutedEventArgs e)
        {
            new NewWindow().ShowDialog();
        }

        private void UNLOAD_Click(Object sender, RoutedEventArgs e)
        {
            this.MAIN_GAME.Content = null;
        }
    }
}
