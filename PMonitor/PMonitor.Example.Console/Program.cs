using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using PMonitor.Core;

namespace PMonitor.BoehmTrader.Console
{
    class Program
    {
        private static string processLocation = @"C:\Users\tpaul\source\repos\BoehmTester\BoehmTrader\bin\Debug\BoehmTrader.exe";
        private static long numMonitoring = 0;
        /// <summary>
        /// On Windows, interrogate the status of the Notepad process every 3 seconds. You can open and close
        /// Notepad in order to see how the program responds to your changes. We print the output on the console.
        /// </summary>
        static void Main()
        {
            System.Console.WriteLine("PMonitor BoehmTrader - Console");
            IProcessMonitor pm = ProcessMonitorFactory.BuildDefaultOSProcessMonitor();
            while (true)
            {
                pm.RefreshInformation();
                BasicProcessInformation bpi = pm.GetProcessInformation().Single();

                if (numMonitoring % 10 == 0)
                    System.Console.WriteLine("{0} Process {1} is {2}", DateTime.Now.ToString(CultureInfo.InvariantCulture), bpi.FriendlyName, bpi.State.ToString());

                if (bpi.State == ProcessState.NotRunning)
                    StartProcess();

                Thread.Sleep(500);
                numMonitoring++;
            }
        }

        private static void StartProcess()
        {
            Thread.Sleep(500);
            System.Console.WriteLine("BoehmTrader will restart--");
            Process.Start(processLocation);
        }
    }
}
