# Tutorial 2: Prototipo Polestar con Blend (diseño gráfico, casi sin código)

Objetivo: construir las 3 pantallas (Inicio, Login, Galería) **dibujando y configurando en los paneles de Blend**. El código escrito a mano se reduce a ~10 líneas opcionales (validación del login).

> Blend genera XAML por detrás de lo que haces. Eso es lo que entregarás, pero no tienes que escribirlo.

## 0. Paneles de Blend que vas a usar

| Panel | Para qué |
|---|---|
| **Assets** (Activos) | Arrastrar controles (Button, TabControl, PasswordBox...), estilos, *Behaviors* |
| **Objects and Timeline** | Árbol de elementos, renombrar, mostrar/ocultar, crear Storyboards |
| **Properties** | Color, tamaño, fuente, márgenes, bordes, esquinas |
| **Resources** | Guardar colores y estilos reutilizables |
| **Triggers / Behaviors** | Eventos → acciones (cambiar propiedad, lanzar animación) sin código |
| **Design surface** | El lienzo donde arrastras |

Abrir el proyecto: Visual Studio → clic derecho en el proyecto → *Diseño en Blend* (o abre el `.slnx` directamente en Blend).

---

## 1. Crear el proyecto

1. Nuevo proyecto → **Aplicación WPF** → `Polestar`.
2. Abre `MainWindow.xaml` en Blend.
3. Panel *Properties* de la ventana: `Title = Polestar España`, `Width = 1280`, `Height = 720`, `MinWidth = 1000`, `MinHeight = 600`.
4. *Project panel* → botón derecho → *Add → Existing Item* para importar imágenes (logo, coches). Anota procedencia y licencia de cada una.

## 2. Paleta y tipografía (Resources)

1. Panel Properties → *Brushes* → *Background* → color `#111111` → pulsa el cuadradito de la derecha → **Convert to New Resource** → nombre `Negro`, ubicación **Application** (`App.xaml`).
2. Repite para: `GrisClaro #F2F2F2`, `GrisTexto #595959`, `Acento #D9A441`, `Error #B00020`.
3. A partir de ahora **elige siempre colores desde la pestaña Resources del selector de color** (no escribas hex): cambiar un color lo cambia en toda la app (consistencia).

Tipografía: usa la fuente por defecto (Segoe UI) o una de Windows; anótalo en la documentación.

## 3. Estructura de la ventana (elementos comunes)

En *Objects and Timeline* deja esta jerarquía (renombra con doble clic):

```
Window
└─ Grid (raiz)
   ├─ Row 0 (70px)  Border "Cabecera"  (fondo Negro)
   │    ├─ Logo (StackPanel: TextBlock POLESTAR + TextBlock ESPAÑA, o Image)
   │    ├─ Menú (se construye en el paso 4)
   │    └─ Botón usuario
   └─ Row 1 (*)     TabControl "Pantallas"
```

Pasos:
1. Selecciona el Grid raíz → en el lienzo, haz clic en el borde izquierdo para crear **2 filas**; en Properties→Layout o arrastrando las guías: fila 0 = `70` px, fila 1 = `*`.
2. Assets → *Border* → dibújalo en la fila 0 (Margin 0, Alignment Stretch). Fondo = recurso `Negro`.
3. Dentro del Border, un *StackPanel* horizontal con dos *TextBlock*: «POLESTAR» (blanco, 24 pt, Bold) y «ESPAÑA» (recurso `Acento`, 12 pt, alineado abajo). Ese es el **logotipo** (elemento común nº 1).
4. Assets → *TabControl* → dibújalo en la fila 1, Margin 0. Se crean 2 pestañas por defecto: **clic derecho en el TabControl → Add TabItem** hasta tener 3. Renómbralas `TabInicio`, `TabLogin`, `TabGaleria` y pon los *Header* a «Inicio», «Login», «Galería».

## 4. Menú de navegación sin código: plantilla del TabControl

Truco: el TabControl ya sabe cambiar de pantalla al pulsar sus cabeceras. Solo hay que **recolocar y estilizar** las cabeceras para que parezcan el menú.

1. Clic derecho en el TabControl → **Edit Template → Edit a Copy…** → nombre `PlantillaNavegacion`, en Application.
2. En Objects and Timeline verás un `Grid` con `TabPanel` (cabeceras) y `ContentPresenter` (contenido).
   - Selecciona el `TabPanel` y en Layout ponlo en la **fila 0** con *Alignment* Center y fondo `Negro`. Así las pestañas quedan arriba.
   - Para que el **logo** también quede en esa franja, mueve la Border/Logo del paso 3 dentro de la misma fila (superpuestos: logo a la izquierda, TabPanel centrado). Alternativa más simple: deja el logo en la Border de la fila 0 del Grid raíz y ocupa esa fila con una **TabPanel centrada** copiada en la plantilla; lo importante es que logo y menú se vean igual en las 3 pantallas.
3. Vuelve al TabControl (flecha «Return scope») → clic derecho → **Edit Additional Templates → Edit Generated Item Container (ItemContainerStyle) → Edit a Copy** → `EstiloOpcionMenu`.
4. En este estilo (es el TabItem) edita su plantilla (*Edit Template*) y usa el **panel States**:
   - Estado base: texto `White`, fondo transparente, Padding `16,8`.
   - `MouseOver`: fondo blanco al 15 % de opacidad.
   - `Selected`: borde inferior de 3 px con recurso `Acento`.
   - `Focused` (teclado): borde `Acento` de 2 px (accesibilidad).
   Para cada estado: selecciónalo en el panel States, con **Recording on** (aparece el borde rojo) cambia la propiedad en Properties.
5. Con el TabControl seleccionado: *Properties → TabStripPlacement = Top*; fondo del cuerpo = `White`.

Resultado: pulsar «Inicio / Galería / Login» cambia de pantalla sin escribir nada. La pestaña activa queda subrayada.

**Botón de usuario (login)**: puedes dejar la pestaña «Login» en el menú, o ocultar su cabecera y poner un *Button* con icono en la esquina derecha de la cabecera:
- Propiedades del Button: *Text → Font Family = Segoe MDL2 Assets*, *Content* = pega el carácter del icono (`E77B`, persona).
- Behavior → **ChangePropertyAction** (ver §8) con `TargetObject = Pantallas`, `PropertyName = SelectedIndex`, `Value = 1`.

## 5. Pantalla INICIO (dentro de `TabInicio`)

Primero pon dentro del TabItem un **ScrollViewer** (Assets → ScrollViewer, Stretch) y dentro un **StackPanel** vertical.

1. **Hero**: *Grid* de alto 420; fondo → pestaña *Gradient brush* → 2 paradas `#111111` → `#3A3A3A`.
   - A la izquierda un StackPanel con TextBlocks: «NOVEDAD» (Acento), «Polestar 4» (64 pt, Light, blanco), descripción (18 pt, `TextWrapping Wrap`) y un Button «Descubrir».
   - A la derecha: Assets → *Image* → Source = foto del modelo (Properties → Common → Source).
2. **Ofertas**: TextBlock título (30 pt SemiBold) y debajo un **UniformGrid** (Assets, buscar «UniformGrid») con *Columns = 3*.
   - Dibuja **una** tarjeta: Border + StackPanel con 4 TextBlocks (etiqueta, nombre, descripción, precio) + Button.
   - Selecciona el Border de la tarjeta → clic derecho → **Make Into Style → Tarjeta**... mejor: **Edit Style → Create Empty**/*Make Into Style* con nombre `Tarjeta` (fondo `GrisClaro`, Padding 24, Margin 10).
   - Copia/pega la tarjeta 2 veces y cambia solo los textos: modelo top (Polestar 3), temporada (campaña de otoño), mejor precio (Polestar 2). Precios reales/verificados o marcados como ficticios.
   - Alternativa más pro: selecciona la tarjeta → clic derecho → **Make into UserControl** → `TarjetaOferta`; luego se arrastra desde Assets.
3. **¿Por qué Polestar?**: otro Border `GrisClaro` con UniformGrid de 3 columnas y 2 TextBlocks cada una.
4. **Pie**: Border `Negro` con UniformGrid 3 columnas: info legal, contacto, redes.
5. Estilo de botones: selecciona un Button → **Edit Template → Edit a Copy** → `BotonPrimario`. Con *States*: `MouseOver` (Opacity 0.8), `Focused` (borde Acento), `Disabled` (Opacity 0.4). Aplícalo a todos los botones con Properties → *Style* (o clic derecho → *Edit Style → Apply Resource*).
6. Para el botón secundario: *Edit Style → Edit a Copy* de `BotonPrimario` → `BotonSecundario` con fondo Transparent.

## 6. Pantalla LOGIN (`TabLogin`)

1. Fondo del TabItem = `GrisClaro`. Dentro, un **Border** blanco de 420 px de ancho, centrado (*HorizontalAlignment/VerticalAlignment = Center*), Padding 40. Nómbralo `TarjetaLogin`.
2. Dentro, un StackPanel:
   - TextBlock «My Polestar Space» (28 pt).
   - **Label** «Usuario» + **TextBox** `TxtUsuario`.
   - **Label** «Contraseña» + **PasswordBox** `PwdClave` (de Assets → *PasswordBox*; oculta el texto).
   - **TextBlock** `TxtError` (color `Error`, SemiBold, texto «Usuario o contraseña incorrectos») con *Visibility = Collapsed*.
   - **Button** «Entrar» `BtnEntrar` con *Properties → Common → IsDefault = True* (Enter lo activa).
3. Accesibilidad: en cada campo, Properties → *Accessibility → AutomationProperties.Name*; *TabIndex* 0,1,2.

### Gestión de errores — dos niveles

**A) Sin código (prototipo puro).** Behaviors (§8): en `BtnEntrar`, evento Click → **ChangePropertyAction** `TxtError.Visibility = Visible`. Así el prototipo siempre muestra el error. Es válido para un maquetado, pero no distingue credenciales correctas.

**B) Recomendado (10 líneas, el único código de la práctica).** Doble clic en el botón «Entrar» crea el evento Click; en `MainWindow.xaml.cs`:

```csharp
private void BtnEntrar_Click(object sender, RoutedEventArgs e)
{
    bool ok = TxtUsuario.Text == "cliente" && PwdClave.Password == "polestar";
    TxtError.Text = string.IsNullOrWhiteSpace(TxtUsuario.Text) || PwdClave.Password.Length == 0
        ? "Introduce usuario y contraseña."
        : "Usuario o contraseña incorrectos.";
    TxtError.Visibility = ok ? Visibility.Collapsed : Visibility.Visible;
    if (ok) Pantallas.SelectedIndex = 2;   // éxito: ir a la galería
}
```

(PasswordBox no se puede enlazar con datos por seguridad, por eso aquí hace falta una línea de código.)

**Animación de error (gráfica, ver §7):** sacudida de la tarjeta.

## 7. Pantalla GALERÍA (`TabGaleria`)

1. Título «Galería de modelos» y debajo un **ListBox** (Assets → ListBox) con *ItemsPanel = WrapPanel*: clic derecho en el ListBox → **Edit Additional Templates → Edit Layout of Items (ItemsPanel) → WrapPanel**.
2. Diseña la tarjeta de producto en un Border (imagen 160 px alto + tipo + nombre + descripción + precio). Copia la tarjeta 4 veces (Polestar 2, 3, 4, 5) cambiando imagen y textos, ya con el estilo `Tarjeta` del §5.
   - Más gráfico: selecciona la tarjeta → **Make into UserControl** → `TarjetaCoche`, y arrastra 4 instancias desde Assets.
3. Filtro (opcional): un ComboBox «Todos / Berlina / SUV». Sin código el filtrado no funciona; puedes dejarlo como elemento visual del prototipo y justificarlo, o usar una colección con datos de ejemplo (*Data panel → Create Sample Data*) si tu versión de Blend la ofrece.

## 8. Interactividad sin código: Behaviors

1. Visual Studio / Blend: *Project → Manage NuGet packages → instalar* **`Microsoft.Xaml.Behaviors.Wpf`**.
2. Assets → pestaña **Behaviors**: aparecen `ChangePropertyAction`, `ControlStoryboardAction`, `CallMethodAction`, `GoToStateAction`...
3. **Botones que navegan** (ej. «Descubrir», «Ver oferta»): arrastra **ChangePropertyAction** sobre el botón. En Properties:
   - *EventName* = `Click`
   - *TargetObject* = (binding de elemento) `Pantallas` (el TabControl)
   - *PropertyName* = `SelectedIndex`
   - *Value* = `2` (Galería).
   El menú sigue sincronizado porque la pestaña seleccionada cambia.
4. **Mostrar error:** como en §6.A.

## 9. Animaciones con el Timeline

Mantén pocas, cortas y con propósito:

| Animación | Duración | Dónde |
|---|---|---|
| Fundido al cambiar de pantalla | 0,25 s | Contenido del TabControl |
| Zoom suave de tarjeta al pasar el ratón | 0,15 s | Estilo `Tarjeta` |
| Sacudida del formulario al fallar login | 0,3 s | `TarjetaLogin` |

**Fundido de pantalla**
1. En Objects and Timeline pulsa **+** en «Storyboards» → nombre `Fundido`.
2. Selecciona la zona de contenido (en la plantilla del TabControl, el ContentPresenter, o cada ScrollViewer de pantalla).
3. Mueve el cabezal a `0:00.000`: Opacity = 0. Cabezal a `0:00.250`: Opacity = 100 %.
4. Cierra el storyboard. En el TabItem o el ScrollViewer: Behavior → **ControlStoryboardAction** con *EventName = Loaded*, *Storyboard = Fundido*, *ControlStoryboardOption = Play*.

**Zoom de tarjeta (con estados)**
1. Edita el estilo/plantilla de la tarjeta (*Edit Template* de un ContentControl o usa el UserControl `TarjetaOferta`) y en **States → MouseOver** añade *RenderTransform → Scale* X/Y = 1.03 con *transition duration* 0,15 s (botón de «Default transition» en el panel States).
2. Fija *RenderTransformOrigin = 0.5, 0.5*.

**Sacudida del error**
1. Storyboard `Sacudida`. Selecciona `TarjetaLogin`. Keyframes de *RenderTransform → Translate X*: 0 s = 0, 0,05 s = -10, 0,15 s = 10, 0,23 s = -6, 0,3 s = 0.
2. Lánzala con **ControlStoryboardAction** (*Play*) en el evento Click del botón Entrar (o, si usas código, desde el evento: `((Storyboard)FindResource("Sacudida")).Begin();`).

## 10. Probar y refinar (visual)

- **F5** para ejecutar. Comprueba: logo y menú iguales en las 3 pantallas; pestaña activa subrayada; foco con Tab visible; login con vacío / error / éxito; animaciones suaves.
- Redimensiona la ventana: ajusta *Alignment* y *Margin* en Properties si algo se descoloca.
- Usa la herramienta de zoom del lienzo y el modo *Design / Split* para revisar márgenes.
- Anota cada fallo y su arreglo para la sección de pruebas.

## 11. Documentación y entrega

Igual que en el enunciado (secciones: requisitos, arquitectura y flujo, justificación de usabilidad/accesibilidad/consistencia/feedback/errores, pruebas, recursos con licencia, **uso de IA documentado**, enlaces públicos al repo/prototipo/recursos).

Aprovecha que el trabajo es gráfico: incluye **capturas de los paneles** (Objects and Timeline, States, Storyboards, Behaviors) y de cada pantalla como evidencia.

Sobre la IA: este tutorial y la revisión del borrador son material generado por IA; indica herramienta, fecha, prompts, qué aceptaste/modificaste y tu aportación real. El borrador original ya provenía de otra IA: documéntalo también.

**Empaquetado:** código fuente + `Imagenes/` + documentación. **No incluyas `bin/`, `obj/`, `.vs/`.** Fecha límite: **11 de noviembre**.

## Notas de honestidad sobre este enfoque

- Los nombres exactos de menús de Blend cambian entre versiones; si algo no está donde indico, busca la opción con la misma palabra clave (*Edit Template*, *Make Into Style*, *States*, *Behaviors*).
- No he podido ejecutar Blend para verificar cada clic, así que prueba paso a paso y contrasta con el XAML que genera (pestaña *Code*).
- La validación real del login y el filtrado de la galería **requieren un poco de código**; el resto es 100 % gráfico.
