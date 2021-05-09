using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Net;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Windows.Forms.DataVisualization.Charting;

namespace Vokabelprogramm
{
    public partial class Main : Form
    {
        //Ab hier werden Klassenmember definiert

        //instanziert ein Objekt der Klasse SoundPlayer. Konstruktorangaben für diese Klasse nicht erforderlich
        private SoundPlayer Player = new SoundPlayer();
        //initialisiert das Testüberschriftenlabel. Der Inhalt dieses Labels wird in zwei gekapselten Methoden benötigt
        private Label LabelTestHead;
        //erstellt zwei generische Datentypen, in welche die Vokabeln verfrachtet werden
        private List<string> InhaltDeutsch = new List<string>();
        private List<string> InhaltForeign = new List<string>();
        //initialisert eine Variable, die die für den Test benötigte Zeit aufnimmt. Wird in mehreren Methoden benötigt
        private double TimeForTest;
        //dient zur "Erinnerung" an die geladene Datei der Testoption
        private string RememberFilename_Test;
        //dient zur "Erinnerung" an die geladene Datei der Editoption
        private string RememberFilename_Edit;
        //legt fest, ob die GridView_Edit bereits erstellt wurde, oder und nur angezeigt werden muss
        private bool CheckGridEdit = false;
        //legt fest, ob die GridView_Statistic bereits erstellt wurde, oder und nur angezeigt werden muss
        private bool CheckGridStat = false;
        //legt fest, ob die GridView_Test bereits erstellt wurde, oder und nur angezeigt werden muss
        private bool CheckGridTest = false;
        //legt fest, ob die GridView_StatisticList bereits erstellt wurde, oder und nur angezeigt werden muss
        private bool CheckGridStatisticList = false;
        //legt fest, ob die DataGridView_ErrorList bereits erstellt wurde, oder und nur angezeigt werden muss
        private bool CheckGridErrorList = false;
        //legt fest, ob das Click-Ereignis aufgerufen werden soll, oder nicht
        private bool CheckClickStatus;
        //legt fest, welche Ausgabe in der MessageBox erfolgen soll
        private bool CheckAusgabe = false;
        //legt fest, ob die Infoboxen abgerufen werden sollen
        private bool ShowInfo = true;
        //da die C#-Methode Exists() in dieser Applikation nicht zuverlässige Werte zurückliefert, wurden diese Variablen initialisert. Sie dienen zur Absicherung 
        private bool CreateDirectory = true;
        private bool CreateDirectory_WVoc = true;
        //instanziert jeweils ein Objekt der Klasse DataGridView, in welcher die geladenen Vokabeln angezeigt werden
        private DataGridView DataGridView_Edit = new DataGridView();
        private DataGridView DataGridView_Test = new DataGridView();
        //instanziert ein Objekt der Klasse DataGridView,in welcher die fehlerhaft eingegebenen Vokabeln und derren korrekte Bezeichnnug angezeigt werden
        private DataGridView DataGridView_Statistic = new DataGridView();
        //instanziert ein Objekt der Klasse DataGridView,in welcher die gesicherten Statistikdaten angezeigt werden 
        private DataGridView DataGridView_StatisticList = new DataGridView();
        //instanziert ein Objekt der Klasse DataGridView,in welche die gesicherten gesuchten, eingegegebenen und fehlerhaften Vokabeln angezeigt werden
        private DataGridView DataGridView_ErrorList = new DataGridView();
        private static string userName = Environment.UserName;
        //instanziert ein Objekt der Klasse DirectoryInfo mit obigem Parameter für den Konstruktor der Klasse DirectInfo
        private DirectoryInfo Main_Directory = new DirectoryInfo(@"C:\Users\" + userName + @"\Documents\Vokabelprogramm\Listen\");
        //instanziert ein Objekt der Klasse DirectoryInfo mit obigem Parameter für den Konstruktor der Klasse DirectInfo
        private DirectoryInfo Directory_Statistic = new DirectoryInfo(@"C:\Users\" + userName + @"\Documents\Vokabelprogramm\Stat\");
        //initialisert die Datei, in welche die nicht gewussten Vokabeln &Co. auf Bedarf gesichert werden
        private string filename = "VocList.wVoc";
        //initialisert die Datei, in welche die Statistikdaten auf Bedarf gesichert werden
        string filename_ = "statistic.stat";
        //instanziert ein Objekt der Klasse DirectoryInfo mit obigem Parameter für den Konstruktor der Klasse DirectInfo
        private DirectoryInfo Directory_ErrorVoc = new DirectoryInfo(@"C:\\Users\" + userName + "\\Documents\\Vokabelprogramm\\ErrorVoc");
        //die Vokabelanzahl extern verfügbar machen, da Ihr Wert in mehreren Methoden benötigt wird
        private int anzahl;
        //die Stoppuhr extern verfügbar machen, da Ihr Wert in mehreren Methoden benötigt wird
        private Stopwatch Time = new Stopwatch();
        //den Testzähler extern verfügbar machen, da der Wert in mehreren Methoden benötigt wird
        private int CountMyRound = 0;
        //Ende der Klassenmemberdefinierung


        //Konstruktor der Klasse
        public Main() {
            InitializeComponent();
            this.Deactivation(3);
            if (!Directory.Exists(Main_Directory.ToString())) {
                Directory.CreateDirectory(Main_Directory.ToString());
            }
        }

        //QUIT: About_Quit-Ereignis des Menus
        private void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        //ABOUT: About_Author-Ereignis des Menus
        private void authorToolStripMenuItem_Click(object sender, EventArgs e) {
            Author fm = new Author();
            fm.Show();
        }

        //HELP: About_Help-Ereignis des Menus
        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Deactivation(0);
            textBox1.Text = " Über das Menu lassen sich folgende Optionen aufrufen:" + Environment.NewLine + Environment.NewLine +
                "1.1: Vokabeln editieren: Lädt eine vorhandene Vokabelliste, um sie zu editieren. Dazu klicken Sie in die Zelle, um den Wert zu verändern. Eine Übernahme der Änderung erfolgt durch den Submitbutton." + Environment.NewLine +
                "1.2: Vokabeln eingeben: Eröffnet die Möglichkeit, Vokabeln einzugeben bzw. eine Vokabelliste zu erstellen. Die von Ihnen eingegebene Vokabelanzahl muss eingehalten werden." + Environment.NewLine +
                "1.3: Vokabeln löschen: Löscht eine Vokabelliste Ihrer Wahl" + Environment.NewLine +
                "1.4: Test starten: Startet einen Vokabeltest und bewertet ihn abschließend. Dazu muss vorher eine Vokabelliste geladen werden. Außerdem können Sie über Radiobuttons bestimmen, welche Vokabelrubrik abgefragt werden soll. Sofern Sie die CheckBox aktiviert haben, wird die Zeit gemessen:Innerhalb einer bestimmten Zeitspanne muss der Test beendet werden, ansonsten interveniert die Applikation. Sie haben am Ende die Möglichkeit, sowohl die Auswertung als auch Ihre falsch eingegebenen Begriffe kumulativ(!) zu sichern." + Environment.NewLine +
                "1.5: Statistik:  Lädt und zeigt die Statistikdaten an. Sofern Sie nach der Auswertung keine Sicherung vorgenommen haben, können sie diesen Menupunkt nicht ausführen; sie werden benachrichtigt." + Environment.NewLine +
                "1.6: Fehlerliste: Lädt und zeigt folgendes an: Den gesuchten Begriff, die eingegegebenen, fehlerhaften Vokabeln und die korrekte Vorgabe. Sofern Sie nach der Auswertung keine Sicherung vorgenommen haben, können sie diesen Menupunkt nicht ausführen; sie werden benachrichtigt." + Environment.NewLine +
                "1.7: Statistik löschen: Löscht alle Daten der Statistikliste." + Environment.NewLine +
                "1.8: Fehlerliste löschen: Löscht alle Daten der Fehlerliste," + Environment.NewLine +
                "2.1: Author: Blendet eine animierte Darstellung der Softwareversion ein." + Environment.NewLine +
                "2.2: Quit: Beendet die Applikation." + Environment.NewLine +
                "2.3: Ton: Zu Beginn ist der Sound deaktiviert. Dieser Menupunkt aktiviert und deaktiviert, je nach Status, das Abspielen. Die Sounddatei befindet sich online auf meinem Pi; sie werden informiert, sobald die Sounddatei zur Verfügung steht. Wenn Sie offline sind, kann der Sound nicht abgespielt werden" + Environment.NewLine +
                "2.4: Infoboxen: Zu Beginn werden Infoboxen standardmäßig aktiviert. Wenn Sie mit der Applikation vertraut sind, empfiehlt es sich, die Infoboxen zu deaktivieren.";
            ;
            if (ShowInfo) {
                using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer()) {
                    speechSynthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
                    speechSynthesizer.Rate = -1;
                    speechSynthesizer.Speak("Welcome to help option of this C# application");
                }
            }
        }

        //INFO: Aktiviert bzw. Deaktiviert die Anzeige der Messageboxen
        private void infoboxenDeaktivierenToolStripMenuItem_Click(object sender, EventArgs e) {
            if (infoboxenDeaktivierenToolStripMenuItem.Text == "Infoboxen deaktivieren") {
                infoboxenDeaktivierenToolStripMenuItem.Text = "Infoboxen aktivieren";
                ShowInfo = false;
            } else {
                infoboxenDeaktivierenToolStripMenuItem.Text = "Infoboxen deaktivieren";
                ShowInfo = true;
            }
        }

        //TON: About_TonAktivieren-Ereignis des Menus
        private void tonAbspielenToolStripMenuItem_Click(object sender, EventArgs e) {

            try {
                if (tonAbspielenToolStripMenuItem.Text == "Ton aktivieren") {
                    Player.SoundLocation = "https://tklustig.de/tetris.wav";
                    Player.LoadAsync();
                    Uri url = new Uri("https://tklustig.de/");
                    if (Player.IsLoadCompleted && this.CheckHost(url))
                        MessageBox.Show("Sounddatei wurde erfolgreich aus dem Webspace des Raspbbery Pi geladen!");
                    else {
                        //MessageBox.Show("Sounddatei konnte nicht geladen werden! Einzelheiten unter About/Help(2.2)", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        using (SoundPlayer player = new SoundPlayer(Properties.Resources.tetris)) {
                            player.LoadAsync();
                            player.Play();
                        }
                    }
                    //Player.Play();
                    tonAbspielenToolStripMenuItem.Text = "Ton deaktivieren";
                } else {
                    tonAbspielenToolStripMenuItem.Text = "Ton aktivieren";
                    Player.Stop();
                }
            } catch (Exception er) {
                MessageBox.Show("Da Sie offline sind, kann der Sound nicht geladen werden..." + Environment.NewLine + Environment.NewLine + er.ToString(), "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        //EINGEBEN: Datei_VokabelnEingeben-Ereignis des Menus
        private void vokabelnEingebenToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Deactivation(3);
            label1.Visible = true;
            trackBar1.Visible = true;
            textBox2.Visible = true;
            button1.Visible = true;
        }

        //LOAD: Datei_VokabelnLaden-Ereignis des Menus
        private void vokabelnLadenToolStripMenuItem_Click(object sender, EventArgs e) {
            //sorgt dafür, dass das Clickereignis ausgelöst wird. Ist standardmäßig auf false gesetzt, wurde jedoch mitunter an anderer Stelle auf true gesetzt
            CheckClickStatus = false;
            if (ShowInfo)
                MessageBox.Show("Wählen Sie die zu editierende Datei aus.", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.InitialDirectory = Main_Directory.ToString();
            OpenFileDialog.Filter = "voca files (*.voca)|*.voca";
            if (OpenFileDialog.ShowDialog() != DialogResult.Cancel) {
                if ((OpenFileDialog.FileName) != null) {
                    this.Deactivation(3);
                    if (!CheckGridEdit) {
                        this.SetupDataGridView_Edit();
                        CheckGridEdit = true;
                    } else {
                        DataGridView_Edit.Rows.Clear();
                        DataGridView_Edit.Visible = true;
                    }
                    var zeilen = File.ReadAllLines(OpenFileDialog.FileName);
                    int haelfte = zeilen.Length / 2;
                    for (int i = 0; i < haelfte; i++) {
                        DataGridView_Edit.Rows.Add(i + 1, zeilen[i], zeilen[haelfte + i]);
                    }
                    RememberFilename_Edit = OpenFileDialog.FileName;
                }
            }
            if (OpenFileDialog.FileName.Length > 1 && ShowInfo)
                MessageBox.Show("Beachten Sie die Informationen unter der Menuoption About/Help(1.1)", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            else if (OpenFileDialog.FileName.Length > 1 && !ShowInfo)
                return;
        }

        //DELETE: Datei_VokabellisteLöschen-Ereignis des Menus
        private void ListeLoeschenToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Deactivation(3);
            if (ShowInfo)
                MessageBox.Show("Wählen Sie die zu löschende Datei aus", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.InitialDirectory = Main_Directory.ToString();
            OpenFileDialog.Filter = "voca files (*.voca)|*.voca";
            if (OpenFileDialog.ShowDialog() == DialogResult.OK) {
                if ((OpenFileDialog.FileName) != null) {
                    File.Delete(OpenFileDialog.FileName);
                    if (ShowInfo)
                        MessageBox.Show("Die Datei " + OpenFileDialog.FileName + " wurde soeben gelöscht!");
                }
            }
        }

        //TEST: Datei_TestStarten-Ereignis des Menus
        private void TestStartenToolStripMenuItem_Click(object sender, EventArgs e) {
            timer1.Enabled = false;
            //sorgt dafür, dass das Clickereignis nicht ausgelöst wird
            CheckClickStatus = true;
            string ausgabe = "";
            Button TestButton;
            Label LabelInfo;
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.InitialDirectory = Main_Directory.ToString();
            OpenFileDialog.Filter = "voca files (*.voca)|*.voca";
            int anzahlVoc = 0;
            if (!CheckAusgabe) {
                ausgabe = "Es wurde noch keine Vokabelliste geladen. Soll eine Liste geladen werden?";
            } else
                ausgabe = "Es wurde bereits eine Vokabelliste geladen. Soll eine neue Liste geladen werden?";

            if (!CheckAusgabe) {
                if (MessageBox.Show(ausgabe, "Hint", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    if (OpenFileDialog.ShowDialog() != DialogResult.Cancel) {
                        if ((OpenFileDialog.FileName) != null) {
                            this.Deactivation(3);
                            if (!CheckGridTest) {
                                this.SetupDataGridView_Test();
                                CheckGridTest = true;
                                DataGridView_Test.Visible = false;
                            } else {
                                DataGridView_Test.Rows.Clear();
                            }
                            var zeilen = File.ReadAllLines(OpenFileDialog.FileName);
                            int haelfte = zeilen.Length / 2;
                            for (int i = 0; i < haelfte; i++) {
                                DataGridView_Test.Rows.Add(i + 1, zeilen[i], zeilen[haelfte + i]);
                            }
                            RememberFilename_Test = OpenFileDialog.FileName;
                            anzahlVoc = DataGridView_Test.Rows.Count;
                            if (ShowInfo)
                                MessageBox.Show("Die Testliste wurde geladen. Sie enthält " + anzahlVoc + " Vokabeln.");
                            DataGridView_Test.Visible = true;
                        }
                    } else {
                        if (ShowInfo)
                            MessageBox.Show("Solange keine Liste geladen wurde, kann der Test nicht gestartet werden!", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        CheckAusgabe = false;
                        return;
                    }
                } else {
                    if (ShowInfo)
                        MessageBox.Show("Solange keine Liste geladen wurde, kann der Test nicht gestartet werden!", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    CheckAusgabe = false;
                    return;
                }
                CheckAusgabe = true;
            } else {
                if (MessageBox.Show(ausgabe, "Hint", MessageBoxButtons.YesNo) == DialogResult.Yes && OpenFileDialog.ShowDialog() != DialogResult.Cancel) {
                    if (OpenFileDialog.FileName != null) {
                        this.Deactivation(3);
                        if (!CheckGridTest)
                            this.SetupDataGridView_Test();
                        else {
                            DataGridView_Test.Rows.Clear();
                        }
                        var zeilen = File.ReadAllLines(OpenFileDialog.FileName);
                        int haelfte = zeilen.Length / 2;
                        for (int i = 0; i < haelfte; i++) {
                            DataGridView_Test.Rows.Add(i + 1, zeilen[i], zeilen[haelfte + i]);
                        }
                        RememberFilename_Test = OpenFileDialog.FileName;
                    }
                    anzahlVoc = DataGridView_Test.Rows.Count;
                    if (ShowInfo)
                        MessageBox.Show("Die neue Testliste wurde geladen. Sie enthält " + anzahlVoc + " Vokabeln.");
                    DataGridView_Test.Visible = true;
                } else {
                    this.Deactivation(3);
                    DataGridView_Test.Rows.Clear();
                    var zeilen = File.ReadAllLines(RememberFilename_Test);
                    int haelfte = zeilen.Length / 2;
                    for (int i = 0; i < haelfte; i++) {
                        DataGridView_Test.Rows.Add(i + 1, zeilen[i], zeilen[haelfte + i]);
                    }
                    anzahlVoc = DataGridView_Test.Rows.Count;
                    if (ShowInfo)
                        MessageBox.Show("Die alte Liste wird weiterverwendet. Sie enthält " + anzahlVoc + " Vokabeln.");
                    DataGridView_Test.Visible = true;
                }
            }
            LabelInfo = new Label();
            LabelInfo.Location = new Point(100, 60);
            LabelInfo.AutoSize = true;
            LabelInfo.Text = "Folgende Vokabeln werden durch einen Zufallsgenerator abgefragt," + Environment.NewLine + "indem Sie den grünen Button pushen.";
            this.Controls.Add(LabelInfo);
            TestButton = new Button();
            TestButton.Size = new Size(50, 23);
            TestButton.Location = new Point(100, 425);
            TestButton.Text = "Test starten";
            TestButton.BackColor = Color.ForestGreen;
            TestButton.FlatStyle = FlatStyle.Popup;
            this.Controls.Add(TestButton);
            TestButton.Click += new EventHandler(TestButton_Click);
            label2.Visible = true;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            /*verfrachtet die in die GridView geladenen Vokabeln in die Listen. Dazu müssen diue Listen vorher entleert werden*/
            InhaltDeutsch.Clear();
            InhaltForeign.Clear();
            var GridInhalt = File.ReadAllLines(RememberFilename_Test);
            for (int i = 0; i < anzahlVoc; i++) {
                InhaltDeutsch.Add(GridInhalt[i]);
            }
            for (int i = anzahlVoc; i < anzahlVoc * 2; i++) {
                InhaltForeign.Add(GridInhalt[i]);
            }
            anzahl = anzahlVoc;
            label6.Visible = true;
            checkBox1.Visible = true;
            /*Jetzt stehen die Vokabeln in den Listen zum Test bereit. Dazu muss ein geeigneter Abfragealgorithums in TestAuswerten geschrieben werden*/
        }

        //zeigt den Wert der Trackbar in einem label an
        private void trackBar1_Scroll(object sender, EventArgs e) {
            label1.Text = trackBar1.Value.ToString();
        }

        //regelt den Ablauf, nachdem auf Submitbutton1 gedrückt wurde: Erstellt die Textboxen. Dieser Button wurde im Designer generiert
        private void button1_Click(object sender, EventArgs e) {
            //aufgrund einer Eigenwilligkeit von C# muss von hinten nach vorne iteriert werden: Entfernt zunächst alle Textboxen....
            for (int i = this.Controls.Count - 1; i >= 0; i--) {
                Control t = this.Controls[i];
                if (t.GetType() == typeof(TextBox)) {
                    TextBox tb = (TextBox)t;
                    if (tb.Name.StartsWith("MYTEXTBOX")) {
                        this.Controls.Remove(tb);
                    }
                }
            }
            this.Deactivation(1);
            button2.Visible = true;
            textBox3.Visible = true;
            //.... um sie sodann neu zu generieren
            for (int i = 1; i <= trackBar1.Value; i++) {
                TextBox textbox = new TextBox();
                textbox.Name = String.Format("MYTEXTBOX_GERMAN{0}", i);
                textbox.Location = new Point(400, 150 + 30 * i);
                this.Controls.Add(textbox);
            }
            for (int i = 1; i <= trackBar1.Value; i++) {
                TextBox textbox = new TextBox();
                textbox.Name = String.Format("MYTEXTBOX_FOREIGN{0}", i);
                textbox.Location = new Point(600, 150 + 30 * i);
                this.Controls.Add(textbox);
            }
            foreach (Control t in this.Controls) {
                if (t.GetType() == typeof(TextBox)) {
                    TextBox tb = (TextBox)t;
                    if (tb.Name == "MYTEXTBOX_GERMAN1") {
                        tb.Focus();
                    }
                }
            }
        }

        /*regelt den Ablauf, nachdem auf Submitbutton2 gedrückt wurde: Validiert die Eingaben und verfrachtet dieselbigen in die generischen Datentypen. Dieser Button
        wurde im Designer generiert*/
        private void button2_Click(object sender, EventArgs e) {
            InhaltDeutsch.Clear();
            InhaltForeign.Clear();
            //Auf sämtliche dynamische Textboxen zugreifen
            foreach (Control t in this.Controls) {
                if (t.GetType() == typeof(TextBox)) {
                    TextBox tb = (TextBox)t;
                    if (tb.Name.StartsWith("MYTEXTBOX_GERMAN")) {
                        InhaltDeutsch.Add(tb.Text);
                    } else {
                        if (tb.Name.StartsWith("MYTEXTBOX_FOREIGN")) {
                            InhaltForeign.Add(tb.Text);
                        }
                    }
                    if (tb.Name.StartsWith("MYTEXTBOX") && tb.Text.Length == 0) {
                        tb.BackColor = Color.Red;
                        tb.Select();
                        if (ShowInfo) {
                            MessageBox.Show("Das rote Feld darf nicht leer sein", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        }
                        return;
                    } else {
                        tb.BackColor = Color.White;
                    }
                }

            }
            this.SaveData();
        }

        //blendet,abhängig vom übergebenen Parameter, verschiedene Steuerelemente aus 
        private void Deactivation(int param) {
            foreach (Control t in this.Controls) {
                if (t.GetType() == typeof(TextBox)) {
                    TextBox tb = (TextBox)t;
                    tb.Visible = false;
                    tb.Enabled = false;
                }
                if (t.GetType() == typeof(DataGridView)) {
                    DataGridView dg = (DataGridView)t;
                    dg.Visible = false;
                }
                if (t.GetType() == typeof(Label)) {
                    Label lb = (Label)t;
                    lb.Visible = false;
                }
                if (t.GetType() == typeof(Button)) {
                    Button bt = (Button)t;
                    bt.Visible = false;
                }
                if (t.GetType() == typeof(TrackBar)) {
                    TrackBar ttb = (TrackBar)t;
                    ttb.Visible = false;
                }
                if (t.GetType() == typeof(ProgressBar)) {
                    ProgressBar pgb = (ProgressBar)t;
                    pgb.Visible = false;
                }
                if (t.GetType() == typeof(RadioButton)) {
                    RadioButton rbt = (RadioButton)t;
                    rbt.Visible = false;
                }
                if (t.GetType() == typeof(CheckBox)) {
                    CheckBox cb = (CheckBox)t;
                    cb.Visible = false;
                }
                if (t.GetType() == typeof(ListBox)) {
                    ListBox lb = (ListBox)t;
                    lb.Visible = false;
                }
                if (param == 0) {
                    textBox1.Visible = true;
                } else if (param == 1) {
                    label3.Visible = true;
                    label4.Visible = true;
                }
                pictureBox1.Visible = false;
            }
        }

        //speichert die Usereingaben bzw. den Inhalt der generischen Datentypen in einer Datei
        private bool SaveData() {
            try {
                bool vorhanden = true;
                string filename = "";
                if (!Main_Directory.Exists) {
                    if (MessageBox.Show("Um eine neue Liste anzulegen, muss das Verzeichnis " + Main_Directory + " neu angelegt werden. Wollen Sie das Verzeichnis anlegen?", "Hint", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        DirectoryInfo info = Directory.CreateDirectory(Main_Directory.ToString());
                    } else {
                        MessageBox.Show("Liste kan nicht angelegt werden!", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        return false;
                    }
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "voca files (*.voca)|*.voca";
                if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                    if ((saveFileDialog.FileName) != null) {
                        filename = saveFileDialog.FileName;
                        if (File.Exists(filename))
                            vorhanden = true;
                        else
                            vorhanden = false;
                        File.WriteAllLines(filename, InhaltDeutsch);
                        File.AppendAllLines(filename, InhaltForeign);


                    }
                    if (vorhanden && ShowInfo)
                        MessageBox.Show(filename + " wurde soeben ersetzt");
                    else if (!vorhanden && ShowInfo)
                        MessageBox.Show(filename + " wurde soeben erstellt");
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        //Initialisiert die GridView für das Editieren
        private void SetupDataGridView_Edit() {
            this.Controls.Add(DataGridView_Edit);
            DataGridView_Edit.ColumnCount = 3;
            DataGridView_Edit.Name = "DataGridView_Edit";
            DataGridView_Edit.Location = new Point(100, 100);
            DataGridView_Edit.Size = new Size(340, 300);
            DataGridView_Edit.ReadOnly = false;
            DataGridView_Edit.AllowUserToDeleteRows = false;
            DataGridView_Edit.AllowUserToAddRows = false;
            DataGridView_Edit.Columns[0].Name = "Number";
            DataGridView_Edit.Columns[1].Name = "Deutsch";
            DataGridView_Edit.Columns[2].Name = "Fremdwort";
            DataGridView_Edit.CellClick += new DataGridViewCellEventHandler(this.DataGridView_CellClick);
        }

        //Initialisiert die GridView für den Test
        private void SetupDataGridView_Test() {
            this.Controls.Add(DataGridView_Test);
            DataGridView_Test.ColumnCount = 3;
            DataGridView_Test.Name = "DataGridView_Test";
            DataGridView_Test.Location = new Point(100, 100);
            DataGridView_Test.Size = new Size(340, 300);
            DataGridView_Test.ReadOnly = true;
            DataGridView_Test.AllowUserToDeleteRows = false;
            DataGridView_Test.AllowUserToAddRows = false;
            DataGridView_Test.Columns[0].Name = "Number";
            DataGridView_Test.Columns[1].Name = "Deutsch";
            DataGridView_Test.Columns[2].Name = "Fremdwort";
        }

        //Initialisiert die GridView für die Statistik
        private void SetupDataGridView_Statistic() {
            this.Controls.Add(DataGridView_Statistic);
            DataGridView_Statistic.ColumnCount = 4;
            DataGridView_Statistic.Name = "DataGridView_Statistic";
            DataGridView_Statistic.Location = new Point(250, 350);
            DataGridView_Statistic.Size = new Size(440, 300);
            DataGridView_Statistic.ReadOnly = true;
            DataGridView_Statistic.AllowUserToDeleteRows = false;
            DataGridView_Statistic.AllowUserToAddRows = false;
            DataGridView_Statistic.Columns[0].Name = "Number";
            DataGridView_Statistic.Columns[1].Name = "Abgefragt";
            DataGridView_Statistic.Columns[2].Name = "Eingegeben";
            DataGridView_Statistic.Columns[3].Name = "Korrekt";
        }

        //Initialisiert die GridView für die gesicherte Statistikliste
        private void SetupDataGridView_StatisticListe() {
            this.Controls.Add(DataGridView_StatisticList);
            DataGridView_StatisticList.ColumnCount = 7;
            DataGridView_StatisticList.Name = "DataGridView_StatisticList";
            DataGridView_StatisticList.Location = new Point(50, 50);
            DataGridView_StatisticList.Size = new Size(900, 300);
            DataGridView_StatisticList.ReadOnly = true;
            DataGridView_StatisticList.AllowUserToDeleteRows = false;
            DataGridView_StatisticList.AllowUserToAddRows = false;
            DataGridView_StatisticList.Columns[0].Name = "DateTime";
            DataGridView_StatisticList.Columns[1].Name = "Gesucht";
            DataGridView_StatisticList.Columns[2].Name = "Gesamt";
            DataGridView_StatisticList.Columns[3].Name = "Falsch";
            DataGridView_StatisticList.Columns[4].Name = "Richtig";
            DataGridView_StatisticList.Columns[5].Name = "Note";
            DataGridView_StatisticList.Columns[6].Name = "benötigte Zeit";
        }

        //Initialisiert die GridView für die gesicherte Errorliste
        private void SetupDataGridView_ErrorListe() {
            this.Controls.Add(DataGridView_ErrorList);
            DataGridView_ErrorList.ColumnCount = 4;
            DataGridView_ErrorList.Name = "DataGridView_ErrorList";
            DataGridView_ErrorList.Location = new Point(50, 50);
            DataGridView_ErrorList.Size = new Size(500, 400);
            DataGridView_ErrorList.ReadOnly = true;
            DataGridView_ErrorList.AllowUserToDeleteRows = false;
            DataGridView_ErrorList.AllowUserToAddRows = false;
            DataGridView_ErrorList.Columns[0].Name = "Nummer";
            DataGridView_ErrorList.Columns[1].Name = "Gesucht";
            DataGridView_ErrorList.Columns[2].Name = "Eingegeben";
            DataGridView_ErrorList.Columns[3].Name = "korrekte Lösung";

        }

        //regelt den Ablauf, nachdem auf eine Zelle der GridView geklickt wurde
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e) {
            Button SubmitButton;
            /*führt nachfolgenden Code nur dann aus, wenn die boolsche Variable false ist. Das ist sie standarmäßig. Mitunter wurde sie an anderer Stelle jedoch
             auf true gesetzt*/
            if (!CheckClickStatus) {
                if (e.ColumnIndex == 0) {
                    if (ShowInfo) {
                        MessageBox.Show("Diesen Wert können Sie nicht verändern");
                    }
                    return;
                }
                if (e.ColumnIndex != -1 && e.ColumnIndex != 0) {
                    SubmitButton = new Button();
                    SubmitButton.Size = new Size(50, 23);
                    SubmitButton.Location = new Point(100, 425);
                    SubmitButton.Text = "Submit";
                    SubmitButton.BackColor = Color.Yellow;
                    SubmitButton.FlatStyle = FlatStyle.Popup;
                    this.Controls.Add(SubmitButton);
                    SubmitButton.Click += new EventHandler(SubmitButton_Click);
                }
            }
        }

        //regelt den Ablauf, nachdem auf den obigen SubmitButton gedrückt wurde: Die Zellen werden gesichert. Dieser Button wurde durch obige Methode erstellt
        private void SubmitButton_Click(object sender, EventArgs e) {
            try {
                string lines = "";
                int VocAmount = (DataGridView_Edit.Columns.Count - 1) * DataGridView_Edit.Rows.Count;
                //bei eins beginnen, damit die Zahlen nicht mitgespeichert werden
                for (int col = 1; col < DataGridView_Edit.Columns.Count; col++) {
                    //alle Reihen zählen, da zusätzliche nicht hinzugefügt werden können, es also auch keine leere Reihen in der GridView gibt
                    for (int row = 0; row < DataGridView_Edit.Rows.Count; row++) {
                        //sofern letzte Zeile erreicht, kein Umbruch mehr durchführen, sonst kommt es zu Array-Index-Fehler in der Auswertung
                        if (col * (row + 1) == VocAmount)
                            lines += DataGridView_Edit.Rows[row].Cells[col].Value.ToString();
                        else
                            lines += DataGridView_Edit.Rows[row].Cells[col].Value.ToString() + Environment.NewLine;
                    }
                }
                StreamWriter objWriter = new StreamWriter(RememberFilename_Edit);
                objWriter.WriteLine(lines);
                objWriter.Close();
                if (ShowInfo)
                    MessageBox.Show("Die editierte Vokabelliste wurde erfolgreich in der Datei " + RememberFilename_Edit + " gesichert");
            } catch (Exception er) {
                if (ShowInfo)
                    MessageBox.Show("Es darf keinerlei leere Zellen geben!" + Environment.NewLine + Environment.NewLine + er.ToString(), "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
        }

        //regelt den Ablauf, nachdem auf TestButton gedrückt wurde:Der Test wird gestartet. Dieser Button wurde dynamisch erstellt
        private void TestButton_Click(object sender, EventArgs e) {
            /*deaktivert den Timer nach jedem Aufruf und setzt ihn zurück*/
            timer1.Enabled = false;
            Time.Reset();
            CountMyRound++;
            if (!radioButton1.Checked && !radioButton2.Checked) {
                if (ShowInfo)
                    MessageBox.Show("Eine der beiden Abfrageoptionen muss gewählt werden. Aktivieren Sie folglich einer der beiden RadioButtons, damit der Test beginnen kan...", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            this.Deactivation(3);
            /*initialisiert die TestÜberschrift (visuell ein <h2>-Tag)*/
            LabelTestHead = new Label();
            LabelTestHead.Location = new Point(400, 20);
            LabelTestHead.AutoSize = true;
            LabelTestHead.Text = "Vokabeltest";
            LabelTestHead.ForeColor = Color.RoyalBlue;
            LabelTestHead.Font = new Font(LabelTestHead.Font.FontFamily, 40);
            LabelTestHead.Visible = true;
            this.Controls.Add(LabelTestHead);
            /*Blendet das TimerLabel ein*/
            label5.Visible = true;
            button3.Visible = true;
            int TimeOver = 0;
            if (anzahl <= 5) TimeOver = 30;
            else if (anzahl <= 10 && anzahl > 5) TimeOver = 60;
            else if (anzahl <= 15 && anzahl > 10) TimeOver = 90;
            else if (anzahl <= 20 && anzahl > 15) TimeOver = 120;

            if (checkBox1.Checked) {
                if (ShowInfo)
                    MessageBox.Show("Ab jetzt haben Sie " + TimeOver + " Sekunden Zeit. Danach erfolgt posthum die Auswertung. Wenn Sie vorher fertig sind, können Sie den gelben Button pushen.");
                timer1.Enabled = true;
                Time.Start();
            } else {
                label5.Text = "Round:" + CountMyRound + "  0:00:000";
                if (ShowInfo)
                    MessageBox.Show(" Die Zeit zur Lösung des Tests spielt keine Rolle. Sie verbleibt bei 0:00:000");
            }
            /*Mischt die Vokabeln durch den Zufallsgenerator der Klasse Random*/
            Random rand = new Random(DateTime.Now.Millisecond);
            if (radioButton1.Checked) {
                InhaltDeutsch.Sort(delegate (string s1, string s2)
                {
                    /* Sortierung wird durch Zufall bestimmt*/
                    return rand.Next(-5, 3);
                });
                /*Stellt die gemischten Vokabeln in den Textboxen dar*/
                for (int i = 0; i < InhaltDeutsch.Count; i++) {
                    TextBox textbox = new TextBox();
                    textbox.Name = String.Format("MYTEXTBOX_GERMAN{0}", i);
                    textbox.Text = InhaltDeutsch[i];
                    textbox.Location = new Point(400, 150 + 30 * i);
                    textbox.Enabled = false;
                    this.Controls.Add(textbox);
                }
                for (int i = 0; i < InhaltForeign.Count; i++) {
                    TextBox textbox = new TextBox();
                    textbox.Name = String.Format("MYTEXTBOX_FOREIGN{0}", i);
                    textbox.Location = new Point(600, 150 + 30 * i);
                    this.Controls.Add(textbox);
                }
            } else if (radioButton2.Checked) {
                InhaltForeign.Sort(delegate (string s1, string s2)
                {
                    return rand.Next(-5, 3);
                });
                for (int i = 0; i < InhaltForeign.Count; i++) {
                    TextBox textbox = new TextBox();
                    textbox.Name = String.Format("MYTEXTBOX_FOREIGN{0}", i);
                    textbox.Text = InhaltForeign[i];
                    textbox.Location = new Point(400, 150 + 30 * i);
                    textbox.Enabled = false;
                    this.Controls.Add(textbox);
                }
                for (int i = 0; i < InhaltDeutsch.Count; i++) {
                    TextBox textbox = new TextBox();
                    textbox.Name = String.Format("MYTEXTBOX_DEUTSCH{0}", i);
                    textbox.Location = new Point(600, 150 + 30 * i);
                    this.Controls.Add(textbox);
                }

            }
        }

        //prüft, ob die Sounddatei asynchron geladen wurde


        //prüft, ob der Rechner online ist
        private bool CheckHost(Uri url) {
            WebRequest request = WebRequest.Create(url);
            request.Timeout = 15000;
            try {
                var response = request.GetResponse();
                bool hasConnected = response.ResponseUri.Host.Contains("de");
                return hasConnected;
            } catch (Exception error) {
                MessageBox.Show(error.Message);
                return false;
            }
        }

        //setzt die Stoppuhr in Gang
        private void timer1_Tick(object sender, EventArgs e) {
            TimeSpan ElapsedTime = Time.Elapsed;
            string ElapsedTimeString = "";
            int TimeOver = 0;
            if (anzahl <= 5) TimeOver = 30;
            else if (anzahl <= 10 && anzahl > 5) TimeOver = 60;
            else if (anzahl <= 15 && anzahl > 10) TimeOver = 90;
            else if (anzahl <= 20 && anzahl > 15) TimeOver = 120;
            ElapsedTimeString = String.Format("Round:{0}  {1:0}:{2:00}:{3:000}", CountMyRound.ToString(),
            ElapsedTime.Minutes, ElapsedTime.Seconds, ElapsedTime.Milliseconds);
            label5.Text = ElapsedTimeString;
            TimeForTest = ElapsedTime.TotalSeconds;
            if (TimeForTest > TimeOver) {
                timer1.Enabled = false;
                Time.Stop();
                button3.Visible = false;
                if (ShowInfo)
                    MessageBox.Show("Nichts geht mehr. Jetzt geht es zur Auswertung....", "TIME OVER", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.TestAuswerten();
            }
        }

        //ruft die Methode TestAuswerten() aus, nachdem auf den entsprechenden 'Auswerten'-Button gedrückt wurde
        private void button3_Click(object sender, EventArgs e) {
            timer1.Enabled = true;
            Time.Stop();
            this.TestAuswerten();
        }

        //wertet den Test aus. Enthält den Algorithmus zur Bestimmung falscher Vokabeleingaben
        private void TestAuswerten() {
            try {
                int WrongValues = 0;
                /*initialisert eine lokale Liste, um die fehlerhaft eingegebenen Vokabeln zu eruieren*/
                List<string> InhaltVergleich = new List<string>();
                List<string> VocListWrong = new List<string>();
                /*initialisiert 2 Arrays, in welche grundsätzlich die Vokabeln verfrachtet werden. Ein Array davon besteht immer aus den Usereingaben, das andere
                aus den Testvokabeln*/
                string[] TextBoxArrayDt = new string[anzahl];
                //anzahl ist eine 'globale' Variable und wurde in TestStartenToolStripMenuItem_Click durch die Zuweisung von anzahlVoc initialisiert
                string[] TextBoxArrayFo = new string[anzahl];
                /*initialisiert 2 Arrays, in welche die Vokabeln aus der geladenen Datei verfrachtet werden. Die Usereingaben werden, abhängig von 
                 radioButton, mit einem der beiden Array verglichen, um zu eruieren, ob die Usereingabe korrekt oder falsch war*/
                string[] OriginArrayDt = new string[anzahl];
                string[] OriginArrayFo = new string[anzahl];
                /*Rubrik: Vokabel <--> geladene Datei :*/
                var zeilen = File.ReadAllLines(RememberFilename_Test);
                for (int i = 0; i < anzahl; i++) {
                    OriginArrayDt[i] = zeilen[i];
                }
                for (int i = 0; i < zeilen.Length; i++) {
                    if (i > anzahl - 1)
                        OriginArrayFo[i - anzahl] = zeilen[i];
                }
                /*Rubrik: Vokabel <--> Usereingabe :
                 Sofern deutsche Begriffe gefragt sind*/
                if (radioButton1.Checked) {
                    for (int i = 0; i < anzahl; i++) {
                        string VocDt = String.Format("MYTEXTBOX_GERMAN{0}", i);
                        string VocFo = String.Format("MYTEXTBOX_FOREIGN{0}", i);
                        var txtDt = (TextBox)this.Controls[VocDt];
                        var txtFo = (TextBox)this.Controls[VocFo];
                        TextBoxArrayDt[i] = txtDt.Text;
                        TextBoxArrayFo[i] = txtFo.Text;

                    }
                }
                /*Sofern fremdsprachige Begriffe gefragt sind*/
                else if (radioButton2.Checked) {
                    for (int i = 0; i < anzahl; i++) {
                        string VocFo = String.Format("MYTEXTBOX_FOREIGN{0}", i);
                        string VocDt = String.Format("MYTEXTBOX_DEUTSCH{0}", i);
                        var txtFo = (TextBox)this.Controls[VocFo];
                        var txtDt = (TextBox)this.Controls[VocDt];
                        TextBoxArrayFo[i] = txtFo.Text;
                        TextBoxArrayDt[i] = txtDt.Text;

                    }
                }
                /*ToDO: Es muss ein Algorithmus implementiert werden, der die Usereingabe(TextBoxArrayFo) mit OriginArrayFo 
                vergleicht. Die Schwierigkeit besteht darin, dass der Zufallsgenerator der Methode TestButton_Click die Vokabeln durcheinander gewürfelt hat.
                Vorausgesetzt,radioButton1 ist true, gelten folgende Optionen:
                1.: TextBoxArrayFo enthält alle eingegebenen Fremdvokabeln
                2.: TextBoxArrayDt enthält die dazu korrespondienden Vokabeln in deutsch
                3.: OriginArrayFo enthält die korrekten Fremdvokabeln*/

                if (radioButton1.Checked) {
                    /*Dieser Alogorithums erfüllt obige Anforderungen*/
                    foreach (String s1 in TextBoxArrayDt) {
                        int zaehler = 0;
                        foreach (String s2 in OriginArrayDt) {
                            zaehler++;
                            if (s1.Equals(s2) == true) {
                                InhaltVergleich.Add(OriginArrayFo[zaehler - 1]);
                            }
                        }
                    }
                    /*verfrachte die jeweiligen Values in eine Liste zwecks späterer Auswertung*/
                    for (int i = 0; i < anzahl; i++) {
                        if (TextBoxArrayFo[i] != InhaltVergleich[i]) {
                            WrongValues++;
                            /*abgefragte Vokabel*/
                            VocListWrong.Add(TextBoxArrayDt[i]);
                            /*eingegebene Vokabel*/
                            VocListWrong.Add(TextBoxArrayFo[i]);
                            /*korrekte Vokabel*/
                            VocListWrong.Add(InhaltVergleich[i]);
                        }
                    }

                    for (int i = 0; i < anzahl; i++) {
                        if (i == TextBoxArrayDt.Length - 1 && VocListWrong.Count > 0) {
                            VocListWrong.Add("ENDE");
                        }
                    }
                } else if (radioButton2.Checked) {
                    /*Genau dasselbe wie oben, allerdings mit anderen Values*/
                    foreach (String s1 in TextBoxArrayFo) {
                        int zaehler = 0;
                        foreach (String s2 in OriginArrayFo) {
                            zaehler++;
                            if (s1.Equals(s2) == true) {
                                InhaltVergleich.Add(OriginArrayDt[zaehler - 1]);
                            }
                        }
                    }
                    for (int i = 0; i < anzahl; i++) {
                        if (TextBoxArrayDt[i] != InhaltVergleich[i]) {
                            WrongValues++;
                            /*abgefragte Vokabel*/
                            VocListWrong.Add(TextBoxArrayFo[i]);
                            /*eingegebene Vokabel*/
                            VocListWrong.Add(TextBoxArrayDt[i]);
                            /*korrekte Vokabel*/
                            VocListWrong.Add(InhaltVergleich[i]);
                        }
                    }
                    for (int i = 0; i < anzahl; i++) {
                        if (i == TextBoxArrayFo.Length - 1 && VocListWrong.Count > 0) {
                            VocListWrong.Add("ENDE");
                        }
                    }
                }
                this.ShowStatistic(WrongValues, VocListWrong);
            } catch (Exception e) {
                MessageBox.Show("Während der Ausführung kam es zu folgendem schweren Fehler" + Environment.NewLine + e.ToString() + Environment.NewLine + "Die Applikation wird deshalb neu gestartet. Wir entschuldigen dieses Unanehmlichkeit!", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                Application.Restart();
            }
        }

        //zeigt eine Zusammenfassung der Testdaten bzw. dessen Resultat an
        private void ShowStatistic(int mistakes, List<string> Fehlerliste) {
            try {
                string TermForTime;
                string note = "";
                DateTime localDate = DateTime.Now;
                for (int i = this.Controls.Count - 1; i >= 0; i--) {
                    Control t = this.Controls[i];
                    if (t.GetType() == typeof(TextBox)) {
                        TextBox tb = (TextBox)t;
                        if (tb.Name.StartsWith("MYTEXTBOX")) {
                            this.Controls.Remove(tb);
                        }
                    }
                }
                /*Der Wert dieser Variablen entspricht zwar einem integer. Da mit ihr jedoch prozentual gerechnet wird, muss sie als double definiert werden*/
                double korrekt = anzahl - mistakes;
                double anzahlCalc = Convert.ToDouble(anzahl);
                double anteil = (korrekt / anzahlCalc) * 100;
                anteil = Math.Round(anteil, 2);
                /*diese Skala entspricht dem Notenschlüssel der IHK*/
                if (anteil < 30) note = "6";
                else if (anteil < 50 && anteil >= 30) note = "5";
                else if (anteil < 67 && anteil >= 50) note = "4";
                else if (anteil < 81 && anteil >= 67) note = "3";
                else if (anteil < 92 && anteil >= 81) note = "2";
                else if (anteil >= 92) note = "1";
                this.Deactivation(3);
                label7.Visible = true;
                listBox1.Visible = true;
                if (listBox1.Items.Contains(Environment.NewLine) && ShowInfo)
                    MessageBox.Show("Es befinden sich mehrere Auswertungen in der Auflistung. Beachten sie die DateTimeangabe und scrollen Sie ggf. nach unten.");
                listBox1.Items.Add(Environment.NewLine);
                listBox1.Items.Add("Aktueller DateTime:" + localDate + " Uhr");
                if (radioButton1.Checked) {
                    listBox1.Items.Add("Gesucht wurde das Fremdwort.");
                } else if (radioButton2.Checked) {
                    listBox1.Items.Add("Gesucht wurde ein deutscher Begriff.");
                }
                listBox1.Items.Add("Anzahl Vokabeln gesamt: " + anzahl + " Stück.");
                listBox1.Items.Add("Sie haben insgesamt " + mistakes + " Vokabel(n) nicht gewusst oder falsch eingegeben!");
                listBox1.Items.Add("Folglich haben Sie " + korrekt + " Vokabeln richtig eingegeben.");
                /*obgleich die Berechnung einen INT liefert, muss er als double initialisert werden, da er für folgende Arithmetik benötigt wird*/
                listBox1.Items.Add("Dies entspricht der Note " + note);
                if (TimeForTest > 0) {
                    TimeForTest = Math.Round(TimeForTest, 2);
                    TermForTime = "Sie haben in etwa " + TimeForTest + " Sekunden für denTest benötigt.";
                } else
                    TermForTime = "Die Zeit spielte in diesem Test keine Rolle.";
                listBox1.Items.Add(TermForTime);
                /*Hier werden die fehlerhaften Eingaben und die korrekten Vokabeln in die Grid verfrachtet und sodann angezeigt*/
                if (!CheckGridStat) {
                    this.SetupDataGridView_Statistic();
                    CheckGridStat = true;
                } else {
                    DataGridView_Statistic.Rows.Clear();
                    DataGridView_Statistic.Visible = true;
                }
                var zeilen = Fehlerliste.Count;
                int zaehler = 1;
                if (zeilen > 0) {
                    for (int i = 0; i < zeilen - 3; i++) {
                        if (i % 3 == 0) {
                            DataGridView_Statistic.Rows.Add(zaehler, Fehlerliste[i], Fehlerliste[i + 1], Fehlerliste[i + 2]);
                            zaehler++;
                        }
                    }
                    DataGridView_Statistic.Rows.Add("", Fehlerliste[zeilen - 1], Fehlerliste[zeilen - 1], Fehlerliste[zeilen - 1]);
                } else {
                    if (ShowInfo)
                        MessageBox.Show("Alles korrekt! Demzufolge hat die GridView keinen Inhalt. Vergewissen Sie sich in der Statistik von ihrem phänomenalen Ergebnis!");
                }
                button4.Visible = true;
                button5.Visible = true;
                this.DrawDiagramm(anteil);
            } catch (Exception er) {
                MessageBox.Show("Unbekannter Fehler..." + Environment.NewLine + Environment.NewLine + er.ToString(), "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

            }
        }

        //erstellt das Diagramm und lädt es in eine PictureBox
        private void DrawDiagramm(double prozAnteil) {
            string bezeichnung = "rot=fehlerhaft" + Environment.NewLine + "blau=korrekt";
            Dictionary<string, double> dataInkorrekt = new Dictionary<string, double>() { { bezeichnung, 100 - prozAnteil } };
            Dictionary<string, double> dataKorrekt = new Dictionary<string, double>() { { bezeichnung, prozAnteil } };
            using (Chart chart = new Chart() {
                Height = 200,
                Width = 220
            }) {
                chart.Titles.Add("Auswertung(in %)");
                chart.ChartAreas.Add(new ChartArea("statistic") {
                    AxisX = new Axis() {
                        MajorGrid = new Grid() {
                            Enabled = false
                        }
                    },
                    AxisY = new Axis() {
                        MajorGrid = new Grid() {
                            LineColor = Color.LightGray,
                            LineDashStyle = ChartDashStyle.Dot,
                        },
                        Title = "%"
                    }
                });

                chart.Series.Add(new Series("dataInkorrekt") {
                    ChartType = SeriesChartType.Column,
                    Color = Color.Red
                });
                chart.Series.Add(new Series("dataKorrekt") {
                    ChartType = SeriesChartType.Column,
                    Color = Color.Blue


                });

                // Daten
                foreach (KeyValuePair<string, double> entry in dataInkorrekt) {
                    chart.Series["dataInkorrekt"].Points.Add(new DataPoint() {
                        AxisLabel = entry.Key,
                        YValues = new double[] { entry.Value }
                    });
                }
                foreach (KeyValuePair<string, double> entry in dataKorrekt) {
                    chart.Series["dataKorrekt"].Points.Add(new DataPoint() {
                        AxisLabel = entry.Key,
                        YValues = new double[] { entry.Value }
                    });
                }

                // Ausgabe
                chart.SaveImage("dia.png", ChartImageFormat.Png);
                string PfadBild = "dia.png"; // hier wird der Pfad des Bildes definiert
                StreamReader SR = new StreamReader(PfadBild);
                Bitmap Bild = new Bitmap(SR.BaseStream);
                SR.Close();
                pictureBox1.Image = Bild;
                pictureBox1.Visible = true;

            }
        }

        //sichert die Statistikdaten in einer Textdatei mit der Endung 'stat'
        private void button4_Click(object sender, EventArgs e) {
            string StringToSave = "";
            string path = Directory_Statistic + "/" + filename_;
            if (!Directory_Statistic.Exists && CreateDirectory) {
                if (MessageBox.Show("Um die Statistikauswertungen zu sichern, muss das Verzeichnis " + Directory_Statistic + " neu angelegt werden. Wollen Sie das Verzeichnis anlegen?", "Hint", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Directory.CreateDirectory(Directory_Statistic.ToString());
                    CreateDirectory = false;
                    if (ShowInfo)
                        MessageBox.Show(Directory_Statistic + " wurde neu angelegt");
                } else {
                    MessageBox.Show("Die Statistik kann nicht gesichert werden", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
            }
            if (Directory_Statistic.Exists || !CreateDirectory) {
                foreach (string listContent in listBox1.Items) {
                    if (listContent != Environment.NewLine)
                        StringToSave += listContent + Environment.NewLine;
                }
                File.AppendAllText(path, StringToSave);
                if (ShowInfo)
                    MessageBox.Show(" Der Inhalt der Statistik wurde in der Datei " + path + " gesichert");
            }
        }

        //sichert die eingegegeben Vokabeln,den gesuchten Begriff und die Vorgabe in einer Textdatei mit der Endung 'wVoc'
        private void button5_Click(object sender, EventArgs e) {
            string path = Directory_ErrorVoc + "/" + filename;
            if (!Directory_ErrorVoc.Exists && CreateDirectory_WVoc) {
                if (MessageBox.Show("Um die Statistikauswertungen zu sichern, muss das Verzeichnis " + Directory_Statistic + " neu angelegt werden. Wollen Sie das Verzeichnis anlegen?", "Hint", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Directory.CreateDirectory(Directory_ErrorVoc.ToString());
                    CreateDirectory_WVoc = false;
                    if (ShowInfo)
                        MessageBox.Show("Das Verzeichnis " + Directory_ErrorVoc + " und die Datei" + filename + " wurden soeben neu angelegt");
                } else {
                    if (ShowInfo)
                        MessageBox.Show("Die GridView kann nicht gesichert werden", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
            }
            if (Directory_ErrorVoc.Exists || !CreateDirectory_WVoc) {
                string lines = "";
                if (DataGridView_Statistic.Rows.Count == 0) {
                    if (ShowInfo) {
                        MessageBox.Show("Die GridView ohne Inhalt zu sichern ergibt keinen Sinn. Der Vorgang wurde folglich abgebrochen", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        return;
                    }

                }

                //bei eins beginnen, damit die Zahlen nicht mitgespeichert werden
                for (int col = 1; col < DataGridView_Statistic.Columns.Count; col++) {
                    //alle Reihen zählen, da zusätzliche nicht hinzugefügt werden können, es also auch keine leere Reihen in der GridView gibt
                    for (int row = 0; row < DataGridView_Statistic.Rows.Count; row++) {
                        if (DataGridView_Statistic.Rows[row].Cells[col].Value.ToString() == "") {
                            DataGridView_Statistic.Rows[row].Cells[col].Value = "keine Eingabe";
                        }

                        lines += DataGridView_Statistic.Rows[row].Cells[col].Value.ToString() + Environment.NewLine;
                    }
                }
                File.AppendAllText(path, lines);
                if (ShowInfo)
                    MessageBox.Show("Die Inhalte der GridView wurden in " + path + " gesichert.");

            }

        }

        //lädt und zeigt die Daten der Statistik an
        private void ladenToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Deactivation(3);
            string path = Directory_Statistic + "/" + filename_;
            if (!File.Exists(path)) {
                if (ShowInfo)
                    MessageBox.Show(path + " existiert nicht. Demzufolge können Sie diesen Menupunkt noch nicht auführen. Absolvieren sie zuerst einen Test und sichern Sie dessen Statistikdaten!", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            } else {
                if (!CheckGridStatisticList) {
                    this.SetupDataGridView_StatisticListe();
                    CheckGridStatisticList = true;
                } else {
                    DataGridView_StatisticList.Rows.Clear();
                    DataGridView_StatisticList.Visible = true;
                }
                var zeilen = File.ReadAllLines(path);
                int grenze = zeilen.Length;
                for (int i = 0; i < grenze - 1; i++) {
                    if (i % 7 == 0) {
                        DataGridView_StatisticList.Rows.Add(zeilen[i], zeilen[i + 1], zeilen[i + 2], zeilen[i + 3], zeilen[i + 4], zeilen[i + 5], zeilen[i + 6]);
                    }
                }
                DataGridView_StatisticList.AutoResizeColumns();
            }
        }

        //lädt und zeigt die Daten der Fehlerliste an 
        private void errorToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Deactivation(3);
            bool InfluenceIndizie = true;
            int IndizieForWhileLoop = 1;
            int RepeatingOfVoc = 0;
            string path = Directory_ErrorVoc + "/" + filename;
            if (!File.Exists(path)) {
                if (ShowInfo)
                    MessageBox.Show(filename + " existiert nicht. Demzufolge können Sie diesen Menupunkt noch nicht auführen. Absolvieren sie zuerst einen Test und sichern Sie die GridView der Auswertung!", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            /*sofern die Datei existiert, kann der Zauber beginnen...*/
            else {
                var zeilen = File.ReadAllLines(path);
                int grenze = zeilen.Length;
                for (int i = 0; i < grenze; i++) {
                    if (zeilen[i] == "ENDE")
                        RepeatingOfVoc++;
                }
                RepeatingOfVoc /= 3;
                /*jetzt weiss ich, wieviel mal durch die Liste iteriert werden muss. Bei 1 Sicherung ergibt sich folgende Rechnung: 3/3=1. Bei 2 Sicherungen ergibt 
                 sich folgende Rechnung:(2*3)/3=2. Bei drei Sicherungen ergibt sich folgende Rechnung:(3*3)/3=3 etc..etc..*/
                if (!CheckGridErrorList) {
                    this.SetupDataGridView_ErrorListe();
                    CheckGridErrorList = true;
                } else {
                    DataGridView_ErrorList.Rows.Clear();
                    DataGridView_ErrorList.Visible = true;
                }
                /*initialisiert drei Arrays, die in der GridView ausgelesen werden*/
                string[] ArrayGesucht = new string[grenze];
                string[] ArrayEingegeben = new string[grenze];
                string[] ArrrayKorrekt = new string[grenze];
                /*Das Riesenproblem besteht darin, einen Algorithmus zu finden, der die Anordnung in der Textdatei, bei unterschiedlicher Anzahl an Vokabeln
                 korrekt wiedergibt. Was mache ich, wenn ich erst 5, dann 10, dann 14 falsch eingeben habe. Es fehlt die mathematische Regelmäßigkeit. Hier also 
                 der Algorithmus*/
                int IndizieA = 0;
                int IndizieB = 0;
                int IndizieC = 0;
                int IndizieD = 0;
                int IndizieE = 0;
                while (IndizieForWhileLoop <= RepeatingOfVoc) {
                    //innere Schleife, um die gesuchten Begriffe in das Array zu verfrachten. Entspricht der zweiten Spalte der GridView
                    for (int i = IndizieA; i < grenze - 1; i++) {
                        if (IndizieC == 0) {
                            if (zeilen[IndizieA] != "ENDE") {
                                ArrayGesucht[IndizieB] = zeilen[i];
                                IndizieA++;
                                IndizieB++;
                            } else {
                                IndizieA = i;
                                IndizieC = IndizieB;
                                IndizieB = 0;
                                break;
                            }
                        } else {
                            if (zeilen[IndizieA] != "ENDE") {
                                ArrayGesucht[IndizieC] = zeilen[i];
                                IndizieA++;
                                IndizieC++;
                            } else {
                                IndizieA = i;
                                break;
                            }

                        }
                    }
                    //innere Schleife, um die eingegebenen Begriffe in das Array zu verfrachten. Entspricht der dritten Spalte der GridView
                    for (int i = IndizieA + 1; i < grenze; i++) {
                        if (zeilen[i] != "ENDE") {
                            ArrayEingegeben[IndizieE] = zeilen[i];
                            IndizieE++;
                        } else {
                            IndizieA = i;
                            break;
                        }
                    }
                    //innere Schleife, um die richtigen Begriffe in das Array zu verfrachten. Entspricht der vierten Spalte der GridView
                    IndizieB = 0;
                    for (int i = IndizieA + 1; i < grenze; i++) {
                        if (InfluenceIndizie) {
                            if (zeilen[i] != "ENDE") {
                                ArrrayKorrekt[IndizieB] = zeilen[i];
                                IndizieB++;
                            } else {
                                InfluenceIndizie = false;
                                IndizieA = i + 1;
                                IndizieD = IndizieB;
                                break;
                            }
                        } else {
                            if (zeilen[i] != "ENDE") {
                                ArrrayKorrekt[IndizieD] = zeilen[i];
                                IndizieD++;
                            } else {
                                IndizieA = i + 1;
                                break;
                            }
                        }
                    }
                    IndizieForWhileLoop++;
                }
                /*Hier ist der Algorithmus zu Ende(dem Debugger sei Dank :=). Was folgt ist die Ausagabe der durch den Algorithmus erzeugten Arrays in der 
                 GridView*/
                for (int i = 0; i < grenze / 3 - (IndizieForWhileLoop - 1); i++) {
                    DataGridView_ErrorList.Rows.Add(i + 1, ArrayGesucht[i], ArrayEingegeben[i], ArrrayKorrekt[i]);
                }
                DataGridView_StatisticList.AutoResizeColumns();
            }
        }

        //löscht die Statistikdatei
        private void statistikLöschenToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Deactivation(3);
            string path = Directory_Statistic + "/" + filename_;
            if (File.Exists(path)) {
                DialogResult result1 = MessageBox.Show("Wollen sie wirklich die Datei " + path + " löschen?", "Important Question", MessageBoxButtons.YesNo);
                if (result1 == DialogResult.Yes) {
                    File.Delete(path);
                    if (ShowInfo)
                        MessageBox.Show("Die Datei " + path + " wurde soeben gelöscht");
                }
            } else {
                if (ShowInfo)
                    MessageBox.Show("Was nicht exisitert, kann auch nicht gelöscht werden. Informieren Sie sich über die HELP-Option!", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        //löscht die Errorlistdatei
        private void fehlerlisteLöschenToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Deactivation(3);
            string path = Directory_ErrorVoc + "/" + filename;
            if (File.Exists(path)) {
                DialogResult result1 = MessageBox.Show("Wollen sie wirklich die Datei " + path + " löschen?", "Important Question", MessageBoxButtons.YesNo);
                if (result1 == DialogResult.Yes) {
                    File.Delete(path);
                    if (ShowInfo)
                        MessageBox.Show("Die Datei " + path + " wurde soeben gelöscht");
                }
            } else {
                if (ShowInfo)
                    MessageBox.Show("Was nicht exisitert, kann auch nicht gelöscht werden. Informieren Sie sich über die HELP-Option!", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
    }
}


