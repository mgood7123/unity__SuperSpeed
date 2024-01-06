using UnityEngine;
using UnityEditor;

public class AnimationProcessorPopup : EditorWindow
{
    private static int count;

    public static void Display(int processedCount)
    {
        count = processedCount;
        AnimationProcessorPopup window = GetWindow<AnimationProcessorPopup>(true, "Animation Processor");
        window.ShowAuxWindow();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(string.Format("{0} clips were processed.", count), EditorStyles.wordWrappedLabel);
        GUILayout.Space(70);
        if (GUILayout.Button("OK"))
        {
            Close();
        }
    }
}