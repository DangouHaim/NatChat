using NATServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NatServer
    {
        private static object _instance = null;
        private static object _lock = new object();
        private NatUdpListener _listener;
        private UdpClient _server;
        private List<IPEndPoint> _clients;

        public NatServer()
        {
            _listener = new NatUdpListener();
            _server = _listener.GetServer();
            _clients = new List<IPEndPoint>();
        }

        public IPEndPoint GetEndPoint()
        {
            return _listener.GetEndPoint();
        }

        public async Task Start()
        {
            while(true)
            {
                try
                {
                    var result = await _server.ReceiveAsync();

                    if(result.Buffer.Length > 0)
                    {
                        if(!_clients.Contains(result.RemoteEndPoint))
                        {
                            _clients.Add(result.RemoteEndPoint);
                        }

                        foreach(var v in _clients)
                        {
                            _server.Send(result.Buffer, result.Buffer.Length, v);
                        }
                    }
                }
                catch { }
            }
        }

        public static NatServer GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new NatServer();
                    }
                }
            }
            return _instance as NatServer;
        }
    }
}
