using System.Net;

namespace UdpUnity {

    /// <summary>
    /// The Cross Platform UdpUnityClient class.
    /// It implements the IUdpUnityClient interface.
    /// </summary>
    public class UpdUnityClient : IUdpUnityClient {
        
        /// <summary>
        /// An event that is invoked when a message is received.
        /// </summary>
        public event OnMessageUdp OnMessage;

        private int _serverPort;
        private IUdpUnitySocket _socket;

        /// <summary>
        /// Creates an instance of the UdpUnityClient class with the specified server port.
        /// </summary>
        /// <param name="serverPort">The port to use for the server. Default value is 8080.</param>
        public UpdUnityClient(int serverPort = 8080) {
            _serverPort = serverPort;
        }

        /// <summary>
        /// Opens the UDP client socket.
        /// </summary>
        public void Open() {
            #if ENABLE_WINMD_SUPPORT 
                _socket = new UwpUdpUnitySocket(_serverPort);
            #else
                _socket = new MonoUdpUnitySocket(_serverPort);
            #endif
            _socket.OnMessage += HandleOnUdpMessageReceived;
            _socket.Open();
        }

        /// <summary>
        /// Closes the UDP client socket.
        /// </summary>
        public void Close() {
            if (_socket != null) {
                _socket.OnMessage -= HandleOnUdpMessageReceived;
                _socket.Close();
            }
        }

        /// <summary>
        /// Broadcasts a message.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        public void Broadcast(string message) {
            _socket.Broadcast(message);
        }

        /// <summary>
        /// Handles the OnMessage event of the UDP socket.
        /// </summary>
        /// <param name="remoteEndPoint">The IPEndPoint instance representing the remote endpoint.</param>
        /// <param name="message">The received message.</param>
        private void HandleOnUdpMessageReceived(IPEndPoint remoteEndPoint, string message) {
            OnMessage?.Invoke(remoteEndPoint, message);
        }
    }
}