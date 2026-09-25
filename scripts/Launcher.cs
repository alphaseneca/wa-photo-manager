using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

public class AppConfig
{
    public List<string> AllowedNumbers = new List<string>();
    public List<string> PhotoCategories = new List<string>();
    public string DownloadsDir = "downloads";
    public string SessionId = "photo-manager-session";
    public bool Headless = true;
    public bool MultiDevice = true;

    public AppConfig()
    {
        // Defaults matching config.js
        PhotoCategories.Add("Photo Category 1");
        PhotoCategories.Add("Photo Category 2");
        PhotoCategories.Add("Photo Category 3");
        PhotoCategories.Add("Photo Category 4");
    }

    public static AppConfig Load(string path)
    {
        AppConfig config = new AppConfig();
        if (!File.Exists(path)) return config;

        try
        {
            string content = File.ReadAllText(path);
            
            Match numbersMatch = Regex.Match(content, @"\""ALLOWED_NUMBERS\""\s*:\s*\[(.*?)\]", RegexOptions.Singleline);
            if (numbersMatch.Success)
            {
                config.AllowedNumbers.Clear();
                MatchCollection matches = Regex.Matches(numbersMatch.Groups[1].Value, @"\""(.*?)\""");
                foreach (Match m in matches)
                {
                    config.AllowedNumbers.Add(m.Groups[1].Value);
                }
            }

            Match categoriesMatch = Regex.Match(content, @"\""PHOTO_CATEGORIES\""\s*:\s*\[(.*?)\]", RegexOptions.Singleline);
            if (categoriesMatch.Success)
            {
                config.PhotoCategories.Clear();
                MatchCollection matches = Regex.Matches(categoriesMatch.Groups[1].Value, @"\""(.*?)\""");
                foreach (Match m in matches)
                {
                    config.PhotoCategories.Add(m.Groups[1].Value);
                }
            }

            Match downloadsMatch = Regex.Match(content, @"\""downloadsDir\""\s*:\s*\""(.*?)\""");
            if (downloadsMatch.Success)
            {
                config.DownloadsDir = downloadsMatch.Groups[1].Value;
            }

            Match sessionMatch = Regex.Match(content, @"\""sessionId\""\s*:\s*\""(.*?)\""");
            if (sessionMatch.Success)
            {
                config.SessionId = sessionMatch.Groups[1].Value;
            }

            Match headlessMatch = Regex.Match(content, @"\""headless\""\s*:\s*(true|false)");
            if (headlessMatch.Success)
            {
                config.Headless = bool.Parse(headlessMatch.Groups[1].Value);
            }

            Match multiDeviceMatch = Regex.Match(content, @"\""multiDevice\""\s*:\s*(true|false)");
            if (multiDeviceMatch.Success)
            {
                config.MultiDevice = bool.Parse(multiDeviceMatch.Groups[1].Value);
            }
        }
        catch (Exception ex)
        {
            ModernMessageBox.Show("Error loading config.json: " + ex.Message, "Configuration Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        return config;
    }

    public void Save(string path)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            
            // ALLOWED_NUMBERS
            sb.Append("  \"ALLOWED_NUMBERS\": [");
            for (int i = 0; i < AllowedNumbers.Count; i++)
            {
                sb.Append("\"" + AllowedNumbers[i] + "\"");
                if (i < AllowedNumbers.Count - 1) sb.Append(", ");
            }
            sb.AppendLine("],");

            // PHOTO_CATEGORIES
            sb.Append("  \"PHOTO_CATEGORIES\": [");
            for (int i = 0; i < PhotoCategories.Count; i++)
            {
                sb.Append("\"" + PhotoCategories[i] + "\"");
                if (i < PhotoCategories.Count - 1) sb.Append(", ");
            }
            sb.AppendLine("],");

            // FOLDER_SETTINGS
            sb.AppendLine("  \"FOLDER_SETTINGS\": {");
            sb.AppendLine("    \"downloadsDir\": \"" + DownloadsDir.Replace("\\", "\\\\") + "\"");
            sb.AppendLine("  },");

            // BOT_CONFIG
            sb.AppendLine("  \"BOT_CONFIG\": {");
            sb.AppendLine("    \"sessionId\": \"" + SessionId + "\",");
            sb.AppendLine("    \"headless\": " + Headless.ToString().ToLower() + ",");
            sb.AppendLine("    \"multiDevice\": " + MultiDevice.ToString().ToLower());
            sb.AppendLine("  }");

            sb.AppendLine("}");

            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
        }
        catch (Exception ex)
        {
            ModernMessageBox.Show("Error saving config.json: " + ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

public static class WindowHelper
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(IntPtr hWnd);

    private const int SW_RESTORE = 9;

    public static void ForceForeground(Form form)
    {
        if (form == null || form.IsDisposed) return;
        try
        {
            if (form.InvokeRequired)
            {
                form.BeginInvoke(new Action(() => ForceForeground(form)));
                return;
            }

            if (!form.Visible)
            {
                form.Show();
            }
            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }

            ShowWindowAsync(form.Handle, SW_RESTORE);
            BringWindowToTop(form.Handle);
            SetForegroundWindow(form.Handle);
            form.Activate();
            form.BringToFront();
        }
        catch {}
    }
}

public static class UITheme
{
    public static readonly Color WhatsAppTeal = Color.FromArgb(0, 168, 132);       // #00A884
    public static readonly Color WhatsAppDarkTeal = Color.FromArgb(7, 94, 84);     // #075E54
    public static readonly Color BackgroundLight = Color.FromArgb(248, 250, 252);  // #F8FAFC
    public static readonly Color CardLight = Color.FromArgb(255, 255, 255);
    public static readonly Color BorderColor = Color.FromArgb(226, 232, 240);      // #E2E8F0
    public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);         // #0F172A
    public static readonly Color TextSecondary = Color.FromArgb(100, 116, 139);    // #64748B
    public static readonly Color ConsoleDark = Color.FromArgb(15, 23, 42);         // #0F172A
    public static readonly Color ConsoleToolbar = Color.FromArgb(30, 41, 59);      // #1E293B

    public static Button CreateButton(string text, Color bg, Color fg, int width = 110, int height = 34, bool isBold = false)
    {
        try
        {
            Program.LogLifecycle("CreateButton start: " + text);
            RoundedButton btn = new RoundedButton();
            Program.LogLifecycle("CreateButton: RoundedButton constructed");
            btn.Text = text;
            btn.BackColor = bg;
            btn.ForeColor = fg;
            btn.Font = new Font("Segoe UI", 9.25F, isBold ? FontStyle.Bold : FontStyle.Regular);
            btn.Size = new Size(width, height);
            btn.CornerRadius = 6;
            Program.LogLifecycle("CreateButton end: " + text);
            return btn;
        }
        catch (Exception ex)
        {
            Program.LogLifecycle("CreateButton ERROR for " + text + ": " + ex.ToString());
            throw;
        }
    }
}

public class RoundedButton : Button
{
    private int _cornerRadius = 6;
    private bool _isHovered = false;
    private bool _isPressed = false;

    public int CornerRadius
    {
        get { return _cornerRadius; }
        set { _cornerRadius = value; Invalidate(); }
    }

    public RoundedButton()
    {
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;
        this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        this.BackColor = Color.White;
        this.Cursor = Cursors.Hand;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        _isPressed = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            _isPressed = true;
            Invalidate();
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _isPressed = false;
        Invalidate();
    }

    private Color GetParentBackColor()
    {
        Control p = this.Parent;
        while (p != null)
        {
            if (p.BackColor != Color.Transparent && p.BackColor != Color.Empty)
            {
                return p.BackColor;
            }
            p = p.Parent;
        }
        return Color.White;
    }

    public static GraphicsPath CreateRoundedRectanglePath(RectangleF rect, float radius)
    {
        GraphicsPath path = new GraphicsPath();
        float diameter = radius * 2f;
        if (rect.Width < diameter) diameter = rect.Width;
        if (rect.Height < diameter) diameter = rect.Height;

        RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);

        // Top-left
        path.AddArc(arc, 180, 90);

        // Top-right
        arc.X = rect.Right - diameter;
        path.AddArc(arc, 270, 90);

        // Bottom-right
        arc.Y = rect.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // Bottom-left
        arc.X = rect.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }

    private static Color AdjustBrightness(Color baseColor, float factor)
    {
        float r = (float)baseColor.R;
        float g = (float)baseColor.G;
        float b = (float)baseColor.B;

        if (factor > 0)
        {
            r = r + (255f - r) * factor;
            g = g + (255f - g) * factor;
            b = b + (255f - b) * factor;
        }
        else
        {
            r = r * (1f + factor);
            g = g * (1f + factor);
            b = b * (1f + factor);
        }

        int ir = (int)Math.Max(0, Math.Min(255, Math.Round(r)));
        int ig = (int)Math.Max(0, Math.Min(255, Math.Round(g)));
        int ib = (int)Math.Max(0, Math.Min(255, Math.Round(b)));

        return Color.FromArgb(baseColor.A, ir, ig, ib);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        Color parentBg = GetParentBackColor();
        using (SolidBrush parentBrush = new SolidBrush(parentBg))
        {
            g.FillRectangle(parentBrush, this.ClientRectangle);
        }

        Color currentColor = this.BackColor;
        if (!this.Enabled)
        {
            currentColor = Color.FromArgb(226, 232, 240);
        }
        else if (_isPressed)
        {
            currentColor = AdjustBrightness(this.BackColor, -0.15f);
        }
        else if (_isHovered)
        {
            if (this.BackColor.R + this.BackColor.G + this.BackColor.B > 600)
            {
                currentColor = AdjustBrightness(this.BackColor, -0.06f);
            }
            else
            {
                currentColor = AdjustBrightness(this.BackColor, -0.08f);
            }
        }

        float offset = 0.5f;
        RectangleF rect = new RectangleF(offset, offset, (float)this.Width - 1f, (float)this.Height - 1f);

        using (GraphicsPath path = CreateRoundedRectanglePath(rect, (float)_cornerRadius))
        {
            using (SolidBrush fillBrush = new SolidBrush(currentColor))
            {
                g.FillPath(fillBrush, path);
            }

            int borderSize = this.FlatAppearance.BorderSize;
            Color borderColor = this.FlatAppearance.BorderColor;
            if (borderSize > 0 && borderColor != Color.Empty && borderColor != Color.Transparent)
            {
                using (Pen pen = new Pen(borderColor, (float)borderSize))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        Color textColor = this.Enabled ? this.ForeColor : Color.FromArgb(148, 163, 184);
        TextRenderer.DrawText(
            g,
            this.Text,
            this.Font,
            this.ClientRectangle,
            textColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
        );
    }
}

public class ModernMessageBox : Form
{
    public ModernMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        InitializeDialog(message, title, buttons, icon);
    }

    public static DialogResult Show(string message, string title = "WhatsApp Photo Manager", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
    {
        using (ModernMessageBox box = new ModernMessageBox(message, title, buttons, icon))
        {
            return box.ShowDialog();
        }
    }

    public static DialogResult Show(IWin32Window owner, string message, string title = "WhatsApp Photo Manager", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
    {
        using (ModernMessageBox box = new ModernMessageBox(message, title, buttons, icon))
        {
            return box.ShowDialog(owner);
        }
    }

    private void InitializeDialog(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        this.Text = title;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.Icon = Program.AppIcon;
        this.ShowInTaskbar = false;

        try
        {
            if (icon == MessageBoxIcon.Error || icon == MessageBoxIcon.Stop || icon == MessageBoxIcon.Hand)
                System.Media.SystemSounds.Hand.Play();
            else if (icon == MessageBoxIcon.Warning || icon == MessageBoxIcon.Exclamation)
                System.Media.SystemSounds.Exclamation.Play();
            else
                System.Media.SystemSounds.Asterisk.Play();
        }
        catch {}

        Color headerColor = UITheme.WhatsAppDarkTeal;
        Color accentColor = UITheme.WhatsAppTeal;

        if (icon == MessageBoxIcon.Error || icon == MessageBoxIcon.Stop || icon == MessageBoxIcon.Hand)
        {
            headerColor = Color.FromArgb(185, 28, 28);
            accentColor = Color.FromArgb(220, 38, 38);
        }
        else if (icon == MessageBoxIcon.Warning || icon == MessageBoxIcon.Exclamation)
        {
            headerColor = Color.FromArgb(180, 83, 9);
            accentColor = Color.FromArgb(217, 119, 6);
        }

        int dialogWidth = 480;
        int textWidth = 370;
        Size textSize;
        using (Graphics g = this.CreateGraphics())
        {
            textSize = TextRenderer.MeasureText(g, message, new Font("Segoe UI", 9.25F), new Size(textWidth, 1000), TextFormatFlags.WordBreak);
        }
        int contentHeight = Math.Max(70, textSize.Height + 24);
        int dialogHeight = 50 + contentHeight + 56;

        this.ClientSize = new Size(dialogWidth, dialogHeight);

        // Header Panel
        Panel pnlHeader = new Panel()
        {
            Dock = DockStyle.Top,
            Height = 48,
            BackColor = headerColor,
            Padding = new Padding(16, 12, 16, 12)
        };
        Label lblTitle = new Label()
        {
            Text = title,
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(16, 13),
            AutoSize = true
        };
        pnlHeader.Controls.Add(lblTitle);

        // Footer Panel
        Panel pnlFooter = new Panel()
        {
            Dock = DockStyle.Bottom,
            Height = 54,
            BackColor = Color.FromArgb(248, 250, 252)
        };
        pnlFooter.Paint += (s, e) => {
            using (Pen p = new Pen(UITheme.BorderColor, 1))
            {
                e.Graphics.DrawLine(p, 0, 0, pnlFooter.Width, 0);
            }
        };

        if (buttons == MessageBoxButtons.OK)
        {
            Button btnOk = UITheme.CreateButton("OK", accentColor, Color.White, width: 88, height: 32, isBold: true);
            btnOk.Location = new Point(dialogWidth - 104, 11);
            btnOk.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };
            pnlFooter.Controls.Add(btnOk);
            this.AcceptButton = btnOk;
            this.CancelButton = btnOk;
        }
        else if (buttons == MessageBoxButtons.OKCancel)
        {
            Button btnCancel = UITheme.CreateButton("Cancel", Color.White, UITheme.TextPrimary, width: 88, height: 32);
            btnCancel.FlatAppearance.BorderColor = UITheme.BorderColor;
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.Location = new Point(dialogWidth - 104, 11);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            Button btnOk = UITheme.CreateButton("OK", accentColor, Color.White, width: 88, height: 32, isBold: true);
            btnOk.Location = new Point(dialogWidth - 200, 11);
            btnOk.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            pnlFooter.Controls.Add(btnOk);
            pnlFooter.Controls.Add(btnCancel);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
        else if (buttons == MessageBoxButtons.YesNo)
        {
            Button btnNo = UITheme.CreateButton("No", Color.White, UITheme.TextPrimary, width: 88, height: 32);
            btnNo.FlatAppearance.BorderColor = UITheme.BorderColor;
            btnNo.FlatAppearance.BorderSize = 1;
            btnNo.Location = new Point(dialogWidth - 104, 11);
            btnNo.Click += (s, e) => { this.DialogResult = DialogResult.No; this.Close(); };

            Button btnYes = UITheme.CreateButton("Yes", accentColor, Color.White, width: 88, height: 32, isBold: true);
            btnYes.Location = new Point(dialogWidth - 200, 11);
            btnYes.Click += (s, e) => { this.DialogResult = DialogResult.Yes; this.Close(); };

            pnlFooter.Controls.Add(btnYes);
            pnlFooter.Controls.Add(btnNo);
            this.AcceptButton = btnYes;
            this.CancelButton = btnNo;
        }

        // Content Panel
        Panel pnlContent = new Panel()
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        PictureBox picIcon = new PictureBox()
        {
            Location = new Point(20, 16),
            Size = new Size(38, 38),
            SizeMode = PictureBoxSizeMode.CenterImage
        };
        picIcon.Paint += (s, e) => {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush bgBrush = new SolidBrush(accentColor))
            {
                g.FillEllipse(bgBrush, 1, 1, 35, 35);
            }
            string symbol = "i";
            if (icon == MessageBoxIcon.Error || icon == MessageBoxIcon.Stop || icon == MessageBoxIcon.Hand)
                symbol = "✕";
            else if (icon == MessageBoxIcon.Warning || icon == MessageBoxIcon.Exclamation)
                symbol = "!";
            else if (icon == MessageBoxIcon.Question)
                symbol = "?";

            using (Font f = new Font("Segoe UI", 12.5F, FontStyle.Bold))
            using (SolidBrush fgBrush = new SolidBrush(Color.White))
            {
                StringFormat sf = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(symbol, f, fgBrush, new RectangleF(0, 0, 38, 38), sf);
            }
        };

        Label lblMessage = new Label()
        {
            Text = message,
            Font = new Font("Segoe UI", 9.25F),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(72, 16),
            Size = new Size(textWidth, contentHeight),
            AutoEllipsis = false
        };

        pnlContent.Controls.Add(picIcon);
        pnlContent.Controls.Add(lblMessage);

        this.Controls.Add(pnlContent);
        this.Controls.Add(pnlFooter);
        this.Controls.Add(pnlHeader);
    }
}

public class AlreadyRunningDialog : Form
{
    public const string WAKE_EVENT_NAME = "Local\\WhatsAppPhotoManagerWakeEvent";
    public const string PING_EVENT_NAME = "Local\\WhatsAppPhotoManagerPingEvent";

    public AlreadyRunningDialog()
    {
        InitializeComponent();
        try
        {
            System.Media.SystemSounds.Asterisk.Play();
        }
        catch {}

        SignalPrimaryPing();
    }

    private void InitializeComponent()
    {
        this.Text = "WhatsApp Photo Manager";
        this.Size = new Size(500, 290);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.Icon = Program.AppIcon;
        this.ShowInTaskbar = true;

        // Top Header
        Panel pnlHeader = new Panel()
        {
            Dock = DockStyle.Top,
            Height = 62,
            BackColor = UITheme.WhatsAppDarkTeal,
            Padding = new Padding(18, 10, 18, 10)
        };

        Label lblHeaderTitle = new Label()
        {
            Text = "WhatsApp Photo Manager",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Location = new Point(18, 10),
            AutoSize = true
        };

        Label lblHeaderSub = new Label()
        {
            Text = "Background service is active and running",
            ForeColor = Color.FromArgb(209, 250, 229),
            Font = new Font("Segoe UI", 8.75F),
            Location = new Point(19, 34),
            AutoSize = true
        };

        pnlHeader.Controls.Add(lblHeaderTitle);
        pnlHeader.Controls.Add(lblHeaderSub);

        // Bottom Footer
        Panel pnlFooter = new Panel()
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            BackColor = Color.FromArgb(248, 250, 252)
        };

        pnlFooter.Paint += (s, e) => {
            using (Pen p = new Pen(UITheme.BorderColor, 1))
            {
                e.Graphics.DrawLine(p, 0, 0, pnlFooter.Width, 0);
            }
        };

        Button btnGotIt = UITheme.CreateButton("Got It", Color.White, UITheme.TextPrimary, width: 90, height: 34);
        btnGotIt.FlatAppearance.BorderColor = UITheme.BorderColor;
        btnGotIt.FlatAppearance.BorderSize = 1;
        btnGotIt.Location = new Point(380, 11);
        btnGotIt.Click += (s, e) => this.Close();

        Button btnOpenLogs = UITheme.CreateButton("View Live Logs", UITheme.WhatsAppTeal, Color.White, width: 135, height: 34, isBold: true);
        btnOpenLogs.Location = new Point(235, 11);
        btnOpenLogs.Click += (s, e) => {
            SignalPrimaryWake();
            this.Close();
        };

        pnlFooter.Controls.Add(btnOpenLogs);
        pnlFooter.Controls.Add(btnGotIt);

        // Center Content Panel
        Panel pnlContent = new Panel()
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        PictureBox picLogo = new PictureBox()
        {
            Location = new Point(22, 18),
            Size = new Size(52, 52),
            SizeMode = PictureBoxSizeMode.Zoom
        };

        try
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string logoPng = Path.Combine(baseDir, "app-logo.png");
            if (!File.Exists(logoPng))
            {
                logoPng = Path.Combine(baseDir, "assets", "app-logo.png");
            }
            if (File.Exists(logoPng))
            {
                using (Image img = Image.FromFile(logoPng))
                {
                    picLogo.Image = new Bitmap(img);
                }
            }
            else if (Program.AppIcon != null)
            {
                picLogo.Image = Program.AppIcon.ToBitmap();
            }
        }
        catch {}

        Label lblHeadline = new Label()
        {
            Text = "Application is Already Running",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(88, 16),
            AutoSize = true
        };

        Label lblDesc = new Label()
        {
            Text = "WhatsApp Photo Manager is already active in your Windows system tray and monitoring chats.",
            Font = new Font("Segoe UI", 9F),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(88, 38),
            Size = new Size(380, 36)
        };

        Panel pnlTipCard = new Panel()
        {
            Location = new Point(88, 76),
            Size = new Size(382, 44),
            BackColor = Color.FromArgb(240, 253, 250)
        };
        pnlTipCard.Paint += (s, e) => {
            using (Pen p = new Pen(Color.FromArgb(153, 246, 228), 1))
            {
                e.Graphics.DrawRectangle(p, 0, 0, pnlTipCard.Width - 1, pnlTipCard.Height - 1);
            }
        };

        Label lblTip = new Label()
        {
            Text = "Tip: Look for the camera icon near your taskbar clock. Right-click it anytime for settings, logs, or to link devices.",
            Font = new Font("Segoe UI", 8.25F),
            ForeColor = Color.FromArgb(17, 94, 89),
            Location = new Point(8, 6),
            Size = new Size(366, 32)
        };
        pnlTipCard.Controls.Add(lblTip);

        pnlContent.Controls.Add(picLogo);
        pnlContent.Controls.Add(lblHeadline);
        pnlContent.Controls.Add(lblDesc);
        pnlContent.Controls.Add(pnlTipCard);

        this.Controls.Add(pnlContent);
        this.Controls.Add(pnlFooter);
        this.Controls.Add(pnlHeader);

        this.AcceptButton = btnOpenLogs;
        this.CancelButton = btnGotIt;
    }

    public static void SignalPrimaryWake()
    {
        try
        {
            using (EventWaitHandle handle = EventWaitHandle.OpenExisting(WAKE_EVENT_NAME))
            {
                handle.Set();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Wake event error: " + ex.Message);
        }
    }

    public static void SignalPrimaryPing()
    {
        try
        {
            using (EventWaitHandle handle = EventWaitHandle.OpenExisting(PING_EVENT_NAME))
            {
                handle.Set();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Ping event error: " + ex.Message);
        }
    }
}

public enum ToastType
{
    Info,
    Success,
    Warning,
    Error
}

public class ModernToastNotification : Form
{
    private static ModernToastNotification _currentToast = null;
    private static readonly object _lockObj = new object();

    private System.Windows.Forms.Timer animTimer;
    private System.Windows.Forms.Timer stayTimer;
    private int targetX;
    private int targetY;
    private int startY;
    private bool isExiting = false;
    private bool isHovered = false;
    private Action clickAction;

    protected override bool ShowWithoutActivation
    {
        get { return true; }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
            cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
            cp.ExStyle |= 0x00000008; // WS_EX_TOPMOST
            return cp;
        }
    }

    public static void Show(string title, string message, ToastType type = ToastType.Info, Action onClick = null, int durationMs = 4500)
    {
        try
        {
            if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
            {
                Application.OpenForms[0].BeginInvoke(new Action(() => Show(title, message, type, onClick, durationMs)));
                return;
            }

            lock (_lockObj)
            {
                if (_currentToast != null && !_currentToast.IsDisposed)
                {
                    _currentToast.CloseImmediately();
                }

                _currentToast = new ModernToastNotification(title, message, type, onClick, durationMs);
                _currentToast.Show();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error showing toast: " + ex.Message);
        }
    }

    public ModernToastNotification(string title, string message, ToastType type, Action onClick, int durationMs)
    {
        this.clickAction = onClick;

        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.Manual;
        this.ShowInTaskbar = false;
        this.TopMost = true;
        this.BackColor = Color.FromArgb(17, 27, 33); // #111B21 (WhatsApp Dark Mode)
        this.ForeColor = Color.White;
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.Cursor = Cursors.Hand;
        this.DoubleBuffered = true;
        this.Opacity = 0.0;

        int toastWidth = 390;
        int toastHeight = 115;
        this.Size = new Size(toastWidth, toastHeight);

        Screen targetScreen = Screen.PrimaryScreen;
        if (targetScreen == null)
        {
            targetScreen = Screen.FromPoint(Cursor.Position);
        }
        Rectangle workingArea = targetScreen.WorkingArea;
        targetX = workingArea.Right - toastWidth - 16;
        targetY = workingArea.Bottom - toastHeight - 16;
        startY = targetY + 20;

        this.Location = new Point(targetX, startY);

        using (GraphicsPath path = RoundedButton.CreateRoundedRectanglePath(new RectangleF(0, 0, toastWidth, toastHeight), 10f))
        {
            this.Region = new Region(path);
        }

        Color accentColor = UITheme.WhatsAppTeal; // #00A884
        string badgeText = "\u25CF ACTIVE";
        Color badgeColor = Color.FromArgb(74, 222, 128); // #4ADE80

        if (type == ToastType.Success)
        {
            accentColor = Color.FromArgb(34, 197, 94); // #22C55E
            badgeText = "\u25CF CONNECTED";
            badgeColor = Color.FromArgb(34, 197, 94);
        }
        else if (type == ToastType.Warning)
        {
            accentColor = Color.FromArgb(245, 158, 11); // #F59E0B
            badgeText = "\u25CF NOTICE";
            badgeColor = Color.FromArgb(245, 158, 11);
        }
        else if (type == ToastType.Error)
        {
            accentColor = Color.FromArgb(239, 68, 68); // #EF4444
            badgeText = "\u25CF ERROR";
            badgeColor = Color.FromArgb(239, 68, 68);
        }

        this.Paint += (s, e) => {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color currentBorder = isHovered ? accentColor : Color.FromArgb(42, 57, 66); // #2A3942
            using (Pen borderPen = new Pen(currentBorder, 1))
            {
                using (GraphicsPath bPath = RoundedButton.CreateRoundedRectanglePath(new RectangleF(0.5f, 0.5f, toastWidth - 1f, toastHeight - 1f), 10f))
                {
                    g.DrawPath(borderPen, bPath);
                }
            }

            using (SolidBrush accentBrush = new SolidBrush(accentColor))
            {
                g.FillRectangle(accentBrush, 0, 0, 4, toastHeight);
            }
        };

        PictureBox picLogo = new PictureBox()
        {
            Location = new Point(14, 11),
            Size = new Size(20, 20),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        try
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string logoPng = Path.Combine(baseDir, "app-logo.png");
            if (!File.Exists(logoPng)) logoPng = Path.Combine(baseDir, "assets", "app-logo.png");
            if (!File.Exists(logoPng)) logoPng = Path.Combine(Directory.GetCurrentDirectory(), "assets", "app-logo.png");
            if (!File.Exists(logoPng)) logoPng = Path.Combine(Directory.GetCurrentDirectory(), "app-logo.png");
            if (File.Exists(logoPng))
            {
                using (Image img = Image.FromFile(logoPng))
                {
                    picLogo.Image = new Bitmap(img);
                }
            }
            else if (Program.AppIcon != null)
            {
                picLogo.Image = Program.AppIcon.ToBitmap();
            }
        }
        catch {}

        Label lblApp = new Label()
        {
            Text = "WhatsApp Photo Manager",
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(233, 237, 239), // #E9EDEF
            Location = new Point(38, 12),
            AutoSize = true,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        Label lblBadge = new Label()
        {
            Text = badgeText,
            Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
            ForeColor = badgeColor,
            Location = new Point(204, 13),
            AutoSize = true,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        Label btnClose = new Label()
        {
            Text = "\u2715",
            Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(134, 150, 160), // #8696A0
            Location = new Point(toastWidth - 28, 8),
            Size = new Size(20, 20),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.White;
        btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.FromArgb(134, 150, 160);
        btnClose.Click += (s, e) => CloseWithFade();

        Label lblTitle = new Label()
        {
            Text = title,
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(14, 38),
            Size = new Size(toastWidth - 32, 20),
            AutoEllipsis = true,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        Label lblMessage = new Label()
        {
            Text = message,
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular),
            ForeColor = Color.FromArgb(209, 215, 219), // #D1D7DB
            Location = new Point(14, 58),
            Size = new Size(toastWidth - 32, 32),
            AutoEllipsis = true,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        Label lblAction = new Label()
        {
            Text = onClick != null ? "Click to view live logs \u2197" : "Active in system tray",
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 168, 132), // WhatsApp Teal
            Location = new Point(14, 91),
            AutoSize = true,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        Control[] interactiveControls = new Control[] { this, picLogo, lblApp, lblBadge, lblTitle, lblMessage, lblAction };
        foreach (Control c in interactiveControls)
        {
            c.Click += (s, e) => {
                CloseWithFade();
                if (clickAction != null)
                {
                    clickAction();
                }
            };
            c.MouseEnter += (s, e) => {
                isHovered = true;
                this.BackColor = Color.FromArgb(24, 34, 41);
                this.Invalidate();
                if (stayTimer != null) stayTimer.Stop();
            };
            c.MouseLeave += (s, e) => {
                isHovered = false;
                this.BackColor = Color.FromArgb(17, 27, 33);
                this.Invalidate();
                if (stayTimer != null && !isExiting) stayTimer.Start();
            };
        }

        this.Controls.Add(picLogo);
        this.Controls.Add(lblApp);
        this.Controls.Add(lblBadge);
        this.Controls.Add(btnClose);
        this.Controls.Add(lblTitle);
        this.Controls.Add(lblMessage);
        this.Controls.Add(lblAction);

        animTimer = new System.Windows.Forms.Timer();
        animTimer.Interval = 15;
        animTimer.Tick += (s, e) => {
            if (!isExiting)
            {
                double nextOpacity = this.Opacity + 0.12;
                if (nextOpacity >= 0.98)
                {
                    this.Opacity = 0.98;
                    this.Location = new Point(targetX, targetY);
                    animTimer.Stop();
                    stayTimer.Start();
                }
                else
                {
                    this.Opacity = nextOpacity;
                    int curY = this.Location.Y;
                    int step = Math.Max(1, (curY - targetY) / 3);
                    this.Location = new Point(targetX, curY - step);
                }
            }
            else
            {
                double nextOpacity = this.Opacity - 0.15;
                if (nextOpacity <= 0)
                {
                    animTimer.Stop();
                    this.Close();
                }
                else
                {
                    this.Opacity = nextOpacity;
                }
            }
        };

        stayTimer = new System.Windows.Forms.Timer();
        stayTimer.Interval = durationMs;
        stayTimer.Tick += (s, e) => {
            stayTimer.Stop();
            CloseWithFade();
        };

        animTimer.Start();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        this.Location = new Point(targetX, startY);
    }

    private void CloseWithFade()
    {
        if (isExiting) return;
        isExiting = true;
        if (stayTimer != null)
        {
            stayTimer.Stop();
            stayTimer.Dispose();
        }
        if (animTimer != null)
        {
            animTimer.Start();
        }
        else
        {
            this.Close();
        }
    }

    public void CloseImmediately()
    {
        isExiting = true;
        if (stayTimer != null) { stayTimer.Stop(); stayTimer.Dispose(); }
        if (animTimer != null) { animTimer.Stop(); animTimer.Dispose(); }
        this.Close();
        this.Dispose();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        if (stayTimer != null) { stayTimer.Dispose(); stayTimer = null; }
        if (animTimer != null) { animTimer.Dispose(); animTimer = null; }
        lock (_lockObj)
        {
            if (_currentToast == this)
            {
                _currentToast = null;
            }
        }
    }
}

public class SettingsForm : Form
{
    private AppConfig config;
    private string configPath;

    private TabControl tabControl;
    private TabPage tabBasic;
    private TabPage tabCategories;
    private TabPage tabAdvanced;

    private TextBox txtDownloads;
    private Button btnBrowse;
    private TextBox txtNumbers;

    private ListBox lstCategories;
    private TextBox txtNewCategory;
    private Button btnAddCategory;
    private Button btnRemoveCategory;

    private TextBox txtSessionId;
    private CheckBox chkHeadless;
    private CheckBox chkMultiDevice;

    private Button btnSave;
    private Button btnCancel;

    public SettingsForm(string configPath)
    {
        this.configPath = configPath;
        this.config = AppConfig.Load(configPath);
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "WhatsApp Photo Manager v1.0.0 - Settings";
        this.Size = new Size(540, 580);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = UITheme.BackgroundLight;
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.Icon = Program.AppIcon;

        // Top Header Banner
        Panel pnlHeader = new Panel()
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = UITheme.WhatsAppTeal
        };
        Label lblHeaderTitle = new Label()
        {
            Text = "Bot & Storage Configuration",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(18, 12),
            AutoSize = true
        };
        Label lblHeaderSub = new Label()
        {
            Text = "Manage authorized contacts, photo storage paths, and bot behavior",
            Font = new Font("Segoe UI", 8.75F),
            ForeColor = Color.FromArgb(230, 250, 245),
            Location = new Point(19, 36),
            AutoSize = true
        };
        pnlHeader.Controls.AddRange(new Control[] { lblHeaderTitle, lblHeaderSub });

        // Tab Control
        tabControl = new TabControl();
        tabControl.Bounds = new Rectangle(16, 78, 492, 420);
        tabControl.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular);

        tabBasic = new TabPage("General");
        tabBasic.BackColor = Color.White;
        tabCategories = new TabPage("Photo Categories");
        tabCategories.BackColor = Color.White;
        tabAdvanced = new TabPage("Advanced");
        tabAdvanced.BackColor = Color.White;

        tabControl.TabPages.Add(tabBasic);
        tabControl.TabPages.Add(tabCategories);
        tabControl.TabPages.Add(tabAdvanced);

        // --- TAB 1: GENERAL SETTINGS ---
        Label lblDownloads = new Label() 
        { 
            Text = "Downloads Storage Directory:", 
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(16, 16), 
            AutoSize = true 
        };
        txtDownloads = new TextBox() 
        { 
            Text = config.DownloadsDir, 
            Location = new Point(16, 38), 
            Size = new Size(360, 26),
            Font = new Font("Segoe UI", 9.5F)
        };
        btnBrowse = UITheme.CreateButton("Browse...", Color.FromArgb(241, 245, 249), UITheme.TextPrimary, 85, 28);
        btnBrowse.FlatAppearance.BorderSize = 1;
        btnBrowse.FlatAppearance.BorderColor = UITheme.BorderColor;
        btnBrowse.Location = new Point(384, 37);
        btnBrowse.Click += BtnBrowse_Click;

        Label lblNumbers = new Label() 
        { 
            Text = "Authorized Contacts (One WhatsApp Phone Number per line):", 
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(16, 80), 
            AutoSize = true 
        };
        Label lblNumbersHint = new Label()
        {
            Text = "Only messages from these phone numbers will be processed. Leave empty to allow all.",
            Font = new Font("Segoe UI", 8.25F),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(16, 100),
            AutoSize = true
        };
        txtNumbers = new TextBox()
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = false,
            Location = new Point(16, 122),
            Size = new Size(453, 245),
            Font = new Font("Consolas", 9.5F),
            Text = string.Join(Environment.NewLine, config.AllowedNumbers)
        };

        tabBasic.Controls.AddRange(new Control[] { lblDownloads, txtDownloads, btnBrowse, lblNumbers, lblNumbersHint, txtNumbers });

        // --- TAB 2: CATEGORIES ---
        Label lblCatTitle = new Label()
        {
            Text = "Registered Categories:",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(16, 16),
            AutoSize = true
        };
        lstCategories = new ListBox() 
        { 
            Location = new Point(16, 38), 
            Size = new Size(270, 310),
            Font = new Font("Segoe UI", 9.5F),
            BorderStyle = BorderStyle.FixedSingle
        };
        foreach (var cat in config.PhotoCategories) lstCategories.Items.Add(cat);

        txtNewCategory = new TextBox() 
        { 
            Location = new Point(298, 38), 
            Size = new Size(170, 26),
            Font = new Font("Segoe UI", 9.5F)
        };
        btnAddCategory = UITheme.CreateButton("+ Add Category", UITheme.WhatsAppTeal, Color.White, 170, 32, true);
        btnAddCategory.Location = new Point(298, 70);
        btnAddCategory.Click += BtnAddCategory_Click;

        btnRemoveCategory = UITheme.CreateButton("Remove Selected", Color.FromArgb(254, 226, 226), Color.FromArgb(185, 28, 28), 170, 32);
        btnRemoveCategory.Location = new Point(298, 108);
        btnRemoveCategory.Click += BtnRemoveCategory_Click;

        Label lblCatHint = new Label() 
        { 
            Text = "Tip:\nEach category automatically creates a subfolder inside your downloads directory.\n\nWhen contacts send photos with a category name (e.g. 'Bill'), the photo is filed into that folder automatically.", 
            Location = new Point(298, 155), 
            Size = new Size(170, 190),
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = UITheme.TextSecondary
        };

        tabCategories.Controls.AddRange(new Control[] { lblCatTitle, lstCategories, txtNewCategory, btnAddCategory, btnRemoveCategory, lblCatHint });

        // --- TAB 3: ADVANCED ---
        Label lblSession = new Label() 
        { 
            Text = "WhatsApp Session Identifier:", 
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(16, 16), 
            AutoSize = true 
        };
        txtSessionId = new TextBox() 
        { 
            Text = config.SessionId, 
            Location = new Point(16, 38), 
            Size = new Size(453, 26),
            Font = new Font("Segoe UI", 9.5F)
        };

        chkHeadless = new CheckBox() 
        { 
            Text = "Run Browser in Background (Headless Mode - Recommended)", 
            Checked = config.Headless, 
            Location = new Point(16, 85), 
            Size = new Size(453, 25),
            Font = new Font("Segoe UI", 9.25F)
        };

        chkMultiDevice = new CheckBox() 
        { 
            Text = "WhatsApp Multi-Device Protocol Support", 
            Checked = config.MultiDevice, 
            Location = new Point(16, 120), 
            Size = new Size(453, 25),
            Font = new Font("Segoe UI", 9.25F)
        };

        Label lblAdvNote = new Label()
        {
            Text = "Changing the session identifier will require you to re-link your phone via QR code.",
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(16, 160),
            Size = new Size(453, 40)
        };

        tabAdvanced.Controls.AddRange(new Control[] { lblSession, txtSessionId, chkHeadless, chkMultiDevice, lblAdvNote });

        // Bottom Action Bar
        Panel pnlBottom = new Panel()
        {
            Dock = DockStyle.Bottom,
            Height = 55,
            BackColor = UITheme.BackgroundLight
        };

        btnSave = UITheme.CreateButton("Save Settings", UITheme.WhatsAppTeal, Color.White, 120, 36, true);
        btnSave.Location = new Point(275, 10);
        btnSave.Click += BtnSave_Click;

        btnCancel = UITheme.CreateButton("Cancel", Color.FromArgb(241, 245, 249), UITheme.TextPrimary, 85, 36);
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = UITheme.BorderColor;
        btnCancel.Location = new Point(405, 10);
        btnCancel.Click += BtnCancel_Click;

        pnlBottom.Controls.AddRange(new Control[] { btnSave, btnCancel });

        this.Controls.AddRange(new Control[] { tabControl, pnlHeader, pnlBottom });
    }

    private void BtnBrowse_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog fbd = new FolderBrowserDialog())
        {
            fbd.Description = "Select Downloads Storage Directory";
            fbd.ShowNewFolderButton = true;
            if (Directory.Exists(txtDownloads.Text))
            {
                fbd.SelectedPath = txtDownloads.Text;
            }
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtDownloads.Text = fbd.SelectedPath;
            }
        }
    }

    private void BtnAddCategory_Click(object sender, EventArgs e)
    {
        string newCat = txtNewCategory.Text.Trim();
        if (string.IsNullOrEmpty(newCat)) return;
        if (lstCategories.Items.Contains(newCat))
        {
            ModernMessageBox.Show(this, "Category already exists.", "Duplicate Category", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        lstCategories.Items.Add(newCat);
        txtNewCategory.Clear();
    }

    private void BtnRemoveCategory_Click(object sender, EventArgs e)
    {
        if (lstCategories.SelectedIndex != -1)
        {
            lstCategories.Items.RemoveAt(lstCategories.SelectedIndex);
        }
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        config.DownloadsDir = txtDownloads.Text.Trim();
        config.SessionId = txtSessionId.Text.Trim();
        config.Headless = chkHeadless.Checked;
        config.MultiDevice = chkMultiDevice.Checked;

        config.AllowedNumbers.Clear();
        string[] numbers = txtNumbers.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string num in numbers)
        {
            string clean = num.Trim();
            if (!string.IsNullOrEmpty(clean))
            {
                config.AllowedNumbers.Add(clean);
            }
        }

        config.PhotoCategories.Clear();
        foreach (var item in lstCategories.Items)
        {
            config.PhotoCategories.Add(item.ToString());
        }

        config.Save(configPath);
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}

public class LogViewerForm : Form
{
    private TextBox txtLog;
    private Label lblStatus;
    private Button btnClear;
    private Button btnCopy;
    private Button btnOpenFolder;

    public LogViewerForm()
    {
        try
        {
            Program.LogLifecycle("LogViewerForm constructor started");
            InitializeComponent();
            Program.LogLifecycle("LogViewerForm InitializeComponent finished");
        }
        catch (Exception ex)
        {
            Program.LogLifecycle("LogViewerForm constructor ERROR: " + ex.ToString());
            throw;
        }
    }

    private void InitializeComponent()
    {
        this.Text = "WhatsApp Photo Manager - Control Center & Live Logs";
        this.Size = new Size(760, 520);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Icon = Program.AppIcon;
        this.ShowInTaskbar = true;
        this.BackColor = UITheme.ConsoleDark;

        // Top Toolbar
        Panel pnlToolbar = new Panel()
        {
            Height = 48,
            Dock = DockStyle.Top,
            BackColor = UITheme.ConsoleToolbar
        };

        lblStatus = new Label()
        {
            Text = "● Bot Offline",
            ForeColor = Color.FromArgb(248, 113, 113),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Location = new Point(15, 15),
            AutoSize = true
        };

        btnOpenFolder = UITheme.CreateButton("Open Log File", Color.FromArgb(51, 65, 85), Color.White, 105, 30);
        btnOpenFolder.Location = new Point(410, 9);
        btnOpenFolder.Click += BtnOpenFolder_Click;

        btnCopy = UITheme.CreateButton("Copy All", Color.FromArgb(51, 65, 85), Color.White, 85, 30);
        btnCopy.Location = new Point(525, 9);
        btnCopy.Click += BtnCopy_Click;

        btnClear = UITheme.CreateButton("Clear", Color.FromArgb(51, 65, 85), Color.White, 75, 30);
        btnClear.Location = new Point(620, 9);
        btnClear.Click += (s, e) => txtLog.Clear();

        pnlToolbar.Controls.AddRange(new Control[] { lblStatus, btnOpenFolder, btnCopy, btnClear });

        // Log Viewer TextBox
        txtLog = new TextBox()
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = true,
            Font = new Font("Consolas", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
            BackColor = UITheme.ConsoleDark,
            ForeColor = Color.FromArgb(203, 213, 225),
            BorderStyle = BorderStyle.None,
            Dock = DockStyle.Fill
        };

        // Bottom Status Hint
        Panel pnlBottom = new Panel()
        {
            Height = 26,
            Dock = DockStyle.Bottom,
            BackColor = Color.FromArgb(10, 15, 26)
        };
        Label lblHint = new Label()
        {
            Text = "Logs are continuously archived to bot_output.log in the installation directory.",
            ForeColor = Color.FromArgb(148, 163, 184),
            Font = new Font("Segoe UI", 8.25F),
            Location = new Point(12, 5),
            AutoSize = true
        };
        pnlBottom.Controls.Add(lblHint);

        this.Controls.Add(txtLog);
        this.Controls.Add(pnlToolbar);
        this.Controls.Add(pnlBottom);

        this.FormClosing += LogViewerForm_FormClosing;
    }

    public void SetBotOnlineStatus(bool online)
    {
        if (this.IsDisposed) return;
        try
        {
            if (!this.IsHandleCreated)
            {
                this.CreateControl();
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(SetBotOnlineStatus), online);
                return;
            }

            if (online)
            {
                lblStatus.Text = "● Bot Online & Running";
                lblStatus.ForeColor = Color.FromArgb(74, 222, 128); // Green
            }
            else
            {
                lblStatus.Text = "○ Bot Offline (Stopped)";
                lblStatus.ForeColor = Color.FromArgb(248, 113, 113); // Red
            }
        }
        catch {}
    }

    private void BtnCopy_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtLog.Text))
        {
            Clipboard.SetText(txtLog.Text);
            btnCopy.Text = "Copied!";
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 1500;
            t.Tick += (s, ev) => {
                btnCopy.Text = "Copy All";
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }
    }

    private void BtnOpenFolder_Click(object sender, EventArgs e)
    {
        string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bot_output.log");
        if (File.Exists(logPath))
        {
            Process.Start("notepad.exe", "\"" + logPath + "\"");
        }
        else
        {
            ModernMessageBox.Show(this, "No log file has been created yet.\nThe background bot will create bot_output.log once started.", "Logs Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void LogViewerForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

    public void AppendLog(string text)
    {
        if (this.IsDisposed) return;
        try
        {
            if (!this.IsHandleCreated)
            {
                this.CreateControl();
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AppendLog), text);
                return;
            }

            if (txtLog.TextLength > 500000)
            {
                txtLog.Text = txtLog.Text.Substring(200000);
            }

            txtLog.AppendText(text);
        }
        catch {}
    }
}

public class QrCodeForm : Form
{
    private PictureBox pictureBox;
    private string imagePath;
    private FileSystemWatcher watcher;
    private Label lblStatus;

    public QrCodeForm(string qrImagePath)
    {
        this.imagePath = qrImagePath;
        InitializeComponent();
        LoadQRImage();
        StartWatcher();
    }

    private void InitializeComponent()
    {
        this.Text = "Link WhatsApp - WhatsApp Photo Manager";
        this.Size = new Size(460, 600);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = true;
        this.ShowInTaskbar = true;
        this.TopMost = true;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(248, 250, 252);
        this.Icon = Program.AppIcon;

        // Top WhatsApp Teal Header
        Panel pnlHeader = new Panel()
        {
            Dock = DockStyle.Top,
            Height = 72,
            BackColor = UITheme.WhatsAppTeal
        };
        Label lblTitle = new Label()
        {
            Text = "Link with WhatsApp",
            Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 14),
            AutoSize = true
        };
        Label lblSub = new Label()
        {
            Text = "Scan this QR code from your phone to connect the bot",
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.FromArgb(230, 250, 245),
            Location = new Point(21, 40),
            AutoSize = true
        };
        pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSub });

        // QR Code Card Container
        Panel cardPanel = new Panel()
        {
            Location = new Point(95, 90),
            Size = new Size(270, 270),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        pictureBox = new PictureBox()
        {
            Location = new Point(10, 10),
            Size = new Size(248, 248),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.White
        };
        cardPanel.Controls.Add(pictureBox);

        // Status Badge
        lblStatus = new Label()
        {
            Text = "● Waiting for phone scan...",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = UITheme.WhatsAppDarkTeal,
            Location = new Point(20, 372),
            Size = new Size(420, 24),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Instructions Card
        Panel pnlInstructions = new Panel()
        {
            Location = new Point(30, 405),
            Size = new Size(400, 95),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        Label step1 = new Label()
        {
            Text = "1. Open WhatsApp on your phone",
            Font = new Font("Segoe UI", 9F),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(14, 10),
            AutoSize = true
        };
        Label step2 = new Label()
        {
            Text = "2. Tap Menu (⋮) or Settings (⚙) and select Linked Devices",
            Font = new Font("Segoe UI", 9F),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(14, 35),
            AutoSize = true
        };
        Label step3 = new Label()
        {
            Text = "3. Tap 'Link a Device' and point your camera at this code",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = UITheme.WhatsAppDarkTeal,
            Location = new Point(14, 60),
            AutoSize = true
        };
        pnlInstructions.Controls.AddRange(new Control[] { step1, step2, step3 });

        // Bottom Action Button
        Button btnTray = UITheme.CreateButton("Minimize to Tray", Color.FromArgb(241, 245, 249), UITheme.TextPrimary, 150, 34);
        btnTray.FlatAppearance.BorderSize = 1;
        btnTray.FlatAppearance.BorderColor = UITheme.BorderColor;
        btnTray.Location = new Point(155, 515);
        btnTray.Click += (s, e) => this.Close();

        this.Controls.AddRange(new Control[] { pnlHeader, cardPanel, lblStatus, pnlInstructions, btnTray });
        this.FormClosed += QrCodeForm_FormClosed;
    }

    private static Bitmap RecolorQrToBlack(Bitmap src)
    {
        Bitmap result = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.Clear(Color.White);
            g.DrawImage(src, 0, 0, src.Width, src.Height);
        }

        BitmapData data = result.LockBits(
            new Rectangle(0, 0, result.Width, result.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppArgb);

        int bytes = Math.Abs(data.Stride) * result.Height;
        byte[] rgbValues = new byte[bytes];
        Marshal.Copy(data.Scan0, rgbValues, 0, bytes);

        for (int i = 0; i < bytes; i += 4)
        {
            byte b = rgbValues[i];
            byte g = rgbValues[i + 1];
            byte r = rgbValues[i + 2];
            byte a = rgbValues[i + 3];

            if (a < 128)
            {
                rgbValues[i] = 255;
                rgbValues[i + 1] = 255;
                rgbValues[i + 2] = 255;
                rgbValues[i + 3] = 255;
            }
            else
            {
                int lum = (r * 299 + g * 587 + b * 114) / 1000;
                if (lum >= 200)
                {
                    rgbValues[i] = 255;
                    rgbValues[i + 1] = 255;
                    rgbValues[i + 2] = 255;
                }
                else
                {
                    rgbValues[i] = 0;
                    rgbValues[i + 1] = 0;
                    rgbValues[i + 2] = 0;
                }
                rgbValues[i + 3] = 255;
            }
        }

        Marshal.Copy(rgbValues, 0, data.Scan0, bytes);
        result.UnlockBits(data);
        return result;
    }

    private void LoadQRImage()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(LoadQRImage));
            return;
        }

        try
        {
            byte[] bytes = null;
            for (int retry = 0; retry < 5; retry++)
            {
                try
                {
                    if (File.Exists(imagePath))
                    {
                        bytes = File.ReadAllBytes(imagePath);
                        if (bytes != null && bytes.Length > 0)
                        {
                            break;
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(60);
                }
            }

            if (bytes != null && bytes.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (Bitmap orig = new Bitmap(ms))
                    {
                        Bitmap recolored = RecolorQrToBlack(orig);
                        Image old = pictureBox.Image;
                        pictureBox.Image = recolored;
                        if (old != null)
                        {
                            old.Dispose();
                        }
                    }
                }
            }
            else
            {
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error reading QR image: " + ex.Message);
        }
    }

    private void StartWatcher()
    {
        string dir = Path.GetDirectoryName(Path.GetFullPath(imagePath));
        string file = Path.GetFileName(imagePath);

        watcher = new FileSystemWatcher(dir, file);
        watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
        
        watcher.Created += (s, e) => LoadQRImage();
        watcher.Changed += (s, e) => LoadQRImage();
        watcher.Deleted += (s, e) => {
            if (!this.IsDisposed)
            {
                this.BeginInvoke(new Action(() => this.Close()));
            }
        };
        watcher.EnableRaisingEvents = true;
    }

    private void QrCodeForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            watcher = null;
        }
        if (pictureBox != null && pictureBox.Image != null)
        {
            pictureBox.Image.Dispose();
            pictureBox.Image = null;
        }
    }
}

public class TrayApplicationContext : ApplicationContext
{
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;
    private ToolStripMenuItem menuHeader;
    private ToolStripMenuItem menuStatus;
    private ToolStripMenuItem menuToggle;
    private ToolStripMenuItem menuSettings;
    private ToolStripMenuItem menuLogs;
    private ToolStripMenuItem menuQR;
    private ToolStripMenuItem menuDownloads;
    private ToolStripMenuItem menuExit;

    private Process botProcess = null;
    private Thread outputThread = null;
    private Thread errorThread = null;
    private LogViewerForm logForm;
    private QrCodeForm qrForm = null;
    private FileSystemWatcher qrWatcher;

    private EventWaitHandle wakeEvent = null;
    private EventWaitHandle pingEvent = null;
    private Thread wakeThread = null;

    private string baseDir;
    private string configJsonPath;
    private string configJsPath;
    private string qrCodePath = "";

    public TrayApplicationContext()
    {
        try
        {
            Program.LogLifecycle("TrayApplicationContext constructor started");
            baseDir = AppDomain.CurrentDomain.BaseDirectory;
            configJsonPath = Path.Combine(baseDir, "config.json");
            configJsPath = Path.Combine(baseDir, "config.js");
            Program.LogLifecycle("Step 1: Paths initialized");

            logForm = new LogViewerForm();
            IntPtr ensureHandle = logForm.Handle;
            Program.LogLifecycle("Step 2: LogViewerForm created, Handle=" + ensureHandle);

            InitializeTrayIcon();
            Program.LogLifecycle("Step 3: TrayIcon initialized");

            StartWakeListener();
            Program.LogLifecycle("Step 4: WakeListener started");

            StartQRWatcher();
            Program.LogLifecycle("Step 5: QRWatcher started");

            AppConfig config = AppConfig.Load(configJsonPath);
            qrCodePath = Path.Combine(baseDir, "qr_code_" + config.SessionId + ".png");
            Program.LogLifecycle("Step 6: Config loaded");

            StartBot();
            Program.LogLifecycle("Step 7: StartBot completed");

            ShowLogs();

            ModernToastNotification.Show(
                "WhatsApp Photo Manager",
                "Bot is running. Right-click the system tray icon anytime for settings.",
                ToastType.Info,
                () => ShowLogs()
            );
            Program.LogLifecycle("Step 8: Toast shown");
            Program.LogLifecycle("TrayApplicationContext constructor finished");
        }
        catch (Exception ex)
        {
            Program.LogLifecycle("TrayApplicationContext constructor ERROR: " + ex.ToString());
            throw;
        }
    }

    private void StartWakeListener()
    {
        try
        {
            wakeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, AlreadyRunningDialog.WAKE_EVENT_NAME);
            pingEvent = new EventWaitHandle(false, EventResetMode.AutoReset, AlreadyRunningDialog.PING_EVENT_NAME);

            wakeThread = new Thread(() => {
                WaitHandle[] handles = new WaitHandle[] { wakeEvent, pingEvent };
                while (true)
                {
                    try
                    {
                        int index = WaitHandle.WaitAny(handles);
                        if (index == 0) // wakeEvent: Bring window to front
                        {
                            Program.LogLifecycle("WakeListener received wakeEvent! Bringing active window to foreground.");
                            if (logForm != null && !logForm.IsDisposed)
                            {
                                logForm.BeginInvoke(new Action(() => {
                                    if (qrForm != null && !qrForm.IsDisposed && qrForm.Visible)
                                    {
                                        WindowHelper.ForceForeground(qrForm);
                                    }
                                    else
                                    {
                                        ShowLogs();
                                    }

                                    ModernToastNotification.Show(
                                        "WhatsApp Photo Manager",
                                        "Application window restored to foreground.",
                                        ToastType.Info,
                                        () => ShowLogs()
                                    );
                                }));
                            }
                        }
                        else if (index == 1) // pingEvent: Pulse notification
                        {
                            if (logForm != null && !logForm.IsDisposed)
                            {
                                logForm.BeginInvoke(new Action(() => {
                                    ModernToastNotification.Show(
                                        "WhatsApp Photo Manager",
                                        "App is active right here in your system tray.",
                                        ToastType.Info,
                                        () => ShowLogs()
                                    );
                                }));
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Wake listener error: " + ex.Message);
                        Thread.Sleep(1000);
                    }
                }
            });
            wakeThread.IsBackground = true;
            wakeThread.Start();
        }
        catch (Exception ex)
        {
            Program.LogLifecycle("Failed to start wake listener: " + ex.Message);
        }
    }

    private void InitializeTrayIcon()
    {
        contextMenu = new ContextMenuStrip();
        contextMenu.Font = new Font("Segoe UI", 9F);
        
        menuHeader = new ToolStripMenuItem("WhatsApp Photo Manager v1.0.0") { Enabled = false, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        menuStatus = new ToolStripMenuItem("Status: Stopped") { Enabled = false };
        
        menuToggle = new ToolStripMenuItem("Start Bot", null, ToggleBot_Click);
        menuSettings = new ToolStripMenuItem("Settings...", null, Settings_Click);
        menuLogs = new ToolStripMenuItem("View Live Logs", null, Logs_Click);
        menuQR = new ToolStripMenuItem("Link WhatsApp (QR Code)", null, QR_Click) { Enabled = false };
        menuDownloads = new ToolStripMenuItem("Open Downloads Folder", null, Downloads_Click);
        
        menuExit = new ToolStripMenuItem("Exit App", null, Exit_Click);

        contextMenu.Items.AddRange(new ToolStripItem[] {
            menuHeader,
            menuStatus,
            new ToolStripSeparator(),
            menuToggle,
            menuSettings,
            menuLogs,
            menuQR,
            menuDownloads,
            new ToolStripSeparator(),
            menuExit
        });

        notifyIcon = new NotifyIcon()
        {
            Icon = Program.AppIcon,
            ContextMenuStrip = contextMenu,
            Text = "WhatsApp Photo Manager v1.0.0 - Bot is offline",
            Visible = true
        };
        
        notifyIcon.DoubleClick += (s, e) => ShowLogs();
    }

    private void StartQRWatcher()
    {
        qrWatcher = new FileSystemWatcher(baseDir, "qr_code_*.png");
        qrWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
        qrWatcher.Created += (s, e) => ShowQR(e.FullPath);
        qrWatcher.Changed += (s, e) => ShowQR(e.FullPath);
        qrWatcher.Deleted += QrWatcher_Deleted;
        qrWatcher.EnableRaisingEvents = true;
    }

    private void ShowQR(string path = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(path))
            {
                qrCodePath = path;
            }
            if (string.IsNullOrEmpty(qrCodePath) || !File.Exists(qrCodePath))
            {
                AppConfig config = AppConfig.Load(configJsonPath);
                string candidate = Path.Combine(baseDir, "qr_code_" + config.SessionId + ".png");
                if (File.Exists(candidate))
                {
                    qrCodePath = candidate;
                }
                else
                {
                    string fallback = Path.Combine(baseDir, "qr_code.png");
                    if (File.Exists(fallback))
                    {
                        qrCodePath = fallback;
                    }
                }
            }

            if (!File.Exists(qrCodePath)) return;

            logForm.AppendLog("[Bot Info] Displaying WhatsApp QR Code: " + Path.GetFileName(qrCodePath) + Environment.NewLine);

            if (logForm != null && logForm.IsHandleCreated && !logForm.IsDisposed)
            {
                logForm.BeginInvoke(new Action(() => {
                    menuQR.Enabled = true;
                    if (qrForm == null || qrForm.IsDisposed)
                    {
                        qrForm = new QrCodeForm(qrCodePath);
                    }
                    WindowHelper.ForceForeground(qrForm);
                }));
            }
        }
        catch {}
    }

    private bool hasShownConnectedToast = false;

    private void CheckForQrDismissal(string line)
    {
        if (string.IsNullOrEmpty(line)) return;

        bool isAuthEvent = line.Contains("QR code scanned") ||
                           line.Contains("Loading your chats") ||
                           line.Contains("Loading session") ||
                           line.Contains("Authenticated") ||
                           line.Contains("Client is ready") ||
                           line.Contains("Main account:") ||
                           line.Contains("Authentication complete");

        if (isAuthEvent)
        {
            DismissQrAndNotifyConnected();
        }
    }

    private void DismissQrAndNotifyConnected()
    {
        try
        {
            if (logForm != null && logForm.IsHandleCreated && !logForm.IsDisposed)
            {
                logForm.BeginInvoke(new Action(() => {
                    menuQR.Enabled = false;
                    if (qrForm != null && !qrForm.IsDisposed)
                    {
                        qrForm.Close();
                        qrForm = null;
                    }

                    if (!hasShownConnectedToast)
                    {
                        hasShownConnectedToast = true;
                        ModernToastNotification.Show(
                            "WhatsApp Connected",
                            "Your WhatsApp account was successfully linked! Photo Manager is active.",
                            ToastType.Success,
                            () => ShowLogs()
                        );
                    }
                }));
            }
        }
        catch {}
    }

    private void QrWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
        logForm.AppendLog("[Bot Info] WhatsApp linked successfully. QR code removed." + Environment.NewLine);
        DismissQrAndNotifyConnected();
    }

    private void ToggleBot_Click(object sender, EventArgs e)
    {
        if (botProcess == null || botProcess.HasExited)
        {
            StartBot();
        }
        else
        {
            StopBot();
        }
    }

    private void StartBot()
    {
        string nodePath = Path.Combine(baseDir, "node.exe");
        string scriptPath = Path.Combine(baseDir, "app", "dist", "index.js");

        if (!File.Exists(nodePath))
        {
            logForm.AppendLog("[Error] node.exe not found at: " + nodePath + Environment.NewLine);
            ModernMessageBox.Show("Could not find portable node.exe in the installation folder.", "Error Starting Bot", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!File.Exists(scriptPath))
        {
            logForm.AppendLog("[Error] App script not found at: " + scriptPath + Environment.NewLine);
            ModernMessageBox.Show("Could not find application script at: app/dist/index.js.\nEnsure the project is correctly built.", "Error Starting Bot", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        AppConfig config = AppConfig.Load(configJsonPath);
        qrCodePath = Path.Combine(baseDir, "qr_code_" + config.SessionId + ".png");
        hasShownConnectedToast = false;

        logForm.AppendLog("[System] Starting WhatsApp Photo Manager bot..." + Environment.NewLine);

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = nodePath;
        startInfo.Arguments = "\"" + scriptPath + "\"";
        startInfo.WorkingDirectory = baseDir;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.StandardOutputEncoding = Encoding.UTF8;
        startInfo.StandardErrorEncoding = Encoding.UTF8;
        startInfo.EnvironmentVariables["PATH"] = baseDir + ";" + Environment.GetEnvironmentVariable("PATH");

        try
        {
            botProcess = new Process();
            botProcess.StartInfo = startInfo;
            botProcess.EnableRaisingEvents = true;
            botProcess.Exited += BotProcess_Exited;

            botProcess.Start();

            outputThread = new Thread(ReadOutput);
            outputThread.IsBackground = true;
            outputThread.Start();

            errorThread = new Thread(ReadError);
            errorThread.IsBackground = true;
            errorThread.Start();

            menuStatus.Text = "Status: Running";
            menuToggle.Text = "Stop Bot";
            notifyIcon.Text = "WhatsApp Photo Manager v1.0.0 - Bot is online";
            logForm.SetBotOnlineStatus(true);
            
            if (File.Exists(qrCodePath))
            {
                menuQR.Enabled = true;
                if (qrForm == null || qrForm.IsDisposed)
                {
                    qrForm = new QrCodeForm(qrCodePath);
                    qrForm.Show();
                }
            }
        }
        catch (Exception ex)
        {
            logForm.AppendLog("[Error] Failed to start process: " + ex.Message + Environment.NewLine);
            ModernMessageBox.Show("Failed to launch background Node process: " + ex.Message, "Error Starting Bot", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StopBot()
    {
        if (botProcess != null && !botProcess.HasExited)
        {
            logForm.AppendLog("[System] Stopping WhatsApp Photo Manager bot..." + Environment.NewLine);
            KillProcessTree(botProcess.Id);
            botProcess = null;
        }
    }

    private void KillProcessTree(int pid)
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo("taskkill", "/F /T /PID " + pid);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi).WaitForExit();
        }
        catch (Exception ex)
        {
            logForm.AppendLog("[System] taskkill failed: " + ex.Message + Environment.NewLine);
        }
    }

    private void BotProcess_Exited(object sender, EventArgs e)
    {
        try
        {
            logForm.AppendLog("[System] Bot process has stopped." + Environment.NewLine);
        }
        catch {}
        botProcess = null;

        try
        {
            if (logForm != null && logForm.IsHandleCreated && !logForm.IsDisposed)
            {
                logForm.BeginInvoke(new Action(UpdateBotStoppedUI));
            }
            else
            {
                UpdateBotStoppedUI();
            }
        }
        catch {}
    }

    private void UpdateBotStoppedUI()
    {
        menuStatus.Text = "Status: Stopped";
        menuToggle.Text = "Start Bot";
        menuQR.Enabled = false;
        notifyIcon.Text = "WhatsApp Photo Manager v1.0.0 - Bot is offline";
        logForm.SetBotOnlineStatus(false);
        
        if (qrForm != null && !qrForm.IsDisposed)
        {
            qrForm.Close();
            qrForm = null;
        }
    }

    private string CleanAnsiCodes(string line)
    {
        if (string.IsNullOrEmpty(line)) return line;
        try
        {
            line = Regex.Replace(line, @"\x1B\[[0-9;]*[a-zA-Z]", "");
            line = Regex.Replace(line, @"\[[0-9;]+m", "");
        }
        catch {}
        return line;
    }

    private void ReadOutput()
    {
        try
        {
            while (botProcess != null && !botProcess.StandardOutput.EndOfStream)
            {
                string line = botProcess.StandardOutput.ReadLine();
                if (line != null)
                {
                    line = CleanAnsiCodes(line);
                    if (IsQrOrTableArt(line)) continue;

                    CheckForQrDismissal(line);
                    if (line.Contains("QR code saved as:") || line.Contains("qr_code"))
                    {
                        ShowQR();
                    }
                    logForm.AppendLog(line + Environment.NewLine);
                    
                    try
                    {
                        File.AppendAllText(Path.Combine(baseDir, "bot_output.log"), "[" + DateTime.Now + "] " + line + Environment.NewLine);
                    }
                    catch { }
                }
            }
        }
        catch { }
    }

    private void ReadError()
    {
        try
        {
            while (botProcess != null && !botProcess.StandardError.EndOfStream)
            {
                string line = botProcess.StandardError.ReadLine();
                if (line != null)
                {
                    line = CleanAnsiCodes(line);
                    if (IsQrOrTableArt(line)) continue;

                    CheckForQrDismissal(line);
                    if (line.Contains("First QR") || line.Contains("Authenticate to continue"))
                    {
                        ShowQR();
                    }

                    // wa-automate outputs internal lifecycle & telemetry lines to stderr.
                    // Only label as [Error] if it is an actual error/exception.
                    string formatted;
                    string lower = line.ToLowerInvariant();
                    bool isRealError = lower.Contains("error:") || 
                                       lower.Contains("exception:") || 
                                       lower.Contains("unhandled") || 
                                       lower.Contains("fatal") || 
                                       lower.Contains("err_") || 
                                       lower.Contains("failed to");

                    if (isRealError)
                    {
                        formatted = "[Error] " + line + Environment.NewLine;
                    }
                    else if (line.Contains("Launch inject:") || line.Contains("Page loaded in") || 
                             line.Contains("Use this easy") || line.Contains("Time to inject") || 
                             line.Contains("WAPI injected") || line.Contains("First QR") ||
                             line.Contains("Found require") || line.Contains("Injecting") ||
                             line.Contains("Base inject") || line.Contains("labels=MD") ||
                             line.Contains("AUTHENTICATED") || line.Contains("STARTING"))
                    {
                        formatted = "[Diagnostics] " + line + Environment.NewLine;
                    }
                    else
                    {
                        formatted = "[Info] " + line.TrimStart('-', ' ') + Environment.NewLine;
                    }

                    logForm.AppendLog(formatted);
                    try
                    {
                        File.AppendAllText(Path.Combine(baseDir, "bot_output.log"), "[" + DateTime.Now + "] " + formatted);
                    }
                    catch { }
                }
            }
        }
        catch { }
    }

    private bool IsQrOrTableArt(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return false;

        foreach (char c in line)
        {
            if (c >= '\u2500' && c <= '\u259F') return true;
        }

        return false;
    }

    private void Settings_Click(object sender, EventArgs e)
    {
        using (SettingsForm form = new SettingsForm(configJsonPath))
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                logForm.AppendLog("[System] Settings updated. Restarting bot to apply configuration..." + Environment.NewLine);
                if (botProcess != null && !botProcess.HasExited)
                {
                    StopBot();
                    Thread.Sleep(1500);
                    StartBot();
                }
            }
        }
    }

    private void Logs_Click(object sender, EventArgs e)
    {
        ShowLogs();
    }

    private void ShowLogs()
    {
        if (logForm == null || logForm.IsDisposed) return;
        WindowHelper.ForceForeground(logForm);
    }

    private void QR_Click(object sender, EventArgs e)
    {
        ShowQR();
        if (qrForm == null || !qrForm.Visible)
        {
            ModernMessageBox.Show("No active QR code waiting for scan.\nIf the bot is already authenticated or starting up, the QR code is not needed.", "Device Already Linked", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void Downloads_Click(object sender, EventArgs e)
    {
        AppConfig config = AppConfig.Load(configJsonPath);
        string downloadsPath = config.DownloadsDir;

        if (!Path.IsPathRooted(downloadsPath))
        {
            downloadsPath = Path.Combine(baseDir, downloadsPath);
        }

        try
        {
            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }
            Process.Start("explorer.exe", "\"" + downloadsPath + "\"");
        }
        catch (Exception ex)
        {
            ModernMessageBox.Show("Could not open downloads directory: " + ex.Message, "Error Opening Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Exit_Click(object sender, EventArgs e)
    {
        ExitApp();
    }

    private void ExitApp()
    {
        StopBot();

        if (wakeThread != null)
        {
            try { wakeThread.Abort(); } catch {}
            wakeThread = null;
        }
        if (wakeEvent != null)
        {
            try { wakeEvent.Close(); } catch {}
            wakeEvent = null;
        }
        if (pingEvent != null)
        {
            try { pingEvent.Close(); } catch {}
            pingEvent = null;
        }
        
        if (qrWatcher != null)
        {
            qrWatcher.EnableRaisingEvents = false;
            qrWatcher.Dispose();
        }

        if (notifyIcon != null)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }

        Application.Exit();
    }

    protected override void OnMainFormClosed(object sender, EventArgs e)
    {
        // Prevent WinForms from terminating the ApplicationContext when a toast or dialog closes
        Program.LogLifecycle("OnMainFormClosed invoked for: " + (sender != null ? sender.GetType().Name : "null") + " (ignored to keep tray app alive)");
    }

    protected override void ExitThreadCore()
    {
        Program.LogLifecycle("ExitThreadCore invoked:\n" + Environment.StackTrace);
        base.ExitThreadCore();
    }
}

public static class Program
{
    private static Mutex mutex = null;

    public static Icon AppIcon
    {
        get
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDir, "app-logo.ico");
                if (!File.Exists(path))
                {
                    path = Path.Combine(baseDir, "assets", "app-logo.ico");
                }
                if (File.Exists(path))
                {
                    return new Icon(path);
                }
            }
            catch {}

            try
            {
                Icon exeIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (exeIcon != null)
                {
                    return exeIcon;
                }
            }
            catch {}

            return SystemIcons.Application;
        }
    }

    private static readonly object _logLock = new object();
    public static void LogLifecycle(string msg)
    {
        try
        {
            lock (_logLock)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_lifecycle.log");
                File.AppendAllText(path, "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + msg + Environment.NewLine);
            }
        }
        catch {}
    }

    [STAThread]
    public static void Main()
    {
        try
        {
            LogLifecycle("Entered Main");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (s, ev) => {
                LogLifecycle("ThreadException: " + ev.Exception.ToString());
            };
            AppDomain.CurrentDomain.UnhandledException += (s, ev) => {
                LogLifecycle("UnhandledException: " + ev.ExceptionObject.ToString());
            };
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => {
                LogLifecycle("ProcessExit fired:\n" + Environment.StackTrace);
            };

            bool createdNew = false;
            try
            {
                mutex = new Mutex(true, "Local\\WhatsAppPhotoManagerTrayAppMutex", out createdNew);
            }
            catch (AbandonedMutexException)
            {
                createdNew = true;
            }

            LogLifecycle("Mutex checked, createdNew=" + createdNew);

            if (!createdNew)
            {
                LogLifecycle("Another instance already running. Signaling primary instance to restore window.");
                AlreadyRunningDialog.SignalPrimaryWake();
                Thread.Sleep(300);
                return;
            }

            LogLifecycle("Calling Application.Run(new TrayApplicationContext())");
            Application.Run(new TrayApplicationContext());
            LogLifecycle("Application.Run returned normally.");
        }
        catch (Exception ex)
        {
            LogLifecycle("Catch in Main: " + ex.ToString());
            ModernMessageBox.Show("Fatal Startup Error:\n" + ex.Message, "WhatsApp Photo Manager Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
