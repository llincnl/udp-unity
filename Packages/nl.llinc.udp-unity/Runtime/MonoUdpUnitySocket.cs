using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UdpUnity {

    /// <summary>
    /// The MonoUdpUnitySocket class provides UDP socket capabilities using System.Net.
    /// It implements the IUdpUnitySocket interface.
    /// </summary>
    public class MonoUdpUnitySocket : IUdpUnitySocket {

        /// <summary>
        /// An event that is invoked when a message is received.
        /// </summary>
        public event OnMessageUdp OnMessage;
        
        private int _serverPort;
        private UdpClient _client;
        private Thread _thread;
        private bool _isRunning;
        
        /// <summary>
        /// Creates an instance of the MonoUdpUnitySocket class with the specified server port.
        /// </summary>
        /// <param name="serverPort">The port to use for the server. Default value is 8080.</param>
        public MonoUdpUnitySocket(int serverPort = 8080) {
            _serverPort = serverPort;
        }

        /// <summary>
        /// Opens the socket and starts the listening thread.
        /// </summary>
        public void Open() {
            var random = new System.Random();
            int port = random.Next(1024, 65535);
            _client = new UdpClient(port);
            _client.MulticastLoopback = false;

            _thread = new Thread(ClientReceiver);
            _thread.IsBackground = true;
            _thread.Start();
            _isRunning = true;
        }

        /// <summary>
        /// Closes the socket and stops the listening thread.
        /// </summary>
        public void Close() {
            _client.Close();
            _isRunning = false;
        }

        /// <summary>
        /// Broadcasts a message.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        public void Broadcast(string message) {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("255.255.255.255"), _serverPort);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            _client.Send(bytes, bytes.Length, ip);
        }
        
        /// <summary>
        /// A method to handle incoming messages in a separate thread.
        /// </summary>
        private void ClientReceiver() {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (_isRunning) {
                try {
                    var receiveBytes = _client.Receive(ref remoteEndPoint);
                    var returnData = Encoding.ASCII.GetString(receiveBytes);
                    if (returnData != null) {
                        OnMessage?.Invoke(remoteEndPoint, returnData);
                    }
                } catch (SocketException e) {
                    if (e.ErrorCode != 10004) {
                        Debug.Log("We got an exception " + e.Message);
                    }
                } catch (Exception e) {
                    Debug.Log("We got an exception " + e.Message);
                }
                Thread.Sleep(1000);
            }
        }
    }
}