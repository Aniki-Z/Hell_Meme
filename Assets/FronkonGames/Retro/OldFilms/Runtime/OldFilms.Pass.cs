////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
#endif

namespace FronkonGames.Retro.OldFilms
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class OldFilms
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private readonly Settings settings;

#if UNITY_6000_0_OR_NEWER
#else
      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

      private readonly int renderTextureHandle0 = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH0");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private ProfilingScope profilingScope;
      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
#endif

      public static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");

        public static readonly int Slope = Shader.PropertyToID("_Slope");
        public static readonly int Offset = Shader.PropertyToID("_Offset");
        public static readonly int Power = Shader.PropertyToID("_Power");
        public static readonly int FilmSaturation = Shader.PropertyToID("_FilmSaturation");
        public static readonly int FilmContrast = Shader.PropertyToID("_FilmContrast");
        public static readonly int FilmGamma = Shader.PropertyToID("_FilmGamma");
        public static readonly int SuperContrast = Shader.PropertyToID("_SuperContrast");
        public static readonly int RandomValue = Shader.PropertyToID("_RandomValue");
        public static readonly int MoveFrame = Shader.PropertyToID("_MoveFrame");
        public static readonly int JumpFrame = Shader.PropertyToID("_JumpFrame");
        public static readonly int Vignette = Shader.PropertyToID("_Vignette");
        public static readonly int Sepia = Shader.PropertyToID("_Sepia");
        public static readonly int Grain = Shader.PropertyToID("_Grain");
        public static readonly int BlinkStrength = Shader.PropertyToID("_BlinkStrength");
        public static readonly int BlinkSpeed = Shader.PropertyToID("_BlinkSpeed");
        public static readonly int Blotches = Shader.PropertyToID("_Blotches");
        public static readonly int BlotchSize = Shader.PropertyToID("_BlotchSize");
        public static readonly int Scratches = Shader.PropertyToID("_Scratches");
        public static readonly int Lines = Shader.PropertyToID("_Lines");
        public static readonly int LinesStrength = Shader.PropertyToID("_LinesStrength");

        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings) : base()
      {
        this.settings = settings;
#if UNITY_6000_0_OR_NEWER
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
#endif
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);

        if (settings.manufacturer != Manufacturers.Custom)
        {
          material.SetVector(ShaderIDs.Slope, FilmCDL.Preset(settings.manufacturer).slope);
          material.SetVector(ShaderIDs.Offset, FilmCDL.Preset(settings.manufacturer).offset);
          material.SetVector(ShaderIDs.Power, FilmCDL.Preset(settings.manufacturer).power);
          material.SetFloat(ShaderIDs.FilmSaturation, FilmCDL.Preset(settings.manufacturer).saturation);
          material.SetFloat(ShaderIDs.FilmContrast, FilmCDL.Preset(settings.manufacturer).contrast);
          material.SetFloat(ShaderIDs.FilmGamma, FilmCDL.Preset(settings.manufacturer).gamma);
          material.SetInt(ShaderIDs.SuperContrast, FilmCDL.Preset(settings.manufacturer).filmContrast ? 1 : 0);
        }
        else
        {
          material.SetVector(ShaderIDs.Slope, settings.customCDL.slope);
          material.SetVector(ShaderIDs.Offset, settings.customCDL.offset);
          material.SetVector(ShaderIDs.Power, settings.customCDL.power);
          material.SetFloat(ShaderIDs.FilmSaturation, settings.customCDL.saturation);
          material.SetFloat(ShaderIDs.FilmContrast, settings.customCDL.contrast);
          material.SetFloat(ShaderIDs.FilmGamma, settings.customCDL.gamma);
          material.SetInt(ShaderIDs.SuperContrast, settings.customCDL.filmContrast ? 1 : 0);
        }

        material.SetVector(ShaderIDs.RandomValue, new Vector4(Random.value, Random.value, Random.value, Random.value));
        material.SetVector(ShaderIDs.MoveFrame, settings.moveFrame * 0.01f);
        material.SetFloat(ShaderIDs.JumpFrame, settings.jumpFrame * 0.1f);
        material.SetFloat(ShaderIDs.Grain, settings.grain);
        material.SetFloat(ShaderIDs.Sepia, settings.sepia);
        material.SetFloat(ShaderIDs.Vignette, settings.vignette);
        material.SetFloat(ShaderIDs.BlinkStrength, settings.blinkStrength);
        material.SetFloat(ShaderIDs.BlinkSpeed, settings.blinkSpeed);
        material.SetInt(ShaderIDs.Blotches, settings.blotches);
        material.SetFloat(ShaderIDs.BlotchSize, settings.blotchSize);
        material.SetFloat(ShaderIDs.Scratches, settings.scratches);
        material.SetInt(ShaderIDs.Lines, settings.lines);
        material.SetFloat(ShaderIDs.LinesStrength, settings.linesStrength / 8.0f);

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);
      }

#if UNITY_6000_0_OR_NEWER
      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        if (material == null || settings.intensity == 0.0f)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && settings.affectSceneView == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureHandle destination = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        UpdateMaterial();

        RenderGraphUtils.BlitMaterialParameters pass = new(source, destination, material, 0);
        renderGraph.AddBlitPass(pass, $"{Constants.Asset.AssemblyName}.Pass");

        resourceData.cameraColor = destination;
      }
#elif UNITY_2022_3_OR_NEWER
      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor, settings.filterMode);
      }

      /// <inheritdoc/>
      public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
      {
        if (material == null ||
            renderingData.postProcessingEnabled == false ||
            settings.intensity <= 0.0f ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
          profilingScope = new ProfilingScope(cmd, profilingSamples);

        UpdateMaterial();

        cmd.Blit(colorBuffer, renderTextureHandle0, material);
        cmd.Blit(renderTextureHandle0, colorBuffer);

        cmd.ReleaseTemporaryRT(renderTextureHandle0);

        if (settings.enableProfiling == true)
          profilingScope.Dispose();

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }

      public override void OnCameraCleanup(CommandBuffer cmd) => cmd.ReleaseTemporaryRT(renderTextureHandle0);
#else
      #error Unsupported Unity version. Please update to a newer version of Unity.
#endif
    }
  }
}
