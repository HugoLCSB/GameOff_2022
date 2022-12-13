Shader "Unlit/ScanLines"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _lineStart ("Line Start", Range(0,1)) = 1
        _lineEnd ("Line End", Range(0,1)) = 0 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            float _lineStart;
            float _lineEnd;

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
                //return _Color;
                //float4 outColor = lerp(float4(0,0,0,1), _Color, i.uv.y);

                float linePos = cos((i.uv.y) +(_Time.w * 5));
                //float t = saturate(InverseLerp(_lineStart, _lineEnd, i.uv.y ) * 0.5 + 0.5) ;
                //float t = frac((i.uv.y -1) + _Time.w) * -1 +1;
                
                float4 t;
                if(linePos >= 0.99999){
                    t = _Color;
                }
                else{
                    t = (0,0,0,0);
                }
                return t;


                //return outColor;
            }
            ENDCG
        }
    }
}
