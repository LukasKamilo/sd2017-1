using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelProject2_Server.CodeBehind
{
    public class VariaveisGlobais
    {
        //Número fixo de servidores
        public const int N_Servidores = 3;

        //Atribuir dados de indentificação do servidor ao iniciá-lo
        public static Servidor servidorLocal = new Servidor();

        //Atribuir informações dos demais servidores vizinhos
        public static List<ServidorVizinho> servidoresVizinhos = new List<ServidorVizinho>();

        //Caminhos arquivos
        public static string caminhoArquivos = "";

    }
}
