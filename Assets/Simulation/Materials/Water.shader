Shader "Custom/WaterSimple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveStrength ("Wave Strength", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        float _WaveStrength;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float wave = sin(_Time.y + IN.uv_MainTex.x * 10) * _WaveStrength;
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex + wave);
            o.Albedo = c.rgb;
            o.Alpha = 0.7;
        }
        ENDCG
    }
}
