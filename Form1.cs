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
            MaterialSkin.Accent.Orange700,
            MaterialSkin.TextShade.BLACK
        );

        // Inicializar timer
        _timerProgreso = new System.Windows.Forms.Timer { Interval = 100 };
        _timerProgreso.Tick += TimerProgreso_Tick;

        // Configurar controles
        materialButton3.Text = "⏮";  // Skip backward
        materialButton1.Text = "▶";   // Play
        materialButton2.Text = "⏭";   // Skip forward

        // Establecer volumen persistente en el slider
        Volumen.Value = (int)(_volumenPersistente * 100);

        // Cargar imagen por defecto para la portada
        CargarPortadaPorDefecto();
    }

    private void CargarPortadaPorDefecto()
    {
        pictureBoxPortada.Image = Properties.Resources.Image_not_found;
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

    private void materialButton1_Click(object sender, EventArgs e)
    {
        // Botón Play/Pause
        if (_audioPlayer == null) return;

        if (_estaReproduciendo)
        {
            _audioPlayer.Pause();
            materialButton1.Text = "▶";
            _estaReproduciendo = false;
            _timerProgreso?.Stop();
        }
        else
        {
            _audioPlayer.Play();
            materialButton1.Text = "⏸";
            _estaReproduciendo = true;
            _timerProgreso?.Start();
        }
    }

    private void materialButton2_Click(object sender, EventArgs e)
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

    private void materialButton3_Click(object sender, EventArgs e)
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
            this.Text = $"Reproductor FLAC - {System.IO.Path.GetFileName(_archivoActual)}";

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

        _audioPlayer = new AudioPlayer(ruta, _volumenPersistente); // Pasar volumen al constructor

        // Resetear botón play
        materialButton1.Text = "▶";
        _estaReproduciendo = false;
        _timerProgreso?.Stop();

        // Actualizar display de tiempo
        ActualizarTiempoDisplay(0);
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
        // Suscribir al evento de cambio de valor del slider
        // MaterialSlider no tiene evento directo, usamos MouseUp
        Volumen.MouseUp += (s, args) => Volumen_ValueChanged(Volumen, EventArgs.Empty);

        // Inicializar label de tiempo
        ActualizarTiempoDisplay(0);
    }

    private void volumeMeter2_Click(object sender, EventArgs e)
    {
        // Placeholder - no usado
    }

    private void btnAbrir_Click(object sender, EventArgs e)
    {
        AbrirArchivo();
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
