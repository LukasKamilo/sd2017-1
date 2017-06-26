using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Transport;
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

        public static Retorno enviarRequisicaoParaServidor(int identificadorServidor)
        {
            Retorno retorno = new Retorno(true);
            ServidorVizinho servidor = VariaveisGlobais.servidoresVizinhos.Where(p => p.Identificador == (identificadorServidor) ).FirstOrDefault();

            try
            {
                TTransport transport = new TSocket(servidor.IP, Convert.ToInt32(servidor.Porta));
                transport.Open();
                TProtocol protocol = new TBinaryProtocol(transport);
                VariaveisGlobais.client_servidor1 = new thriftGrafo.GrafoService.Client(protocol);

                retorno = VariaveisGlobais.client_servidor1.getGrafo();

                transport.Close();
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível iniciar o servidor. " + ex.Message;
                return retorno;
            }
            
            return retorno;
        }

        public static Retorno enviarRequisicaoParaServidor_DeleteAresta(int identificadorServidor, Aresta a)
        {
            Retorno retorno = new Retorno(true);
            ServidorVizinho servidor = VariaveisGlobais.servidoresVizinhos.Where(p => p.Identificador == (identificadorServidor + 1)).FirstOrDefault();

            try
            {
                TTransport transport = new TSocket(servidor.IP, Convert.ToInt32(servidor.Porta));
                transport.Open();
                TProtocol protocol = new TBinaryProtocol(transport);
                VariaveisGlobais.client_servidor1 = new thriftGrafo.GrafoService.Client(protocol);

                retorno = VariaveisGlobais.client_servidor1.deleteAresta(a);

                transport.Close();
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível iniciar o servidor. " + ex.Message;
                return retorno;
            }

            return retorno;
        }

        public static Retorno AlgoritmoMenorCaminho(GrafoCB gr, Vertice origem, Vertice destino)
        {
            //Precisamos carregar todos os dados em um único servidor

            int nServidores = VariaveisGlobais.servidoresVizinhos.Count();

            for(var i = 0; i < nServidores; i++)
            {
                Retorno retorno = enviarRequisicaoParaServidor(VariaveisGlobais.servidoresVizinhos[i].Identificador);

                if (retorno.Sucesso)
                {
                    GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(retorno.Retorno_);

                    gr.Vertices.AddRange(grafo.Vertices);
                    gr.Arestas.AddRange(grafo.Arestas);
                }
            }

            origem = gr.Vertices.Where(p => p.Nome == origem.Nome).FirstOrDefault();
            destino = gr.Vertices.Where(p => p.Nome == destino.Nome).FirstOrDefault();

            if(origem != null && destino != null)
            {
                return algoritmoDijkstra(gr, origem, destino);
            }
            else
            {
                Retorno r = new Retorno();
                r.Sucesso = false;
                r.Mensagem = "Não foi possível identificar os vértices informados";

                return r;
            }
        }

        /// <summary>
        /// Algoritmo de Dijkstra
        /// </summary>
        ///<param name="gr">Grafo de entrada</param>
        ///<param name="origem">Vértice de origem</param>
        ///<param name="destino">Vértice de destino</param>
        ///<returns>O caminho é retornado em resultado</returns>
        public static Retorno algoritmoDijkstra(GrafoCB gr, Vertice origem, Vertice destino)
        {
            Retorno retorno = new Retorno(true);
            List<Vertice> visitados = new List<Vertice>();
            List<Dijkstra> alg = new List<Dijkstra>();

            Vertice atual;

            foreach (Vertice item in gr.Vertices)
            {

                alg.Add(new Dijkstra()
                {
                    vertice = item,
                    estimativa = (item.Nome == origem.Nome ? 0 : Int32.MaxValue),
                    precedente = null,
                    visitado = false
                });
            }

            while (visitados.Count < gr.Vertices.Count)
            {
                alg = alg.OrderBy(p => p.estimativa).ToList();

                Dijkstra controleV1 = alg.Where(p => p.visitado == false).FirstOrDefault();

                if (controleV1 == null)
                {
                    break;
                }

                atual = controleV1.vertice;

                //Setar vértice como visitado
                Dijkstra controleAtual = alg.Where(p => p.vertice.Nome == atual.Nome).FirstOrDefault();
                int indiceAtual = alg.IndexOf(controleAtual);
                alg[indiceAtual].visitado = true;
                visitados.Add(atual);

                //Regra
                //Encontrar todos os vértices não visitados adjacentes ao vértice atual
                List<Aresta> arestasAdjacentes = new List<Aresta>(); //Lista de arestas adjacentes não visitados

                foreach (Aresta item in gr.Arestas)
                {
                    //if (item.FlagBidirecional && (item.VerticeInicio == atual.Nome || item.VerticeFim == atual.Nome))
                    //{
                    //    //Aresta indo
                    //    arestasAdjacentes.Add(item);

                    //    //Vamos inverter a aresta para simular o bidirecionamento, aresta vindo
                    //    arestasAdjacentes.Add(new Aresta()
                    //    {
                    //        VerticeInicio = item.VerticeFim,
                    //        VerticeFim = item.VerticeInicio,
                    //        FlagBidirecional = item.FlagBidirecional,
                    //        Peso = item.Peso,
                    //        Descricao = "I" + item.Descricao
                    //    });
                    //}
                    //else 
                    if (item.VerticeInicio == atual.Nome)
                    {
                        arestasAdjacentes.Add(item);
                    }
                }

                foreach (Aresta item in arestasAdjacentes)
                {
                    //Vamos considerar inicialmente só o caso de ser direcionado
                    Vertice v = gr.Vertices.Where(p => p.Nome == item.VerticeFim).FirstOrDefault();

                    //Se o vértice não foi visitado adiciona na lista
                    if (!visitados.Contains(v))
                    {
                        //Calcular a estimativa dos vértice adjacente
                        Dijkstra verticeAdj = alg.Where(p => p.vertice.Nome == v.Nome).FirstOrDefault();
                        int indice = alg.IndexOf(verticeAdj);

                        double estimativaCalculada = controleAtual.estimativa + item.Peso;

                        if (alg[indice].estimativa > estimativaCalculada)
                        {
                            alg[indice].estimativa = estimativaCalculada;
                            alg[indice].precedente = atual;
                        }
                    }
                }
            }

            Vertice final = destino;
            List<Vertice> caminho = new List<Vertice>();
            while (final.Nome != origem.Nome)
            {
                caminho.Insert(0, final);

                Dijkstra aux3 = alg.Where(p => p.vertice.Descricao == final.Descricao).FirstOrDefault();

                final = aux3.precedente;
            }

            caminho.Insert(0, origem);

            retorno.Retorno_ = JsonConvert.SerializeObject(caminho);

            return retorno;
        }

        public class Dijkstra
        {
            public Vertice vertice;
            public double estimativa;
            public Vertice precedente;
            public bool visitado = false;
        }
    }
}
