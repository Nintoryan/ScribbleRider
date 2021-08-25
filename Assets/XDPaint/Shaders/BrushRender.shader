Shader "XD Paint/Brush Render" {
    Properties {
        _MainTex ("Main", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
		_RadiusX ("Radius X", Range(0, 1)) = 1
        _RadiusY ("Radius Y", Range(0, 1)) = 1
        _Hardness ("Hardness", Range(-20, 1)) = 0.9
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Cull Off Lighting Off ZTest Off ZWrite Off Fog { Color (0,0,0,0) }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            uniform float4 _MainTex_TexelSize;
            float4 _Color;
		    float _RadiusX;
            float _RadiusY;
            float _Hardness;

            float4 frag (v2f_img i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv) * _Color;
                if (i.uv.x <= _MainTex_TexelSize.x || i.uv.x >= 1.0 - _MainTex_TexelSize.x || 
                    i.uv.y <= _MainTex_TexelSize.y || i.uv.y >= 1.0 - _MainTex_TexelSize.x)
                {
                    color.a = 0;
                }
                else if (_RadiusX > 0.0001 && _RadiusY > 0.0001 && _Hardness < 1.0) 
                {
                    float x = 2 * (i.uv.x - 0.5) / _RadiusX;
                    float y = 2 * (i.uv.y - 0.5) / _RadiusY;
                    float value = x * x + y * y;
                    color.a *= smoothstep(1.0, _Hardness, value);
                }
			    return color;
            }
            ENDCG
        }
    }
}