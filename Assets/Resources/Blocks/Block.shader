﻿Shader "Blocks/Block" 
{
    Properties 
    {
        _Color ("Base Color", Color) = (0,0,0,0)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlockIndex ("Block Index", float) = 0
    }
    
    SubShader 
    {
        Tags { "RenderType" = "Opaque" }
        LOD 150
    
        CGPROGRAM
        #include "Blocks.cginc"
        #pragma surface surf Lambert noforwardadd
        
        sampler2D _MainTex;
        fixed4 _Color;
        float _BlockIndex;
        
        struct Input 
        {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutput o) 
        {
            float2 uv = blockUV(_BlockIndex, IN.uv_MainTex);
            fixed4 c = tex2D(_MainTex, uv);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    
    Fallback "Mobile/VertexLit"
}
