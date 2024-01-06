using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ItchyOwl.Editor
{
    public static class AnimationProcessor
    {
        /// <summary>
        /// Processes the animations by matching the clips to the presets. Only the first matching perset is applied per clip.
        /// </summary>
        /// <returns>Returns the count of processed animations.</returns>
        public static int ProcessAnimations(Object asset, IEnumerable<AnimationClipPreset> clipPresets, bool loadOriginalClips)
        {
            var path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
            {
                Debug.LogWarningFormat("[AnimationImporter] Cannot cast the importer as ModelImporter. The asset {0} is probably of wrong type.", asset.name);
                return 0;
            }
            if (!importer.importAnimation)
            {
                Debug.LogWarning("[AnimationImporter] Animation importing has been disabled on the importer settings of " + asset.name);
                return 0;
            }
            var clips = loadOriginalClips ? importer.defaultClipAnimations : importer.clipAnimations;
            int processed = 0;
            foreach (var clip in clips)
            {
                processed += ProcessClip(clip, clipPresets);
            }
            importer.clipAnimations = clips;
            AssetDatabase.ImportAsset(path);
            AssetDatabase.SaveAssets();
            return processed;
        }

        /// <summary>
        /// Tries to match the clip to the preset and appliers the preset to the clip. Only the first matching preset is applied.
        /// </summary>
        /// <returns>Returns 1 if the clip was processed, else returns 0.</returns>
        private static int ProcessClip(ModelImporterClipAnimation clip, IEnumerable<AnimationClipPreset> clipPresets)
        {
            foreach (var preset in clipPresets.OrderByDescending(p => p.priority))
            {
                if (!string.IsNullOrEmpty(preset.ignoredPhrase) && clip.name.ToLowerInvariant().Contains(preset.ignoredPhrase.ToLowerInvariant()))
                {
                    //Debug.LogFormat("[AnimationProcessor] Ignoring preset {0} regarding clip {1}", preset.name, clip.name);
                    continue;
                }
                if ((!string.IsNullOrEmpty(preset.clipFullName) && clip.name == preset.clipFullName) || (preset.clipPartialPhrases.Any() && preset.clipPartialPhrases.All(s => clip.name.ToLowerInvariant().Contains(s.ToLowerInvariant()))))
                {
                    Debug.LogFormat("[AnimationProcessor] Applying preset {0} to clip {1}", preset.name, clip.name);
                    if (preset.avatarMask != null)
                    {
                        clip.maskType = ClipAnimationMaskType.CopyFromOther;
                        clip.maskSource = preset.avatarMask;
                        clip.ConfigureClipFromMask(preset.avatarMask);
                    }
                    else
                    {
                        clip.maskType = ClipAnimationMaskType.None;
                        clip.maskSource = null;
                    }

                    if (preset.startFrame != 0)
                    {
                        clip.firstFrame = preset.startFrame;
                    }
                    if (preset.endFrame != 0)
                    {
                        clip.lastFrame = preset.endFrame;
                    }

                    clip.loop = preset.loop;
                    clip.loopPose = preset.loopPose;
                    clip.loopTime = preset.loopTime;
                    clip.cycleOffset = preset.cycleOffSet;

                    clip.hasAdditiveReferencePose = preset.hasAdditiveReferencePose;
                    clip.additiveReferencePoseFrame = preset.additiveReferencePoseFrame;

                    clip.heightFromFeet = preset.heightFromFeet;
                    clip.heightOffset = preset.heightOffset;
                    clip.rotationOffset = preset.rotationOffset;

                    clip.keepOriginalOrientation = preset.keepOriginalOrientation;
                    clip.keepOriginalPositionXZ = preset.keepOriginalPositionXZ;
                    clip.keepOriginalPositionY = preset.keepOriginalPositionY;

                    clip.lockRootHeightY = preset.lockRootHeightY;
                    clip.lockRootPositionXZ = preset.lockRootPositionXZ;
                    clip.lockRootRotation = preset.lockRootRotation;

                    clip.mirror = preset.mirror;

                    // Not sure what this is for
                    //clip.takeName = preset.takeName;

                    // Not sure how this affects mecanim animations, enable if you know what you do
                    //clip.wrapMode = preset.wrapMode;

                    if (!string.IsNullOrEmpty(preset.forceClipName))
                    {
                        clip.name = preset.forceClipName;
                    }
                    // curves
                    if (preset.curves.Count > 0)
                    {
                        var curves = new List<ClipAnimationInfoCurve>();
                        foreach (var c in preset.curves)
                        {
                            curves.Add(new ClipAnimationInfoCurve
                            {
                                name = c.name,
                                curve = c.curve
                            });
                        }
                        clip.curves = curves.ToArray();
                    }
                    else
                    {
                        clip.curves = new ClipAnimationInfoCurve[0];
                    }
                    // events
                    if (preset.events.Count > 0)
                    {
                        var events = new List<AnimationEvent>();
                        foreach (var e in preset.events)
                        {
                            events.Add(new AnimationEvent
                            {
                                stringParameter = e.stringParameter,
                                floatParameter = e.floatParameter,
                                intParameter = e.intParameter,
                                objectReferenceParameter = e.objectReferenceParameter,
                                functionName = e.functionName,
                                time = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, clip.lastFrame, e.frame))
                            });
                        }
                        clip.events = events.ToArray();
                    }
                    else
                    {
                        clip.events = new AnimationEvent[0];
                    }

                    return 1;
                }
            }
            return 0;
        }
    }
}
