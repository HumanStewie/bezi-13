using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class RenderOutlinePass : ScriptableRenderPass
{
    static readonly int IdTexId = Shader.PropertyToID("_IdTex");
    static readonly int CombinedId = Shader.PropertyToID("_CombinedTex");

    RenderStateBlock m_RenderStateBlock;
    readonly ShaderTagId[] shaderTagIDs;
    readonly Material outline;
    readonly int layerMask;

    public RenderOutlinePass(string profilerTag, RenderPassEvent renderPassEvent, RenderQueueType renderQueueType, int layerMask)
    {
        outline = new Material(Shader.Find("Hidden/Outline"));
        profilingSampler = new ProfilingSampler(nameof(RenderObjectsPass));

        this.renderPassEvent = renderPassEvent;
        RenderQueueRange renderQueueRange = (renderQueueType == RenderQueueType.Transparent)
            ? RenderQueueRange.transparent
            : RenderQueueRange.opaque;

        this.layerMask = layerMask;

        shaderTagIDs = new ShaderTagId[]
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("LightweightForward")
        };

        m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;
        if((cameraData.camera.cullingMask & layerMask) == 0)
        {
            return;
        }

        int w = (int)(cameraData.camera.scaledPixelWidth * cameraData.renderScale);
        int h = (int)(cameraData.camera.scaledPixelHeight * cameraData.renderScale);

        CommandBuffer cmd = CommandBufferPool.Get("Render Outline Command Buffer");
        cmd.GetTemporaryRT(IdTexId, w, h, 16, FilterMode.Point, RenderTextureFormat.Default);
        cmd.GetTemporaryRT(CombinedId, w, h, 16, FilterMode.Point, RenderTextureFormat.Default);
        cmd.SetRenderTarget(IdTexId, 0);
        cmd.ClearRenderTarget(true, true, Color.black);
        cmd.SetGlobalFloat("_RenderID", 1);

        RendererListDesc rendererListDesc = new(shaderTagIDs, renderingData.cullResults, cameraData.camera)
        {
            layerMask = layerMask,
            rendererConfiguration = PerObjectData.None,
            sortingCriteria = SortingCriteria.CommonOpaque,
            stateBlock = m_RenderStateBlock,
            overrideMaterialPassIndex = 0,
            excludeObjectMotionVectors = false,
            renderQueueRange = RenderQueueRange.all
        };
        RendererList rendererList = context.CreateRendererList(rendererListDesc);

        cmd.DrawRendererList(rendererList);
        cmd.SetGlobalFloat("_RenderID", 0);
        cmd.SetGlobalTexture("_IdTex", IdTexId);
        cmd.SetGlobalTexture("_MainTex", cameraData.renderer.cameraColorTargetHandle);
        cmd.SetGlobalColor("_OutlineColor", ColorUtils.ToRGBA(0x252945).linear);
        cmd.Blit(cameraData.renderer.cameraColorTargetHandle, CombinedId, outline);
        cmd.Blit(CombinedId, cameraData.renderer.cameraColorTargetHandle);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}