// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Leaves shader"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Ammount("Ammount", Range(0, 1)) = 0.0
        _Speed("Speed", Float) = 0.0
        _Randomness("Randomness", Float) = 10.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200
            Cull Off
            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            //#pragma surface surf Standard fullforwardshadows
            #pragma surface surf Standard vertex:vert addshadow 
            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
            };

            half _Glossiness;
            half _Metallic;
            float _Ammount;
            float _Speed;
            float _Randomness;
            fixed4 _Color;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)
            float rand(float3 co)
            {
                return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }
            float windAxis(float posPerAxis, float objectPosPerAxis)
            {
                return sin(posPerAxis + (rand(objectPosPerAxis)*_Randomness) + _Time.x * _Speed) * _Ammount;
            }
            void vert(inout appdata_full v) {
                float3 baseWorldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                float3 vertexPos = v.vertex.xyz;
                float3 offset = float3(windAxis(vertexPos.x, baseWorldPos.x), windAxis(vertexPos.y, baseWorldPos.y), windAxis(vertexPos.z, baseWorldPos.z) / 10);
                v.vertex.xyz += offset * _Ammount;
            }
            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                //World position of the object
                float3 baseWorldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                //Pixel color from palette
                float4 pixelColor = tex2D(_MainTex, float2(rand(baseWorldPos), 0));

                // Albedo comes from a random pixel in the color palette
                o.Albedo = pixelColor;
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
            }

            ENDCG
        }
            FallBack "Diffuse"
}
