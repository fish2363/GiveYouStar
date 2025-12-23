Shader "Unlit/OutLineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size (px)", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 이미 불투명한 픽셀은 원본 유지
                if (col.a > 0.01)
                    return col;

                float2 px = _MainTex_TexelSize.xy * _OutlineSize;

                float a = 0;

                // 4방향
                a += tex2D(_MainTex, i.uv + float2(px.x, 0)).a;
                a += tex2D(_MainTex, i.uv + float2(-px.x, 0)).a;
                a += tex2D(_MainTex, i.uv + float2(0, px.y)).a;
                a += tex2D(_MainTex, i.uv + float2(0, -px.y)).a;

                // 대각선 4방향 (중요)
                a += tex2D(_MainTex, i.uv + float2(px.x, px.y)).a;
                a += tex2D(_MainTex, i.uv + float2(-px.x, px.y)).a;
                a += tex2D(_MainTex, i.uv + float2(px.x, -px.y)).a;
                a += tex2D(_MainTex, i.uv + float2(-px.x, -px.y)).a;

                if (a > 0.01)
                    return _OutlineColor;

                return 0;
            }
            ENDCG
        }
    }
}
