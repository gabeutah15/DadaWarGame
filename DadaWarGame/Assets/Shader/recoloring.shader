// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/recoloring" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _KeyColor01("Key Color #1", Color) = (1,0,0,1)
        _TargetColor01("Target Color #1", Color) = (1,0,1,1)
        _Threshold("Threshold", Range(0.0,1.0)) = 0.1
    }
    SubShader{
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off Lighting Off ZWrite Off
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            //  =============================================
            //        IO structures
            //  =============================================

            struct vertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct fragmentInput {
                float4 position : SV_POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            //  =============================================
            //        uniforms / varyings
            //  =============================================

            uniform sampler2D _MainTex;
            uniform float4 _KeyColor01;
            uniform float4 _TargetColor01;
            uniform float  _Threshold;
            //  =============================================
            //        vertex shader
            //  =============================================
            fragmentInput vert(vertexInput i)
            {
                fragmentInput o;
                o.position = UnityObjectToClipPos(i.vertex);
                o.texcoord0 = i.texcoord0;
                return o;
            }

            //  =============================================
            //        pixel shader
            //  =============================================

            float4 frag(fragmentInput i) : COLOR
            {
                // fetch pixel color from texture
                float4 texColor = tex2D(_MainTex, i.texcoord0.xy);
                // luminance weights
                const float4 lumWeights = float4(.2126, .7152, .0722, 0.0);

                // compute color luminance
                float luminance = dot(texColor, lumWeights);
                // compute the new color using input target color
                float4 newColor = float4(luminance * _TargetColor01.rgb, 1.0);
                // adjust the alpha values
                float alpha = texColor.a * 2.0;
                // feathering effect for alpha masking
                // alpha_lerp = 1.0 - step(alpha_lerp, 0.1);
                // lerp the result to avoid if statement and taking advantage of anti-aliasing
                return lerp(float4(texColor.rgb, alpha), newColor, alpha - 1.0);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
