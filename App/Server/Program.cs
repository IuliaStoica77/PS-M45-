using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulator;

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
        static Simulator.Simulator process;
        static ProcessLogic logic;

        static void Main()
        {
            Console.WriteLine("Press enter to start...");
            Console.ReadLine();

            process = new Simulator.Simulator();
            logic = new ProcessLogic(process);

            Thread processThread = new Thread(process.Simulate);
            processThread.Start();                  //pornire thread proces

            Thread logicThread = new Thread(logic.Run);
            logicThread.Start();                    //pornire thread logica

            Thread senderThread = new Thread(Send);
            senderThread.Start();                   //pornire thread send

            Thread listenThread = new Thread(Listen);
            listenThread.Start();                   //pornire thread listen
        }

        private static void Send()
        {
            Sender sender = new Sender(process);
            sender.Send();                  //pornire sender
        }

        private static void Listen()
        {
            Listener listener = new Listener(logic);
            listener.Listen();              //pornire listener
        }
    }
}
