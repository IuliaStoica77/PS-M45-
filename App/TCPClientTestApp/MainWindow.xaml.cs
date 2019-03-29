﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Simulator;

namespace TCPMonitor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		NetworkStream stream;
		BackgroundWorker bck;
		TcpClient client;

		public MainWindow()
		{
			InitializeComponent();            
        }



        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            tbDataReceived.Text = "Start Listening";
            bck = new BackgroundWorker();
            bck.WorkerReportsProgress = true;
            bck.ProgressChanged += Bck_ProgressChanged;
            bck.DoWork += Bck_DoWork;
            bck.RunWorkerAsync();
        }

        private void Bck_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener server = null;
            try
            {
                Int32 port = 3000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                //Start listening for client requests.
                server.Start();

                //Buffer for reading data
                Byte[] bytes = new Byte[8];

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
                        bck.ReportProgress(0, bytes[1].ToString() + ", " + bytes[6].ToString());
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

            tbDataReceived.Text = string.Format("Received: {0}", data) + Environment.NewLine + tbDataReceived.Text;
        }

        private void SendMessage(int Message)
        {
            //change IP address to the machine where you want to send the message to
            if(client==null)
            {
                client = new TcpClient("127.0.0.1", 2000);
            }
            
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = new byte[8];           
            bytesToSend[1] = (byte)Message;

            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        /// <summary>
        /// /// eveniment folosit pentru a trimite o comanda catre proces/simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnSend_Click(object sender, RoutedEventArgs e)
		{	try
			{
                SendMessage((int)Command.Started);             
			}
			catch (Exception ex)
			{
				tbDataReceived.Text = tbDataReceived + ex.ToString();
			}
		}

		

        /// <summary>
        /// eveniment folosit pentru a trimite o comanda catre proces/simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comboBox.SelectedIndex != -1)
                    switch (comboBox.SelectedIndex)
                    {
                        case 0:
                            SendMessage((int)Command.Stopped | (int)Command.Case_1);
                            break;
                        case 1:
                            SendMessage((int)Command.Stopped | (int)Command.Case_2);
                            break;
                        case 2:
                            SendMessage((int)Command.Stopped | (int)Command.Case_3);
                            break;
                        case 3:
                            SendMessage((int)Command.Stopped | (int)Command.Case_4);
                            break;
                    }
                else throw new Exception("Nu a fost selectat un scenariu!");
            }
            catch (Exception ex)
            {
                tbDataReceived.Text = tbDataReceived + ex.ToString();
            }
        }
    }
}
