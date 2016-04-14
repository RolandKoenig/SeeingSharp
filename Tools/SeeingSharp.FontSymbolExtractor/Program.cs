using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

// Namespace mappings
using GDI = System.Drawing;

namespace SeeingSharp.FontSymbolExtractor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Common settings (32x32 Pixel)
            int bitmapWidthPx = 64;
            int bitmapHeightPx = 64;
            float symbolOriginX = -8f;
            float symbolOriginY = -2f;
            float fontSizePt = 45f;
            ushort charCodeStart = 0xE000;
            ushort charCodeEnd = 0xE21D;
            string fontName = "SAP-icons";
            GDI.Color backColor = GDI.Color.Transparent;
            GDI.Color foreColor = GDI.Color.Black;
            string foreColorName = nameof(GDI.Color.Black);

            string targetDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Generated Icons",
                $"{fontName} {bitmapWidthPx}x{bitmapHeightPx} {foreColorName}");

            // Select font
            FontFamily selectedFamily = FontFamily.Families.FirstOrDefault(
                (actFamily) =>
                    actFamily.Name.IndexOf(fontName, StringComparison.OrdinalIgnoreCase) != -1);
            if (selectedFamily == null)
            {
                Console.WriteLine("SAP Font family not found!");
                return;
            }

            // Ensure target directory
            if (!Directory.Exists(targetDir)) { Directory.CreateDirectory(targetDir); }
            
            using (SolidBrush foreBrush = new SolidBrush(foreColor))
            using (SolidBrush backBrush = new SolidBrush(backColor))
            using (Bitmap targetBitmap = new Bitmap(bitmapWidthPx, bitmapHeightPx))
            using (Graphics bitmapGraphics = Graphics.FromImage(targetBitmap))
            using (Font newFont = new Font(selectedFamily, fontSizePt))
            {
                bitmapGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                bitmapGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                bitmapGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                bitmapGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                for (ushort loopChar = charCodeStart; loopChar <= charCodeEnd; loopChar++)
                {
                    bitmapGraphics.Clear(backColor);
                    bitmapGraphics.DrawString(
                        Encoding.Unicode.GetString(BitConverter.GetBytes(loopChar)),
                        newFont,
                        foreBrush,
                        new PointF(symbolOriginX, symbolOriginY));
                    bitmapGraphics.Flush();
                    bitmapGraphics.Flush();
                    bitmapGraphics.Flush();

                    targetBitmap.Save(Path.Combine(targetDir, "Icon_" + loopChar + ".png"));
                }
            }

            Console.WriteLine("Finished writing icons..");
        }
    }
}
