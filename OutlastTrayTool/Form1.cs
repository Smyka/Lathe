using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace OutlastTrayTool
{
    public partial class Form1 : Form
    {
        private ModManager modManager;
        private Config config;
        private GameManager gameManager;
        private DiscordPresenceLoop presence;
        private ModManagerAPI modManagerAPI;
        private NotifyIcon trayIcon;
        private bool startupLaunch = false;
        private bool realExit = false;
        private string currentVersion = "0.2.1";

        public Form1()
        {

            InitializeComponent();
            Load += async (_, __) => await InitializeAsync();
        }
        private async Task InitializeAsync()
        {
            startupLaunch = Environment.GetCommandLineArgs()
                .Any(arg => arg.Equals("--startup", StringComparison.OrdinalIgnoreCase));

            config = new Config();

            gameManager = new GameManager(config);
            modManager = new ModManager(config);

            modManagerAPI = new ModManagerAPI();

            await CheckModUpdatesAsync();

            modManager.SetRefreshUiAction(() =>
            {
                if (InvokeRequired)
                    Invoke(new Action(LoadCheckboxes));
                else
                    LoadCheckboxes();
            });

            modManager.RefreshMods();
            modManager.StartDownloadWatcher();

            CheckForUpdates();

            LoadPresence();
            LoadTrayIcon();
            LoadFOVSlider();
            LoadScreenPercentageSlider();
            LoadFogComboBox();
            LoadCheckboxes();
            LoadStartupComboBox();
            LoadPresenceComboBox();

            if (startupLaunch)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }

            panel8.Visible = true;
        }

        public async void CheckForUpdates()
        {
            string apiUrl = "https://raw.githubusercontent.com/Smyka/Lathe/refs/heads/master/api/appInformation.json";

            try
            {
                using HttpClient client = new HttpClient();

                var response = await client.GetAsync(apiUrl);

                string result = await response.Content.ReadAsStringAsync();

                dynamic finalResult = JsonConvert.DeserializeObject(result);
                string webVersion = finalResult.version;

                if (currentVersion != webVersion && currentVersion != "unknown")
                {
                    var choiceResult = MessageBox.Show($"An update is available for Lathe version {currentVersion}, click Yes to go to GitHub and download it or No to continue", "Link", MessageBoxButtons.YesNo);
                    if (choiceResult == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://github.com/Smyka/Lathe/releases",
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch
            {
            }
        }

        private void LoadPresence()
        {
            if (config.LoadConfig()["presence"] == "Enabled")
            {
                presence = new DiscordPresenceLoop();
                Task.Run(() => presence.StartLoop());
            }

        }
        private void LoadCheckboxes()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.Add(label4);

            JObject configObj = JObject.Parse(File.ReadAllText(config.configPath));
            JObject modMap = (JObject)configObj["modMap"];
            List<JProperty> modGroups = modMap.Properties()
                .Where(mod => mod.Name != "undefined")
                .ToList();

            JProperty undefinedGroup = modMap.Properties()
                .FirstOrDefault(mod => mod.Name == "undefined");

            if (undefinedGroup != null)
            {
                modGroups.Add(undefinedGroup);
            }

            foreach (JProperty modGroup in modGroups)
            {
                if (modGroup.Value is not JObject modObj)
                    continue;

                string modName = modObj["name"]?.ToString() ?? modGroup.Name;
                string modVersion = modObj["version"]?.ToString() ?? modGroup.Name;
                string update = modObj["update"]?.ToString() ??  modGroup.Name;
                string modId = modGroup.Name;

                if (modGroup.Name == "undefined")
                {
                    modName = "Other";
                }

                JArray files = modObj["files"] as JArray;

                if (files == null || files.Count == 0)
                    continue;

                Label modNameLabel = new Label();
                modNameLabel.Text = modName;
                modNameLabel.AutoSize = true;
                modNameLabel.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
                modNameLabel.ForeColor = Color.Honeydew;
                modNameLabel.Margin = new Padding(3, 10, 0, 3);

                

                flowLayoutPanel1.Controls.Add(modNameLabel);

                if (modGroup.Name != "undefined")
                {
                    int prefixLinkLength = 11 + modVersion.Length; // puts link area after version text
                    LinkLabel versionLabel = new LinkLabel();
                    versionLabel.Text = $"Version {modVersion} - Nexus page";
                    versionLabel.LinkColor = Color.LightBlue;
                    versionLabel.Links.Add(prefixLinkLength, 10, "");
                    versionLabel.AutoSize = true;
                    versionLabel.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
                    versionLabel.ForeColor = Color.Honeydew;
                    versionLabel.Margin = new Padding(5, 0, 3, 0);

                    

                    versionLabel.LinkClicked += (s, ev) =>
                    {
                        string url = ev.Link.LinkData.ToString();
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = $"https://www.nexusmods.com/theoutlasttrials/mods/{modId}",
                            UseShellExecute = true
                        });
                    };

                    flowLayoutPanel1.Controls.Add(versionLabel);
                    if (update == "True")
                    {
                        LinkLabel updateLabel = new LinkLabel();
                        updateLabel.Text = "Update available!";
                        updateLabel.Links.Add(0, 30, "");
                        updateLabel.AutoSize = true;
                        updateLabel.Font = new Font("Segoe UI", 8f, FontStyle.Regular);
                        updateLabel.LinkColor = Color.LightGreen;
                        updateLabel.Margin = new Padding(5, 0, 0, 0);

                        updateLabel.LinkClicked += (s, ev) =>
                        {
                            string url = ev.Link.LinkData.ToString();
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = $"https://www.nexusmods.com/theoutlasttrials/mods/{modId}?tab=files",
                                UseShellExecute = true
                            });
                        };

                        flowLayoutPanel1.Controls.Add(updateLabel);
                    }
                }
                

                

                foreach (JToken fileToken in files)
                {
                    string fileName = fileToken.ToString();

                    bool isEnabled = modManager.enabledMods.Contains(fileName);
                    bool isDisabled = modManager.disabledMods.Contains(fileName);

                    if (!isEnabled && !isDisabled)
                        continue;

                    CheckBox chk = new CheckBox();
                    chk.Text = fileName;
                    chk.Name = "chk_" + fileName;
                    chk.AutoSize = true;
                    chk.Checked = isEnabled;
                    chk.Font = new Font("Segoe UI", 10f);
                    chk.Margin = new Padding(20, 2, 3, 2);

                    chk.CheckedChanged += (s, ev) =>
                    {
                        CheckBox senderChk = (CheckBox)s;

                        if (senderChk.Checked)
                        {
                            modManager.EnableMod(senderChk.Text);
                        }
                        else
                        {
                            modManager.DisableMod(senderChk.Text);
                        }

                        modManager.RefreshMods();
                        LoadCheckboxes();
                    };

                    flowLayoutPanel1.Controls.Add(chk);
                }
            }
        }

        public async Task CheckModUpdatesAsync()
        {
            JObject configObj = JObject.Parse(File.ReadAllText(config.configPath));
            JObject modMap = (JObject)configObj["modMap"];

            bool changed = false;

            foreach (JProperty modGroup in modMap.Properties())
            {
                if (modGroup.Name == "undefined")
                    continue;

                if (modGroup.Value is not JObject modObj)
                    continue;

                string modId = modGroup.Name;
                string localVersion = modObj["version"]?.ToString() ?? "0";

                try
                {
                    dynamic json = await ModManagerAPI.GetModVersion(Convert.ToInt32(modId));
                    string webVersion = json.data.mod.version?.ToString();

                    if (string.IsNullOrWhiteSpace(webVersion))
                        continue;

                    bool hasUpdate = localVersion.Trim() != webVersion.Trim();

                    bool previousValue = modObj["update"]?.Value<bool>() ?? false;

                    modObj["update"] = hasUpdate;

                    if (previousValue != hasUpdate)
                        changed = true;
                }
                catch
                {
                    bool previousValue = modObj["update"]?.Value<bool>() ?? false;

                    modObj["update"] = false;

                    if (previousValue != false)
                        changed = true;
                }
            }

            if (changed)
            {
                File.WriteAllText(
                    config.configPath,
                    configObj.ToString(Formatting.Indented)
                );
            }
        }

        private void LoadFOVSlider()
        {
            textBox2.Text = config.LoadConfig()["FOV"].ToString();
            trackBar1.Value = config.LoadConfig()["FOV"];
        }

        private void LoadScreenPercentageSlider()
        {
            textBox1.Text = config.LoadConfig()["screenPercentage"].ToString();
            trackBar2.Value = config.LoadConfig()["screenPercentage"];
        }

        private void LoadFogComboBox()
        {
            comboBox1.SelectedItem = config.LoadConfig()["fog"].ToString();
        }

        private void LoadPresenceComboBox()
        {
            comboBox2.SelectedItem = config.LoadConfig()["presence"].ToString();
        }

        private void LoadStartupComboBox()
        {
            comboBox3.SelectedItem = config.LoadConfig()["startup"].ToString();
        }


        /*private async void LoadModBrowser() // add page param later
        {
            dynamic json = await ModManagerAPI.GetModPageAsync();

            for (int i = 0; i < 20; i++)
            {
                string name = json.data.mods.nodes[i].name;
                string uploader = json.data.mods.nodes[i].uploader.name;
                string thumbnail = json.data.mods.nodes[i].thumbnailUrl;
                string summary = json.data.mods.nodes[i].summary;
                int downloads = json.data.mods.nodes[i].downloads;
                int endorsements = json.data.mods.nodes[i].endorsements;
                int modId = json.data.mods.nodes[i].modId;
                string modUrl = $"https://www.nexusmods.com/theoutlasttrials/mods/{modId}";
                DateTime updatedAt = json.data.mods.nodes[i].updatedAt;

                Panel panel = new Panel();
                panel.Size = new Size(295, 268);
                panel.Margin = new Padding(5);
                panel.BorderStyle = BorderStyle.FixedSingle;

                FlowLayoutPanel detailsPanel = new FlowLayoutPanel();
                detailsPanel.Size = new Size(200, 30);
                detailsPanel.Location = new Point(3, 210);

                PictureBox thumbnailBox = new PictureBox();
                thumbnailBox.Size = new Size(295, 166);
                thumbnailBox.Location = new Point(0, 0);
                thumbnailBox.SizeMode = PictureBoxSizeMode.Zoom;

                PictureBox downloadIconBox = new PictureBox();
                downloadIconBox.Size = new Size(12, 12);
                downloadIconBox.SizeMode = PictureBoxSizeMode.Zoom;
                downloadIconBox.ImageLocation = @"assets/downloadIcon.png";
                downloadIconBox.LoadAsync();

                PictureBox likeIconBox = new PictureBox();
                likeIconBox.Size = new Size(12, 12);
                likeIconBox.SizeMode = PictureBoxSizeMode.Zoom;
                likeIconBox.ImageLocation = @"assets/likeIcon.png";
                likeIconBox.LoadAsync();

                Label nameLabel = new Label();
                nameLabel.Text = name;
                nameLabel.Font = new Font("Segoe UI", 12f);
                nameLabel.ForeColor = Color.Honeydew;
                nameLabel.Location = new Point(3, 170);
                nameLabel.Size = new Size(274, 22);

                Label uploaderLabel = new Label();
                uploaderLabel.Text = uploader;
                uploaderLabel.Font = new Font("Segoe UI", 9f);
                uploaderLabel.ForeColor = Color.LightGray;
                uploaderLabel.Location = new Point(3, 192);
                nameLabel.Size = new Size(274, 22);

                Label downloadLabel = new Label();
                downloadLabel.Text = downloads.ToString();
                downloadLabel.Font = new Font("Segoe UI", 8f);
                downloadLabel.ForeColor = Color.LightGray;
                downloadLabel.AutoSize = true;
                downloadLabel.Margin = new Padding(0, 2, 5, 3);

                Label likeLabel = new Label();
                likeLabel.Text = endorsements.ToString();
                likeLabel.Font = new Font("Segoe UI", 8f);
                likeLabel.ForeColor = Color.LightGray;
                likeLabel.AutoSize = true;
                likeLabel.Margin = new Padding(0, 2, 5, 3);

                Label updatedAtLabel = new Label();
                updatedAtLabel.Text = updatedAt.ToShortDateString();
                updatedAtLabel.Font = new Font("Segoe UI", 8f);
                updatedAtLabel.ForeColor = Color.LightGray;
                updatedAtLabel.AutoSize = true;
                updatedAtLabel.Margin = new Padding(0, 2, 5, 3);

                Button viewOnNexusButton = new Button();
                viewOnNexusButton.Text = "View on Nexus";
                viewOnNexusButton.Location = new Point(5, 233);
                viewOnNexusButton.FlatStyle = FlatStyle.Flat;
                viewOnNexusButton.Size = new Size(100, 27);
                viewOnNexusButton.BackColor = Color.FromArgb(47, 54, 61);
                viewOnNexusButton.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
                viewOnNexusButton.ForeColor = Color.Honeydew;

                viewOnNexusButton.Click += (s, e) =>
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = modUrl,
                        UseShellExecute = true
                    });
                };

                Button detailsButton = new Button();
                detailsButton.Text = "Details";
                detailsButton.Location = new Point(110, 233);
                detailsButton.FlatStyle = FlatStyle.Flat;
                detailsButton.Size = new Size(70, 27);
                detailsButton.BackColor = Color.FromArgb(47, 54, 61);
                detailsButton.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
                detailsButton.ForeColor = Color.Honeydew;

                detailsButton.Click += (s, e) =>
                {
                    Debug.Write(summary);
                    toolTip1.Show(summary, detailsButton);
                };

                Button installButton = new Button();
                installButton.Text = "Install";
                installButton.Location = new Point(185, 233);
                installButton.FlatStyle = FlatStyle.Flat;
                installButton.Size = new Size(102, 27);
                installButton.BackColor = Color.FromArgb(47, 54, 61);
                installButton.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
                installButton.ForeColor = Color.Honeydew;


                await ModManagerAPI.LoadImageAsync(thumbnail, thumbnailBox);

                detailsPanel.Controls.Add(downloadIconBox);
                detailsPanel.Controls.Add(downloadLabel);
                detailsPanel.Controls.Add(likeIconBox);
                detailsPanel.Controls.Add(likeLabel);
                detailsPanel.Controls.Add(updatedAtLabel);




                panel.Controls.Add(detailsPanel);
                panel.Controls.Add(nameLabel);
                panel.Controls.Add(uploaderLabel);
                panel.Controls.Add(thumbnailBox);
                panel.Controls.Add(viewOnNexusButton);
                panel.Controls.Add(detailsButton);
                panel.Controls.Add(installButton);
                viewOnNexusButton.BringToFront();
                detailsButton.BringToFront();
                installButton.BringToFront();


                flowLayoutPanel5.Controls.Add(panel);
            }
        }*/

        private void LoadTrayIcon()
        {
            trayIcon = new NotifyIcon();
            trayIcon.Icon = this.Icon;
            trayIcon.Text = "Lathe";
            trayIcon.Visible = true;

            trayIcon.DoubleClick += (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                BringToFront();
            };

            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem openItem = new ToolStripMenuItem("Open");
            openItem.Click += (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                BringToFront();
            };

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) =>
            {
                realExit = true;
                trayIcon.Visible = false;
                Application.Exit();
            };

            menu.Items.Add(openItem);
            menu.Items.Add(exitItem);

            trayIcon.ContextMenuStrip = menu;
        }
        private void HideTabPanels()
        {
            List<Panel> allPanels = this.Controls.OfType<Panel>().ToList();
            foreach (Panel panel in allPanels)
            {
                //ignore the spacer and menu panels
                if (panel.Name != "panel1" && panel.Name != "panel3")
                {
                    panel.Visible = false;
                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (config.LoadConfig()["presence"] == "Disabled")
            {
                trayIcon.Visible = false;
                return;
            }
            if (!realExit)
            {
                e.Cancel = true;
                Hide();

                trayIcon.ShowBalloonTip(
                    1000,
                    "Still running",
                    "Lathe is still running in the tray.",
                    ToolTipIcon.Info
                );
            }
        }

        private void ResetMenuButtonColorsAndBorders()
        {
            button4.BackColor = Color.FromArgb(36, 41, 46);
            button5.BackColor = Color.FromArgb(36, 41, 46);
            button6.BackColor = Color.FromArgb(36, 41, 46);
            button4.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            button5.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            button6.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            modManager.RefreshMods();
            LoadCheckboxes();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Pak Files (*.pak)|*.pak|Zip Files (*.zip)|*.zip|Rar Files (*.rar)|*.rar|7z Files (*.7z)|*.7z";

            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            modManager.AddMod(dialog.FileName);

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = new Size(250, 120);
        }
        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                gameManager.ChangeFOV(Convert.ToInt32(textBox2.Text));
                MessageBox.Show($"Changed FOV to {textBox2.Text}");
            }
            catch (FormatException ex)
            {
                trackBar1.Value = 0;
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                trackBar1.Value = Convert.ToInt32(textBox2.Text);
            }
            catch (FormatException ex)
            {
                trackBar1.Value = 0;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                trackBar1.Value = 150;
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = $"{trackBar1.Value}";
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void flowLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                modManager.AddMod(file);
            }
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HideTabPanels();
            ResetMenuButtonColorsAndBorders();
            panel2.Visible = true;
            button4.BackColor = Color.FromArgb(47, 54, 61);
            button4.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            HideTabPanels();
            ResetMenuButtonColorsAndBorders();
            panel4.Visible = true;

            button5.BackColor = Color.FromArgb(47, 54, 61);
            button5.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            HideTabPanels();
            ResetMenuButtonColorsAndBorders();
            panel9.Visible = true;

            button6.BackColor = Color.FromArgb(47, 54, 61);
            button6.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = $"{trackBar2.Value}";
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                gameManager.ChangeScreenPercentage(Convert.ToInt32(textBox1.Text));
                MessageBox.Show($"Changed screen percentage to {textBox1.Text}");
            }
            catch (FormatException ex)
            {
                trackBar2.Value = 0;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            gameManager.ChangeFogToggle(comboBox1.SelectedItem.ToString());
            MessageBox.Show($"Changed fog to {comboBox1.SelectedItem.ToString()}");
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }


        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1. Go to nexusmods.com or click the button below\r\n2. Download mods while leaving this tool running and it will \r\nautomatically add the mod to the mod manager\r\n3. If you need to manually add mods, you can drag and drop\r\nthem or click the open button\r\n");
        }

        private void button12_Click(object sender, EventArgs e)
        {
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string modPageUrl = "https://www.nexusmods.com/games/theoutlasttrials/mods";
            Process.Start(new ProcessStartInfo
            {
                FileName = modPageUrl,
                UseShellExecute = true
            });
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == "Enabled")
            {
                if (config.LoadConfig()["startup"] == "Disabled")
                {
                    MessageBox.Show("Discord presence has been enabled for this session\nHowever, it will not properly work after restarts unless you enable\n launch on startup");
                }
                presence = new DiscordPresenceLoop();
                Task.Run(() => presence.StartLoop());
                config.ChangeProperty("presence", "Enabled");
            }

        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem == "Enabled")
            {
                StartupManager.EnableStartup();
                config.ChangeProperty("startup", "Enabled");
            }
            else
            {
                StartupManager.DisableStartup();
                config.ChangeProperty("startup", "Disabled");
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
