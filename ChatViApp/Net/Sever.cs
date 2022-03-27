using ChatClient.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    internal class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgRecebidaEvent;
        public event Action usuarioDesconectadoEvent;
        public Server()
        {
            _client = new TcpClient();
        }

        public void ConectarServidor(string NomeUsuario)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(NomeUsuario))
                {

                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(NomeUsuario);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgRecebidaEvent.Invoke();
                            break;
                        case 10:
                            usuarioDesconectadoEvent.Invoke();
                            break;


                        default:
                            Console.WriteLine("Yep...");
                            break;

                    }
                }
            });
        }

        public void EnviarMessagemServidor(string menssagem)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(menssagem);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
