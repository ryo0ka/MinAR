Shader "Blocks/BlockDamage" 
{
    Properties 
    {
        _Color ("Base Color", Color) = (0,0,0,0)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DamageIndex ("Damage Index", float) = 0
        _Offset("Offset", float) = 0
    }
    
    SubShader 
    {
        Tags
        {
            "Queue" = "Transparent" 
            "RenderType"="Transparent" 
        }
 
        LOD 150
            
        CGPROGRAM
        #include "Blocks.cginc"
        #pragma surface surf Lambert noforwardadd vertex:vert alpha:fade
        
        sampler2D _MainTex;
        fixed4 _Color;
        float _DamageIndex;
        float _Offset;
        
        struct Input 
        {
            float2 uv_MainTex;
        };
        
        void vert (inout appdata_full v) 
        {
            v.vertex.xyz += v.normal * _Offset;
        }
         
        void surf (Input IN, inout SurfaceOutput o) 
        {
            float2 uv = damageUV(_DamageIndex, IN.uv_MainTex);
            fixed4 c = tex2D(_MainTex, uv);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    
    Fallback "Mobile/VertexLit"
}
