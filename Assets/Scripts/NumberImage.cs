using GameCanvas;
using Unity.Mathematics;

/// <summary>
/// 数字画像を扱うためのヘルパーです。
/// </summary>
public static class NumberImage
{
    public static readonly GcImage[] Image =
    {
        GcImage.Number_0,
        GcImage.Number_1,
        GcImage.Number_2,
        GcImage.Number_3,
        GcImage.Number_4,
        GcImage.Number_5,
        GcImage.Number_6,
        GcImage.Number_7,
        GcImage.Number_8,
        GcImage.Number_9,
    };

    public static readonly int2 Size = new int2(36, 36);
}
