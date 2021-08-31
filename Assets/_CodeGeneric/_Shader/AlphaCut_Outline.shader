Shader "Unlit/AlphaCut_Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTint ("Tint", Color) = (1,1,1,1)
        _TransparentColor ("_TransparentColor", Color) = (1,1,1,1)
        _CutoffValue ("_CutoffValue", Float) = 0.1
        _ParallaxOffset("Parallax Offset", Vector) = (1,0,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _ColorTint;
            float4 _TransparentColor;
            float _CutoffValue;
            float _ParallaxSpeed;
            float2 _ParallaxOffset;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv + _ParallaxOffset);

                float distanceToNullColor = 1 - length(_TransparentColor.xyz - col.xyz);

                fixed alphaValue = step(distanceToNullColor, _CutoffValue);

                float lerpValue = 1-fwidth(col.xy);

                float3 colour = lerp(_TransparentColor,_ColorTint,lerpValue); 

                return float4(colour, alphaValue);
            }
            ENDCG
        }
    }
}