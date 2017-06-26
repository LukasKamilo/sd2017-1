namespace csharp thriftGrafo

struct Aresta{
	1: i32 verticeInicio
	2: i32 verticeFim
	3: bool FlagBidirecional
	4: double peso
	5: string descricao
}

struct Vertice{
	1: i32 nome
	2: i32 cor
	3: string descricao
	4: double peso
}

struct GrafoAtributo{
	1: list<Vertice> vertices
	2: list<Aresta> arestas
}

struct Retorno{
	1: bool sucesso
	2: string mensagem
	3: string retorno
}

service GrafoService{
	Retorno getGrafo(),
	Retorno insertVertice(1: Vertice v),
	Retorno updateVertice(1: Vertice v),
	Retorno deleteVertice(1: Vertice v),
	Retorno insertAresta(1: Aresta a),
	Retorno updateAresta(1: Aresta a),
	Retorno deleteAresta(1: Aresta a),
	Retorno listarVerticesAresta(1: Aresta a),
	Retorno listarArestasVertice(1: Vertice v),
	Retorno listarVizinhoVertice(1: Vertice v),
	Retorno menorCaminho(1: Vertice origem 2: Vertice destino),
	Retorno excluirGrafo()
	
}