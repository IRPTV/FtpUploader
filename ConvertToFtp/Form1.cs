using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;



namespace ConvertToFtp
{
    public partial class Form1 : Form
    {

        string _TempDir = "";
        string _DateFolder = "";
        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "mp4 files (*.mp4)|*.mp4";
            openFileDialog1.Title = "Select Mp4 File";
            openFileDialog1.Multiselect = true;
            openFileDialog1.InitialDirectory = textBox1.Text;
            openFileDialog1.ShowDialog();
        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Maximum = 100;
            _TempDir = System.Configuration.ConfigurationSettings.AppSettings["TempDirectory"].Trim();


            String[] FtpDirs =
                   System.Configuration.ConfigurationManager.AppSettings["DestDirectory"].Trim().Split('#');
            comboBox1.Items.Clear();
            foreach (string item in FtpDirs)
            {
                NewListItem Itm = new NewListItem();
                Itm.Value = item.Trim();
                Itm.Text = item.Trim();
                comboBox1.Items.Add(Itm);
            }
           


            String[] LocalDirs =
               System.Configuration.ConfigurationManager.AppSettings["LocalServer"].Trim().Split('#');
            comboBox2.Items.Clear();
            foreach (string item in LocalDirs)
            {
                NewListItem Itm = new NewListItem();
                Itm.Value = item.Trim();
                Itm.Text = item.Trim();
                comboBox2.Items.Add(Itm);
            }
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }


        }
        //protected void FtpDirectories()
        //{
        //    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(System.Configuration.ConfigurationSettings.AppSettings["Server"].Trim());
        //    request.Method = WebRequestMethods.Ftp.ListDirectory;
        //    request.Credentials = new NetworkCredential(System.Configuration.ConfigurationSettings.AppSettings["UserName"].Trim(),
        //        System.Configuration.ConfigurationSettings.AppSettings["PassWord"].Trim());
        //    request.UsePassive = true;
        //    request.UseBinary = true;
        //    request.KeepAlive = false;
        //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //    Stream responseStream = response.GetResponseStream();
        //    StreamReader reader = new StreamReader(responseStream);
        //   // comboBox1.Items.Clear();
        //    while (!reader.EndOfStream)
        //    {
        //        Application.DoEvents();
        //        string Ln = reader.ReadLine();
        //        if (IsDirectory(Ln))
        //        {
        //            NewListItem Dir = new NewListItem();
        //            Dir.Text = Ln;
        //            Dir.Value = Ln;
        //            comboBox1.Items.Add(Dir);
        //        }
        //    }
        //    reader.Close();
        //    responseStream.Close();
        //    response.Close();
        //    if (comboBox1.Items.Count > 0)
        //    {
        //        comboBox1.SelectedIndex = 0;
        //    }
        //}
        public bool IsDirectory(string directory)
        {
            if (directory == null)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (System.IO.Path.GetExtension(directory) == string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool CheckDir()
        {
            try
            {
              //  _DateFolder = string.Format("{0:0000}", DateTime.Now.Year) + "" + string.Format("{0:00}", DateTime.Now.Month) + string.Format("{0:00}", DateTime.Now.Day);

                _DateFolder = string.Format("{0:0000}", dateTimePicker1.Value.Year) + "" + string.Format("{0:00}", dateTimePicker1.Value.Month) + string.Format("{0:00}", dateTimePicker1.Value.Day);


                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(System.Configuration.ConfigurationSettings.AppSettings["Server"].Trim()
                   + "/" + ((NewListItem)(comboBox1.SelectedItem)).Value + "/" + _DateFolder + "/");


                if (!checkBox6.Checked)
                {
                    request = (FtpWebRequest)FtpWebRequest.Create(System.Configuration.ConfigurationSettings.AppSettings["Server"].Trim()
                  + "/" + ((NewListItem)(comboBox1.SelectedItem)).Value + "/");
                }




                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(System.Configuration.ConfigurationSettings.AppSettings["UserName"].Trim(),
                    System.Configuration.ConfigurationSettings.AppSettings["PassWord"].Trim());
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = true;
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    if (resp.StatusCode == FtpStatusCode.PathnameCreated)
                    {
                        //MessageBox.Show("Directoy " + textBox2.Text + " created.");
                        Console.WriteLine(resp.StatusCode);
                        resp.Close();
                    }
                }
                return true;
            }
            catch
            {
                return true;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 5)
            {
                DialogResult DateResult = System.Windows.Forms.DialogResult.Yes;
                if (checkBox6.Checked)
                {                   
                    if (dateTimePicker1.Value.ToShortDateString() != DateTime.Now.ToShortDateString())
                    {
                        DateResult = MessageBox.Show("Selected Date is: " + dateTimePicker1.Value.ToShortDateString() + "\nToday is: " + DateTime.Now.ToShortDateString() + "\nAre you sure?",
                            "Confirm Selected Date?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    }

                }

                if (DateResult == System.Windows.Forms.DialogResult.Yes)
                {
                    _DateFolder = string.Format("{0:0000}", dateTimePicker1.Value.Year) + "" + string.Format("{0:00}", dateTimePicker1.Value.Month) + string.Format("{0:00}", dateTimePicker1.Value.Day);
                    string FileDestNAme = ((NewListItem)(comboBox2.SelectedItem)).Value + "\\" + _DateFolder + "\\" + Path.GetFileName(textBox1.Text.Trim());

                    if (!checkBox6.Checked)
                    {
                        FileDestNAme = ((NewListItem)(comboBox2.SelectedItem)).Value + "\\"  + Path.GetFileName(textBox1.Text.Trim());
                    }

                    bool LocalExist=false;

                    if (checkBox5.Checked)
                    {
                        if (File.Exists(FileDestNAme))
                        {
                            LocalExist=true;
                        }
                    }
                   
                    if(!LocalExist)
                    {
                        richTextBox1.Text = "";
                        richTextBox2.Text = "";
                        dataGridView1.Rows.Clear();
                        richTextBox2.BackColor = Color.MidnightBlue;
                        if (button1.Text == "Start")
                        {
                            button1.Text = "Started";
                            button1.BackColor = Color.Red;


                        }


                        if (CheckDir())
                        {
                            String[] Bitrates =
                                System.Configuration.ConfigurationManager.AppSettings["Bitrates"].Trim().Split('#');
                            if (checkBox4.Checked)
                            {
                                dataGridView1.Rows.Add("ORIG", GetDuration(textBox1.Text));
                            }
                            if (checkBox2.Checked)
                            {
                                Convert(textBox1.Text.Trim(), "", true);
                            }
                            else
                            {                              
                                    File.Copy(textBox1.Text, _TempDir + "\\" + Path.GetFileNameWithoutExtension(textBox1.Text.Trim()) + ".mp4");
                               
                            }
                            foreach (string Bt in Bitrates)
                            {
                                richTextBox2.Text += "\n===================\n";
                                richTextBox2.Text += Bt + "\n";
                                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                                richTextBox2.ScrollToCaret();
                                Application.DoEvents();
                                Convert(_TempDir + "\\" + Path.GetFileNameWithoutExtension(textBox1.Text.Trim()) + ".mp4", Bt, false);
                                richTextBox2.Text += "\n===================\n";
                                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                                richTextBox2.ScrollToCaret();
                                Application.DoEvents();
                            }


                            if (checkBox5.Checked)
                            {
                                string LocalPath = ((NewListItem)(comboBox2.SelectedItem)).Value + "\\" + _DateFolder;
                                if (!checkBox6.Checked)
                                {
                                    LocalPath = ((NewListItem)(comboBox2.SelectedItem)).Value.ToString();
                                }

                                if (!Directory.Exists(LocalPath))
                                {
                                    Directory.CreateDirectory(LocalPath);
                                }

                                string DestFile = LocalPath+ "\\" + Path.GetFileName(textBox1.Text.Trim());
                                richTextBox2.Text += "Start Copy To Local: " + DestFile + "\n";
                                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                                richTextBox2.ScrollToCaret();
                                Application.DoEvents();

                                File.Copy(_TempDir + "\\" + Path.GetFileNameWithoutExtension(textBox1.Text.Trim()) + ".mp4", DestFile);
                                //  CopyLocal(textBox1.Text.Trim());
                                richTextBox2.Text += "End Copy To Local: " + DestFile + "\n";
                                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                                richTextBox2.ScrollToCaret();
                                Application.DoEvents();
                               
                            }

                            if (checkBox1.Checked)
                            {
                                File.Delete(_TempDir + "\\" + Path.GetFileNameWithoutExtension(textBox1.Text.Trim()) + ".mp4");
                            }

                            button1.Text = "Start";
                            button1.BackColor = Color.Black;

                        }
                    }
                    else
                    {
                        if (!checkBox6.Checked)
                        {
                            MessageBox.Show("Selected file is already exist:\n" + ((NewListItem)(comboBox2.SelectedItem)).Value +"\\"+Path.GetFileName(textBox1.Text.Trim()) + "\n Please change file name", "FILE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                        else
                        {
                            MessageBox.Show("Selected file is already exist:\n" + ((NewListItem)(comboBox2.SelectedItem)).Value + "\\" + _DateFolder + "\\" + Path.GetFileName(textBox1.Text.Trim()) + "\n Please change file name", "FILE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please change the date and start again.", "Selected Date", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            else
            {
                MessageBox.Show("Please select a input file.", "Input File", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        //protected void CopyLocal(string SourceFile)
        //{
        //    richTextBox2.Text += "\n===================\n";
        //    richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //    richTextBox2.ScrollToCaret();
        //    Application.DoEvents();
        //    string DestFile = System.Configuration.ConfigurationSettings.AppSettings["LocalServer"].Trim() + "\\" + _DateFolder + "\\" + Path.GetFileName(SourceFile);

        //    richTextBox2.Text += "Start Copy To Local: " + DestFile + "\n";
        //    richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //    richTextBox2.ScrollToCaret();
        //    Application.DoEvents();

        //    List<String> TempFiles = new List<String>();
        //    TempFiles.Add(SourceFile);

        //    CopyFiles.CopyFiles Temp = new CopyFiles.CopyFiles(TempFiles, DestFile);
        //    Temp.EV_copyCanceled += Temp_EV_copyCanceled;
        //    Temp.EV_copyComplete += Temp_EV_copyComplete;

        //    CopyFiles.DIA_CopyFiles TempDiag = new CopyFiles.DIA_CopyFiles();
        //    TempDiag.SynchronizationObject = this;
        //    Temp.CopyAsync(TempDiag);


        //}
        void Temp_EV_copyComplete()
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                richTextBox2.Text += "End Copy To Local\n";
                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                richTextBox2.ScrollToCaret();
                Application.DoEvents();
                richTextBox2.Text += "\n===================\n";
                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                richTextBox2.ScrollToCaret();
                Application.DoEvents();

                // Convert(_TempDir + _EpisodeNum +"-Orig"+ _Extention, _TempDir + _EpisodeNum + ".mp4", _TempDir, "mp4");
            }));

        }
        //void Temp_EV_copyCanceled(List<CopyFiles.CopyFiles.ST_CopyFileDetails> filescopied)
        //{
        //    //throw new NotImplementedException();
        //    MessageBox.Show("عملیات کپی متوقف شد");


        //}
        protected bool Convert(string InFile, string Bt,bool Orig)
        {

            string LogoStr = "";
            if (checkBox3.Checked)
            {
                LogoStr = " -vf \"movie=logo.png [watermark]; [in][watermark] overlay=0:0 [out]\"";
            }
            else
            {
                LogoStr = "";
            }
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            progressBar1.Value = 0;
            label1.Text = "0%";

            Process proc = new Process(); if (Environment.Is64BitOperatingSystem)
            {
                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
            }
            else
            {
                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
            }

            DirectoryInfo Dir = new DirectoryInfo(_TempDir);
            if (!Dir.Exists)
            {
                Dir.Create();
            }

            string DestFilePath = _TempDir + "\\" + Path.GetFileNameWithoutExtension(InFile) + "_LOW_" + Bt;
            if (Orig)
            {
                Bt = "1000";
                proc.StartInfo.Arguments = "-i " + "\"" + InFile + "\"" +LogoStr+ "   -b 1000k -s 640x360   -ar 48000 -ab 128k -async 1     -y  " + "\"" + _TempDir + "\\" + Path.GetFileNameWithoutExtension(InFile) + ".mp4" + "\"";
                DestFilePath = _TempDir + "\\" + Path.GetFileNameWithoutExtension(InFile);
            }
            else
            {
                proc.StartInfo.Arguments = "-i " + "\"" + InFile + "\"" + "   -b " + Bt + "k     -y  " + "\"" + DestFilePath + ".mp4" + "\"";
            }
        //    proc.StartInfo.Arguments = "-i " + "\"" + InFile + "\"" + "   -b " + Bt + "k     -y  " + "\"" + DestFilePath + ".mp4" + "\"";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.Exited += new EventHandler(myProcess_Exited);
            if (!proc.Start())
            {
                return false;
            }

            richTextBox2.Text += "Convert Started : " + InFile + "\n";
            richTextBox2.SelectionStart = richTextBox2.Text.Length;
            richTextBox2.ScrollToCaret();
            Application.DoEvents();



            proc.PriorityClass = ProcessPriorityClass.Normal;
            StreamReader reader = proc.StandardError;
            string line;

           
          


            while ((line = reader.ReadLine()) != null)
            {
                if (richTextBox1.Lines.Length > 5)
                {
                    richTextBox1.Text = "";
                }

                FindDuration(line, "1");
                richTextBox1.Text += (line) + " \n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();
            }
            proc.Close();
            richTextBox2.Text += "Convert Finished : " + InFile + "\n";
            richTextBox2.SelectionStart = richTextBox2.Text.Length;
            richTextBox2.ScrollToCaret();
            Application.DoEvents();

            //Check time 2013-11-18
            int RowIndx=0;
            bool AllowUpload = false;
            if (checkBox4.Checked)
            {
                if (File.Exists(DestFilePath + ".mp4"))
                {
                    dataGridView1.Rows.Add(Bt, GetDuration(DestFilePath + ".mp4"));
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() == Bt)
                        {
                            RowIndx = i;
                        }
                    }
                    if (RowIndx > 0)
                    {
                        string[] TimesOrig = dataGridView1.Rows[0].Cells[1].Value.ToString().Trim().Split(':');
                        double OrigSecond = double.Parse(TimesOrig[0].ToString()) * (3600) +
                            double.Parse(TimesOrig[1].ToString()) * (60) +
                            double.Parse(TimesOrig[2].ToString());

                        string[] TimesBt = dataGridView1.Rows[RowIndx].Cells[1].Value.ToString().Trim().Split(':');
                        double BtSecond = double.Parse(TimesBt[0].ToString()) * (3600) +
                            double.Parse(TimesBt[1].ToString()) * (60) +
                            double.Parse(TimesBt[2].ToString());


                        if (!((BtSecond >= OrigSecond - 5) && (BtSecond <= OrigSecond + 5)))
                        {
                            dataGridView1.Rows[RowIndx].Cells[0].Style.BackColor = Color.Red;
                            dataGridView1.Rows[RowIndx].Cells[1].Style.BackColor = Color.Red;
                            AllowUpload = false;
                        }
                        else
                        {
                            AllowUpload = true;
                        }
                    }
                    else
                    {
                        AllowUpload = false;
                    }
                }
            }
            else
            {
                AllowUpload = true;
            }

            if (checkBox1.Checked)
            {
                if (AllowUpload)
                {
                    if (Orig)
                    {
                        UploadVideo(DestFilePath + ".mp4", false);
                    }
                    else
                    {
                        UploadVideo(DestFilePath + ".mp4", true);
                    }
                }
                else
                {
                    richTextBox2.BackColor = Color.Red;
                    richTextBox2.Text += "There was a problem in convert file : " + InFile + "\n";
                    richTextBox2.SelectionStart = richTextBox2.Text.Length;
                    richTextBox2.ScrollToCaret();
                    Application.DoEvents();
                }

            }
           
            return true;
        }
        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                //progressBar1.Value = 100;
                //label1.Text = ((progressBar1.Value * 100) / progressBar1.Maximum).ToString() + "%";
                //Application.DoEvents();
                //if (dataGridView1.Rows[_RowIndx].Cells[4].Value.ToString() == "Ar")
                //{

                //    Random Rdm = new Random();
                //    int indx = Rdm.Next(0, progressBar1.Maximum);
                //    ThumbGenerate(indx, _TempDir + _EpisodeNum + "-Orig" + _Extention, 640, 360, _TempDir + _EpisodeNum + ".png");
                //    ThumbGenerate(indx, _TempDir + _EpisodeNum + "-Orig" + _Extention, 480, 270, _TempDir + _EpisodeNum + "-m.png");
                //    ThumbGenerate(indx, _TempDir + _EpisodeNum + "-Orig" + _Extention, 96, 54, _TempDir + _EpisodeNum + "-l.png");
                //    ThumbGenerate(indx, _TempDir + _EpisodeNum + "-Orig" + _Extention, 178, 110, _TempDir + _EpisodeNum + "-ml.png");
                //    UploadPng(_EpisodeNum + ".png");
                //    UploadPng(_EpisodeNum + "-m.png");
                //    UploadPng(_EpisodeNum + "-l.png");
                //    UploadPng(_EpisodeNum + "-ml.png");
                //}



            }));
        }
        protected void FindDuration(string Str, string ProgressControl)
        {
            string TimeCode = "";
            if (Str.Contains("Duration:"))
            {
                TimeCode = Str.Substring(Str.IndexOf("Duration: "), 21).Replace("Duration: ", "").Trim();
                string[] Times = TimeCode.Split('.')[0].Split(':');
                double Frames = double.Parse(Times[0].ToString()) * (3600) * (25) +
                    double.Parse(Times[1].ToString()) * (60) * (25) +
                    double.Parse(Times[2].ToString()) * (25);
                if (ProgressControl == "1")
                {
                    progressBar1.Maximum = int.Parse(Frames.ToString());
                }
                else
                {

                }
            }
            if (Str.Contains("time="))
            {
                try
                {
                    string CurTime = "";
                    CurTime = Str.Substring(Str.IndexOf("time="), 16).Replace("time=", "").Trim();
                    string[] CTimes = CurTime.Split('.')[0].Split(':');
                    double CurFrame = double.Parse(CTimes[0].ToString()) * (3600) * (25) +
                        double.Parse(CTimes[1].ToString()) * (60) * (25) +
                        double.Parse(CTimes[2].ToString()) * (25);

                    if (ProgressControl == "1")
                    {
                        progressBar1.Value = int.Parse(CurFrame.ToString());

                        label1.Text = ((progressBar1.Value * 100) / progressBar1.Maximum).ToString() + "%";
                    }
                    else
                    {

                    }

                    //label3.Text = CurFrame.ToString();
                    Application.DoEvents();
                }
                catch
                {


                }
            }
            if (Str.Contains("fps="))
            {

                string Speed = "";

                Speed = Str.Substring(Str.IndexOf("fps="), 8).Replace("fps=", "").Trim();

                label4.Text = "Speed: " + (float.Parse(Speed) / 25).ToString() + " X ";
                Application.DoEvents();

            }
        }
        protected string GetDuration(string InFile)
        {
            Process proc = new Process(); if (Environment.Is64BitOperatingSystem)
            {
                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
            }
            else
            {
                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
            }

          
         
                proc.StartInfo.Arguments = "-i " + "\"" + InFile + "\"" ;
          
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.Exited += new EventHandler(myProcess_Exited);
            if (!proc.Start())
            {
               
            }  
            //proc.PriorityClass = ProcessPriorityClass.Normal;
            StreamReader reader = proc.StandardError;
            string line;
            string TimeCode = "";
            double Frames = 0;
            string TimeString = "00:00:00";
          
              
            while ((line = reader.ReadLine()) != null)
            {
                try
                {
                    if (line.Contains("Duration:"))
                    {
                        TimeCode = line.Substring(line.IndexOf("Duration: "), 21).Replace("Duration: ", "").Trim();
                        string[] Times = TimeCode.Split('.')[0].Split(':');                      
                        Frames = double.Parse(Times[0].ToString()) * (3600) * (50) +
                            double.Parse(Times[1].ToString()) * (60) * (50) +
                            double.Parse(Times[2].ToString()) * (50);

                        ///
                        TimeString = TimeCode.Split('.')[0].ToString();
                    }
                }
                catch (Exception Exp)
                {

                    throw;
                }
            }
            proc.Close();

            return TimeString;
        }    
        protected void UploadVideo(string FilePath, bool Delete)
        {
            try
            {
                richTextBox2.BackColor = Color.MidnightBlue;
                string FileNameSuffix = "";

                var ftpWebRequest = (FtpWebRequest)WebRequest.Create(System.Configuration.ConfigurationSettings.AppSettings["Server"].Trim() +
               "/" + ((NewListItem)(comboBox1.SelectedItem)).Value + "/" + _DateFolder + "/" + Path.GetFileNameWithoutExtension(FilePath) + ".mp4");

                if (!checkBox6.Checked)
                {
                    ftpWebRequest = (FtpWebRequest)WebRequest.Create(System.Configuration.ConfigurationSettings.AppSettings["Server"].Trim() +
               "/" + ((NewListItem)(comboBox1.SelectedItem)).Value + "/" + Path.GetFileNameWithoutExtension(FilePath) + ".mp4");
                }





                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpWebRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationSettings.AppSettings["UserName"].Trim(),
                    System.Configuration.ConfigurationSettings.AppSettings["PassWord"].Trim());
                ftpWebRequest.UsePassive = true;
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.KeepAlive = true;
                //  ftpWebRequest. = 10000;
                using (var inputStream = File.OpenRead(FilePath))
                using (var outputStream = ftpWebRequest.GetRequestStream())
                {
                    var buffer = new byte[32 * 1024];
                    int totalReadBytesCount = 0;
                    int readBytesCount;
                    //while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    //{
                    //    outputStream.Write(buffer, 0, readBytesCount);
                    //    totalReadBytesCount += readBytesCount;
                    //    var progress = totalReadBytesCount * 100.0 / inputStream.Length;
                    //    progressBar1.Value = (int)Math.Ceiling(double.Parse(progress.ToString()));
                    //    label1.Text = progress.ToString() + "%";
                    //    Application.DoEvents();
                    //}
                    long length = inputStream.Length;
                    long bfr = 0;
                    progressBar1.Maximum = 100;
                    richTextBox2.Text += "Start Upload Ftp: " + ftpWebRequest.RequestUri.ToString() + "\n";
                    richTextBox2.SelectionStart = richTextBox2.Text.Length;
                    richTextBox2.ScrollToCaret();
                    Application.DoEvents();
                    while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bfr += readBytesCount;
                        int Pr = (int)Math.Ceiling(double.Parse(((bfr * 100) / length).ToString()));
                        progressBar1.Value = Pr;
                        outputStream.Write(buffer, 0, readBytesCount);
                        label1.Text = Pr.ToString() + "%";
                        Application.DoEvents();
                    }
                }
                richTextBox2.Text += "End Upload To Ftp: " + ftpWebRequest.RequestUri.ToString() + "\n";
                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                richTextBox2.ScrollToCaret();
                Application.DoEvents();
                if (Delete)
                {
                    File.Delete(FilePath);
                    richTextBox2.Text += "Delete File: " + FilePath + "\n";
                    richTextBox2.SelectionStart = richTextBox2.Text.Length;
                    richTextBox2.ScrollToCaret();
                    Application.DoEvents();
                }

            }
            catch (Exception Exp)
            {

                Application.DoEvents();
                richTextBox2.BackColor = Color.Red;
                richTextBox2.Text += "Error Upload To Ftp: " + FilePath + "\n";
                richTextBox2.Text += Exp.Message + "\n";
                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                richTextBox2.ScrollToCaret();
                Application.DoEvents();

                try
                {
                    foreach (var process in Process.GetProcessesByName("ffmpeg64"))
                    {
                        process.Kill();
                    }
                    foreach (var process in Process.GetProcessesByName("ffmpeg32"))
                    {
                        process.Kill();
                    }
                }
                catch
                {
                }
                Thread.Sleep(5000);
                if (FilePath.ToLower().Contains("low"))
                {
                    UploadVideo(FilePath, true);
                }
                else
                {
                    UploadVideo(FilePath, false);
                }
            }

        }
        protected void ThumbGenerate(double TimeSec, string FileName, int Width, int Height, string ImageFileName)
        {
            //  System.Diagnostics.Process.Start(_DirPath);
            double SelectedTime = TimeSec;
            SelectedTime = Math.Round((SelectedTime * 25));
            Process proc = new Process();
            if (Environment.Is64BitOperatingSystem)
            {
                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
            }
            else
            {
                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
            }
            proc.StartInfo.Arguments = "-i " + "\"" + FileName + "\"" + " -filter:v select=\"eq(n\\," + SelectedTime.ToString() + ")\",scale=" + Width + ":" + Height + ",crop=iw:" + Height + " -vframes 1  -y    \"" + ImageFileName + "\"";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.Exited += new EventHandler(Thumb_Exited);
            if (!proc.Start())
            {
                richTextBox1.Text += " \n" + "Error starting";
                return;
            }
            StreamReader reader = proc.StandardError;
            string line;
            richTextBox2.Text += "Start create Image: " + ImageFileName + "\n";
            richTextBox2.SelectionStart = richTextBox2.Text.Length;
            richTextBox2.ScrollToCaret();
            Application.DoEvents();
            while ((line = reader.ReadLine()) != null)
            {
                if (richTextBox1.Lines.Length > 5)
                {
                    richTextBox1.Text = "";
                }
                richTextBox1.Text += (line) + " \n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();
            }
            proc.Close();
            richTextBox2.Text += "End Create Image: " + ImageFileName + "\n";
            richTextBox2.SelectionStart = richTextBox2.Text.Length;
            richTextBox2.ScrollToCaret();
            Application.DoEvents();
        }
        private void Thumb_Exited(object sender, System.EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
            {


            }));
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = comboBox1.SelectedIndex;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add("HQ", GetDuration(textBox1.Text));
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = !comboBox2.Enabled;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !dateTimePicker1.Enabled;
        }
        //protected void UploadPng(string FileName)
        //{
        //    try
        //    {
        //        var ftpWebRequest = (FtpWebRequest)WebRequest.Create(System.Configuration.ConfigurationSettings.AppSettings["Server"].Trim() +
        //      "/" + dataGridView1.Rows[_RowIndx].Cells[1].Value.ToString().Trim() + "/" + FileName);

        //        ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
        //        ftpWebRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationSettings.AppSettings["UserName"].Trim(),
        //            System.Configuration.ConfigurationSettings.AppSettings["PassWord"].Trim());
        //        ftpWebRequest.UsePassive = true;
        //        ftpWebRequest.UseBinary = true;
        //        ftpWebRequest.KeepAlive = false;
        //        //  ftpWebRequest. = 10000;
        //        using (var inputStream = File.OpenRead(_TempDir + FileName))
        //        using (var outputStream = ftpWebRequest.GetRequestStream())
        //        {
        //            var buffer = new byte[32 * 1024];
        //            int totalReadBytesCount = 0;
        //            int readBytesCount;
        //            //while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        //            //{
        //            //    outputStream.Write(buffer, 0, readBytesCount);
        //            //    totalReadBytesCount += readBytesCount;
        //            //    var progress = totalReadBytesCount * 100.0 / inputStream.Length;
        //            //    progressBar1.Value = (int)Math.Round( double.Parse(progress.ToString()));
        //            //    label1.Text = ((progressBar1.Value * 100) / progressBar1.Maximum).ToString() + "%";
        //            //    Application.DoEvents();
        //            //}
        //            long length = inputStream.Length;
        //            long bfr = 0;
        //            progressBar1.Maximum = 100;
        //            richTextBox2.Text += "Start Upload Image: " + ftpWebRequest.RequestUri.ToString() + "\n";
        //            richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //            richTextBox2.ScrollToCaret();
        //            Application.DoEvents();
        //            while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                bfr += readBytesCount;
        //                int Pr = (int)Math.Ceiling(double.Parse(((bfr * 100) / length).ToString()));
        //                progressBar1.Value = Pr;
        //                outputStream.Write(buffer, 0, readBytesCount);
        //                label1.Text = Pr.ToString() + "%";
        //                Application.DoEvents();
        //            }
        //            richTextBox2.Text += "End Upload Image: " + ftpWebRequest.RequestUri.ToString() + "\n";
        //            richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //            richTextBox2.ScrollToCaret();
        //            Application.DoEvents();
        //        }
        //        File.Delete(_TempDir + FileName);
        //        richTextBox2.Text += "Delete Local Image: " + _TempDir + FileName + "\n";
        //        richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //        richTextBox2.ScrollToCaret();
        //        Application.DoEvents();
        //    }
        //    catch (Exception Exp)
        //    {
        //        Application.DoEvents();
        //        richTextBox2.Text += "Error Upload To Ftp: " + _TempDir + FileName + "\n";
        //        richTextBox2.Text += Exp.Message + "\n";
        //        richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //        richTextBox2.ScrollToCaret();
        //        Application.DoEvents();
        //        Thread.Sleep(5000);
        //        UploadPng(FileName);
        //    }

        //}

        //private void button5_Click(object sender, EventArgs e)
        //{
        //    //using (Ftp ftp = new Ftp())
        //    //{
        //    //    ftp.Connect("64.150.186.181");  // or ConnectSSL for SSL
        //    //    ftp.Login("Naziri", "7617");
        //    //    ftp.SetTransferMode(FtpTransferMode.Zlib);
        //    //    ftp.ChangeFolder("test");
        //    //    ftp.Upload("1.mp4", @"C:\FILES\f\1.mp4");

        //    //    ftp.Close();
        //    //}




        //    try
        //    {
        //        string FileNameSuffix = "";

        //        var ftpWebRequest = (FtpWebRequest)WebRequest.Create(System.Configuration.ConfigurationSettings.AppSettings["Server"].Trim() +
        //       "/12.mp4");

        //        ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
        //        ftpWebRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationSettings.AppSettings["UserName"].Trim(),
        //            System.Configuration.ConfigurationSettings.AppSettings["PassWord"].Trim());
        //        ftpWebRequest.UsePassive = true;
        //        ftpWebRequest.UseBinary = true;
        //        ftpWebRequest.KeepAlive = false;
        //        //  ftpWebRequest. = 10000;
        //        using (var inputStream = File.OpenRead("c:\\files\\f\\1.mp4"))
        //        using (var outputStream = ftpWebRequest.GetRequestStream())
        //        {
        //            var buffer = new byte[inputStream.Length];
        //            int totalReadBytesCount = 0;
        //            int readBytesCount;
        //            //while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        //            //{
        //            //    outputStream.Write(buffer, 0, readBytesCount);
        //            //    totalReadBytesCount += readBytesCount;
        //            //    var progress = totalReadBytesCount * 100.0 / inputStream.Length;
        //            //    progressBar1.Value = (int)Math.Ceiling(double.Parse(progress.ToString()));
        //            //    label1.Text = progress.ToString() + "%";
        //            //    Application.DoEvents();
        //            //}
        //            long length = inputStream.Length;
        //            long bfr = 0;
        //            progressBar1.Maximum = 100;
        //            richTextBox2.Text += "Start Upload To Ftp: " + ftpWebRequest.RequestUri.ToString() + "\n";
        //            richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //            richTextBox2.ScrollToCaret();
        //            Application.DoEvents();
        //            while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                bfr += readBytesCount;
        //                int Pr = (int)Math.Ceiling(double.Parse(((bfr * 100) / length).ToString()));
        //                progressBar1.Value = Pr;
        //                outputStream.Write(buffer, 0, readBytesCount);
        //                label1.Text = Pr.ToString() + "%";
        //                Application.DoEvents();
        //            }
        //        }
        //        richTextBox2.Text += "End Upload To Ftp: " + ftpWebRequest.RequestUri.ToString() + "\n";
        //        richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //        richTextBox2.ScrollToCaret();
        //        Application.DoEvents();
        //        richTextBox2.Text += "Delete File: " + _TempDir + _EpisodeNum + "-Orig" + _Extention + "\n";
        //        richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //        richTextBox2.ScrollToCaret();
        //        Application.DoEvents();
        //        File.Delete(_TempDir + _EpisodeNum + "-Orig" + _Extention);
        //        File.Delete(_TempDir + _EpisodeNum + ".mp4");
        //        richTextBox2.Text += "Delete File: " + _TempDir + _EpisodeNum + ".mp4" + "\n";
        //        richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //        richTextBox2.ScrollToCaret();
        //        Application.DoEvents();
        //        dataGridView1.Rows[_RowIndx].Cells[3].Value = "Done";
        //        richTextBox2.Text += "Task Finished: " + _TempDir + _EpisodeNum + ".mp4" + "\n";
        //        richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //        richTextBox2.ScrollToCaret();
        //        Application.DoEvents();


        //    }
        //    catch (Exception Exp)
        //    {
        //        Application.DoEvents();
        //       // richTextBox2.Text += "Error Upload To Ftp: " + _TempDir + _EpisodeNum + ".mp4" + "\n";
        //        richTextBox2.Text += Exp.Message + "\n";
        //        richTextBox2.SelectionStart = richTextBox2.Text.Length;
        //        richTextBox2.ScrollToCaret();
        //        Application.DoEvents();
        //        Thread.Sleep(5000);
        //        UploadVideo();
        //    }
        //}
    }
}
