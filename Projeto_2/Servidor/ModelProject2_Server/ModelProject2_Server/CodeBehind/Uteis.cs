using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using thriftGrafo;

namespace ModelProject2_Server.CodeBehind
{
    public class Uteis
    {
        public static bool escreverGrafoArquivo(GrafoCB gr)
        {

            try
            {
                using (StreamWriter writer = new StreamWriter(VariaveisGlobais.caminhoArquivos + "arestas.txt"))
                {

                    string a = JsonConvert.SerializeObject(gr.Arestas);

                    writer.Write(a);
                }

                using (StreamWriter writer = new StreamWriter(VariaveisGlobais.caminhoArquivos + "vertices.txt"))
                {

                    string a = JsonConvert.SerializeObject(gr.Vertices);
                    writer.Write(a);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool lerGrafoArquivo(GrafoCB gr)
        {

            try
            {
                using (StreamReader reader = new StreamReader(VariaveisGlobais.caminhoArquivos + "arestas.txt"))
                {
                    string linha = reader.ReadToEnd();

                    gr.Arestas = JsonConvert.DeserializeObject<List<Aresta>>(linha);
                }

                using (StreamReader reader = new StreamReader(VariaveisGlobais.caminhoArquivos + "vertices.txt"))
                {
                    string linha = reader.ReadToEnd();

                    gr.Vertices = JsonConvert.DeserializeObject<List<Vertice>>(linha);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calcula o Hash MD5 de uma string
        /// </summary>
        /// <param name="strEntrada">String para calcular o hash</param>
        /// <returns>O hash MD5 da string</returns>
        public static string GeraHashMD5(string strEntrada)
        {
            if (String.IsNullOrEmpty(strEntrada))
            {
                return "";
            }

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(strEntrada));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        /// <summary>
        /// Calcula o servidor que será destinado dado a string codificada
        /// </summary>
        /// <param name="dado">String para efeutar o calculo</param>
        /// <returns>O identificador do servidor</returns>
        public static int CalcularModHash(string dado)
        {
            int soma = 0;

            for (var i = 0; i < 4; i++)
            {
                soma += Convert.ToInt16(dado[i]);
            }

            return soma % VariaveisGlobais.N_Servidores;
        }

        /// <summary>
        /// Calcula o servidor que será destinado
        /// </summary>
        /// <param name="valor">String para efeutar o calculo</param>
        /// <returns>O identificador do servidor</returns>
        public static int GetServidor(string valor)
        {
            string dado = GeraHashMD5(valor);

            return CalcularModHash(dado);
        }

    }
}
