Shader "Custom/StandardVertexColorWithCustomFog" {
	Properties {
		_FogExponent ("Fog Exponent", Range(0, 10)) = 3
		_FogMinStrength ("Minimum Fog Strength", Range(0, 1)) = 0.1
		_FogMaxStrength ("Maximum Fog Strength", Range(0, 1)) = 1
		_BrightnessThreshold ("Brightness Threshold", Range(0, 5)) = 1
		_BrightnessInfluence ("Brightness Influence", Range(0, 1)) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
}