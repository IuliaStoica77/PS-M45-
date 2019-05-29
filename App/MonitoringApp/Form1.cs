using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        private readonly Int32 portListen = 33000;
        private readonly Int32 portSend = 33001;

        public Form1()
        {
            InitializeComponent();

            log = new Log();                //se instantiaza un nou formular (LOG)

            log.addText("Start Listening" + Environment.NewLine);

            bck = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            bck.ProgressChanged += Update;
            bck.DoWork += Listen;

            bck.RunWorkerAsync();               //se porneste threadul de listen de la server

            label12.Text = String.Format("{0} l/s", Int32.Parse(textBox1.Text)*2);                  //se actualizeaza debitul de umplere
        }


        private void Listen(object sender, DoWorkEventArgs e)                   //functie thread pentru a primi date de la server
        {
            TcpListener listener = null;

            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                listener = new TcpListener(localAddr, portListen);    

                listener.Start();         //pornire server

                Byte[] bytes = new Byte[2];             //buffer pentru datele primite

                while (true)
                {
                    bck.ReportProgress(0, "Waiting for a connection... ");              //raportare progres

                    TcpClient client = listener.AcceptTcpClient();            //acceptare client

                    bck.ReportProgress(0, "Connected!");            //raportare progres

                    stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)          //primire date
                    {
                        bck.ReportProgress(0, bytes[0].ToString());         //raportare progres
                    }

                    client.Close();             //oprire conexiune
                }

            }
            catch (Exception ex)
            {
                bck.ReportProgress(0, string.Format("SocketException: {0}", ex.ToString()));            //raportare progres
            }
        }

        private void Update_Details(byte state)                 //functie pentru a actualiza detaliile debitelor in relatie cu starea primita de la server a procesului
        {
            byte maskResultPumps = (byte)(state & 7);
            byte maskResultValve = (byte)(state & 8);

            if (maskResultValve == 0)               //daca valva nu e pornita
                label12.Text = "0 l/s";

            if(maskResultPumps == 1 | maskResultPumps == 2 | maskResultPumps == 4)              //daca 1 pompe pornite
                label16.Text = "150 l/s";

            if (maskResultPumps == 3 | maskResultPumps == 5 | maskResultPumps == 6)             //daca 2 pompe pornite
                label16.Text = "300 l/s";

            if (maskResultPumps == 7)               //daca 3 pompe pornite
                label16.Text = "450 l/s";

            if (maskResultPumps == 0)               //daca 0 pompe pornite
                label16.Text = "0 l/s";
        }

        private void Update(object sender, ProgressChangedEventArgs e)              //functie pentru a procesa datele primite de la bck pentru listen
        {
            byte state;

            try
            {
                state = Convert.ToByte(e.UserState.ToString());             //daca se reuseste conversia atunci se actualizeaza grafica si detaliile debitelor

                Update_Graphics(state);
                Update_Details(state);


                log.addText(string.Format("Received: {0}", state) + Environment.NewLine);
            }
            catch(Exception)
            {
                log.addText(string.Format("{0}", e.UserState.ToString()) + Environment.NewLine);                //daca nu se reuseste conversia, atunci e string si se afiseaza in log
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)                   //initializare detalii grafice
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

        private void Update_Graphics(byte bytestate)                //functie pentru actualizarea detaliilor grafice in relatie cu starea actuala a procesului
        {
            Graphics graphicsObj;

            BitArray array = new BitArray(new byte[] { bytestate, 0x00 });           //starea procesului
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

        private void SendCommand(byte command, byte fillingSpeed)               //functie pentru a trimite comenzi catre server/proces
        {
            if (client == null)
                client = new TcpClient("127.0.0.1", portSend);

            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = new byte[2];
            bytesToSend[0] = (byte)command;
            bytesToSend[1] = (byte)fillingSpeed;
            label12.Text = String.Format("{0} l/s", fillingSpeed * (int)2);

            log.addText(string.Format("Sending : {0}, {1} " + Environment.NewLine, bytesToSend[0], bytesToSend[1]));
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);         //trimit catre server
        }

        private void StartProcess_Click(object sender, EventArgs e)             //functie pentru pornirea procesului
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

        private bool Check_Textbox()                //functie pentru verificarea textului din textbox1
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

        private void Button1_Click(object sender, EventArgs e)                  //butonul Update
        {
            if (Check_Textbox() == true)
            {
                if (checkBox2.Checked & !checkBox3.Checked)
                    SendCommand((byte)Command.PumpOneOff, Convert.ToByte(Int32.Parse(textBox1.Text)));
                if (checkBox3.Checked & !checkBox2.Checked)
                    SendCommand((byte)Command.PumpTwoOff, Convert.ToByte(Int32.Parse(textBox1.Text)));
                if (!checkBox3.Checked & !checkBox2.Checked)
                    SendCommand((byte)0, Convert.ToByte(Int32.Parse(textBox1.Text)));
            }
        }

        private void StopProcess_Click(object sender, EventArgs e)              //functie pentru oprirea procesului
        {
            try
            {

                if (processStarted == true)
                {
                    processStarted = false;

                    SendCommand((byte)Command.Stop, Convert.ToByte(textBox1.Text));
                }
                else
                    throw new Exception("Process has not been started yet!");
            }
            catch(Exception ex)
            {
                log.addText("Process has not been started yet! \n");
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

                    Update_Graphics((byte)1);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)3);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)7);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)15);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)31);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)63);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)127);
                    System.Threading.Thread.Sleep(3000);

                    Update_Graphics((byte)63);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)31);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)15);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)7);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)3);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)1);
                    System.Threading.Thread.Sleep(1000);

                    Update_Graphics((byte)0);
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
