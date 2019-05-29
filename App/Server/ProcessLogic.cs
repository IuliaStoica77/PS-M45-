using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Timers;
using Simulator;


namespace TCP_PLC
{
    public class ProcessLogic
    {
        private readonly Simulator.Simulator process;               //procesul de condus
        private BitArray state;                     //starea actualaprocesului
        private BitArray previousState;                 //starea anterioara a procesului
        private readonly Timer threeSecRule;                    //timer pentru 3 secunde
        private bool[] functioningPumps;                    //vector bool pentru a stii daca este stricata vreuna din pompe

        public Simulator.Simulator Process => process;

        public ProcessLogic(Simulator.Simulator process)
        {
            this.process = process;
            this.previousState = new BitArray(new byte[] { process.Get_State(), 0x00 });
            this.threeSecRule = new Timer(3000)
            {
                AutoReset = false
            };
            this.threeSecRule.Elapsed += ThreeSecRuleCommand;
            this.functioningPumps = new bool[] { true, true };
        }

        private void ThreeSecRuleCommand(object sender, ElapsedEventArgs e)                 //functie pentru cazul 3 sec elapsed
        {
            if (state[6] == false & state[5] == true)                   //daca nivelul 3 nu este activ si nivelul 2 este inca activ
            {
                if (state[0] == true & Pump_Function(ProcessState.Pump_2))                  //daca pompa 1 pornita si pompa doi e functionala
                    process.Command_Update(Command.Pump_2_On);
                else
                    if(state[1] == true & Pump_Function(ProcessState.Pump_1))                   //daca pompa 2 pornita si pompa unu e functionala
                    process.Command_Update(Command.Pump_1_On);
                    else
                    process.Command_Update(Command.Pump_3_On);                  //daca pompa 1 merge si pompa doi e functionala
                this.threeSecRule.Enabled = false;                  //dezactivam timer
            }
        }

        private bool Pump_Function(ProcessState pump)                   //returneaza daca pump este functionala sau nu
        {
            switch (pump)
            {
                case ProcessState.Pump_1:
                    if (functioningPumps[0] == true)
                        return true;
                    break;
                case ProcessState.Pump_2:
                    if (functioningPumps[1] == true)
                        return true;
                    break;
            }
            return false;
        }

        public void Run()
        {
            while (true)
            {
                state = new BitArray(new byte[] { process.Get_State(), 0x00 });                 //preluare date de la proces

                if (state[7] == false)                  //daca procesul nu e oprit
                {
                    if (state[4] == false & state[5] == false & state[6] == false)                  //daca s-a coborat sub nivelul 1 se opresc pompepe pornitr
                    {
                        if(state[0] == true)
                            process.Command_Update(Command.PumpOneOff);

                        if (state[1] == true)
                            process.Command_Update(Command.PumpTwoOff);

                        if (state[2] == true)
                            process.Command_Update((Command)251);
                    }

                    if (state[4] == true & state[5] == true & state[6] == false)                    //daca s-a atins nivelul 2
                    {
                        if (previousState[4] == true)                   //daca inainte a fost atins nivelul 1
                        {
                            if (Pump_Function(ProcessState.Pump_1) == true)                 //daca pompa 1 functionala se porneste
                                process.Command_Update(Command.Pump_1_On);
                            else
                                if (Pump_Function(ProcessState.Pump_2) == true)                 //daca pompa 2 functionala se porneste
                                process.Command_Update(Command.Pump_2_On);

                            threeSecRule.Enabled = true;                    //se reactiveaza timer
                        }
                    }

                    if (state[4] == true & state[5] == true & state[6] == true)                 //daca se atinge nivelul 3 se opreste valva
                    {
                        process.Command_Update(Command.ValveOff);
                    }

                    previousState = state;              //se reactualizeaza starea anterioara
                }
                else
                {
                    state = new BitArray(new byte[] { 0x80, 0x00 });                //sistemul este oprit
                }
            }
        }

        internal void Filling_Speed(byte fillingSpeed)              //functie pentru schimbarea debitului de umplere
        {
            process.FillSpeed = (int)fillingSpeed;
        }

        public void Process_Request(Command command)                    //functie pentru intermedierea comenzilor venite de la app de monitorizare
        {
            switch(command)
            {
                case Command.Start:
                    process.Command_Update(Command.Start);
                    break;
                case Command.Stop:
                    process.Command_Update(Command.Stop);
                    functioningPumps = new bool[] { true, true};
                    break;
                case Command.PumpOneOff:
                    process.Command_Update(Command.PumpOneOff);
                    functioningPumps[0] = false;
                    break;
                case Command.PumpTwoOff:
                    process.Command_Update(Command.PumpTwoOff);
                    functioningPumps[1] = false;
                    break;
            }
        }
    }
}
