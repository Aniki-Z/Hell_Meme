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
Shader "Hidden/Fronkon Games/Retro/Old Films URP"
{
  Properties
  {
    _MainTex("Main Texture", 2D) = "white" {}
  }

  SubShader
  {
    Tags
    {
      "RenderType" = "Opaque"
      "RenderPipeline" = "UniversalPipeline"
    }

    Pass
    {
      Name "Fronkon Games Retro Old Films"

      HLSLPROGRAM
      #include "Retro.hlsl"

      #pragma vertex RetroVert
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers gles d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      float3 _Slope;
      float3 _Offset;
      float3 _Power;
      float _FilmSaturation;
      float _FilmContrast;
      float _FilmGamma;
      int _SuperContrast;
      float4 _RandomValue;
      float2 _MoveFrame;
      float _JumpFrame;
      float _Vignette;
      float _Sepia;
      float _Grain;
      float _BlinkStrength;
      float _BlinkSpeed;
      int _Blotches;
      float _BlotchSize;
      int _Lines;
      float _LinesStrength;
      float _Scratches;

      inline float3 Saturation(const float3 pixel, const float adjustment)
      {
        const float3 W = float3(0.2126, 0.7152, 0.0722);

        return lerp((float3)dot(pixel, W), pixel, adjustment);
      }

      inline float3 Contrast(const float3 pixel, const float4 contrast)
      {
        float3 c = contrast.rgb * (float3)contrast.a;

        return (1.0 - c.rgb) * 0.5 + c.rgb * pixel;
      }

      inline float3 FilmContrast(const float3 contrast) 
      {
        return 1.0 / (1.0 + (exp(-(contrast - 0.5) * 7.0)));
      }

      inline float3 NaturalVignette(float3 pixel, const float2 uv)
      {
        const float2 coord = (uv - 0.5) * (_ScreenParams.x / _ScreenParams.y) * 2.0;

        const float rf = sqrt(dot(coord, coord)) * _Vignette;
        const float rf2_1 = rf * rf + 1.0;
        const float e = 1.0 / (rf2_1 * rf2_1);

        return pixel * e;
      }

      inline float FilmBlotch(const float seed, const float2 uv)
      {
        float x = Rand(seed);
        float y = Rand(seed + 1.0);
        float s = 0.01 * Rand(seed + 2.0);
	      
        float2 p = float2(x, y) - uv;
        p.x *= _ScreenParams.x / _ScreenParams.y;
	      
        const float a = atan2(p.y, p.x);
        const float ss = s * s * (sin(6.2831 * a * x) * 0.1 + _BlotchSize);

        float v = 0.2;
        
        UNITY_BRANCH
        if (dot(p, p) > ss)
         v = pow(abs(dot(p, p) - ss), 0.0625);

        return lerp(0.3 + 0.2 * (1.0 - (s / 0.02)), 1.0, v);
      }

      inline float FilmLine(const float seed, const float2 uv)
      {
        const float a = Rand(seed + 1.0);
        const float b = 0.01 * Rand(seed);
        const float c = Rand(seed + 2.0) - 0.5;
        const float mu = Rand(seed + 3.0);

        float l = 1.0;
        UNITY_BRANCH
        if (mu > 0.2)
          l = pow(abs(a * uv.x + b * uv.y + c ), _LinesStrength);
        else
          l = 2.0 - pow(abs(a * uv.x + b * uv.y + c), _LinesStrength);

        return lerp(0.5, 1.0, l);
      }

      float4 frag(const VertexOutput input) : SV_Target 
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        const float4 color = SAMPLE_MAIN(uv);
        float4 pixel = color;

        // Move frame.
        uv.x += _MoveFrame.x * (cos(_Time.y * 3.0) * sin(_Time.y * 12.0 + 0.25));
        uv.y += _MoveFrame.y * (sin(_Time.y * 1.0 + 0.5) * cos(_Time.y * 15.0 + 0.25));

        // Jump frame.
        uv += float2(min(frac(Rand(_Time.y)) - _JumpFrame, 0.0), 0.0);

        pixel = SAMPLE_MAIN(uv);

        // CDL values.
        pixel.rgb = pow(abs(pixel.rgb), (float3)_FilmGamma);
        pixel.rgb = pow(clamp(((pixel.rgb * _Slope) + _Offset), 0.0, 1.0), _Power);
        pixel.rgb = Saturation(pixel.rgb, _FilmSaturation);
        pixel.rgb = Contrast(pixel.rgb, (float4)_FilmContrast);

        UNITY_BRANCH
        if (_SuperContrast == 1)
          pixel.rgb = FilmContrast(pixel.rgb);

        // Grain.
        const float x = (uv.x + 4.0) * (uv.y + 4.0) * (_Time.y * 10.0);
        const float grain = (mod((mod(x, 13.0) + 1.0) * (mod(x, 123.0) + 1.0), 0.01) - 0.005) * 16.0 * _Grain;
        pixel.rgb += grain;
        pixel.rgb = clamp(pixel.rgb, 0.0, 1.0);

        const float t = float(int(_Time.y * 15.0));
        
        // Blotches.
        for (int i = 0; i < _Blotches; ++i)
          pixel.rgb *= FilmBlotch(t + 10.0 * float(i), uv);
        
          // Lines.
        for (int j = 0; j < _Lines; ++j)
          pixel.rgb *= FilmLine(t + 10.0 * float(j), uv);

          // Scratches.
        UNITY_BRANCH
        if (_Scratches > 0.0 && _RandomValue.z < _Scratches)
        {
          const float dist = 1.0 / _Scratches;
          float d = distance(uv, float2(_RandomValue.y * dist, _RandomValue.x * dist));
          UNITY_BRANCH
          if (d < 0.4)
          {
            const float xPeriod = 8.0;
            const float yPeriod = 1.0;
            const float turbulence = snoise(uv * 2.5);
            float scratch = 0.5 + (sin(((uv.x * xPeriod + uv.y * yPeriod + turbulence)) * PI + _Time.y) * 0.5);
            scratch = clamp((scratch * 10000.0) + 0.35, 0.0, 1.0);
            
            pixel.rgb *= scratch;
          }
        }

        // Blink.
        pixel.rgb *= (1.0 - _BlinkStrength) + _BlinkStrength * sin(_BlinkSpeed * _Time.y);

        // Natural vignette.
        pixel.rgb = NaturalVignette(pixel.rgb, uv);

        // Sepia.
        const float3x3 sepiaMatrix = float3x3
				(
					0.393, 0.349, 0.272,	// Red.
					0.769, 0.686, 0.534,	// Green.
					0.189, 0.168, 0.131		// Blue.
				);
        pixel.rgb = lerp(pixel.rgb, mul(pixel.rgb, sepiaMatrix), _Sepia);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

        return lerp(color, pixel, _Intensity);
      }

      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
