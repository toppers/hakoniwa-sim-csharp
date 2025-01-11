using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace hakoniwa.sim.core.impl
{
    public static class HakoConductor
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string DllName = "conductor"; // Windows用DLL名
#else
        private const string DllName = "libconductor"; // Ubuntu/Mac用DLL名
#endif

        /*
         * Start the Conductor
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int hako_conductor_start(ulong delta_usec, ulong max_delay_usec);

        public static bool Start(ulong deltaUsec, ulong maxDelayUsec)
        {
            try
            {
                int ret = hako_conductor_start(deltaUsec, maxDelayUsec);
                if (ret != 0) //true
                {
                    return true;//success
                }
                else //false
                {
                    return false;
                }
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError($"DllNotFoundException: {e.Message}");
                return false;
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogError($"EntryPointNotFoundException: {e.Message}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
                return false;
            }
        }

        /*
         * Stop the Conductor
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void hako_conductor_stop();

        public static void Stop()
        {
            try
            {
                hako_conductor_stop();
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError($"DllNotFoundException: {e.Message}");
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogError($"EntryPointNotFoundException: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
            }
        }
    }
}
