using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StoryProgramming
{
    public class VATGenerator
    {
        const string SAVE_FOLDER = "/VertexAnimationTexture/Animations/";
        const string SAVE_FOLDER_TEXTURES = "/VertexAnimationTexture/Animations/Textures/";

        public void GenerateVAT(string targetName, int renderersCount, float duration, int frames, Bounds bounds, Bounds startBounds, List<Vector3>[] renderersPositions, List<Quaternion>[] renderersRotations)
        {
            EnsureFolders();
            WritePositionsTexture(targetName, renderersCount, frames, bounds, renderersPositions);
            WriteRotationsTexture(targetName, renderersCount, frames, renderersRotations);

            VATAnimation vatAnimation = ScriptableObject.CreateInstance<VATAnimation>();
            vatAnimation.RenderersCount = renderersCount;
            vatAnimation.Frames = frames;
            vatAnimation.Duration = duration;
            vatAnimation.BoundsCenter = bounds.center;
            vatAnimation.BoundsExtents = bounds.extents;
            vatAnimation.StartBoundsCenter = startBounds.center;
            vatAnimation.StartBoundsExtents = startBounds.extents;
            vatAnimation.PositionsTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/" + SAVE_FOLDER_TEXTURES + targetName + "_PositionTex.png", typeof(Texture2D));
            SetTextureSettings(vatAnimation.PositionsTex);
            vatAnimation.RotationsTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/" + SAVE_FOLDER_TEXTURES + targetName + "_RotationTex.png", typeof(Texture2D));
            SetTextureSettings(vatAnimation.RotationsTex);
            AssetDatabase.CreateAsset(vatAnimation, "Assets/" + SAVE_FOLDER + targetName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.LogError("Generation Finished");
        }

        void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/VertexAnimationTexture/Animations"))
            {
                AssetDatabase.CreateFolder("Assets/VertexAnimationTexture", "Animations");
            }
            if (!AssetDatabase.IsValidFolder("Assets/VertexAnimationTexture/Animations/Textures"))
            {
                AssetDatabase.CreateFolder("Assets/VertexAnimationTexture/Animations", "Textures");
            }
        }

        void WritePositionsTexture(string targetName, int renderersCount, int frames, Bounds bounds, List<Vector3>[] renderersPositions)
        {
            Texture2D positionsTex = new Texture2D(renderersCount, frames, TextureFormat.RGB24, false, true);
            for (int x = 0; x < renderersCount; x++)
            {
                for (int y = 0; y < frames; y++)
                {

                    Vector3 positionInBounds = renderersPositions[x][y] - bounds.center;
                    positionInBounds = new Vector3(Mathf.InverseLerp(-bounds.extents.x, bounds.extents.x, positionInBounds.x),
                                                   Mathf.InverseLerp(-bounds.extents.y, bounds.extents.y, positionInBounds.y),
                                                   Mathf.InverseLerp(-bounds.extents.z, bounds.extents.z, positionInBounds.z));

                    Color encodedPosition = new Color(positionInBounds.x, positionInBounds.y, positionInBounds.z, 1);
                    positionsTex.SetPixel(x, y, encodedPosition);
                }
            }
            positionsTex.Apply();
            var resultTextureBytes = positionsTex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + SAVE_FOLDER_TEXTURES + targetName + "_PositionTex.png", resultTextureBytes);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        void WriteRotationsTexture(string targetName, int renderersCount, int frames, List<Quaternion>[] renderersRotations)
        {
            Texture2D rotationTex = new Texture2D(renderersCount, frames, TextureFormat.RGBA32, false, true);
            for (int x = 0; x < renderersCount; x++)
            {
                for (int y = 0; y < frames; y++)
                {
                    //Debug.LogError(_renderersRotations[x][y].x + " " + _renderersRotations[x][y].y + " " + _renderersRotations[x][y].z + " " + _renderersRotations[x][y].w);

                    Color rotation = new Color(renderersRotations[x][y].x.Remap(-1, 1, 0, 1), renderersRotations[x][y].y.Remap(-1, 1, 0, 1), renderersRotations[x][y].z.Remap(-1, 1, 0, 1), renderersRotations[x][y].w.Remap(-1, 1, 0, 1));
                    //Vector3 euler = renderersRotations[x][y].eulerAngles;
                    //Color rotation = new Color(MathHelpers.AngleFrom0To360(euler.x) / 360f, MathHelpers.AngleFrom0To360(euler.y) / 360f, MathHelpers.AngleFrom0To360(euler.z) / 360f, 1);
                    rotationTex.SetPixel(x, y, rotation);
                }
            }
            rotationTex.Apply();
            var resultTextureBytes = rotationTex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + SAVE_FOLDER_TEXTURES + targetName + "_RotationTex.png", resultTextureBytes);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }


        void SetTextureSettings(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.sRGBTexture = false;
                textureImporter.isReadable = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                TextureImporterPlatformSettings textureImporterPlatformSettings = textureImporter.GetDefaultPlatformTextureSettings();
                textureImporterPlatformSettings.format = TextureImporterFormat.RGBA32;
                textureImporter.SetPlatformTextureSettings(textureImporterPlatformSettings);
                AssetDatabase.ImportAsset(assetPath);
                AssetDatabase.Refresh();
            }
        }
    }
}
