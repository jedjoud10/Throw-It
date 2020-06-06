Shader "Custom/LeavesShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Lighting On
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            float rand(float3 co)
            {
                return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 baseWorldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                // sample the texture
                float4 pixelColor = tex2D(_MainTex, float2(rand(baseWorldPos), 0));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, pixelColor);
                return pixelColor;
            }
            ENDCG
        }
    }
    Fallback "VertexLit", 1
}