using System;
using System.Collections.Generic;
using Listener.Device.Devices;
using Listener.Device.Interface;

namespace Listener.Utility
{
    public static class Command
    {
        private static bool debug; 
        private static readonly LedStrip LedStrip;

        static Command()
        {
            debug = false;
            LedStrip = new LedStrip();
        }

        public static void ParseAndExecute(IEnumerable<string> command)
        {
            // Command line parsing
            var instruction = new Arguments(command);

            if (instruction.Command == "IOT+CMD")
            {
                try
                {
                    // Look for specific arguments values and display them if they exist (return null if they don't)
                    if (instruction["switch"] != null)
                    {
                        DebugMessage("Switch value: " + instruction["switch"]);
                        var cmd = instruction["switch"];
                        LedStrip.Switch(cmd == "On" ? Status.On : Status.Off);
                    }
                    else DebugMessage("Switch not defined !");

                    if (instruction["dim"] != null)
                    {
                        DebugMessage("Dim value: " + instruction["dim"]);
                        var cmd = instruction["dim"];
                        LedStrip.Dim(Convert.ToSingle(cmd));
                    }
                    else DebugMessage("Dim not defined !");

                    if (instruction["debug"] != null)
                    {
                        debug = true;
                        DebugMessage("Debug value: " + instruction["debug"]);
                    }
                    else
                    {
                        debug = false;
                        DebugMessage("Debug not defined !");
                    }
                    DebugMessage("Arguments parsed.");
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    DebugMessage(exception.Message);
                }
            }
            else DebugMessage("No IoT command.");
        }

        private static void DebugMessage(string msg)
        {
            if (debug) Console.Out.WriteLine(msg);
        }
    }
}
