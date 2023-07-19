using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net;
using System.Text;

#if ENABLE_WINMD_SUPPORT
using Windows;
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
#endif

namespace UdpUnity {
#if ENABLE_WINMD_SUPPORT

    /// <summary>
    /// UwpUdpUnitySocket provides an implementation of IUdpUnitySocket for UWP.
    /// This class uses the Windows.Networking.Sockets.DatagramSocket for networking.
    /// </summary>
    public class UwpUdpUnitySocket : IUdpUnitySocket {

        /// <summary>
        /// An event that is invoked when a message is received.
        /// </summary>
        public event OnMessageUdpSocket OnMessage;

        private int _serverPort;
        private Windows.Networking.Sockets.DatagramSocket _clientDatagramSocket;

        /// <summary>
        /// Creates a new instance of the UwpUdpUnitySocket with the given server port.
        /// </summary>
        public UwpUdpUnitySocket(int serverPort = 8080) {
            _serverPort = serverPort;
        }

        /// <summary>
        /// Opens the UDP socket and starts listening for messages.
        /// </summary>
        public void Open() {
            StartClient();
        }
        
        /// <summary>
        /// Closes the UDP socket and stops listening for messages.
        /// </summary>
        public void Close() {
            _clientDatagramSocket.MessageReceived -= ClientDatagramSocketMessageReceived;
            _clientDatagramSocket.Dispose();
        }
        
        /// <summary>
        /// Broadcasts a message.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        public void Broadcast(string message) {
            BroadcastAsync(message);
        }

        /// <summary>
        /// Asynchronously starts the UDP client.
        /// </summary>
        public async void StartClient() {
            try {
                _clientDatagramSocket = new Windows.Networking.Sockets.DatagramSocket();
                _clientDatagramSocket.MessageReceived += ClientDatagramSocketMessageReceived;
                await _clientDatagramSocket.BindServiceNameAsync(0.ToString());
            } catch (Exception ex) {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.Log("Error: "+ webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }
        
        /// <summary>
        /// Handles the MessageReceived event of the DatagramSocket.
        /// </summary>
        private async void ClientDatagramSocketMessageReceived(Windows.Networking.Sockets.DatagramSocket sender, Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args) {
           string response;
           using (DataReader dataReader = args.GetDataReader()) {
               response = dataReader.ReadString(dataReader.UnconsumedBufferLength).Trim();
               if (response.Length > 0) {
                   IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                   OnMessage?.Invoke(remoteEndPoint, response);
               }
           }
           sender.Dispose();
        }
    
        /// <summary>
        /// Asynchronously broadcasts a message to all clients.
        /// </summary>
        private async void BroadcastAsync(string message) {
           try {
               DataWriter writer = null;
               var ip = IPAddress.Parse("255.255.255.255");
               var hostName = new Windows.Networking.HostName(ip.ToString());
               writer = new DataWriter(await _clientDatagramSocket.GetOutputStreamAsync(hostName, _serverPort.ToString()));
               writer.WriteBytes(Encoding.ASCII.GetBytes(message));
               await writer.StoreAsync();
           } catch (Exception ex) {
               Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
               Debug.Log("Error: "+webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
           }
        }
    }
#endif
}
