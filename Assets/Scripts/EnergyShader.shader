Shader "Unlit/EnergyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Position ("Position", Vector) = (0,0,0,0)
        _Mass ("Mass", Float) = 1.0
        _DeltaTime ("DeltaTime", Float) = 0.0
        _SpringConstant ("Spring Constant", Float) = 1.0
        _SpringPosition ("Spring Position", Vector) = (0,0,0,0)
        _SpringLength ("Spring Length", Float) = 1.0
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
            // #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 world_vertex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Position;
            float _Mass;
            float _DeltaTime;
            float _SpringConstant;
            float4 _SpringPosition;
            float _SpringLength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.world_vertex = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
                // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 viridis_cmap(float fac) {
                fac = saturate(fac);
                float t = fac;
                float t2 = t * t;
                float t3 = t2 * t;
                float t4 = t3 * t;
                float t5 = t4 * t;
                float r = saturate(0.13572138 + 4.61539260 * t - 42.66032258 * t2 + 132.13108234 * t3 - 152.94239396 * t4 + 59.28637943 * t5);
                float g = saturate(0.09140261 + 2.19418839 * t + 4.84296658 * t2 - 31.47409875 * t3 + 48.08529055 * t4 - 23.18859299 * t5);
                float b = saturate(0.10667330 + 9.43280896 * t - 42.23708223 * t2 + 59.99836499 * t3 - 35.39876996 * t4 + 7.15735319 * t5);
                return float4(r, g, b, 1.0);
            }

            float4 turbo_cmap(float fac) {
                fac = clamp(fac, 0.0, 1.0);
                float x = fac;
                float r = 0.13572137988692035 + x*(4.597363719627905  + x*(-42.327689751912274 + x*( 130.58871182451415 + x*(-150.56663492057857 + x*58.137453451135656))));
                float g = 0.09140261235958302 + x*(2.1856173378635675 + x*( 4.805204796477784  + x*(-14.019450960349728 + x*(  4.210856355081685 + x*2.7747311504638876))));
                float b = 0.10667330048674728 + x*(12.592563476453211 + x*(-60.10967551582361  + x*( 109.07449945380961 + x*( -88.50658250648611 + x*26.818260967511673))));
                return float4(r, g, b, 1.0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float3 rvec = _Position.xyz - i.world_vertex.xyz;
                float momentum_energy = 0.5 * _Mass * dot(rvec, rvec) / _DeltaTime;
                float3 springvec = _SpringPosition.xyz - i.world_vertex.xyz;
                float spring_displacement = length(springvec) - _SpringLength;
                float spring_energy = 0.5 * _SpringConstant * spring_displacement * spring_displacement;
                // fixed4 col = momentum_energy * fixed4(1, 1, 1, 1);
                fixed4 col = turbo_cmap(momentum_energy + spring_energy);
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
