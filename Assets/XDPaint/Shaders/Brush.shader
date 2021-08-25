Shader "XD Paint/Brush"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _SrcColorBlend ("__srcC", Int) = 5
        _DstColorBlend ("__dstC", Int) = 10
        _SrcAlphaBlend ("__srcA", Int) = 5
        _DstAlphaBlend ("__dstA", Int) = 1
        _BlendOpColor ("__blendC", Int) = 0
        _BlendOpAlpha ("__blendA", Int) = 0
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Cull Off Lighting Off ZWrite Off ZTest Off Fog { Color (0,0,0,0) }
        Pass
        {
//          ERASE:
//          Blend Zero OneMinusSrcAlpha, Zero OneMinusSrcAlpha
//          PAINT:
//          Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha One
            BlendOp [_BlendOpColor], [_BlendOpAlpha]
            Blend [_SrcColorBlend] [_DstColorBlend], [_SrcAlphaBlend] [_DstAlphaBlend]
            SetTexture [_MainTex] { combine texture }
           }
    }
}