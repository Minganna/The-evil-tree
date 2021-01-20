Shader "MarcoMinganna/ToonHologram" {
	Properties{
		_RimColor("Rim Color",Color) = (0,0.5,0.5,0.0)
		_RimPower("Rim Power",Range(0.5,8.0)) = 3.0
		_RampTex("Ramp Texture", 2D) = "white"{}
	}
		SubShader{
		Tags{
				"Queue" = "Transparent"
			}

		Pass{
		ZWrite On
		ColorMask 0
		}

		CGPROGRAM
		  #pragma surface surf ToonRamp alpha:fade
		struct Input {
		float3 viewDir;
		};
		sampler2D _RampTex;
		float4 _RimColor;
		float _RimPower;

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

	void surf(Input IN, inout SurfaceOutput o) {
		half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
		o.Emission = _RimColor.rgb * pow(rim, _RimPower) * 10;
		o.Alpha = pow(rim, _RimPower);
	}
	ENDCG
	}
		Fallback"Diffuse"
}
