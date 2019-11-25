using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
/// <summary>
/// Adds Barycentric coords for selected UV, they used by wireframe shader
/// </summary>
public class WireframeMeshGenerator : EditorWindow
{
    Object _originalMeshObject;
    string _outputPath = "Assets/Meshes/";
    enum UsedUV
    {
        UV1,
        UV2,
        UV3,
        UV4,
    }
    UsedUV _usedUV = UsedUV.UV4;


    [MenuItem("Meshes/Wireframe Mesh Generator")]
    public static void OpenWindow()
    {
        GetWindow<WireframeMeshGenerator>("Wireframe Mesh Generator");
    }

    void OnGUI()
    {
        _originalMeshObject = EditorGUILayout.ObjectField(_originalMeshObject, typeof(Mesh), true);
        _outputPath = EditorGUILayout.TextField("Output path", _outputPath);
        _usedUV = (UsedUV)EditorGUILayout.EnumPopup("UsedUV", _usedUV);

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate"))
        {
            Mesh originalMesh = _originalMeshObject as Mesh;
            if (originalMesh != null)
            {
                Mesh wireFrameMesh = CreateWireframeMesh(originalMesh);
                string path = GetMeshPath(originalMesh.name, wireFrameMesh);
                SaveMesh(path, wireFrameMesh);
            }
        }
    }

    Mesh CreateWireframeMesh(Mesh mesh)
    {

        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector4[] tangents = mesh.tangents;
        Color[] colors = mesh.colors;
        Vector2[] uv = mesh.uv;
        Vector2[] uv2 = mesh.uv2;
        Vector2[] uv3 = mesh.uv3;
        Vector2[] uv4 = mesh.uv4;
        

        List<int> wireframeTriangles = new List<int>();
        List<Vector3> wireframeVertices = new List<Vector3>();
        List<Vector3> wireframeNormals = new List<Vector3>();
        List<Vector4> wireframeTangents = new List<Vector4>();
        List<Color> wireframeColors = new List<Color>();
        List<Vector2> wireframeUV = new List<Vector2>();
        List<Vector2> wireframeUV2 = new List<Vector2>();
        List<Vector2> wireframeUV3 = new List<Vector2>();
        List<Vector2> wireframeUV4 = new List<Vector2>();
               

        List<Vector3> barycentricCoords = new List<Vector3>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            wireframeTriangles.Add(i);
            wireframeTriangles.Add(i + 1);
            wireframeTriangles.Add(i + 2);

            barycentricCoords.Add(new Vector3(1, 0, 0));
            barycentricCoords.Add(new Vector3(0, 1, 0));
            barycentricCoords.Add(new Vector3(0, 0, 1));

            wireframeVertices.Add(vertices[triangles[i]]);
            wireframeVertices.Add(vertices[triangles[i + 1]]);
            wireframeVertices.Add(vertices[triangles[i + 2]]);

            wireframeNormals.Add(normals[triangles[i]]);
            wireframeNormals.Add(normals[triangles[i + 1]]);
            wireframeNormals.Add(normals[triangles[i + 2]]);

            wireframeTangents.Add(tangents[triangles[i]]);
            wireframeTangents.Add(tangents[triangles[i + 1]]);
            wireframeTangents.Add(tangents[triangles[i + 2]]);

            if (colors.Length > 0)
            {
                wireframeColors.Add(colors[triangles[i]]);
                wireframeColors.Add(colors[triangles[i + 1]]);
                wireframeColors.Add(colors[triangles[i + 2]]);
            }

            wireframeUV.Add(uv[triangles[i]]);
            wireframeUV.Add(uv[triangles[i + 1]]);
            wireframeUV.Add(uv[triangles[i + 2]]);

            if (uv2.Length > 0)
            {
                wireframeUV2.Add(uv2[triangles[i]]);
                wireframeUV2.Add(uv2[triangles[i + 1]]);
                wireframeUV2.Add(uv2[triangles[i + 2]]);
            }

            if (uv3.Length > 0)
            {
                wireframeUV3.Add(uv3[triangles[i]]);
                wireframeUV3.Add(uv3[triangles[i + 1]]);
                wireframeUV3.Add(uv3[triangles[i + 2]]);
            }

            if (uv4.Length > 0)
            {
                wireframeUV4.Add(uv4[triangles[i]]);
                wireframeUV4.Add(uv4[triangles[i + 1]]);
                wireframeUV4.Add(uv4[triangles[i + 2]]);
            }
        }

        Mesh wireMesh = new Mesh()
        {
            vertices = wireframeVertices.ToArray(),
            uv = wireframeUV.ToArray(),
            uv2 = wireframeUV2.ToArray(),
            uv3 = wireframeUV3.ToArray(),
            uv4 = wireframeUV4.ToArray(),
            colors = wireframeColors.ToArray(),
            normals = wireframeNormals.ToArray(),
            tangents = wireframeTangents.ToArray(),
            triangles = wireframeTriangles.ToArray()
        };


        wireMesh.SetUVs((int)_usedUV, barycentricCoords);
        return wireMesh;
    }

    Mesh ChangeUV3(Mesh mesh, float value)
    {
        Mesh result = Instantiate(mesh);

        Vector2[] uv3 = new Vector2[mesh.vertexCount];
        for (int i = 0; i < uv3.Length; i++)
        {
            uv3[i] = new Vector2(value, value);
        }

        result.uv3 = uv3;
        return result;
    }
    void SaveMesh(string path, Mesh mesh)
    {
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    string GetMeshPath(string name, Mesh mesh)
    {
        return _outputPath + "WireFrame_" + _usedUV.ToString() + " " + name + mesh.GetInstanceID() + ".asset";
    }
}