namespace ReproductorFLAC;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;


    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        materialProgressBar1 = new MaterialSkin.Controls.MaterialProgressBar();
        Volumen = new MaterialSkin.Controls.MaterialSlider();
        btnAbrir = new MaterialSkin.Controls.MaterialButton();
        pictureBoxPortada = new PictureBox();
        lblTiempo = new MaterialSkin.Controls.MaterialLabel();
        lblTitulo = new MaterialSkin.Controls.MaterialLabel();
        lblArtistaAlbum = new MaterialSkin.Controls.MaterialLabel();
        pictureBox1 = new PictureBox();
        pictureBox2 = new PictureBox();
        materialFloatingActionButton1 = new MaterialSkin.Controls.MaterialFloatingActionButton();
        pictureBox3 = new PictureBox();
        pictureBox4 = new PictureBox();
        lbltiempo2 = new MaterialSkin.Controls.MaterialLabel();
        ((System.ComponentModel.ISupportInitialize)pictureBoxPortada).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
        SuspendLayout();
        // 
        // materialProgressBar1
        // 
        materialProgressBar1.Depth = 0;
        materialProgressBar1.Location = new Point(59, 577);
        materialProgressBar1.MouseState = MaterialSkin.MouseState.HOVER;
        materialProgressBar1.Name = "materialProgressBar1";
        materialProgressBar1.Size = new Size(531, 5);
        materialProgressBar1.TabIndex = 5;
        materialProgressBar1.Click += materialProgressBar1_Click;
        // 
        // Volumen
        // 
        Volumen.Depth = 0;
        Volumen.ForeColor = Color.FromArgb(222, 0, 0, 0);
        Volumen.Location = new Point(181, 733);
        Volumen.MouseState = MaterialSkin.MouseState.HOVER;
        Volumen.Name = "Volumen";
        Volumen.ShowText = false;
        Volumen.ShowValue = false;
        Volumen.Size = new Size(286, 40);
        Volumen.TabIndex = 6;
        Volumen.Text = "";
        // 
        // btnAbrir
        // 
        btnAbrir.AutoSize = false;
        btnAbrir.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnAbrir.BackColor = Color.Transparent;
        btnAbrir.BackgroundImageLayout = ImageLayout.None;
        btnAbrir.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
        btnAbrir.Depth = 0;
        btnAbrir.FlatStyle = FlatStyle.Flat;
        btnAbrir.HighEmphasis = true;
        btnAbrir.Icon = null;
        btnAbrir.Location = new Point(546, 737);
        btnAbrir.Margin = new Padding(4, 6, 4, 6);
        btnAbrir.MouseState = MaterialSkin.MouseState.HOVER;
        btnAbrir.Name = "btnAbrir";
        btnAbrir.NoAccentTextColor = Color.Empty;
        btnAbrir.Size = new Size(73, 36);
        btnAbrir.TabIndex = 9;
        btnAbrir.Text = "📁 Abrir";
        btnAbrir.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
        btnAbrir.UseAccentColor = false;
        btnAbrir.UseVisualStyleBackColor = false;
        btnAbrir.Click += btnAbrir_Click;
        // 
        // pictureBoxPortada
        // 
        pictureBoxPortada.BackColor = Color.FromArgb(240, 240, 240);
        pictureBoxPortada.BorderStyle = BorderStyle.FixedSingle;
        pictureBoxPortada.Location = new Point(134, 94);
        pictureBoxPortada.Name = "pictureBoxPortada";
        pictureBoxPortada.Size = new Size(380, 380);
        pictureBoxPortada.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxPortada.TabIndex = 10;
        pictureBoxPortada.TabStop = false;
        // 
        // lblTiempo
        // 
        lblTiempo.AutoSize = true;
        lblTiempo.Depth = 0;
        lblTiempo.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        lblTiempo.Location = new Point(59, 588);
        lblTiempo.Margin = new Padding(0, 0, 10, 10);
        lblTiempo.MouseState = MaterialSkin.MouseState.HOVER;
        lblTiempo.Name = "lblTiempo";
        lblTiempo.Size = new Size(41, 19);
        lblTiempo.TabIndex = 11;
        lblTiempo.Text = "00:00";
        lblTiempo.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblTitulo
        // 
        lblTitulo.Depth = 0;
        lblTitulo.Font = new Font("Roboto", 24F, FontStyle.Bold, GraphicsUnit.Pixel);
        lblTitulo.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
        lblTitulo.Location = new Point(25, 492);
        lblTitulo.Margin = new Padding(0, 0, 10, 10);
        lblTitulo.MouseState = MaterialSkin.MouseState.HOVER;
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Size = new Size(600, 29);
        lblTitulo.TabIndex = 12;
        lblTitulo.Text = "Título";
        lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblArtistaAlbum
        // 
        lblArtistaAlbum.Depth = 0;
        lblArtistaAlbum.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        lblArtistaAlbum.Location = new Point(174, 541);
        lblArtistaAlbum.Margin = new Padding(0, 0, 10, 10);
        lblArtistaAlbum.MouseState = MaterialSkin.MouseState.HOVER;
        lblArtistaAlbum.Name = "lblArtistaAlbum";
        lblArtistaAlbum.Size = new Size(300, 19);
        lblArtistaAlbum.TabIndex = 13;
        lblArtistaAlbum.Text = "Artista - Álbum";
        lblArtistaAlbum.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pictureBox1
        // 
        pictureBox1.BackColor = Color.Transparent;
        pictureBox1.BackgroundImage = Reproductor.WinForm.Properties.Resources.skip_forward;
        pictureBox1.BackgroundImageLayout = ImageLayout.Center;
        pictureBox1.Cursor = Cursors.Hand;
        pictureBox1.Location = new Point(361, 638);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(56, 56);
        pictureBox1.TabIndex = 15;
        pictureBox1.TabStop = false;
        pictureBox1.Click += pictureBox1_Click;
        // 
        // pictureBox2
        // 
        pictureBox2.BackColor = Color.Transparent;
        pictureBox2.BackgroundImage = Reproductor.WinForm.Properties.Resources.skip_back;
        pictureBox2.BackgroundImageLayout = ImageLayout.Center;
        pictureBox2.Cursor = Cursors.Hand;
        pictureBox2.Location = new Point(231, 638);
        pictureBox2.Name = "pictureBox2";
        pictureBox2.Size = new Size(56, 56);
        pictureBox2.TabIndex = 16;
        pictureBox2.TabStop = false;
        pictureBox2.Click += pictureBox2_Click;
        // 
        // materialFloatingActionButton1
        // 
        materialFloatingActionButton1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        materialFloatingActionButton1.Depth = 0;
        materialFloatingActionButton1.Icon = Reproductor.WinForm.Properties.Resources.play;
        materialFloatingActionButton1.Location = new Point(296, 638);
        materialFloatingActionButton1.MouseState = MaterialSkin.MouseState.HOVER;
        materialFloatingActionButton1.Name = "materialFloatingActionButton1";
        materialFloatingActionButton1.Size = new Size(56, 56);
        materialFloatingActionButton1.TabIndex = 17;
        materialFloatingActionButton1.Click += materialFloatingActionButton1_Click_1;
        // 
        // pictureBox3
        // 
        pictureBox3.BackColor = Color.Transparent;
        pictureBox3.BackgroundImage = Reproductor.WinForm.Properties.Resources.forward_10;
        pictureBox3.BackgroundImageLayout = ImageLayout.Center;
        pictureBox3.Cursor = Cursors.Hand;
        pictureBox3.Location = new Point(470, 644);
        pictureBox3.Name = "pictureBox3";
        pictureBox3.Size = new Size(44, 44);
        pictureBox3.TabIndex = 18;
        pictureBox3.TabStop = false;
        pictureBox3.Click += pictureBox3_Click;
        // 
        // pictureBox4
        // 
        pictureBox4.BackColor = Color.Transparent;
        pictureBox4.BackgroundImage = Reproductor.WinForm.Properties.Resources.replay_10;
        pictureBox4.BackgroundImageLayout = ImageLayout.Center;
        pictureBox4.Cursor = Cursors.Hand;
        pictureBox4.Location = new Point(134, 644);
        pictureBox4.Name = "pictureBox4";
        pictureBox4.Size = new Size(44, 44);
        pictureBox4.TabIndex = 19;
        pictureBox4.TabStop = false;
        pictureBox4.Click += pictureBox4_Click;
        // 
        // lbltiempo2
        // 
        lbltiempo2.AutoSize = true;
        lbltiempo2.Depth = 0;
        lbltiempo2.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        lbltiempo2.Location = new Point(546, 588);
        lbltiempo2.Margin = new Padding(0, 0, 10, 10);
        lbltiempo2.MouseState = MaterialSkin.MouseState.HOVER;
        lbltiempo2.Name = "lbltiempo2";
        lbltiempo2.Size = new Size(41, 19);
        lbltiempo2.TabIndex = 20;
        lbltiempo2.Text = "00:00";
        lbltiempo2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // Form1
        // 
        AutoScaleMode = AutoScaleMode.None;
        ClientSize = new Size(650, 800);
        Controls.Add(lbltiempo2);
        Controls.Add(pictureBox4);
        Controls.Add(pictureBox3);
        Controls.Add(materialFloatingActionButton1);
        Controls.Add(pictureBox2);
        Controls.Add(pictureBox1);
        Controls.Add(lblArtistaAlbum);
        Controls.Add(lblTitulo);
        Controls.Add(lblTiempo);
        Controls.Add(btnAbrir);
        Controls.Add(Volumen);
        Controls.Add(materialProgressBar1);
        Controls.Add(pictureBoxPortada);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        Name = "Form1";
        Padding = new Padding(0);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Reproductor de Musica";
        Load += Form1_Load;
        ((System.ComponentModel.ISupportInitialize)pictureBoxPortada).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
    private MaterialSkin.Controls.MaterialProgressBar materialProgressBar1;
    private MaterialSkin.Controls.MaterialSlider Volumen;
    private MaterialSkin.Controls.MaterialButton btnAbrir;
    private PictureBox pictureBoxPortada;
    private MaterialSkin.Controls.MaterialLabel lblTiempo;
    private MaterialSkin.Controls.MaterialLabel lblTitulo;
    private MaterialSkin.Controls.MaterialLabel lblArtistaAlbum;
    private PictureBox pictureBox1;
    private PictureBox pictureBox2;
    private MaterialSkin.Controls.MaterialFloatingActionButton materialFloatingActionButton1;
    private PictureBox pictureBox3;
    private PictureBox pictureBox4;
    private MaterialSkin.Controls.MaterialLabel lbltiempo2;
}
