# TextRender for Veldrid

A simple text rendering library for the [Veldrid](https://github.com/mellinoe/veldrid) library and .NET Core.

![Preview](https://github.com/drogoganor/TextRender.Veldrid/blob/master/images/Example.png)

TextRender is available on NuGet:

[![NuGet](https://img.shields.io/nuget/v/TextRender.Veldrid.svg)](https://www.nuget.org/packages/TextRender.Veldrid)

## About

Generates text on the CPU using [SixLabors](https://github.com/SixLabors) libraries. Renders text using an orthographic projection (2D).

Veldrid font texture generation thanks to [OpenSAGE](https://github.com/OpenSAGE/OpenSAGE).

Shaders are included in the TextRender.Veldrid library as embedded resources. They can be compiled separately using the TextRender.Veldrid.Shaders project.

## Basic usage

#### Setting up:

```
var textRenderer = new TextRender.TextRenderer(_graphicsDevice);
var text = new Text(textRenderer, "Hello world!")
{
	Position = new Vector2(200, 100),
	FontSize = 24,
	Color = RgbaFloat.White
};
text.Initialize();
```

#### Drawing:

```
text.Draw();
```

#### Cleaning up:

```
text.Dispose();
textRenderer.Dispose();
```

## Batched usage

#### Setting up:

```
var textRenderer = new TextRender.TextRenderer(_graphicsDevice);
var text = new Text(textRenderer, "Hello world!")
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
```

#### Drawing:

```
textRenderer.BeginDraw();
text.DrawBatched();
text2.DrawBatched();
textRenderer.EndDraw();
```

#### Cleaning up:

```
text.Dispose();
text2.Dispose();
textRenderer.Dispose();
```

## Thanks to

  * [Veldrid](https://github.com/mellinoe/veldrid)
  * [SixLabors](https://github.com/SixLabors)
  * [OpenSAGE](https://github.com/OpenSAGE/OpenSAGE)