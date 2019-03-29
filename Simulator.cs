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
        SmallDelay = 2,        
        LongDelay = 8,        
        PumpOne = 32,
        PumpTwo = 64,
        PumpThree = 128
    }

    /// <summary>
    /// Enumerare folosita pentru a modela starea procesului
    /// </summary>
    public enum ProcessState
    {        
        PumpOne = 1,
        PumpTwo = 2,
        PumpThree=4,
        Valve=8,
        Level_1 = 16,
        Level_2 = 32,
        Level_3 = 64,
        Stopped = 128
    }



    public class Simulator
    {
        private byte[] _state = new byte[] { 0x00, 0x00};
        
        private void ExecuteLongDelayCommand(int Delay)
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

        private void ExecuteShortDelayCommand(int Delay, int opt)
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
                        _state[0]=(int)ProcessState.Valve;
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
                        break;
                    }
            }

    }
        

        public void UpdateState(int CommandState)
        {            
            int longDelayForBothPump = (int)Command.LongDelay  | (int)Command.PumpOne    | (int)Command.PumpTwo  | (int)Command.Started;

            int shortDelayForBothPump = (int)Command.SmallDelay    | (int)Command.PumpOne   | (int)Command.PumpTwo  | (int)Command.Stopped;

            if(CommandState == longDelayForBothPump)
            {
                ExecuteLongDelayCommand(5000);
            }
            else if (CommandState == shortDelayForBothPump)
            {
                ExecuteShortDelayCommand(2000);
            }            
        }

        public byte[] GetState()
        {
            return _state;
        }
    }
}
