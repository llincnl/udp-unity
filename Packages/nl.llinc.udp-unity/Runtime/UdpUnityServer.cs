using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UdpUnity {
    
    /// <summary>
    /// The UdpUnityServer class.
    /// It implements the IUdpUnityServer interface.
    /// </summary>
    public class UdpUnityServer : IUdpUnityServer {

        /// <summary>
        /// An event that is invoked when a message is received.
        /// </summary>
        public event OnMessageUdp OnMessage;
        
        private int _port;
        private UdpClient _server;
        private Thread _thread;
        private bool _isRunning;
        
        /// <summary>
        /// Creates an instance of the UdpUnityServer class with the specified port.
        /// </summary>
        /// <param name="port">The port to use for the server. Default value is 8080.</param>
        public UdpUnityServer(int port = 8080) {
            _port = port;
        }

        /// <summary>
        /// Opens the UDP server socket.
        /// </summary>
        public void Open() {
            _server = new UdpClient(_port);
            _server.MulticastLoopback = false;
            _server.EnableBroadcast = true;
            
            _thread = new Thread(ServerReceiver);
            _thread.IsBackground = true;
            _thread.Start();
            _isRunning = true;
        }

        /// <summary>
        /// Closes the UDP server socket.
        /// </summary>
        public void Close() {
            if (_isRunning) {
                _isRunning = false;
                _server.Close();
            }
        }

        /// <summary>
        /// Sends a message to the specified remote endpoint.
        /// </summary>
        /// <param name="remoteEndPoint">The IPEndPoint instance representing the remote endpoint.</param>
        /// <param name="message">The message to send.</param>
        public void Send(IPEndPoint remoteEndPoint, string message) {
            Byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            _server.Send(messageBytes, messageBytes.Length, remoteEndPoint);
        }
        
        /// <summary>
        /// Handles incoming messages in a separate thread.
        /// </summary>
        public void ServerReceiver() {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (_isRunning) {
                try {
                    Byte[] receiveBytes = _server.Receive(ref remoteEndPoint);
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    if (returnData != null && returnData.Length > 0) {
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
            _server.Dispose();
        }
    }
}

