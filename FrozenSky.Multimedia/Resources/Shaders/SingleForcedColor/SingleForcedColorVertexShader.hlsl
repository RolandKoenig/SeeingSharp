//-----------------------------------------------------------------------------
//Include all common items
#include "../_mainInclude.hlsl"

//-----------------------------------------------------------------------------
//VertexShader implementation
PSInputStandard main(VSInputStandard input) 
{
    PSInputStandard output = (PSInputStandard)0;
    
    // Calculate position of the vertex
	float4x4 localWorldViewProj = mul(World, ViewProj);
    output.pos = mul(float4(input.pos.xyz, 1.0), localWorldViewProj);

    return output;
}
