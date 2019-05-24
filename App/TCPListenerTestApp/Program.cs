using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimulatorImproved;

namespace TCP_PLC
{
    /// <summary>
    /// Program  folosit  pentru a simula functionarea automatului.
    /// Exista o clasa Sender care trimite date catre apliatia de monitorizare
    /// si o clasa Listner care primeste comenzi de la aplicatia de monitorizare
    /// partea de comunicare cu aplicatia de monitorizare trebuie implementata in paralel
    /// existand un thread separat pentru transmiterea de date si un thread separat pentru receptionarea de date
    /// </summary>
    public class Program
	{        
        static Simulator process;

		static void Main()
		{
            Console.WriteLine("Press enter to start...");
            Console.ReadLine();

            process = new Simulator(50);

            Thread senderThread = new Thread(Send);
            senderThread.Start();

            Thread listenThread = new Thread(Listen);
            listenThread.Start();
		}

        private static void Send()
        {
            Sender sender = new Sender(process);
            sender.Send();
        }			

        private static void Listen()
        {
            Listener listener = new Listener();
            listener.Listen();
        }
	}
}
