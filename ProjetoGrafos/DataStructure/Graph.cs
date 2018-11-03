using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoGrafos.DataStructure
{
    /// <summary>
    /// Classe que representa um grafo.
    /// </summary>
    public class Graph
    {

        #region Atributos

        /// <summary>
        /// Lista de nós que compõe o grafo.
        /// </summary>
        private List<Node> nodes;

        #endregion

        #region Propridades

        /// <summary>
        /// Mostra todos os nós do grafo.
        /// </summary>
        public Node[] Nodes
        {
            get { return this.nodes.ToArray(); }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Cria nova instância do grafo.
        /// </summary>
        public Graph()
        {
            this.nodes = new List<Node>();
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Encontra o nó através do seu nome.
        /// </summary>
        /// <param name="name">O nome do nó.</param>
        /// <returns>O nó encontrado ou nulo caso não encontre nada.</returns>
        private Node Find(string name)
        {
            Node n = null;
            foreach (Node node in nodes)
            {
                if (node.Name == name)
                    n = node;
            }
            return n;
        }

        /// <summary>
        /// Adiciona um nó ao grafo.
        /// </summary>
        /// <param name="name">O nome do nó a ser adicionado.</param>
        /// <param name="info">A informação a ser armazenada no nó.</param>
        public void AddNode(string name)
        {
            AddNode(name, null);
        }

        /// <summary>
        /// Adiciona um nó ao grafo.
        /// </summary>
        /// <param name="name">O nome do nó a ser adicionado.</param>
        /// <param name="info">A informação a ser armazenada no nó.</param>
        public void AddNode(string name, object info)
        {
            nodes.Add(new Node(name, info));
        }

        /// <summary>
        /// Remove um nó do grafo.
        /// </summary>
        /// <param name="name">O nome do nó a ser removido.</param>
        public void RemoveNode(string name)
        {
            Node n = Find(name);
            if (n != null)
            {
                nodes.Remove(n);
                foreach (Node node in nodes)
                {
                    for (int i = 0; i < node.Edges.Count; i++)
                    {
                        Edge e = node.Edges[i];
                        if (e.To.Name == name)
                        {
                            node.Edges.Remove(e);
                            i--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adiciona o arco entre dois nós associando determinado custo.
        /// </summary>
        /// <param name="from">O nó de origem.</param>
        /// <param name="to">O nó de destino.</param>
        /// <param name="cost">O cust associado.</param>
        public void AddEdge(string from, string to, double cost)
        {
            Node nFrom = Find(from);
            Node nTo = Find(to);
            if (nFrom != null && nTo != null)
            {
                nFrom.AddEdge(nTo, cost);
                //nTo.AddEdge(nFrom, cost);
            }
        }

        /// <summary>
        /// Obtem todos os nós vizinhos de determinado nó.
        /// </summary>
        /// <param name="node">O nó origem.</param>
        /// <returns></returns>
        public Node[] GetNeighbours(string from)
        {
            Node n = Find(from);
            Node[] neighbours = null;
            if (n != null)
            {
                neighbours = new Node[n.Edges.Count];
                int i = 0;
                foreach (Edge e in n.Edges)
                {
                    neighbours[i++] = e.To;
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Valida um caminho, retornando a lista de nós pelos quais ele passou.
        /// </summary>
        /// <param name="nodes">A lista de nós por onde passou.</param>
        /// <param name="path">O nome de cada nó na ordem que devem ser encontrados.</param>
        /// <returns></returns>
        public bool IsValidPath(ref Node[] nodes, params string[] path)
        {
            bool valid = true;
            if (path.Length > 0)
            {
                List<Node> pathNodes = new List<Node>();
                Node n = Find(path[0]);
                if (n == null)
                    return false;
                for (int i = 1; i < path.Length; i++)
                {
                    Node[] neighbours = GetNeighbours(n.Name);
                    foreach (Node node in neighbours)
                    {
                        if (node.Name == path[i])
                            n = node;
                    }
                    if (n.Name != path[i])
                        return false;
                }
            }
            return valid;
        }

        public Graph Prim()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            int index = rnd.Next(this.nodes.Count);
            return Prim(this.nodes[index].Name);
        }

        public Graph Kruskal()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            int index = rnd.Next(this.nodes.Count);
            return Kruskal(this.nodes[index].Name);
        }

        #region Prim

        public Graph Prim(string name)
        {
            Graph gTree = new Graph();

            gTree.AddNode(Find(name).Name);

            do
            {
                for (int i = 0; i < gTree.nodes.Count; i++)
                {
                    Node node = Find(gTree.nodes[i].Name);
                    foreach (Edge e in node.Edges)
                    {
                        if (e.To.Visited != true)
                        {
                            e.To.Visited = true;
                            if (gTree.Find(e.To.Name) != null)
                            {
                                gTree.AddNode(e.To.Name);
                                AddEdge(gTree.nodes[i], e, gTree);
                            }

                        }
                    }
                }

            } while (gTree.nodes.Count != nodes.Count);

            return gTree;
        }

        private void AddEdge(Node nFrom, Edge eTo, Graph gTree)
        {
            Node nTo = gTree.Find(eTo.To.Name);
            Edge eHelp = GetEdge(nTo, gTree);

            if (eHelp == null)
            {
                gTree.Find(nFrom.Name).AddEdge(nTo, eTo.Cost);
            }
            else
            {
                if (eHelp.Cost > eTo.Cost)
                {
                    gTree.Find(nFrom.Name).AddEdge(nTo, eTo.Cost);
                }
            }
        }

        private Edge GetEdge(Node nTo, Graph gTree)
        {
            foreach (var n in gTree.nodes)
            {
                foreach (var e in n.Edges)
                {
                    if (e.To.Name == nTo.Name)
                    {
                        return e;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Kruskal

        public Graph Kruskal(string name)
        {
            Graph gTree = new Graph();
            List<Edge> eList = new List<Edge>();

            foreach (Node n in nodes)
            {
                gTree.AddNode(n.Name, n.Info);
                eList.AddRange(n.Edges);
            }

            eList = eList.OrderBy(c => c.Cost).ToList();

            foreach (Edge e in eList)
            {
                if (!ExistEdge(gTree.Find(e.To.Name), gTree))
                {
                    gTree.Find(e.From.Name).AddEdge(e.To, e.Cost);
                }
            }

            return gTree;
        }

        private bool ExistEdge(Node nTo, Graph gTree)
        {
            foreach (var n in gTree.nodes)
            {
                foreach (var e in n.Edges)
                {
                    if (e.To.Name == nTo.Name)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #endregion

    }
}
