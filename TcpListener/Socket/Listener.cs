using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Listener.Utility;

namespace Listener.Socket
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public System.Net.Sockets.Socket WorkSocket;
        // Size of receive buffer.
        public const int BufferSize = 4096;
        // Receive buffer.
        public byte[] Buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder Sb = new StringBuilder();
    }

    public class Work
    {
        // Thread signal.
        public static ManualResetEvent AllDone = new ManualResetEvent(false);

        public static void Start()
        {
            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // We use IP4 because ESP8266 modules don't support IP6
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork) ?? 
                                                            new IPAddress(new byte[] { 127, 0, 0, 1 }); // if no IP 
            var localEndPoint = new IPEndPoint(ipAddress, 7777);

            // Create a TCP/IP socket.
            var listener = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    AllDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(AcceptCallback, listener);

                    // Wait until a connection is made before continuing.
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            AllDone.Set();

            // Get the socket that handles the client request.
            var listener = (System.Net.Sockets.Socket)ar.AsyncState;
            var handler = listener.EndAccept(ar);

            // Create the state object.
            var state = new StateObject { WorkSocket = handler };
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            var state = (StateObject)ar.AsyncState;
            var handler = state.WorkSocket;

            // Read data from the client socket. 
            var bytesRead = handler.EndReceive(ar);

            if (bytesRead <= 0) return;
            // There  might be more data, so store the data received so far.
            state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

            // Check for end-of-line tag <cr><lf>. If it is not there, read more data.
            var content = state.Sb.ToString();
            var newlinePos = GetIndex(content, "\r") ?? GetIndex(content, "\n");

            if (newlinePos.HasValue)
            {
                // All the data has been read from the 
                // client. Display it on the console.
                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                // Echo the data back to the client.
                Send(handler, content);
            }
            else
            {
                // Not all data received. Get more.
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
        }

        private static void Send(System.Net.Sockets.Socket handler, String data)
        {
            // Execute the command - arguments are separated by a space
            Task.Factory.StartNew(() => Command.ParseAndExecute(data.Split(' ')));

            // Convert the string data to byte data using ASCII encoding.
            var byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, (SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var handler = (System.Net.Sockets.Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                var bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static int? GetIndex(string text, string substr)
        {
            var index = text.IndexOf(substr, StringComparison.Ordinal);
            return index >= 0 ? (int?)index : null;
        }
    }
}
