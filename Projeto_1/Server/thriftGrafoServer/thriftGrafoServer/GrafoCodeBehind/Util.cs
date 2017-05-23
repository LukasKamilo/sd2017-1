using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thriftGrafo.GrafoCodeBehind
{
    public class Util
    {
        public static string caminho = "D:/UFU/SD/Projeto 1-1/Server/thriftGrafoServer/thriftGrafoServer/Arquivos/";

        public static bool escreverGrafoArquivo(GrafoCB gr, string caminhoArquivos)
        {

            try
            {
                using (StreamWriter writer = new StreamWriter(caminhoArquivos + "arestas.txt"))
                {

                    string a = JsonConvert.SerializeObject(gr.Arestas);

                    writer.Write(a);
                }

                using (StreamWriter writer = new StreamWriter(caminhoArquivos + "vertices.txt"))
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

        public static bool lerGrafoArquivo(GrafoCB gr, string caminhoArquivos)
        {

            try
            {
                using (StreamReader reader = new StreamReader(caminhoArquivos + "arestas.txt"))
                {
                    string linha = reader.ReadToEnd();

                    gr.Arestas = JsonConvert.DeserializeObject<List<Aresta>>(linha);
                }

                using (StreamReader reader = new StreamReader(caminhoArquivos + "vertices.txt"))
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

        public static Retorno algoritmoDijkstra(GrafoCB gr, Vertice origem, Vertice destino)
        {
            Retorno retorno = new Retorno();
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

            while(visitados.Count < gr.Vertices.Count)
            {
                alg = alg.OrderBy(p => p.estimativa).ToList();

                Dijkstra controleV1 = alg.Where(p => p.visitado == false).FirstOrDefault();

                if(controleV1 == null)
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

                foreach(Aresta item in gr.Arestas)
                {
                    if (item.FlagBidirecional && (item.VerticeInicio == atual.Nome || item.VerticeFim == atual.Nome))
                    {
                        //Aresta indo
                        arestasAdjacentes.Add(item);

                        //Vamos inverter a aresta para simular o bidirecionamento, aresta vindo
                        arestasAdjacentes.Add(new Aresta()
                        {
                            VerticeInicio = item.VerticeFim,
                            VerticeFim = item.VerticeInicio,
                            FlagBidirecional = item.FlagBidirecional,
                            Peso = item.Peso,
                            Descricao = "I"+item.Descricao
                        });
                    }
                    else if(!item.FlagBidirecional && (item.VerticeInicio == atual.Nome))
                    {
                        arestasAdjacentes.Add(item);
                    }
                }

                foreach(Aresta item in arestasAdjacentes)
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

                        if(alg[indice].estimativa > estimativaCalculada)
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
