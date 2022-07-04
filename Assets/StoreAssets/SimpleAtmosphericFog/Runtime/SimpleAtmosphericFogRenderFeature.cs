using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OG {
    public class SimpleAtmosphericFogRenderFeature : ScriptableRendererFeature {
        class CustomRenderPass : ScriptableRenderPass{
            
            static readonly int _FogDensity = Shader.PropertyToID("_FogDensity");
            static readonly int _FogPower = Shader.PropertyToID("_FogPower");
            static readonly int _SkyAlpha = Shader.PropertyToID("_SkyAlpha");
            static readonly int _FogColor = Shader.PropertyToID("_FogColor");

            static readonly int _UseHeight = Shader.PropertyToID("_UseHeight");
            static readonly int _BaseHeight = Shader.PropertyToID("_BaseHeight");
            static readonly int _MaxHeight = Shader.PropertyToID("_MaxHeight");
            static readonly int _HeightPower = Shader.PropertyToID("_HeightPower");
            
            static readonly int _SecondBaseHeight = Shader.PropertyToID("_SecondBaseHeight");
            static readonly int _SecondMaxHeight = Shader.PropertyToID("_SecondMaxHeight");
            static readonly int _SecondHeightPower = Shader.PropertyToID("_SecondHeightPower");

            static readonly int _DirectionalIntesity = Shader.PropertyToID("_DirectionalIntesity");
            static readonly int _UseMainLightColor = Shader.PropertyToID("_UseMainLightColor");
            static readonly int _DirectionalColor = Shader.PropertyToID("_DirectionalColor");
            static readonly int _DirectionalPower = Shader.PropertyToID("_DirectionalPower");

            public RenderTargetIdentifier source;

            RenderTargetHandle tempTexture;
            Material material;

            public CustomRenderPass(){
                Shader shader = Shader.Find("OG/SimpleAtmosphericFog");
                if(shader == null) return;
                material = CoreUtils.CreateEngineMaterial(shader);
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor){
                cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData){
                if (!renderingData.cameraData.postProcessEnabled) return;

                var stack = VolumeManager.instance.stack;
                var exponentialHeightFog = stack.GetComponent<SimpleAtmosphericFog>();
                if (exponentialHeightFog == null) { return; }
                if (exponentialHeightFog.IsActive() == false) { return; }

                if (material == null) return;
                if(renderingData.cameraData.cameraType != CameraType.Game && renderingData.cameraData.cameraType != CameraType.SceneView) return;

                CommandBuffer cmd = CommandBufferPool.Get();
                cmd.Clear();

                //Shader.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);
                material.SetMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);
                
                material.SetFloat(_FogDensity, (float) exponentialHeightFog.fogDensity);
                material.SetFloat(_FogPower, (float) exponentialHeightFog.fogPower);
                material.SetFloat(_SkyAlpha, (float) exponentialHeightFog.skyAlpha);
                material.SetColor(_FogColor, (Color) exponentialHeightFog.fogColor);

                material.SetFloat(_UseHeight, ((bool) exponentialHeightFog.useHeightFog) ? 1 : 0);
                material.SetFloat(_BaseHeight, (float) exponentialHeightFog.baseHeight);
                material.SetFloat(_MaxHeight, (float) exponentialHeightFog.maxHeight);
                material.SetFloat(_HeightPower, (float) exponentialHeightFog.heightPower);

                material.SetFloat(_SecondBaseHeight, (float) exponentialHeightFog.secondFogBaseHeight);
                material.SetFloat(_SecondMaxHeight, (float) exponentialHeightFog.secondMaxHeight);
                material.SetFloat(_SecondHeightPower, (float) exponentialHeightFog.secondFogHeightPower);

                material.SetFloat(_DirectionalIntesity, (float) exponentialHeightFog.directionalIntesity);
                material.SetInt(_UseMainLightColor, ((bool) exponentialHeightFog.useMainLightColor) ? 1 : 0);
                material.SetColor(_DirectionalColor, (Color) exponentialHeightFog.directionalColor);
                material.SetFloat(_DirectionalPower, (float) exponentialHeightFog.directionalPower);
                //

                cmd.Blit(source, tempTexture.Identifier(), material, 0);
                cmd.Blit(tempTexture.Identifier(), source);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd){}
        }
        //

        CustomRenderPass m_ScriptablePass;

        public override void Create(){
            m_ScriptablePass = new CustomRenderPass();
            m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData){
            m_ScriptablePass.source = renderer.cameraColorTarget;
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
