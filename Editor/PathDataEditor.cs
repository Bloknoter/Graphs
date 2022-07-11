using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Graphs;

[CustomEditor(typeof(PathData))]
public class PathDataEditor : Editor
{

    private SerializedProperty graphProperty;

    private int fisrtver;

    private int secondver;

    private void OnEnable()
    {
        graphProperty = serializedObject.FindProperty("path");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if(graphProperty.objectReferenceValue == null)
        {
            Debug.LogWarning("Cannot find graph data, creating new...");

            graphProperty.objectReferenceValue = CreateInstance<Graph>();
        }

        Graph graph = (Graph)graphProperty.objectReferenceValue;

        EditorGUILayout.LabelField($"{graphProperty.displayName} graph");

        EditorGUILayout.LabelField($"Vertexes: {graph.vertexesCount}");

        EditorGUILayout.LabelField($"Edges: {graph.edgesCount}");

        EditorGUILayout.LabelField($"Edges' instanses: {graph.edgesInstancesCount} , must be: {(graph.vertexesCount * graph.vertexesCount - graph.vertexesCount) / 2}");

        Graph.GraphState graphState = graph.GetGraphState();

        EditorGUILayout.BeginHorizontal();

        if(graphState != Graph.GraphState.Normal)
        {
            EditorGUILayout.HelpBox($"Graph state: {graphState}", MessageType.Error);

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Fix..."))
            {
                graph.FixProblems();
            }

            if (GUILayout.Button("Debug"))
            {
                graph.DebugAll();
            }

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.HelpBox($"Graph state: {graphState}", MessageType.Info);

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Check weights...")) 
            {
                graph.FixProblems();
            }

            if (GUILayout.Button("Debug"))
            {
                graph.DebugAll();
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        fisrtver = EditorGUILayout.IntField("Start ID", fisrtver);

        secondver = EditorGUILayout.IntField("Destination ID", secondver);

        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Find path"))
        {
            int[] path = graph.FindPathWithDebug(fisrtver, secondver).path;
            string pathresult = "Path: ";
            for(int i = 0; i < path.Length;i++)
            {
                pathresult += $"{path[i] + 1} ";
            }
            Debug.Log(pathresult);
        }

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(target);

    }

    private int editedvertex = -1;

    private int editededge = 0;

    private void OnSceneGUI()
    {
        PathData pathData = (PathData)target;

        if (pathData.path != null)
        {

            for (int i = 0; i < pathData.path.vertexesCount; i++)
            {
                bool changed = false;
                Vertex vertex = pathData.path.GetVertexbyID(i);
                if (editedvertex == i)
                {
                    Vector2 vec = Handles.PositionHandle(new Vector3(vertex.position.x, vertex.position.y, 0), Quaternion.identity);
                    if (vertex.position != vec)
                        changed = true;
                    vertex.position = vec;
                }
                Handles.Label(new Vector3(vertex.position.x, vertex.position.y, 0), $"vertex {i + 1}");
                if(Handles.Button(new Vector3(vertex.position.x - 0.5f, vertex.position.y - 0.5f, 0), Quaternion.identity, 1f, 0.5f, Handles.SphereHandleCap))
                {
                    editedvertex = i;
                    editededge = 0;
                    if(editedvertex == editededge)
                    {
                        editededge++;
                    }
                }
                if (editedvertex != -1 && pathData.path.vertexesCount > 1)
                {
                    if (pathData.path.GetEdgebyID(editedvertex, editededge).IsInitialized && changed)
                    {
                        pathData.path.SetEdge(editedvertex, editededge, Vector2.Distance(pathData.path.GetVertexbyID(editedvertex).position, pathData.path.GetVertexbyID(editededge).position));
                    }
                }
            }

            List<List<bool>> map = pathData.path.GetGraphMap();

            for(int i = 0; i < map.Count;i++)
            {
                for(int d = 0; d < map[i].Count;d++)
                {
                    if(map[i][d])
                    {
                        Vector2 firstpoint = pathData.path.GetVertexbyID(i + 1).position;
                        Vector2 secondpoint = pathData.path.GetVertexbyID(d).position;

                        Handles.DrawLine(firstpoint, secondpoint);
                    }
                }
            }

            GUILayout.BeginArea(new Rect(10, 10, 200, 270));

            EditorGUILayout.BeginVertical("Tooltip");

            if (GUILayout.Button("Add vertex", GUILayout.Width(120)))
            {
                pathData.path.AddVertex();
                pathData.path.GetVertexbyID(pathData.path.vertexesCount - 1).position = HandleUtility.GUIPointToWorldRay(SceneView.mouseOverWindow.position.size / 2f).origin;

                editedvertex = pathData.path.vertexesCount - 1;
                editededge = 0;
            }

            EditorGUILayout.Separator();

            if (pathData.path.vertexesCount == 0)
            {
                EditorGUILayout.LabelField("Graph is empty");
            }
            else if(editedvertex == -1)
            {
                EditorGUILayout.LabelField("No vertex selected");
            }
            else if(pathData.path.vertexesCount == 1)
            {
                EditorGUILayout.LabelField("Only 1 vertex detected");
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Vertex: 1", GUILayout.Width(80));

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    pathData.path.DeleteVertex(0);
                    editedvertex = -1;
                    editededge = 0;
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Vertex: {editedvertex + 1}", GUILayout.Width(80));

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    pathData.path.DeleteVertex(editedvertex);
                    editedvertex = -1;
                    editededge = 0;
                    return;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Edge to ", GUILayout.Width(50));

                if (GUILayout.Button("<", GUILayout.Width(18)))
                {
                    int tryingvalue = editededge;
                    if (editededge == 0)
                    {
                        tryingvalue = pathData.path.vertexesCount - 1;
                    }
                    else
                    {
                        tryingvalue--;
                    }
                    if(tryingvalue == editedvertex)
                    {
                        if (tryingvalue == 0)
                        {
                            tryingvalue = pathData.path.vertexesCount - 1;
                        }
                        else
                        {
                            tryingvalue--;
                        }
                    }
                    editededge = tryingvalue;
                }

                EditorGUILayout.LabelField($"{editededge + 1}", GUILayout.Width(15));

                if (GUILayout.Button(">", GUILayout.Width(18)))
                {
                    int tryingvalue = editededge;
                    if (editededge == pathData.path.vertexesCount - 1)
                    {
                        tryingvalue = 0;
                    }
                    else
                    {
                        tryingvalue++;
                    }
                    if (tryingvalue == editedvertex)
                    {
                        if (tryingvalue == pathData.path.vertexesCount - 1)
                        {
                            tryingvalue = 0;
                        }
                        else
                        {
                            tryingvalue++;
                        }
                    }
                    editededge = tryingvalue;
                }

                EditorGUILayout.LabelField(" vertex");

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField($"Weight: {pathData.path.GetEdgebyID(editedvertex, editededge).Weight}");

                if (!pathData.path.GetEdgebyID(editedvertex, editededge).IsInitialized)
                {
                    if (GUILayout.Button("Add edge", GUILayout.Width(100)))
                    {
                        pathData.path.SetEdge(editedvertex, editededge, Vector2.Distance(pathData.path.GetVertexbyID(editedvertex).position, pathData.path.GetVertexbyID(editededge).position));
                    }
                }
                else
                {
                    if (GUILayout.Button("Remove edge", GUILayout.Width(110)))
                    {
                        pathData.path.SetEdge(editedvertex, editededge, -1);
                    }
                }

                pathData.path.GetEdgebyID(editedvertex, editededge).Multiplier = EditorGUILayout.FloatField($"Multiplier: ", pathData.path.GetEdgebyID(editedvertex, editededge).Multiplier);

            }

            EditorGUILayout.EndVertical();

            GUILayout.EndArea();
        }
    }
}
