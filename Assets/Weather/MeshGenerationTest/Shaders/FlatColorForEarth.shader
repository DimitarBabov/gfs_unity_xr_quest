Shader "Custom/FlatColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlobalAlpha ("Global Alpha", Float) = 1.0 // Global alpha for controlling overall transparency
        _Color1 ("Color 1", Color) = (1, 0, 0, 1) // Color for value 0
        _Color2 ("Color 2", Color) = (0, 1, 0, 1) // Color for value 0.2
        _Color3 ("Color 3", Color) = (0, 0, 1, 1) // Color for value 0.4
        _Color4 ("Color 4", Color) = (1, 1, 0, 1) // Color for value 0.6
        _Color5 ("Color 5", Color) = (0, 1, 1, 1) // Color for value 0.8
        _Color6 ("Color 6", Color) = (1, 0, 1, 1) // Color for value 1
        _Range1 ("Range 1", Float) = 0.2 // Range for transition between Color1 and Color2
        _Range2 ("Range 2", Float) = 0.4 // Range for transition between Color2 and Color3
        _Range3 ("Range 3", Float) = 0.6 // Range for transition between Color3 and Color4
        _Range4 ("Range 4", Float) = 0.8 // Range for transition between Color4 and Color5
        _Range5 ("Range 5", Float) = 1.0 // Range for transition between Color5 and Color6
        _uvEdgeAdjustment ("UV Edge Adjustment", Float) = 0.05 // Adjustable edge clamping
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha // Enable blending for transparency
        ZWrite Off // Disable depth writing for transparency
        //Cull Off // Disable culling

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
            float _GlobalAlpha; // Global alpha value for controlling overall transparency
            float _uvEdgeAdjustment; // Adjustable UV edge clamping
            float4 _Color1, _Color2, _Color3, _Color4, _Color5, _Color6; // Colors with adjustable alpha
            float _Range1, _Range2, _Range3, _Range4, _Range5; // Adjustable ranges

            // Function to interpolate between 6 colors based on greyscale value with smoothstep for smooth interpolation
            float4 GetGreyscaleColor(float greyscale)
            {
                // Smooth transitions using smoothstep
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
                else
                {
                    return lerp(_Color5, _Color6, smoothstep(_Range4, _Range5, greyscale));
                }
            }

            v2f vert(appdata v)
            {
                v2f o;

                // Clamp UVs with adjustable margin to avoid sampling extreme edges
                float2 clampedUV = clamp(v.uv, 0.0 + _uvEdgeAdjustment, 1.0 - _uvEdgeAdjustment);

                // Sample greyscale value from the texture using clamped UV coordinates (using the red channel as greyscale)
                float greyscaleValue = tex2Dlod(_MainTex, float4(clampedUV, 0, 0)).r;

                // Pass the color based on the greyscale value
                o.color = GetGreyscaleColor(greyscaleValue);

                // Transform vertex position to clip space
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Pass the original UV for further use
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Return the color passed from the vertex shader with global alpha applied
                float4 finalColor = i.color;
                finalColor.a *= _GlobalAlpha; // Apply global alpha
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}