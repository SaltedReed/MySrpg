Shader "MySrpg/Sh_RoundedRect"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Radius("Radius", Range(0, 0.5)) = 0.1
        _MainTex("MainTex", 2D) = "white"{}
    }
        SubShader
    {
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass{
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Radius;
            sampler2D _MainTex;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 col = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

                float uvX = abs(i.uv.x - 0.5);
                float uvY = abs(i.uv.y - 0.5);

                uvX = max(0, uvX - (0.5 - _Radius));
                uvY = max(0, uvY - (0.5 - _Radius));

                float resultAlpha = uvX * uvX + uvY * uvY > _Radius * _Radius ? 0 : _Color.a;

                return fixed4(col,resultAlpha);
            }
            ENDCG
        }
    }

}
