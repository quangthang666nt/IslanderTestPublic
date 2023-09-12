Shader "Custom/WaterEdgeShader" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_Color2 ("Color 2", Vector) = (1,1,1,1)
		_NoiseTiling ("Noise Tiling", Float) = 10
		_NoiseCutoffLow ("Noise Cutoff Low", Range(0, 1)) = 0.5
		_NoiseCutoffHigh ("Noise Cutoff High", Range(0, 1)) = 0.8
		_NoiseAnimSpeed ("Noise Animation Speed", Range(0, 100)) = 50
		_FoamColor ("Foam Color", Vector) = (1,1,1,1)
		_MaxFoamDistance ("Max Foam Distance", Float) = 0.1
		_FogExponent ("Fog Exponent", Range(0, 10)) = 3
		_FogMinStrength ("Minimum Fog Strength", Range(0, 1)) = 0.1
		_FogMaxStrength ("Maximum Fog Strength", Range(0, 1)) = 1
		_BrightnessThreshold ("Brightness Threshold", Range(0, 5)) = 1
		_BrightnessInfluence ("Brightness Influence", Range(0, 1)) = 1
		_SampleDistance ("Sample Distance", Float) = 0.02
		_MaxHeight ("Max Height", Float) = 1
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
}