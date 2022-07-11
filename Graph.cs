using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    [System.Serializable]
    public class Graph : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<Vertex> vertexes;

        private List<List<Edge>> edges = new List<List<Edge>>();

        [SerializeField]
        private List<Edge> saveableedges;

        #region General Getters

        public int vertexesCount 
        { 
            get 
            { 
                return vertexes.Count; 
            } 
        }

        public int edgesCount 
        { 
            get 
            {
                int res = 0;
                for(int i = 0; i < edges.Count;i++)
                {
                    for (int d = 0; d < edges[i].Count; d++)
                    {
                        if (edges[i][d].IsInitialized)
                            res++;
                    }
                }
                return res;
            } 
        }

        public int edgesInstancesCount
        {
            get
            {
                int res = 0;
                for (int i = 0; i < edges.Count; i++)
                {
                    for (int d = 0; d < edges[i].Count; d++)
                    {
                        res++;
                    }
                }
                return res;
            }
        }

        public List<List<bool>> GetGraphMap()
        {
            List<List<bool>> result = new List<List<bool>>();

            for (int i = 0; i < edges.Count; i++)
            {
                result.Add(new List<bool>());
                for (int d = 0; d < edges[i].Count; d++)
                {
                    result[i].Add(edges[i][d].IsInitialized);

                }
            }
            return result;
        }

        #endregion

        #region Work with the graph

        public Vertex AddVertex()
        {
            Vertex newvertex = new Vertex();
            return AddVertex(newvertex);
        }

        public Vertex AddVertex(Vertex newvertex)
        {
            vertexes.Add(newvertex);

            if (vertexes.Count > 1)
            {
                edges.Add(new List<Edge>());
                for (int i = 0; i < vertexes.Count - 1; i++)
                {
                    Edge newedge = new Edge();
                    edges[edges.Count - 1].Add(newedge);
                }
            }
            
            return newvertex;
        }

        public void SetEdge(int firstvertex, int secondvertex, float newweight)
        {
            CheckVertex(firstvertex, "SetEdge");
            CheckVertex(secondvertex, "SetEdge");

            if(firstvertex == secondvertex)
            {
                return;
            }
            if(firstvertex > secondvertex)
            {
                firstvertex--;
                edges[firstvertex][secondvertex].Weight = newweight;
            }
            else
            {
                secondvertex--;
                edges[secondvertex][firstvertex].Weight = newweight;
            }
    
        }

        public void SetEdge(int firstvertex, int secondvertex, Edge edge)
        {
            CheckVertex(firstvertex, "SetEdge");
            CheckVertex(secondvertex, "SetEdge");

            if (firstvertex == secondvertex)
            {
                return;
            }

            if (firstvertex > secondvertex)
            {
                firstvertex--;
                edges[firstvertex][secondvertex] = edge;
            }
            else
            {
                secondvertex--;
                edges[secondvertex][firstvertex] = edge;
            }

        }

        public void DeleteVertex(int id)
        {
            CheckVertex(id, "DeleteVertex");

            vertexes.RemoveAt(id);

            if (id == 1)
            {

                edges.RemoveAt(0);

                for (int i = 0; i < vertexes.Count - 1; i++)
                {
                    edges[i].RemoveAt(id);
                }
                return;
            }

            if (id > 0)
            {
                
                edges.RemoveAt(id - 1);
                
                for (int i = id - 1; i < vertexes.Count - id + 1; i++)
                {
                    edges[i].RemoveAt(id);
                }
                return;
            }

            if (id == 0)
            {
                if (edges.Count > 0)
                {
                    edges.RemoveAt(0);
                    for (int i = 0; i < vertexes.Count - 1; i++)
                    {
                        edges[i].RemoveAt(0);
                    }
                }
            }  
        }

        public Vertex GetVertexbyID(int id)
        {
            CheckVertex(id, "GetVertexbyID");
            return vertexes[id];
        }

        public int GetIDofVertex(Vertex vertex)
        {
            for(int i = 0; i < vertexes.Count;i++)
            {
                if(vertexes[i] == vertex)
                {
                    return i;
                }
            }
            return -1;
        }

        public int[] GetVertexesIDbyType(Vertex.VertexType vertexType)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i].vertexType == vertexType)
                {
                    result.Add(i);
                }
            }
            return result.ToArray();
        }

        public int GetVertexIDbyType(Vertex.VertexType vertexType, int objectid)
        {
            for(int i = 0; i < vertexes.Count;i++)
            {
                if(vertexes[i].vertexType == vertexType && vertexes[i].ObjectID == objectid)
                {
                    return i;
                }
            }
            return -1;
        }

        public Edge GetEdgebyID(int firstvertex, int secondvertex)
        {
            CheckVertex(firstvertex, "GetEdgebyID");
            CheckVertex(secondvertex, "GetEdgebyID");

            if(firstvertex == secondvertex)
            {
                throw new System.Exception($"First and second vertexes are equal (to {firstvertex}). Edge does not exist");
            }
            
            if(firstvertex > secondvertex)
            {
                return edges[firstvertex - 1][secondvertex];
            }
            else
            {
                return edges[secondvertex - 1][firstvertex];
            }
        }

        public Vertex[] GetNeighbours(int vertex)
        {
            CheckVertex(vertex, "GetNeighbours");

            List<Vertex> neighbours = new List<Vertex>();

            for(int i = 0;i < vertexes.Count;i++)
            {
                if(i != vertex)
                {
                    if (GetEdgebyID(i, vertex).IsInitialized)
                        neighbours.Add(vertexes[i]);
                }
            }

            return neighbours.ToArray();
        }

        public int[] GetNeighboursIDs(int vertex)
        {
            CheckVertex(vertex, "GetNeighboursIDs");

            List<int> neighbours = new List<int>();

            for (int i = 0; i < vertexes.Count; i++)
            {
                if (i != vertex)
                {
                    if (GetEdgebyID(i, vertex).IsInitialized)
                        neighbours.Add(i);
                }
            }

            return neighbours.ToArray();
        }

        private void CheckVertex(int id, string methodname)
        {
            if (!(id >= 0 && id < vertexes.Count))
            {
                throw new System.Exception($"You are trying to get vertex of id = {id} but the amount of vertexes is {vertexes.Count}. Function: '{methodname}'");
            }
        }

        #endregion

        #region PathFinding Algorythms

        public Path FindPath(Vertex fisrtvertex, Vertex secondvertex)
        {
            int fisrt = 0;
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i] == fisrtvertex)
                {
                    fisrt = i;
                    break;
                }

            }
            int second = 0;
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i] == secondvertex)
                {
                    second = i;
                    break;
                }

            }
            return FindPath(fisrt, second);
        }

        public Path FindPath(int firstvertex, int secondvertex)
        {
            return Find_Path(firstvertex, secondvertex, false);
        }

        public Path FindPathWithDebug(Vertex fisrtvertex, Vertex secondvertex)
        {
            int fisrt = 0;
            for(int i = 0; i < vertexes.Count;i++)
            {
                if(vertexes[i] == fisrtvertex)
                {
                    fisrt = i;
                    break;
                }
                        
            }
            int second = 0;
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i] == secondvertex)
                {
                    second = i;
                    break;
                }

            }
            return FindPathWithDebug(fisrt, second);
        }

        public Path FindPathWithDebug(int firstvertex, int secondvertex)
        {
            return Find_Path(firstvertex, secondvertex, true);
        }

        private Path Find_Path(int firstvertex, int secondvertex, bool debugging)
        {
            CheckVertex(firstvertex, "Find_Path");
            CheckVertex(secondvertex, "Find_Path");

            if (debugging)
                Debug.Log($"FINDING PATH BETWEEN {firstvertex} AND {secondvertex}");

            if (firstvertex == secondvertex)
            {
                return new Path(new int[] { firstvertex }, 0);
            }

            float[] pathlabels = new float[vertexes.Count];
            int[] parentsid = new int[vertexes.Count];// Each element shows the previous vertex of the vertex of the same id
            bool[] visited = new bool[vertexes.Count];

            pathlabels[firstvertex] = 0;
            int currentverid = firstvertex;
            bool allisvisited = false;

            for (int i = 0; i < pathlabels.Length; i++)
            {
                pathlabels[i] = -1;
                parentsid[i] = -1;
            }

            do
            {

                // Getting neighbours of current vertex
                int[] neighbours = GetNeighboursIDs(currentverid);

                if (debugging)
                {
                    Debug.Log($"CURRENT VERTEX: {currentverid}, label: {pathlabels[currentverid]}, parent: {parentsid[currentverid]}");
                    Debug.Log($"Neighbours count: {neighbours.Length}");
                }


                // Setting the label of unvisited neighbours and finding the neighbour with the least label 
                for (int i = 0; i < neighbours.Length; i++)
                {
                    if (!visited[neighbours[i]] && (GetEdgebyID(currentverid, neighbours[i]).Weight + pathlabels[currentverid] < pathlabels[neighbours[i]] || pathlabels[neighbours[i]] == -1))
                    {
                        pathlabels[neighbours[i]] = GetEdgebyID(currentverid, neighbours[i]).Weight + pathlabels[currentverid];
                        parentsid[neighbours[i]] = currentverid;
                        if(debugging)
                        {
                            Debug.Log($"Setting new label to {neighbours[i]} : {GetEdgebyID(currentverid, neighbours[i]).Weight + pathlabels[currentverid]}; new parent: {currentverid}");
                        }
                    }
                }

                visited[currentverid] = true;

                // Now, we will find unvisited vertex with the least label
                int unvisitedvertexwithminimumlabel = -1;
                for (int i = 0; i < visited.Length; i++)
                {
                    if (!visited[i] && i != secondvertex && pathlabels[i] != -1)
                    {
                        if (pathlabels[secondvertex] == -1 || (pathlabels[secondvertex] > 0 && pathlabels[i] < pathlabels[secondvertex]))
                        {
                            if (unvisitedvertexwithminimumlabel == -1)
                            {
                                unvisitedvertexwithminimumlabel = i;
                            }
                            else
                            {
                                if (pathlabels[i] < pathlabels[unvisitedvertexwithminimumlabel])
                                {
                                    unvisitedvertexwithminimumlabel = i;
                                }
                            }
                        }
                    }
                }

                // If there are some unvisited vertex with the least label...
                if (unvisitedvertexwithminimumlabel != -1)
                {
                    // We will work with it in the next step of algorythm 
                    currentverid = unvisitedvertexwithminimumlabel;
                    if(debugging)
                    {
                        Debug.Log($"Found unvisited vertex with the smallest label: {unvisitedvertexwithminimumlabel}");
                    }
                }
                else
                {
                    // If no, algorythm checked all the vertexes and must be finished
                    allisvisited = true;
                    if (debugging)
                        Debug.Log("Unvisited vertex with the smallest label wasn't found");
                }

            } while (!allisvisited);

            if (debugging)
            {
                Debug.Log("The pathfinding part of function has been finished");
                Debug.Log("Constructing path...");
            }

            List<int> newpath = new List<int>();
            newpath.Add(secondvertex);
            currentverid = secondvertex;
            do//for(int i = 0; i < 10 || currentverid == firstvertex; i++)
            {
                if(debugging)
                {
                    Debug.Log($"Current vertex: {currentverid}, previous: {parentsid[currentverid]}");
                }
                newpath.Insert(0, parentsid[currentverid]);
                currentverid = parentsid[currentverid];
            } while (currentverid != firstvertex);
            if (debugging)
                Debug.Log($"Function has been completed. Total path length: {pathlabels[secondvertex]}");
            return new Path(newpath.ToArray(), pathlabels[secondvertex]);
        }

        #endregion

        #region Debug and Fixing propblems

        public enum GraphState { Normal, Line_Corrupted, Line__Elements_Corrupted }

        public GraphState GetGraphState()
        {
            if (vertexes.Count == 0)
                return GraphState.Normal;
            if (edges.Count != vertexesCount - 1)
                return GraphState.Line_Corrupted;
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i].Count != i + 1)
                    return GraphState.Line__Elements_Corrupted;
            }
            return GraphState.Normal;
        }

        public void FixProblems()
        {
            if(GetGraphState() != GraphState.Normal)
            {
                if(edges.Count > vertexesCount - 1)
                {
                    
                    for(int i = 0; i < edges.Count - (vertexesCount - 1);i++)
                    {
                        edges.RemoveAt(edges.Count - 1);
                    }
                }
                else if(edges.Count < vertexesCount - 1)
                {
                    for (int i = 0; i < (vertexesCount - 1) - edges.Count; i++)
                    {
                        edges.Add(new List<Edge>());
                        for(int d = 0; d < edges.Count;d++)
                        {
                            edges[edges.Count - 1].Add(new Edge());
                        }
                    }
                }

                for(int i = 0; i < edges.Count;i++)
                {
                    if(edges[i].Count > i + 1)
                    {
                        for (int d = 0; d < edges[i].Count - (i + 1); d++)
                        {
                            edges[i].RemoveAt(edges[i].Count - 1);
                        }
                    }
                    else if(i + 1 > edges[i].Count)
                    {
                        for (int d = 0; d < (i + 1) - edges[i].Count; d++)
                        {
                            edges[i].Add(new Edge());
                        }
                    }
                }
            }

            if (edges.Count > 0)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    for (int d = 0; d < edges[i].Count; d++)
                    {
                        if (edges[i][d].IsInitialized)
                        {
                            edges[i][d].Weight = Vector2.Distance(vertexes[i + 1].position, vertexes[d].position);
                        }
                    }
                }
            }
        }

        public void DebugAll()
        {
            if (edges.Count > 0)
            {
                Debug.Log("Debug information:  -1 - edge does not exist, > 0 - edge exists");
                for (int i = 0; i < edges.Count; i++)
                {
                    string line = $"Vertex: {i + 2}, Line: {i}, edges[i] count: {edges[i].Count} | ";
                    for (int d = 0; d < edges[i].Count; d++)
                    {
                        line += $" {edges[i][d].Weight} ";
                    }
                    Debug.Log(line);
                }
            }
            else
            {
                Debug.Log("The graph is empty");
            }
        }

        #endregion

        #region Serialization and Deserialization

        public void OnBeforeSerialize()
        {
            saveableedges.Clear();

            for(int i = 0; i < edges.Count;i++)
            {
                for (int d = 0; d < edges[i].Count; d++)
                {
                    saveableedges.Add(edges[i][d]);
                }
            }
        }

        public void OnAfterDeserialize() { }

        private void OnEnable()
        {
            if (vertexes == null)
            {
                vertexes = new List<Vertex>();
                Debug.LogWarning("The vertexes data of graph could not be found");
            }

            if (saveableedges != null)
            {
                int amount = 1;
                int delta = 2;
                if (saveableedges.Count > 0)
                {
                    edges.Add(new List<Edge>());
                    for (int i = 0; i < saveableedges.Count; i++)
                    {
                        if (i == amount)
                        {
                            amount += delta;
                            delta++;
                            edges.Add(new List<Edge>());
                        }
                        edges[edges.Count - 1].Add(saveableedges[i]);
                    }
                }
            }
            else
            {
                saveableedges = new List<Edge>();
                Debug.LogWarning("The edges data of graph could not be found");
            }
        }

        #endregion
    }

    public class Path
    {
        public int[] path;
        public float Length { get; private set; }
        public Path(int[] vertexes, float length)
        {
            path = vertexes;
            Length = length;
        }
    }

    [System.Serializable]
    public class Vertex 
    {
        public Vector2 position;
    }

    [System.Serializable]
    public class Edge
    {
        [SerializeField]
        private float weight;

        [SerializeField]
        private float multiplier;

        public float Weight
        {
            get 
            {
                if (weight > 0)
                    return weight * multiplier;
                else
                    return -1;
            }
            set
            {
                if(value > 0)
                {
                    weight = value;
                }
                else
                {
                    weight = -1;
                }
            }
        }

        public float Multiplier
        {
            get { return multiplier; }
            set
            {
                if (value > 0)
                {
                    multiplier = value;
                }
                else
                {
                    multiplier = 1;
                }
            }
        }

        public bool IsInitialized
        {
            get { return weight > 0; }
        }

        public Edge()
        {
            weight = -1;
        }

        public Edge(float _weight)
        {
            weight = _weight;
        }

    }
}
