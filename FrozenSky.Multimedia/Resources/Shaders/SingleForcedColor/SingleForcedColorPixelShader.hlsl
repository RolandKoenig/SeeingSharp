//-----------------------------------------------------------------------------
//Include all common items
#include "../_mainInclude.hlsl"

//-----------------------------------------------------------------------------
//PixelShader implementation
float4 main(PSInputStandard input) : SV_Target
{
	float4 result = ObjectColor;
	result.a = Opacity;

	return result;
}