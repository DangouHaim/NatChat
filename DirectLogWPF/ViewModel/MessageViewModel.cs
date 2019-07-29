using BLL;
using DirectLogWPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DirectLogWPF.ViewModel
{
    class MessageViewModel : INotifyPropertyChanged
    {
        private NatClient _client;
        private NatServer _server;

        private ObservableCollection<MessageModel> _messages { get; set; }
        public ObservableCollection<MessageModel> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                OnPropertyChanged("Messages");
            }
        }

        private string _currentMessage { get; set; }
        public string CurrentMessage
        {
            get
            {
                return _currentMessage;
            }
            set
            {
                _currentMessage = value;
                OnPropertyChanged("CurrentMessage");
            }
        }

        private string _nick { get; set; }
        public string Nick
        {
            get
            {
                return _nick;
            }
            set
            {
                _nick = value;
                OnPropertyChanged("Nick");
            }
        }

        private string _address { get; set; }
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }

        public MessageViewModel()
        {
            Messages = new ObservableCollection<MessageModel>();
        }

        private CustomCommand _sendCommand;
        public CustomCommand SendCommand
        {
            get
            {
                return _sendCommand ??
                  (_sendCommand = new CustomCommand(obj =>
                  {
                      if(!string.IsNullOrEmpty(CurrentMessage))
                      {
                          _client.SendMessage(CurrentMessage);
                          CurrentMessage = "";
                      }
                  }));
            }
        }

        private CustomCommand _connectCommand;
        public CustomCommand ConnectCommand
        {
            get
            {
                return _connectCommand ??
                  (_connectCommand = new CustomCommand(obj =>
                  {
                      if(string.IsNullOrEmpty(Address))
                      {
                          _server = NatServer.GetInstance();
                          var server = _server.Start();
                          _client = new NatClient(_server.GetEndPoint(), Nick);
                          Address = _server.GetEndPoint().ToString();
                      }
                      else
                      {
                          string[] data = Address.Split(':');
                          _client = new NatClient(
                              new IPEndPoint(IPAddress.Parse(data[0]), int.Parse(data[1])),
                              Nick);
                      }
                      var task = _client.Receive();
                      var checkMessages = CheckMessages();
                      _client.SendMessage("Joined to chat.");
                  }));
            }
        }

        private async Task CheckMessages()
        {
            while(true)
            {
                await Task.Delay(500);
                if(_client.Messages.Count > Messages.Count)
                {
                    Messages.Clear();

                    foreach (var v in _client.Messages)
                    {
                        Messages.Add(new MessageModel() { Message = v });
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
