Shader "Unlit/ScanLines/Flowing"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Render Image", 2D) = "black" {}
        _UnscaledTime ("Unscaled Time", float) = 0
        _Color ("Color", Color) = (1,1,1,1)
        _LineThick ("Line Thickness", Range(0.8,1)) = 0.99999
        _SpeedMult ("Speed Multiplier", float) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            //Blend One One //additive
            Blend DstColor Zero  //multiplicative
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _LineThick;
            float _SpeedMult;
            float _UnscaledTime;

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

            float InverseLerp(float a, float b, float v){
                return (v-a)/(b-a);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float linePos = cos((i.uv.y) +(/*_Time.w*/_UnscaledTime * _SpeedMult));
                float4 t;

                //return linePos;
                if(linePos >= _LineThick){
                    return  tex2D( _MainTex, i.uv ) + _Color;
                }
                /*if((linePos <= (1 -_LineThick+0.1)) && (linePos >= -(1 -_LineThick+0.1))){
                    return  tex2D( _MainTex, i.uv ) + (.1,0,0,1);
                }*/
                else if((linePos <= -_LineThick) && linePos < 0){
                    return tex2D( _MainTex, i.uv ) + _Color;
                }
                else{
                    return tex2D( _MainTex, i.uv );
                }
                //return t;

                //float4 tex = tex2D( _MainTex, i.uv );
                //return _Color;
                //return tex + t;
            }
            ENDCG
        }
    }
}
