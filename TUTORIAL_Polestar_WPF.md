# Tutorial: prototipo Polestar España en WPF (paso a paso)

Proyecto WPF (.NET) con 3 secciones: **Inicio**, **Login ("My Polestar Space")** y **Galería**, con cabecera común (logo + menú) y animaciones sutiles.

## 0. Revisión del borrador vs. enunciado

| Requisito del PDF | Estado del borrador |
|---|---|
| Proyecto WPF | ✅ |
| Pantalla de inicio (novedades, mejor precio, ofertas estacionales) | ✅ Hecha (hero + 3 tarjetas), con precios `XX.XXX €` por rellenar |
| Pantalla de login con PasswordBox y gestión de errores | ❌ No existe |
| Galería de productos | ❌ No existe |
| Logo + menú comunes a todas las pantallas | ⚠️ Están en la cabecera, pero no hay más pantallas ni navegación |
| Botones con funcionalidad | ❌ `MainWindow.xaml.cs` está vacío: ningún botón hace nada |
| Animaciones (con mesura) | ❌ Ninguna (solo un cambio de opacidad al pasar el ratón) |
| Usabilidad/accesibilidad | ⚠️ Foco de teclado no visible, `ResizeMode="NoResize"`, contraste del acento `#D9A441` sobre blanco bajo |
| Documentación técnica + uso de IA documentado | ❌ No existe |
| Enlaces públicos (repo, prototipo, recursos) | ❌ Pendiente |

Otros problemas técnicos: el `ScrollViewer` usa `Margin="10,42,-10,-42"` (parche que descoloca el contenido), `Image1.png` no se usa, la ventana no tiene icono, y las carpetas `bin/`, `obj/` y `.vs/` no deben entregarse (el enunciado avisa de que archivos detectados como virus no se corrigen: entrega solo código fuente).

> **Fecha límite: 11 de noviembre.** Prioridad: 1) app funcionando, 2) documentación + IA, 3) pulido.

**Decisión de arquitectura:** una `MainWindow` con la cabecera fija y un `Frame` que carga tres `Page` (navegación). Así logo y menú son comunes por construcción.

```
MainWindow  (cabecera común + Frame)
 ├─ InicioPage
 ├─ LoginPage
 └─ GaleriaPage
App.xaml    (colores y estilos compartidos)
```

---

## 1. Crear el proyecto

1. Visual Studio → *Nuevo proyecto* → **Aplicación WPF** (C#) → nombre `Polestar` (puedes reutilizar el existente borrándole el contenido de `MainWindow.xaml`).
2. Para abrirlo en Blend: clic derecho en el proyecto → *Diseño en Blend*.
3. Añade las páginas: clic derecho en el proyecto → *Agregar → Página (WPF)* y crea `InicioPage.xaml`, `LoginPage.xaml`, `GaleriaPage.xaml`.
4. Crea la carpeta `Imagenes/` y copia ahí tus imágenes (propiedad *Acción de compilación = Resource*). Anota la procedencia y licencia de cada una (se pide en la documentación).

---

## 2. Estilos globales: `App.xaml`

```xml
<Application x:Class="Polestar.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <SolidColorBrush x:Key="Negro" Color="#111111"/>
        <SolidColorBrush x:Key="GrisClaro" Color="#F2F2F2"/>
        <SolidColorBrush x:Key="GrisTexto" Color="#595959"/>   <!-- contraste >= 4.5:1 sobre blanco -->
        <SolidColorBrush x:Key="Acento" Color="#D9A441"/>
        <SolidColorBrush x:Key="Error" Color="#B00020"/>

        <!-- Botón primario -->
        <Style x:Key="BotonPrimario" TargetType="Button">
            <Setter Property="Background" Value="{StaticResource Negro}"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderBrush" Value="{StaticResource Negro}"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="Padding" Value="28,12"/>
            <Setter Property="FontSize" Value="14"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border x:Name="fondo" Background="{TemplateBinding Background}"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                BorderThickness="{TemplateBinding BorderThickness}"
                                Padding="{TemplateBinding Padding}">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="fondo" Property="Opacity" Value="0.8"/>
                            </Trigger>
                            <!-- Foco visible con teclado (accesibilidad) -->
                            <Trigger Property="IsKeyboardFocused" Value="True">
                                <Setter TargetName="fondo" Property="BorderBrush" Value="{StaticResource Acento}"/>
                                <Setter TargetName="fondo" Property="BorderThickness" Value="2"/>
                            </Trigger>
                            <Trigger Property="IsEnabled" Value="False">
                                <Setter TargetName="fondo" Property="Opacity" Value="0.4"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="BotonSecundario" TargetType="Button" BasedOn="{StaticResource BotonPrimario}">
            <Setter Property="Background" Value="Transparent"/>
            <Setter Property="Foreground" Value="{StaticResource Negro}"/>
        </Style>

        <!-- Opción del menú: RadioButton que se ve como pestaña con subrayado cuando está activa -->
        <Style x:Key="OpcionMenu" TargetType="RadioButton">
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="FontSize" Value="15"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="Margin" Value="6,0"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="RadioButton">
                        <Border x:Name="raiz" Padding="16,8" BorderThickness="0,0,0,3"
                                BorderBrush="Transparent" Background="Transparent">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsChecked" Value="True">
                                <Setter TargetName="raiz" Property="BorderBrush" Value="{StaticResource Acento}"/>
                            </Trigger>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="raiz" Property="Background" Value="#22FFFFFF"/>
                            </Trigger>
                            <Trigger Property="IsKeyboardFocused" Value="True">
                                <Setter TargetName="raiz" Property="Background" Value="#44FFFFFF"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <!-- Tarjeta con animación suave al pasar el ratón -->
        <Style x:Key="Tarjeta" TargetType="Border">
            <Setter Property="Background" Value="{StaticResource GrisClaro}"/>
            <Setter Property="Padding" Value="24"/>
            <Setter Property="Margin" Value="10"/>
            <Setter Property="RenderTransformOrigin" Value="0.5,0.5"/>
            <Setter Property="RenderTransform">
                <Setter.Value><ScaleTransform ScaleX="1" ScaleY="1"/></Setter.Value>
            </Setter>
            <Style.Triggers>
                <EventTrigger RoutedEvent="MouseEnter">
                    <BeginStoryboard>
                        <Storyboard>
                            <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleX" To="1.03" Duration="0:0:0.15"/>
                            <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleY" To="1.03" Duration="0:0:0.15"/>
                        </Storyboard>
                    </BeginStoryboard>
                </EventTrigger>
                <EventTrigger RoutedEvent="MouseLeave">
                    <BeginStoryboard>
                        <Storyboard>
                            <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleX" To="1" Duration="0:0:0.15"/>
                            <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleY" To="1" Duration="0:0:0.15"/>
                        </Storyboard>
                    </BeginStoryboard>
                </EventTrigger>
            </Style.Triggers>
        </Style>
    </Application.Resources>
</Application>
```

---

## 3. Ventana principal con elementos comunes: `MainWindow.xaml`

Logo + menú viven aquí, así aparecen en las 3 pantallas. Se quita `NoResize` (mejor usabilidad) y se fija un tamaño mínimo.

```xml
<Window x:Class="Polestar.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Polestar España" Height="720" Width="1280" MinWidth="1000" MinHeight="600"
        WindowStartupLocation="CenterScreen" Background="White">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="70"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- CABECERA COMÚN -->
        <Border Grid.Row="0" Background="{StaticResource Negro}" Padding="40,0">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>

                <!-- Logotipo (clic = ir a Inicio) -->
                <StackPanel Grid.Column="0" Orientation="Horizontal" VerticalAlignment="Center"
                            HorizontalAlignment="Left" Cursor="Hand" MouseLeftButtonUp="Logo_Click"
                            ToolTip="Ir a Inicio">
                    <!-- Si tienes logo propio: <Image Source="Imagenes/logo.png" Height="28"/> -->
                    <TextBlock Text="POLESTAR" Foreground="White" FontSize="24" FontWeight="Bold"/>
                    <TextBlock Text="ESPAÑA" Foreground="{StaticResource Acento}" FontSize="12"
                               VerticalAlignment="Bottom" Margin="8,0,0,4"/>
                </StackPanel>

                <!-- MENÚ de navegación -->
                <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                    <RadioButton x:Name="RbInicio" GroupName="menu" Content="Inicio"
                                 Style="{StaticResource OpcionMenu}" Checked="Menu_Checked"/>
                    <RadioButton x:Name="RbGaleria" GroupName="menu" Content="Galería"
                                 Style="{StaticResource OpcionMenu}" Checked="Menu_Checked"/>
                </StackPanel>

                <RadioButton x:Name="RbLogin" Grid.Column="2" GroupName="menu"
                             HorizontalAlignment="Right" Style="{StaticResource OpcionMenu}"
                             FontFamily="Segoe MDL2 Assets" FontSize="20" Content="&#xE77B;"
                             ToolTip="My Polestar Space (iniciar sesión)"
                             AutomationProperties.Name="Iniciar sesión" Checked="Menu_Checked"/>
            </Grid>
        </Border>

        <!-- CONTENIDO: aquí se cargan las páginas -->
        <Frame x:Name="MarcoPrincipal" Grid.Row="1" NavigationUIVisibility="Hidden"
               Navigated="MarcoPrincipal_Navigated"/>
    </Grid>
</Window>
```

### `MainWindow.xaml.cs`

```csharp
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Polestar
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RbInicio.IsChecked = true;          // dispara Menu_Checked -> carga Inicio
        }

        private void Menu_Checked(object sender, RoutedEventArgs e)
        {
            if (MarcoPrincipal == null) return;  // se dispara durante la carga

            if (sender == RbInicio) MarcoPrincipal.Navigate(new InicioPage());
            else if (sender == RbGaleria) MarcoPrincipal.Navigate(new GaleriaPage());
            else if (sender == RbLogin) MarcoPrincipal.Navigate(new LoginPage());
        }

        // Método público para que las páginas puedan navegar y el menú quede sincronizado
        public void IrA(string destino)
        {
            switch (destino)
            {
                case "inicio": RbInicio.IsChecked = true; break;
                case "galeria": RbGaleria.IsChecked = true; break;
                case "login": RbLogin.IsChecked = true; break;
            }
        }

        private void Logo_Click(object sender, MouseButtonEventArgs e) => IrA("inicio");

        // Animación de transición: fundido de 250 ms (sutil, no distrae)
        private void MarcoPrincipal_Navigated(object sender, NavigationEventArgs e)
        {
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
            MarcoPrincipal.BeginAnimation(OpacityProperty, fade);
        }
    }
}
```

---

## 4. Pantalla de Inicio: `InicioPage.xaml`

Aprovecha lo bueno del borrador, sin el margen negativo y con botones conectados. Datos de ejemplo: **sustituye los precios por valores reales (o claramente ficticios) y cita la fuente**.

```xml
<Page x:Class="Polestar.InicioPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      Title="Inicio">
    <ScrollViewer VerticalScrollBarVisibility="Auto">
        <StackPanel>

            <!-- HERO / campaña principal -->
            <Grid Height="420">
                <Grid.Background>
                    <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                        <GradientStop Color="#111111" Offset="0"/>
                        <GradientStop Color="#3A3A3A" Offset="1"/>
                    </LinearGradientBrush>
                </Grid.Background>
                <StackPanel Margin="80,0,0,0" VerticalAlignment="Center" HorizontalAlignment="Left" Width="520">
                    <TextBlock Text="NOVEDAD" Foreground="{StaticResource Acento}" FontWeight="Bold"/>
                    <TextBlock Text="Polestar 4" Foreground="White" FontSize="64" FontWeight="Light"/>
                    <TextBlock Text="Diseño escandinavo y prestaciones eléctricas, ya disponible en España."
                               Foreground="#DDDDDD" FontSize="18" TextWrapping="Wrap" Margin="0,8,0,28"/>
                    <Button Content="Descubrir" Click="IrGaleria_Click" HorizontalAlignment="Left"
                            Style="{StaticResource BotonPrimario}" Background="White"
                            Foreground="{StaticResource Negro}"/>
                </StackPanel>
                <!-- Imagen del modelo (pon tu archivo en Imagenes/) -->
                <Image Source="/Imagenes/polestar4.png" HorizontalAlignment="Right" Width="520"
                       Margin="0,40,60,40" Stretch="Uniform"
                       AutomationProperties.Name="Polestar 4"/>
            </Grid>

            <!-- OFERTAS -->
            <StackPanel Margin="60,40,60,10">
                <TextBlock Text="Ofertas y novedades" FontSize="30" FontWeight="SemiBold" Margin="10,0,0,16"/>
                <UniformGrid Columns="3">
                    <Border Style="{StaticResource Tarjeta}">
                        <StackPanel>
                            <TextBlock Text="MODELO TOP" Foreground="#8A6212" FontWeight="Bold" FontSize="12"/>
                            <TextBlock Text="Polestar 3" FontSize="26" FontWeight="SemiBold" Margin="0,8,0,0"/>
                            <TextBlock Text="Nuestro SUV eléctrico más completo." TextWrapping="Wrap"
                                       Foreground="{StaticResource GrisTexto}" Margin="0,8,0,0" Height="50"/>
                            <TextBlock Text="Desde 79.900 €" FontSize="20" FontWeight="Bold" Margin="0,8,0,16"/>
                            <Button Content="Ver en galería" Click="IrGaleria_Click"
                                    Style="{StaticResource BotonPrimario}" HorizontalAlignment="Left"/>
                        </StackPanel>
                    </Border>
                    <Border Style="{StaticResource Tarjeta}">
                        <StackPanel>
                            <TextBlock Text="OFERTA DE TEMPORADA" Foreground="#8A6212" FontWeight="Bold" FontSize="12"/>
                            <TextBlock Text="Campaña de otoño" FontSize="26" FontWeight="SemiBold" Margin="0,8,0,0"/>
                            <TextBlock Text="Ventajas exclusivas al reservar tu Polestar esta temporada."
                                       TextWrapping="Wrap" Foreground="{StaticResource GrisTexto}"
                                       Margin="0,8,0,0" Height="50"/>
                            <TextBlock Text="Hasta -3.000 €" FontSize="20" FontWeight="Bold" Margin="0,8,0,16"/>
                            <Button Content="Ver en galería" Click="IrGaleria_Click"
                                    Style="{StaticResource BotonPrimario}" HorizontalAlignment="Left"/>
                        </StackPanel>
                    </Border>
                    <Border Style="{StaticResource Tarjeta}">
                        <StackPanel>
                            <TextBlock Text="MEJOR PRECIO" Foreground="#8A6212" FontWeight="Bold" FontSize="12"/>
                            <TextBlock Text="Polestar 2" FontSize="26" FontWeight="SemiBold" Margin="0,8,0,0"/>
                            <TextBlock Text="La forma más accesible de entrar en el mundo Polestar."
                                       TextWrapping="Wrap" Foreground="{StaticResource GrisTexto}"
                                       Margin="0,8,0,0" Height="50"/>
                            <TextBlock Text="Desde 44.900 €" FontSize="20" FontWeight="Bold" Margin="0,8,0,16"/>
                            <Button Content="Ver en galería" Click="IrGaleria_Click"
                                    Style="{StaticResource BotonPrimario}" HorizontalAlignment="Left"/>
                        </StackPanel>
                    </Border>
                </UniformGrid>
            </StackPanel>

            <!-- ¿Por qué Polestar? -->
            <Border Background="{StaticResource GrisClaro}" Padding="70,40" Margin="0,30,0,0">
                <StackPanel>
                    <TextBlock Text="¿Por qué elegir Polestar?" FontSize="28" FontWeight="SemiBold"
                               HorizontalAlignment="Center" Margin="0,0,0,24"/>
                    <UniformGrid Columns="3">
                        <StackPanel Margin="20,0">
                            <TextBlock Text="Diseño" FontSize="20" FontWeight="SemiBold" HorizontalAlignment="Center"/>
                            <TextBlock Text="Líneas limpias de inspiración escandinava." TextWrapping="Wrap"
                                       TextAlignment="Center" Foreground="{StaticResource GrisTexto}" Margin="0,8,0,0"/>
                        </StackPanel>
                        <StackPanel Margin="20,0">
                            <TextBlock Text="100 % eléctrico" FontSize="20" FontWeight="SemiBold" HorizontalAlignment="Center"/>
                            <TextBlock Text="Prestaciones y autonomía para el día a día." TextWrapping="Wrap"
                                       TextAlignment="Center" Foreground="{StaticResource GrisTexto}" Margin="0,8,0,0"/>
                        </StackPanel>
                        <StackPanel Margin="20,0">
                            <TextBlock Text="Servicio" FontSize="20" FontWeight="SemiBold" HorizontalAlignment="Center"/>
                            <TextBlock Text="Atención cercana antes y después de la compra." TextWrapping="Wrap"
                                       TextAlignment="Center" Foreground="{StaticResource GrisTexto}" Margin="0,8,0,0"/>
                        </StackPanel>
                    </UniformGrid>
                </StackPanel>
            </Border>

            <!-- Pie -->
            <Border Background="{StaticResource Negro}" Padding="70,30" Margin="0,40,0,0">
                <UniformGrid Columns="3">
                    <StackPanel>
                        <TextBlock Text="POLESTAR ESPAÑA" Foreground="White" FontWeight="Bold" FontSize="16"/>
                        <TextBlock Text="Aviso legal · Privacidad · Cookies" Foreground="#BBBBBB" Margin="0,10,0,0"/>
                    </StackPanel>
                    <StackPanel>
                        <TextBlock Text="Contacto" Foreground="White" FontWeight="Bold" FontSize="16"/>
                        <TextBlock Text="info@polestar.example" Foreground="#BBBBBB" Margin="0,10,0,0"/>
                        <TextBlock Text="+34 900 000 000" Foreground="#BBBBBB" Margin="0,4,0,0"/>
                    </StackPanel>
                    <StackPanel>
                        <TextBlock Text="Redes y soporte" Foreground="White" FontWeight="Bold" FontSize="16"/>
                        <TextBlock Text="Instagram · X · YouTube" Foreground="#BBBBBB" Margin="0,10,0,0"/>
                        <TextBlock Text="Centro de ayuda" Foreground="#BBBBBB" Margin="0,4,0,0"/>
                    </StackPanel>
                </UniformGrid>
            </Border>
        </StackPanel>
    </ScrollViewer>
</Page>
```

### `InicioPage.xaml.cs`

```csharp
using System.Windows;
using System.Windows.Controls;

namespace Polestar
{
    public partial class InicioPage : Page
    {
        public InicioPage() { InitializeComponent(); }

        private void IrGaleria_Click(object sender, RoutedEventArgs e)
            => ((MainWindow)Application.Current.MainWindow).IrA("galeria");
    }
}
```

> Si aún no tienes `polestar4.png`, comenta la etiqueta `<Image>` o usa temporalmente `Image1.png` del borrador (si lo conservas). Una imagen que falta en tiempo de ejecución rompe la ventana.

---

## 5. Pantalla de Login: `LoginPage.xaml`

Requisitos: `PasswordBox` (texto oculto) y gestión de errores sencilla pero efectiva: campos vacíos, credenciales erróneas con mensaje claro (sin revelar cuál de los dos falla), contador de intentos y bloqueo temporal tras 3 fallos.

```xml
<Page x:Class="Polestar.LoginPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      Title="Login">
    <Grid Background="{StaticResource GrisClaro}">
        <Border x:Name="TarjetaLogin" Background="White" Width="420" Padding="40"
                VerticalAlignment="Center" HorizontalAlignment="Center"
                RenderTransformOrigin="0.5,0.5">
            <Border.RenderTransform><TranslateTransform x:Name="Desplazamiento"/></Border.RenderTransform>

            <!-- Formulario -->
            <StackPanel x:Name="PanelFormulario">
                <TextBlock Text="My Polestar Space" FontSize="28" FontWeight="SemiBold"/>
                <TextBlock Text="Accede a tu cuenta de cliente" Foreground="{StaticResource GrisTexto}"
                           Margin="0,4,0,24"/>

                <Label Content="Usuario" Target="{Binding ElementName=TxtUsuario}" Padding="0,0,0,4"/>
                <TextBox x:Name="TxtUsuario" Height="36" FontSize="15" Padding="6,0"
                         VerticalContentAlignment="Center" TabIndex="0"/>

                <Label Content="Contraseña" Target="{Binding ElementName=PwdClave}" Padding="0,16,0,4"/>
                <PasswordBox x:Name="PwdClave" Height="36" FontSize="15" Padding="6,0"
                             VerticalContentAlignment="Center" TabIndex="1" KeyDown="Pwd_KeyDown"/>

                <!-- Mensaje de error (oculto por defecto) -->
                <TextBlock x:Name="TxtError" Foreground="{StaticResource Error}" TextWrapping="Wrap"
                           Margin="0,12,0,0" Visibility="Collapsed" FontWeight="SemiBold"/>

                <Button x:Name="BtnEntrar" Content="Entrar" Click="BtnEntrar_Click" IsDefault="True"
                        Style="{StaticResource BotonPrimario}" Margin="0,24,0,0" TabIndex="2"/>

                <TextBlock Margin="0,16,0,0" Foreground="{StaticResource GrisTexto}" FontSize="12"
                           Text="Demo: usuario «cliente» / contraseña «polestar»"/>
            </StackPanel>
        </Border>
    </Grid>
</Page>
```

### `LoginPage.xaml.cs`

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Polestar
{
    public partial class LoginPage : Page
    {
        private const string UsuarioOk = "cliente";
        private const string ClaveOk = "polestar";   // prototipo: en producción nunca se guardan claves así
        private int intentos = 0;

        public LoginPage()
        {
            InitializeComponent();
            Loaded += (s, e) => TxtUsuario.Focus();   // foco inicial en el primer campo
        }

        private void Pwd_KeyDown(object sender, KeyEventArgs e) { /* IsDefault ya gestiona Enter */ }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Visibility = Visibility.Collapsed;

            // 1) Validación de campos vacíos
            if (string.IsNullOrWhiteSpace(TxtUsuario.Text))
            {
                MostrarError("Introduce tu usuario.");
                TxtUsuario.Focus();
                return;
            }
            if (PwdClave.Password.Length == 0)
            {
                MostrarError("Introduce tu contraseña.");
                PwdClave.Focus();
                return;
            }

            // 2) Comprobación de credenciales
            if (TxtUsuario.Text.Trim() == UsuarioOk && PwdClave.Password == ClaveOk)
            {
                MostrarBienvenida();
                return;
            }

            // 3) Error de identificación
            intentos++;
            PwdClave.Clear();
            PwdClave.Focus();
            Sacudir();

            if (intentos >= 3)
            {
                BloquearTemporalmente(15);
            }
            else
            {
                MostrarError($"Usuario o contraseña incorrectos. Intento {intentos} de 3.");
            }
        }

        private void MostrarError(string texto)
        {
            TxtError.Text = texto;
            TxtError.Visibility = Visibility.Visible;
        }

        private void BloquearTemporalmente(int segundos)
        {
            BtnEntrar.IsEnabled = TxtUsuario.IsEnabled = PwdClave.IsEnabled = false;
            int restante = segundos;
            MostrarError($"Demasiados intentos. Espera {restante} s para volver a intentarlo.");

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) =>
            {
                restante--;
                if (restante > 0)
                {
                    MostrarError($"Demasiados intentos. Espera {restante} s para volver a intentarlo.");
                    return;
                }
                timer.Stop();
                intentos = 0;
                BtnEntrar.IsEnabled = TxtUsuario.IsEnabled = PwdClave.IsEnabled = true;
                TxtError.Visibility = Visibility.Collapsed;
                TxtUsuario.Focus();
            };
            timer.Start();
        }

        // Animación de "sacudida" breve para dar feedback del error
        private void Sacudir()
        {
            var anim = new DoubleAnimationUsingKeyFrames { Duration = TimeSpan.FromMilliseconds(300) };
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(-10, TimeSpan.FromMilliseconds(50)));
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(10, TimeSpan.FromMilliseconds(150)));
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(-6, TimeSpan.FromMilliseconds(230)));
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(300)));
            Desplazamiento.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, anim);
        }

        private void MostrarBienvenida()
        {
            PanelFormulario.Children.Clear();
            PanelFormulario.Children.Add(new TextBlock
            {
                Text = "¡Bienvenido a My Polestar Space!",
                FontSize = 24, FontWeight = FontWeights.SemiBold, TextWrapping = TextWrapping.Wrap
            });
            var btn = new Button
            {
                Content = "Ver la galería",
                Style = (Style)FindResource("BotonPrimario"),
                Margin = new Thickness(0, 24, 0, 0)
            };
            btn.Click += (s, e) => ((MainWindow)Application.Current.MainWindow).IrA("galeria");
            PanelFormulario.Children.Add(btn);
        }
    }
}
```

---

## 6. Galería de productos: `GaleriaPage.xaml`

Usa un modelo de datos + `ItemsControl` con `DataTemplate` (más profesional que copiar tarjetas) y un filtro por tipo.

### Modelo — añade la clase `Coche.cs`

```csharp
namespace Polestar
{
    public class Coche
    {
        public string Nombre { get; set; } = "";
        public string Tipo { get; set; } = "";       // Berlina, SUV, Coupé SUV...
        public string Descripcion { get; set; } = "";
        public string Precio { get; set; } = "";
        public string Imagen { get; set; } = "";      // ruta relativa en /Imagenes
    }
}
```

### `GaleriaPage.xaml`

```xml
<Page x:Class="Polestar.GaleriaPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      Title="Galería">
    <Grid Margin="60,30,60,0">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- Barra de filtro -->
        <DockPanel Grid.Row="0" Margin="0,0,0,16">
            <TextBlock Text="Galería de modelos" FontSize="30" FontWeight="SemiBold" DockPanel.Dock="Left"/>
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" VerticalAlignment="Center">
                <Label Content="Filtrar por tipo:" Target="{Binding ElementName=CmbTipo}" VerticalAlignment="Center"/>
                <ComboBox x:Name="CmbTipo" Width="160" Height="30" SelectionChanged="CmbTipo_SelectionChanged"/>
                <TextBlock x:Name="TxtContador" Margin="16,0,0,0" VerticalAlignment="Center"
                           Foreground="{StaticResource GrisTexto}"/>
            </StackPanel>
        </DockPanel>

        <ScrollViewer Grid.Row="1" VerticalScrollBarVisibility="Auto">
            <ItemsControl x:Name="ListaCoches">
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate><WrapPanel/></ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <Border Style="{StaticResource Tarjeta}" Width="340">
                            <StackPanel>
                                <Border Height="160" Background="#DDDDDD">
                                    <Image Source="{Binding Imagen}" Stretch="Uniform"
                                           AutomationProperties.Name="{Binding Nombre}"/>
                                </Border>
                                <TextBlock Text="{Binding Tipo}" Foreground="#8A6212" FontWeight="Bold"
                                           FontSize="12" Margin="0,12,0,0"/>
                                <TextBlock Text="{Binding Nombre}" FontSize="24" FontWeight="SemiBold"/>
                                <TextBlock Text="{Binding Descripcion}" TextWrapping="Wrap" Height="44"
                                           Foreground="{StaticResource GrisTexto}" Margin="0,6,0,0"/>
                                <TextBlock Text="{Binding Precio}" FontSize="18" FontWeight="Bold" Margin="0,8,0,0"/>
                            </StackPanel>
                        </Border>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>
        </ScrollViewer>
    </Grid>
</Page>
```

### `GaleriaPage.xaml.cs`

```csharp
using System.Linq;
using System.Windows.Controls;

namespace Polestar
{
    public partial class GaleriaPage : Page
    {
        private readonly List<Coche> todos = new()
        {
            new Coche { Nombre = "Polestar 2", Tipo = "Berlina", Precio = "Desde 44.900 €",
                        Descripcion = "Fastback eléctrico, el más accesible.", Imagen = "/Imagenes/polestar2.png" },
            new Coche { Nombre = "Polestar 3", Tipo = "SUV", Precio = "Desde 79.900 €",
                        Descripcion = "SUV eléctrico de altas prestaciones.", Imagen = "/Imagenes/polestar3.png" },
            new Coche { Nombre = "Polestar 4", Tipo = "SUV Coupé", Precio = "Desde 62.900 €",
                        Descripcion = "SUV coupé sin luneta trasera.", Imagen = "/Imagenes/polestar4.png" },
            new Coche { Nombre = "Polestar 5", Tipo = "Gran Turismo", Precio = "Consultar",
                        Descripcion = "Gran turismo de cuatro puertas.", Imagen = "/Imagenes/polestar5.png" },
        };

        public GaleriaPage()
        {
            InitializeComponent();
            CmbTipo.ItemsSource = new[] { "Todos" }.Concat(todos.Select(c => c.Tipo).Distinct()).ToList();
            CmbTipo.SelectedIndex = 0;   // dispara SelectionChanged y rellena la lista
        }

        private void CmbTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipo = (string)CmbTipo.SelectedItem;
            var filtrados = tipo == "Todos" ? todos : todos.Where(c => c.Tipo == tipo).ToList();
            ListaCoches.ItemsSource = filtrados;
            TxtContador.Text = $"{filtrados.Count} modelo(s)";
        }
    }
}
```

> Los precios y datos de arriba son orientativos: **verifícalos en la web oficial y cita la fuente**, o márcalos como "datos ficticios de prototipo". Las imágenes de Polestar tienen copyright: usa las de prensa oficial indicando la procedencia, o imágenes con licencia libre (Wikimedia Commons), y regístralo en la documentación.

---

## 7. Probar (y anotar para la documentación)

Ejecuta con **F5** y comprueba:

- [ ] Logo y menú visibles en las 3 pantallas; el logo vuelve a Inicio.
- [ ] La opción activa del menú se subraya; "Descubrir" y "Ver en galería" también la actualizan.
- [ ] Navegación solo con teclado (Tab, Espacio/Enter): foco visible en todo.
- [ ] Login: campos vacíos, clave incorrecta ×3 (sacudida + bloqueo 15 s), clave correcta (bienvenida). La contraseña se muestra como puntos.
- [ ] Galería: el filtro cambia la lista y el contador.
- [ ] Redimensionar la ventana no rompe el diseño.
- [ ] Animaciones: fundido entre pantallas (250 ms), zoom de tarjeta (150 ms), sacudida de error (300 ms). Ninguna es continua ni bloquea la tarea.

Apunta los problemas que encuentres y cómo los arreglaste: **el enunciado exige esa sección**.

---

## 8. Documentación y entrega (obligatorio en el enunciado)

Redacta un PDF/MD con estos apartados:

1. **Interpretación de requisitos** y decisiones tomadas.
2. **Arquitectura**: diagrama `MainWindow → Frame → 3 Page`, flujo de navegación, función de cada pantalla.
3. **Justificación de diseño** con principios: consistencia (cabecera común, estilos en `App.xaml`), jerarquía visual, feedback (hover, foco, sacudida), prevención de errores (campos obligatorios, bloqueo), accesibilidad (contraste, foco visible, `Label Target`, `AutomationProperties.Name`, navegación por teclado), diseño centrado en el usuario.
4. **Pruebas** (checklist del punto 7), problemas y mejoras.
5. **Recursos**: imágenes, tipografías (Segoe UI / Segoe MDL2 Assets de Windows), bibliotecas (solo .NET/WPF), con procedencia y licencia.
6. **Uso de IA** (el borrador lo generó una IA, así que esto es obligatorio): herramienta y versión/fecha, objetivo, prompts relevantes y respuestas, qué modificaste, la revisión humana (correctitud, usabilidad, accesibilidad, seguridad, sesgos, propiedad intelectual) y tu contribución real frente a la de la IA, justificando qué aceptaste/modificaste/descartaste. Incluye también esta revisión y tutorial como material generado por IA. No pegues en la IA datos personales ni credenciales.
7. **Enlaces públicos sin credenciales**: repositorio (GitHub), prototipo (capturas/vídeo o enlace) y recursos.

**Empaquetado:** comprime solo el código fuente (`.csproj`, `.xaml`, `.cs`, `Imagenes/`, documentación). **Excluye `bin/`, `obj/` y `.vs/`**. Entrega por la plataforma virtual **antes del 11 de noviembre**.

## 9. Extras opcionales (justifícalos si los añades)

- Casilla "Mostrar contraseña".
- Botón "Cerrar sesión" y nombre de usuario en la cabecera tras iniciar sesión.
- Pantalla de detalle de modelo al hacer clic en una tarjeta.
- Comparador de modelos.
