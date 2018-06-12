using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Listener
{
    using Listener.Utility;
    using Listener.Socket;

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
            SocketListener.Start();
            return 0;
        }
    }
}