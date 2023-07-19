using System.Net;

namespace UdpUnity {
    
    /// <summary>
    /// Interface defining the operations for a UDP server.
    /// </summary>
    public interface IUdpUnityServer {

        /// <summary>
        /// Event triggered when a message is received.
        /// </summary>
        public event OnMessageUdp OnMessage;
        
        /// <summary>
        /// Opens the UDP server and starts listening for incoming messages.
        /// </summary>
        public void Open();
        
        /// <summary>
        /// Closes the UDP server and stops listening for incoming messages.
        /// </summary>
        public void Close();
        
        /// <summary>
        /// Sends a message to a specific client.
        /// </summary>
        /// <param name="remoteEndPoint">The end point of the recipient client.</param>
        /// <param name="message">The message to send.</param>
        public void Send(IPEndPoint remoteEndPoint, string message);
    }
}