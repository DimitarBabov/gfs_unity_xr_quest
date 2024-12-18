Shader "Custom/DisplaceByTwoTexturesWith10Colors"
{
    Properties
    {
        // First texture and associated properties
        _MainTex1 ("Heightmap 1", 2D) = "white" {}
        _MainTex2 ("Heightmap 2", 2D) = "white" {}

        // Shared properties
        _HeightScale ("Height Scale", Float) = 10.0
        _Trim1 ("Trim Value 1", Range(0.0, 1.0)) = 0.1
        _Trim2 ("Trim Value 2", Range(0.0, 1.0)) = 0.1
        _GlobalAlpha ("Global Alpha", Float) = 1.0

        // Colors and ranges for the first texture (10 colors)
        _Color1_1 ("Color 1 (Tex 1)", Color) = (1, 0, 0, 1)
        _Color2_1 ("Color 2 (Tex 1)", Color) = (0, 1, 0, 1)
        _Color3_1 ("Color 3 (Tex 1)", Color) = (0, 0, 1, 1)
        _Color4_1 ("Color 4 (Tex 1)", Color) = (1, 1, 0, 1)
        _Color5_1 ("Color 5 (Tex 1)", Color) = (0, 1, 1, 1)
        _Color6_1 ("Color 6 (Tex 1)", Color) = (1, 0, 1, 1)
        _Color7_1 ("Color 7 (Tex 1)", Color) = (0.5, 0.5, 0.5, 1)
        _Color8_1 ("Color 8 (Tex 1)", Color) = (0.8, 0.3, 0.3, 1)
        _Color9_1 ("Color 9 (Tex 1)", Color) = (0.3, 0.8, 0.3, 1)
        _Color10_1 ("Color 10 (Tex 1)", Color) = (0.3, 0.3, 0.8, 1)
        [Range(0.0, 1.0)] _Range1_1 ("Range 1 (Tex 1)", Float) = 0.1
        [Range(0.0, 1.0)] _Range2_1 ("Range 2 (Tex 1)", Float) = 0.2
        [Range(0.0, 1.0)] _Range3_1 ("Range 3 (Tex 1)", Float) = 0.3
        [Range(0.0, 1.0)] _Range4_1 ("Range 4 (Tex 1)", Float) = 0.4
        [Range(0.0, 1.0)] _Range5_1 ("Range 5 (Tex 1)", Float) = 0.5
        [Range(0.0, 1.0)] _Range6_1 ("Range 6 (Tex 1)", Float) = 0.6
        [Range(0.0, 1.0)] _Range7_1 ("Range 7 (Tex 1)", Float) = 0.7
        [Range(0.0, 1.0)] _Range8_1 ("Range 8 (Tex 1)", Float) = 0.8
        [Range(0.0, 1.0)] _Range9_1 ("Range 9 (Tex 1)", Float) = 1.0

        // Colors and ranges for the second texture (10 colors)
        _Color1_2 ("Color 1 (Tex 2)", Color) = (1, 0, 0, 1)
        _Color2_2 ("Color 2 (Tex 2)", Color) = (0, 1, 0, 1)
        _Color3_2 ("Color 3 (Tex 2)", Color) = (0, 0, 1, 1)
        _Color4_2 ("Color 4 (Tex 2)", Color) = (1, 1, 0, 1)
        _Color5_2 ("Color 5 (Tex 2)", Color) = (0, 1, 1, 1)
        _Color6_2 ("Color 6 (Tex 2)", Color) = (1, 0, 1, 1)
        _Color7_2 ("Color 7 (Tex 2)", Color) = (0.5, 0.5, 0.5, 1)
        _Color8_2 ("Color 8 (Tex 2)", Color) = (0.8, 0.3, 0.3, 1)
        _Color9_2 ("Color 9 (Tex 2)", Color) = (0.3, 0.8, 0.3, 1)
        _Color10_2 ("Color 10 (Tex 2)", Color) = (0.3, 0.3, 0.8, 1)
        [Range(0.0, 1.0)] _Range1_2 ("Range 1 (Tex 2)", Float) = 0.1
        [Range(0.0, 1.0)] _Range2_2 ("Range 2 (Tex 2)", Float) = 0.2
        [Range(0.0, 1.0)] _Range3_2 ("Range 3 (Tex 2)", Float) = 0.3
        [Range(0.0, 1.0)] _Range4_2 ("Range 4 (Tex 2)", Float) = 0.4
        [Range(0.0, 1.0)] _Range5_2 ("Range 5 (Tex 2)", Float) = 0.5
        [Range(0.0, 1.0)] _Range6_2 ("Range 6 (Tex 2)", Float) = 0.6
        [Range(0.0, 1.0)] _Range7_2 ("Range 7 (Tex 2)", Float) = 0.7
        [Range(0.0, 1.0)] _Range8_2 ("Range 8 (Tex 2)", Float) = 0.8
        [Range(0.0, 1.0)] _Range9_2 ("Range 9 (Tex 2)", Float) = 1.0

        _uvEdgeAdjustment ("UV Edge Adjustment", Float) = 0.05 // Adjustable edge clamping
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Add semantics for each member in the v2f struct
            struct v2f
            {
                float2 uv : TEXCOORD0;        // Texture coordinates
                float4 pos : SV_POSITION;     // Clip-space position
                float4 color1 : COLOR0;       // Color from texture 1
                float4 color2 : COLOR1;       // Color from texture 2
            };

            sampler2D _MainTex1, _MainTex2;
            float _HeightScale;
            float _Trim1, _Trim2;
            float _GlobalAlpha;
            float _uvEdgeAdjustment;

            // Colors and ranges for texture 1
            float4 _Color1_1, _Color2_1, _Color3_1, _Color4_1, _Color5_1, _Color6_1, _Color7_1, _Color8_1, _Color9_1, _Color10_1;
            float _Range1_1, _Range2_1, _Range3_1, _Range4_1, _Range5_1, _Range6_1, _Range7_1, _Range8_1, _Range9_1;

            // Colors and ranges for texture 2
            float4 _Color1_2, _Color2_2, _Color3_2, _Color4_2, _Color5_2, _Color6_2, _Color7_2, _Color8_2, _Color9_2, _Color10_2;
            float _Range1_2, _Range2_2, _Range3_2, _Range4_2, _Range5_2, _Range6_2, _Range7_2, _Range8_2, _Range9_2;

            // Function to interpolate between 10 colors based on greyscale value
            float4 GetGreyscaleColor(float greyscale, float4 color1, float4 color2, float4 color3, float4 color4, float4 color5, float4 color6, float4 color7, float4 color8, float4 color9, float4 color10, float range1, float range2, float range3, float range4, float range5, float range6, float range7, float range8, float range9)
            {
                if (greyscale < range1)
                {
                    return lerp(color1, color2, smoothstep(0.0, range1, greyscale));
                }
                else if (greyscale < range2)
                {
                    return lerp(color2, color3, smoothstep(range1, range2, greyscale));
                }
                else if (greyscale < range3)
                {
                    return lerp(color3, color4, smoothstep(range2, range3, greyscale));
                }
                else if (greyscale < range4)
                {
                    return lerp(color4, color5, smoothstep(range3, range4, greyscale));
                }
                else if (greyscale < range5)
                {
                    return lerp(color5, color6, smoothstep(range4, range5, greyscale));
                }
                else if (greyscale < range6)
                {
                    return lerp(color6, color7, smoothstep(range5, range6, greyscale));
                }
                else if (greyscale < range7)
                {
                    return lerp(color7, color8, smoothstep(range6, range7, greyscale));
                }
                else if (greyscale < range8)
                {
                    return lerp(color8, color9, smoothstep(range7, range8, greyscale));
                }
                else
                {
                    return lerp(color9, color10, smoothstep(range8, range9, greyscale));
                }
            }

            v2f vert(appdata v)
            {
                v2f o;

                float2 clampedUV = clamp(v.uv, 0.0 + _uvEdgeAdjustment, 1.0 - _uvEdgeAdjustment);

                // Sample greyscale values from both textures
                float greyscaleValue1 = tex2Dlod(_MainTex1, float4(clampedUV, 0, 0)).r;
                float greyscaleValue2 = tex2Dlod(_MainTex2, float4(clampedUV, 0, 0)).r;

                // Calculate vertex displacement (accumulative)
                float height1 = greyscaleValue1 * _HeightScale;
                float height2 = greyscaleValue2 * _HeightScale;

                // Adjust the vertex's Y position based on both heights
                float4 modifiedVertex = v.vertex;
                modifiedVertex.y += height1 + height2;

                o.color1 = GetGreyscaleColor(greyscaleValue1, _Color1_1, _Color2_1, _Color3_1, _Color4_1, _Color5_1, _Color6_1, _Color7_1, _Color8_1, _Color9_1, _Color10_1, _Range1_1, _Range2_1, _Range3_1, _Range4_1, _Range5_1, _Range6_1, _Range7_1, _Range8_1, _Range9_1);
                o.color2 = GetGreyscaleColor(greyscaleValue2, _Color1_2, _Color2_2, _Color3_2, _Color4_2, _Color5_2, _Color6_2, _Color7_2, _Color8_2, _Color9_2, _Color10_2, _Range1_2, _Range2_2, _Range3_2, _Range4_2, _Range5_2, _Range6_2, _Range7_2, _Range8_2, _Range9_2);

                o.pos = UnityObjectToClipPos(modifiedVertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Combine the colors of both textures
                float4 finalColor1 = i.color1;
                float4 finalColor2 = i.color2;

                float4 finalColor = lerp(finalColor1, finalColor2, 0.5); // Combine both textures equally

                // Apply trim for both textures
                float greyscaleValue1 = tex2Dlod(_MainTex1, float4(i.uv, 0, 0)).r;
                float greyscaleValue2 = tex2Dlod(_MainTex2, float4(i.uv, 0, 0)).r;

                if (greyscaleValue1 < _Trim1 || greyscaleValue2 < _Trim2)
                {
                    finalColor.a = 0.0; // Set alpha to 0 if below trim value
                }

                // Apply global alpha
                finalColor.a *= _GlobalAlpha;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
