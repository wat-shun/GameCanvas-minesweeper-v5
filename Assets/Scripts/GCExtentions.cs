using System.ComponentModel;
using GameCanvas;
using Unity.Mathematics;

/// <summary>
/// gcの状態に依存しかつ複数のクラスから呼び出したい処理を拡張メソッドとして実装します。
/// </summary>
public static class GcProxyExtensions
{
    /// <summary>
    /// <see cref="DrawImageWithAnchor"/>を使うときの描画する画像の左上の座標を計算します。
    /// </summary>
    /// <param name="gc"></param>
    /// <param name="image">GcImage</param>
    /// <param name="anchor">GcAnchor</param>
    /// <param name="padding">指定したアンカーから右下方向にどれだけずらすか</param>
    /// <returns>左上座標</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static float2 CalcImageUpperLeftPosition(this IGameCanvas gc,
        in GcImage image, in GcAnchor anchor, in float2 padding)
    {
        var pos = padding;

        // x座標
        switch (anchor)
        {
            case GcAnchor.UpperLeft:
            case GcAnchor.MiddleLeft:
            case GcAnchor.LowerLeft:
                break;

            case GcAnchor.UpperCenter:
            case GcAnchor.MiddleCenter:
            case GcAnchor.LowerCenter:
                pos.x += gc.CanvasCenter.x - gc.GetImageWidth(image) / 2.0f;
                break;

            case GcAnchor.UpperRight:
            case GcAnchor.MiddleRight:
            case GcAnchor.LowerRight:
                pos.x += gc.CanvasWidth - gc.GetImageWidth(image);
                break;

            default:
                throw new InvalidEnumArgumentException(nameof(anchor), (int)anchor, typeof(GcAnchor));
        }

        // y座標
        switch (anchor)
        {
            case GcAnchor.UpperLeft:
            case GcAnchor.UpperCenter:
            case GcAnchor.UpperRight:
                break;

            case GcAnchor.MiddleLeft:
            case GcAnchor.MiddleCenter:
            case GcAnchor.MiddleRight:
                pos.y += gc.CanvasCenter.y - gc.GetImageWidth(image) / 2.0f;
                break;

            case GcAnchor.LowerLeft:
            case GcAnchor.LowerCenter:
            case GcAnchor.LowerRight:
                pos.y += gc.CanvasHeight - gc.GetImageHeight(image);
                break;

            default:
                throw new InvalidEnumArgumentException(nameof(anchor), (int)anchor, typeof(GcAnchor));
        }

        return pos;
    }

    /// <summary>
    /// <see cref="GcAnchor"/>を流用して、キャンバス内のアンカーに相当する場所に、アンカーが指定された画像を描画します。
    /// </summary>
    /// <param name="gc"></param>
    /// <param name="image">GcImage</param>
    /// <param name="anchor">GcAnchor</param>
    /// <param name="offset">指定したアンカーから右下方向にどれだけずらすか</param>
    public static void DrawImageWithAnchor(this IGameCanvas gc,
        in GcImage image, GcAnchor anchor = GcAnchor.UpperLeft, float2 offset = default)
    {
        // 基本になる位置を計算
        switch (anchor)
        {
            case GcAnchor.UpperLeft:
                break;

            case GcAnchor.UpperCenter:
                offset += new float2(gc.CanvasCenter.x, 0);
                break;

            case GcAnchor.UpperRight:
                offset += new float2(gc.CanvasWidth, 0);
                break;

            case GcAnchor.MiddleLeft:
                offset += new float2(0, gc.CanvasCenter.y);
                break;

            case GcAnchor.MiddleCenter:
                offset += gc.CanvasCenter;
                break;

            case GcAnchor.MiddleRight:
                offset += new float2(gc.CanvasWidth, gc.CanvasCenter.y);
                break;

            case GcAnchor.LowerLeft:
                offset += new float2(0, gc.CanvasHeight);
                break;

            case GcAnchor.LowerCenter:
                offset += new float2(gc.CanvasCenter.x, gc.CanvasHeight);
                break;

            case GcAnchor.LowerRight:
                offset += new float2(gc.CanvasWidth, gc.CanvasHeight);
                break;

            default:
                throw new InvalidEnumArgumentException(nameof(anchor), (int)anchor, typeof(GcAnchor));
        }

        using (gc.StyleScope)
        {
            gc.SetRectAnchor(anchor);
            gc.DrawImage(image, offset);
        }
    }
}
