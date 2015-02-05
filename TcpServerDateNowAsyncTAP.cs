using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace test{    
    public class TimeServer{
        
        private readonly TcpListener _listener;

        public TimeServer(int port){
            _listener = new TcpListener(IPAddress.Any, port);
        }
        
        public async Task Start(){
            try{
                _listener.Start();
                for(;;){
                  using(TcpClient socket = await _listener.AcceptTcpClientAsync()){
                    Console.WriteLine(String.Format("Connection established with {0}.", socket.Client.RemoteEndPoint));
                    socket.LingerState = new LingerOption(true, 8);            //optimizações apos Close().
                    NetworkStream s = socket.GetStream();
                    //tratamento da resposta
                    string text = DateTime.Now.ToString();
                    byte[] b = Encoding.ASCII.GetBytes(text);
                    //envio da resposta
                    await s.WriteAsync(b, 0, b.Length);  
                  }
                }
            }finally{
                if(_listener != null)
                    _listener.Stop();
            }
        }
        
         static void Main(){
            TimeServer ts = new TimeServer(8888);
            ts.Start().Wait();
        }
        
    }
}