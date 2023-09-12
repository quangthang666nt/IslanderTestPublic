using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Scripting;

[Preserve]
public sealed class SepiaRenderer : PostProcessEffectRenderer<Sepia>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Custom/Sepia"));
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
