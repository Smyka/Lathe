using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OutlastTrayTool
{
    public static class Theme
    {
        // ── Gothic palette ────────────────────────────────────────────
        static readonly Color G_BgDark    = Color.FromArgb(8, 5, 8);
        static readonly Color G_BgMid     = Color.FromArgb(18, 10, 18);
        static readonly Color G_BgPanel   = Color.FromArgb(22, 14, 22);
        static readonly Color G_Crimson   = Color.FromArgb(140, 0, 0);
        static readonly Color G_CrimDim   = Color.FromArgb(80, 0, 0);
        static readonly Color G_AshWhite  = Color.FromArgb(220, 215, 210);
        static readonly Color G_AshDim    = Color.FromArgb(140, 130, 120);
        static readonly Color G_Gold      = Color.FromArgb(180, 140, 60);

        // ── Outlast Trials palette ────────────────────────────────────
        static readonly Color O_BgDark    = Color.FromArgb(5, 10, 5);
        static readonly Color O_BgMid     = Color.FromArgb(8, 16, 8);
        static readonly Color O_BgPanel   = Color.FromArgb(10, 20, 10);
        static readonly Color O_Green     = Color.FromArgb(0, 180, 0);
        static readonly Color O_GreenDim  = Color.FromArgb(0, 80, 0);
        static readonly Color O_GreenBrt  = Color.FromArgb(80, 220, 80);
        static readonly Color O_TextMain  = Color.FromArgb(200, 240, 200);
        static readonly Color O_TextDim   = Color.FromArgb(100, 160, 100);

        // ── Pastel palette ────────────────────────────────────────────
        static readonly Color P_BgLight   = Color.FromArgb(255, 240, 245);
        static readonly Color P_BgMid     = Color.FromArgb(255, 228, 238);
        static readonly Color P_BgPanel   = Color.FromArgb(255, 218, 232);
        static readonly Color P_Pink      = Color.FromArgb(255, 150, 180);
        static readonly Color P_PinkDim   = Color.FromArgb(220, 100, 140);
        static readonly Color P_PinkBrt   = Color.FromArgb(255, 180, 210);
        static readonly Color P_TextMain  = Color.FromArgb(120, 60, 90);
        static readonly Color P_TextDim   = Color.FromArgb(180, 120, 150);
        static readonly Color P_Lavender  = Color.FromArgb(210, 190, 255);
        static readonly Color P_Mint      = Color.FromArgb(180, 240, 210);

        // Originals storage
        private static readonly System.Collections.Generic.Dictionary<Control, (Color Back, Color Fore, Font Font, FlatStyle? Flat, BorderStyle? Border, bool UseVisual, Color? BtnBorderColor, Color? BtnHoverColor)> originals
            = new System.Collections.Generic.Dictionary<Control, (Color, Color, Font, FlatStyle?, BorderStyle?, bool, Color?, Color?)>();

        private static PaintEventHandler? currentPaintHandler = null;
        private static Form? currentForm = null;

        // ─────────────────────────────────────────────────────────────
        public static void Apply(Form f, ThemeMode mode)
        {
            // Unhook previous paint
            if (currentForm != null && currentPaintHandler != null)
                currentForm.Paint -= currentPaintHandler;

            if (mode == ThemeMode.Original)
            {
                RestoreOriginals(f);
                foreach (Control c in GetAll(f)) StyleControl(c, ThemeMode.Original);
                currentPaintHandler = null;
                currentForm = null;
                return;
            }

            if (originals.Count == 0) SaveOriginals(f);

            SetDoubleBuffered(f, true);

            if (mode == ThemeMode.Gothic)
            {
                currentPaintHandler = (s, e) => DrawGothicBg(e.Graphics, ((Form)s).ClientSize);
                f.BackColor = G_BgDark;
                f.ForeColor = G_AshWhite;
                f.Font = SafeFont("Palatino Linotype", 9f, FontStyle.Regular);
                foreach (Control c in GetAll(f)) StyleControl(c, mode);
            }
            else if (mode == ThemeMode.Outlast)
            {
                currentPaintHandler = (s, e) => DrawOutlastBg(e.Graphics, ((Form)s).ClientSize);
                f.BackColor = O_BgDark;
                f.ForeColor = O_TextMain;
                f.Font = SafeFont("Roboto Mono", 9f, FontStyle.Regular);
                foreach (Control c in GetAll(f)) StyleControl(c, mode);
            }
            else // Pastel
            {
                currentPaintHandler = (s, e) => DrawPastelBg(e.Graphics, ((Form)s).ClientSize);
                f.BackColor = P_BgLight;
                f.ForeColor = P_TextMain;
                f.Font = SafeFont("Comic Sans MS", 9f, FontStyle.Regular);
                foreach (Control c in GetAll(f)) StyleControl(c, mode);
            }

            currentForm = f;
            f.Paint += currentPaintHandler;
            f.Invalidate(true);
        }

        // ─────────────────────────────────────────────────────────────
        private static void SaveOriginals(Control top)
        {
            originals.Clear();
            foreach (var c in GetAll(top))
            {
                try
                {
                    FlatStyle? fs = null;
                    BorderStyle? bs = null;
                    bool useVisual = false;
                    Color? btnBorder = null;
                    Color? btnHover = null;

                    if (c is Button btn)
                    {
                        fs = btn.FlatStyle;
                        useVisual = btn.UseVisualStyleBackColor;
                        btnBorder = btn.FlatAppearance.BorderColor;
                        btnHover  = btn.FlatAppearance.MouseOverBackColor;
                    }
                    if (c is TextBox tb)  bs = tb.BorderStyle;
                    if (c is ComboBox cb) fs = cb.FlatStyle;

                    originals[c] = (c.BackColor, c.ForeColor, c.Font, fs, bs, useVisual, btnBorder, btnHover);
                }
                catch { }
            }
        }

        public static void RestoreOriginals(Form f)
        {
            f.SuspendLayout();
            f.BackColor = Color.FromArgb(210, 210, 210);
            f.ForeColor = SystemColors.ControlText;
            f.Font      = SystemFonts.DefaultFont;
            foreach (var kv in originals)
            {
                try
                {
                    var c = kv.Key;
                    if (c == null || c.IsDisposed) continue;
                    c.BackColor = kv.Value.Back;
                    c.ForeColor = kv.Value.Fore;
                    c.Font      = kv.Value.Font;

                    if (c is Button btn)
                    {
                        if (kv.Value.Flat.HasValue)        btn.FlatStyle = kv.Value.Flat.Value;
                        btn.UseVisualStyleBackColor        = kv.Value.UseVisual;
                        if (kv.Value.BtnBorderColor.HasValue)
                            btn.FlatAppearance.BorderColor      = kv.Value.BtnBorderColor.Value;
                        if (kv.Value.BtnHoverColor.HasValue)
                            btn.FlatAppearance.MouseOverBackColor = kv.Value.BtnHoverColor.Value;
                    }
                    if (c is TextBox tb && kv.Value.Border.HasValue)
                        tb.BorderStyle = kv.Value.Border.Value;
                    if (c is ComboBox cb && kv.Value.Flat.HasValue)
                        cb.FlatStyle = kv.Value.Flat.Value;
                }
                catch { }
            }
            f.ResumeLayout(true);
            f.Invalidate(true);
        }

        // ── Per-control styling ───────────────────────────────────────
        private static void StyleControl(Control c, ThemeMode mode)
        {
            try
            {
                if (mode == ThemeMode.Gothic)        StyleGothic(c);
                else if (mode == ThemeMode.Outlast)  StyleOutlast(c);
                else if (mode == ThemeMode.Pastel)   StylePastel(c);
                else                                 StyleOriginal(c);
            }
            catch { }
        }

        // Only fix what RestoreOriginals can't: FlatAppearance properties
        // which WinForms doesn't reset automatically
        private static void StyleOriginal(Control c)
        {
            if (c is Button btn)
            {
                btn.FlatAppearance.BorderColor        = Color.Empty;
                btn.FlatAppearance.MouseOverBackColor = Color.Empty;
                btn.FlatAppearance.MouseDownBackColor = Color.Empty;
            }
        }

        private static void StyleGothic(Control c)
        {
            if (c is FlowLayoutPanel)    { c.BackColor = Color.FromArgb(28, 16, 28); c.ForeColor = G_AshWhite; }
            else if (c is Panel)         { c.BackColor = Color.FromArgb(22, 14, 22); c.ForeColor = G_AshWhite; }
            else if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = Color.FromArgb(35, 20, 35);
                btn.ForeColor = G_AshWhite;
                btn.FlatAppearance.BorderColor = Color.FromArgb(100, 60, 0, 0);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 30, 5, 5);
                btn.Font = SafeFont("Palatino Linotype", Math.Max(9f, btn.Font.Size), FontStyle.Regular);
            }
            else if (c is Label lbl)
            {
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = lbl.Font.Size >= 12f ? G_AshWhite : G_AshDim;
                lbl.Font = SafeFont("Palatino Linotype", Math.Max(9f, lbl.Font.Size), FontStyle.Regular);
            }
            else if (c is TextBox tb)    { tb.BackColor = Color.FromArgb(28, 16, 28); tb.ForeColor = G_AshWhite; tb.BorderStyle = BorderStyle.FixedSingle; }
            else if (c is ComboBox cb)   { cb.BackColor = Color.FromArgb(28, 16, 28); cb.ForeColor = G_AshWhite; cb.FlatStyle = FlatStyle.Flat; }
            else if (c is CheckBox chk)  { chk.BackColor = Color.Transparent; chk.ForeColor = G_AshWhite; }
            else if (c is TrackBar)      { c.BackColor = G_BgMid; }
            else if (c is PictureBox)    { c.BackColor = Color.Transparent; }
            else                         { c.BackColor = G_BgMid; c.ForeColor = G_AshWhite; }
        }

        private static void StyleOutlast(Control c)
        {
            if (c is FlowLayoutPanel)    { c.BackColor = Color.FromArgb(8, 18, 8); c.ForeColor = O_TextMain; }
            else if (c is Panel)         { c.BackColor = Color.FromArgb(10, 20, 10); c.ForeColor = O_TextMain; }
            else if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = Color.FromArgb(8, 22, 8);
                btn.ForeColor = O_GreenBrt;
                btn.FlatAppearance.BorderColor = Color.FromArgb(0, 100, 0);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 40, 0);
                btn.Font = SafeFont("Roboto Mono", Math.Max(8f, btn.Font.Size), FontStyle.Regular);
            }
            else if (c is Label lbl)
            {
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = lbl.Font.Size >= 12f ? O_TextMain : O_TextDim;
                lbl.Font = SafeFont("Roboto Mono", Math.Max(8f, lbl.Font.Size), FontStyle.Regular);
            }
            else if (c is TextBox tb)    { tb.BackColor = Color.FromArgb(5, 15, 5); tb.ForeColor = O_GreenBrt; tb.BorderStyle = BorderStyle.FixedSingle; }
            else if (c is ComboBox cb)   { cb.BackColor = Color.FromArgb(5, 15, 5); cb.ForeColor = O_GreenBrt; cb.FlatStyle = FlatStyle.Flat; }
            else if (c is CheckBox chk)  { chk.BackColor = Color.Transparent; chk.ForeColor = O_TextMain; }
            else if (c is TrackBar)      { c.BackColor = O_BgMid; }
            else if (c is PictureBox)    { c.BackColor = Color.Transparent; }
            else                         { c.BackColor = O_BgMid; c.ForeColor = O_TextMain; }
        }

        private static void StylePastel(Control c)
        {
            if (c is FlowLayoutPanel)    { c.BackColor = P_BgMid;   c.ForeColor = P_TextMain; }
            else if (c is Panel)         { c.BackColor = P_BgPanel; c.ForeColor = P_TextMain; }
            else if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = P_PinkBrt;
                btn.ForeColor = P_TextMain;
                btn.FlatAppearance.BorderColor        = P_PinkDim;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 200, 220);
                btn.Font = SafeFont("Comic Sans MS", Math.Max(9f, btn.Font.Size), FontStyle.Regular);
            }
            else if (c is Label lbl)
            {
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = lbl.Font.Size >= 12f ? P_TextMain : P_TextDim;
                lbl.Font = SafeFont("Comic Sans MS", Math.Max(9f, lbl.Font.Size), FontStyle.Regular);
            }
            else if (c is TextBox tb)    { tb.BackColor = Color.White; tb.ForeColor = P_TextMain; tb.BorderStyle = BorderStyle.FixedSingle; }
            else if (c is ComboBox cb)   { cb.BackColor = Color.White; cb.ForeColor = P_TextMain; cb.FlatStyle = FlatStyle.Flat; }
            else if (c is CheckBox chk)  { chk.BackColor = Color.Transparent; chk.ForeColor = P_TextMain; }
            else if (c is TrackBar)      { c.BackColor = P_BgMid; }
            else if (c is PictureBox)    { c.BackColor = Color.Transparent; }
            else                         { c.BackColor = P_BgMid; c.ForeColor = P_TextMain; }
        }

        // ── Background painters ───────────────────────────────────────
        private static void DrawGothicBg(Graphics g, Size sz)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Near-black radial with crimson center
            using (var bg = new PathGradientBrush(new PointF[]
            { new(0,0), new(sz.Width,0), new(sz.Width,sz.Height), new(0,sz.Height) }))
            {
                bg.CenterPoint    = new PointF(sz.Width / 2f, sz.Height * 0.3f);
                bg.CenterColor    = Color.FromArgb(255, 28, 8, 12);
                bg.SurroundColors = new[] { G_BgDark, G_BgDark, G_BgDark, G_BgDark };
                g.FillRectangle(bg, 0, 0, sz.Width, sz.Height);
            }

            // Gold rule at top
            DrawGradientLine(g, sz.Width, 58, G_Gold);

            // Outer crimson border
            using (var p = new Pen(Color.FromArgb(120, G_CrimDim), 1.5f))
                g.DrawRectangle(p, 1, 1, sz.Width - 3, sz.Height - 3);

            // Inner gold border
            using (var p = new Pen(Color.FromArgb(40, G_Gold), 0.8f))
                g.DrawRectangle(p, 6, 6, sz.Width - 13, sz.Height - 13);

            DrawCorners(g, sz.Width, sz.Height, G_Gold);
        }

        private static void DrawOutlastBg(Graphics g, Size sz)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Near-black with deep green center
            using (var bg = new PathGradientBrush(new PointF[]
            { new(0,0), new(sz.Width,0), new(sz.Width,sz.Height), new(0,sz.Height) }))
            {
                bg.CenterPoint    = new PointF(sz.Width / 2f, sz.Height * 0.3f);
                bg.CenterColor    = Color.FromArgb(255, 5, 22, 5);
                bg.SurroundColors = new[] { O_BgDark, O_BgDark, O_BgDark, O_BgDark };
                g.FillRectangle(bg, 0, 0, sz.Width, sz.Height);
            }

            // Green rule at top
            DrawGradientLine(g, sz.Width, 58, O_Green);

            // Outer green border
            using (var p = new Pen(Color.FromArgb(120, O_GreenDim), 1.5f))
                g.DrawRectangle(p, 1, 1, sz.Width - 3, sz.Height - 3);

            // Inner bright green border
            using (var p = new Pen(Color.FromArgb(50, O_Green), 0.8f))
                g.DrawRectangle(p, 6, 6, sz.Width - 13, sz.Height - 13);

            DrawCorners(g, sz.Width, sz.Height, O_Green);

            // Scanline effect — subtle horizontal lines
            using (var scanBrush = new SolidBrush(Color.FromArgb(12, 0, 0, 0)))
                for (int y = 0; y < sz.Height; y += 3)
                    g.FillRectangle(scanBrush, 0, y, sz.Width, 1);
        }

        private static void DrawPastelBg(Graphics g, Size sz)
        {
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = TextRenderingHint.AntiAlias;

            // Soft pink radial gradient background
            using (var bg = new PathGradientBrush(new PointF[]
            { new(0,0), new(sz.Width,0), new(sz.Width,sz.Height), new(0,sz.Height) }))
            {
                bg.CenterPoint    = new PointF(sz.Width / 2f, sz.Height * 0.3f);
                bg.CenterColor    = Color.FromArgb(255, 255, 235, 245);
                bg.SurroundColors = new[] { P_BgLight, P_BgLight, P_BgLight, P_BgLight };
                g.FillRectangle(bg, 0, 0, sz.Width, sz.Height);
            }

            // Pink rule at top
            DrawGradientLine(g, sz.Width, 58, P_Pink);

            // Outer pink border
            using (var p = new Pen(Color.FromArgb(120, P_PinkDim), 1.5f))
                g.DrawRectangle(p, 1, 1, sz.Width - 3, sz.Height - 3);

            // Inner lavender border
            using (var p = new Pen(Color.FromArgb(80, P_Lavender), 0.8f))
                g.DrawRectangle(p, 6, 6, sz.Width - 13, sz.Height - 13);

            // Scatter little hearts around the corners and edges
            DrawHeart(g, Color.FromArgb(60, P_Pink),  14,       14,       6);
            DrawHeart(g, Color.FromArgb(60, P_Lavender), sz.Width - 14, 14, 6);
            DrawHeart(g, Color.FromArgb(60, P_Mint),  14,       sz.Height - 14, 6);
            DrawHeart(g, Color.FromArgb(60, P_Pink),  sz.Width - 14, sz.Height - 14, 6);

            // A few extra subtle hearts scattered around
            DrawHeart(g, Color.FromArgb(30, P_PinkBrt), sz.Width / 4,      30,              4);
            DrawHeart(g, Color.FromArgb(30, P_Lavender), sz.Width * 3 / 4, 30,              4);
            DrawHeart(g, Color.FromArgb(25, P_Mint),    sz.Width / 2,      sz.Height - 22,  4);
        }

        private static void DrawHeart(Graphics g, Color col, int cx, int cy, int r)
        {
            using var b = new SolidBrush(col);
            // A heart made of two circles and a triangle
            g.FillEllipse(b, cx - r,     cy - r, r, r);
            g.FillEllipse(b, cx,         cy - r, r, r);
            g.FillPolygon(b, new PointF[]
            {
                new(cx - r,     cy - r / 2f),
                new(cx + r * 2, cy - r / 2f),
                new(cx + r / 2f, cy + r * 1.2f)
            });
        }

        private static void DrawGradientLine(Graphics g, int W, int y, Color col)
        {
            using (var lb = new LinearGradientBrush(
                new Point(0, y), new Point(W, y),
                Color.FromArgb(0, col), Color.FromArgb(0, col)))
            {
                var cb = new ColorBlend(5);
                cb.Colors    = new[] { Color.FromArgb(0, col), Color.FromArgb(100, col), Color.FromArgb(180, col), Color.FromArgb(100, col), Color.FromArgb(0, col) };
                cb.Positions = new[] { 0f, 0.2f, 0.5f, 0.8f, 1f };
                lb.InterpolationColors = cb;
                using (var pen = new Pen(lb, 1f))
                    g.DrawLine(pen, 30, y, W - 30, y);
            }
        }

        private static void DrawCorners(Graphics g, int W, int H, Color col)
        {
            using (var pen = new Pen(Color.FromArgb(100, col), 1f))
            {
                int m = 14, arm = 18;
                g.DrawLine(pen, m, m, m+arm, m);   g.DrawLine(pen, m, m, m, m+arm);
                g.DrawLine(pen, W-m, m, W-m-arm, m); g.DrawLine(pen, W-m, m, W-m, m+arm);
                g.DrawLine(pen, m, H-m, m+arm, H-m); g.DrawLine(pen, m, H-m, m, H-m-arm);
                g.DrawLine(pen, W-m, H-m, W-m-arm, H-m); g.DrawLine(pen, W-m, H-m, W-m, H-m-arm);
            }
            using (var b = new SolidBrush(Color.FromArgb(120, col)))
            {
                DrawDiamond(g, b, 14, 14, 3);   DrawDiamond(g, b, W-14, 14, 3);
                DrawDiamond(g, b, 14, H-14, 3); DrawDiamond(g, b, W-14, H-14, 3);
            }
        }

        private static void DrawDiamond(Graphics g, Brush b, int cx, int cy, int r)
            => g.FillPolygon(b, new PointF[] { new(cx, cy-r), new(cx+r, cy), new(cx, cy+r), new(cx-r, cy) });

        // ── Menu helpers (called from Form1) ──────────────────────────
        public static ToolStripRenderer GetMenuRenderer(ThemeMode mode) => mode switch
        {
            ThemeMode.Gothic   => new GothicMenuRenderer(),
            ThemeMode.Outlast  => new OutlastMenuRenderer(),
            ThemeMode.Pastel   => new PastelMenuRenderer(),
            _                  => new OriginalMenuRenderer()
        };

        public static Color GetMenuBackground(ThemeMode mode) => mode switch
        {
            ThemeMode.Gothic  => Color.FromArgb(18, 10, 18),
            ThemeMode.Outlast => Color.FromArgb(8, 14, 8),
            ThemeMode.Pastel  => Color.FromArgb(255, 228, 238),
            _                 => Color.White
        };

        public static Color GetMenuForeground(ThemeMode mode) => mode switch
        {
            ThemeMode.Gothic  => Color.FromArgb(220, 215, 210),
            ThemeMode.Outlast => Color.FromArgb(200, 240, 200),
            ThemeMode.Pastel  => Color.FromArgb(120, 60, 90),
            _                 => Color.FromArgb(30, 30, 30)
        };

        public static Font GetMenuFont(ThemeMode mode) => mode switch
        {
            ThemeMode.Gothic  => SafeFont("Palatino Linotype", 10f, FontStyle.Regular),
            ThemeMode.Outlast => SafeFont("Roboto Mono", 10f, FontStyle.Regular),
            ThemeMode.Pastel  => SafeFont("Comic Sans MS", 10f, FontStyle.Regular),
            _                 => SafeFont("Segoe UI", 10f, FontStyle.Regular)
        };

        // ── Helpers ───────────────────────────────────────────────────
        private static Font SafeFont(string family, float size, FontStyle style)
        {
            try { return new Font(family, size, style, GraphicsUnit.Point); }
            catch
            {
                foreach (var f in new[] { "Segoe UI", "Arial", "Tahoma" })
                    try { return new Font(f, size, style); } catch { }
                return SystemFonts.DefaultFont;
            }
        }

        private static Control[] GetAll(Control top)
            => top.Controls.Cast<Control>()
                .SelectMany(c => new[] { c }.Concat(GetAll(c)))
                .ToArray();

        private static void SetDoubleBuffered(Control c, bool on)
        {
            try
            {
                typeof(Control)
                    .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.SetValue(c, on, null);
            }
            catch { }
        }

        // Keep old entry point for compatibility
        public static void ApplyDark(Form f, bool _ = false) => Apply(f, ThemeMode.Gothic);
    }
}
