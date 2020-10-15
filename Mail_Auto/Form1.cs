using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SendTask;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace Mail_Auto
{
    public partial class Form1 : Form
    {
        string path = System.Environment.CurrentDirectory;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;//禁止窗体的拉伸
            toolStripStatusLabel1.Text = "准备列表中...";
            try //初始化send.txt文件
            {
                AddTxtToLst(path + @"\send.txt", listBox1);
            }
            catch (IOException ee)
            {
                MessageBox.Show(ee.Message + "\r---------------" + "\r发件人文件不存在，请在程序目录创建send.txt文件");
                Close();
            }

            try //初始化receive.txt文件
            {
                AddTxtToLst(path + @"\receive.txt", listBox2);
            }
            catch (IOException ee)
            {
                MessageBox.Show(ee.Message + "\r---------------" + "\r收件人文件不存在，请在程序目录创建receive.txt文件");
                Close();
            }
            try //初始化body.txt文件
            {
                AddTxtToTxb(path + @"\body.txt", textBox1);
            }
            catch (IOException ee)
            {
                MessageBox.Show(ee.Message + "\r---------------" + "\r内容文件不存在，请在程序目录创建body.txt文件");
                Close();
            }

            toolStripStatusLabel1.Text = "初始化完成，点击开始发送邮件。";




        }
        private void AddTxtToLst(string path, ListBox lst)
        {
            StreamReader file = new StreamReader(path, Encoding.UTF8);
            string s = "";
            while (s != null)
            {
                s = file.ReadLine();
                if (!string.IsNullOrEmpty(s))
                    lst.Items.Add(s);
            }
            file.Close();
        }
        private void AddTxtToTxb(string path, TextBox txb)
        {
            StreamReader file = new StreamReader(path, Encoding.UTF8);
            string s = "";
            while (s != null)
            {
                s = file.ReadLine();
                if (!string.IsNullOrEmpty(s))
                    txb.AppendText(s + "\r\n\r\n");
            }
            file.Close();
        }
        public void Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "整理邮件信息...";
            int Sum_Mail = listBox2.Items.Count; //获取总需要发的邮件
            int Sum_thread = listBox1.Items.Count; //一个发件人一个线程
            Sum.Text = Sum_Mail.ToString();

            progressBar1.Maximum = Sum_Mail;//设置进度条
            progressBar1.Value = 0;

            List<Task> arryTask = new List<Task>();//每个发件人创建一个对象（线程）
            for (int i = 0; i <= Sum_thread - 1; i++)//给列表添加对象
            {
                toolStripStatusLabel1.Text = "初始化第" + i + 1 + "个发件人";
                string addAndPwd = listBox1.Items[i].ToString();
                string add = addAndPwd.Split('|')[0]; //获取发件人地址
                string name = add.Split('@')[0]; //获取发件人的名字
                string pwd = addAndPwd.Split('|')[1];//获取发件人的密码
                arryTask.Add(new Task
                {
                    Mail_add = add,
                    Mail_name = name,
                    Mail_passwd = pwd,
                    Mail_receive = new List<string>(),
                });
            }
            toolStripStatusLabel1.Text = "发件人初始化完毕...";
            Thread.Sleep(200);
            toolStripStatusLabel1.Text = "正在发送...";
            if (Sum_Mail >= Sum_thread)//判断是不是发件人比收件人多
            {
                if (Sum_Mail % Sum_thread == 0)
                {
                    int group = Sum_Mail / Sum_thread;  //每个发件人负责的邮件数量
                    for (int j = 0; j < Sum_thread; j++)
                    {
                        for (int i = 0; i < group; i++)
                        {
                            int last = listBox2.Items.Count - 1;
                            string text = listBox2.Items[last].ToString();
                            // MessageBox.Show(arryTask[i].Mail_receive[0]);
                            arryTask[j].Mail_receive.Add(text);
                            listBox2.SetSelected(last, true);
                            listBox2.Items.Remove(listBox2.SelectedItem);
                        }
                        //包装好一个发件人。创建线程
                        ThreadPool.QueueUserWorkItem(new WaitCallback(send_mail), arryTask[j]);
                    }
                }
                else
                {
                    int group = Sum_Mail / Sum_thread;  //每个发件人负责的邮件数量
                    for (int j = 0; j < Sum_thread; j++)
                    {
                        for (int i = 0; i < group; i++)
                        {
                            int last = listBox2.Items.Count - 1;
                            string text = listBox2.Items[last].ToString();
                            arryTask[j].Mail_receive.Add(text);
                            listBox2.SetSelected(last, true);
                            listBox2.Items.Remove(listBox2.SelectedItem);
                        }
                        //包装好一个发件人。创建线程
                    }
                    int done = group * Sum_thread; //获取已近处理的邮件个数。
                    int remain = Sum_Mail % Sum_thread; //获取未处理的邮件个数。
                    for (int j = 0; j < Sum_thread; j++)
                    {
                        if (listBox2.Items.Count == 0) break;
                        for (int i = 0; i < remain; i++)
                        {
                            int last = listBox2.Items.Count - 1;
                            string text = listBox2.Items[last].ToString();
                            arryTask[i].Mail_receive.Add(text);
                            listBox2.SetSelected(last, true);
                            listBox2.Items.Remove(listBox2.SelectedItem);
                        }

                    }
                    for (int j = 0; j < arryTask.Count; j++)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(send_mail), arryTask[j]);
                        // textBox2.Text += arryTask[j].Mail_add +  "\r\n \r\n\r\n\r\n";
                        //for (int i = 0; i < arryTask[j].Mail_receive.Count; i++)
                        //{
                        //    textBox2.Text += arryTask[j].Mail_receive[i]+ "\r\n";
                        //}
                    }


                }

            }
        }
        private void send_mail(object task1)
        {

            try
            {
                Task task = task1 as Task;
                for (int i = 0; i < task.Mail_receive.Count; i++)
                {
                    string emailAcount = task.Mail_add;
                    string emailPassword = task.Mail_passwd;
                    string content = null;
                    this.Invoke(new Action(() => content = textBox1.Text + "\r" + task.Mail_name + "\r"));
                    MailMessage message = new MailMessage();
                    //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
                    MailAddress fromAddr = new MailAddress("randell@mipodns.org.cn");
                    message.From = fromAddr;

                    //设置抄送人
                    //message.CC.Add("5953181443@qq.com");
                    //设置邮件标题
                    message.Subject = "这是标题";
                    //设置邮件内容
                    message.Body = content;
                    //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
                    SmtpClient client = new SmtpClient("mail.mipodns.org.cn", 25);
                    //设置发送人的邮箱账号和密码
                    client.Credentials = new NetworkCredential(emailAcount, emailPassword);
                    //启用ssl,也就是安全发送
                    client.EnableSsl = false;
                    string reciver = null;
                    reciver = task.Mail_receive[i];
                    //设置收件人,可添加多个,添加方法与下面的一样
                    message.To.Add(reciver);
                    //发送邮件
                    client.Send(message);
                    int time = 120000;
                    this.Invoke(new Action(() => Success.Text = (Convert.ToInt32(Success.Text) + 1).ToString()));
                    this.Invoke(new Action(() => progressBar1.Value = Convert.ToInt32(Success.Text) + Convert.ToInt32(fail.Text)));
                    this.Invoke(new Action(() => time = Convert.ToInt32(textBox2.Text)));
                    this.Invoke(new Action(() => toolStripStatusLabel1.Text="正在发送"+ reciver));
                    Thread.Sleep(time);
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                this.Invoke(new Action(() => fail.Text = (Convert.ToInt32(fail.Text) + 1).ToString()));
                this.Invoke(new Action(() => progressBar1.Value = Convert.ToInt32(Success.Text) + Convert.ToInt32(fail.Text)));
                
            }
        }
    }
}
