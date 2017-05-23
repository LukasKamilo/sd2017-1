using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Server;
using Thrift.Transport;
using thriftGrafo.GrafoCodeBehind;

namespace thriftGrafo
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                GrafoCB hwServer = new GrafoCB();
                GrafoService.Processor processor = new GrafoService.Processor(hwServer);
                TServerTransport serverTransport = new TServerSocket(9091);
                TServer server = new TThreadPoolServer(processor, serverTransport);
                Console.WriteLine("Iniciando Servidor...");
                server.Serve();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Finalizado!");
        }
    }
}
