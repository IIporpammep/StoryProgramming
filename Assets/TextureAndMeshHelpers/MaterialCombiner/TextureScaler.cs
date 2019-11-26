using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    /// <summary>
    /// Scales textures using GPU(Graphics.Blit)
    /// </summary>
    public class TextureScaler
    {
        public static void Scale(Texture2D texture2D, int sizeX, int sizeY, FilterMode filterMode = FilterMode.Bilinear)
        {
            texture2D.filterMode = filterMode;

            RenderTexture renderTexture = RenderTexture.GetTemporary(sizeX, sizeY);
            renderTexture.Create();
            renderTexture.filterMode = filterMode;

            RenderTexture previousRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Graphics.Blit(texture2D, renderTexture);
            texture2D.Resize(sizeX, sizeY);
            texture2D.ReadPixels(new Rect(0, 0, sizeX, sizeY), 0, 0);
            texture2D.Apply();

            RenderTexture.ReleaseTemporary(renderTexture);
            RenderTexture.active = previousRenderTexture;
        }
        public static Texture2D ScaleCopy(Texture2D texture2D, int sizeX, int sizeY, FilterMode filterMode = FilterMode.Bilinear)
        {
            Texture2D copyTexture = new Texture2D(texture2D.width, texture2D.height, texture2D.format, false);
            copyTexture.SetPixels32(texture2D.GetPixels32());
            copyTexture.Apply();
            Scale(copyTexture, sizeX, sizeY, filterMode);
            return copyTexture;
        }
    }
}
