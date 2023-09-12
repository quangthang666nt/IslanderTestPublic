Shader "Hidden/CubeBlend" {
	Properties {
		[NoScaleOffset] _TexA ("Cubemap", Cube) = "grey" {}
		[NoScaleOffset] _TexB ("Cubemap", Cube) = "grey" {}
		_value ("Value", Range(0, 1)) = 0.5
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
}