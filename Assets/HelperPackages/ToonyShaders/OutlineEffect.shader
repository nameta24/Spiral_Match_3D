Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0,0.1)) = 0.02
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
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
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Check the pixels around the current pixel
                float2 offset = _OutlineThickness * _MainTex_ST.xy;
                fixed4 left = tex2D(_MainTex, i.uv + float2(-offset.x, 0));
                fixed4 right = tex2D(_MainTex, i.uv + float2(offset.x, 0));
                fixed4 up = tex2D(_MainTex, i.uv + float2(0, offset.y));
                fixed4 down = tex2D(_MainTex, i.uv + float2(0, -offset.y));

                // If there's transparency around the current pixel, draw the outline
                if (col.a > 0.1 && (left.a < 0.1 || right.a < 0.1 || up.a < 0.1 || down.a < 0.1))
                {
                    return _OutlineColor;
                }

                return col;
            }
            ENDCG
        }
    }
}
