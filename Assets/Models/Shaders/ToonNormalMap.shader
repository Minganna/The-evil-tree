Shader "MarcoMinganna/ToonNormalMap" {
	Properties
	{
		_Color(" Emissive Color", Color) = (0,0,0,1)
		_MainTex("MainTex", 2D) = "white"{}
		_RampTex("Ramp Texture", 2D) = "white"{}
		_myBump("Normal Texture",2D) = "bump"{}
		_mySlider("Bump Amount",Range(0,10)) = 1

	}

		SubShader
	{

		CGPROGRAM
		#pragma surface surf ToonRamp

		float4 _Color;
		sampler2D _RampTex;
		sampler2D _MainTex;
		sampler2D _myBump;
		half _mySlider;

		float4 LightingToonRamp(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			float diff = dot(s.Normal, lightDir);
			float h = diff * 0.5 + 0.5;
			float2 rh = h;
			float3 ramp = tex2D(_RampTex, rh).rgb;

			float4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (ramp);
			c.a = s.Alpha;
			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_myBump;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Emission = _Color.rgb;
			o.Normal = UnpackNormal(tex2D(_myBump, IN.uv_myBump));
			o.Normal *= float3(_mySlider, _mySlider, 1);
		}

		ENDCG
	}

		FallBack "Diffuse"

}
