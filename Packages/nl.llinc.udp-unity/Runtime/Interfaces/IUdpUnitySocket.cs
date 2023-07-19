using System.Net;

namespace UdpUnity {
    
    /// <summary>
    /// Delegate for handling incoming UDP messages.
    /// </summary>
    /// <param name="remoteEndPoint">The remote end point sending the message.</param>
    /// <param name="message">The message received.</param>
    public delegate void OnMessageUdp(IPEndPoint endPoint, string message);

    /// <summary>
    /// Interface defining the operations for a UDP socket.
    /// </summary>
    public interface IUdpUnitySocket {
        
        /// <summary>
        /// Event triggered when a message is received.
        /// </summary>
        public event OnMessageUdp OnMessage;
        
        /// <summary>
        /// Opens the UDP socket and starts listening for incoming messages.
        /// </summary>
        public void Open();
        
        /// <summary>
        /// Closes the UDP socket and stops listening for incoming messages.
        /// </summary>
        public void Close();
        
        /// <summary>
        /// Broadcasts a message.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        public void Broadcast(string message);
    }
}