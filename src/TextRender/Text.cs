using OpenSage;
using SixLabors.Fonts;
using System.Numerics;
using Veldrid;

namespace TextRender
{
    public class Text : DisposableBase
    {
        private TextRenderer _renderer;

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = new Vector2(200, 100);
        public string Content { get; set; } = string.Empty;
        public int FontSize { get; set; } = 12;
        public string FontName { get; set; } = "Arial";
        public FontStyle FontStyle { get; set; } = FontStyle.Regular;
        public TextAlignment TextAlignment { get; set; }
        public RgbaFloat Color { get; set; } = RgbaFloat.White;

        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private ResourceSet textureSet;
        private Texture texture;
        private TextureView textureView;

        public Text(TextRenderer renderer, string text)
        {
            Content = text;
            _renderer = renderer;
        }

        public void Initialize()
        {
            var device = _renderer.Device;
            var factory = device.ResourceFactory;

            BufferDescription vbDescription = new BufferDescription(
                4 * VertexPositionTextureColor.SizeInBytes,
                BufferUsage.VertexBuffer);
            vertexBuffer = AddDisposable(factory.CreateBuffer(vbDescription));

            BufferDescription ibDescription = new BufferDescription(
                4 * sizeof(ushort),
                BufferUsage.IndexBuffer);
            indexBuffer = AddDisposable(factory.CreateBuffer(ibDescription));

            var topLeft = new Vector2(Position.X, Position.Y);
            var topRight = new Vector2(Position.X + Size.X, Position.Y);
            var bottomLeft = new Vector2(Position.X, Position.Y + Size.Y);
            var bottomRight = new Vector2(Position.X + Size.X, Position.Y + Size.Y);

            VertexPositionTextureColor[] quadVertices =
            {
                new VertexPositionTextureColor(topLeft, new Vector2(0f, 0f), RgbaFloat.Yellow),
                new VertexPositionTextureColor(topRight, new Vector2(1f, 0f), RgbaFloat.Yellow),
                new VertexPositionTextureColor(bottomLeft, new Vector2(0f, 1f), RgbaFloat.Yellow),
                new VertexPositionTextureColor(bottomRight, new Vector2(1f, 1f), RgbaFloat.Yellow)
            };
            ushort[] quadIndices = { 0, 1, 2, 3 };

            device.UpdateBuffer(vertexBuffer, 0, quadVertices);
            device.UpdateBuffer(indexBuffer, 0, quadIndices);

            var fontCache = AddDisposable(new TextCache(device));
            var font = SystemFonts.CreateFont(FontName, FontSize, FontStyle);
            texture = AddDisposable(fontCache.GetTextTexture(Content, font, TextAlignment, Color, Size));
            textureView = AddDisposable(device.ResourceFactory.CreateTextureView(texture));

            textureSet = AddDisposable(device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                _renderer.Shader.TextureLayout,
                textureView,
                device.Aniso4xSampler)));
        }
        
        public void DrawBatched()
        {
            var cl = _renderer.CommandList;

            // Set all relevant state to draw our quad.
            cl.SetVertexBuffer(0, vertexBuffer);
            cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            cl.SetGraphicsResourceSet(0, _renderer.Shader.ProjViewSet);
            cl.SetGraphicsResourceSet(1, textureSet);

            // Issue a Draw command for a single instance with 4 indices.
            cl.DrawIndexed(
                indexCount: (uint)4,
                instanceCount: 1,
                indexStart: (uint)0,
                vertexOffset: 0,
                instanceStart: 0);
        }

        /// <summary>
        /// Render this text in a single draw call
        /// </summary>
        public void Draw()
        {
            _renderer.BeginDraw();
            DrawBatched();
            _renderer.EndDraw();
        }
    }
}
