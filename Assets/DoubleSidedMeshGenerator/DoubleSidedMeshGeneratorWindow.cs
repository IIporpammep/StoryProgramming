using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace StoryProgramming
{
    /// <summary>
    /// Can generate double-sided mesh from given, useful for double sided rendering
    /// </summary>
    public class DoubleSidedMeshGeneratorWindow : EditorWindow
    {
        Object _originalMeshObject;
        string _outputPath = "Assets/Meshes/";
        enum CombineOrder
        {
            InvertedThenOriginal,
            OriginalThenInverted
        }
        CombineOrder _combineOrder;


        [MenuItem("Meshes/Double-Sided Mesh Generator")]
        public static void OpenWindow()
        {
            GetWindow<DoubleSidedMeshGeneratorWindow>("Double-Sided Mesh Generator");
        }

        void OnGUI()
        {
            _originalMeshObject = EditorGUILayout.ObjectField(_originalMeshObject, typeof(Mesh), true);
            _outputPath = EditorGUILayout.TextField("Output path", _outputPath);
            _combineOrder = (CombineOrder)EditorGUILayout.EnumPopup("Combine order", _combineOrder);

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate inverted"))
            {
                Mesh originalMesh = _originalMeshObject as Mesh;
                if (originalMesh != null)
                {
                    Mesh invertedMesh = CreateInvertedMesh(originalMesh);
                    string path = GetInvertedMeshPath(originalMesh.name, invertedMesh);
                    SaveMesh(path, invertedMesh);
                }
            }

            if (GUILayout.Button("Generate double-sided"))
            {
                Mesh originalMesh = _originalMeshObject as Mesh;
                if (originalMesh != null)
                {
                    Mesh invertedMesh = CreateInvertedMesh(originalMesh);
                    Mesh combinedMesh = CombineMeshes(originalMesh, invertedMesh);
                    string path = GetDoubleSidedMeshPath(originalMesh.name, combinedMesh);
                    SaveMesh(path, combinedMesh);
                }
            }

            if (GUILayout.Button("Generate double-sided with UV3 = 1 for inverted mesh"))
            {
                Mesh originalMesh = _originalMeshObject as Mesh;
                if (originalMesh != null)
                {
                    Mesh invertedMesh = CreateInvertedMesh(originalMesh);
                    Mesh combinedMesh = CombineMeshes(ChangeUV3(originalMesh, 0), ChangeUV3(invertedMesh, 1));
                    string path = GetDoubleSidedMeshPath(originalMesh.name, combinedMesh, true);
                    SaveMesh(path, combinedMesh);
                }
            }
        }

        Mesh CreateInvertedMesh(Mesh mesh)
        {
            Vector3[] normals = mesh.normals;
            Vector3[] invertedNormals = new Vector3[normals.Length];
            for (int i = 0; i < invertedNormals.Length; i++)
            {
                invertedNormals[i] = -normals[i];
            }
            Vector4[] tangents = mesh.tangents;
            Vector4[] invertedTangents = new Vector4[tangents.Length];

            for (int i = 0; i < invertedTangents.Length; i++)
            {
                invertedTangents[i] = tangents[i];
                invertedTangents[i].w = -invertedTangents[i].w;
            }

            return new Mesh
            {
                vertices = mesh.vertices,
                uv = mesh.uv,
                normals = invertedNormals,
                tangents = invertedTangents,
                triangles = mesh.triangles.Reverse().ToArray()
            };
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

        Mesh CombineMeshes(Mesh mesh, Mesh invertedMesh)
        {
            CombineInstance[] combineInstancies = new CombineInstance[2]
            {
            new CombineInstance(){mesh = invertedMesh, transform = Matrix4x4.identity},
            new CombineInstance(){mesh = mesh, transform = Matrix4x4.identity}
            };

            if (_combineOrder == CombineOrder.OriginalThenInverted)
            {
                combineInstancies = combineInstancies.Reverse().ToArray();
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstancies);
            return combinedMesh;
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

        string GetDoubleSidedMeshPath(string name, Mesh mesh, bool uv3Modified = false)
        {
            string uv3 = (uv3Modified) ? "_UV3_" : "_";
            string order = (_combineOrder == CombineOrder.InvertedThenOriginal) ? "I_O_" : "O_I_";
            return _outputPath + "DoubleSidedMesh_" + name + uv3 + order + mesh.GetInstanceID() + ".asset";
        }

        string GetInvertedMeshPath(string name, Mesh mesh)
        {
            return _outputPath + "InvertedMesh_" + name + mesh.GetInstanceID() + ".asset";
        }
    }
}