using ModelProject2_Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Transport;

namespace ModelProject2_Server.CodeBehind
{
    public class VariaveisGlobais
    {
        //Número fixo de servidores
        public const int N_Servidores = 3;

        //Atribuir dados de indentificação do servidor ao iniciá-lo
        public static Servidor servidor1 = new Servidor();
        public static Servidor servidor2 = new Servidor();
        public static Servidor servidor3 = new Servidor();

        //Clients
        public static thriftGrafo.GrafoService.Client client_servidor1;
        public static thriftGrafo.GrafoService.Client client_servidor2;
        public static thriftGrafo.GrafoService.Client client_servidor3;

        public static Aguarde wait;

    }
}
