using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thriftGrafo;

namespace ModelProject2_Server.CodeBehind
{
    public class GrafoCB : GrafoService.Iface
    {

        #region Atributos e propriedades

        public List<Vertice> Vertices;
        public List<Aresta> Arestas;

        #endregion

        #region Construtor Grafo

        public GrafoCB()
        {
            this.Vertices = new List<Vertice>();
            this.Arestas = new List<Aresta>();

        }

        //public GrafoCB(string caminhoArquivo)
        //{
        //    this.Vertices = new List<Vertice>();
        //    this.Arestas = new List<Aresta>();

        //    Util.lerGrafoArquivo(this, caminhoArquivo);

        //}

        #endregion 

        #region Métodos
        public Retorno deleteAresta(Aresta a)
        {
            throw new NotImplementedException();
        }

        public Retorno deleteVertice(Vertice v)
        {
            throw new NotImplementedException();
        }

        public Retorno excluirGrafo()
        {
            throw new NotImplementedException();
        }

        public Retorno getGrafo()
        {
            throw new NotImplementedException();
        }

        public Retorno insertAresta(Aresta a)
        {
            throw new NotImplementedException();
        }

        public Retorno insertVertice(Vertice v)
        {
            throw new NotImplementedException();
        }

        public Retorno listarArestasVertice(Vertice v)
        {
            throw new NotImplementedException();
        }

        public Retorno listarVerticesAresta(Aresta a)
        {
            throw new NotImplementedException();
        }

        public Retorno listarVizinhoVertice(Vertice v)
        {
            throw new NotImplementedException();
        }

        public Retorno menorCaminho(Vertice origem, Vertice destino)
        {
            throw new NotImplementedException();
        }

        public Retorno updateAresta(Aresta a)
        {
            throw new NotImplementedException();
        }

        public Retorno updateVertice(Vertice v)
        {
            throw new NotImplementedException();
        }

        #endregion Métodos
    }
}
