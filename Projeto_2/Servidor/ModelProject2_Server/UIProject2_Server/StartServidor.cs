using ModelProject2_Server.CodeBehind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Thrift.Server;
using Thrift.Transport;
using thriftGrafo;

namespace UIProject2_Server
{
    public class StartServidor
    {
        public static bool IniciarServidor()
        {
            try
            {
                Thread threadServidor = new Thread(new ThreadStart(ThreadStartPoint));
                threadServidor.SetApartmentState(ApartmentState.STA);
                threadServidor.IsBackground = true;
                threadServidor.Start();

            }
            catch (Exception ex)
            {
                //Criar método para escrever no Log
                VariaveisGlobais.telaPrincipal.AtualizarLog(ex.Message);
                return false;
            }

            return true;
        }

        private static void ThreadStartPoint()
        {
            GrafoCB hwServer = new GrafoCB(true);
            GrafoService.Processor processor = new GrafoService.Processor(hwServer);
            TServerTransport serverTransport = new TServerSocket(Convert.ToInt16(VariaveisGlobais.servidorLocal.Porta));
            TServer server = new TThreadPoolServer(processor, serverTransport);
            server.Serve();
        }
    }
}
