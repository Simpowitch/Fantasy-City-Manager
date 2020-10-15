// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "SmartLighting2D/AlphaMask" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Mask ("Mask (A)", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "Queue" = "Transparent+10"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
	
        Pass {   
            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 

            uniform sampler2D _MainTex;
            uniform sampler2D _Mask;

            struct VertexInput
            {
                float4 pos : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;

                output.pos = UnityObjectToClipPos(input.pos);
                output.uv = float2(input.uv.xy);

                return output;
            }

            float4 frag(VertexOutput input) : COLOR {
                float4 color = tex2D(_MainTex, input.uv);
                float4 mask = tex2D (_Mask, input.uv);

                if (color.a > 0) {
                    color.a = color.a * ((mask.r + mask.g + mask.b) * 3);
                }

                return(color);
            }

            ENDCG
        }
    }
  
}