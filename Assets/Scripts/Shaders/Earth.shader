Shader "Custom/Earth"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _AlbedoColor ("Albedo Color", Color) = (1,1,1,1) // Add this line
        _Longitude("Longitude", Range(-180, 180)) = 0
        _Latitude("Latitude", Range(-90, 90)) = 0
        _TestRadiusKM("Test Radius KM", Float) = 100
    }
    SubShader
    {
        Tags { "Queue"="Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Scripts/Shader Common/GeoMath.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 objPos : TEXCOORD0;
                float3 worldNormal : NORMAL;
            };

            sampler2D _MainTex;
            float _Longitude, _Latitude, _TestRadiusKM;
            fixed4 _AlbedoColor; // Add this line

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.objPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 spherePos = normalize(i.objPos);
                float2 texCoord = pointToUV(spherePos);
                fixed4 col = tex2D(_MainTex, texCoord) * _AlbedoColor; // Multiply with Albedo Color

                return col;
            }
            ENDCG
            
        }
        
    }
    Fallback "VertexLit"
}