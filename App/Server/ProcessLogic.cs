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
        private readonly Simulator.Simulator process;
        private BitArray stateArray;
        private BitArray previousState;
        private readonly Timer threeSecRule;
        private bool[] functioningPumps;

        public ProcessLogic(Simulator.Simulator process)
        {
            this.process = process;
            this.previousState = new BitArray(new byte[] { process.Get_State(), 0x00 });
            this.stateArray = new BitArray(new byte[] { process.Get_State(), 0x00 });
            this.threeSecRule = new Timer(3000)
            {
                AutoReset = false
            };
            this.threeSecRule.Elapsed += ThreeSecRuleCommand;
            this.functioningPumps = new bool[] { true, true };
        }

        private void ThreeSecRuleCommand(object sender, ElapsedEventArgs e)
        {
            if (stateArray[6] == false & stateArray[5] == true)
            {
                if (stateArray[0] == true & Pump_Function(ProcessState.Pump_2))
                    process.Command_Update(Command.Pump_2_On);
                else
                    if(stateArray[1] == true & Pump_Function(ProcessState.Pump_1))
                        process.Command_Update(Command.Pump_2_On);
                    else
                    process.Command_Update(Command.Pump_3_On);
                this.threeSecRule.Enabled = false;
            }
        }

        public void Run()
        {

            while (true)
            {
                stateArray = new BitArray(new byte[] { process.Get_State(), 0x00 });
                if (stateArray[7] == false)
                {
                    if (stateArray[4] == false & stateArray[5] == false & stateArray[6] == false)
                    {
                        process.Command_Update(Command.PumpOneOff);
                        if (stateArray[1] == true)
                            process.Command_Update(Command.PumpTwoOff);
                        if (stateArray[2] == true)
                            process.Command_Update((Command)251);
                    }
                    if (stateArray[4] == true & stateArray[5] == true & stateArray[6] == false)
                    {
                        if (previousState[4] == true)
                        {
                            if(Pump_Function(ProcessState.Pump_1) == true)
                                process.Command_Update(Command.Pump_1_On);
                            else
                                if (Pump_Function(ProcessState.Pump_2) == true)
                                    process.Command_Update(Command.Pump_2_On);
                            if (stateArray[2] == false)
                                threeSecRule.Enabled = true;
                        }
                    }
                    if (stateArray[4] == true & stateArray[5] == true & stateArray[6] == true)
                    {
                        process.Command_Update(Command.ValveOff);
                    }
                    previousState = stateArray;
                }
            }
        }

        private bool Pump_Function(ProcessState pump)
        {
            switch(pump)
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

        internal void Filling_Speed(byte fillingSpeed)
        {
            process.FillSpeed = (int)fillingSpeed;
        }

        public void Process_Request(Command command)
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
