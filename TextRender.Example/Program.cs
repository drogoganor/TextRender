using SixLabors.Fonts;
using System.IO;
using System.Numerics;
using System.Threading;
using TextRender;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace GettingStarted
{
    static class Program
    {
        private static GraphicsDevice _graphicsDevice;
        private static CommandList _commandList;
        private static Pipeline _pipeline;
        
        private static TextRender.TextRenderer textRenderer;
        private static Text text;
        private static Text text2;
        private static TextShader textShader;

        static void Main(string[] args)
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Veldrid Tutorial"
            };
            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, new GraphicsDeviceOptions()
            {
                SwapchainDepthFormat = PixelFormat.R16_UNorm
            });

            textRenderer = new TextRender.TextRenderer(_graphicsDevice);
            text = new Text(textRenderer, "Hello world!")
            {
                Position = new Vector2(200, 100),
                FontSize = 24,
                Color = RgbaFloat.White
            };
            text.Initialize();

            text2 = new Text(textRenderer, "Test")
            {
                Position = new Vector2(400, 150),
                FontSize = 36,
                FontStyle = FontStyle.Bold,
                FontName = "Courier New",
                Color = RgbaFloat.Yellow
            };
            text2.Initialize();

            CreateResources();

            while (window.Exists)
            {
                window.PumpEvents();

                if (window.Exists)
                {
                    Draw();
                    Thread.Sleep(1);
                }
            }

            DisposeResources();
        }

        private static void CreateResources()
        {
            ResourceFactory factory = _graphicsDevice.ResourceFactory;

            textShader = new TextShader(factory);

            // Create pipeline
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { textShader.Layout },
                shaders: new Shader[] { textShader.VertexShader, textShader.FragmentShader });
            pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            _commandList = factory.CreateCommandList();
        }

        private static void Draw()
        {
            // Begin() must be called before commands can be issued.
            _commandList.Begin();

            // We want to render directly to the output window.
            _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
            _commandList.SetFullViewports();
            _commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);

            _commandList.SetPipeline(_pipeline);

            // End() must be called before commands can be submitted for execution.
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);
 
            textRenderer.BeginDraw();
            text.DrawBatched();
            text2.DrawBatched();
            textRenderer.EndDraw();
            
            // Once commands have been submitted, the rendered image can be presented to the application window.
            _graphicsDevice.SwapBuffers();
        }

        private static void DisposeResources()
        {
            text.Dispose();
            text2.Dispose();
            textRenderer.Dispose();

            _pipeline.Dispose();
            textShader.Dispose();
            _commandList.Dispose();
            _graphicsDevice.Dispose();
        }
    }

    struct VertexPositionColor
    {
        public const uint SizeInBytes = 24;
        public Vector2 Position;
        public RgbaFloat Color;
        public VertexPositionColor(Vector2 position, RgbaFloat color)
        {
            Position = position;
            Color = color;
        }
    }
}
