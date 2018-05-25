using Veldrid;

namespace TextRender.Shaders
{
    internal class TextShader : ShaderAbstract
    {
        public DeviceBuffer ProjectionBuffer;
        public ResourceLayout ProjViewLayout;
        public ResourceLayout TextureLayout;

        public ResourceSet ProjViewSet;

        public TextShader(ResourceFactory factory) : base(factory, "Text")
        {
            Layout = new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4));

            ProjectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            ProjViewLayout = AddDisposable(factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex))
                    ));

            TextureLayout = AddDisposable(factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment))));

            ProjViewSet = AddDisposable(factory.CreateResourceSet(new ResourceSetDescription(
                ProjViewLayout,
                ProjectionBuffer)));
        }

        public void UpdateBuffers()
        {

        }
    }
}
