void audiosample_float(UnityTexture2D _AudioTex, float2 _UV, float3 _Color, out float3 _OUT)
{
    float x = _UV.x;
    float y = _UV.y;
    float fft, fft2;

    fft = tex2D(_AudioTex, float2(x/2,0.5)).r/2;
    fft2 = tex2D(_AudioTex, float2(x/2+0.5,0.5)).r/2;

    float v =  step(y, fft) || step(1-fft2,y);

    _OUT =  _Color * v;
}