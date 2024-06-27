Shader "Custom/UIImageOutlineShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0,0.1)) = 0.02
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Name "Default"
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 texcoord  : TEXCOORD0;
                float4 vertex    : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.texcoord);
                
                // Check the pixels around the current pixel
                float2 offset = _OutlineThickness * _MainTex_ST.xy;
                fixed4 left = tex2D(_MainTex, i.texcoord + float2(-offset.x, 0));
                fixed4 right = tex2D(_MainTex, i.texcoord + float2(offset.x, 0));
                fixed4 up = tex2D(_MainTex, i.texcoord + float2(0, offset.y));
                fixed4 down = tex2D(_MainTex, i.texcoord + float2(0, -offset.y));

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
