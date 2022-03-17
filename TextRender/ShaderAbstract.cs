using System.IO;
using Veldrid;
using Veldrid.SPIRV;

namespace TextRender
{
    public class ShaderAbstract : DisposableManager
    {
        public string Name { get; protected set; }
        public Shader VertexShader { get; protected set; }
        public Shader FragmentShader { get; protected set; }
        public VertexLayoutDescription Layout { get; protected set; }

        public ShaderAbstract(ResourceFactory factory, string name)
        {
            Name = name;

            var vertSpirvBytes = ReadEmbeddedAssetBytes($"TextRender.Shaders.{Name}.vert.spv");
            var fragSpirvBytes = ReadEmbeddedAssetBytes($"TextRender.Shaders.{Name}.frag.spv");

            var shaders = factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, vertSpirvBytes, "main"),
                new ShaderDescription(ShaderStages.Fragment, fragSpirvBytes, "main")
                );

            VertexShader = shaders[0];
            FragmentShader = shaders[1];
        }
        
        protected Stream OpenEmbeddedAssetStream(string name) => GetType().Assembly.GetManifestResourceStream(name);

        protected byte[] ReadEmbeddedAssetBytes(string name)
        {
            using (Stream stream = OpenEmbeddedAssetStream(name))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }
    }
}
