using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Scripting;

[Serializable]
[Preserve]
[PostProcess(typeof(SepiaRenderer), PostProcessEvent.AfterStack, "Custom/Sepia", true)]
public sealed class Sepia : PostProcessEffectSettings
{
}
