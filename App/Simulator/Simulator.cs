using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{

    /// <summary>
    /// Enumerare folosita pentru a transmite/receptiona comenzi
    /// </summary>
    public enum Command
    {
        Stopped = 0,
        Started = 1,
        Case_1 = 2,
        Case_2 = 4,
        Case_3 = 8,
        Case_4 = 16
    }

    /// <summary>
    /// Enumerare folosita pentru a modela starea procesului
    /// </summary>
    public enum ProcessState
    {        
        PumpOne = 1,
        PumpTwo = 2,
        PumpThree=4,
        Valve = 8,
        Level_1 = 16,
        Level_2 = 32,
        Level_3 = 64,
        Stopped = 128
    }



    public class Simulator
    {
        private byte[] _state = new byte[] { 0x00, 0x00};
        
        private void Start(int Delay)
        {
            System.Threading.Thread.Sleep(Delay);
            _state[0] = (int)ProcessState.Level_1
            | (int)ProcessState.Valve;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.Valve;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.PumpOne
                | (int)ProcessState.Valve;
            System.Threading.Thread.Sleep(Delay);
        }

        private void Stop(int Delay, int opt)
        {
            switch (opt)
            {
                case 1:
                {
                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Level_1
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0]= (int)ProcessState.Stopped
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    break;
                }
                case 2:
                {
                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Level_1
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.Stopped
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    break;
                }
                case 3:
                {
                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpThree
                | (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpThree
                | (int)ProcessState.Level_1
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpThree
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.Stopped
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                        break;
                }
                case 4:
                {
                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.Level_3
                | (int)ProcessState.Valve;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.PumpTwo
                | (int)ProcessState.Level_1
                | (int)ProcessState.Level_2
                | (int)ProcessState.Level_3;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.PumpTwo
                | (int)ProcessState.Level_1
                | (int)ProcessState.Level_2;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.PumpTwo
                | (int)ProcessState.Level_1;      
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.PumpTwo;
                    System.Threading.Thread.Sleep(Delay);

                    _state[0] = (int)ProcessState.Stopped;
                    System.Threading.Thread.Sleep(Delay);

                    break;
                }
                default:
                {
                    _state[0] = (int)ProcessState.Stopped;
                    System.Threading.Thread.Sleep(Delay);

                    break;
                }
            }

        }  

        public void UpdateState(int CommandState)
        {            
            if(CommandState == (int)Command.Started)
            {
                Start(5000);
            }
            else
            {
                switch(CommandState)
                {
                    case ((int)Command.Stopped | (int)Command.Case_1):
                        Stop(5000, 1);
                        break;
                    case ((int)Command.Stopped | (int)Command.Case_2):
                        Stop(5000, 2);
                        break;
                    case ((int)Command.Stopped | (int)Command.Case_3):
                        Stop(5000, 3);
                        break;
                    case ((int)Command.Stopped | (int)Command.Case_4):
                        Stop(5000, 4);
                        break;
                }

            }            
        }

        public byte[] GetState()
        {
            return _state;
        }
    }
}
