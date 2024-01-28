Shader "Posterize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Levels ("Quantization Levels", Float) = 8
        _Range ("Color Range", Float) = 8
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            int n = 8;

            // 8 x 8 bayer mat
            float bayer_mat[8][8] = {
                {-0.5, 0.0, -0.375, 0.125, -0.46875, 0.03125, -0.34375, 0.15625},
                {0.25, -0.25, 0.375, -0.125, 0.28125, -0.21875, 0.40625, -0.09375},
                {-0.3125, 0.1875, -0.4375, 0.0625, -0.28125, 0.21875, -0.40625, 0.09375},
                {0.4375, -0.0625, 0.3125, -0.1875, 0.46875, -0.03125, 0.34375, -0.15625},
                {-0.453125, 0.046875, -0.328125, 0.171875, -0.484375, 0.015625, -0.359375, 0.140625},
                {0.296875, -0.203125, 0.421875, -0.078125, 0.265625, -0.234375, 0.390625, -0.109375},
                {-0.265625, 0.234375, -0.390625, 0.109375, -0.296875, 0.203125, -0.421875, 0.078125},
                {0.484375, -0.015625, 0.359375, -0.140625, 0.453125, -0.046875, 0.328125, -0.171875}
            };


            sampler2D _MainTex;
            float _Levels;
            float _Range;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float bayer_val = bayer_mat[i.uv.y % n][i.uv.x % n];
                fixed4 newCol = col + _Range * bayer_val;
                col = floor(((32 - 1) * newCol) + 0.5) / (32 - 1);

                // float grayscale = max(col.r, max(col.g, col.b));
                
                // // Calculate the floor of the gradient
                // float lower = floor(grayscale * _Levels) / _Levels;
                // float lowerDiff = abs(grayscale - lower);

                // // Calculate the ceiling of the gradient
                // float upper = ceil(grayscale * _Levels) / _Levels;
                // float upperDiff = abs(upper - grayscale);

                // // Calculate whether the ceiling or the floor is closer
                // float level = lowerDiff <= upperDiff ? lower : upper;
                // float mult = level / grayscale;

                // // Apply the final color
                // col.rgb *= mult;
                return col;
            }
            ENDCG
        }
    }
    Fallback Off
}
