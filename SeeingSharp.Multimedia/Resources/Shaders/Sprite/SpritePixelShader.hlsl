//-----------------------------------------------------------------------------
//Include all common items
#include "../_mainInclude.hlsl"

//-----------------------------------------------------------------------------
//PixelShader implementation
float4 main(PSInputStandard input ) : SV_Target
{
	float4 textureColor = ObjectTexture.Sample(ObjectTextureSampler, input.tex.xy);

	// Merge given color with vertex color
    float4 outputColor = textureColor; //textureColor * textureColor.a + input.col * (1- textureColor.a);

    // Apply Accentuation effect
    outputColor = ApplyAccentuation(outputColor);

    return outputColor;
}
