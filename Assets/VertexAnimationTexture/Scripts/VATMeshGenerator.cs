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

        public void SetMeshBounds(string targetName, Bounds bounds)
        {
            Mesh mesh = (Mesh)AssetDatabase.LoadAssetAtPath(GetMeshPath(targetName), typeof(Mesh));
            mesh.bounds = bounds;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void SetAnimationToPrefab(string targetName, VATAnimation vatAnimation)
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(GetPrefabPath(targetName), typeof(GameObject));
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.GetComponent<VATGPUPlayer>().SetAnimation(vatAnimation);

            PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
            GameObject.Destroy(instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeGameObject = prefab;
            EditorGUIUtility.PingObject(prefab);
        }


        /// <summary>
        /// Must be called before recording 
        /// </summary>
        /// <param name="targetRenderers"></param>
        public void GenerateMesh(string targetName, Renderer[] targetRenderers, Bounds startBounds)
        {
            bool useUV3ForIds = targetRenderers.Length > 256;

            EnsureFolders();
            CombineInstance[] combine = new CombineInstance[targetRenderers.Length];
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                Mesh mesh = GameObject.Instantiate(targetRenderers[i].GetComponent<MeshFilter>().sharedMesh);

                List<Color> colors = new List<Color>();
                List<Vector2> uv3 = new List<Vector2>();

                //Store pivots in colors of mesh to be able to rotate mesh.
                Vector3 pivotPositionInBounds = targetRenderers[i].transform.position - startBounds.center;
                pivotPositionInBounds = new Vector3(Mathf.InverseLerp(-startBounds.extents.x, startBounds.extents.x, pivotPositionInBounds.x),
                                               Mathf.InverseLerp(-startBounds.extents.y, startBounds.extents.y, pivotPositionInBounds.y),
                                               Mathf.InverseLerp(-startBounds.extents.z, startBounds.extents.z, pivotPositionInBounds.z));

                float rendererID = i / (float)(targetRenderers.Length - 1); //to indexate columns in VAT
                Color encodedPosition = new Color(pivotPositionInBounds.x, pivotPositionInBounds.y, pivotPositionInBounds.z, (useUV3ForIds) ? 1 : rendererID);

                Vector2 rendererIDasRG = MathHelpers.EncodeFloatRG(i / (float)(targetRenderers.Length));
                for (int q = 0; q < mesh.vertexCount; q++)
                {
                    colors.Add(encodedPosition);
                    if (useUV3ForIds)
                    {
                        uv3.Add(rendererIDasRG);//Do not substract 1 from targetRenderers.Length cause EncodeFloatRG can't encode 1
                    }
                }

                mesh.SetColors(colors);
                if (useUV3ForIds)
                {
                    mesh.SetUVs(2, uv3);
                }
                combine[i].mesh = mesh;

                Matrix4x4 localToWorlds = targetRenderers[i].transform.localToWorldMatrix;
                //Remove rotation from baking into mesh, because rotation encoded into rotation texure.
                Vector3 position = localToWorlds.GetColumn(3);
                Vector3 scale = new Vector3(localToWorlds.GetColumn(0).magnitude, localToWorlds.GetColumn(1).magnitude, localToWorlds.GetColumn(2).magnitude);
                Matrix4x4 trsWithOutRotation = Matrix4x4.TRS(position, Quaternion.identity, scale);
                combine[i].transform = trsWithOutRotation;
            }
            SavePrefab(targetName, combine);
        }

        void SavePrefab(string targetName, CombineInstance[] combine)
        {
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            string path = GetMeshPath(targetName);
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


            PrefabUtility.SaveAsPrefabAsset(combinedGO, GetPrefabPath(targetName));
            GameObject.Destroy(combinedGO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string GetPrefabPath(string targetName)
        {
            return "Assets" + SAVE_FOLDER_PREFABS + targetName + ".prefab";
        }

        string GetMeshPath(string targetName)
        {
            return "Assets/" + SAVE_FOLDER_MESHES + targetName + "_Mesh" + ".asset";
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
