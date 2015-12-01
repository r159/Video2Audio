using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace V2A
{
    public partial class Video2Audio : Form
    {
        List<string> FilePaths = new List<string>();
        public string folderPath = @"C:\Ffmeg\newoutput\";
        OrderedDictionary  FileNamePath = new OrderedDictionary();
        List<string> avoidedFiles = new List<string>();
        public Video2Audio()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           BackgroundWorker bg2 = new BackgroundWorker();
           bg2.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
           bg2.RunWorkerAsync();
            
            if((textBox2.Text != "") || (textBox2.Text != null) && FileNamePath.Count > 1)
            {
                     ProcessV2a();
                     
            }
            else
            {
                MessageBox.Show("Please select Destination foler and atleast one file to convert");

            }
         
       
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label6.Visible = false;
            listBox4.Visible = false;
            this.listBox1.Items.Clear();
             OpenFileDialog fileDialog = new OpenFileDialog();
            
            fileDialog.InitialDirectory = @"C:\";
            fileDialog.Multiselect = true;
            fileDialog.Title = "Upload Files";
           
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                int count = fileDialog.FileNames.Count();
                label3.Text = label3.Text + count.ToString();
                foreach (string item in fileDialog.FileNames)
                {
                    FileInfo f = new FileInfo(item);
                    long length =  f.Length;
                    long lengthinMB = (length / (1024 * 1024));
                    if (lengthinMB > 120)
                    {
                       // avoidedFiles.Add(Path.GetFileName(item));
                        label6.Visible = true;
                        listBox4.Visible = true;
                        listBox4.Items.Add(Path.GetFileName(item));
                    }
                    else
                    {
                        listBox1.Items.Add(Path.GetFileName(item));
                        FilePaths.Add(item);
                        FileNamePath.Add(Path.GetFileName(item), item);
                    }

                }
       

            }
            

        }

        private async void ProcessV2a()
        {
           
            Task[] tasks = new Task[10];
            int filescount = FileNamePath.Count;
            object[] keys = new object[FileNamePath.Keys.Count];
            FileNamePath.Keys.CopyTo(keys, 0);
            if (filescount <= 10) {
                for (int taskNumber =1; taskNumber <= filescount;)
                {
                  string filename = keys[taskNumber-1].ToString();
                  string filepath = FileNamePath[taskNumber-1].ToString();
                    tasks[taskNumber-1] = Task.Factory.StartNew(() => SendFilesToffmpeg(filename,filepath));
                    taskNumber = taskNumber + 1;
                     
                }
                textBox2.Text = "";
                FileNamePath.Clear();
               
            }
            else
            {
                int taskNumber =0;
                int tasklimit = 10;
                int startTask = 1;
                int curretiteration =0;
                int addnumber = 0;
                int adnumber1 = 1;
                int filestemp = 0;
                float noofiteration = (filescount / 10F);
                double noofiterations = Math.Ceiling(noofiteration);
                while (curretiteration < noofiterations)
                {

                    if (noofiterations > 1 && curretiteration != 0)
                    {
                        int tempcurretiteration = curretiteration + 1;
                        //int tempcurr2 = curretiteration + 2;
                        tasklimit = int.Parse(tempcurretiteration.ToString() + addnumber.ToString());  //curretiteration + Int16.Parse("0");
                        tasklimit = tasklimit > filescount ? filescount : tasklimit;
                        tasklimit = tasklimit - 10;
                        tasks = new Task[tasklimit];
                        startTask = int.Parse(curretiteration.ToString() + adnumber1.ToString());
                        startTask = startTask > tasklimit ? (startTask - 10) : startTask;

                    }

                    for (taskNumber = startTask; taskNumber <= tasklimit;)
                    {
                        string filename = keys[filestemp].ToString();
                        string filepath = FileNamePath[filestemp].ToString();
                        tasks[taskNumber - 1] = Task.Factory.StartNew(() => SendFilesToffmpeg(filename, filepath));
                        taskNumber = taskNumber + 1;
                        filestemp = filestemp + 1;
                    }
                  //  Task.WaitAll();
                    curretiteration = curretiteration + 1;
                   // Task.WaitAll();
                    await Task.WhenAll(tasks);
                    //for (int i = 1; i < tasks.Count(); i++)
                    //{
                    //    if (tasks[i] != null)
                    //        tasks[i].Wait();
                    //}
                }
                textBox2.Text = "";
                FileNamePath.Clear();
               
            }
           
        }

        private void SendFilesToffmpeg(string filename,string filepath)
        {
           
            try {
                //MessageBox.Show("From SendFiles" + FileNamePath.Count.ToString());
               
           
        
            //this.listBox2.Items.Add(filename);
            this.Invoke((Action)(() => listBox2.Items.Add(filename)));
            string filenamewithoutext = filename.Remove(filename.IndexOf("."));
            string convertedfilename = filenamewithoutext + ".mp3";
            string outpupath = folderPath + "\\" +convertedfilename;
            string cmmd = " -i \"" + filepath + "\" \"" + outpupath + "\"";
            //-i input -vn -f mp3 -ab 192k output.mp3

            //string filepathexe = @"C:\Ffmeg\ffmpeg.exe";
            string app = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] getapp = app.Split(new string[] { "bin" }, StringSplitOptions.None);
            string ffmpegpath = getapp[0].ToString() + "ffmpeg" + "\\" + "ffmpeg.exe";
            string filepathexe = ffmpegpath;
            //InsertLogstotable(filepathexe);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = filepathexe;

            //Path of exe that will be executed, only for "filebuffer" it will be "flvtool2.exe"

            proc.StartInfo.Arguments = cmmd;

            //The command which will be executed

            proc.StartInfo.UseShellExecute = false;

            proc.StartInfo.CreateNoWindow = true;

            proc.StartInfo.RedirectStandardOutput = false;

            proc.Start();

            while (proc.HasExited == false)
            {

            }
           }
            
            catch(Exception e)
            {

            }
            finally
            {
              //  this.listBox3.Items.Add(filename);
                this.Invoke((Action)(() => listBox3.Items.Add(filename)));
                //myForm.Invoke((Action)(() => listBox3.Items.Clear()));
                //this.Invoke((Action)(() => listBox3.Items.Clear()));
                this.Invoke((Action)(() => listBox2.Items.Remove(filename)));
                this.Invoke((Action)(() => listBox1.Items.Remove(filename)));

                


            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
           
           
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
                textBox2.Text = folderPath;
            }
        }

        private void Video2Audio_Load(object sender, EventArgs e)
        {
           // string applicationDirectory = Application.ExecutablePath;
            label6.Visible = false;
            listBox4.Visible = false;
           
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bool islistboxOneContains = true;
            while (islistboxOneContains)
            {
                if(listBox1.Items.Count >= 1)
                {

                      this.Invoke((Action)(() => button2.Enabled = false));
                    this.Invoke((Action)(() =>button3.Enabled = false));
                    this.Invoke((Action)(() =>button1.Enabled = false));
                }
                else
                {
                    this.Invoke((Action)(() =>button2.Enabled = true));
                    this.Invoke((Action)(() =>button3.Enabled = true));
                    this.Invoke((Action)(() =>button1.Enabled = true));
                    islistboxOneContains = false;
                }


            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileNamePath.Clear();
            textBox2.Text = "";
            listBox1.Items.Clear();
            label6.Visible = false;
            listBox4.Visible = false;
        }
    }
}
