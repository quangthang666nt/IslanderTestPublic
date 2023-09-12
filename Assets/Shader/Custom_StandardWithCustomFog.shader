Shader "Custom/StandardWithCustomFog" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_FogExponent ("Fog Exponent", Range(0, 10)) = 3
		_FogMinStrength ("Minimum Fog Strength", Range(0, 1)) = 0.1
		_FogMaxStrength ("Maximum Fog Strength", Range(0, 1)) = 1
		_BrightnessThreshold ("Brightness Threshold", Range(0, 5)) = 1
		_BrightnessInfluence ("Brightness Influence", Range(0, 1)) = 1
		_NightLum ("Night Ilumination", Range(0, 1)) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}