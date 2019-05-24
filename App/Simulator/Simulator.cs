using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Simulator
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

    public enum ProcessState
    {
        Pump_1 = 1,
        Pump_2 = 2,
        Pump_3 = 4,
        Valve = 8,
        Level_1 = 16,
        Level_2 = 32,
        Level_3 = 64,
        Stopped = 128
    }

    public class Simulator
    {
        private int fillSpeed;
        private readonly static int pumpSpeed = 100;
        private byte state;
        private int level;
        private int pastLevel;
        private readonly int capacity = 10000;
        private BitArray stateArray;
        private byte[] intermediateState;

        public Simulator()
        {
            this.FillSpeed = 0;
            this.pastLevel = 0;
            this.state = new byte();
        }

        public int FillSpeed { get => fillSpeed; set => fillSpeed = value; }
        public int Level { get => level; set {
                if (value > 0)
                    level = value;
                if (value < 10000)
                    level = value;
                if (value < 0)
                    level = 0;
                if (value > 10000)
                    level = 10000;
            } }

        public void Simulate()
        {
            while (true)
            {
                stateArray = new BitArray(new byte[] { state, 0x00 });
                if (stateArray[7] == false)
                {
                    if (stateArray[3] == true)    //daca valva e deschisa
                        Level += FillSpeed;

                    if (stateArray[0] == true)    //daca pompa 1 este functionala/pornita
                        Level -= pumpSpeed;

                    if (stateArray[1] == true)    // daca pompa 2 este functionala/pornita
                        Level -= pumpSpeed;

                    if (stateArray[2] == true)    // daca pompa 3 este functionala/pornita
                        Level -= pumpSpeed;

                    float percentage = Level / (float)capacity;       // valoare preocentuala a nivelului de apa din tanc actuala
                    float pastPercentage = pastLevel / (float)capacity;         // valoare preocentuala a nivelului de apa din tanc precedenta

                    if (percentage >= 0.9)          // daca procentaj mai mare decat niv 3 = 0.9
                        stateArray[6] = true;

                    if (percentage >= 0.65)          // daca procentaj mai mare decat niv 2 = 0.65 dar mai mica ca niv 3
                        if (percentage < 0.9)
                        {
                            if (pastPercentage >= 0.9)          // daca inainte era setat si nivelul 3 il dezactivam
                                stateArray[6] = false;
                            stateArray[5] = true;           // activam niv 2
                        }

                    if (percentage >= 0.15)             // daca procentaj mai mare decat niv 1 = 0.15 dar mai mica ca niv 2
                        if (percentage < 0.65)
                        {
                            if (pastPercentage >= 0.65)             // daca inainte era setat si nivelul 2 il dezactivam
                                stateArray[5] = false;
                            stateArray[4] = true;           //activam niv 1
                        }

                    if (percentage < 0.15)              // daca procentaj mai mic ca niv 1
                        if (pastPercentage >= 0.15)             // daca inainte a fost mai mare ca niv 1 il dezactivam
                            stateArray[4] = false;

                    intermediateState = new byte[] { 0x00, 0x00 };

                    stateArray.CopyTo(intermediateState, 0);

                    state = intermediateState[0];           // se salveaza starea noua a procesului
                    pastLevel = Level;          // se salveaza valoarea curenta a nivelului apei pentru urmatoarea iteratie

                    System.Threading.Thread.Sleep(250);
                }
                else
                    System.Threading.Thread.Sleep(10000);
            }
        }

        public void Command_Update(Command command)
        {
            switch (command)
            {
                case Command.Pump_1_On:
                    state = Convert.ToByte((int)state | (int)ProcessState.Pump_1);
                    break;
                case Command.Pump_2_On:
                    state = Convert.ToByte((int)state | (int)ProcessState.Pump_2);
                    break;
                case Command.Pump_3_On:
                    state = Convert.ToByte((int)state | (int)ProcessState.Pump_3);
                    break;
                case Command.ValveOff:
                    state = Convert.ToByte((int)state & 247);
                    break;
                case Command.PumpOneOff:
                    state = Convert.ToByte((int)state & 254);
                    break;
                case Command.PumpTwoOff:
                    state = Convert.ToByte((int)state & 253);
                    break;
                case Command.Start:
                    state = (byte)ProcessState.Valve;
                    break;
                case Command.Stop:
                    state = (byte)ProcessState.Stopped;
                    Level = 0;
                    break;
                case (Command)251:          //comanda dezactivare pompa 3 
                    state = Convert.ToByte((int)state & 251);
                    break;
            }
        }

        public byte Get_State()
        {
            return state;
        }
    }
}
