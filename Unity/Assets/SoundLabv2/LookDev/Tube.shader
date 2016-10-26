Shader "Custom/Tube" {
	
	CGINCLUDE



	ENDCG



	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_TWOPI("float", Float) = 6.283185

		_Radius("Radius", Range(0,10)) = 1
		_DCOffset("DC Offset", Vector) = (0,0,0,0)
		_Amp("Amp", Vector) = (1,1,1,1)
		_Freq("Freq", Vector) = (1,1,1,1)
		_Phase("Phase", Vector) = (0,0,0,0)
		_Movement("Movement", Range(0, 100)) = 0.0

		_Bounds("Bounds", Float) = 1

		_Gaze("Center (World) ", Vector) = (0,0,0,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile CNOISE PNOISE SNOISE SNOISE_AGRAD SNOISE_NGRAD
			#pragma multi_compile _ THREED
			#pragma multi_compile _ FRACTAL

			#include "UnityCG.cginc"
			#include "Noise/SimplexNoise3D.cginc"
			






			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;

			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD1;
				float4 objPos : TEXCOORD2;
				float R : FLOAT;
				float R2 : FLOAT;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _TWOPI;

			half _Radius;
			half3 _DCOffset;
			half3 _Amp;
			half3 _Freq;
			half3 _Phase;
			half _Movement;

			half4 _Gaze;


			v2f vert(appdata v)
			{
				v2f o;

				o.objPos =  v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.normal = mul(unity_ObjectToWorld, v.normal);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				float3 I = normalize(o.worldPos - _WorldSpaceCameraPos.xyz);
				o.R = saturate( pow(1 + dot(I, normalize(o.normal)), 1 ) );
				o.R2 = o.R;

				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				const float epsilon = 0.0001;

				half2 uv = half2( IN.objPos.x, IN.objPos.y/2. );

				float o = 0.5;
				float s = 1;
				float w = 0.25;

				for (int i = 0; i < 2; i++)
				{
					float3 coord = float3( uv * s, _Time.y*.1);
					float3 period = float3(s, s, 1.0) * 2.0;

					o += snoise(coord) * w;

					s *= 2.0;
					w *= 0.5;
				}


				float radius = 1 - pow(saturate((distance(_Gaze.xyz, IN.worldPos)) * _Radius), 2);

				float d = 1 - saturate(distance(_Gaze.xyz, IN.worldPos));

				o = o * d;
				saturate(o);

				// sample the texture
				fixed4 col = tex2D(_MainTex, IN.uv);

				float r = cos(_Movement*_Time + o * _TWOPI * 2 * _Freq[0] - _Phase[0] * _TWOPI) * _Amp[0] + _DCOffset[0];
				float g = cos(_Movement*_Time + o * _TWOPI * 2 * _Freq[1] - _Phase[1] * _TWOPI) * _Amp[1] + _DCOffset[1];
				float b = cos(_Movement*_Time + o *  _TWOPI * 2 * _Freq[2] - _Phase[2] * _TWOPI) * _Amp[2] + _DCOffset[2];


				float3 I = normalize(IN.worldPos - _WorldSpaceCameraPos.xyz);
				float fresnel =  pow(1 + dot(I, normalize(IN.normal) ), 2 );

				//return half4(IN.R, IN.R, IN.R, 1);
				return fresnel + saturate(1-IN.R) * half4(r, g, b, 1);
				//return half4(radius, radius, radius, 1);


			}
			ENDCG
		}
	}
}
