#include <metal_stdlib>
using namespace metal;
struct TextRender_Resources_Text_VertexInput
{
    float2 Position [[ attribute(0) ]];
    float2 TexCoords [[ attribute(1) ]];
    float4 Color [[ attribute(2) ]];
};

struct TextRender_Resources_Text_FragmentInput
{
    float4 SystemPosition [[ position ]];
    float2 TexCoords [[ attribute(0) ]];
    float4 Color [[ attribute(1) ]];
};

struct ShaderContainer {
constant float4x4& Projection;
constant float4& Position;

ShaderContainer(
constant float4x4& Projection_param, constant float4& Position_param
)
:
Projection(Projection_param), Position(Position_param)
{}
TextRender_Resources_Text_FragmentInput VS( TextRender_Resources_Text_VertexInput input)
{
    TextRender_Resources_Text_FragmentInput output;
    float4 worldPosition = Projection * float4(float4(input.Position, 0, 1) + Position);
    output.SystemPosition = worldPosition;
    output.Color = input.Color;
    output.TexCoords = input.TexCoords;
    return output;
}


};

vertex TextRender_Resources_Text_FragmentInput VS(TextRender_Resources_Text_VertexInput input [[ stage_in ]], constant float4x4 &Projection [[ buffer(0) ]], constant float4 &Position [[ buffer(1) ]])
{
return ShaderContainer(Projection, Position).VS(input);
}
