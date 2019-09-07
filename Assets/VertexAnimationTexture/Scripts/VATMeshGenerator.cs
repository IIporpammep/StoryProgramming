using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StoryProgramming
{
    /// <summary>
    /// Writes pivots and ids to all meshes and combines them to single mesh
    /// </summary>
    public class VATMeshGenerator
    {
        const string SAVE_FOLDER = "/VertexAnimationTexture/Animations/";
        const string SAVE_FOLDER_MESHES = "/VertexAnimationTexture/Animations/Meshes/";
        const string SAVE_FOLDER_PREFABS = "/VertexAnimationTexture/Animations/Prefabs/";
        const string SAVE_FOLDER_MATERIALS = "/VertexAnimationTexture/Animations/Materials/";

        /// <summary>
        /// Must be called before recording 
        /// </summary>
        /// <param name="targetRenderers"></param>
        public void GenerateMesh(string targetName, Renderer[] targetRenderers, Bounds startBounds)
        {
            if (targetRenderers.Length > 256)
            {
                Debug.LogError("Too much renderers, rendererID indexing only for 256.");
            }

            EnsureFolders();
            CombineInstance[] combine = new CombineInstance[targetRenderers.Length];
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                Mesh mesh = GameObject.Instantiate(targetRenderers[i].GetComponent<MeshFilter>().sharedMesh);

                List<Color> colors = new List<Color>();
                for (int q = 0; q < mesh.vertexCount; q++)
                {
                    Vector3 positionInBounds = targetRenderers[i].transform.position - startBounds.center;
                    positionInBounds = new Vector3(Mathf.InverseLerp(-startBounds.extents.x, startBounds.extents.x, positionInBounds.x),
                                                   Mathf.InverseLerp(-startBounds.extents.y, startBounds.extents.y, positionInBounds.y),
                                                   Mathf.InverseLerp(-startBounds.extents.z, startBounds.extents.z, positionInBounds.z));

                    float rendererID = i / (float)(targetRenderers.Length - 1);
                    Color encodedPosition = new Color(positionInBounds.x, positionInBounds.y, positionInBounds.z, rendererID);
                    colors.Add(encodedPosition);
                }

                mesh.SetColors(colors);
                combine[i].mesh = mesh;
                combine[i].transform = targetRenderers[i].transform.localToWorldMatrix;
            }
            SavePrefab(targetName, combine);
        }

        void SavePrefab(string targetName, CombineInstance[] combine)
        {
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            string path = "Assets/" + SAVE_FOLDER_MESHES + targetName + "_Mesh" + ".asset";
            AssetDatabase.CreateAsset(combinedMesh, path);

            GameObject combinedGO = new GameObject();
            MeshFilter meshFilter = combinedGO.AddComponent<MeshFilter>();
            meshFilter.mesh = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
            MeshRenderer meshRenderer = combinedGO.AddComponent<MeshRenderer>();

            Material material = new Material(Shader.Find("Shader Graphs/VATShader"));
            string matPath = "Assets/" + SAVE_FOLDER_MATERIALS + targetName + "_Mat" + ".mat";
            AssetDatabase.CreateAsset(material, matPath);
            meshRenderer.sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath(matPath, typeof(Material));

            combinedGO.AddComponent<VATGPUPlayer>();


            PrefabUtility.SaveAsPrefabAsset(combinedGO, "Assets" + SAVE_FOLDER_PREFABS + targetName + ".prefab");
            GameObject.Destroy(combinedGO);
        }

        void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/VertexAnimationTexture/Animations"))
            {
                AssetDatabase.CreateFolder("Assets/VertexAnimationTexture", "Animations");
            }
            if (!AssetDatabase.IsValidFolder("Assets/VertexAnimationTexture/Animations/Meshes"))
            {
                AssetDatabase.CreateFolder("Assets/VertexAnimationTexture/Animations", "Meshes");
            }
            if (!AssetDatabase.IsValidFolder("Assets/VertexAnimationTexture/Animations/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets/VertexAnimationTexture/Animations", "Prefabs");
            }
            if (!AssetDatabase.IsValidFolder("Assets/VertexAnimationTexture/Animations/Materials"))
            {
                AssetDatabase.CreateFolder("Assets/VertexAnimationTexture/Animations", "Materials");
            }
        }
    }
}
