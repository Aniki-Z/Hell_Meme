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
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Retro.OldFilms
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class OldFilms
  {
    /// <summary> Settings. </summary>
    [System.Serializable]
    public sealed class Settings
    {
#region Common settings.
      /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
      /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
      public float intensity = 1.0f;
#endregion

#region Old Films settings.
      /// <summary> Film manufacturer. Default Black & White. </summary>
      public Manufacturers manufacturer = Manufacturers.Black_white;

      /// <summary> Frame movement. Default (0.2, 0.2). </summary>
      public Vector2 moveFrame = Vector2.one * 0.2f;

      /// <summary> Frame jumping [0, 1]. Default 0.1. </summary>
      public float jumpFrame = 0.1f;

      /// <summary> Natural vignette [0, 2]. Default 0.2. </summary>
      public float vignette = 0.2f;

      /// <summary> Sepia tones [0, 1]. Default 0. </summary>
      public float sepia = 0.0f;
      
      /// <summary> Grain strength [0, 1]. Default 0.5. </summary>
      public float grain = 0.5f;

      /// <summary> Blink strength [0, 1]. Default 0. </summary>
      public float blinkStrength = 0.0f;

      /// <summary> Blink speed. Default 10. </summary>
      public float blinkSpeed = 10.0f;

      /// <summary> Film blotches [0 - 6]. Default 3. </summary>
      public int blotches = 3;

      /// <summary> Film blotch size [0 - 10]. Default 1. </summary>
      public float blotchSize = 1.0f;

      /// <summary> Film scratches [0.0 - 1]. Default 0.5. </summary>
      public float scratches = 0.5f;

      /// <summary> Film lines [0 - 8]. Default 4. </summary>
      public int lines = 4;

      /// <summary> Lines strength [0.0 - 1]. Default 0.25. </summary>
      public float linesStrength = 0.25f;
      
      /// <summary> Custom CDL values. Only with Manufacturers.Custom. </summary>
      public FilmCDL customCDL = new();
#endregion

#region Color settings.
      /// <summary> Brightness [-1, 1]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0, 10]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary>Gamma [0.1, 10]. Default 1. </summary>      
      public float gamma = 1.0f;

      /// <summary> The color wheel [0, 1]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>      
      public float saturation = 1.0f;
      #endregion

      #region Advanced settings.
      /// <summary> Does it affect the Scene View? </summary>
      public bool affectSceneView = false;

#if !UNITY_6000_0_OR_NEWER
      /// <summary> Enable render pass profiling. </summary>
      public bool enableProfiling = false;

      /// <summary> Filter mode. Default Bilinear. </summary>
      public FilterMode filterMode = FilterMode.Bilinear;
#endif

      /// <summary> Render pass injection. Default BeforeRenderingPostProcessing. </summary>
      public RenderPassEvent whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      #endregion

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        manufacturer = Manufacturers.Black_white;
        customCDL.slope = Vector3.one;
        customCDL.offset = Vector3.zero;
        customCDL.power = Vector3.one;
        customCDL.saturation = 1.0f;
        customCDL.contrast = 1.0f;
        customCDL.gamma = 1.0f;
        customCDL.filmContrast = false;
        moveFrame = Vector2.one * 0.2f;
        jumpFrame = 0.1f;
        vignette = 0.2f;
        sepia = 0.0f;
        grain = 0.5f;
        blinkStrength = 0.0f;
        blinkSpeed = 10.0f;
        blotches = 3;
        blotchSize = 1.0f;
        scratches = 0.5f;
        lines = 4;
        linesStrength = 0.25f;

        brightness = 0.0f;
        contrast = 1.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;

        affectSceneView = false;
#if !UNITY_6000_0_OR_NEWER
        enableProfiling = false;
        filterMode = FilterMode.Bilinear;
#endif
        whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      }
    }    
  }
}
