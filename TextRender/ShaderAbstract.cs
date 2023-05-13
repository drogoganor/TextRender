using System;
using System.IO;
using Veldrid;
using Veldrid.SPIRV;

namespace TextRender
{
    public class ShaderAbstract : IDisposable

    {
    public string Name { get; protected set; }
    public Shader VertexShader { get; protected set; }
    public Shader FragmentShader { get; protected set; }
    public VertexLayoutDescription Layout { get; protected set; }

    protected ShaderAbstract(ResourceFactory factory, string name, bool InvertY = false)
    {
        Name = name;

        var vertSpirvBytes = ReadEmbeddedAssetBytes($"TextRender.Shaders.{Name}.vert.spv");
        var fragSpirvBytes = ReadEmbeddedAssetBytes($"TextRender.Shaders.{Name}.frag.spv");

        SpecializationConstant[] constants = null;

        if (factory.BackendType != GraphicsBackend.Vulkan || InvertY)
        {
            constants = new SpecializationConstant[] {new SpecializationConstant(0, false)};
        }

        var shaders = factory.CreateFromSpirv(
            new ShaderDescription(ShaderStages.Vertex, vertSpirvBytes, "main"),
            new ShaderDescription(ShaderStages.Fragment, fragSpirvBytes, "main"),
            new CrossCompileOptions()
            {
                Specializations = constants
            }
        );

        VertexShader = shaders[0];
        FragmentShader = shaders[1];
    }

    public virtual void Dispose()
    {
        if (VertexShader != null && FragmentShader != null)
        {
            VertexShader.Dispose();
            VertexShader = null;
            FragmentShader.Dispose();
            FragmentShader = null;
        }
    }

    protected Stream OpenEmbeddedAssetStream(string name) => GetType().Assembly.GetManifestResourceStream(name);

    protected byte[] ReadEmbeddedAssetBytes(string name)
    {
        using Stream stream = OpenEmbeddedAssetStream(name);
        byte[] bytes = new byte[stream.Length];
        using MemoryStream ms = new MemoryStream(bytes);
        stream.CopyTo(ms);
        return bytes;
    }
    }
}
