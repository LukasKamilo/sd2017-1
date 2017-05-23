using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace thriftGrafo.GrafoCodeBehind
{
    public class GrafoCB : GrafoService.Iface
    {
        private object locker = new object();

        #region Atributos e propriedades do Grafo

        public List<Vertice> Vertices { get; set; }
        public List<Aresta> Arestas { get; set; }

        #endregion

        #region Construtor Grafo

        public GrafoCB()
        {
            this.Vertices = new List<Vertice>();
            this.Arestas = new List<Aresta>();

        }

        public GrafoCB(string caminhoArquivo)
        {
            this.Vertices = new List<Vertice>();
            this.Arestas = new List<Aresta>();

            Util.lerGrafoArquivo(this, caminhoArquivo);

        }

        #endregion 

        #region Métodos do grafo

        public void atualizarGrafo()
        {
            Util.lerGrafoArquivo(this, Util.caminho);
        }

        public Retorno getGrafo()
        {
            Retorno retorno = new Retorno();

            Monitor.Enter(locker);
            atualizarGrafo();
            Monitor.Exit(locker);

            retorno.Retorno_ = JsonConvert.SerializeObject(this);

            return retorno;
        }

        public Retorno insertVertice(Vertice v)
        {

            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            this.atualizarGrafo();

            Vertice aux = null;

            if (this.Vertices.Count > 0)
            {
                aux = this.Vertices.Where(p => p.Nome == v.Nome).FirstOrDefault();
            }

            if (aux != null)
            {
                //Implica que já existe outro vertice com o mesmo nome
                Console.WriteLine("O nome destinado ao vértice já foi utilizado!");
                retorno.Sucesso = false;
                retorno.Mensagem = "O nome destinado ao vértice já foi utilizado!";
                return retorno;
            }

            this.Vertices.Add(v);

            if (!Util.escreverGrafoArquivo(this, Util.caminho))
            {
                Console.WriteLine("Não foi possível gravar o vértice no arquivo!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível gravar o vértice no arquivo!";
                return retorno;
            }

            Monitor.Exit(locker);

            Console.WriteLine("Vértice cadastrado com sucesso!");
            retorno.Mensagem = "Vértice cadastrado com sucesso!";

            return retorno;
        }

        public Retorno updateVertice(Vertice v)
        {

            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            atualizarGrafo();

            Vertice aux = null;
            aux = this.Vertices.Where(p => p.Nome == v.Nome).FirstOrDefault();

            if (aux != null && this.Vertices.Remove(aux))
            {
                this.Vertices.Add(v);
            }
            else
            {
                Console.WriteLine("Erro ao atualizar o vértice!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Erro ao atualizar o vértice!";
                return retorno;
            }

            if (!Util.escreverGrafoArquivo(this, Util.caminho))
            {
                Console.WriteLine("Não foi possível atualizar o vértice no arquivo!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível atualizar o vértice no arquivo!";
                return retorno;
            }

            Monitor.Exit(locker);

            Console.WriteLine("Vértice atualizado com sucesso!");
            retorno.Mensagem = "Vértice atualizado com sucesso!";

            return retorno;
        }

        public Retorno deleteVertice(Vertice v)
        {

            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            atualizarGrafo();

            Vertice aux = null;
            aux = this.Vertices.Where(p => p.Nome == v.Nome).FirstOrDefault();

            //Remover todas as arestas que possuem ligação com o vértice em questão
            List<Aresta> paraRemover = new List<Aresta>();
            foreach(Aresta item in this.Arestas)
            {
                if(item.VerticeInicio == aux.Nome || item.VerticeFim == aux.Nome)
                {
                    paraRemover.Add(item);
                }
            }

            this.Arestas = this.Arestas.Except(paraRemover).ToList();

            if (aux == null || !this.Vertices.Remove(aux))
            {
                Console.WriteLine("Não foi possível excluir o vértice!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível excluir o vértice!";
                return retorno;
            }

            if (!Util.escreverGrafoArquivo(this, Util.caminho))
            {
                Console.WriteLine("Não foi possível remover o vértice no arquivo!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível remover o vértice no arquivo!";
                return retorno;
            }

            Monitor.Exit(locker);

            Console.WriteLine("Vértice excluído com sucesso!");
            retorno.Mensagem = "Vértice excluído com sucesso!";

            return retorno;
        }

        public Retorno insertAresta(Aresta a)
        {

            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            atualizarGrafo();

            Vertice v1 = null;
            Vertice v2 = null;

            v1 = this.Vertices.Where(p => p.Nome == a.VerticeInicio).FirstOrDefault();
            v2 = this.Vertices.Where(p => p.Nome == a.VerticeFim).FirstOrDefault();

            if (v1 == null || v2 == null)
            {
                //Implica que algum vértice informado não existe
                Console.WriteLine("Algum dos vértices informados não existe!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Algum dos vértices informados não existe!";
                return retorno;
            }

            this.Arestas.Add(a);

            if (!Util.escreverGrafoArquivo(this, Util.caminho))
            {
                Console.WriteLine("Não foi possível gravar a aresta no arquivo!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível gravar a aresta no arquivo!";
                return retorno;
            }

            Monitor.Exit(locker);

            Console.WriteLine("Aresta cadastrado com sucesso!");
            retorno.Mensagem = "Aresta cadastrado com sucesso!";

            return retorno;
        }

        public Retorno updateAresta(Aresta a)
        {

            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            atualizarGrafo();

            Aresta aux = null;
            aux = this.Arestas.Where(p => p.Descricao == a.Descricao).FirstOrDefault();

            if (aux != null && this.Arestas.Remove(aux))
            {
                this.Arestas.Add(a);
            }
            else
            {
                Console.WriteLine("Erro ao atualizar o aresta!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Erro ao atualizar o aresta!";
                return retorno;
            }

            if (!Util.escreverGrafoArquivo(this, Util.caminho))
            {
                Console.WriteLine("Não foi possível atualizar a aresta no arquivo!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível atualizar a aresta no arquivo!";
                return retorno;
            }

            Monitor.Exit(locker);

            Console.WriteLine("Aresta atualizado com sucesso!");
            retorno.Mensagem = "Aresta atualizado com sucesso!";

            return retorno;
        }

        public Retorno deleteAresta(Aresta a)
        {

            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            atualizarGrafo();

            Aresta aux = null;
            aux = this.Arestas.Where(p => p.Descricao == a.Descricao).FirstOrDefault();

            if (aux == null || !this.Arestas.Remove(aux))
            {
                Console.WriteLine("Não foi possível excluir a aresta!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível excluir a aresta!";
                return retorno;
            }

            if (!Util.escreverGrafoArquivo(this, Util.caminho))
            {
                Console.WriteLine("Não foi possível excluir a aresta no arquivo!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível excluir a aresta no arquivo!";
                return retorno;
            }

            Monitor.Exit(locker);

            Console.WriteLine("Aresta excluído com sucesso!");
            retorno.Mensagem = "Aresta excluído com sucesso!";

            return retorno;
        }

        public Retorno listarVerticesAresta(Aresta a)
        {

            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            atualizarGrafo();

            Monitor.Exit(locker);

            List<Vertice> vertices = new List<Vertice>();

            Vertice vOrigem = null;
            Vertice vDestino = null;

            vOrigem = this.Vertices.Where(p => p.Nome == a.VerticeInicio).FirstOrDefault();
            vDestino = this.Vertices.Where(p => p.Nome == a.VerticeFim).FirstOrDefault();

            vertices.Add(vOrigem);
            vertices.Add(vDestino);

            //Serializado como uma lista de vertices
            retorno.Retorno_ = JsonConvert.SerializeObject(vertices);

            return retorno;
        }

        public Retorno listarArestasVertice(Vertice v)
        {
            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            atualizarGrafo();

            Monitor.Exit(locker);

            List<Aresta> _Arestas = new List<Aresta>();

            _Arestas = this.Arestas.Where(p => p.VerticeInicio == v.Nome || p.VerticeFim == v.Nome).ToList();

            //Serializado como uma lista de arestas
            retorno.Retorno_ = JsonConvert.SerializeObject(_Arestas);

            return retorno;
        }

        public Retorno listarVizinhoVertice(Vertice v)
        {

            Retorno retorno = new Retorno(true);

            Monitor.Enter(locker);

            atualizarGrafo();

            Monitor.Exit(locker);

            List<Vertice> vizinhos = new List<Vertice>();

            List<Aresta> arestas = null;

            //Todas as arestas do vertice
            arestas = this.Arestas.Where(p => p.VerticeInicio == v.Nome || p.VerticeFim == v.Nome).ToList();

            if (arestas != null)
            {
                foreach (Aresta item in arestas)
                {
                    if (item.VerticeInicio == v.Nome)
                    {
                        vizinhos.Add(this.Vertices.Where(p => p.Nome == item.VerticeFim).FirstOrDefault());
                    }
                    else
                    {
                        vizinhos.Add(this.Vertices.Where(p => p.Nome == item.VerticeInicio).FirstOrDefault());
                    }
                }
            }

            //Serializado em uma lista de vertices
            retorno.Retorno_ = JsonConvert.SerializeObject(vizinhos);

            return retorno;
        }

        public Retorno menorCaminho(Vertice origem, Vertice destino)
        {
            Retorno retorno = new Retorno();

            retorno = Util.algoritmoDijkstra(this, origem, destino);

            return retorno;
        }

        public Retorno excluirGrafo()
        {
            Retorno retorno = new Retorno();

            Monitor.Enter(locker);

            this.Vertices.Clear();
            this.Arestas.Clear();

            if (!Util.escreverGrafoArquivo(this, Util.caminho))
            {
                Console.WriteLine("Não foi possível excluir o grafo!");
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível excluir o grafo!";
                return retorno;
            }

            Monitor.Exit(locker);

            retorno.Mensagem = "Grafo excluido com sucesso!";

            return retorno;
        }

        #endregion
    }
}
