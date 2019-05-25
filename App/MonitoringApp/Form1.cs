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

    public enum Command
    {
        Pump_1_On = 1,
        Pump_2_On = 2,
        Pump_3_On = 4,
        ValveOff = 8,
        PumpOneOff = 16,
        PumpTwoOff = 32,
        Start = 64,
        Stop = 128

    }
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
            catch (Exception ex)
            {
                state = 0;
            }

            byte[] bytestate = BitConverter.GetBytes(state);

            UpdateGraphics(bytestate);
            log.addText(string.Format("Received: {0}", data) + Environment.NewLine);
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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphicsObj;

            graphicsObj = this.CreateGraphics();

            graphicsObj.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(new Point(300, 75), new Size(200, 200)));
            graphicsObj.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(new Point(310, 85), new Size(180, 180)));

            Pen linePen = new Pen(Color.MediumSpringGreen, 4);        //pixul pentru desenat nivelele

            graphicsObj.DrawLine(linePen, new Point(290, 240), new Point(310, 240));
            graphicsObj.DrawLine(linePen, new Point(290, 145), new Point(310, 145));
            graphicsObj.DrawLine(linePen, new Point(290, 100), new Point(310, 100));

            linePen.Color = Color.SteelBlue;      //pixul pentru desenat valva

            graphicsObj.DrawLine(linePen, new Point(280, 120), new Point(320, 120));
            graphicsObj.DrawLine(linePen, new Point(320, 120), new Point(320, 130));

            linePen.Color = Color.LightBlue;        //pixul pentru desenat pompa

            graphicsObj.DrawLine(linePen, new Point(330, 270), new Point(330, 290));
            graphicsObj.DrawLine(linePen, new Point(400, 270), new Point(400, 290));
            graphicsObj.DrawLine(linePen, new Point(470, 270), new Point(470, 290));
        }

        private void UpdateGraphics(byte[] bytestate)
        {
            Graphics graphicsObj;

            BitArray array = new BitArray(bytestate);
            graphicsObj = this.CreateGraphics();

            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.Blue, 4), new Point(330, 270), new Point(330, 290));
                        else
                            graphicsObj.DrawLine(new Pen(Color.LightBlue, 4), new Point(330, 270), new Point(330, 290));
                        break;
                    case 1:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.Blue, 4), new Point(400, 270), new Point(400, 290));
                        else
                            graphicsObj.DrawLine(new Pen(Color.LightBlue, 4), new Point(400, 270), new Point(400, 290));
                        break;
                    case 2:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.Blue, 4), new Point(470, 270), new Point(470, 290));
                        else
                            graphicsObj.DrawLine(new Pen(Color.LightBlue, 4), new Point(470, 270), new Point(470, 290));
                        break;
                    case 3:
                        if (array[i] == true)
                            graphicsObj.DrawLine(new Pen(Color.DeepSkyBlue, 4), new Point(280, 120), new Point(300, 120));
                        else
                            graphicsObj.DrawLine(new Pen(Color.SteelBlue, 4), new Point(280, 120), new Point(300, 120));
                        break;
                    case 4:
                        if (array[i] == true)
                        {
                            graphicsObj.DrawLine(new Pen(Color.Red, 4), new Point(290, 240), new Point(310, 240));
                            graphicsObj.FillRectangle(new SolidBrush(Color.Blue), 312, 240, 176, 23);
                        }
                        else
                        {
                            graphicsObj.DrawLine(new Pen(Color.MediumSpringGreen, 4), new Point(290, 240), new Point(310, 240));
                            graphicsObj.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), 312, 240, 176, 23);
                        }
                        break;
                    case 5:
                        if (array[i] == true)
                        {
                            graphicsObj.DrawLine(new Pen(Color.Red, 4), new Point(290, 145), new Point(310, 145));
                            graphicsObj.FillRectangle(new SolidBrush(Color.Blue), 312, 145, 176, 100);
                        }
                        else
                        {
                            graphicsObj.DrawLine(new Pen(Color.MediumSpringGreen, 4), new Point(290, 145), new Point(310, 145));
                            graphicsObj.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), 312, 145, 176, 100);
                        }
                        break;
                    case 6:
                        if (array[i] == true)
                        {
                            graphicsObj.DrawLine(new Pen(Color.Red, 4), new Point(290, 100), new Point(310, 100));
                            graphicsObj.FillRectangle(new SolidBrush(Color.Blue), 312, 100, 176, 45);
                        }
                        else
                        {
                            graphicsObj.DrawLine(new Pen(Color.MediumSpringGreen, 4), new Point(290, 100), new Point(310, 100));
                            graphicsObj.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), 312, 100, 176, 45);
                            graphicsObj.DrawLine(new Pen(Color.SteelBlue, 4), new Point(310, 120), new Point(320, 120));
                            graphicsObj.DrawLine(new Pen(Color.SteelBlue, 4), new Point(320, 120), new Point(320, 130));
                        }
                        break;
                }
            }
        }

        private void SendCommand(byte command, byte fillingSpeed)
        {
            //change IP address to the machine where you want to send the message to
            if (client == null)
            {
                client = new TcpClient("127.0.0.1", 2000);
            }

            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = new byte[2];
            bytesToSend[0] = (byte)command;
            bytesToSend[1] = (byte)fillingSpeed;

            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        private void StartProcess_Click(object sender, EventArgs e)
        {
            try
            {
                SendCommand((byte)Command.Start, Convert.ToByte(textBox1.Text));
                processStarted = true;
            }
            catch (Exception ex)
            {
                log.addText(ex.ToString());
            }
        }

        private bool CheckTextbox()
        {
            try
            {
                byte value = Convert.ToByte(textBox1.Text);
            }
            catch (Exception MyException)
            {
                log.addText("Dimensiune necurespunzatoare! \n");
                return false;
            }
            return true;
        }

        private int CheckBoxes()
        {
            int result = 0;

            if (checkBox2.Checked == true)
                result += 1;

            if (checkBox3.Checked == true)
                result += 1;

            return result;
        }

        private void Button1_Click(object sender, EventArgs e)                  //butonul Update
        {
            if (CheckTextbox() == true)
                if (CheckBoxes() < 2)
                {
                    if (checkBox2.Checked)
                        SendCommand((byte)Command.PumpOneOff, Convert.ToByte(Int32.Parse(textBox1.Text)));
                    if (checkBox3.Checked)
                        SendCommand((byte)Command.PumpTwoOff, Convert.ToByte(Int32.Parse(textBox1.Text)));
                    if (CheckBoxes() == 0)
                        SendCommand((byte)0, Convert.ToByte(Int32.Parse(textBox1.Text)));
                }
        }

        private void StopProcess_Click(object sender, EventArgs e)
        {
            try
            {
                if (processStarted == true)
                {
                    processStarted = false;
                    SendCommand((byte)Command.Stop, Convert.ToByte(textBox1.Text));
                }
                else
                    throw new Exception("The process hasn't been started!");
            }
            catch (Exception ex)
            {
                log.addText(ex.ToString() + Environment.NewLine);
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)        //Show log checkbox
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                if(textBox1.Text.CompareTo(string.Format("test")) == 0)
                {
                    log.addText("GRAPHICS TEST MODE \n");
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)1, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)3, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)7, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)15, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)31, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)63, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)127, 0x00 });
                    System.Threading.Thread.Sleep(3000);
                    UpdateGraphics(new byte[] { (byte)63, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)31, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)15, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)7, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)3, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)1, 0x00 });
                    System.Threading.Thread.Sleep(1000);
                    UpdateGraphics(new byte[] { (byte)0, 0x00 });
                    MessageBox.Show("THAT WAS AMAZING. \n     View log.");

                }
                else
                    log.Show();
            }
            else
                if (checkBox1.CheckState == CheckState.Unchecked)
                log.Hide();
        }
    }
}
