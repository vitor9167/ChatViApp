using Chat_teste_.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    internal class MainViewModel
    {

        public ObservableCollection<ModeloUsuario> Usuarios { get; set; }
        public ObservableCollection<string> Menssagems { get; set; }

        public RelayCommand ConectarServidorComando { get; set; }
        public RelayCommand EnviarMenssagemComando { get; set; }

        public string NomeUsuario { get; set; }
        public string Menssagem { get; set; }
        private Server _server;

        public MainViewModel()
        {
            Usuarios = new ObservableCollection<ModeloUsuario>();
            Menssagems = new ObservableCollection<string>();
            _server = new Server();
            _server.connectedEvent += UsuarioConecatado;
            _server.msgRecebidaEvent += MenssagemRecebida;
            _server.usuarioDesconectadoEvent += RemoverUsuario;
            ConectarServidorComando = new RelayCommand(o => _server.ConectarServidor(NomeUsuario), o => !string.IsNullOrEmpty(NomeUsuario));

            EnviarMenssagemComando = new RelayCommand(o => _server.EnviarMessagemServidor(Menssagem), o => !string.IsNullOrEmpty(Menssagem));
        }

        private void RemoverUsuario()
        {
            var uid = _server.PacketReader.LerMenssagem();
            var usuario = Usuarios.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Usuarios.Remove(usuario));
        }

        private void MenssagemRecebida()
        {
            var msg = _server.PacketReader.LerMenssagem();
            Application.Current.Dispatcher.Invoke(() => Menssagems.Add(msg));
        }

        private void UsuarioConecatado()
        {
            var usuario = new ModeloUsuario
            {
                NomeUsuario = _server.PacketReader.LerMenssagem(),
                UID = _server.PacketReader.LerMenssagem(),
            };

            if (!Usuarios.Any(x => x.UID == usuario.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Usuarios.Add(usuario));
            }

        }
    }
}
