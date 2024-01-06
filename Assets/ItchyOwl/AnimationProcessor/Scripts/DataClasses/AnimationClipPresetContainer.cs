using System.Collections.Generic;
using UnityEngine;

namespace ItchyOwl.Editor
{
    /// <summary>
    /// The purpose of this class is simply to be a persistent container for AnimationClipPresets, so that you don't have to drag multiple presets to the Animation Processor Window.
    /// Simply drag this container, once you have setup the references to the presets here.
    /// </summary>
    public class AnimationClipPresetContainer : ScriptableObject
    {
        public List<AnimationClipPreset> presets = new List<AnimationClipPreset>();
    }
}
