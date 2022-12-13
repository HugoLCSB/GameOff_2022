Shader "Unlit/ScreenTint"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Render Image", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }

        Pass
        {
            //Blend One One //additive
            //Blend DstColor Zero  //multiplicative
            //ZWrite Off

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;

            /*struct appdata
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
            }*/

            fixed4 frag (v2f_img i) : SV_Target
            {
                float4 col = tex2D( _MainTex, i.uv );
				return col * _Color;
            }
            ENDCG
        }
    }
}
