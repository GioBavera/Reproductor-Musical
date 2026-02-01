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
        materialButton1 = new MaterialSkin.Controls.MaterialButton();
        materialProgressBar1 = new MaterialSkin.Controls.MaterialProgressBar();
        Volumen = new MaterialSkin.Controls.MaterialSlider();
        materialButton2 = new MaterialSkin.Controls.MaterialButton();
        materialButton3 = new MaterialSkin.Controls.MaterialButton();
        btnAbrir = new MaterialSkin.Controls.MaterialButton();
        pictureBoxPortada = new PictureBox();
        lblTiempo = new MaterialSkin.Controls.MaterialLabel();
        ((System.ComponentModel.ISupportInitialize)pictureBoxPortada).BeginInit();
        SuspendLayout();
        // 
        // materialButton1
        // 
        materialButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
        materialButton1.Depth = 0;
        materialButton1.HighEmphasis = true;
        materialButton1.Icon = null;
        materialButton1.Location = new Point(311, 597);
        materialButton1.Margin = new Padding(4, 6, 4, 6);
        materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
        materialButton1.Name = "materialButton1";
        materialButton1.NoAccentTextColor = Color.Empty;
        materialButton1.Size = new Size(64, 36);
        materialButton1.TabIndex = 4;
        materialButton1.Text = "▶";
        materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
        materialButton1.UseAccentColor = false;
        materialButton1.UseVisualStyleBackColor = true;
        materialButton1.Click += materialButton1_Click;
        // 
        // materialProgressBar1
        // 
        materialProgressBar1.Depth = 0;
        materialProgressBar1.Location = new Point(78, 550);
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
        Volumen.Location = new Point(184, 684);
        Volumen.MouseState = MaterialSkin.MouseState.HOVER;
        Volumen.Name = "Volumen";
        Volumen.Size = new Size(286, 40);
        Volumen.TabIndex = 6;
        Volumen.Text = "Volumen";
        // 
        // materialButton2
        // 
        materialButton2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
        materialButton2.Depth = 0;
        materialButton2.HighEmphasis = true;
        materialButton2.Icon = null;
        materialButton2.Location = new Point(443, 597);
        materialButton2.Margin = new Padding(4, 6, 4, 6);
        materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
        materialButton2.Name = "materialButton2";
        materialButton2.NoAccentTextColor = Color.Empty;
        materialButton2.Size = new Size(64, 36);
        materialButton2.TabIndex = 7;
        materialButton2.Text = "⏭";
        materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
        materialButton2.UseAccentColor = false;
        materialButton2.UseVisualStyleBackColor = true;
        materialButton2.Click += materialButton2_Click;
        // 
        // materialButton3
        // 
        materialButton3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        materialButton3.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
        materialButton3.Depth = 0;
        materialButton3.HighEmphasis = true;
        materialButton3.Icon = null;
        materialButton3.Location = new Point(179, 597);
        materialButton3.Margin = new Padding(4, 6, 4, 6);
        materialButton3.MouseState = MaterialSkin.MouseState.HOVER;
        materialButton3.Name = "materialButton3";
        materialButton3.NoAccentTextColor = Color.Empty;
        materialButton3.Size = new Size(64, 36);
        materialButton3.TabIndex = 8;
        materialButton3.Text = "⏮";
        materialButton3.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
        materialButton3.UseAccentColor = false;
        materialButton3.UseVisualStyleBackColor = true;
        materialButton3.Click += materialButton3_Click;
        // 
        // btnAbrir
        // 
        btnAbrir.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnAbrir.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
        btnAbrir.Depth = 0;
        btnAbrir.HighEmphasis = true;
        btnAbrir.Icon = null;
        btnAbrir.Location = new Point(591, 81);
        btnAbrir.Margin = new Padding(4, 6, 4, 6);
        btnAbrir.MouseState = MaterialSkin.MouseState.HOVER;
        btnAbrir.Name = "btnAbrir";
        btnAbrir.NoAccentTextColor = Color.Empty;
        btnAbrir.Size = new Size(73, 36);
        btnAbrir.TabIndex = 9;
        btnAbrir.Text = "📁 Abrir";
        btnAbrir.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
        btnAbrir.UseAccentColor = false;
        btnAbrir.UseVisualStyleBackColor = true;
        btnAbrir.Click += btnAbrir_Click;
        // 
        // pictureBoxPortada
        // 
        pictureBoxPortada.BackColor = Color.FromArgb(240, 240, 240);
        pictureBoxPortada.BorderStyle = BorderStyle.FixedSingle;
        pictureBoxPortada.Location = new Point(175, 160);
        pictureBoxPortada.Name = "pictureBoxPortada";
        pictureBoxPortada.Size = new Size(336, 336);
        pictureBoxPortada.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxPortada.TabIndex = 10;
        pictureBoxPortada.TabStop = false;
        // 
        // lblTiempo
        // 
        lblTiempo.AutoSize = true;
        lblTiempo.Depth = 0;
        lblTiempo.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        lblTiempo.Location = new Point(295, 523);
        lblTiempo.Margin = new Padding(0, 0, 10, 10);
        lblTiempo.MouseState = MaterialSkin.MouseState.HOVER;
        lblTiempo.Name = "lblTiempo";
        lblTiempo.Size = new Size(96, 19);
        lblTiempo.TabIndex = 11;
        lblTiempo.Text = "00:00 / 00:00";
        lblTiempo.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 19F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(740, 870);
        Controls.Add(lblTiempo);
        Controls.Add(pictureBoxPortada);
        Controls.Add(btnAbrir);
        Controls.Add(materialButton3);
        Controls.Add(materialButton2);
        Controls.Add(Volumen);
        Controls.Add(materialProgressBar1);
        Controls.Add(materialButton1);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Reproductor FLAC";
        Load += Form1_Load;
        ((System.ComponentModel.ISupportInitialize)pictureBoxPortada).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private MaterialSkin.Controls.MaterialButton materialButton1;
    private MaterialSkin.Controls.MaterialProgressBar materialProgressBar1;
    private MaterialSkin.Controls.MaterialSlider Volumen;
    private MaterialSkin.Controls.MaterialButton materialButton2;
    private MaterialSkin.Controls.MaterialButton materialButton3;
    private MaterialSkin.Controls.MaterialButton btnAbrir;
    private PictureBox pictureBoxPortada;
    private MaterialSkin.Controls.MaterialLabel lblTiempo;
}
