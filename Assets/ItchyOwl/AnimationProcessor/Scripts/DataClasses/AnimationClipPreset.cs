using UnityEngine;
using System;
using System.Collections.Generic;

namespace ItchyOwl.Editor
{
    public class AnimationClipPreset : ScriptableObject
    {
        [Tooltip("If the clip name contains this phrase, it will be ignored and these settings won't be applied. Case insensitive. First priority.")]
        public string ignoredPhrase;
        [Tooltip("If the clip name matches exactly to this string, these settings will be applied to it. Case sensitive. Secondary priority.")]
        public string clipFullName;
        [Tooltip("If the clip name contains all of the phrases, these settings will be applied to it. Case insensitive. Secondary priority.")]
        public List<string> clipPartialPhrases = new List<string>();

        [Tooltip("Increase the value to evaluate this preset before others. Decreaset it to set this to be evaluated after other presets (e.g. as a default fallback).")]
        public int priority;

        [Tooltip("Leave at 0 if you want to use the original clip settings.")]
        public float startFrame;
        [Tooltip("Leave at 0 if you want to use the original clip settings.")]
        public float endFrame;

        public bool loop;
        public bool loopPose;
        public bool loopTime;
        public float cycleOffSet;

        public bool heightFromFeet = true;

        public float heightOffset;
        public float rotationOffset;

        public bool keepOriginalPositionXZ;
        public bool keepOriginalPositionY = true;
        public bool keepOriginalOrientation;

        public bool lockRootHeightY;
        public bool lockRootPositionXZ;
        public bool lockRootRotation;

        public bool mirror;

        [Tooltip("Leave empty, if you don't want to force a clip name.")]
        public string forceClipName;

        // Not sure how this affects mecanim animations, enable if you know what you do
        //public WrapMode wrapMode;

        public bool hasAdditiveReferencePose;
        public float additiveReferencePoseFrame;

        public AvatarMask avatarMask;

        public List<SerializableClipAnimationInfoCurve> curves = new List<SerializableClipAnimationInfoCurve>();
        public List<SerializableAnimationEvent> events = new List<SerializableAnimationEvent>();
    }

    [Serializable]
    public class SerializableClipAnimationInfoCurve
    {
        public string name;
        public AnimationCurve curve;
    }

    [Serializable]
    public class SerializableAnimationEvent
    {
        public string stringParameter;
        public float floatParameter;
        public int intParameter;
        public UnityEngine.Object objectReferenceParameter;
        public string functionName;
        public float frame;
    }
}
