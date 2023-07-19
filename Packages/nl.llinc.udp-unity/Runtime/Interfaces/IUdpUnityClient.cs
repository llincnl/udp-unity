namespace UdpUnity {
    
    /// <summary>
    /// Interface defining the operations for a UDP client.
    /// </summary>
    public interface IUdpUnityClient {
        
        /// <summary>
        /// Event triggered when a message is received.
        /// </summary>
        public event OnMessageUdp OnMessage;
        
        /// <summary>
        /// Opens the UDP client and starts listening for incoming messages.
        /// </summary>
        public void Open();
        
        /// <summary>
        /// Closes the UDP client and stops listening for incoming messages.
        /// </summary>
        public void Close();
        
        /// <summary>
        /// Broadcasts a message.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        public void Broadcast(string message);
    }
}