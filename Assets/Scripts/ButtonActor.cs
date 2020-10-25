using GameCanvas;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 画像を描画し、その画像に対するホールド操作・リリース操作を検出するクラスです
/// </summary>
public sealed class ButtonActor : GcActor
{
    private const float HoldThreshold = 0.5f; // ホールド判定閾値

    private readonly GcImage _image;
    private readonly GcAnchor _anchor;
    private readonly float2 _offset;
    private readonly GcAABB _AABB;

    public ButtonActor(in GcImage image, in GcAnchor anchor = GcAnchor.UpperLeft, in float2 offset = default)
    {
        _image = image;
        _anchor = anchor;
        _offset = offset;

        var leftUpperPosition = gc.CalcImageUpperLeftPosition(_image, _anchor, _offset);
        var imageSize = gc.GetImageSize(_image);

        _AABB = GcAABB.XYWH(leftUpperPosition, imageSize);
    }

    /// <summary>
    /// このボタンの上で、指定秒数ホールドされて続けているかどうか
    /// </summary>
    public bool IsHolding()
    {
        return gc.IsTouched(out GcPointerTrace t)
            && t.Duration >= HoldThreshold
            && gc.HitTest(_AABB, t.Begin.Point)
            && gc.HitTest(_AABB, t.Current.Point);
    }

    /// <summary>
    /// タップ判定
    /// </summary>
    public bool IsTapped()
    {
        return gc.IsTapped(_AABB, out _);
    }

    /// <summary>
    /// このボタンがリリースされたか
    /// </summary>
    public bool IsReleased()
    {
        return gc.IsTouchEnded(_AABB, out _);
    }

    public override void Update()
    {
    }

    public override void Draw()
    {
        gc.DrawImageWithAnchor(_image, _anchor, _offset);

        // デバッグ用の当たり判定描画
        //if (gc.IsTouched(_AABB, out float2 pos))
        //{
        //    using (gc.StyleScope)
        //    {
        //        gc.SetColor(255, 0, 0, 64);
        //        gc.FillRect(_AABB);

        //        gc.SetColor(32, 0, 0);
        //        gc.FillCircle(pos, 4);
        //    }
        //}
    }
}
