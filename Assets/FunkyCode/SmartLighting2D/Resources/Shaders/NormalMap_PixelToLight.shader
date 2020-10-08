// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "SmartLighting2D/NormalMapPixelToLight" {
    Properties
    {
        _MainTex ("Diffuse Texture", 2D) = "white" {}
        _Bump ("Bump", 2D) = "Bump" {}

        _LightSize ("LightSize", Float) = 1

        _LightX ("LightX", Float) = 1
        _LightY ("LightY", Float) = 1
        _LightZ ("LightZ", Float) = 1
        _LightIntensity ("LightIntensity", Float) = 1
        _LightColor("LightColor", float) = 1
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {    
            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _Bump;
  
            uniform float _LightSize;
            uniform float _LightX;
            uniform float _LightY;
            uniform float _LightZ;
            uniform float _LightIntensity;
            uniform float _LightColor;

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };

            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;

                output.pos = UnityObjectToClipPos(input.vertex);
                output.posWorld = mul(unity_ObjectToWorld, input.vertex);

                output.uv = float2(input.uv.xy);
                output.color = input.color;

                return output;
            }

            float4 frag(VertexOutput input) : COLOR {
                float alpha = tex2D(_MainTex, input.uv).a;

                float3 normalDirection = (tex2D(_Bump, input.uv).xyz - 0.5f) * 2.0f;

                float4 normalColor = float4(normalDirection.xyz, 1.0f);

                normalDirection = float3(mul(normalColor, unity_WorldToObject).xyz);

                normalDirection.x *= _LightX;
                normalDirection.y *= _LightY;
        

                normalDirection.z *= -1;
                normalDirection = normalize(normalDirection);

                float3 posWorld = float3(input.posWorld.xyz);
                posWorld.z = 0;

                float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) );


           
                float3 vertexToLightSource = float3(0,0, -_LightZ) - posWorld.xyz;

     
                float distance = 1; // length(vertexToLightSource); 
                float lightUV = 1 - ((distance - _LightSize) / _LightSize);

                float color = _LightColor;      
                lightUV *= color;

               //  float distance = length(vertexToLightSource);
               // float lightUV = 1 - ((distance - _LightSize) / _LightSize);

                float attenuation = sqrt(distance * distance) * _LightIntensity; 
                float3 lightDirection = normalize(vertexToLightSource);

                float normalDotLight = dot(normalDirection, lightDirection);
                float diffuseLevel = attenuation * max(0.0f, normalDotLight);

                float specularLevel;
                if (normalDotLight < 0.0f) {
                    specularLevel = 0.0f;
                } else {
                    specularLevel = attenuation * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), float3(0.0f, 0.0f, -1.0f))), 10);
                }

                float3 diffuseReflection = diffuseLevel * lightUV;
                float3 specularReflection = specularLevel * lightUV;

                return float4(diffuseReflection + specularReflection, alpha);
             }

             ENDCG
        }
    }
}
