Shader "Custom/DisplaceByTextureWithColor"
{
    Properties
    {
        _MainTex ("Heightmap", 2D) = "white" {}
        _HeightScale ("Height Scale", Float) = 10.0
        _MinHeight ("Min Height", Float) = 0.0
        _MaxHeight ("Max Height", Float) = 100.0
        _GlobalAlpha ("Global Alpha", Float) = 1.0 // Global alpha for controlling overall transparency

        // Color properties for the 10 colors
        _Color1 ("Color 1", Color) = (1, 0, 0, 1) // Color for value 0
        _Color2 ("Color 2", Color) = (0, 1, 0, 1) // Color for value 0.1
        _Color3 ("Color 3", Color) = (0, 0, 1, 1) // Color for value 0.2
        _Color4 ("Color 4", Color) = (1, 1, 0, 1) // Color for value 0.3
        _Color5 ("Color 5", Color) = (0, 1, 1, 1) // Color for value 0.4
        _Color6 ("Color 6", Color) = (1, 0, 1, 1) // Color for value 0.5
        _Color7 ("Color 7", Color) = (0.5, 0.5, 0.5, 1) // Color for value 0.6
        _Color8 ("Color 8", Color) = (0.8, 0.3, 0.3, 1) // Color for value 0.7
        _Color9 ("Color 9", Color) = (0.3, 0.8, 0.3, 1) // Color for value 0.8
        _Color10 ("Color 10", Color) = (0.3, 0.3, 0.8, 1) // Color for value 1.0

        // Ranges for smooth transitions between the colors (with sliders)
        [Range(0.0, 0.2)] _Range1 ("Range 1", Float) = 0.1
        [Range(0.2, 0.4)] _Range2 ("Range 2", Float) = 0.2
        [Range(0.3, 0.5)] _Range3 ("Range 3", Float) = 0.3
        [Range(0.4, 0.6)] _Range4 ("Range 4", Float) = 0.4
        [Range(0.5, 0.7)] _Range5 ("Range 5", Float) = 0.5
        [Range(0.6, 0.8)] _Range6 ("Range 6", Float) = 0.6
        [Range(0.7, 0.9)] _Range7 ("Range 7", Float) = 0.7
        [Range(0.8, 1.0)] _Range8 ("Range 8", Float) = 0.8
        [Range(0.9, 1.0)] _Range9 ("Range 9", Float) = 1.0
        [Range(0.0, 1.0)] _Trim ("Trim Value", Float) = 0.1 // Trim value to control alpha transparency

        _uvEdgeAdjustment ("UV Edge Adjustment", Float) = 0.05 // Adjustable edge clamping
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha // Enable blending for transparency
        ZWrite Off // Disable depth writing for transparency
        Cull Off // Disable culling

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

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 color : COLOR; // Color based on greyscale pixel value
            };

            sampler2D _MainTex;
            float _HeightScale;
            float _MinHeight;
            float _MaxHeight;
            float _GlobalAlpha; // Global alpha value for controlling overall transparency
            float _uvEdgeAdjustment; // Adjustable UV edge clamping
            float4 _Color1, _Color2, _Color3, _Color4, _Color5, _Color6, _Color7, _Color8, _Color9, _Color10; // Colors with adjustable alpha
            float _Range1, _Range2, _Range3, _Range4, _Range5, _Range6, _Range7, _Range8, _Range9; // Adjustable ranges
             float _Trim; // Trim value to set alpha to 0 for values below it
            // Function to interpolate between 10 colors based on greyscale value with smoothstep for smooth interpolation
            float4 GetGreyscaleColor(float greyscale)
            {
                if (greyscale < _Range1)
                {
                    return lerp(_Color1, _Color2, smoothstep(0.0, _Range1, greyscale));
                }
                else if (greyscale < _Range2)
                {
                    return lerp(_Color2, _Color3, smoothstep(_Range1, _Range2, greyscale));
                }
                else if (greyscale < _Range3)
                {
                    return lerp(_Color3, _Color4, smoothstep(_Range2, _Range3, greyscale));
                }
                else if (greyscale < _Range4)
                {
                    return lerp(_Color4, _Color5, smoothstep(_Range3, _Range4, greyscale));
                }
                else if (greyscale < _Range5)
                {
                    return lerp(_Color5, _Color6, smoothstep(_Range4, _Range5, greyscale));
                }
                else if (greyscale < _Range6)
                {
                    return lerp(_Color6, _Color7, smoothstep(_Range5, _Range6, greyscale));
                }
                else if (greyscale < _Range7)
                {
                    return lerp(_Color7, _Color8, smoothstep(_Range6, _Range7, greyscale));
                }
                else if (greyscale < _Range8)
                {
                    return lerp(_Color8, _Color9, smoothstep(_Range7, _Range8, greyscale));
                }
                else
                {
                    return lerp(_Color9, _Color10, smoothstep(_Range8, _Range9, greyscale));
                }
            }

            v2f vert(appdata v)
            {
                v2f o;

                // Clamp UVs with adjustable margin to avoid sampling extreme edges
                float2 clampedUV = clamp(v.uv, 0.0 + _uvEdgeAdjustment, 1.0 - _uvEdgeAdjustment);

                // Sample greyscale value from the texture using clamped UV coordinates (using the red channel as greyscale)
                float greyscaleValue = tex2Dlod(_MainTex, float4(clampedUV, 0, 0)).r;

                // Sample height from the texture for vertex displacement
                float height = greyscaleValue * _HeightScale;

                // Adjust the vertex's Y position based on height
                float4 modifiedVertex = v.vertex;
                modifiedVertex.y += height;

                // Pass the color based on the greyscale value
                o.color = GetGreyscaleColor(greyscaleValue);

                // Transform vertex position to clip space
                o.pos = UnityObjectToClipPos(modifiedVertex);
                o.uv = v.uv; // Pass the original UV for further use
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Return the color passed from the vertex shader with global alpha applied
                float4 finalColor = i.color;
                // Apply trim: set alpha to 0 if greyscale value is less than the trim value
                float greyscaleValue = tex2Dlod(_MainTex, float4(i.uv, 0, 0)).r;
                if (greyscaleValue < _Trim)
                {
                    finalColor.a = 0.0; // Set alpha to 0 if below trim value
                }
                finalColor.a *= _GlobalAlpha; // Apply global alpha
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
