using System;
using System.IO;
using System.Text;

// ReSharper disable UnusedMember.Global
namespace Listener.Driver
{
    public class Pwm
    {
        private readonly bool isLinux = Helper.Environment.IsLinux;
        private const string FifoName = "/dev/pi-blaster";
        private readonly StreamWriter pin;

        /// <summary>
        /// Pin to GPIO mapping on raspberry pi header P1 
        /// </summary>
        public enum HeaderP1
        {
            Pin7  = 4, 
            Pin11 = 17,
            Pin12 = 18,
            Pin13 = 21,
            Pin15 = 22,
            Pin16 = 23,
            Pin18 = 24,
            Pin22 = 25       
        }
        
        /// <summary>
        /// PWM (pulse width modulation) constructor
        /// </summary>
        public Pwm()
        {
            if (!isLinux) return;
            var file = new FileInfo(FifoName).OpenWrite();
            pin = new StreamWriter(file, Encoding.ASCII);
        }

        /// <summary>
        /// Set a pulse width 
        /// </summary>
        /// <param name="channel">Take this channel (pin) and set a pwm value</param>
        /// <param name="value">Pwm rate from 0 - 100 percent</param>
        public void Set(HeaderP1 channel, double value)
        {
            if ((value < 0) || (value > 100))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 - 100 percent.");

            var s = (int)channel + "=" + value + "\n";
            Console.WriteLine(s);

            if (!isLinux) return;
            pin.Write(s);
            pin.Flush();
        }
    }
}
