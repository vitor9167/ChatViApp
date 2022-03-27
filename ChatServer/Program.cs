using ChatServer.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        static List<Client> _usuarios;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _usuarios = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _usuarios.Add(client);

                //transmitir a conexão com todo mundo no app
                TransmitirConexao();
            }
        }

        static void TransmitirConexao()
        {
            foreach (var usuario in _usuarios)
            {
                foreach(var usr in _usuarios)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.NomeUsuario);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    usuario.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());

                }
            }
        }

        public static void TransmitirMenssagem(string menssagem)
        {
            foreach (var usuario in _usuarios)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(menssagem);
                usuario.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void MenssagemDesconectado(string uid)
        {
            var usuarioDesconectado = _usuarios.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _usuarios.Remove(usuarioDesconectado);

            foreach (var usuario in _usuarios)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                usuario.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            TransmitirMenssagem($"[{usuarioDesconectado.NomeUsuario}] Desconectou!");
        }

    }
}
