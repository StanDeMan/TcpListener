using System;

namespace Listener
{
    // internal namespace
    using Socket;

    // State object for reading client data asynchronously
    public class StateObject
    {
         /// <summary>
        /// Listener main programme.
        /// Everything starts here.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(String[] args)
        {
            Work.Start();
            return 0;
        }
    }
}