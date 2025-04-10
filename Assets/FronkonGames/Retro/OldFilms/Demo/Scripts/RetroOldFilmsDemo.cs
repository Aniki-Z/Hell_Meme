using System;
using UnityEngine;
using FronkonGames.Retro.OldFilms;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(RetroOldFilmsDemo))]
public class OldFilmsWarning : Editor
{
  private GUIStyle Style => style ??= new GUIStyle(GUI.skin.GetStyle("HelpBox")) { richText = true, fontSize = 14, alignment = TextAnchor.MiddleCenter };
  private GUIStyle style;
  public override void OnInspectorGUI()
  {
    EditorGUILayout.TextArea($"\nThis code is only for the demo\n\n<b>DO NOT USE</b> it in your projects\n\nIf you have any questions,\ncheck the <a href='{Constants.Support.Documentation}'>online help</a> or use the <a href='mailto:{Constants.Support.Email}'>support email</a>,\n<b>thanks!</b>\n", Style);
    DrawDefaultInspector();
  }
}
#endif

/// <summary> Retro: Old Films demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class RetroOldFilmsDemo : MonoBehaviour
{
  [Space]

  [SerializeField]
  private Transform floor;

  [SerializeField, Range(0.0f, 10.0f)]
  private float angularVelocity;

  private OldFilms.Settings settings;

  private GUIStyle styleTitle;
  private GUIStyle styleLabel;
  private GUIStyle styleButton;

  private void Awake()
  {
    if (OldFilms.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        EditorApplication.isPlaying = false;
#endif
    }

    this.enabled = OldFilms.IsInRenderFeatures();
  }

  private void Start()
  {
    settings = OldFilms.Instance.settings;
    settings?.ResetDefaultValues();
  }

  private void Update()
  {
    if (floor != null && angularVelocity > 0.0f)
      floor.rotation = Quaternion.Euler(0.0f, floor.rotation.eulerAngles.y + Time.deltaTime * angularVelocity * 10.0f, 0.0f);
  }

  private void OnGUI()
  {
    styleTitle ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.LowerCenter,
      fontSize = 32,
      fontStyle = FontStyle.Bold
    };

    styleLabel ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.UpperLeft,
      fontSize = 24
    };

    styleButton ??= new GUIStyle(GUI.skin.button)
    {
      fontSize = 24
    };

    GUILayout.BeginHorizontal();
    {
      GUILayout.BeginVertical("box", GUILayout.Width(500.0f), GUILayout.Height(Screen.height));
      {
        const float space = 10.0f;

        GUILayout.Space(space);
        GUILayout.Label(Constants.Asset.Name.ToUpper(), styleTitle);
        GUILayout.Space(space);

        settings.intensity = SliderField("Intensity", settings.intensity);

        settings.manufacturer = EnumField("Manufacturer", settings.manufacturer);
        settings.vignette = SliderField("Vignette", settings.vignette, 0.0f, 2.0f);
        settings.sepia = SliderField("Sepia", settings.sepia, 0.0f, 1.0f);
        settings.grain = SliderField("Grain", settings.grain, 0.0f, 1.0f);
        settings.moveFrame = Vector2.one * SliderField("Move frame", settings.moveFrame.x, 0.0f, 1.0f);
        settings.jumpFrame = SliderField("Jump frame", settings.jumpFrame, 0.0f, 1.0f);
        settings.blinkStrength = SliderField("Blink", settings.blinkStrength, 0.0f, 1.0f);
        settings.blotches = SliderField("Blotches", settings.blotches, 0, 10);
        settings.scratches = SliderField("Scratches", settings.scratches, 0.0f, 1.0f);
        settings.lines = SliderField("Lines", settings.lines, 0, 8);
        
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("RESET", styleButton) == true)
          settings.ResetDefaultValues();

        GUILayout.Space(4.0f);

        if (GUILayout.Button("ONLINE DOCUMENTATION", styleButton) == true)
          Application.OpenURL(Constants.Support.Documentation);

        GUILayout.Space(4.0f);

        if (GUILayout.Button("❤️ WRITE A REVIEW ❤️", styleButton) == true)
          Application.OpenURL(Constants.Support.Store);

        GUILayout.Space(space * 2.0f);
      }
      GUILayout.EndVertical();

      GUILayout.FlexibleSpace();
    }
    GUILayout.EndHorizontal();
  }

  private bool ToggleField(string label, bool value)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = GUILayout.Toggle(value, string.Empty);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private float SliderField(string label, float value, float min = 0.0f, float max = 1.0f)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = GUILayout.HorizontalSlider(value, min, max);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private int SliderField(string label, int value, int min, int max)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = (int)GUILayout.HorizontalSlider(value, min, max);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private Color ColorField(string label, Color value, bool alpha = true)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      float originalAlpha = value.a;

      Color.RGBToHSV(value, out float h, out float s, out float v);
      h = GUILayout.HorizontalSlider(h, 0.0f, 1.0f);
      value = Color.HSVToRGB(h, s, v);

      if (alpha == false)
        value.a = originalAlpha;
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private Vector2 Vector2Field(string label, Vector2 value, float min, float max) => Vector2Field(label, value, "X", "Y", min, max);

  private Vector2 Vector2Field(string label, Vector2 value, string x = "X", string y = "Y", float min = 0.0f, float max = 1.0f)
  {
    GUILayout.Label(label, styleLabel);

    value.x = SliderField($"   {x}", value.x, min, max);
    value.y = SliderField($"   {y}", value.y, min, max);

    return value;
  }

  private Vector3 Vector3Field(string label, Vector3 value, float min, float max) => Vector3Field(label, value, "X", "Y", "Z", min, max);

  private Vector3 Vector3Field(string label, Vector3 value, string x = "X", string y = "Y", string z = "Z", float min = 0.0f, float max = 1.0f)
  {
    GUILayout.Label(label, styleLabel);

    value.x = SliderField($"   {x}", value.x, min, max);
    value.y = SliderField($"   {y}", value.y, min, max);
    value.z = SliderField($"   {z}", value.z, min, max);

    return value;
  }

  private Vector4 Vector4Field(string label, Vector4 value, float min, float max) => Vector4Field(label, value, "X", "Y", "Z", "W", min, max);

  private Vector4 Vector4Field(string label, Vector4 value, string x = "X", string y = "Y", string z = "Z", string w = "W", float min = 0.0f, float max = 1.0f)
  {
    GUILayout.Label(label, styleLabel);

    value.x = SliderField($"   {x}", value.x, min, max);
    value.y = SliderField($"   {y}", value.y, min, max);
    value.z = SliderField($"   {z}", value.z, min, max);
    value.w = SliderField($"   {w}", value.w, min, max);

    return value;
  }

  private T EnumField<T>(string label, T value) where T : Enum
  {
    string[] names = Enum.GetNames(typeof(T));
    Array values = Enum.GetValues(typeof(T));
    int index = Array.IndexOf(values, value);

    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      if (GUILayout.Button("<", styleButton) == true)
        index = index > 0 ? index - 1 : values.Length - 1;

      GUILayout.Label(names[index], styleLabel);

      if (GUILayout.Button(">", styleButton) == true)
        index = index < values.Length - 1 ? index + 1 : 0;
    }
    GUILayout.EndHorizontal();

    return (T)(object)index;
  }
}