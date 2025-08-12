using System;

namespace VoxelMeshOptimizer.Core;

/// <summary>
/// Represents a texture atlas divided into a grid where each cell corresponds
/// to an individual texture. The atlas is defined by the number of textures
/// along the horizontal and vertical axes as well as the total pixel size.
/// </summary>
public class TextureAtlas
{
    /// <summary>
    /// Number of textures laid out horizontally in the atlas.
    /// </summary>
    public uint TexturesX { get; }

    /// <summary>
    /// Number of textures laid out vertically in the atlas.
    /// </summary>
    public uint TexturesY { get; }

    /// <summary>
    /// Total width of the atlas in pixels.
    /// </summary>
    public uint PixelsX { get; }

    /// <summary>
    /// Total height of the atlas in pixels.
    /// </summary>
    public uint PixelsY { get; }

    /// <summary>
    /// Width in pixels of a single texture inside the atlas.
    /// </summary>
    public uint TextureWidth => PixelsX / TexturesX;

    /// <summary>
    /// Height in pixels of a single texture inside the atlas.
    /// </summary>
    public uint TextureHeight => PixelsY / TexturesY;

    /// <summary>
    /// Initializes a new instance of <see cref="TextureAtlas"/>.
    /// </summary>
    /// <param name="texturesX">Number of textures horizontally.</param>
    /// <param name="texturesY">Number of textures vertically.</param>
    /// <param name="pixelsX">Total horizontal pixel count.</param>
    /// <param name="pixelsY">Total vertical pixel count.</param>
    /// <exception cref="ArgumentException">Thrown when textures count is zero
    /// or when the pixel dimensions are not divisible by the number of textures.</exception>
    public TextureAtlas(uint texturesX, uint texturesY, uint pixelsX, uint pixelsY)
    {
        if (texturesX == 0 || texturesY == 0)
            throw new ArgumentException("Texture counts must be greater than zero.");

        if (pixelsX % texturesX != 0 || pixelsY % texturesY != 0)
            throw new ArgumentException("Pixel dimensions must be divisible by the number of textures.");

        TexturesX = texturesX;
        TexturesY = texturesY;
        PixelsX = pixelsX;
        PixelsY = pixelsY;
    }

    /// <summary>
    /// Returns the pixel rectangle for the texture at the given index. The
    /// index is zero-based and follows row-major order.
    /// </summary>
    /// <param name="index">Index of the texture.</param>
    /// <returns>A tuple containing the x and y coordinates of the texture in
    /// pixels along with its width and height.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is
    /// outside the bounds of the atlas.</exception>
    public (uint x, uint y, uint width, uint height) GetTextureRect(uint index)
    {
        if (index >= TexturesX * TexturesY)
            throw new ArgumentOutOfRangeException(nameof(index));

        uint ix = index % TexturesX;
        uint iy = index / TexturesX;

        uint x = ix * TextureWidth;
        uint y = iy * TextureHeight;

        return (x, y, TextureWidth, TextureHeight);
    }
}