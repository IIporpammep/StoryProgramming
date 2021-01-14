using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RayMarching.Basic
{
    public class RayMarchingBasicRenderFeature : ScriptableRendererFeature
    {
        public bool IsEnabled;

        [Serializable]
        public class Settings
        {
        }

        [SerializeField]
        private Settings _settings;

        private RayMarchingBasicRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new RayMarchingBasicRenderPass(_settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isSceneViewCamera || !Application.isPlaying || !IsEnabled) return;

            renderer.EnqueuePass(_renderPass);
        }
    }
}