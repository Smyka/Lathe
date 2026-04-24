using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace OutlastTrayTool
{
    public class SplashScreen : Form
    {
        private System.Windows.Forms.Timer fadeTimer;
        private System.Windows.Forms.Timer closeTimer;
        private float opacity = 0f;
        private bool fadingIn = true;
        private bool fadingOut = false;

        // Portrait artwork
        private Image lathePortrait;

        // Gothic color palette
        private readonly Color bgDark       = Color.FromArgb(255,  8,  5,  8);
        private readonly Color bgMid        = Color.FromArgb(255, 18, 10, 18);
        private readonly Color bloodRed     = Color.FromArgb(255,140,  0,  0);
        private readonly Color bloodDim     = Color.FromArgb(255, 80,  0,  0);
        private readonly Color ashWhite     = Color.FromArgb(255,220,215,210);
        private readonly Color ashDim       = Color.FromArgb(255,140,130,120);
        private readonly Color goldAccent   = Color.FromArgb(255,180,140, 60);

        public SplashScreen()
        {
            // ── Window setup ──────────────────────────────────────────
            FormBorderStyle  = FormBorderStyle.None;
            StartPosition    = FormStartPosition.CenterScreen;
            Size             = new Size(1440, 810);
            BackColor        = Color.FromArgb(8, 5, 8);
            Opacity          = 0;
            TopMost          = true;
            ShowInTaskbar    = false;
            DoubleBuffered   = true;

            // Load LATHE portrait splash artwork — try embedded resource first, then file path
            try
            {
                // 1. Try embedded resource (works in published .exe)
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                string[] candidates = {
                    "Lathe.assets.splash_lathe_portrait.jpg",
                    "Lathe.assets.splash_lathe_portrait.png",
                    "OutlastTrayTool.assets.splash_lathe_portrait.jpg",
                    "OutlastTrayTool.assets.splash_lathe_portrait.png"
                };
                foreach (var res in candidates)
                {
                    using var stream = asm.GetManifestResourceStream(res);
                    if (stream != null)
                    {
                        lathePortrait = Image.FromStream(stream);
                        break;
                    }
                }
            }
            catch { }

            // 2. Fallback — load from file path (works when running from VS)
            if (lathePortrait == null)
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string[] fileCandidates = new[]
                    {
                        System.IO.Path.Combine(baseDir, "assets", "splash_lathe_portrait.jpg"),
                        System.IO.Path.Combine(baseDir, "assets", "splash_lathe_portrait.png"),
                        System.IO.Path.Combine(baseDir, "..", "..", "..", "assets", "splash_lathe_portrait.jpg"),
                        System.IO.Path.Combine(baseDir, "..", "..", "..", "assets", "splash_lathe_portrait.png")
                    };
                    foreach (var p in fileCandidates)
                    {
                        string full = System.IO.Path.GetFullPath(p);
                        if (System.IO.File.Exists(full))
                        {
                            lathePortrait = Image.FromFile(full);
                            break;
                        }
                    }
                }
                catch { }
            }

            // ── Fade-in / auto-close timers ───────────────────────────
            fadeTimer = new System.Windows.Forms.Timer { Interval = 16 };
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();

            closeTimer = new System.Windows.Forms.Timer { Interval = 3200 };
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();
                fadingIn  = false;
                fadingOut = true;
                fadeTimer.Start();
            };
            closeTimer.Start();
        }

        // ── Fade logic ────────────────────────────────────────────────
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (fadingIn)
            {
                opacity += 0.04f;
                if (opacity >= 1f) { opacity = 1f; fadeTimer.Stop(); }
                Opacity = opacity;
            }
            else if (fadingOut)
            {
                opacity -= 0.05f;
                if (opacity <= 0f)
                {
                    opacity = 0f;
                    fadeTimer.Stop();
                    Close();
                }
                Opacity = opacity;
            }
        }

        // ── All drawing ───────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = TextRenderingHint.AntiAliasGridFit;
            g.InterpolationMode  = InterpolationMode.HighQualityBicubic;

            int W = ClientSize.Width;
            int H = ClientSize.Height;

            // 1. Background gradient — near-black with a deep-crimson tint at center
            using (var bg = new PathGradientBrush(new PointF[]
            {
                new PointF(0, 0), new PointF(W, 0),
                new PointF(W, H), new PointF(0, H)
            }))
            {
                bg.CenterPoint  = new PointF(W / 2f, H / 2f);
                bg.CenterColor  = Color.FromArgb(255, 28, 8, 12);
                bg.SurroundColors = new[] { bgDark, bgDark, bgDark, bgDark };
                g.FillRectangle(bg, 0, 0, W, H);
            }

            // 2. Vertical blood-red glow strip behind title area
            using (var glow = new LinearGradientBrush(
                new Point(0, 60), new Point(0, 220),
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0)))
            {
                var cb = new ColorBlend(5);
                cb.Colors    = new[] {
                    Color.FromArgb(0, bloodDim),
                    Color.FromArgb(60, bloodDim),
                    Color.FromArgb(90, bloodRed),
                    Color.FromArgb(60, bloodDim),
                    Color.FromArgb(0, bloodDim)
                };
                cb.Positions = new[] { 0f, 0.25f, 0.5f, 0.75f, 1f };
                glow.InterpolationColors = cb;
                g.FillRectangle(glow, 0, 60, W, 160);
            }

            // 2b. Portrait background image (subtle overlay)
            if (lathePortrait != null)
            {
                int imgW = (int)(W * 0.78f); // smaller
                int imgH = (int)(imgW * lathePortrait.Height / (float)lathePortrait.Width);
                if (imgH > H * 0.65f)
                {
                    imgH = (int)(H * 0.65f);
                    imgW = (int)(imgH * lathePortrait.Width / (float)lathePortrait.Height);
                }

                int x = (W - imgW) / 2;
                int topReserved = 280; // more space for title + subtitle
                int y = topReserved + (H - topReserved - imgH) / 2;

                var dest = new Rectangle(x, y, imgW, imgH);
                using (var ia = new System.Drawing.Imaging.ImageAttributes())
                {
                    var cm = new System.Drawing.Imaging.ColorMatrix
                    {
                        Matrix33 = 0.85f
                    };
                    ia.SetColorMatrix(cm);
                    g.DrawImage(
                        lathePortrait,
                        dest,
                        0, 0, lathePortrait.Width, lathePortrait.Height,
                        GraphicsUnit.Pixel,
                        ia);
                }
            }

            // 3. Decorative horizontal rules — thin gold lines
            DrawGoldRule(g, W, 58);
            DrawGoldRule(g, W, 215);

            // 4. Ornamental corner thorns
            DrawCornerOrnaments(g, W, H);

            // 5. Cross / sigil watermark — faint behind text
            DrawFaintCross(g, W, H);

            // 6. Title: "LATHE" (top center, above portrait)
            using (var titleFont = new Font("Palatino Linotype", 60f, FontStyle.Bold, GraphicsUnit.Point))
            using (var titleBrush = new LinearGradientBrush(
                new Point(0, 40), new Point(0, 160),
                ashWhite, Color.FromArgb(255, 160, 145, 130)))
            {
                string title = "LATHE";
                SizeF titleSize = g.MeasureString(title, titleFont);

                float titleX = (W - titleSize.Width) / 2f;
                float titleY = 55f; // raised a bit

                using (var shadow = new SolidBrush(Color.FromArgb(180, bloodRed)))
                    g.DrawString(title, titleFont, shadow, titleX + 4, titleY + 6);

                g.DrawString(title, titleFont, titleBrush, titleX, titleY);

                // 7. Subtitle — spaced small caps style, directly under title with padding
                using (var subFont = new Font("Palatino Linotype", 11f, FontStyle.Italic, GraphicsUnit.Point))
                using (var subBrush = new SolidBrush(Color.FromArgb(220, goldAccent)))
                {
                    string sub = "O U T L A S T   T R I A L S   M O D   M A N A G E R";
                    SizeF subSize = g.MeasureString(sub, subFont);

                    float subX = (W - subSize.Width) / 2f;
                    float subY = titleY + titleSize.Height + 4f; // slightly tighter gap

                    g.DrawString(sub, subFont, subBrush, subX, subY);
                }
            }

            // 8. Version tag — bottom left
            using (var verFont = new Font("Courier New", 8f, FontStyle.Regular, GraphicsUnit.Point))
            using (var verBrush = new SolidBrush(Color.FromArgb(120, ashDim)))
            {
                g.DrawString("v 0.1", verFont, verBrush, 20, H - 26);
            }

            // 9. "Loading…" dots — bottom right
            DrawLoadingDots(g, W, H);

            // 10. Thin outer border — dark crimson
            using (var borderPen = new Pen(Color.FromArgb(160, bloodDim), 1.5f))
                g.DrawRectangle(borderPen, 1, 1, W - 3, H - 3);

            // 11. Inner border inset
            using (var innerPen = new Pen(Color.FromArgb(60, goldAccent), 0.8f))
                g.DrawRectangle(innerPen, 8, 8, W - 17, H - 17);
        }

        // ── Helpers ───────────────────────────────────────────────────

        private void DrawGoldRule(Graphics g, int W, int y)
        {
            using (var pen = new LinearGradientBrush(
                new Point(0, y), new Point(W, y),
                Color.FromArgb(0, goldAccent), Color.FromArgb(0, goldAccent)))
            {
                var cb = new ColorBlend(5);
                cb.Colors    = new[] {
                    Color.FromArgb(0, goldAccent),
                    Color.FromArgb(120, goldAccent),
                    Color.FromArgb(200, goldAccent),
                    Color.FromArgb(120, goldAccent),
                    Color.FromArgb(0, goldAccent)
                };
                cb.Positions = new[] { 0f, 0.2f, 0.5f, 0.8f, 1f };
                pen.InterpolationColors = cb;
                using (var p = new Pen(pen, 1f))
                    g.DrawLine(p, 30, y, W - 30, y);
            }
        }

        private void DrawCornerOrnaments(Graphics g, int W, int H)
        {
            using (var pen = new Pen(Color.FromArgb(130, goldAccent), 1f))
            {
                int m = 18, arm = 22;
                // Top-left
                g.DrawLine(pen, m, m, m + arm, m);
                g.DrawLine(pen, m, m, m, m + arm);
                // Top-right
                g.DrawLine(pen, W - m, m, W - m - arm, m);
                g.DrawLine(pen, W - m, m, W - m, m + arm);
                // Bottom-left
                g.DrawLine(pen, m, H - m, m + arm, H - m);
                g.DrawLine(pen, m, H - m, m, H - m - arm);
                // Bottom-right
                g.DrawLine(pen, W - m, H - m, W - m - arm, H - m);
                g.DrawLine(pen, W - m, H - m, W - m, H - m - arm);
            }

            // Diamond pip at each corner tip
            using (var brush = new SolidBrush(Color.FromArgb(150, goldAccent)))
            {
                DrawDiamond(g, brush, 18, 18, 4);
                DrawDiamond(g, brush, W - 18, 18, 4);
                DrawDiamond(g, brush, 18, H - 18, 4);
                DrawDiamond(g, brush, W - 18, H - 18, 4);
            }
        }

        private void DrawDiamond(Graphics g, Brush b, int cx, int cy, int r)
        {
            var pts = new PointF[]
            {
                new PointF(cx,     cy - r),
                new PointF(cx + r, cy),
                new PointF(cx,     cy + r),
                new PointF(cx - r, cy)
            };
            g.FillPolygon(b, pts);
        }

        private void DrawFaintCross(Graphics g, int W, int H)
        {
            using (var pen = new Pen(Color.FromArgb(18, bloodRed), 38f))
            {
                g.DrawLine(pen, W / 2, 0, W / 2, H);         // vertical
                g.DrawLine(pen, 0, H / 2 - 20, W, H / 2 - 20); // horizontal (offset up a touch)
            }
        }

        private void DrawLoadingDots(Graphics g, int W, int H)
        {
            int dotCount = 3;
            int dotR     = 3;
            int spacing  = 10;
            int totalW   = dotCount * (dotR * 2) + (dotCount - 1) * spacing;
            int startX   = W - 30 - totalW;
            int y        = H - 22;

            using (var brush = new SolidBrush(Color.FromArgb(100, bloodRed)))
            using (var brightBrush = new SolidBrush(Color.FromArgb(200, bloodRed)))
            {
                for (int i = 0; i < dotCount; i++)
                {
                    int x = startX + i * (dotR * 2 + spacing);
                    // Pulse — brighten one dot based on time
                    bool lit = (DateTime.Now.Millisecond / 333) % dotCount == i;
                    g.FillEllipse(lit ? brightBrush : brush, x, y, dotR * 2, dotR * 2);
                }
            }

            using (var font = new Font("Courier New", 8f))
            using (var b    = new SolidBrush(Color.FromArgb(90, ashDim)))
                g.DrawString("initializing", font, b, startX - 80, H - 26);

            // Repaint to animate dots
            System.Threading.Tasks.Task.Delay(333).ContinueWith(_ =>
            {
                try { Invoke(new Action(Invalidate)); } catch { }
            });
        }
    }
}
