using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth;
using DollyProtocol;

namespace BluetoothConnectionManager
{
    /// <summary>
    /// Class to control the bluetooth connection to the Arduino.
    /// </summary>
    public class ConnectionManager
    {
        /// <summary>
        /// Socket used to communicate with Arduino.
        /// </summary>
        private StreamSocket socket;

        /// <summary>
        /// DataWriter used to send commands easily.
        /// </summary>
        private DataWriter dataWriter;

        /// <summary>
        /// DataReader used to receive messages easily.
        /// </summary>
        private DataReader dataReader;

        /// <summary>
        /// Delegate used by event handler.
        /// </summary>
        /// <param name="message">The message received.</param>
        public delegate void MessageReceivedHandler(string message);

        /// <summary>
        /// Event fired when a new message is received from Arduino.
        /// </summary>
        public event MessageReceivedHandler MessageReceived;

        /// <summary>
        /// Initialize the manager, should be called in OnNavigatedTo of main page.
        /// </summary>
        public void Initialize()
        {
            socket = new StreamSocket();
        }

        /// <summary>
        /// Finalize the connection manager, should be called in OnNavigatedFrom of main page.
        /// </summary>
        public void Terminate()
        {
            if (socket != null)
            {
                socket.Dispose();
            }
        }

        /// <summary>
        /// Connect to the given host device.
        /// </summary>
        /// <param name="deviceHostName">The host device name.</param>
        public async Task<bool> Connect(HostName deviceHostName)
        {

            if (socket != null)
            {
                try
                {
                    Debug.WriteLine("Trying to connect to BL device...");
                    await socket.ConnectAsync(deviceHostName, "1");
                    dataReader = new DataReader(socket.InputStream);
                    dataWriter = new DataWriter(socket.OutputStream);
                    await Task.Factory.StartNew(() =>
                    {
                        ReceiveMessages();
                    });
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("...Fail!!!");
                    // If this is an unknown status, 
                    // it means that the error is fatal and retry will likely fail.
                    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    {
                        Debug.WriteLine("...BADLY!!!");
                        //throw;
                        return false;
                    }

                    //BtStatusText.Text = "Failed: " + exception.Message;
                    Debug.WriteLine("Exception msg = " +exception.Message);
                    // Could retry the connection, but for this simple example
                    // just close the socket.

                    //closing = true;
                    // the Close method is mapped to the C# Dispose
                    socket.Dispose();
                    socket = null;

                }
            }
            else
            {
                Debug.WriteLine("...CANNOT!!!");
            }
            return true;
        }

        /// <summary>
        /// Receive messages from the Arduino through bluetooth.
        /// </summary>
        private async void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    // Search and read sync byte  
                    uint syncFieldCount = await dataReader.LoadAsync(1);
                    if (syncFieldCount != 1)
                    {
                        Debug.WriteLine("Sync byte not found!");
                        return;
                    }
                    uint syncByte = dataReader.ReadByte();
                    if (syncByte != Protocol.SyncByte)
                    {
                        Debug.WriteLine("Sync byte not found!");
                        return;
                    }

                    // Read length byte  
                    uint sizeFieldCount = await dataReader.LoadAsync(1);
                    if (sizeFieldCount != 1)
                    {
                        Debug.WriteLine("Length byte not found!");
                        return;
                    }

                    uint messageLength = dataReader.ReadByte();
                    if (messageLength < 0x01)
                    {
                        Debug.WriteLine("Invalid Message: Length should be >= 1 !, now it's only = " + messageLength.ToString());
                        return;
                    }
                    // Read the message 
                    uint actualMessageLength = await dataReader.LoadAsync(messageLength);
                    if (messageLength != actualMessageLength)
                    {
                        Debug.WriteLine("Message read failed: Not enough bytes read!");
                        return;
                    }
                    // Process the message
                    string message = dataReader.ReadString(actualMessageLength);
                    MessageReceived(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Send command to the Arduino through bluetooth.
        /// </summary>
        /// <param name="command">The sent command.</param>
        /// <returns>The number of bytes sent</returns>
        public async Task<uint> SendCommand(string command)
        {
            uint sentCommandSize = 0;
            if (dataWriter != null)
            {
                uint commandSize = dataWriter.MeasureString(command);
                dataWriter.WriteByte((byte)Protocol.SyncByte);
                dataWriter.WriteByte((byte)commandSize);
                sentCommandSize = dataWriter.WriteString(command);
                Debug.WriteLine("SEND: len = " + commandSize + ", msg = " + command);
                await dataWriter.StoreAsync();
            }
            return sentCommandSize;
        }
    }
}
