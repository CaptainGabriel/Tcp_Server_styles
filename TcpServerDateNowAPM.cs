using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace test{    
    public class TimeServer{

        private readonly TcpListener _listener;

        public TimeServer(int port){
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start(){
            try{
                _listener.Start();
                _listener.BeginAcceptTcpClient(GetClientCallback, null);
                //apenas para aguentar execução
                Console.ReadLine();
            }finally{
                if(_listener != null)
                    _listener.Stop();
            }
        }

        private void GetClientCallback(IAsyncResult res){
            try{
                TcpClient socket = _listener.EndAcceptTcpClient(res);
                 _listener.BeginAcceptTcpClient(GetClientCallback, null);
                Console.WriteLine(String.Format("Connection established with {0}.", socket.Client.RemoteEndPoint));
                socket.LingerState = new LingerOption(true, 8);            //optimizações apos Close().
                NetworkStream s = socket.GetStream();
                //tratamento da resposta
                string text = DateTime.Now.ToString();
                byte[] b = Encoding.ASCII.GetBytes(text);
                //envio da resposta
                s.BeginWrite(b, 0, b.Length, EndReply, s);  //stream não é uma variavel global, logo tem de ir no estado.

            }catch(SocketException){
                //alguma mensagem
            }catch(ObjectDisposedException){
                //ignorada
            }
        }

        private void EndReply(IAsyncResult res){
            //stream via estado
            NetworkStream ns = res.AsyncState as NetworkStream;
            //por simplicidade, apenas serve para terminar o processo.
            ns.EndWrite(res);
        }
        
        static void Main(){
            TimeServer ts = new TimeServer(8888);
            ts.Start();
        }
    }
}
