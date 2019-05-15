Shader "Custom/Pulse" {
	Properties {
		_Color1 ("Color1", Color) = (1,1,1,1)
		_Color2 ("Color2", Color) = (1,1,1,1)
		_Pulsing("Pulsing", Range(0,1)) = 0.0
		_Speed("Speed", Range(0.25,2)) = 0.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
	//	#pragma surface surf Lambert
		#pragma vertex vert
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
			float3 vertColor;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color1;
		fixed4 _Color2;
		float _Pulsing;
		float _Speed;
		
		void vert(inout appdata_full v, out Input o)
		{
			//float height = v.vertex.x;

	  		float height = (sin(pow(_Time.z,_Speed))*_Pulsing)+1.1;	
			v.vertex.xyz = float3( v.vertex.x * height, v.vertex.y * height, v.vertex.z * height);

			o.vertColor  = height;
			o.uv_MainTex = v.texcoord;
		}
		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float waveHeight = sin(pow(_Time.z,_Speed))+1.1;

			o.Albedo = (_Color1 * waveHeight) + (_Color2 * abs(waveHeight - 2))  ;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
