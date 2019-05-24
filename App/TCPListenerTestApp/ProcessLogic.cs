using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Timers;
using SimulatorImproved;

namespace TCP_PLC
{
    public class ProcessLogic
    {
        private readonly Simulator process;
        private BitArray stateArray;
        private BitArray previousState;
        private readonly Timer threeSecRule;

        public ProcessLogic(Simulator process)
        {
            this.process = process;
            this.previousState = new BitArray(new byte[] { process.Get_State(), 0x00 });
            this.stateArray = new BitArray(new byte[] { process.Get_State(), 0x00 });
            this.threeSecRule = new Timer(3000)
            {
                AutoReset = false
            };
            this.threeSecRule.Elapsed += ThreeSecRuleCommand;
        }

        private void ThreeSecRuleCommand(object sender, ElapsedEventArgs e)
        {
            if(stateArray[6] == false)
                process.Command_Update(Command.Pump_2_On);
        }

        public void Run()
        {
            while (true)
            {
                stateArray = new BitArray(new byte[] { process.Get_State(), 0x00 });
                if (stateArray[7] == false)
                {
                    if(stateArray[4] == true)
                    {
                        if (previousState[5] == true)
                        {
                            process.Command_Update(Command.PumpOneOff);
                            if (stateArray[1] == true)
                                process.Command_Update(Command.PumpTwoOff);
                            if (stateArray[2] == true)
                                process.Command_Update((Command)251);
                        }
                    }
                    if(stateArray[5] == true)
                    {
                        if(previousState[4] == true)
                        {
                            process.Command_Update(Command.Pump_1_On);
                            threeSecRule.Enabled = true;
                        }
                    }
                    if(stateArray[6] == true)
                    {
                        process.Command_Update(Command.ValveOff);
                    }
                }
                previousState = stateArray;
            }
        }
    }
}
