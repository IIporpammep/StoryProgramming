using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RayMarching.Basic
{
    public class RayMarchingBasicRenderPass : ScriptableRenderPass
    {
        private const string PROFILER_TAG = "RayMarchingBasicRenderPass";
        private static readonly int FrustumCornersVS = Shader.PropertyToID("_FrustumCornersVS");

        private RayMarchingBasicRenderFeature.Settings _settings;

        private readonly Mesh _mesh;
        private readonly Material _material;

        public RayMarchingBasicRenderPass(RayMarchingBasicRenderFeature.Settings settings)
        {
            _settings = settings;
            _material = new Material(Shader.Find("Unlit/RayMarchingBasic"));
            _mesh = GenerateQuad();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var commandBuffer = CommandBufferPool.Get(PROFILER_TAG);
            var camera = renderingData.cameraData.camera;

            EnsureMaterialUniforms(camera);
            commandBuffer.DrawMesh(_mesh, GetMeshTRS(camera), _material);

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        private Matrix4x4 GetMeshTRS(Camera camera)
        {
            GetFrustumSize(camera, out float frustumWidth, out float frustumHeight);
            Vector3 scale = new Vector3(frustumWidth, frustumHeight, 1);

            return Matrix4x4.TRS(camera.transform.position + camera.transform.forward * DistanceFromCamera(camera),
                camera.transform.rotation, scale);
        }

        private void EnsureMaterialUniforms(Camera camera)
        {
            Matrix4x4 frustumCornersVS = new Matrix4x4();
            Vector3[] frustumCornersVSVectors = new Vector3[4];

            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), DistanceFromCamera(camera),
                Camera.MonoOrStereoscopicEye.Mono, frustumCornersVSVectors);

            for (int i = 0; i < frustumCornersVSVectors.Length; i++)
            {
                frustumCornersVS.SetRow(i, camera.transform.TransformVector(frustumCornersVSVectors[i]));
            }

            _material.SetMatrix(FrustumCornersVS, frustumCornersVS);
        }

        private void GetFrustumSize(Camera camera, out float frustumWidth, out float frustumHeight)
        {
            frustumHeight = 2.0f * DistanceFromCamera(camera) * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            frustumWidth = frustumHeight * camera.aspect;
        }

        private float DistanceFromCamera(Camera camera)
        {
            return camera.nearClipPlane + 1;
        }

        private static Mesh GenerateQuad()
        {
            Vector3 centre = new Vector3(0.5f, 0.5f, 0);
            var vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0) - centre,
                new Vector3(1, 0, 0) - centre,
                new Vector3(0, 1, 0) - centre,
                new Vector3(1, 1, 0) - centre
            };

            var triangles = new int[6]
            {
                0, 2, 1,
                2, 3, 1
            };

            var mesh = new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
            };

            //Store frustum ray indexes in uv.z, the order of frustum corners is:
            //bottomLeft = 0
            //bottomRight = 3
            //topLeft = 1
            //topRight = 2
            var uv = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 3),
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 2)
            };

            mesh.SetUVs(0, uv);

            return mesh;
        }
    }
}