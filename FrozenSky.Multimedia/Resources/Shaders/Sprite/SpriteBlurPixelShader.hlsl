//-----------------------------------------------------------------------------
//Include all common items
#include "../_mainInclude.hlsl"

//-----------------------------------------------------------------------------
//PixelShader implementation
float4 main(PSInputStandard input ) : SV_Target
{
    // Get original texture color
    float4 textureColor = ObjectTexture.Sample(ObjectTextureSampler, input.tex.xy);

    // Get parameters of this effect
	float opacity = Opacity;
    float blurIntensity = BorderMultiplyer;
    float grayscaleIntensity = BorderPart;

    // Calculate blured color from texture
    float4 outputColor = 0;
	outputColor += ObjectTexture.Sample(ObjectTextureSampler, float2(clamp(input.tex.x + 0.004 * Opacity, 0, 1), clamp(input.tex.y - 0.004 * Opacity, 0, 1))) * 0.1;
	outputColor += ObjectTexture.Sample(ObjectTextureSampler, float2(clamp(input.tex.x + 0.003 * Opacity, 0, 1), clamp(input.tex.y - 0.003 * Opacity, 0, 1))) * 0.2;
	outputColor += ObjectTexture.Sample(ObjectTextureSampler, float2(clamp(input.tex.x + 0.002 * Opacity, 0, 1), clamp(input.tex.y - 0.002 * Opacity, 0, 1))) * 0.4;
	outputColor += ObjectTexture.Sample(ObjectTextureSampler, float2(clamp(input.tex.x - 0.003 * Opacity, 0, 1), clamp(input.tex.y + 0.003 * Opacity, 0, 1))) * 0.2;
	outputColor += ObjectTexture.Sample(ObjectTextureSampler, float2(clamp(input.tex.x - 0.004 * Opacity, 0, 1), clamp(input.tex.y + 0.004 * Opacity, 0, 1))) * 0.1;

    // Merge colors based on given blue intensity
    outputColor = outputColor * blurIntensity + (textureColor * (1 - blurIntensity));

    // Perform grayscale and highlight effect
    float averageColor = (outputColor.x + outputColor.y + outputColor.z + 2.9) / 6.0;
    outputColor.x = averageColor * grayscaleIntensity + (outputColor.x * (1 - grayscaleIntensity));
    outputColor.y = averageColor * grayscaleIntensity + (outputColor.y * (1 - grayscaleIntensity));
    outputColor.z = averageColor * grayscaleIntensity + (outputColor.z * (1 - grayscaleIntensity));

	// Apply opacity
	outputColor.a = min(outputColor.a, opacity);

    // Return found color
    return outputColor;
}