using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ItchyOwl.Editor
{
    public class AnimationProcessorWindow : EditorWindow
    {
        private static AnimationProcessorWindow window;
        private int assetCount = 1;
        private List<Object> assets = new List<Object>();
        private AnimationClipPresetContainer presetContainer;
        private int presetCount = 1;
        private List<AnimationClipPreset> clipPresets = new List<AnimationClipPreset>();
        private bool loadOriginalClips;

        [MenuItem("Assets/Process Animations")]
        public static void Init()
        {
            window = GetWindow<AnimationProcessorWindow>(true, "Animation Processor");
            window.Show();
        }

        private void OnGUI()
        {
            loadOriginalClips = EditorGUILayout.Toggle(new GUIContent("Load Original Clips", "Use the clips that were originally defined in the asset. All clips created in Unity are lost."), loadOriginalClips);
            assetCount = EditorGUILayout.IntSlider(new GUIContent("Asset Count", "Drag the slider to adjust the asset count."), assetCount, 1, 10);
            assetCount = ManageList(assets, assetCount);
            for (int i = 0; i < assetCount; i++)
            {
                assets[i] = EditorGUILayout.ObjectField(new GUIContent("Asset " + (i + 1), "Drag here the asset(s) containing the animations."), assets[i], typeof(Object), false);
            }
            presetContainer = EditorGUILayout.ObjectField(new GUIContent("Preset Container", "Drag here an instance of AnimationClipPresetContainer with predefined references to the clip presets."), presetContainer, typeof(AnimationClipPresetContainer), false) as AnimationClipPresetContainer;
            presetCount = EditorGUILayout.IntSlider(new GUIContent("Preset Count", "Drag the slider to adjust the preset count."), presetCount, 0, 10);
            presetCount = ManageList(clipPresets, presetCount);
            for (int i = 0; i < presetCount; i++)
            {
                clipPresets[i] = (AnimationClipPreset)EditorGUILayout.ObjectField(new GUIContent("Preset " + (i + 1), "Drag here the clip preset(s). You can also use the preset container."), clipPresets[i], typeof(AnimationClipPreset), false);
            }
            if (GUILayout.Button("Process"))
            {
                int processed = 0;
                foreach (var asset in assets.Where(a => a != null))
                {
                    IEnumerable<AnimationClipPreset> presets = clipPresets;
                    if (presetContainer != null)
                    {
                        presets = presets.Concat(presetContainer.presets).Where(cp => cp != null);
                    }    
                    processed += AnimationProcessor.ProcessAnimations(asset, presets, loadOriginalClips);
                }
                AnimationProcessorPopup.Display(processed);
            }
            if (GUILayout.Button(new GUIContent("Create New Preset", "Creates a new asset (scriptable object) that can be used as a preset for the animation clips. Use this asset to define the clip settings.")))
            {
                ScriptableObjectFactory.Create<AnimationClipPreset>("New Animation Clip Preset");
            }
            if (GUILayout.Button(new GUIContent("Create New Container", "Creates a new asset (scriptable object) that can be used to store the references of animation clip presets.")))
            {
                ScriptableObjectFactory.Create<AnimationClipPresetContainer>("New Animation Clip Preset Container");
            }
        }

        private int ManageList<T>(List<T> list, int count)
        {
            if (count > list.Count)
            {
                for (int i = list.Count; i < count; i++)
                {
                    list.Add(default(T));
                }
            }
            else if (count < list.Count)
            {
                for (int i = list.Count; i > count; i--)
                {
                    list.Remove(list.LastOrDefault());
                }
            }
            return count;
        }
    }
}
