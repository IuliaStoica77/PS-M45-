using SimulatorImproved;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace MonitoringApp
{
    public partial class Form1 : Form
    {
        private NetworkStream stream;
        private BackgroundWorker bck;
        private TcpClient client;
        private readonly Log log;
        private bool processStarted;
        private readonly Int32 port = 30000;

        public Form1()
        {
            log = new Log();
            StartListening();
            InitializeComponent();
        }

        private void StartListening()
        {
            log.addText("Start Listening" + Environment.NewLine);
            bck = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            bck.ProgressChanged += Bck_ProgressChanged;
            bck.DoWork += Bck_DoWork;
            bck.RunWorkerAsync();
        }

        private void Bck_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                //Start listening for client requests.
                server.Start();

                //Buffer for reading data
                Byte[] bytes = new Byte[2];

                //Enter the listening loop.
                while (true)
                {
                    bck.ReportProgress(0, "Waiting for a connection... ");

                    //Accept TcpClient
                    TcpClient client = server.AcceptTcpClient();
                    bck.ReportProgress(0, "Connected!");


                    stream = client.GetStream();

                    int i;

                    //Get all data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        //display received data by reporting progress to the background worker

                        //bck.ReportProgress(0, bytes[1].ToString() + ", " + bytes[6].ToString());
                        bck.ReportProgress(0, bytes[0].ToString());
                    }

                    //Shutdown and end connection
                    client.Close();
                }

            }
            catch (Exception ex)
            {
                bck.ReportProgress(0, string.Format("SocketException: {0}", ex.ToString()));
            }
        }

        private void Bck_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string data = (string)e.UserState;
            int state;
            try
            {
                state = Int32.Parse(data);
            }
            catch(Exception ex)
            {
                state = 0;
            }
            byte[] bytestate = BitConverter.GetBytes(state);
            UpdateGraphics(bytestate);
            log.addText(string.Format("Received: {0}", data) + Environment.NewLine);
        }

        Graphics graphicsObj;

        private void UpdateGraphics(byte[] bytestate)
        {
            //Graphics graphicsObj;

            BitArray array = new BitArray(bytestate);
            graphicsObj = this.CreateGraphics();

            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        if(array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.LightGreen, 4), new Point(330, 295), new Point(330, 315));
                        else
                            graphicsObj.DrawLine(new Pen(Color.Green, 4), new Point(330, 295), new Point(330, 315));
                        break;
                    case 1:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.LightGreen, 4), new Point(400, 295), new Point(400, 315));
                        else
                            graphicsObj.DrawLine(new Pen(Color.Green, 4), new Point(400, 295), new Point(400, 315));
                        break;
                    case 2:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.LightGreen, 4), new Point(470, 295), new Point(470, 315));
                        else
                            graphicsObj.DrawLine(new Pen(Color.Green, 4), new Point(470, 295), new Point(470, 315));
                        break;
                    case 3:
                        if (array[i] == true)
                        {
                            graphicsObj.DrawLine(new Pen(System.Drawing.Color.IndianRed, 4), new Point(280, 145), new Point(320, 145));
                            graphicsObj.DrawLine(new Pen(System.Drawing.Color.IndianRed, 4), new Point(320, 145), new Point(320, 155));
                        }
                        else
                        {
                            graphicsObj.DrawLine(new Pen(System.Drawing.Color.Red, 4), new Point(280, 145), new Point(320, 145));
                            graphicsObj.DrawLine(new Pen(System.Drawing.Color.Red, 4), new Point(320, 145), new Point(320, 155));
                        }
                        break;
                    case 4:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.LightBlue, 4), new Point(290, 270), new Point(310, 270));
                        else
                            graphicsObj.DrawLine(new Pen(Color.Blue, 4), new Point(290, 270), new Point(310, 270));
                        break;
                    case 5:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.LightBlue, 4), new Point(290, 170), new Point(310, 170));
                        else
                            graphicsObj.DrawLine(new Pen(Color.Blue, 4), new Point(290, 170), new Point(310, 170));
                        break;
                    case 6:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.LightBlue, 4), new Point(290, 120), new Point(310, 120));
                        else
                            graphicsObj.DrawLine(new Pen(Color.Blue, 4), new Point(290, 120), new Point(310, 120));
                        break;
                    //case 7:
                    //    if (array[i] == true)
                    //        graphicsObj.DrawLine(new Pen(Color.LightGreen, 4), new Point(330, 295), new Point(330, 315));
                    //    else
                    //        graphicsObj.DrawLine(new Pen(Color.Green, 4), new Point(330, 295), new Point(330, 315));
                    //    break;
                }
            }
        }

        private void SendMessage(int Message)
        {
            //change IP address to the machine where you want to send the message to
            if (client == null)
            {
                client = new TcpClient("127.0.0.1", 2000);
            }

            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = new byte[2];
            bytesToSend[0] = (byte)Message;

            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        private void StartProcess_Click(object sender, EventArgs e)
        {
            try
            {
                SendMessage((int)Command.Started);
                processStarted = true;
            }
            catch (Exception ex)
            {
                log.addText(ex.ToString());
            }
        }

        private void StopProcess_Click(object sender, EventArgs e)
        {
            try
            {
                if (processStarted == true)
                {
                    processStarted = false;
                    if (radioButton1.Checked == true)
                        SendMessage((int)Command.Stopped | (int)Command.Case_1);
                    else
                        if (radioButton2.Checked == true)
                        SendMessage((int)Command.Stopped | (int)Command.Case_2);
                    else
                            if (radioButton3.Checked == true)
                        SendMessage((int)Command.Stopped | (int)Command.Case_3);
                    else
                                if (radioButton4.Checked == true)
                        SendMessage((int)Command.Stopped | (int)Command.Case_4);
                    else
                    {
                        processStarted = true;
                        throw new Exception("Scenario not selected!");
                    }
                }
                else
                {
                    processStarted = true;
                    throw new Exception("The process hasn't been started!");
                }
            }
            catch (Exception ex)
            {
                log.addText(ex.ToString() + Environment.NewLine);
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
                log.Show();
            else
                if (checkBox1.CheckState == CheckState.Unchecked)
                    log.Hide();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphicsObj;

            graphicsObj = this.CreateGraphics();

            graphicsObj.DrawRectangle(new Pen(System.Drawing.Color.Black, 2), new Rectangle(new Point(300, 100), new Size(200, 200)));

            Pen linePen = new Pen(System.Drawing.Color.Blue, 4);

            graphicsObj.DrawLine(linePen, new Point(290, 270), new Point(310, 270));
            graphicsObj.DrawLine(linePen, new Point(290, 170), new Point(310, 170));
            graphicsObj.DrawLine(linePen, new Point(290, 120), new Point(310, 120));

            linePen.Color = Color.Red;

            graphicsObj.DrawLine(linePen, new Point(280, 145), new Point(320, 145));
            graphicsObj.DrawLine(linePen, new Point(320, 145), new Point(320, 155));

            linePen.Color = Color.Green;

            graphicsObj.DrawLine(linePen, new Point(330, 295), new Point(330, 315));
            graphicsObj.DrawLine(linePen, new Point(400, 295), new Point(400, 315));
            graphicsObj.DrawLine(linePen, new Point(470, 295), new Point(470, 315));
        }
    }
}
