using NATServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NatClient
    {
        public ObservableCollection<string> Messages { get; set; }

        private NatUdpClient _connection;
        private UdpClient _client;
        private string _nickname;

        public NatClient(IPEndPoint endpoint, string nickname)
        {
            Messages = new ObservableCollection<string>();
            _nickname = nickname;
            _connection = new NatUdpClient(endpoint);
            _client = _connection.GetClient();
        }

        public void SendMessage(string message)
        {
            string messageBody = _nickname + " : " + message;
            var buffer = Encoding.UTF8.GetBytes(messageBody);
            _client.Send(buffer, buffer.Length);
        }

        public async Task Receive()
        {
            while(true)
            {
                try
                {
                    var result = await _client.ReceiveAsync();

                    if(result.Buffer.Length > 0)
                    {
                        Messages.Add(Encoding.UTF8.GetString(result.Buffer));
                    }
                }
                catch { }
            }
        }
    }
}
