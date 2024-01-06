v. 1.1

Thank you for downloading Animation Processor!

With this tool, you can setup multiple animation clips in multiple files at once.

DESCRIPTION
Use AnimationClipPresets (ScriptableObjects) to define the settings. These can be defined per clip or per clip type. For example, you may want to define settings for all your idle animations by using one preset and for action animations using another.

The presets are applied by string comparisons. You have three options: 
a) Define the full name of the clip 
b) Define partial phrases of the clip name (e.g. "loop", "walk" etc.) 
c) Define a phrase that will be used to ignore files.

Clip name is defined in the animation import window. Ignored phrases are examined first and names after that. So even if the clip name would match the string, the clip will be ignored, if it also contains the ignored phrase. The clip name has to contain all the partial phrases to match the preset.

So, if you want to apply certain settings for example to all idle animations, it's useful to follow coherent naming conventions, like Aggressive_Idle1, Defensive_Idle5 etc. Then create a common preset for all clips using the "clipPartialName" field, which in this case should be "idle". Note that full name is case sensitive, and the other two are case insensitive.

Note that there is the "priority" variable, which defines ther order of presets. So the order of presets only matter if the priority is the same. Presets with high priority are evaluated first.

If you have many presets and iterate a lot, setup the references to the presets in a container file. There is an example in the ConfigAssets/Containers/ folder. If you use both containers and direct references to the presets, both will be used. Duplicates don't matter.

If you have any questions, don't hesitate to mail me at contact@itchyowl.com!

STEP BY STEP INSTRUCTIONS
- Go to Assets -> Process Animations
- Assign (up to 10) assets containing the animations. There is an example asset found in ItchyOwl/AnimationProcessor/ExampleAssets.
- Assign (up to 10) presets for clip settings. There are ready presets found in ConfigAssets/Presets/. You can use (and duplicate) these or create a new by clicking "Create New Preset".
- AND/OR Assign a container that holds references to the presets. There is an example in the ConfigAssets/Containers/ folder. You can use (and duplicate) this or create a new by clicking "Create New Container".
- Click process.
- Take a look at the animation asset and ensure that all the settings are as they should. If not, iterate.

NOTE
- Loading original clips destroys the clips created in Unity
- Currently it's only possible to apply all or none of the settings of a preset to a clip. However, if the values are left unassigned, they should match the default values of the animation importer.
- ScriptableObjects can also be created via Assets -> Create -> ScriptableObject. Animation Processor window calls this under the hood. This is a generic factory that can be used for creating any ScriptableObjects. Feel free to use it for other purposes. The factory files can be found in the folders under ItchyOwl/_Common/ScriptableObjectFactory/.
- Variable names and count differ a bit from the settings that can be found in the Animation Import window, but they follow in most parts the namings of ModelImporterClipAnimation class, which is used in animation importing. For example, if you want to bake y position based the center of mass, set lockRootHeightY and keepOriginalPositionY to false. In the future, I might create a custom inspector for the AnimationClipPreset class. When I have time to do this, there most likely will be the option to ignore some settings instead of applying all or none.

CHANGE LOG
v. 1.1
- Set lockRootHeightY and keepOriginalPositionY default values to true so that they match the defaults of the animation importer.
- Fix the null reference exception that was thrown when no container was given.
- Fix the presets being applied to clips incorrectly when the partial phrases list was empty.
- Tweak the example presets and clip names.