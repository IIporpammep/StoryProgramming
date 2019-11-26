using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
namespace StoryProgramming
{
    [Serializable]
    [PostProcess(typeof(PostEffectGridRenderer), PostProcessEvent.AfterStack, "Custom/PostEffectGrid")]
    public sealed class PostEffectGrid : PostProcessEffectSettings
    {
        [Range(0f, 1f), Tooltip("Grid effect intensity.")]
        public FloatParameter Blend = new FloatParameter { value = 0.5f };
        [Range(0.01f, 10f), Tooltip("Grid distance btw lines.")]
        public FloatParameter DistanceBetweenLines = new FloatParameter { value = 5f };
        [Range(0.01f, 1f), Tooltip("Grid lines width.")]
        public FloatParameter LinesWidth = new FloatParameter { value = 0.5f };
        [Tooltip("Grid lines color.")]
        public ColorParameter LinesColor = new ColorParameter { value = Color.red };
    }

    public sealed class PostEffectGridRenderer : PostProcessEffectRenderer<PostEffectGrid>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/PostEffectGridShader"));
            sheet.properties.SetFloat("_Blend", settings.Blend);
            sheet.properties.SetFloat("_DistanceBtwLines", settings.DistanceBetweenLines);
            sheet.properties.SetFloat("_LinesWidth", settings.LinesWidth);
            sheet.properties.SetColor("_LinesColor", settings.LinesColor);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}