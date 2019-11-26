#pragma warning disable CS0649 //hide never assigned SerializeField warnings
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace StoryProgramming
{
    /// <summary>
    /// Can combine several textures into one, with ability to select channels.
    /// </summary>
    [CreateAssetMenu(fileName = "TextureChannelCombinerData", menuName = "StoryProgramming/TextureChannelCombinerData", order = 1)]
    public class TextureChannelCombiner : ScriptableObject
    {
        [SerializeField]
        string _resultTextureName = "CombinedTexture";
        [SerializeField]
        List<TextureChanelPair> _textureChanelPairs;

        public enum TextureChannel
        {
            R, G, B, A
        }

        [Serializable]
        public class TextureChanelPair
        {
            public Texture2D Texture;
            public TextureChannel SourceChannel = TextureChannel.R;
            public TextureChannel TargetChannel = TextureChannel.R;
        }


        bool CanGenerateCombinedTexture()
        {
            if (_textureChanelPairs == null || _textureChanelPairs.Count == 0)
            {
                Debug.LogError("TextureChanelPairs not set.");
                return false;
            }
            List<int> usedTargetChannels = new List<int>();
            for (int i = 0; i < _textureChanelPairs.Count; i++)
            {
                if (usedTargetChannels.Contains((int)_textureChanelPairs[i].TargetChannel))
                {
                    Debug.LogError($"Target Channel {_textureChanelPairs[i].TargetChannel} used more than once.");
                    return false;
                }
                else
                {
                    usedTargetChannels.Add((int)_textureChanelPairs[i].TargetChannel);
                }
            }

            for (int i = 0; i < _textureChanelPairs.Count; i++)
            {
                if (_textureChanelPairs[i].Texture == null)
                {
                    Debug.LogError($"Texture with id {i} is null, you don't need to specify empty texture for not used channels.");
                    return false;
                }
            }
            int textureWidth = _textureChanelPairs[0].Texture.width;
            int textureHeight = _textureChanelPairs[0].Texture.height;
            for (int i = 1; i < _textureChanelPairs.Count; i++)
            {
                if (_textureChanelPairs[i].Texture.width != textureWidth || _textureChanelPairs[i].Texture.height != textureHeight)
                {
                    string errorText = "All textures must have the same sizes.\n";
                    for (int j = 0; j < _textureChanelPairs.Count; j++)
                    {
                        errorText += $" {_textureChanelPairs[i].Texture.name} W:{_textureChanelPairs[i].Texture.width} H: {_textureChanelPairs[i].Texture.height}\n";
                    }
                    Debug.LogError(errorText);
                    return false;
                }
            }

            return true;
        }

        public void Generate()
        {
            if (!CanGenerateCombinedTexture())
            {
                return;
            }

            int width = _textureChanelPairs[0].Texture.width;
            int height = _textureChanelPairs[0].Texture.height;

            for (int i = 0; i < _textureChanelPairs.Count; i++)
            {
                EnsureReadableTexuture(_textureChanelPairs[i].Texture);
            }

            Texture2D combinedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = MatchSourceChannelToTargetChannel(x, y);
                    combinedTexture.SetPixel(x, y, color);
                }
            }
            combinedTexture.Apply();
            var resultTextureBytes = combinedTexture.EncodeToPNG();
            string pathToCombineSOFolder = Application.dataPath + AssetDatabase.GetAssetPath(this).Replace("Assets", "").Replace(this.name, "").Replace(".asset", "");
            File.WriteAllBytes(pathToCombineSOFolder + _resultTextureName + ".png", resultTextureBytes);
            AssetDatabase.Refresh();
        }

        Color MatchSourceChannelToTargetChannel(int x, int y)
        {
            Color color = new Color();
            for (int i = 0; i < _textureChanelPairs.Count; i++)
            {
                float sourceValue = 0;
                switch (_textureChanelPairs[i].SourceChannel)
                {
                    case TextureChannel.R:
                        {
                            sourceValue = _textureChanelPairs[i].Texture.GetPixel(x, y).r;
                            break;
                        }
                    case TextureChannel.G:
                        {
                            sourceValue = _textureChanelPairs[i].Texture.GetPixel(x, y).g;
                            break;
                        }
                    case TextureChannel.B:
                        {
                            sourceValue = _textureChanelPairs[i].Texture.GetPixel(x, y).b;
                            break;
                        }
                    case TextureChannel.A:
                        {
                            sourceValue = _textureChanelPairs[i].Texture.GetPixel(x, y).a;
                            break;
                        }
                }
                switch (_textureChanelPairs[i].TargetChannel)
                {
                    case TextureChannel.R:
                        {
                            color.r = sourceValue;
                            break;
                        }
                    case TextureChannel.G:
                        {
                            color.g = sourceValue;
                            break;
                        }
                    case TextureChannel.B:
                        {
                            color.b = sourceValue;
                            break;
                        }
                    case TextureChannel.A:
                        {
                            color.a = sourceValue;
                            break;
                        }
                }
            }

            return color;
        }

        void EnsureReadableTexuture(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter != null && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(assetPath);
                AssetDatabase.Refresh();
            }
        }
    }
}