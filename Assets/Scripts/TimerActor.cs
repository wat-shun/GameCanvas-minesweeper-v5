using GameCanvas;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// 秒のみの時計を描画をするアクターです。
/// 左に伸びる形で描画するため、右上の座標を指定します。
/// </summary>
public sealed class TimerActor : GcActor
{
    public int Second { get; set; } // 秒は外部から更新したいときもあるので公開する
    private readonly int2 _rightUpperPos;

    public TimerActor(int second, int2 rightUpperPos)
    {
        Second = second;
        _rightUpperPos = rightUpperPos;
    }

    public override void Draw()
    {
        Assert.IsTrue(Second >= 0); // 負の秒数には対応しない

        // 桁数を計算する
        var k = (Second == 0) ? 1 : ((int) Mathf.Log10(Second) + 1);

        // 「秒」の描画
        gc.DrawImage(
            GcImage.Second,
            _rightUpperPos.x - NumberImage.Size.x,
            _rightUpperPos.y
        );

        // 1桁目から順番に描画
        var tmp = Second;
        for (var i = 0; i < k; i++)
        {
            var number = tmp % 10;
            gc.DrawImage(
                NumberImage.Image[number],
                _rightUpperPos.x - NumberImage.Size.x * (i + 2),
                _rightUpperPos.y
            );
            tmp /= 10;
        }
    }
}