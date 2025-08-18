Shader "Jettelly/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Grass Speed",Range(0,5)) = 1
        _Amplitude("Grass Amplitude",Range(0,1)) = 0.1
        _Frequency("Grass Frequency",Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _Frequency;
            float _Amplitude;

            float4 wind(float4 vertexPos, float2 uv)
            {
                vertexPos.x += sin((uv - (_Time.y * _Speed)) * _Frequency) * (uv.y * _Amplitude);
                float4 vertex = UnityObjectToClipPos(vertexPos);
                return vertex;
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = wind(v.vertex, v.uv);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
