using MaterialSkin.Controls;
using NAudio.Wave;

namespace ReproductorFLAC;

public partial class Form1 : MaterialForm
{
    private IAudioPlayer? _audioPlayer;
    private string? _archivoActual;
    private bool _estaReproduciendo = false;
    private System.Windows.Forms.Timer? _timerProgreso;

    // Volumen persistente (variable estatica para mantener entre canciones)
    private static float _volumenPersistente = 0.5f; // 50% por defecto
    private int _duracionTotalSegundos = 0;

    // Tamaño CORRECTO y FIJO del formulario
    private static readonly Size TAMANO_FORMULARIO = new Size(650, 800);

    public Form1()
    {
        InitializeComponent();

        // Configurar tema
        var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
        materialSkinManager.AddFormToManage(this);
        materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
        materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
            MaterialSkin.Primary.Blue700,
            MaterialSkin.Primary.Blue900,
            MaterialSkin.Primary.Blue500,
            MaterialSkin.Accent.Blue400,  // Cambiado de Orange700 a Blue400
            MaterialSkin.TextShade.BLACK
        );

        // Inicializar timer
        _timerProgreso = new System.Windows.Forms.Timer { Interval = 100 };
        _timerProgreso.Tick += TimerProgreso_Tick;

        // Configurar controles

        // Establecer volumen persistente en el slider
        Volumen.Value = (int)(_volumenPersistente * 100);

        // IMPORTANTE: Desactivar AutoSize de los labels para evitar que se redimensionen solos
        lblTitulo.AutoSize = false;
        lblArtistaAlbum.AutoSize = false;

        // Cargar imagen por defecto para la portada
        CargarPortadaPorDefecto();

        // Inicializar labels de metadatos
        lblTitulo.Text = "Reproductor FLAC";
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
        }
    }

    private void ActualizarTiempoDisplay(int posicionActual)
    {
        lblTiempo.Text = $"{FormatearTiempo(posicionActual)} / {FormatearTiempo(_duracionTotalSegundos)}";
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
        ofd.Title = "Seleccionar archivo FLAC";

        if (ofd.ShowDialog() == DialogResult.OK)
        {
            _archivoActual = ofd.FileName;

            // Restablecer labels mientras carga
            lblTitulo.Text = "Cargando...";
            lblArtistaAlbum.Text = "";

            CargarArchivo(_archivoActual);

            // Extraer y mostrar portada
            ExtraerPortada(_archivoActual);
        }
    }

    private void CargarArchivo(string ruta)
    {
        _audioPlayer?.Dispose();

        using var reader = new AudioFileReader(ruta);

        _duracionTotalSegundos = (int)reader.TotalTime.TotalSeconds;
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

        // Actualizar labels en la interfaz
        lblTitulo.Text = metadatos.titulo.ToUpper();

        if (!string.IsNullOrEmpty(metadatos.album))
        {
            lblArtistaAlbum.Text = $"{metadatos.artista} - {metadatos.album}";
        }
        else
        {
            lblArtistaAlbum.Text = metadatos.artista;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _audioPlayer?.Dispose();
        _timerProgreso?.Stop();
        _timerProgreso?.Dispose();
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

    private void volumeMeter2_Click(object sender, EventArgs e)
    {
        // Placeholder - no usado
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
        // Futura Implementacion.
    }

    // Previous button
    private void pictureBox2_Click(object sender, EventArgs e)
    {
        // Futura Implementacion.
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
            _audioFileReader = new AudioFileReader(_rutaArchivo);
            _audioFileReader.Volume = _volumenInicial; // Establecer volumen al crear el reader
            _wavePlayer = new WaveOutEvent();
            _wavePlayer.Init(_audioFileReader);
        }

        _wavePlayer.Play();
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
