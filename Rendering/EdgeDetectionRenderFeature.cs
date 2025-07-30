using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// ScriptableRendererFeature for post-process edge detection using a custom shader
public class EdgeDetectionFeature : ScriptableRendererFeature
{
    // Injects a render pass that applies edge detection to the camera output
    class EdgeDetectionPass : ScriptableRenderPass
    {
        private Material material;
        private RTHandle temporaryColorTexture;
        private ScriptableRenderer renderer;
        private RTHandle source;

        public EdgeDetectionPass(Material material)
        {
            this.material = material;
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        // Sets the source render target for the pass
        public void SetSource(RTHandle source)
        {
            this.source = source;
        }

        // Allocates a temporary texture for edge detection output
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref temporaryColorTexture, descriptor, name: "_TemporaryColorTexture");
        }

        // Executes the edge detection shader and blits results back to the camera target
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null || source == null)
                return;
            Debug.Log("EdgeDetection pass executing");
            CommandBuffer cmd = CommandBufferPool.Get("Edge Detection Pass");
            Blit(cmd, source, temporaryColorTexture, material); // Apply edge detection
            Blit(cmd, temporaryColorTexture, source);           // Output result to camera
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Releases the temporary texture after rendering
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (temporaryColorTexture != null)
            {
                temporaryColorTexture.Release();
            }
        }
    }

    public Shader shader;
    private Material material;
    private EdgeDetectionPass pass;

    // Creates the material and render pass for edge detection
    public override void Create()
    {
        if (shader == null)
        {
            return;
        }
        material = new Material(shader);
        pass = new EdgeDetectionPass(material);
    }

    // Adds the edge detection pass to the renderer
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null)
        {
            return;
        }
        pass.SetSource(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(pass);
    }

    // Cleans up the material when the feature is disposed
    protected override void Dispose(bool disposing)
    {
        if (disposing && material != null)
        {
            CoreUtils.Destroy(material);
        }
    }
}