//-----------------------------------------------------------------------------
//Include all common items
#include "../_mainInclude.hlsl"

float getGray(float4 c)
{
	return(dot(c.rgb, ((0.33333).xxx)));
}

//-----------------------------------------------------------------------------
//PixelShader implementation
float4 main(PSInputStandard input ) : SV_Target
{
	// Edge detection using kernel filter method
	//  see http://digitalerr0r.wordpress.com/2009/03/22/xna-shader-programming-tutorial-7-toon-shading/
	//  Other tutorials at http://digitalerr0r.wordpress.com/tutorials/
	//  More info about kernel filter method: http://en.wikipedia.org/wiki/Kernel_(image_processing)

	// Get normal output color from source texture / configured opacity
	float4 outputColor = ObjectTexture.Sample(ObjectTextureSampler, input.tex);
	outputColor.a = min(Opacity, 1.0 - outputColor.x);
	clip(outputColor.a - 0.01);

	// Configure border (hardcoded)
	float Thickness = 0.5f;
	float Threshold = 0.2f; 

	// Perofrm edge detection using kernel filter method
	float2 ox = float2(Thickness / ScreenPixelSize.x, 0.0);
	float2 oy = float2(0.0, Thickness / ScreenPixelSize.y);
	float2 uv = input.tex.xy;
	float2 PP = uv - oy;
	float4 CC = ObjectTexture.Sample(ObjectTextureSampler, PP - ox); float g00 = getGray(CC);
	CC = ObjectTexture.Sample(ObjectTextureSampler, PP);    float g01 = getGray(CC);
	CC = ObjectTexture.Sample(ObjectTextureSampler, PP + ox); float g02 = getGray(CC);
	PP = uv;
	CC = ObjectTexture.Sample(ObjectTextureSampler, PP - ox); float g10 = getGray(CC);
	CC = ObjectTexture.Sample(ObjectTextureSampler, PP);    float g11 = getGray(CC);
	CC = ObjectTexture.Sample(ObjectTextureSampler, PP + ox); float g12 = getGray(CC);
	PP = uv + oy;
	CC = ObjectTexture.Sample(ObjectTextureSampler, PP - ox); float g20 = getGray(CC);
	CC = ObjectTexture.Sample(ObjectTextureSampler, PP);    float g21 = getGray(CC);
	CC = ObjectTexture.Sample(ObjectTextureSampler, PP + ox); float g22 = getGray(CC);
	float K00 = -1;
	float K01 = -2;
	float K02 = -1;
	float K10 = 0;
	float K11 = 0;
	float K12 = 0;
	float K20 = 1;
	float K21 = 2;
	float K22 = 1;
	float sx = 0;
	float sy = 0;
	sx += g00 * K00;
	sx += g01 * K01;
	sx += g02 * K02;
	sx += g10 * K10;
	sx += g11 * K11;
	sx += g12 * K12;
	sx += g20 * K20;
	sx += g21 * K21;
	sx += g22 * K22;
	sy += g00 * K00;
	sy += g01 * K10;
	sy += g02 * K20;
	sy += g10 * K01;
	sy += g11 * K11;
	sy += g12 * K21;
	sy += g20 * K02;
	sy += g21 * K12;
	sy += g22 * K22;
	float dist = sqrt(sx*sx + sy*sy);
	float result = 1;
	if (dist>Threshold) { result = 0; }

	// Generate output (clip unseen pixels)
	outputColor.xyz = result.xxx;
	outputColor.a = 1.0 - result.x;
	clip(outputColor.a - 0.1);

	return outputColor;
}