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

public static class NumberExtentions
{
    /// <summary>
    /// 各桁の数字を求める
    /// </summary>
    public static byte[] CalcDigits(this in int number)
    {
        if (number < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(number));
        }

        // 桁数を計算する
        var k = (number == 0)
            ? 1
            : ((int)math.log10(number) + 1);

        var ret = new byte[k];

        // 各桁の数字を求める
        var tmp = number;
        for (var i = 0; i < k; i++)
        {
            ret[i] = (byte)(tmp % 10);
            tmp /= 10;
        }

        // 配列で返す
        return ret;
    }
}
