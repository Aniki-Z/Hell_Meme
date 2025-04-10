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

namespace FronkonGames.Retro.OldFilms
{
  /// <summary> Film 'Color Decision List' (https://en.wikipedia.org/wiki/ASC_CDL). </summary>
  public class FilmCDL
  {
    /// <summary> Slope of the transfer function without shifting the black level. </summary>
    public Vector3 slope = Vector3.one;

    /// <summary> Raises or lowers overall brightness of a component. </summary>
    public Vector3 offset = Vector3.zero;

    /// <summary> Changes the intermediate shape of the transfer function. </summary>
    public Vector3 power = Vector3.one;

    /// <summary> Color saturation. </summary>
    public float saturation = 1.0f;

    /// <summary> Color contrast. </summary>
    public float contrast = 1.0f;

    /// <summary> Gamma. </summary>
    public float gamma = 1.0f;

    /// <summary>  </summary>
    public bool filmContrast = false;

    public static FilmCDL Kodak2383 = new()
    {
      slope = new Vector3(1.01f, 1.0f, 1.0f),
      offset = Vector3.zero,
      power = new Vector3(0.95f, 1.0f, 1.0f),
      saturation = 1.2f,
      contrast = 1.0f,
      gamma = 1.0f,
      filmContrast = true
    };

    public static FilmCDL Kodak2393 = new()
    {
      slope = new Vector3(1.08f, 1.19f, 1.07f),
      offset = new Vector3(0.04f, -0.06f, 0.02f),
      power = new Vector3(1.07f, 1.11f, 1.20f),
      saturation = 1.0f,
      contrast = 1.0f,
      gamma = 1.0f,
      filmContrast = true
    };

    public static FilmCDL Kodak2395 = new ()
    {
      slope = new Vector3(0.98f, 1.0f, 1.03f),
      offset = Vector3.zero,
      power = new Vector3(0.84f, 0.97f, 1.10f),
      saturation = 1.0f,
      contrast = 1.0f,
      gamma = 1.0f,
      filmContrast = true
    };

    public static FilmCDL Agfa1978 = new ()
    {
      slope = new Vector3(1.12f, 1.42f, 1.19f),
      offset = new Vector3(0.04f, -0.06f, 0.02f),
      power = new Vector3(0.94f, 0.81f, 0.83f),
      saturation = 0.7f,
      contrast = 1.06f,
      gamma = 1.0f,
      filmContrast = true
    };

    public static FilmCDL Agfa1905 = new()
    {
      slope = Vector3.one,
      offset = new Vector3(-0.05f, -0.04f, -0.03f),
      power = Vector3.one,
      saturation = 0.0f,
      contrast = 1.33f,
      gamma = 0.6f,
      filmContrast = false
    };

    public static FilmCDL Agfa1935 = new()
    {
      slope = new Vector3(1.33f, 1.01f, 0.63f),
      offset = Vector3.zero,
      power = new Vector3(1.21f, 0.96f, 0.74f),
      saturation = 0.6f,
      contrast = 1.0f,
      gamma = 0.83f,
      filmContrast = true
    };

    public static FilmCDL Beer1973 = new()
    {
      slope = new Vector3(0.88f, 0.96f, 1.24f),
      offset = Vector3.zero,
      power = new Vector3(1.45f, 1.29f, 1.27f),
      saturation = 1.0f,
      contrast = 0.93f,
      gamma = 0.9f,
      filmContrast = true
    };

    public static FilmCDL Beer1933 = new()
    {
      slope = new Vector3(1.2f, 1.2f, 1.2f),
      offset = Vector3.zero,
      power = new Vector3(1.3f, 1.3f, 1.3f),
      saturation = 0.0f,
      contrast = 0.8f,
      gamma = 1.2f,
      filmContrast = true
    };

    public static FilmCDL Beer2001 = new()
    {
      slope = new Vector3(0.93f, 0.94f, 0.96f),
      offset = Vector3.zero,
      power = new Vector3(1.6f, 1.1f, 0.95f),
      saturation = 0.4f,
      contrast = 1.1f,
      gamma = 0.7f,
      filmContrast = true
    };

    public static FilmCDL Beer2006 = new()
    {
      slope = new Vector3(1.616452f, 1.331932f, 0.842867f),
      offset = new Vector3(-0.152205f, 0.079621f, 0.197558f),
      power = new Vector3(1.650251f, 1.536614f, 1.553357f),
      saturation = 0.7f,
      contrast = 1.0f,
      gamma = 1.1f,
      filmContrast = false
    };

    public static FilmCDL Polaroid = new()
    {
      slope = new Vector3(0.65f, 1.0f, 0.8f),
      offset = new Vector3(0.07f, 0.0f, 0.08f),
      power = Vector3.one,
      saturation = 1.4f,
      contrast = 1.0f,
      gamma = 1.0f,
      filmContrast = true
    };

    public static FilmCDL CubaLibre = new()
    {
      slope = new Vector3(1.19f, 1.1f, 0.77f),
      offset = new Vector3(-0.04f, -0.08f, -0.07f),
      power = new Vector3(0.8f, 0.8f, 0.8f),
      saturation = 0.9f,
      contrast = 1.0f,
      gamma = 0.9f,
      filmContrast = true
    };

    public static FilmCDL Fuji4711 = new()
    {
      slope = new Vector3(1.1f, 1.0f, 0.8f),
      offset = Vector3.zero,
      power = new Vector3(1.5f, 1.0f, 1.0f),
      saturation = 0.6f,
      contrast = 1.0f,
      gamma = 0.9f,
      filmContrast = true
    };

    public static FilmCDL ORWO0815 = new()
    {
      slope = new Vector3(1.15f, 1.11f, 0.86f),
      offset = new Vector3(0.0f, 0.01f, -0.02f),
      power = new Vector3(1.41f, 1.0f, 0.74f),
      saturation = 0.45f,
      contrast = 0.98f,
      gamma = 0.86f,
      filmContrast = true
    };

    public static FilmCDL BlackAndWhite = new()
    {
      slope = Vector3.one,
      offset = Vector3.zero,
      power = Vector3.one,
      saturation = 0.0f,
      contrast = 1.1f,
      gamma = 0.7f,
      filmContrast = true
    };

    public static FilmCDL Spearmint = new()
    {
      slope = new Vector3(1.02f, 1.32f, 1.09f),
      offset = new Vector3(0.04f, -0.06f, 0.02f),
      power = new Vector3(0.70f, 0.44f, 0.51f),
      saturation = 0.8f,
      contrast = 1.0f,
      gamma = 1.30f,
      filmContrast = true
    };

    public static FilmCDL TeaTime = new()
    {
      slope = new Vector3(1.297496f, 0.943091f, 0.501793f),
      offset = new Vector3(-0.132450f, 0.036699f, 0.147457f),
      power = new Vector3(1.180667f, 1.032265f, 1.215274f),
      saturation = 1.0f,
      contrast = 1.0f,
      gamma = 1.0f,
      filmContrast = false
    };

    public static FilmCDL PurpleRain = new()
    {
      slope = new Vector3(1.671897f, 1.274243f, 0.994490f),
      offset = new Vector3(-0.052148f, -0.026815f, 0.483182f),
      power = new Vector3(1.650251f, 1.536614f, 1.553357f),
      saturation = 0.282609f,
      contrast = 1.0f,
      gamma = 1.0f,
      filmContrast = false
    };

    public static FilmCDL Preset(Manufacturers manufacturer)
    {
      switch (manufacturer)
      {
        case Manufacturers.Kodak_2383:  return Kodak2383;
        case Manufacturers.Kodak_2393:  return Kodak2393;
        case Manufacturers.Kodak_2395:  return Kodak2395;
        case Manufacturers.Agfa_1978:   return Agfa1978;
        case Manufacturers.Agfa_1905:   return Agfa1905;
        case Manufacturers.Agfa_1935:   return Agfa1935;
        case Manufacturers.Beer_1973:   return Beer1973;
        case Manufacturers.Beer_1933:   return Beer1933;
        case Manufacturers.Beer_2001:   return Beer2001;
        case Manufacturers.Beer_2006:   return Beer2006;
        case Manufacturers.Polaroid:    return Polaroid;
        case Manufacturers.Cuba_libre:  return CubaLibre;
        case Manufacturers.Fuji_4711:   return Fuji4711;
        case Manufacturers.ORWO_0815:   return ORWO0815;
        case Manufacturers.Black_white: return BlackAndWhite;
        case Manufacturers.Spearmint:   return Spearmint;
        case Manufacturers.Tea_time:    return TeaTime;
        case Manufacturers.Purple_rain: return PurpleRain;

        default: return BlackAndWhite;
      }
    }
  }
}
