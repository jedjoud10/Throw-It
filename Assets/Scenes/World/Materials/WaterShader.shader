/// Created by @cortvi
Shader "Custom/Water Refractive surface"
{
	Properties
	{
		[Header(Color)]
		[HDR] _LitColor("Lit water color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5

		[Header(Dissortion)]
		_DissortAmt("Dissortion amount", float) = 0.3

		[Header(Waves)]
		_WaveHeight("Wave Height", float) = 1.0
		_WaveTexture("Wave Height Texture", 2D) = "white" {}

		_SpeedX("Waves speed (X)", float) = 0.5
		_SpeedY("Waves speed (Y)", float) = 0.5
	}

		CGINCLUDE
			// Some helpers
#define DISSORTION_MAX 127
#define SPEED_UV(c) _Time.c * float2(_SpeedX, _SpeedY)
#define TEX(name) tex2D(name, i.uv##name)
			ENDCG

			SubShader
		{
			Tags{ "Queue" = "Transparent+1" "RenderType" = "Transparent" }

			GrabPass { }

			Zwrite Off
			CGPROGRAM
			#pragma surface surf Standard nolightmap noshadow
			#pragma target 3.0
			struct Input
			{
				float4 screenPos;
				float3 viewDir;
			};
			uniform sampler2D _GrabTexture;
			uniform float4 _GrabTexture_TexelSize;

			uniform fixed4 _DarkColor;
			uniform half4 _LitColor;
			uniform float _Glossiness;

			uniform float _DissortAmt;
			uniform float _BumpScale;
			uniform float _WaveHeight;
			sampler2D _WaveTexture;

			uniform float _SpeedX;
			uniform float _SpeedY;
			void vert(inout appdata_full v)
			{
				v.vertex.y += sin(v.vertex.x + _Time * _SpeedX) * _WaveHeight;
			}
			void surf(Input i, inout SurfaceOutputStandard s)
			{

				// Calculate dissorted UVs
				float2 dissort = pow(_DissortAmt * DISSORTION_MAX + 1, 2.0);
				i.screenPos.xy += (dissort * _GrabTexture_TexelSize.xy) * i.screenPos.z;

				// I'm not really sure of this part :/
				#ifndef UNITY_UV_STARTS_AT_TOP
					i.screenPos.y = 1 - i.screenPos.y;
				#endif

				// Compute final fragment color
				half3 frag;
				frag = lerp(tex2Dproj(_GrabTexture, i.screenPos), _LitColor, _LitColor.a).rgb;

				// Feed output
				s.Albedo = frag;
				s.Smoothness = _Glossiness;
			}
				ENDCG
		}
			FallBack "Standard"
}