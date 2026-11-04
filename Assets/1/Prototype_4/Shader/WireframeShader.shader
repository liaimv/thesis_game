Shader "Unlit/WireframeShader"
{
    //Tutorial from https://www.youtube.com/watch?v=FQFk2hR89XA
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WireframeColor ("Wireframe Color", color) = (1.0, 1.0, 1.0, 1.0)
        _WireframeScale ("Wireframe Scale", float) = 1.5

        [KeywordEnum(BASIC, FIXEDWIDTH, ANTIALIASING)] _WIREFRAME ("Wireframe rendering type", Integer) = 0
        [Toggle] _QUADS("Show only quads", Integer) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            //Don't depth test or cull so it can draw every wireframe line
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #pragma shader_feature _WIREFRAME_BASIC _WIREFRAME_FIXEDWITH _WIREFRAME_ANTIALIASING 
            #pragma shader_feature _QUADS_ON

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
    
            struct g2f {
                float4 pos : SV_POSITION;
                float3 barycentric : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                // o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2f IN[3], inout TriangleStream<g2f> triStream) {
                float3 modifier = float3(0.0, 0.0, 0.0);

                #if _QUADS_ON
                    float edgeLength0 = distance(IN[1].vertex, IN[2].vertex);
                    float edgeLength1 = distance(IN[0].vertex, IN[2].vertex);
                    float edgeLength2 = distance(IN[0].vertex, IN[1].vertex);

                    if ((edgeLength0 > edgeLength1) && (edgeLength0 > edgeLength2)) {
                        modifier = float3(1.0, 0.0, 0.0);
                    }
                    else if ((edgeLength1 > edgeLength0) && (edgeLength1 > edgeLength2)) {
                        modifier = float3(0.0, 1.0, 0.0);
                    }
                    else if ((edgeLength2 > edgeLength0) && (edgeLength2 > edgeLength1)) {
                        modifier = float3(0.0, 0.0, 1.0);
                    }
                #endif

                g2f o;

                o.pos = UnityObjectToClipPos(IN[0].vertex);
                o.barycentric = float3(1.0, 0.0, 0.0) + modifier;
                triStream.Append(o);

                o.pos = UnityObjectToClipPos(IN[1].vertex);
                o.barycentric = float3(0.0, 1.0, 0.0) + modifier;
                triStream.Append(o);

                o.pos = UnityObjectToClipPos(IN[2].vertex);
                o.barycentric = float3(0.0, 0.0, 1.0) + modifier;
                triStream.Append(o);
            }

            fixed4 _WireframeColor;
            float _WireframeScale;

            fixed4 frag (g2f i) : SV_Target
            {
                #if _WIREFRAME_BASIC
                    float closest = min(i.barycentric.x, min(i.barycentric.y, i.barycentric.z));
                    float alpha = step(closest, _WireframeScale / 20.0);

                #elif _WIREFRAME_FIXEDWITH
                    float3 unitWidth = fwidth(i.barycentric);
                    float3 edge = step(i.barycentric, unitWidth * _WireframeScale);
                    float alpha = max(edge.x, max(edge.y, edge.z));

                #elif _WIREFRAME_ANTIALIASING
                    float3 unitWidth = fwidth(i.barycentric);
                    float3 aliased = smoothstep(float3(0.0, 0.0, 0.0), unitWidth * _WireframeScale, i.barycentric);
                    float alpha = 1 - min(aliased.x, min(aliased.y, aliased.z));
                #endif

                return fixed4(_WireframeColor.r, _WireframeColor.g, _WireframeColor.b, alpha);
            }
            ENDCG
        }
    }
}
