Shader "Unlit/Bullet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 텍스처 프로퍼티
        _Color ("Color", Color) = (1, 1, 1, 1) // 색상 및 투명도 조절
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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
            fixed4 _Color; // Color 프로퍼티로 알파값 처리

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 텍스처 샘플링
                fixed4 texColor = tex2D(_MainTex, i.uv);
                // Color의 알파값 적용
                return texColor * _Color;
            }
            ENDCG
        }
    }
}
