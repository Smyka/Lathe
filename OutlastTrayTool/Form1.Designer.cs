using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace OutlastTrayTool
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

  private FlowLayoutPanel flowLayoutPanel1;
  private Button button1;
  private OpenFileDialog openFileDialog1;
  private Button button2;
  private TrackBar trackBar1;
  private Button button3;
  private TextBox textBox2;
  private Panel panel1;
  private Button button4;
  private Button button5;
  private Button button6;
  private Panel panel2;
  private Panel panel3;
  private Panel panel4;
  private Label label2;
  private Label label4;
  private Label label5;
  private Label label6;
  private FlowLayoutPanel flowLayoutPanel2;
  private Panel panel5;
  private ToolTip FOVtooltip;
  private Panel panel6;
  private Button button7;
  private TextBox textBox1;
  private Label label3;
  private TrackBar trackBar2;
  private ToolTip ScreenPercentagetooltip;
  private Label label7;
  private ToolTip toolTip1;
  private PictureBox pictureBox1;
  private PictureBox pictureBox2;
  private Label label9;
  private FlowLayoutPanel flowLayoutPanel3;
  private Panel panel8;
  private Label label13;
  private FlowLayoutPanel flowLayoutPanel5;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            flowLayoutPanel1 = new FlowLayoutPanel();
            label4 = new Label();
            button2 = new Button();
            button1 = new Button();
            openFileDialog1 = new OpenFileDialog();
            trackBar1 = new TrackBar();
            button3 = new Button();
            textBox2 = new TextBox();
            panel1 = new Panel();
            label6 = new Label();
            label5 = new Label();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            panel2 = new Panel();
            button13 = new Button();
            button10 = new Button();
            label10 = new Label();
            panel3 = new Panel();
            panel4 = new Panel();
            label9 = new Label();
            flowLayoutPanel3 = new FlowLayoutPanel();
            label7 = new Label();
            flowLayoutPanel2 = new FlowLayoutPanel();
            panel5 = new Panel();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            panel6 = new Panel();
            pictureBox2 = new PictureBox();
            textBox1 = new TextBox();
            button7 = new Button();
            label3 = new Label();
            trackBar2 = new TrackBar();
            FOVtooltip = new ToolTip(components);
            pictureBox8 = new PictureBox();
            pictureBox5 = new PictureBox();
            ScreenPercentagetooltip = new ToolTip(components);
            toolTip1 = new ToolTip(components);
            label17 = new Label();
            label15 = new Label();
            panel8 = new Panel();
            flowLayoutPanel5 = new FlowLayoutPanel();
            label13 = new Label();
            panel9 = new Panel();
            label14 = new Label();
            flowLayoutPanel6 = new FlowLayoutPanel();
            panel14 = new Panel();
            button18 = new Button();
            comboBox2 = new ComboBox();
            panel12 = new Panel();
            button16 = new Button();
            comboBox3 = new ComboBox();
            flowLayoutPanel1.SuspendLayout();
            ((ISupportInitialize)trackBar1).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel4.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            panel5.SuspendLayout();
            ((ISupportInitialize)pictureBox1).BeginInit();
            panel6.SuspendLayout();
            ((ISupportInitialize)pictureBox2).BeginInit();
            ((ISupportInitialize)trackBar2).BeginInit();
            ((ISupportInitialize)pictureBox8).BeginInit();
            ((ISupportInitialize)pictureBox5).BeginInit();
            panel8.SuspendLayout();
            panel9.SuspendLayout();
            flowLayoutPanel6.SuspendLayout();
            panel14.SuspendLayout();
            panel12.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AllowDrop = true;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel1.Controls.Add(label4);
            flowLayoutPanel1.Dock = DockStyle.Left;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.ForeColor = SystemColors.Control;
            flowLayoutPanel1.Location = new Point(40, 66);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(5);
            flowLayoutPanel1.Size = new Size(481, 433);
            flowLayoutPanel1.TabIndex = 2;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.DragDrop += flowLayoutPanel1_DragDrop;
            flowLayoutPanel1.DragEnter += flowLayoutPanel1_DragEnter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.Honeydew;
            label4.Location = new Point(8, 5);
            label4.Name = "label4";
            label4.Size = new Size(326, 17);
            label4.TabIndex = 6;
            label4.Text = "Drag and drop mods here or click open to add mods!";
            label4.TextAlign = ContentAlignment.TopCenter;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(47, 54, 61);
            button2.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 9.75F);
            button2.ForeColor = Color.Honeydew;
            button2.Location = new Point(527, 66);
            button2.Name = "button2";
            button2.Size = new Size(88, 32);
            button2.TabIndex = 4;
            button2.Text = "Open";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(47, 54, 61);
            button1.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 9.75F);
            button1.ForeColor = Color.Honeydew;
            button1.Location = new Point(527, 104);
            button1.Name = "button1";
            button1.Size = new Size(88, 32);
            button1.TabIndex = 3;
            button1.Text = "Refresh";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.FileOk += openFileDialog1_FileOk;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(26, 58);
            trackBar1.Maximum = 150;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(240, 45);
            trackBar1.TabIndex = 5;
            trackBar1.TickFrequency = 5;
            trackBar1.TickStyle = TickStyle.None;
            trackBar1.Value = 90;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(47, 54, 61);
            button3.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 9.75F);
            button3.ForeColor = Color.Honeydew;
            button3.Location = new Point(191, 97);
            button3.Name = "button3";
            button3.Size = new Size(75, 27);
            button3.TabIndex = 7;
            button3.Text = "Apply";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(64, 64, 64);
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.ForeColor = Color.White;
            textBox2.Location = new Point(132, 97);
            textBox2.MaxLength = 4;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(50, 23);
            textBox2.TabIndex = 8;
            textBox2.TextAlign = HorizontalAlignment.Center;
            textBox2.TextChanged += textBox2_TextChanged;
            textBox2.KeyPress += textBox1_KeyPress;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(36, 41, 46);
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(button6);
            panel1.Controls.Add(button5);
            panel1.Controls.Add(button4);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(10, 10);
            panel1.Name = "panel1";
            panel1.Size = new Size(231, 541);
            panel1.TabIndex = 11;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.749999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.Honeydew;
            label6.Location = new Point(5, 518);
            label6.Name = "label6";
            label6.Size = new Size(76, 17);
            label6.TabIndex = 9;
            label6.Text = "Version 0.4";
            label6.Click += label6_Click;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11.9999981F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.Honeydew;
            label5.Location = new Point(5, 498);
            label5.Name = "label5";
            label5.Size = new Size(52, 21);
            label5.TabIndex = 8;
            label5.Text = "Lathe";
            // 
            // button6
            // 
            button6.BackColor = Color.FromArgb(36, 41, 46);
            button6.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            button6.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 54, 61);
            button6.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 54, 61);
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            button6.ForeColor = Color.Honeydew;
            button6.Image = (Image)resources.GetObject("button6.Image");
            button6.ImageAlign = ContentAlignment.MiddleLeft;
            button6.Location = new Point(-1, 137);
            button6.Name = "button6";
            button6.Padding = new Padding(10, 0, 0, 0);
            button6.Size = new Size(231, 70);
            button6.TabIndex = 7;
            button6.Text = "Presence  ";
            button6.UseVisualStyleBackColor = false;
            button6.Click += button6_Click;
            // 
            // button5
            // 
            button5.BackColor = Color.FromArgb(36, 41, 46);
            button5.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            button5.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 54, 61);
            button5.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 54, 61);
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            button5.ForeColor = Color.Honeydew;
            button5.Image = (Image)resources.GetObject("button5.Image");
            button5.ImageAlign = ContentAlignment.MiddleLeft;
            button5.Location = new Point(-1, 68);
            button5.Name = "button5";
            button5.Padding = new Padding(10, 0, 0, 0);
            button5.Size = new Size(231, 70);
            button5.TabIndex = 6;
            button5.Text = "Tweaks     ";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.FromArgb(36, 41, 46);
            button4.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            button4.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 54, 61);
            button4.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 54, 61);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            button4.ForeColor = Color.Honeydew;
            button4.Image = (Image)resources.GetObject("button4.Image");
            button4.ImageAlign = ContentAlignment.MiddleLeft;
            button4.Location = new Point(-1, -1);
            button4.Name = "button4";
            button4.Padding = new Padding(10, 0, 0, 0);
            button4.Size = new Size(231, 70);
            button4.TabIndex = 0;
            button4.Text = "Mods        ";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.FromArgb(36, 41, 46);
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(button13);
            panel2.Controls.Add(button10);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(flowLayoutPanel1);
            panel2.Controls.Add(button2);
            panel2.Location = new Point(251, 10);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(40, 66, 40, 40);
            panel2.Size = new Size(720, 541);
            panel2.TabIndex = 12;
            // 
            // button13
            // 
            button13.BackColor = Color.FromArgb(47, 54, 61);
            button13.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            button13.FlatStyle = FlatStyle.Flat;
            button13.Font = new Font("Segoe UI", 9.75F);
            button13.ForeColor = Color.Honeydew;
            button13.Location = new Point(527, 180);
            button13.Name = "button13";
            button13.Size = new Size(88, 67);
            button13.TabIndex = 18;
            button13.Text = "Open Nexus\r\nMod Page\r\n";
            button13.UseVisualStyleBackColor = false;
            button13.Click += button13_Click;
            // 
            // button10
            // 
            button10.BackColor = Color.FromArgb(47, 54, 61);
            button10.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            button10.FlatStyle = FlatStyle.Flat;
            button10.Font = new Font("Segoe UI", 9.75F);
            button10.ForeColor = Color.Honeydew;
            button10.Location = new Point(527, 142);
            button10.Name = "button10";
            button10.Size = new Size(88, 32);
            button10.TabIndex = 17;
            button10.Text = "Instructions";
            button10.UseVisualStyleBackColor = false;
            button10.Click += button10_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.ForeColor = Color.Honeydew;
            label10.Location = new Point(235, 25);
            label10.Name = "label10";
            label10.Size = new Size(52, 21);
            label10.TabIndex = 14;
            label10.Text = "Mods";
            // 
            // panel3
            // 
            panel3.Dock = DockStyle.Left;
            panel3.Location = new Point(241, 10);
            panel3.Name = "panel3";
            panel3.Size = new Size(10, 541);
            panel3.TabIndex = 13;
            // 
            // panel4
            // 
            panel4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel4.BackColor = Color.FromArgb(36, 41, 46);
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(label9);
            panel4.Controls.Add(flowLayoutPanel3);
            panel4.Controls.Add(label7);
            panel4.Controls.Add(flowLayoutPanel2);
            panel4.Location = new Point(251, 10);
            panel4.Name = "panel4";
            panel4.Size = new Size(720, 541);
            panel4.TabIndex = 14;
            panel4.Visible = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = Color.Honeydew;
            label9.Location = new Point(467, 25);
            label9.Name = "label9";
            label9.Size = new Size(52, 21);
            label9.TabIndex = 15;
            label9.Text = "Other";
            label9.Visible = false;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.AutoScroll = true;
            flowLayoutPanel3.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel3.Location = new Point(345, 66);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(299, 553);
            flowLayoutPanel3.TabIndex = 14;
            flowLayoutPanel3.Visible = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.Honeydew;
            label7.Location = new Point(131, 25);
            label7.Name = "label7";
            label7.Size = new Size(116, 21);
            label7.TabIndex = 13;
            label7.Text = "Game Settings";
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel2.Controls.Add(panel5);
            flowLayoutPanel2.Controls.Add(panel6);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Location = new Point(40, 66);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(299, 553);
            flowLayoutPanel2.TabIndex = 12;
            // 
            // panel5
            // 
            panel5.BorderStyle = BorderStyle.FixedSingle;
            panel5.Controls.Add(pictureBox1);
            panel5.Controls.Add(button3);
            panel5.Controls.Add(textBox2);
            panel5.Controls.Add(label2);
            panel5.Controls.Add(trackBar1);
            panel5.Location = new Point(3, 3);
            panel5.Name = "panel5";
            panel5.Size = new Size(292, 150);
            panel5.TabIndex = 13;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(251, 23);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(19, 19);
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            FOVtooltip.SetToolTip(pictureBox1, "Changes in-game FOV\r\nRecommended: 110-120");
            pictureBox1.Click += pictureBox1_Click_1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Honeydew;
            label2.Location = new Point(104, 21);
            label2.Name = "label2";
            label2.Size = new Size(84, 21);
            label2.TabIndex = 5;
            label2.Text = "FOV Slider";
            FOVtooltip.SetToolTip(label2, "Changes in-game FOV\r\nRecommended: 110-120");
            label2.Click += label2_Click;
            // 
            // panel6
            // 
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel6.Controls.Add(pictureBox2);
            panel6.Controls.Add(textBox1);
            panel6.Controls.Add(button7);
            panel6.Controls.Add(label3);
            panel6.Controls.Add(trackBar2);
            panel6.Location = new Point(3, 159);
            panel6.Name = "panel6";
            panel6.Size = new Size(292, 150);
            panel6.TabIndex = 14;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(251, 23);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(19, 19);
            pictureBox2.TabIndex = 10;
            pictureBox2.TabStop = false;
            FOVtooltip.SetToolTip(pictureBox2, "Allows you to render higher than your screen\r\nresolution, then downsamples to give a crisp image");
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(64, 64, 64);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(132, 98);
            textBox1.MaxLength = 4;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(50, 23);
            textBox1.TabIndex = 8;
            textBox1.TextAlign = HorizontalAlignment.Center;
            textBox1.TextChanged += textBox1_TextChanged_1;
            textBox1.KeyPress += textBox1_KeyPress;
            // 
            // button7
            // 
            button7.BackColor = Color.FromArgb(47, 54, 61);
            button7.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
            button7.FlatStyle = FlatStyle.Flat;
            button7.Font = new Font("Segoe UI", 9.75F);
            button7.ForeColor = Color.Honeydew;
            button7.Location = new Point(191, 98);
            button7.Name = "button7";
            button7.Size = new Size(75, 27);
            button7.TabIndex = 7;
            button7.Text = "Apply";
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Honeydew;
            label3.Location = new Point(77, 21);
            label3.Name = "label3";
            label3.Size = new Size(137, 21);
            label3.TabIndex = 5;
            label3.Text = "Screen Percentage";
            ScreenPercentagetooltip.SetToolTip(label3, "Allows you to render higher than your screen\r\nresolution, then downsamples to give a crisp image\r\n");
            // 
            // trackBar2
            // 
            trackBar2.Location = new Point(26, 58);
            trackBar2.Maximum = 400;
            trackBar2.Minimum = 100;
            trackBar2.Name = "trackBar2";
            trackBar2.Size = new Size(240, 45);
            trackBar2.TabIndex = 5;
            trackBar2.TickFrequency = 5;
            trackBar2.TickStyle = TickStyle.None;
            trackBar2.Value = 100;
            trackBar2.Scroll += trackBar2_Scroll;
            // 
            // pictureBox8
            // 
            pictureBox8.Location = new Point(247, 23);
            pictureBox8.Name = "pictureBox8";
            pictureBox8.Size = new Size(19, 19);
            pictureBox8.TabIndex = 11;
            pictureBox8.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Location = new Point(247, 23);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(19, 19);
            pictureBox5.TabIndex = 11;
            pictureBox5.TabStop = false;
            // 
            // toolTip1
            // 
            toolTip1.Popup += toolTip1_Popup;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label17.ForeColor = Color.Honeydew;
            label17.Location = new Point(64, 19);
            label17.Name = "label17";
            label17.Size = new Size(163, 21);
            label17.TabIndex = 5;
            label17.Text = "Discord Rich Presence";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label15.ForeColor = Color.Honeydew;
            label15.Location = new Point(78, 21);
            label15.Name = "label15";
            label15.Size = new Size(136, 21);
            label15.TabIndex = 5;
            label15.Text = "Launch on Startup";
            // 
            // panel8
            // 
            panel8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel8.BackColor = Color.FromArgb(36, 41, 46);
            panel8.BorderStyle = BorderStyle.FixedSingle;
            panel8.Controls.Add(flowLayoutPanel5);
            panel8.Controls.Add(label13);
            panel8.Location = new Point(2511, 10);
            panel8.Name = "panel8";
            panel8.Size = new Size(720, 541);
            panel8.TabIndex = 15;
            panel8.Visible = false;
            // 
            // flowLayoutPanel5
            // 
            flowLayoutPanel5.AutoScroll = true;
            flowLayoutPanel5.AutoSize = true;
            flowLayoutPanel5.Dock = DockStyle.Fill;
            flowLayoutPanel5.Location = new Point(0, 0);
            flowLayoutPanel5.Name = "flowLayoutPanel5";
            flowLayoutPanel5.Padding = new Padding(20, 50, 20, 20);
            flowLayoutPanel5.Size = new Size(718, 539);
            flowLayoutPanel5.TabIndex = 12;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label13.ForeColor = Color.Honeydew;
            label13.Location = new Point(444, 25);
            label13.Name = "label13";
            label13.Size = new Size(0, 21);
            label13.TabIndex = 13;
            // 
            // panel9
            // 
            panel9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel9.BackColor = Color.FromArgb(36, 41, 46);
            panel9.BorderStyle = BorderStyle.FixedSingle;
            panel9.Controls.Add(label14);
            panel9.Controls.Add(flowLayoutPanel6);
            panel9.Location = new Point(251, 10);
            panel9.Name = "panel9";
            panel9.Size = new Size(720, 541);
            panel9.TabIndex = 16;
            panel9.Visible = false;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label14.ForeColor = Color.Honeydew;
            label14.Location = new Point(99, 25);
            label14.Name = "label14";
            label14.Size = new Size(188, 21);
            label14.TabIndex = 13;
            label14.Text = "Enable Discord Presence";
            label14.Click += label14_Click;
            // 
            // flowLayoutPanel6
            // 
            flowLayoutPanel6.AutoScroll = true;
            flowLayoutPanel6.Controls.Add(panel14);
            flowLayoutPanel6.Controls.Add(panel12);
            flowLayoutPanel6.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel6.Location = new Point(40, 66);
            flowLayoutPanel6.Name = "flowLayoutPanel6";
            flowLayoutPanel6.Size = new Size(299, 553);
            flowLayoutPanel6.TabIndex = 12;
            // 
            // panel14
            // 
            panel14.BorderStyle = BorderStyle.FixedSingle;
            panel14.Controls.Add(pictureBox8);
            panel14.Controls.Add(button18);
            panel14.Controls.Add(comboBox2);
            panel14.Controls.Add(label17);
            panel14.Location = new Point(3, 3);
            panel14.Name = "panel14";
            panel14.Size = new Size(292, 116);
            panel14.TabIndex = 15;
            // 
            // button18
            // 
            button18.BackColor = Color.FromArgb(47, 54, 61);
            button18.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
            button18.FlatStyle = FlatStyle.Flat;
            button18.Font = new Font("Segoe UI", 9.75F);
            button18.ForeColor = Color.Honeydew;
            button18.Location = new Point(191, 63);
            button18.Name = "button18";
            button18.Size = new Size(75, 27);
            button18.TabIndex = 7;
            button18.Text = "Apply";
            button18.UseVisualStyleBackColor = false;
            button18.Click += button18_Click;
            // 
            // comboBox2
            // 
            comboBox2.BackColor = SystemColors.Control;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FlatStyle = FlatStyle.Flat;
            comboBox2.Font = new Font("Segoe UI", 9F);
            comboBox2.FormattingEnabled = true;
            comboBox2.ItemHeight = 15;
            comboBox2.Items.AddRange(new object[] { "Enabled", "Disabled" });
            comboBox2.Location = new Point(108, 63);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(74, 23);
            comboBox2.TabIndex = 9;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // panel12
            // 
            panel12.BorderStyle = BorderStyle.FixedSingle;
            panel12.Controls.Add(pictureBox5);
            panel12.Controls.Add(button16);
            panel12.Controls.Add(comboBox3);
            panel12.Controls.Add(label15);
            panel12.Location = new Point(3, 125);
            panel12.Name = "panel12";
            panel12.Size = new Size(292, 116);
            panel12.TabIndex = 16;
            // 
            // button16
            // 
            button16.BackColor = Color.FromArgb(47, 54, 61);
            button16.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
            button16.FlatStyle = FlatStyle.Flat;
            button16.Font = new Font("Segoe UI", 9.75F);
            button16.ForeColor = Color.Honeydew;
            button16.Location = new Point(191, 63);
            button16.Name = "button16";
            button16.Size = new Size(75, 27);
            button16.TabIndex = 7;
            button16.Text = "Apply";
            button16.UseVisualStyleBackColor = false;
            button16.Click += button16_Click;
            // 
            // comboBox3
            // 
            comboBox3.BackColor = SystemColors.Control;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.FlatStyle = FlatStyle.Flat;
            comboBox3.Font = new Font("Segoe UI", 9F);
            comboBox3.FormattingEnabled = true;
            comboBox3.ItemHeight = 15;
            comboBox3.Items.AddRange(new object[] { "Enabled", "Disabled" });
            comboBox3.Location = new Point(108, 63);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(74, 23);
            comboBox3.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(29, 33, 37);
            ClientSize = new Size(984, 561);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Controls.Add(panel8);
            Controls.Add(panel4);
            Controls.Add(panel2);
            Controls.Add(panel9);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(930, 400);
            Name = "Form1";
            Padding = new Padding(10);
            Text = "Lathe";
            FormClosing += Form1_FormClosing;
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((ISupportInitialize)trackBar1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ((ISupportInitialize)pictureBox1).EndInit();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            ((ISupportInitialize)pictureBox2).EndInit();
            ((ISupportInitialize)trackBar2).EndInit();
            ((ISupportInitialize)pictureBox8).EndInit();
            ((ISupportInitialize)pictureBox5).EndInit();
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            flowLayoutPanel6.ResumeLayout(false);
            panel14.ResumeLayout(false);
            panel14.PerformLayout();
            panel12.ResumeLayout(false);
            panel12.PerformLayout();
            ResumeLayout(false);
        }
        private Label label10;
        private Button button10;
        private Button button13;
        private Panel panel9;
        private Label label14;
        private FlowLayoutPanel flowLayoutPanel6;
        private Panel panel14;
        private PictureBox pictureBox8;
        private Button button18;
        private ComboBox comboBox2;
        private Label label17;
        private Panel panel12;
        private PictureBox pictureBox5;
        private Button button16;
        private ComboBox comboBox3;
        private Label label15;
    }
}