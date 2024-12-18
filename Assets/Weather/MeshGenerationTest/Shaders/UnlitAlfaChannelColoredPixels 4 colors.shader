Shader "Custom/UnlitTextureWithAlphaAndLerp"
{
    Properties
    {
        _MainTex ("Main Texture (Greyscale)", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,0,0,1)    // Color for value 0
        _Color2 ("Color 2", Color) = (0,1,0,1)    // Color for value 0.33
        _Color3 ("Color 3", Color) = (0,0,1,1)    // Color for value 0.66
        _Color4 ("Color 4", Color) = (1,1,0,1)    // Color for value 1
        _GlobalAlpha ("Global Alpha", Float) = 1.0  // Alpha value to control transparency of all pixels
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        // Enable blending for transparency
        Blend SrcAlpha OneMinusSrcAlpha

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
            float _GlobalAlpha;  // Global alpha parameter

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

                // Lerp between the colors based on the grayscale value
                float4 color;
                if (greyscaleValue <= 0.33)
                {
                    color = lerp(_Color1, _Color2, greyscaleValue / 0.33);
                }
                else if (greyscaleValue <= 0.66)
                {
                    color = lerp(_Color2, _Color3, (greyscaleValue - 0.33) / 0.33);
                }
                else
                {
                    color = lerp(_Color3, _Color4, (greyscaleValue - 0.66) / 0.34);
                }

                // Get the alpha from the texture and apply the global alpha
                float alpha = tex2D(_MainTex, i.uv).a * _GlobalAlpha;

                // Return the final color with modified alpha
                return float4(color.rgb, alpha);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
