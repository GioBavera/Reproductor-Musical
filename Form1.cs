using MaterialSkin.Controls;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ReproductorFLAC;

// <summary>
/// Reproductor de musica FLAC para Windows usando NAudio
/// MaterialSkin para una IU "Moderna"
/// ATENCION: Presenta problemas en archivos FLAC de alta calidad (24-bit o más)
/// <summary>

public partial class Form1 : MaterialForm
{
    private IAudioPlayer? _audioPlayer;
    private string? _archivoActual;
    private bool _estaReproduciendo = false;
    private System.Windows.Forms.Timer? _timerProgreso;

    // Volumen persistente (variable estatica para mantener entre canciones)
    private static float _volumenPersistente = 0.5f; // 50% por defecto
    private int _duracionTotalSegundos = 0;

    // Estado del botón de muteo
    private bool _estaMuteado = false;
    private float _volumenAntesDelMute = 0.5f;

    private ToolTip _toolTip;

    // Lista de reproducción
    private List<string> _listaReproduccion = new List<string>();
    private int _indiceActual = -1;
    private bool _repetirCancion = false; // Modo repeticion
    private System.Windows.Forms.Timer? _timerShuffle; // Timer para efecto shuffle

    // Tamaño fijo del formulario
    private static readonly Size TAMANO_FORMULARIO = new Size(950, 800);

    public Form1()
    {
        InitializeComponent();

        // Configurar tema
        var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
        materialSkinManager.AddFormToManage(this);
        materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
        materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
            MaterialSkin.Primary.Blue700,
            MaterialSkin.Primary.Blue700,
            MaterialSkin.Primary.Blue700,
            MaterialSkin.Accent.Orange700,
            MaterialSkin.TextShade.BLACK
        );

        // Inicializar ToolTip
        _toolTip = new ToolTip
        {
            AutoPopDelay = 5000,
            InitialDelay = 500,
            ReshowDelay = 100,
            ShowAlways = true
        };

        // Inicializar timer de progreso
        _timerProgreso = new System.Windows.Forms.Timer { Interval = 100 };
        _timerProgreso.Tick += TimerProgreso_Tick;

        // Inicializar timer para efecto visual de shuffle
        _timerShuffle = new System.Windows.Forms.Timer { Interval = 100 };
        _timerShuffle.Tick += TimerShuffle_Tick;

        // Establecer volumen persistente en el slider
        Volumen.Value = (int)(_volumenPersistente * 100);

        // Cargar imagen por defecto para la portada
        CargarPortadaPorDefecto();

        // Inicializar labels de metadatos
        lblTitulo.Text = "Reproductor";
        lblArtistaAlbum.Text = "Abre un archivo para comenzar";
        materialFloatingActionButton1.Icon = Reproductor.WinForm.Properties.Resources.play;
    }

    private void CargarPortadaPorDefecto()
    {
        pictureBoxPortada.Image = Reproductor.WinForm.Properties.Resources.Image_not_found;
    }

    private void ExtraerPortada(string rutaArchivo)
    {
        try
        {
            using var file = TagLib.File.Create(rutaArchivo);

            if (file.Tag.Pictures.Length > 0)
            {
                var picture = file.Tag.Pictures[0];
                var imageData = picture.Data.Data;

                using var ms = new System.IO.MemoryStream(imageData);
                pictureBoxPortada.Image = System.Drawing.Image.FromStream(ms);
            }
            else
            {
                CargarPortadaPorDefecto();
            }
        }
        catch
        {
            CargarPortadaPorDefecto();
        }
    }

    private string FormatearTiempo(int segundos)
    {
        TimeSpan ts = TimeSpan.FromSeconds(segundos);
        if (ts.Hours > 0)
            return ts.ToString(@"h\:mm\:ss");
        return ts.ToString(@"mm\:ss");
    }

    private void TimerProgreso_Tick(object? sender, EventArgs e)
    {
        if (_audioPlayer != null && _estaReproduciendo)
        {
            int posicionActual = _audioPlayer.GetCurrentPosition();
            if (posicionActual <= materialProgressBar1.Maximum)
            {
                materialProgressBar1.Value = posicionActual;
                ActualizarTiempoDisplay(posicionActual);
            }

            // Detectar cuando termina la canción
            if (posicionActual >= materialProgressBar1.Maximum)
            {
                if (_repetirCancion)
                {
                    // Repetir la canción actual
                    ReiniciarCancionActual();
                }
                else
                {
                    // Pasar a la siguiente canción
                    ReproducirSiguiente();
                }
            }
        }
    }

    private void ReiniciarCancionActual()
    {
        if (_audioPlayer != null)
        {
            _audioPlayer.SetPosition(0);
            materialProgressBar1.Value = 0;
            ActualizarTiempoDisplay(0);
        }
    }

    private void ActualizarListaReproduccion()
    {
        listaReproduccion.Items.Clear();

        for (int i = 0; i < _listaReproduccion.Count; i++)
        {
            string nombreArchivo = System.IO.Path.GetFileNameWithoutExtension(_listaReproduccion[i]);
            string prefijo = (i == _indiceActual) ? "▶ " : "   ";
            listaReproduccion.Items.Add(prefijo + nombreArchivo);
        }

        // Mostrar la lista si hay archivos
        listaReproduccion.Visible = _listaReproduccion.Count > 0;

        // Seleccionar el ítem actual
        if (_indiceActual >= 0 && _indiceActual < listaReproduccion.Items.Count)
        {
            listaReproduccion.SelectedIndex = _indiceActual;
        }
    }

    private void listaReproduccion_DoubleClick(object? sender, EventArgs e)
    {
        if (listaReproduccion.SelectedIndex >= 0)
        {
            _indiceActual = listaReproduccion.SelectedIndex;
            ReproducirIndice(_indiceActual);
        }
    }

    private void listaReproduccion_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            int indice = listaReproduccion.IndexFromPoint(e.Location);

            if (indice >= 0)
            {
                listaReproduccion.SelectedIndex = indice;
                contextMenuLista.Show(listaReproduccion, e.Location);
            }
        }
    }

    private void eliminarCancionToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (listaReproduccion.SelectedIndex >= 0)
        {
            EliminarCancionDeLista(listaReproduccion.SelectedIndex);
        }
    }

    private void btnLimpiarLista_Click(object? sender, EventArgs e)
    {
        if (_listaReproduccion.Count == 0)
            return;

        // Detener reproducción actual
        _audioPlayer?.Stop();
        _audioPlayer?.Dispose();
        _audioPlayer = null;
        _estaReproduciendo = false;
        _timerProgreso?.Stop();

        // Limpiar todo
        _listaReproduccion.Clear();
        _indiceActual = -1;
        _archivoActual = null;

        // Resetear interfaz
        listaReproduccion.Items.Clear();
        listaReproduccion.Visible = false;
        lblTitulo.Text = "Reproductor";
        lblArtistaAlbum.Text = "Lista vacía";
        CargarPortadaPorDefecto();
        materialProgressBar1.Value = 0;
        ActualizarTiempoDisplay(0);
        materialFloatingActionButton1.Icon = Reproductor.WinForm.Properties.Resources.play;
    }

    public void EliminarCancionDeLista(int indice)
    {
        if (indice < 0 || indice >= _listaReproduccion.Count)
            return;

        // Si es la canción actual
        if (indice == _indiceActual)
        {
            // Detener reproducción
            _audioPlayer?.Stop();
            _estaReproduciendo = false;
            _timerProgreso?.Stop();
            materialFloatingActionButton1.Icon = Reproductor.WinForm.Properties.Resources.play;

            // Eliminar y ajustar índice
            _listaReproduccion.RemoveAt(indice);

            // Si quedan canciones, reproducir la siguiente
            if (_listaReproduccion.Count > 0)
            {
                if (_indiceActual >= _listaReproduccion.Count)
                    _indiceActual = 0;

                ReproducirIndice(_indiceActual);
            }
            else
            {
                _indiceActual = -1;
                lblTitulo.Text = "Reproductor";
                lblArtistaAlbum.Text = "Lista vacía";
                CargarPortadaPorDefecto();
            }
        }
        else
        {
            // Si no es la actual, simplemente eliminar
            _listaReproduccion.RemoveAt(indice);

            // Ajustar índice actual si es necesario
            if (indice < _indiceActual)
            {
                _indiceActual--;
            }
        }

        ActualizarListaReproduccion();
    }

    private void ReproducirSiguiente()
    {
        if (_listaReproduccion.Count == 0)
            return;

        _indiceActual++;

        if (_indiceActual >= _listaReproduccion.Count)
        {
            _indiceActual = 0; // Volver al principio
        }

        ReproducirIndice(_indiceActual);
    }

    private void ReproducirAnterior()
    {
        if (_listaReproduccion.Count == 0)
            return;

        _indiceActual--;

        if (_indiceActual < 0)
        {
            _indiceActual = _listaReproduccion.Count - 1; // Ir al final
        }

        ReproducirIndice(_indiceActual);
    }

    private void ReproducirIndice(int indice)
    {
        if (indice < 0 || indice >= _listaReproduccion.Count)
            return;

        _archivoActual = _listaReproduccion[indice];

        // Restablecer labels mientras carga
        lblTitulo.Text = "Cargando...";
        lblArtistaAlbum.Text = "";

        CargarArchivo(_archivoActual);

        // Extraer y mostrar portada
        ExtraerPortada(_archivoActual);

        // Actualizar ListBox
        ActualizarListaReproduccion();
    }

    private void ActualizarTiempoDisplay(int posicionActual)
    {
        lblTiempo.Text = $"{FormatearTiempo(posicionActual)}";
        lbltiempo2.Text = $"{FormatearTiempo(_duracionTotalSegundos)}";
    }

    private void materialProgressBar1_Click(object sender, EventArgs e)
    {
        // Click en barra de progreso para navegar
        if (_audioPlayer != null)
        {
            // Calcular posición basada en el click
            int mouseX = Control.MousePosition.X - materialProgressBar1.PointToScreen(Point.Empty).X;
            int newVal = (mouseX * materialProgressBar1.Maximum) / materialProgressBar1.Width;
            newVal = Math.Max(0, Math.Min(materialProgressBar1.Maximum, newVal));

            _audioPlayer.SetPosition(newVal);
            materialProgressBar1.Value = newVal;
            ActualizarTiempoDisplay(newVal);
        }
    }

    // Evento cuando cambia el slider de volumen
    private void Volumen_ValueChanged(object sender, EventArgs e)
    {
        // Guardar el volumen en la variable persistente
        _volumenPersistente = Volumen.Value / 100f;

        // Si estábamos muteados y el usuario mueve el slider, salir del estado muteado
        if (_estaMuteado && Volumen.Value > 0)
        {
            _estaMuteado = false;
            pictureBox7.Image = Reproductor.WinForm.Properties.Resources.volume_up;
            pictureBox7.BackColor = Color.Transparent;
            _volumenAntesDelMute = _volumenPersistente;
        }

        // Si el volumen es 0, actualizar al estado muteado
        if (Volumen.Value == 0 && !_estaMuteado)
        {
            _estaMuteado = true;
            pictureBox7.Image = Reproductor.WinForm.Properties.Resources.volume_off;
            pictureBox7.BackColor = Color.FromArgb(255, 128, 0); // Naranja
        }

        // Aplicar al reproductor si está activo
        if (_audioPlayer != null)
        {
            _audioPlayer.SetVolume(_volumenPersistente);
        }
    }

    public void AbrirArchivo()
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = "Archivos FLAC|*.flac|Todos los archivos|*.*";
        ofd.Title = "Seleccionar archivos FLAC";
        ofd.Multiselect = true;

        if (ofd.ShowDialog() == DialogResult.OK)
        {
            // Limpiar lista anterior y agregar nuevos archivos
            _listaReproduccion.Clear();
            _listaReproduccion.AddRange(ofd.FileNames);
            _indiceActual = 0;

            // Cargar el primer archivo
            _archivoActual = _listaReproduccion[_indiceActual];

            // Restablecer labels mientras carga
            lblTitulo.Text = "Cargando...";
            lblArtistaAlbum.Text = "";

            CargarArchivo(_archivoActual);

            // Extraer y mostrar portada
            ExtraerPortada(_archivoActual);

            // Actualizar ListBox
            ActualizarListaReproduccion();
        }
    }

    private void CargarArchivo(string ruta)
    {
        _audioPlayer?.Dispose();

        try
        {
            // Obtener duración sin cargar todo el archivo en memoria
            _duracionTotalSegundos = ObtenerDuracionArchivo(ruta);
            materialProgressBar1.Maximum = _duracionTotalSegundos;
            materialProgressBar1.Value = 0;

            _audioPlayer = new AudioPlayer(ruta, _volumenPersistente);

            // Extraer y mostrar metadatos en el título de la ventana
            var metadatos = ExtraerMetadatos(ruta);
            ActualizarTituloVentana(metadatos);

            _audioPlayer.Play();
            _estaReproduciendo = true;
            _timerProgreso?.Start();
            materialFloatingActionButton1.Icon = Reproductor.WinForm.Properties.Resources.pause;

            // Actualizar display de tiempo
            ActualizarTiempoDisplay(0);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al cargar el archivo:\n\n{ex.Message}\n\nEl archivo puede tener un formato no compatible o estar dañado.",
                "Error de Reproducción",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            // Resetear interfaz
            lblTitulo.Text = "Error al cargar";
            lblArtistaAlbum.Text = "Intenta con otro archivo";
            CargarPortadaPorDefecto();
        }
    }

    private int ObtenerDuracionArchivo(string ruta)
    {
        try
        {
            // Usar AudioFileReader para obtener la duración
            using var reader = new AudioFileReader(ruta);
            return (int)reader.TotalTime.TotalSeconds;
        }
        catch (Exception ex)
        {
            // Último recurso: intentar con TagLib para obtener la duración de los metadatos
            try
            {
                using var file = TagLib.File.Create(ruta);
                if (file.Properties.Duration.TotalSeconds > 0)
                {
                    return (int)file.Properties.Duration.TotalSeconds;
                }
            }
            catch { }

            throw new Exception("No se pudo obtener la duración del archivo", ex);
        }
    }

    private (string titulo, string artista, string album) ExtraerMetadatos(string rutaArchivo)
    {
        try
        {
            using var file = TagLib.File.Create(rutaArchivo);

            string titulo = file.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(rutaArchivo);
            string artista = file.Tag.FirstAlbumArtist ?? file.Tag.FirstPerformer ?? "Desconocido";
            string album = file.Tag.Album ?? "";

            return (titulo, artista, album);
        }
        catch
        {
            return (System.IO.Path.GetFileNameWithoutExtension(rutaArchivo), "Desconocido", "");
        }
    }

    private void ActualizarTituloVentana((string titulo, string artista, string album) metadatos)
    {
        this.Text = $"Reproductor de Musica";

        // Truncar título si es muy largo y configurar tooltip
        string tituloCompleto = metadatos.titulo.ToUpper();
        string tituloTruncado = TruncarTexto(tituloCompleto, lblTitulo);
        lblTitulo.Text = tituloTruncado;
        _toolTip.SetToolTip(lblTitulo, tituloCompleto);

        // Truncar artista/álbum si es muy largo y configurar tooltip
        string artistaAlbumCompleto = !string.IsNullOrEmpty(metadatos.album)
            ? $"{metadatos.artista} - {metadatos.album}"
            : metadatos.artista;

        string artistaAlbumTruncado = TruncarTexto(artistaAlbumCompleto, lblArtistaAlbum);
        lblArtistaAlbum.Text = artistaAlbumTruncado;
        _toolTip.SetToolTip(lblArtistaAlbum, artistaAlbumCompleto);
    }

    private string TruncarTexto(string texto, Label label)
    {
        if (string.IsNullOrEmpty(texto))
            return texto;

        using (Graphics g = label.CreateGraphics())
        {
            SizeF tamanho = g.MeasureString(texto, label.Font);

            // Si el texto cabe completo, devolverlo tal cual
            if (tamanho.Width <= label.Width)
                return texto;

            // Calcular cuántos caracteres caben aproximadamente
            float anchoCaracter = tamanho.Width / texto.Length;
            int caracteresQueCaben = (int)(label.Width / anchoCaracter) - 5;

            if (caracteresQueCaben > 0)
                return texto.Substring(0, caracteresQueCaben) + "...";

            return "...";
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _audioPlayer?.Dispose();
        _timerProgreso?.Stop();
        _timerProgreso?.Dispose();
        _timerShuffle?.Stop();
        _timerShuffle?.Dispose();
        _toolTip?.Dispose();
        base.OnFormClosing(e);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        // Forzar tamaño correcto
        this.Size = TAMANO_FORMULARIO;

        // Timer para verificar periódicamente que el tamaño no haya cambiado
        var timerVerificador = new System.Windows.Forms.Timer { Interval = 100 };
        timerVerificador.Tick += (s, args) =>
        {
            if (this.Size != TAMANO_FORMULARIO)
            {
                this.Size = TAMANO_FORMULARIO;
            }
        };
        timerVerificador.Start();

        // Suscribir al evento de cambio de valor del slider
        // MaterialSlider no tiene evento directo, usamos MouseUp
        Volumen.MouseUp += (s, args) => Volumen_ValueChanged(Volumen, EventArgs.Empty);

        // Inicializar label de tiempo
        ActualizarTiempoDisplay(0);
    }

    // Override de OnResize para prevenir cambios
    protected override void OnResize(EventArgs e)
    {
        if (this.Size != TAMANO_FORMULARIO)
        {
            this.Size = TAMANO_FORMULARIO;
            return; // No llamar al base para evitar que se procese el resize
        }
        base.OnResize(e);
    }

    private void btnAbrir_Click(object sender, EventArgs e)
    {
        AbrirArchivo();
    }

    // Play/Pause button
    private void materialFloatingActionButton1_Click_1(object sender, EventArgs e)
    {
        if (_audioPlayer == null) return;

        if (_estaReproduciendo)
        {
            _audioPlayer.Pause();
            _estaReproduciendo = false;
            _timerProgreso?.Stop();
            materialFloatingActionButton1.Icon = Reproductor.WinForm.Properties.Resources.play;  // CAMBIADO a play
        }
        else
        {
            _audioPlayer.Play();
            _estaReproduciendo = true;
            _timerProgreso?.Start();
            materialFloatingActionButton1.Icon = Reproductor.WinForm.Properties.Resources.pause;  // CAMBIADO a pause
        }
    }

    // Next button
    private void pictureBox1_Click(object sender, EventArgs e)
    {
        ReproducirSiguiente();
    }

    // Previous button
    private void pictureBox2_Click(object sender, EventArgs e)
    {
        ReproducirAnterior();
    }

    // Forward +10 buttom
    private void pictureBox4_Click(object sender, EventArgs e)
    {
        // Skip backward -10 segundos
        if (_audioPlayer != null)
        {
            int currentPos = _audioPlayer.GetCurrentPosition();
            int newPos = Math.Max(0, currentPos - 10);
            _audioPlayer.SetPosition(newPos);
            materialProgressBar1.Value = newPos;
            ActualizarTiempoDisplay(newPos);
        }
    }

    // Replay -10 buttom
    private void pictureBox3_Click(object sender, EventArgs e)
    {
        // Skip forward +10 segundos
        if (_audioPlayer != null)
        {
            int currentPos = _audioPlayer.GetCurrentPosition();
            int newPos = Math.Min(materialProgressBar1.Maximum, currentPos + 10);
            _audioPlayer.SetPosition(newPos);
            materialProgressBar1.Value = newPos;
            ActualizarTiempoDisplay(newPos);
        }
    }

    // Shuffle buttom
    private void pictureBox6_Click_1(object sender, EventArgs e)
    {
        if (_listaReproduccion.Count <= 1)
            return;

        // Mostrar efecto visual de shuffle activado
        pictureBox6.BackgroundImage = Reproductor.WinForm.Properties.Resources.shuffleON;

        // Guardar la canción actual
        string cancionActual = _listaReproduccion[_indiceActual];

        // Mezclar la lista (excepto la canción actual si está reproduciendo)
        Random rng = new Random();
        int n = _listaReproduccion.Count;

        // Crear una lista temporal sin la canción actual
        var listaTemporal = _listaReproduccion.Where((x, i) => i != _indiceActual).ToList();

        // Mezclar la lista temporal usando Fisher-Yates
        for (int i = listaTemporal.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (listaTemporal[i], listaTemporal[j]) = (listaTemporal[j], listaTemporal[i]);
        }

        // Reconstruir la lista con la canción actual al principio
        _listaReproduccion.Clear();
        _listaReproduccion.Add(cancionActual);
        _listaReproduccion.AddRange(listaTemporal);
        _indiceActual = 0;

        ActualizarListaReproduccion();

        // Iniciar timer para volver al icono original después de 1 segundo
        _timerShuffle?.Stop();
        _timerShuffle?.Start();
    }

    private void TimerShuffle_Tick(object? sender, EventArgs e)
    {
        // Volver al icono original
        pictureBox6.BackgroundImage = Reproductor.WinForm.Properties.Resources.shuffle;
        _timerShuffle?.Stop();
    }

    // Repeat buttom
    private void pictureBox5_Click(object sender, EventArgs e)
    {
        _repetirCancion = !_repetirCancion;

        // Cambiar icono segun estado
        if (_repetirCancion)
        {
            pictureBox5.BackgroundImage = Reproductor.WinForm.Properties.Resources.repeatON;
        }
        else
        {
            pictureBox5.BackgroundImage = Reproductor.WinForm.Properties.Resources.repeat;
        }
    }

    // Clean buttom
    private void materialButton1_Click(object sender, EventArgs e)
    {
        if (_listaReproduccion.Count == 0)
            return;

        // Detener reproducción actual
        _audioPlayer?.Stop();
        _audioPlayer?.Dispose();
        _audioPlayer = null;
        _estaReproduciendo = false;
        _timerProgreso?.Stop();

        // Limpiar todo
        _listaReproduccion.Clear();
        _indiceActual = -1;
        _archivoActual = null;

        // Resetear interfaz
        listaReproduccion.Items.Clear();
        listaReproduccion.Visible = false;
        lblTitulo.Text = "Reproductor";
        lblArtistaAlbum.Text = "Lista vacía";
        CargarPortadaPorDefecto();
        materialProgressBar1.Value = 0;
        ActualizarTiempoDisplay(0);
        materialFloatingActionButton1.Icon = Reproductor.WinForm.Properties.Resources.play;
    }

    private void pictureBox7_Click(object sender, EventArgs e)
    {
        if (!_estaMuteado)
        {
            // Guardar el volumen actual antes de mutear
            _volumenAntesDelMute = Volumen.Value / 100f;

            // Poner volumen a 0 (muteado)
            Volumen.Value = 0;
            _volumenPersistente = 0;

            // Cambiar icono a muteado
            pictureBox7.Image = Reproductor.WinForm.Properties.Resources.volume_off;

            _estaMuteado = true;
        }
        else
        {
            // Restaurar volumen anterior
            Volumen.Value = (int)(_volumenAntesDelMute * 100);
            _volumenPersistente = _volumenAntesDelMute;

            // Cambiar icono a volumen activo
            pictureBox7.Image = Reproductor.WinForm.Properties.Resources.volume_up;

            // Quitar color de fondo naranja (volver transparente)
            pictureBox7.BackColor = Color.Transparent;

            _estaMuteado = false;
        }

        // Aplicar al reproductor si está activo
        if (_audioPlayer != null)
        {
            _audioPlayer.SetVolume(_volumenPersistente);
        }
    }
}

public interface IAudioPlayer
{
    void Play();
    void Pause();
    void Stop();
    void SetVolume(float volume);
    void SetPosition(int seconds);
    int GetCurrentPosition();
    void Dispose();
}

public class AudioPlayer : IAudioPlayer, IDisposable
{
    private IWavePlayer? _wavePlayer;
    private ISampleProvider? _sampleProvider;
    private AudioFileReader? _audioFileReader;
    private readonly string _rutaArchivo;
    private readonly float _volumenInicial;

    public AudioPlayer(string rutaArchivo, float volumen = 0.5f)
    {
        _rutaArchivo = rutaArchivo;
        _volumenInicial = volumen;
    }

    public void Play()
    {
        if (_wavePlayer == null)
        {
            try
            {
                // Crear el reader del archivo
                _audioFileReader = new AudioFileReader(_rutaArchivo);
                _audioFileReader.Volume = _volumenInicial;

                // Para archivos de 24-bit o alta calidad, agregar conversión si es necesario
                _sampleProvider = CrearCadenaConversion(_audioFileReader);

                // Usar WaveOutEvent con buffer más grande para archivos grandes
                _wavePlayer = new WaveOutEvent
                {
                    DesiredLatency = 200, // Latencia más alta para archivos grandes
                    NumberOfBuffers = 2
                };

                _wavePlayer.Init(_sampleProvider);
            }
            catch (Exception ex)
            {
                // Limpiar recursos si falla
                _audioFileReader?.Dispose();
                _audioFileReader = null;
                throw new Exception($"Error al inicializar el audio: {ex.Message}", ex);
            }
        }

        _wavePlayer.Play();
    }

    private ISampleProvider CrearCadenaConversion(AudioFileReader reader)
    {
        ISampleProvider provider = reader;

        // Si el archivo tiene una sample rate muy alta (mayor a 48kHz), resamplear
        // Esto reduce el uso de CPU y mejora la compatibilidad
        if (reader.WaveFormat.SampleRate > 48000)
        {
            // Usar WdlResamplingSampleProvider para reducir a 48kHz
            provider = new WdlResamplingSampleProvider(reader, 48000);
        }

        return provider;
    }

    public void Pause()
    {
        _wavePlayer?.Pause();
    }

    public void Stop()
    {
        _wavePlayer?.Stop();
        _audioFileReader?.Dispose();
        _wavePlayer?.Dispose();
        _audioFileReader = null;
        _wavePlayer = null;
        _sampleProvider = null;
    }

    public void SetVolume(float volume)
    {
        if (_audioFileReader != null)
        {
            _audioFileReader.Volume = volume;
        }
    }

    public void SetPosition(int seconds)
    {
        if (_audioFileReader != null)
        {
            _audioFileReader.CurrentTime = TimeSpan.FromSeconds(seconds);
        }
    }

    public int GetCurrentPosition()
    {
        if (_audioFileReader != null)
        {
            return (int)_audioFileReader.CurrentTime.TotalSeconds;
        }
        return 0;
    }

    public void Dispose()
    {
        Stop();
    }
}
