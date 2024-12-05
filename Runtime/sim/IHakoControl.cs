using System;

namespace hakoniwa.sim
{
    public enum HakoState
    {
        Stopped = 0,
        Runnable = 1,
        Running = 2,
        Stopping = 3,
        Terminated = 99,
    }
    public interface IHakoControl
    {
        long GetWorldTime();
        HakoState GetState();
        bool SimulationStart();
        bool SimulationStop();
        bool SimulationReset();
    }
}
