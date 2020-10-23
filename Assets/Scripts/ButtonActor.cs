using GameCanvas;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 画像を描画し、その画像に対するホールド操作・リリース操作を検出するクラスです
/// </summary>
public sealed class ButtonActor : GcActor
{
    private readonly GcImage _image;
    private readonly GcAnchor _anchor;
    private readonly float2 _padding;

    public GcAABB AABB { get; } // TODO 当たり判定は外部で使いたい時もあるので露出させてるけどやめたほうがいい気もする

    public ButtonActor(in GcImage image, in GcAnchor anchor = GcAnchor.UpperLeft, in float2 padding = new float2())
    {
        _image = image;
        _anchor = anchor;
        _padding = padding;

        var leftUpperPosition = gc.CalcImageUpperLeftPosition(_image, _anchor, _padding);
        var imageSize = gc.GetImageSize(_image);

        AABB = new GcAABB(
            new Rect(
                leftUpperPosition.x,
                leftUpperPosition.y,
                imageSize.x,
                imageSize.y
            )
        );
    }

    /// <summary>
    /// このボタンが指定秒数ホールドされているか
    /// </summary>
    /// <returns></returns>
    public bool IsHolding(in float threshold)
    {
        gc.TryGetPointerEvent(0, out var touchEvent);
        return touchEvent.Phase == GcPointerEventPhase.Hold
               && gc.HitTest(AABB, touchEvent.Point)
               && gc.GetPointerDuration(0) >= threshold;
    }

    /// <summary>
    /// このボタンがリリースされたか
    /// </summary>
    /// <returns></returns>
    public bool IsReleased()
    {
        gc.TryGetPointerEvent(0, out var touchEvent);
        return touchEvent.Phase == GcPointerEventPhase.End
               && gc.HitTest(AABB, touchEvent.Point);
    }

    public override void Update()
    {
    }

    public override void Draw()
    {
        // gc.DrawRect(AABB.Center - AABB.HalfSize, AABB.HalfSize * 2); // デバッグ用の当たり判定描画
        gc.DrawImageWithAnchor(_image, _anchor, _padding);
    }
}