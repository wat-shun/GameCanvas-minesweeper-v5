using System;
using GameCanvas;
using Unity.Mathematics;

/// <summary>
/// gcの状態に依存しかつ複数のクラスから呼び出したい処理を拡張メソッドとして実装します。
/// </summary>
public static class GcProxyExtensions
{
    /// <summary>
    /// 矩形アンカーを既定値に戻します
    /// </summary>
    /// <param name="gc"></param>
    public static void ResetRectAnchor(this GcProxy gc)
    {
        gc.SetRectAnchor(GcAnchor.UpperLeft);
    }

    /// <summary>
    /// 文字列アンカーを既定値に戻します。
    /// </summary>
    /// <param name="gc"></param>
    public static void ResetStringAnchor(this GcProxy gc)
    {
        gc.SetStringAnchor(GcAnchor.UpperLeft);
    }


    /// <summary>
    /// Game.DrawImageByAnchorを使うときの描画する画像の左上の座標を計算します。
    /// </summary>
    /// <param name="gc"></param>
    /// <param name="image">GcImage</param>
    /// <param name="anchor">GcAnchor</param>
    /// <param name="padding">指定したアンカーから右下方向にどれだけずらすか</param>
    /// <returns>左上座標</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static float2 CalcImageUpperLeftPosition(this GcProxy gc,
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
                throw new ArgumentOutOfRangeException();
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
                throw new ArgumentOutOfRangeException();
        }

        return pos;
    }

    /// <summary>
    /// GcAnchorを流用して、キャンバス内のアンカーに相当する場所に、アンカーが指定された画像を描画します。
    /// </summary>
    /// <param name="gc"></param>
    /// <param name="image">GcImage</param>
    /// <param name="anchor">GcAnchor</param>
    /// <param name="padding">指定したアンカーから右下方向にどれだけずらすか</param>
    public static void DrawImageWithAnchor(this GcProxy gc,
        in GcImage image, in GcAnchor anchor = GcAnchor.UpperLeft, in float2 padding = new float2())
    {
        // 基本になる位置を計算
        // todo 変数名がmarginだと間違ってる気もする
        float2 margin;
        switch (anchor)
        {
            case GcAnchor.UpperLeft:
                margin = new float2(0, 0);
                break;
            case GcAnchor.UpperCenter:
                margin = new float2(gc.CanvasCenter.x, 0);
                break;
            case GcAnchor.UpperRight:
                margin = new float2(gc.CanvasWidth, 0);
                break;
            case GcAnchor.MiddleLeft:
                margin = new float2(0, gc.CanvasCenter.y);
                break;
            case GcAnchor.MiddleCenter:
                margin = gc.CanvasCenter;
                break;
            case GcAnchor.MiddleRight:
                margin = new float2(gc.CanvasWidth, gc.CanvasCenter.y);
                break;
            case GcAnchor.LowerLeft:
                margin = new float2(0, gc.CanvasHeight);
                break;
            case GcAnchor.LowerCenter:
                margin = new float2(gc.CanvasCenter.x, gc.CanvasHeight);
                break;
            case GcAnchor.LowerRight:
                margin = new float2(gc.CanvasWidth, gc.CanvasHeight);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null);
        }

        gc.SetRectAnchor(anchor);
        gc.DrawImage(image, padding + margin);
        gc.ResetRectAnchor();
    }
}
