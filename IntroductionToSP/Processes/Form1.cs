using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;

namespace Processes
{
    public partial class Form1 : Form
    {
        List<Process> process_list;
        public Form1()
        {
            InitializeComponent();
            process_list = new List<Process>();
            // AllignText();
            richTextBoxProcessName.Text = "notepad"; // чтобы нотепад высвечивался при запуске
            //InitProcess();
            lvProcesses.Columns.Add("PID"); // создание колонок
            lvProcesses.Columns.Add("Name");
            lvProcesses.Columns.Add("Base Priority");
            lvProcesses.Columns.Add("Start time");
            lvProcesses.Columns.Add("Total CPU time");
            lvProcesses.Columns.Add("User CPU time");
            lvProcesses.Columns.Add("Session ID");
            lvProcesses.Columns.Add("Priority Class");
            lvProcesses.Columns.Add("Affinity"); 
            lvProcesses.Columns.Add("Threads");
        }

        //void Form1_Closing(object sender, CancelEventArgs e) //~Form1() // диструктор в C# не работает, т к заркывает в виртуалке. Нужно событие закрытия окна
        //{
        //    myProcess.CloseMainWindow();
        //    myProcess.Close();
        //}

        void InitProcess()
        {
            AllignText();
            //myProcess = new System.Diaognostic.Process(richTextBoxProcessName.Text);
            myProcess = new Process();//добавление еще одного объекта, чтобы запоминао 2 процесса
            myProcess.StartInfo = new System.Diagnostics.ProcessStartInfo(richTextBoxProcessName.Text);
            myProcess.Start();
            //process_list.Add(myProcess);
            //lvProcesses.Items.Add(myProcess.ToString());
            lvProcesses.Items.Add(myProcess.Id.ToString());
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.ProcessName);
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.BasePriority.ToString());
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.StartTime.ToString());
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.TotalProcessorTime.ToString());
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.UserProcessorTime.ToString());
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.SessionId.ToString());
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.PriorityClass.ToString());
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.ProcessorAffinity.ToString());
            lvProcesses.Items[lvProcesses.Items.Count - 1].SubItems.Add(myProcess.Threads.ToString());
            
        }

        void AllignText()
        {
            richTextBoxProcessName.SelectAll();
            richTextBoxProcessName.SelectionAlignment = HorizontalAlignment.Center; // имя процесса отображается по середине
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            InitProcess();
            //myProcess.Start();
            //Info();
            //this.TopMost = true; (должно стоять перед Info();, если нет этого, то добавить)
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            //if (process_list.Count > 0)
            if (lvProcesses.Items.Count > 0)
            {
                try
                {
                    //myProcess = process_list.Last();//First?
                    myProcess = Process.GetProcessById(Convert.ToInt32(lvProcesses.Items[lvProcesses.Items.Count - 1].Text));//text в конце пишется, чтобы выбирался только ID
                    myProcess.CloseMainWindow();
                    myProcess.Close(); //освобождает ресурсы, занимаемые процессом
                    //process_list.RemoveAt(process_list.Count - 1);
                    //lvProcesses.Items[lvProcesses.Items.Count - 1];
                    //lvProcesses.Items.RemoveByKey(myProcess.Id.ToString());
                    lvProcesses.Items.RemoveAt(lvProcesses.Items.Count - 1);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //process_list.RemoveAt(process_list.Count - 1);
                    //lvProcesses.Items.RemoveByKey(myProcess.Id.ToString());
                    lvProcesses.Items.RemoveAt(lvProcesses.Items.Count - 1);
                }
                //Info();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            while (process_list.Count > 0)
            {
                try
                {
                    process_list.First().CloseMainWindow();
                    process_list.First().Close();
                    process_list.RemoveAt(0);
                }
                catch (Exception ex)
                {
                    process_list.RemoveAt(0);
                }
            }
            
            //myProcess.CloseMainWindow();
            //myProcess.Close();
        }

        void Info()
        {
            if (process_list.Count > 0)
            {
                myProcess = process_list.First();
                labelProcessInfo.Text = $"Total process count: {process_list.Count}\n";
                labelProcessInfo.Text = "Current process:\n";
                labelProcessInfo.Text = "Process info:\n";
                labelProcessInfo.Text += $"PID:              {myProcess.Id}\n";
                labelProcessInfo.Text += $"Base Priority:    {myProcess.BasePriority}\n";
                labelProcessInfo.Text += $"Start time:       {myProcess.StartTime}\n";
                labelProcessInfo.Text += $"Total CPU time:   {myProcess.TotalProcessorTime}\n";
                labelProcessInfo.Text += $"User CPU time:    {myProcess.UserProcessorTime}\n";
                labelProcessInfo.Text += $"Session ID:       {myProcess.SessionId}\n";
                labelProcessInfo.Text += $"Priority Class:   {myProcess.PriorityClass}\n";
                labelProcessInfo.Text += $"Name:             {myProcess.ProcessName}\n";
                labelProcessInfo.Text += $"Affinity:         {myProcess.ProcessorAffinity}\n";
                labelProcessInfo.Text += $"Threads:          {myProcess.Threads.Count}\n";
            }
            else labelProcessInfo.Text = "Нет запущенных процессов";
            
        }

       
    }
}
