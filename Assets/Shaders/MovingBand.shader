Shader "Unlit/MovingBand"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [MaterialToggle] ReverseDirection ("ReverseDirection", Float) = 0
        Freq ("Freq", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            bool ReverseDirection;
            float Freq;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float offset = ReverseDirection ? _Time.z : -_Time.z;
                if (frac(i.uv.x * Freq + offset) < 0.5) {
                    col = fixed4(0, 0, 0, 1);
                }
                
                
                // col = fixed4(abs(_SinTime.z), abs(_SinTime.z), abs(_SinTime.z), 1);

                // col = fixed4(frac(_Time.z), frac(_Time.z), frac(_Time.z), 1);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
