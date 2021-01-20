Shader "MarcoMinganna/Cutoff" {
	Properties{
		_myDiffuse("Diffuse Texture", 2D) = "white" {}
		_RimColor("Rim Color",Color) = (0,0.5,0.5,0.0)
		_RimPower("Rim Power",Range(0.5,8.0)) = 3.0
		_stripePower("Stripe Power",Range(0.1,1.0)) = 0.4
	}
		SubShader{
		CGPROGRAM
		  #pragma surface surf Lambert
		struct Input {
		float3 viewDir;
		float3 worldPos;
		float2 uv_myDiffuse;
		};
		float4 _RimColor;
		float _RimPower;
		float _stripePower;
		sampler2D _myDiffuse;
	void surf(Input IN, inout SurfaceOutput o) {
		half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
		o.Albedo = tex2D(_myDiffuse, IN.uv_myDiffuse ).rgb;
		//o.Emission = _RimColor.rgb * rim > 0.5 ? rim:0; //one color around the edges
		//o.Emission = rim > 0.5 ? float3(1,0,0):rim>0.3? float3(0,1,0):0; //two color around the edges
		//o.Emission = IN.worldPos.y > 1 ? float3(0, 1, 0) : float3(1, 0, 0); //change color based on position
		o.Emission = frac(IN.worldPos.y * 10 * 0.5) > _stripePower ? float3(0, 1, 0) * rim : float3(1, 0, 0)*rim;
	}
	ENDCG
	}
		Fallback"Diffuse"
}

