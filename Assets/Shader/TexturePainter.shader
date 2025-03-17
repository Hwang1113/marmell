Shader "TNTC/TexturePainter" {   

    // Properties 섹션은 사용자 정의 변수를 선언하는 부분입니다.
    Properties {
        // Painter Color 속성은 색상 값을 받아서 페인팅에 사용됩니다.
        _PainterColor ("Painter Color", Color) = (0, 0, 0, 0)  // 기본값은 투명한 검정색
    }

    SubShader {
        // Cull, ZWrite, ZTest 옵션 설정
        Cull Off          // Back-face culling을 끔 (앞면/뒷면 모두 렌더링)
        ZWrite Off        // 깊이 버퍼에 값을 기록하지 않음
        ZTest Off         // 깊이 테스트를 비활성화

        Pass {
            CGPROGRAM
            // vertex와 fragment shader를 지정
            #pragma vertex vert
            #pragma fragment frag

            // Unity의 기본 쉐이더 라이브러리 포함
            #include "UnityCG.cginc"

            // 텍스처와 매개변수 선언
            sampler2D _MainTex;  // 텍스처
            float4 _MainTex_ST;  // 텍스처 변환 정보

            // 페인팅에 필요한 매개변수
            float3 _PainterPosition;  // 페인팅의 중심 위치
            float _Radius;            // 페인팅 반경
            float _Hardness;          // 페인팅의 경도 (얼마나 부드럽게 적용될지)
            float _Strength;          // 페인팅 강도
            float4 _PainterColor;     // 페인팅 색상
            float _PrepareUV;         // UV 준비를 위한 조건 (디버깅 목적일 수 있음)

            // Vertex와 Fragment에서 사용할 데이터 구조체 정의
            struct appdata {
                float4 vertex : POSITION;  // 위치 정보
                float2 uv : TEXCOORD0;     // 텍스처 좌표
            };

            struct v2f {
                float4 vertex : SV_POSITION;  // 화면 공간 위치
                float2 uv : TEXCOORD0;        // 텍스처 좌표
                float4 worldPos : TEXCOORD1;  // 월드 공간 위치
            };

            // 페인트 마스크를 계산하는 함수 (페인팅 효과를 적용할 영역을 결정)
            float mask(float3 position, float3 center, float radius, float hardness) {
                // 중심과의 거리를 계산
                float m = distance(center, position);
                // 거리 값에 따라 페인팅 효과가 부드럽게 적용되도록 처리 (smoothstep)
                return 1 - smoothstep(radius * hardness, radius, m);    
            }

            // vertex shader (정점 셰이더)
            v2f vert(appdata v) {
                v2f o;
                // 월드 공간으로 위치 변환
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                
                // UV 좌표 변환 (디버깅 목적일 가능성 있음)
                float4 uv = float4(0, 0, 0, 1);
                uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * float2(2, 2) - float2(1, 1));
                o.vertex = uv;  // 변환된 위치 반환
                return o;
            }

            // fragment shader (픽셀 셰이더)
            fixed4 frag(v2f i) : SV_Target {
                // _PrepareUV가 0보다 크면, 디버깅용으로 파란색을 반환 (UV 계산을 위한 조건)
                if (_PrepareUV > 0) {
                    return float4(0, 0, 1, 1);  // 파란색 (디버깅)
                }

                // 기본 텍스처 색상 샘플링
                float4 col = tex2D(_MainTex, i.uv);

                // 페인트 영역을 계산 (마스크 함수 사용)
                float f = mask(i.worldPos, _PainterPosition, _Radius, _Hardness);

                // 페인트 경계 강도 계산 (강도 * 페인팅 효과)
                float edge = f * _Strength;

                // 원본 텍스처 색상과 페인팅 색상 사이를 보간하여 반환
                return lerp(col, _PainterColor, edge);
            }
            ENDCG
        }
    }
}
