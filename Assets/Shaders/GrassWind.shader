// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GrassWind" {

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _WaveSpeed("Wave Speed", float) = 1.0
        _WaveAmp("Wave Amp", float) = 1.0
		_HeightCutoff("Height Cutoff", float) = 1.2
        _WindSpeed("Wind Speed", vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "IGNOREPROJECTOR" = "true" "RenderType"="Transparent" "Queue"="Transparent+500" } //"LIGHTMODE"="FORWARDBASE" "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="TransparentCutout" "DisableBatching"="LodFading"
        LOD 100

        Pass
        {   
            //ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            float _HeightCutoff;
            float _WaveSpeed;
            float _WaveAmp;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if you want to access instanced properties in fragment Shader.
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            float4 CalculateWind(float4 oldPos) {
                float4 newPos = oldPos;
                
                float windSample = _WaveSpeed * _Time.y;

                float scale = 0.001; // Why they so small?!
                // Boolean typ value 
                float shouldAnimate = oldPos.y > (_HeightCutoff * scale);
                
                newPos.z += sin(windSample) * _WaveAmp * scale * shouldAnimate;
                newPos.x += cos(windSample) * _WaveAmp * scale * shouldAnimate;

                return newPos;
            }

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o); // necessary only if you want to access instanced properties in the fragment Shader.


                //copy them so we can change them (demonstration purpos only)
                float4x4 newViewMatrix = UNITY_MATRIX_V;

                //break out the axis
                float3 right = normalize(newViewMatrix._m00_m01_m02);
                float3 up = float3(0, 1, 0);
                float3 forward = normalize(newViewMatrix._m20_m21_m22);
                //get the rotation parts of the matrix
                float4x4 rotationMatrix = float4x4(
                    right, 0,
                    up, 0,
                    forward, 0,
                    0, 0, 0, 1);

                //the inverse of a rotation matrix happens to always be the transpose
                float4x4 rotationMatrixInverse = transpose(rotationMatrix);

                //apply the rotationMatrixInverse, model, view and projection matrix
                float4 pos = v.vertex;
                pos = mul(rotationMatrixInverse, pos);
                pos = CalculateWind(pos);
                pos = UnityObjectToClipPos(pos);
                o.vertex = pos;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_SETUP_INSTANCE_ID(i); // necessary only if any instanced properties are going to be accessed in the fragment Shader.
                return col;
            }
            ENDCG
        }
    }
}