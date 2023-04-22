
/*
Copyright © 2022 Redhacker1

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Numerics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Veldrid;

namespace TextRender;

public class TextCache : DisposableManager
{
    GraphicsDevice _device;
    ResourcePool<Texture, Vector2> Textures;
    // TODO: Maybe cache actual TextImages as well, in case they are used in multiple places.



    public TextCache(GraphicsDevice device)
    {
        Textures = new ResourcePool<Texture, Vector2>(ToKeyValue, FromKeyValue);
        
        _device = device;
    }

    Texture FromKeyValue(Vector2 arg)
    {
        TextureDescription textureDescription = TextureDescription.Texture2D((uint)arg.X, (uint)arg.Y, mipLevels: 1, 1,
            PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled);
        return _device.ResourceFactory.CreateTexture(textureDescription);
    }

    Vector2 ToKeyValue(Texture arg)
    {
        return new Vector2(arg.Width, arg.Height);
    }


    public Texture GetTextTexture(string content, Font font, TextAlignment HorizontalAlignmentState, RgbaFloat color, Vector2 size)
    {
        PointF offset = new PointF(0 , size.Y /2);
        if ( HorizontalAlignmentState == TextAlignment.Center )
        {
	        offset.X = size.X /2;
        }
        
        Image<Rgba32> image = new Image<Rgba32>((int)size.X, (int)size.Y);
        
        TextOptions textOptions = new TextOptions(font)
        {
	        VerticalAlignment = VerticalAlignment.Center,
	        HorizontalAlignment = HorizontalAlignmentState == TextAlignment.Center ? HorizontalAlignment.Center : HorizontalAlignment.Left,
	        WrappingLength = size.X,
	        Origin = offset,
        };
        // Draws the text with horizontal red and blue hatching with a dash dot pattern outline.
        image.Mutate(x=> x.DrawText(textOptions, content, new Color(color.ToVector4())));
        Texture texture = Textures.Acquire(new Vector2(size.X, size.Y));
        image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> data);
        _device.UpdateTexture(texture,data.Span, 0, 0,0,(uint)size.X, (uint)size.Y, 1, 0, 0);
        image.Dispose();
        AddDisposable(texture);
        

        return texture;
    }

    public void Dispose()
    {
        Textures.Dispose();
    }
}
