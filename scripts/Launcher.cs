using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        // Defaults matching config.js (AllowedNumbers empty by default)
        PhotoCategories.Add("4x6 Size Photo");
        PhotoCategories.Add("A4 Photo Frame");
        PhotoCategories.Add("Polaroid Photo");
        PhotoCategories.Add("18x24 Banner");
    }

    public static AppConfig Load(string path)
    {
        AppConfig config = new AppConfig();
        if (!File.Exists(path)) return config;

        try
        {
            string content = File.ReadAllText(path);
            
            // Extract ALLOWED_NUMBERS
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

            // Extract PHOTO_CATEGORIES
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

            // Extract FOLDER_SETTINGS
            Match downloadsMatch = Regex.Match(content, @"\""downloadsDir\""\s*:\s*\""(.*?)\""");
            if (downloadsMatch.Success)
            {
                config.DownloadsDir = downloadsMatch.Groups[1].Value;
            }

            // Extract BOT_CONFIG
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
            MessageBox.Show("Error loading config.json: " + ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error saving config.json: " + ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    // Basic Settings Controls
    private Label lblDownloads;
    private TextBox txtDownloads;
    private Button btnBrowse;
    private Label lblNumbers;
    private TextBox txtNumbers;

    // Category Controls
    private ListBox lstCategories;
    private TextBox txtNewCategory;
    private Button btnAddCategory;
    private Button btnRemoveCategory;

    // Advanced Bot Controls
    private Label lblSessionId;
    private TextBox txtSessionId;
    private CheckBox chkHeadless;
    private CheckBox chkMultiDevice;

    // Global Buttons
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
        this.Size = new Size(500, 520);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.Icon = Program.AppIcon;

        // Tab Control
        tabControl = new TabControl();
        tabControl.Bounds = new Rectangle(12, 12, 460, 400);

        tabBasic = new TabPage("General Settings");
        tabCategories = new TabPage("Photo Categories");
        tabAdvanced = new TabPage("Connection Settings");

        tabControl.TabPages.Add(tabBasic);
        tabControl.TabPages.Add(tabCategories);
        tabControl.TabPages.Add(tabAdvanced);

        // --- TAB 1: BASIC SETTINGS ---
        lblDownloads = new Label() { Text = "Downloads Storage Directory:", Location = new Point(15, 20), Size = new Size(300, 20) };
        txtDownloads = new TextBox() { Text = config.DownloadsDir, Location = new Point(15, 42), Size = new Size(320, 25) };
        btnBrowse = new Button() { Text = "Browse...", Location = new Point(345, 41), Size = new Size(80, 27) };
        btnBrowse.Click += BtnBrowse_Click;

        lblNumbers = new Label() { Text = "Authorized WhatsApp Contacts / Phone Numbers (one per line):", Location = new Point(15, 90), Size = new Size(400, 20) };
        txtNumbers = new TextBox()
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = false,
            Location = new Point(15, 112),
            Size = new Size(410, 220),
            Text = string.Join(Environment.NewLine, config.AllowedNumbers)
        };

        tabBasic.Controls.AddRange(new Control[] { lblDownloads, txtDownloads, btnBrowse, lblNumbers, txtNumbers });

        // --- TAB 2: CATEGORIES ---
        lstCategories = new ListBox() { Location = new Point(15, 20), Size = new Size(260, 310) };
        foreach (var cat in config.PhotoCategories) lstCategories.Items.Add(cat);

        txtNewCategory = new TextBox() { Location = new Point(290, 20), Size = new Size(140, 25) };
        btnAddCategory = new Button() { Text = "Add Category", Location = new Point(290, 52), Size = new Size(140, 30) };
        btnAddCategory.Click += BtnAddCategory_Click;

        btnRemoveCategory = new Button() { Text = "Remove Selected", Location = new Point(290, 92), Size = new Size(140, 30) };
        btnRemoveCategory.Click += BtnRemoveCategory_Click;

        Label lblCatHint = new Label() 
        { 
            Text = "Note: Base directories for these categories will be automatically created inside your download folder.", 
            Location = new Point(290, 140), 
            Size = new Size(145, 180),
            ForeColor = Color.DimGray
        };

        tabCategories.Controls.AddRange(new Control[] { lstCategories, txtNewCategory, btnAddCategory, btnRemoveCategory, lblCatHint });

        // --- TAB 3: ADVANCED ---
        lblSessionId = new Label() { Text = "WhatsApp Session Identifier:", Location = new Point(15, 20), Size = new Size(200, 20) };
        txtSessionId = new TextBox() { Text = config.SessionId, Location = new Point(15, 42), Size = new Size(410, 25) };

        chkHeadless = new CheckBox() 
        { 
            Text = "Run Browser in Background (Headless Mode)", 
            Checked = config.Headless, 
            Location = new Point(15, 95), 
            Size = new Size(410, 25) 
        };

        chkMultiDevice = new CheckBox() 
        { 
            Text = "Enable WhatsApp Multi-Device Beta Support", 
            Checked = config.MultiDevice, 
            Location = new Point(15, 130), 
            Size = new Size(410, 25) 
        };

        tabAdvanced.Controls.AddRange(new Control[] { lblSessionId, txtSessionId, chkHeadless, chkMultiDevice });

        // --- GLOBAL ACTIONS ---
        btnSave = new Button() { Text = "Save Settings", Location = new Point(290, 430), Size = new Size(100, 35) };
        btnSave.Click += BtnSave_Click;

        btnCancel = new Button() { Text = "Cancel", Location = new Point(400, 430), Size = new Size(70, 35) };
        btnCancel.Click += BtnCancel_Click;

        this.Controls.AddRange(new Control[] { tabControl, btnSave, btnCancel });
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
            MessageBox.Show("Category already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Parse Allowed Numbers (skipping digits-only validation since WhatsApp supports usernames and characters in IDs)
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

        // Parse Categories
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
    private Button btnClear;
    private Panel panelBottom;

    public LogViewerForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "WhatsApp Photo Manager v1.0.0 - Live Console Logs";
        this.Size = new Size(700, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Icon = Program.AppIcon;

        txtLog = new TextBox()
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = true,
            Font = new Font("Consolas", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
            BackColor = Color.Black,
            ForeColor = Color.LightGray,
            Dock = DockStyle.Fill
        };

        panelBottom = new Panel()
        {
            Height = 45,
            Dock = DockStyle.Bottom,
            BackColor = Color.FromArgb(240, 240, 240)
        };

        btnClear = new Button()
        {
            Text = "Clear Logs",
            Location = new Point(12, 8),
            Size = new Size(100, 30)
        };
        btnClear.Click += (s, e) => txtLog.Clear();
        panelBottom.Controls.Add(btnClear);

        this.Controls.Add(txtLog);
        this.Controls.Add(panelBottom);

        this.FormClosing += LogViewerForm_FormClosing;
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

        if (this.InvokeRequired)
        {
            this.BeginInvoke(new Action<string>(AppendLog), text);
            return;
        }

        // Keep character limit to avoid memory bloat
        if (txtLog.TextLength > 500000)
        {
            txtLog.Text = txtLog.Text.Substring(200000);
        }

        txtLog.AppendText(text);
    }
}

public class QrCodeForm : Form
{
    private PictureBox pictureBox;
    private string imagePath;
    private FileSystemWatcher watcher;

    public QrCodeForm(string qrImagePath)
    {
        this.imagePath = qrImagePath;
        InitializeComponent();
        LoadQRImage();
        StartWatcher();
    }

    private void InitializeComponent()
    {
        this.Text = "Scan WhatsApp QR Code";
        this.Size = new Size(330, 360);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Icon = Program.AppIcon;
        this.TopMost = true;

        pictureBox = new PictureBox()
        {
            Location = new Point(15, 15),
            Size = new Size(280, 280),
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle
        };

        Label lblHelp = new Label()
        {
            Text = "Open WhatsApp on your phone -> Settings -> Linked Devices -> Scan QR Code.",
            Location = new Point(15, 300),
            Size = new Size(280, 30),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.DarkSlateGray
        };

        this.Controls.AddRange(new Control[] { pictureBox, lblHelp });
        this.FormClosed += QrCodeForm_FormClosed;
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
            if (File.Exists(imagePath))
            {
                // Read into MemoryStream to avoid locking the physical file!
                byte[] bytes = File.ReadAllBytes(imagePath);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    pictureBox.Image = Image.FromStream(ms);
                }
            }
            else
            {
                pictureBox.Image = null;
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

    private string baseDir;
    private string configJsonPath;
    private string configJsPath;
    private string qrCodePath = "";

    public TrayApplicationContext()
    {
        baseDir = AppDomain.CurrentDomain.BaseDirectory;
        configJsonPath = Path.Combine(baseDir, "config.json");
        configJsPath = Path.Combine(baseDir, "config.js");

        // Set up double-clicking and form instances
        logForm = new LogViewerForm();

        InitializeTrayIcon();
        StartQRWatcher();

        // Load config and set session-based QR name
        AppConfig config = AppConfig.Load(configJsonPath);
        qrCodePath = Path.Combine(baseDir, "qr_code_" + config.SessionId + ".png");

        // Automatically start the bot on launch
        StartBot();

        // Show the log viewer form automatically on startup
        ShowLogs();
    }

    private void InitializeTrayIcon()
    {
        contextMenu = new ContextMenuStrip();
        
        menuHeader = new ToolStripMenuItem("WhatsApp Photo Manager v1.0.0") { Enabled = false, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        menuStatus = new ToolStripMenuItem("Status: Stopped") { Enabled = false };
        
        menuToggle = new ToolStripMenuItem("Start Bot", null, ToggleBot_Click);
        menuSettings = new ToolStripMenuItem("Configure Settings...", null, Settings_Click);
        menuLogs = new ToolStripMenuItem("View Console Logs", null, Logs_Click);
        menuQR = new ToolStripMenuItem("Show Scan QR Code", null, QR_Click) { Enabled = false };
        menuDownloads = new ToolStripMenuItem("Open Downloads Storage", null, Downloads_Click);
        
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
        // Monitor current folder for QR codes matching any session ID
        qrWatcher = new FileSystemWatcher(baseDir, "qr_code_*.png");
        qrWatcher.NotifyFilter = NotifyFilters.FileName;
        qrWatcher.Created += QrWatcher_Created;
        qrWatcher.Deleted += QrWatcher_Deleted;
        qrWatcher.EnableRaisingEvents = true;
    }

    private void QrWatcher_Created(object sender, FileSystemEventArgs e)
    {
        // Update current QR path and open window
        qrCodePath = e.FullPath;
        
        this.logForm.AppendLog("[C# Tray] QR code generated on disk: " + e.Name + Environment.NewLine);
        
        // Open QR Code dialog dynamically
        this.logForm.BeginInvoke(new Action(() => {
            menuQR.Enabled = true;
            if (qrForm == null || qrForm.IsDisposed)
            {
                qrForm = new QrCodeForm(qrCodePath);
                qrForm.Show();
            }
        }));
    }

    private void QrWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
        this.logForm.AppendLog("[C# Tray] QR code removed from disk." + e.Name + Environment.NewLine);
        
        this.logForm.BeginInvoke(new Action(() => {
            menuQR.Enabled = false;
            if (qrForm != null && !qrForm.IsDisposed)
            {
                qrForm.Close();
                qrForm = null;
            }
        }));
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
            MessageBox.Show("Could not find portable node.exe in the installation folder.", "Error Starting Bot", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!File.Exists(scriptPath))
        {
            logForm.AppendLog("[Error] App script not found at: " + scriptPath + Environment.NewLine);
            MessageBox.Show("Could not find application script at: app/dist/index.js.\nEnsure the project is correctly built.", "Error Starting Bot", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Dynamically get session-based QR name from config.json
        AppConfig config = AppConfig.Load(configJsonPath);
        qrCodePath = Path.Combine(baseDir, "qr_code_" + config.SessionId + ".png");

        logForm.AppendLog("[C# Tray] Starting WhatsApp Photo Manager bot..." + Environment.NewLine);

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

        try
        {
            botProcess = new Process();
            botProcess.StartInfo = startInfo;
            botProcess.EnableRaisingEvents = true;
            botProcess.Exited += BotProcess_Exited;

            botProcess.Start();

            // Spawning threads to read standard output & error streams
            outputThread = new Thread(ReadOutput);
            outputThread.IsBackground = true;
            outputThread.Start();

            errorThread = new Thread(ReadError);
            errorThread.IsBackground = true;
            errorThread.Start();

            menuStatus.Text = "Status: Running";
            menuToggle.Text = "Stop Bot";
            notifyIcon.Text = "WhatsApp Photo Manager v1.0.0 - Bot is online";
            
            // Check if QR code file is already on disk (leftovers)
            if (File.Exists(qrCodePath))
            {
                menuQR.Enabled = true;
                
                // Automatically display the QR form on startup if it exists
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
            MessageBox.Show("Failed to launch background Node process: " + ex.Message, "Error Starting Bot", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StopBot()
    {
        if (botProcess != null && !botProcess.HasExited)
        {
            logForm.AppendLog("[C# Tray] Stopping WhatsApp Photo Manager bot..." + Environment.NewLine);
            
            // Forcefully terminate process and all of its subprocesses (Chromium, Puppeteer)
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
            logForm.AppendLog("[C# Tray] taskkill failed: " + ex.Message + Environment.NewLine);
        }
    }

    private void BotProcess_Exited(object sender, EventArgs e)
    {
        logForm.AppendLog("[C# Tray] Bot process exited." + Environment.NewLine);
        
        botProcess = null;

        // Perform UI updates on form thread
        logForm.BeginInvoke(new Action(() => {
            menuStatus.Text = "Status: Stopped";
            menuToggle.Text = "Start Bot";
            menuQR.Enabled = false;
            notifyIcon.Text = "WhatsApp Photo Manager v1.0.0 - Bot is offline";
            
            if (qrForm != null && !qrForm.IsDisposed)
            {
                qrForm.Close();
                qrForm = null;
            }
        }));
    }

    private string CleanAnsiCodes(string line)
    {
        if (string.IsNullOrEmpty(line)) return line;
        try
        {
            // Strip standard ANSI escape sequences (e.g. ESC[91m)
            line = Regex.Replace(line, @"\x1B\[[0-9;]*[a-zA-Z]", "");
            // Strip raw/mangled color codes (e.g. [91m)
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

                    // Write to live textbox logs
                    logForm.AppendLog(line + Environment.NewLine);
                    
                    // Write to local file logs
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

                    logForm.AppendLog("[Error Log] " + line + Environment.NewLine);
                    try
                    {
                        File.AppendAllText(Path.Combine(baseDir, "bot_output.log"), "[" + DateTime.Now + "] [ERR] " + line + Environment.NewLine);
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

        // Check for standard UTF-8 box-drawing and block elements
        foreach (char c in line)
        {
            // Box drawing: U+2500 to U+257F, Block elements: U+2580 to U+259F
            if ((c >= 0x2500 && c <= 0x257F) || (c >= 0x2580 && c <= 0x259F))
            {
                return true;
            }
        }

        // Check for common mangled UTF-8 characters under ANSI shell (like â”, â–, â–▄, â–█, â–¬)
        if (line.Contains("â”") || line.Contains("â–") || line.Contains("â–▄") || line.Contains("â–█") || line.Contains("â–¬"))
        {
            return true;
        }

        // Check if line is a banner consisting only of slashes, backslashes, underscores, and spaces
        string stripped = Regex.Replace(line, @"[\s_/\\]", "");
        if (stripped.Length == 0)
        {
            return true;
        }

        return false;
    }

    private void Settings_Click(object sender, EventArgs e)
    {
        using (SettingsForm form = new SettingsForm(configJsonPath))
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                logForm.AppendLog("[C# Tray] Settings updated. Restarting bot to apply configuration..." + Environment.NewLine);
                if (botProcess != null && !botProcess.HasExited)
                {
                    StopBot();
                    // Wait a moment for processes to free up
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
        if (logForm.Visible)
        {
            logForm.Activate();
        }
        else
        {
            logForm.Show();
        }
    }

    private void QR_Click(object sender, EventArgs e)
    {
        if (File.Exists(qrCodePath))
        {
            if (qrForm == null || qrForm.IsDisposed)
            {
                qrForm = new QrCodeForm(qrCodePath);
                qrForm.Show();
            }
            else
            {
                qrForm.Activate();
            }
        }
        else
        {
            MessageBox.Show("No active QR code found. If the bot is already authenticated or is starting up, the QR code will not be available.", "QR Code Offline", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void Downloads_Click(object sender, EventArgs e)
    {
        AppConfig config = AppConfig.Load(configJsonPath);
        string downloadsPath = config.DownloadsDir;

        // If path is relative, resolve it relative to base directory
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
            MessageBox.Show("Could not open downloads directory: " + ex.Message, "Error Opening Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Exit_Click(object sender, EventArgs e)
    {
        ExitApp();
    }

    private void ExitApp()
    {
        StopBot();
        
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
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app-logo.ico");
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

    [STAThread]
    public static void Main()
    {
        bool createdNew;
        mutex = new Mutex(true, "Global\\WhatsAppPhotoManagerTrayAppMutex", out createdNew);

        if (!createdNew)
        {
            MessageBox.Show("WhatsApp Photo Manager is already running in the system tray.", "Already Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        Application.Run(new TrayApplicationContext());
    }
}
