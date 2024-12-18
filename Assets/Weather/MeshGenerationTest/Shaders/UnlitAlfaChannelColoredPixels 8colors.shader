Shader "Custom/UnlitTextureWithAlphaAnd8Colors"
{
    Properties
    {
        _MainTex ("Main Texture (Greyscale)", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,0,0,1)    // Color for value 0
        _Color2 ("Color 2", Color) = (1,0.5,0,1)  // Color for value 0.14
        _Color3 ("Color 3", Color) = (1,1,0,1)    // Color for value 0.28
        _Color4 ("Color 4", Color) = (0,1,0,1)    // Color for value 0.42
        _Color5 ("Color 5", Color) = (0,1,1,1)    // Color for value 0.57
        _Color6 ("Color 6", Color) = (0,0,1,1)    // Color for value 0.71
        _Color7 ("Color 7", Color) = (0.5,0,1,1)  // Color for value 0.85
        _Color8 ("Color 8", Color) = (1,0,1,1)    // Color for value 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;
            float4 _Color5;
            float4 _Color6;
            float4 _Color7;
            float4 _Color8;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Sample the grayscale texture
                float greyscaleValue = tex2D(_MainTex, i.uv).r;

                // Lerp between the 8 colors based on the grayscale value
                float4 color;
                if (greyscaleValue <= 0.14)
                {
                    color = lerp(_Color1, _Color2, greyscaleValue / 0.14);
                }
                else if (greyscaleValue <= 0.28)
                {
                    color = lerp(_Color2, _Color3, (greyscaleValue - 0.14) / 0.14);
                }
                else if (greyscaleValue <= 0.42)
                {
                    color = lerp(_Color3, _Color4, (greyscaleValue - 0.28) / 0.14);
                }
                else if (greyscaleValue <= 0.57)
                {
                    color = lerp(_Color4, _Color5, (greyscaleValue - 0.42) / 0.15);
                }
                else if (greyscaleValue <= 0.71)
                {
                    color = lerp(_Color5, _Color6, (greyscaleValue - 0.57) / 0.14);
                }
                else if (greyscaleValue <= 0.85)
                {
                    color = lerp(_Color6, _Color7, (greyscaleValue - 0.71) / 0.14);
                }
                else
                {
                    color = lerp(_Color7, _Color8, (greyscaleValue - 0.85) / 0.15);
                }

                // Return the final color with the alpha from the texture
                float alpha = tex2D(_MainTex, i.uv).a;
                return float4(color.rgb, alpha);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
