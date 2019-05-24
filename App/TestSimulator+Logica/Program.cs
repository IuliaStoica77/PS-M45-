using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace TestSimulatorImproved
{
    class Program
    {
        static void Main()
        {
            Simulator.Simulator process = new Simulator.Simulator();
            TCP_PLC.ProcessLogic processLogic = new TCP_PLC.ProcessLogic(process);
            //int i = 0;

            process.Command_Update(Simulator.Command.Start);

            Thread processThread = new Thread(process.Simulate);

            processThread.Start();

            Thread processLogicThread = new Thread(processLogic.Run);

            processLogicThread.Start();

            while (true)
            {
                Console.WriteLine(process.Get_State());
                Console.WriteLine(process.Level);
                Thread.Sleep(500);
            }
            //process.PumpSpeed = 110;
            //process.Command_Update(SimulatorImproved.Command.Pump_1_On);
            //while (i < 10000)
            //{
            //    Console.WriteLine(process.Get_State());
            //    Console.WriteLine(process.level);
            //    i++;
            //    if (i > 20)
            //        process.Command_Update(SimulatorImproved.Command.Pump_2_On);
            //    Thread.Sleep(500);
            //}
        }

        private static void StartProcess()
        {
            throw new NotImplementedException();
        }
    }
}
