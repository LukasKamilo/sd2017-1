using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public GrafoCB(bool carregarArquivo)
        {
            this.Vertices = new List<Vertice>();
            this.Arestas = new List<Aresta>();

            Uteis.lerGrafoArquivo(this);

        }

        #endregion 

        #region Métodos

        //Método para Vértices
        public Retorno insertVertice(Vertice v)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.lockerVertice);

                this.atualizarGrafo();

                Vertice aux = null;

                if (this.Vertices.Count > 0)
                {
                    aux = this.Vertices.Where(p => p.Nome == v.Nome).FirstOrDefault();
                }

                if (aux != null)
                {
                    //Implica que já existe outro vertice com o mesmo nome
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível inserir o vértice " + v.Nome + ". O nome destinado ao vértice já foi utilizado!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "O nome destinado ao vértice já foi utilizado!";
                    return retorno;
                }

                this.Vertices.Add(v);

                if (!Uteis.escreverGrafoArquivo(this))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível gravar o vértice " + v.Nome + " no arquivo!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível gravar o vértice no arquivo!";
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog(ex.Message);
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível inserir o vértice! " + ex.Message;
                return retorno;
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.lockerVertice);
            }

            VariaveisGlobais.telaPrincipal.AtualizarLog("Vértice " + v.Nome + " cadastrado com sucesso!");
            retorno.Mensagem = "Vértice cadastrado com sucesso!";

            return retorno;
        }
        public Retorno updateVertice(Vertice v)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.lockerVertice);

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
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Erro ao atualizar o vértice " + v.Nome + "!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Erro ao atualizar o vértice!";
                    return retorno;
                }

                if (!Uteis.escreverGrafoArquivo(this))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível atualizar o vértice " + v.Nome + " no arquivo!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível atualizar o vértice no arquivo!";
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog(ex.Message);
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível atualizar o vértice! " + ex.Message;
                return retorno;
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.lockerVertice);
            }

            VariaveisGlobais.telaPrincipal.AtualizarLog("Vértice " + v.Nome + " atualizado com sucesso!");
            retorno.Mensagem = "Vértice atualizado com sucesso!";

            return retorno;
        }
        public Retorno deleteVertice(Vertice v)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.lockerVertice);

                atualizarGrafo();

                Vertice aux = null;
                aux = this.Vertices.Where(p => p.Nome == v.Nome).FirstOrDefault();

                //Remover todas as arestas que possuem ligação com o vértice em questão
                List<Aresta> paraRemover = new List<Aresta>();
                foreach (Aresta item in this.Arestas)
                {
                    if (item.VerticeInicio == aux.Nome || item.VerticeFim == aux.Nome)
                    {
                        paraRemover.Add(item);
                    }
                }

                //Remover arestas bidirecionais que estão em outros servidores
                foreach (Aresta item in paraRemover)
                {
                    if (item.FlagBidirecional)
                    {
                        if (item.VerticeInicio == aux.Nome)
                        {
                            int servidor = Uteis.GetServidor(item.VerticeFim.ToString());

                            Uteis.enviarRequisicaoParaServidor_DeleteAresta(servidor, item);
                        }
                        else
                        {
                            int servidor = Uteis.GetServidor(item.VerticeInicio.ToString());

                            Uteis.enviarRequisicaoParaServidor_DeleteAresta(servidor, item);
                        }
                    }
                }

                this.Arestas = this.Arestas.Except(paraRemover).ToList();

                if (aux == null || !this.Vertices.Remove(aux))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível excluir o vértice " + v.Nome + " !");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível excluir o vértice!";
                    return retorno;
                }

                if (!Uteis.escreverGrafoArquivo(this))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível remover o vértice " + v.Nome + " no arquivo!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível remover o vértice no arquivo!";
                    return retorno;
                }
            }catch(Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog(ex.Message);
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível remover o vértice! " + ex.Message;
                return retorno;
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.lockerVertice);
            }

            VariaveisGlobais.telaPrincipal.AtualizarLog("Vértice " + v.Nome + " excluído com sucesso!");
            retorno.Mensagem = "Vértice excluído com sucesso!";

            return retorno;
        }

        //Métodos para Arestas
        public Retorno insertAresta(Aresta a)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.lockerAresta);

                atualizarGrafo();

                Vertice v1 = null;
                Vertice v2 = null;

                v1 = this.Vertices.Where(p => p.Nome == a.VerticeInicio).FirstOrDefault();
                v2 = this.Vertices.Where(p => p.Nome == a.VerticeFim).FirstOrDefault();

                //Devemos verificar se o vértice de destino existe em algum outro servidor
                if (v2 == null)
                {
                    int servidor = Uteis.GetServidor(a.VerticeFim.ToString());

                    Retorno r = Uteis.enviarRequisicaoParaServidor(servidor + 1);

                    if (r.Sucesso)
                    {
                        GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

                        v2 = grafo.Vertices.Where(p => p.Nome == a.VerticeFim).FirstOrDefault();
                    }
                }

                if (v1 == null || v2 == null)
                {
                    //Implica que algum vértice informado não existe
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível cadastrar a aresta " + a.Descricao + ".Algum dos vértices informados não existe!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Algum dos vértices informados não existe!";
                    return retorno;
                }

                this.Arestas.Add(a);

                if (!Uteis.escreverGrafoArquivo(this))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível gravar a aresta " + a.Descricao + " no arquivo!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível gravar a aresta no arquivo!";
                    return retorno;
                }
            }
            catch(Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog(ex.Message);
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível gravar a aresta! " + ex.Message ;
                return retorno;
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.lockerAresta);
            }

            VariaveisGlobais.telaPrincipal.AtualizarLog("Aresta " + a.Descricao + " cadastrado com sucesso!");
            retorno.Mensagem = "Aresta cadastrado com sucesso!";

            return retorno;
        }
        public Retorno updateAresta(Aresta a)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.lockerAresta);

                atualizarGrafo();

                Aresta aux = null;
                aux = this.Arestas.Where(p => p.Descricao == a.Descricao).FirstOrDefault();

                if (aux != null && this.Arestas.Remove(aux))
                {
                    this.Arestas.Add(a);
                }
                else
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Erro ao atualizar a aresta " + a.Descricao + "!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Erro ao atualizar a aresta!";
                    return retorno;
                }

                if (!Uteis.escreverGrafoArquivo(this))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível atualizar a aresta " + a.Descricao + " no arquivo!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível atualizar a aresta no arquivo!";
                    return retorno;
                }
            }catch(Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog(ex.Message);
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível atualizar a aresta! " + ex.Message;
                return retorno;
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.lockerAresta);
            }

            VariaveisGlobais.telaPrincipal.AtualizarLog("Aresta " + a.Descricao + " atualizado com sucesso!");
            retorno.Mensagem = "Aresta atualizado com sucesso!";

            return retorno;
        }
        public Retorno deleteAresta(Aresta a)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.lockerAresta);

                atualizarGrafo();

                Aresta aux = null;
                aux = this.Arestas.Where(p => p.Descricao == a.Descricao).FirstOrDefault();

                if (aux == null || !this.Arestas.Remove(aux))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível excluir a aresta " + a.Descricao + "!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível excluir a aresta!";
                    return retorno;
                }

                if (!Uteis.escreverGrafoArquivo(this))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível excluir a aresta " + a.Descricao + " no arquivo!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível excluir a aresta no arquivo!";
                    return retorno;
                }
            }catch(Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog(ex.Message);
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível excluir a aresta! " + ex.Message;
                return retorno;
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.lockerAresta);
            }

            VariaveisGlobais.telaPrincipal.AtualizarLog("Aresta " + a.Descricao + " excluído com sucesso!");
            retorno.Mensagem = "Aresta excluído com sucesso!";

            return retorno;
        }

        //Demais métodos
        public Retorno excluirGrafo()
        {
            Retorno retorno = new Retorno(true);

            this.Vertices.Clear();
            this.Arestas.Clear();

            try
            {
                Monitor.Enter(VariaveisGlobais.locker);

                if (!Uteis.escreverGrafoArquivo(this))
                {
                    VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível excluir o grafo!");
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Não foi possível excluir o grafo!";
                    return retorno;
                }
            }catch(Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível excluir o grafo! " + ex.Message);
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível excluir o grafo! " + ex.Message;
                return retorno;
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.locker);
            }

            VariaveisGlobais.telaPrincipal.AtualizarLog("Grafo excluído com sucesso!");
            retorno.Mensagem = "Grafo excluido com sucesso!";

            return retorno;
        }
        public Retorno getGrafo()
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.locker);
                atualizarGrafo();
            }catch(Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível ler o grafo! " + ex.Message);
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.locker);
            }

            retorno.Retorno_ = JsonConvert.SerializeObject(this);

            return retorno;
        }
        public Retorno listarArestasVertice(Vertice v)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.locker);
                atualizarGrafo();
            }
            catch (Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível ler o grafo! " + ex.Message);
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.locker);
            }

            List<Aresta> _Arestas = new List<Aresta>();

            _Arestas = this.Arestas.Where(p => p.VerticeInicio == v.Nome).ToList();

            //Serializado como uma lista de arestas
            retorno.Retorno_ = JsonConvert.SerializeObject(_Arestas);

            return retorno;
        }
        public Retorno listarVerticesAresta(Aresta a)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.locker);
                atualizarGrafo();
            }
            catch (Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível ler o grafo! " + ex.Message);
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.locker);
            }

            List<Vertice> vertices = new List<Vertice>();

            Vertice vOrigem = null;
            Vertice vDestino = null;

            Aresta arestaCorrente = Arestas.Where(p => p.Descricao == a.Descricao).FirstOrDefault();

            if(arestaCorrente == null)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "A aresta informada não existe!";

                return retorno;
            }

            vOrigem = Vertices.Where(p => p.Nome == arestaCorrente.VerticeInicio).FirstOrDefault();

            if (vOrigem != null)
            {
                int servidor = Uteis.GetServidor(arestaCorrente.VerticeFim.ToString());

                if ((servidor + 1) == VariaveisGlobais.servidorLocal.Identificador)
                {
                    vDestino = Vertices.Where(p => p.Nome == arestaCorrente.VerticeFim).FirstOrDefault();
                }
                else
                {
                    Retorno r = Uteis.enviarRequisicaoParaServidor(servidor + 1);

                    if (r.Sucesso)
                    {
                        GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

                        vDestino = grafo.Vertices.Where(p => p.Nome == arestaCorrente.VerticeFim).FirstOrDefault();
                    }
                }

                vertices.Add(vOrigem);
                vertices.Add(vDestino);

                //Serializado como uma lista de vertices
                retorno.Sucesso = true;
                retorno.Retorno_ = JsonConvert.SerializeObject(vertices);

            }
            else
            {
                retorno.Sucesso = false;
            }

            return retorno;
        }
        public Retorno listarVizinhoVertice(Vertice v)
        {
            Retorno retorno = new Retorno(true);

            try
            {
                Monitor.Enter(VariaveisGlobais.locker);
                atualizarGrafo();
            }
            catch (Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível ler o grafo! " + ex.Message);
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.locker);
            }

            List<Vertice> vizinhos = new List<Vertice>();

            List<Aresta> arestas = null;

            //Todas as arestas do vertice
            arestas = this.Arestas.Where(p => p.VerticeInicio == v.Nome).ToList();

            if (arestas != null)
            {
                foreach (Aresta item in arestas)
                {

                    Vertice v1 = this.Vertices.Where(p => p.Nome == item.VerticeFim).FirstOrDefault();

                    if(v1 != null)
                    {
                        vizinhos.Add(v1);
                    }
                    else
                    {

                        int servidor = Uteis.GetServidor(item.VerticeFim.ToString());
                        Retorno r = Uteis.enviarRequisicaoParaServidor(servidor + 1);

                        GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

                        v1 = grafo.Vertices.Where(p => p.Nome == item.VerticeFim).FirstOrDefault();

                        vizinhos.Add(v1);

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

            try
            {
                Monitor.Enter(VariaveisGlobais.locker);
                atualizarGrafo();
            }
            catch (Exception ex)
            {
                VariaveisGlobais.telaPrincipal.AtualizarLog("Não foi possível ler o grafo! " + ex.Message);
            }
            finally
            {
                Monitor.Exit(VariaveisGlobais.locker);
            }

            retorno = Uteis.AlgoritmoMenorCaminho(this, origem, destino);

            return retorno;
        }

        #endregion Métodos

        #region Métodos Auxiliares

        public void atualizarGrafo()
        {
            Uteis.lerGrafoArquivo(this);
        }

        #endregion
    }
}
