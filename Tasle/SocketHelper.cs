using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Tasle
{
    public sealed class SocketHelper
    {
        
        private static object sLock = new object();
        private static SocketHelper sSocketHelper;
        private int mTimeOut = 5000;

        public delegate void ConnectCallback();

        ConnectCallback mConnectCallback = null;
        ConnectCallback mConnectFailedCallback = null;
        private Socket socket;

        public static SocketHelper Instance
        {
            get
            {
                if (sSocketHelper == null)
                {
                    lock (sLock)
                    {
                        if (sSocketHelper == null)
                            sSocketHelper = new SocketHelper();
                    }
                }
                return sSocketHelper;
            }
        }


        //using Tcp as Connect protocol
        public void Connect(string serverIp, int serverPort, 
            ConnectCallback connectCallback, 
            ConnectCallback connectFailedCallback
            )
        {
            mConnectCallback = connectCallback;
            mConnectFailedCallback = connectFailedCallback;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(serverIp);
            IPEndPoint endpoint = new IPEndPoint(address, serverPort);

            IAsyncResult result = socket.BeginConnect(endpoint, new AsyncCallback(ConnectedCallback), socket);

            //timeout detecting
            bool success = result.AsyncWaitHandle.WaitOne(mTimeOut, true);
            if (!success)
            {
                //time out occurs
                mConnectFailedCallback?.Invoke();
            }
            else
            {
                StartReceive();
            }

        }

        private void StartReceive()
        {
            Thread thread = new Thread(new ThreadStart(ReceiveSocket));
            thread.IsBackground = true;
            thread.Start();
        }

        private void ReceiveSocket()
        {

        }

        private void ConnectedCallback(IAsyncResult asyncConnect)
        {
            if (!socket.Connected)
            {
                mConnectFailedCallback?.Invoke();
                return;
            }

            mConnectCallback?.Invoke();
        }
    }
}
