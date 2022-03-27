using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Client
    {
        public string NomeUsuario{ get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            NomeUsuario = _packetReader.LerMenssagem();

            Console.WriteLine($"[{DateTime.Now}]: O cliente foi conectado com o Usuario: {NomeUsuario}");

            Task.Run(() => Processo());
        }

        void Processo()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadString();
                            Console.WriteLine($"[{DateTime.Now}]: Menssagem recebida! {msg}");
                            Program.TransmitirMenssagem($"[{DateTime.Now}]: [{NomeUsuario}]: {msg}");
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Desconectado!");
                    Program.MenssagemDesconectado(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
