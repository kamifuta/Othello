Shader "Unlit/DiscShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Offset("Offset",Range(-1.0,1.0)) = 0
        _Color1("Color1",Color) = (1,1,1,1)
        _Color2("Color2",Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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
                float4 objectVertex : TEXCOORD1;
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Offset;
            float4 _Color1;
            float4 _Color2;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.objectVertex = v.vertex;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col = (i.objectVertex.y > _Offset) ? col *= _Color1 : _Color2;
                return col;
            }
            ENDCG
        }
    }
}
