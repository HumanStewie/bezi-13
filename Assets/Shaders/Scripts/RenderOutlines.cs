using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Experimental.Rendering.Universal
{

    [ExcludeFromPreset]
    public class RenderOutlines : ScriptableRendererFeature
    {
        [System.Serializable]
        public class RenderObjectsSettings
        {
            public string passTag = "RenderOutlinesFeature";
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

            public RenderQueueType renderQueueType = RenderQueueType.Opaque;
            public LayerMask layerMask;
        }

        public RenderObjectsSettings settings = new();

        RenderOutlinePass renderOutlinePass;

        public override void Create()
        {
            if (settings.Event < RenderPassEvent.BeforeRenderingPrePasses)
                settings.Event = RenderPassEvent.BeforeRenderingPrePasses;

            renderOutlinePass = new RenderOutlinePass(settings.passTag, settings.Event,
                settings.renderQueueType, settings.layerMask);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderOutlinePass.ConfigureInput(ScriptableRenderPassInput.None);
            renderer.EnqueuePass(renderOutlinePass);
        }
    }
}

