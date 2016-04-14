using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeeingSharp.FontSymbolExtractor
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            m_cmbFont.Items.Clear();
            m_cmbFont.Items.AddRange(FontFamily.Families);
        }

        private void OnCmdConvert_Click(object sender, EventArgs e)
        {
            FontFamily selectedFamily = m_cmbFont.SelectedItem as FontFamily;
            if(selectedFamily == null) { return; }

            int bitmapWidth = 0;
            int bitmapHeight = 0;
            if(!Int32.TryParse(m_txtSizeWidth.Text, out bitmapWidth)) { return; }
            if(!Int32.TryParse(m_txtSizeHeight.Text, out bitmapHeight)) { return; }

            if(m_dlgSelectTargetFolder.ShowDialog(this) == DialogResult.OK)
            {
                string targetDir = m_dlgSelectTargetFolder.SelectedPath;

                using (SolidBrush foreBrush = new SolidBrush(System.Drawing.Color.Black))
                using (SolidBrush backBrush = new SolidBrush(System.Drawing.Color.Transparent))
                using (Bitmap targetBitmap = new Bitmap(bitmapWidth, bitmapHeight))
                using (Graphics bitmapGraphics = Graphics.FromImage(targetBitmap))
                using (Font newFont = new Font(selectedFamily, 16f))
                {
                    bitmapGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    bitmapGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                    bitmapGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    bitmapGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    for (ushort loopChar = 0xE000; loopChar < 0xE21E; loopChar++)
                    {
                        bitmapGraphics.Clear(System.Drawing.Color.Transparent);
                        bitmapGraphics.DrawString(
                            Encoding.Unicode.GetString(BitConverter.GetBytes(loopChar)),
                            newFont,
                            foreBrush,
                            new PointF(-1f, 1f));
                        bitmapGraphics.Flush();

                        targetBitmap.Save(Path.Combine(targetDir, "Icon_" + loopChar + ".png"));
                    }
                }
            }

        }
    }
}
