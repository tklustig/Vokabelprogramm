using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System;
using System.Runtime.InteropServices;

namespace Vokabelprogramm
{
    public partial class Author : Form
    {
        [DllImport("user32.dll")]
        public static extern int ShowWindow(int Wnd, int Flags);
        [DllImport("user32.dll")]
        public static extern int FindWindow(string strCName, string strWndName);
        private System.ComponentModel.IContainer components;

        public Author()
        {
            InitializeComponent();

        }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Author));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form2
            // 
            resources.ApplyResources(this, "$this");
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseClick);
            this.ResumeLayout(false);

        }
        #endregion
        private System.Windows.Forms.Timer timer1;
        private float angle = 0;

        private LinearGradientBrush GetBrush()
        {
            return new LinearGradientBrush(
              new Rectangle(1, 1, 1024, 700),
              Color.Red,
              Color.Yellow,
              0.0F,
              true);
        }

        private void Rotate(Graphics graphics, LinearGradientBrush brush)
        {
            brush.RotateTransform(angle);
            brush.SetBlendTriangularShape(.5F);
            graphics.FillRectangle(brush, brush.Rectangle);
        }

        private void Rotate(Graphics graphics)
        {
            angle += 5 % 360;
            Rotate(graphics, GetBrush());
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            Rotate(CreateGraphics());
            Graphics g = this.CreateGraphics();
            Icon IconPhone = new Icon("phone.ico");
            Icon IconUrl = new Icon("web.ico");
            Icon IconMail = new Icon("mail.ico");
            using (Font bigFont = new Font(SystemFonts.DefaultFont.FontFamily, 28, FontStyle.Regular))
            using (Font mediumFont = new Font(SystemFonts.DefaultFont.FontFamily, 18, FontStyle.Regular))
            using (Font smallFont = new Font(SystemFonts.DefaultFont.FontFamily, 14, FontStyle.Regular))
            using (Font VsmallFont = new Font(SystemFonts.DefaultFont.FontFamily, 10, FontStyle.Regular))
            {
                g.DrawString("Vokabeltrainer", bigFont, Brushes.Green, 30, 30);
                g.DrawString("© by Thomas Kipp", mediumFont, Brushes.Blue, 30, 70);
                g.DrawIcon(IconMail, 20, 140);
                g.DrawIcon(IconPhone, 25, 165);
                g.DrawIcon(IconUrl, 20, 195);
                g.DrawString("Contact:" + Environment.NewLine +
                "    t.kipp@eisvogel-online-software.de" + Environment.NewLine +
                "    0152/37389041" + Environment.NewLine +
                "    http://tklustig.ddns.net:1026", mediumFont, Brushes.Black, 30, 110);
                g.DrawString("programming language: C# (WindowsForms)", smallFont, Brushes.Turquoise, 30, 225);
                g.DrawString("click me to get closed", VsmallFont, Brushes.Beige, 30, 255);
            }
            ShowWindow(FindWindow("Progman", "Program Manager"), 1);
        }
        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Rotate(e.Graphics);
        }

        private void Form2_MouseClick(object sender, MouseEventArgs e)
        {
            timer1.Enabled = false;
            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            {
                speechSynthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Child);
                speechSynthesizer.Rate = -2;
                speechSynthesizer.Speak("For questions, please contact me by phone, or by email");
            }
            this.Close();
        }
    }
}

