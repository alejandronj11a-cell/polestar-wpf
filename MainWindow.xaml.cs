using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PoleStar
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Pantallas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != Pantallas) return;

            if (Pantallas.SelectedContent is UIElement contenido)
            {
                contenido.BeginAnimation(OpacityProperty,
                    new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250)));
            }
        }

        private void IrAGaleria_Click(object sender, RoutedEventArgs e)
        {
            Pantallas.SelectedIndex = 2;
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            bool vacio = string.IsNullOrWhiteSpace(TxtUsuario.Text) || PwdClave.Password.Length == 0;
            bool correcto = TxtUsuario.Text == "cliente" && PwdClave.Password == "polestar";

            if (correcto)
            {
                TxtError.Visibility = Visibility.Collapsed;
                PwdClave.Clear();
                Pantallas.SelectedIndex = 2;
                return;
            }

            TxtError.Text = vacio ? "Introduce usuario y contraseña."
                                  : "Usuario o contraseña incorrectos.";
            TxtError.Visibility = Visibility.Visible;

            System.Windows.Automation.Peers.UIElementAutomationPeer.FromElement(TxtError)?
                .RaiseAutomationEvent(System.Windows.Automation.Peers.AutomationEvents.LiveRegionChanged);

            ((Storyboard)FindResource("Sacudida")).Begin(TarjetaLogin);
        }

        private void CmbFiltro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PanelGaleria == null) return;

            string filtro = (CmbFiltro.SelectedItem as ComboBoxItem)?.Tag as string ?? "Todos";
            foreach (FrameworkElement tarjeta in PanelGaleria.Children)
            {
                bool visible = filtro == "Todos" || (tarjeta.Tag as string) == filtro;
                tarjeta.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
