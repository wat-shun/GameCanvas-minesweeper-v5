using UnityEngine;
using Unity.Mathematics;

public class MinesweeperBoard
{
    private readonly int2 _boardSize; // 盤面の大きさ
    private readonly int _bombCount; // 爆弾の数

    private readonly bool[,] _isOpen; // 空いているか
    private readonly bool[,] _isBomb; // 爆弾があるか
    private readonly bool[,] _isFlag; // 旗があるか

    private readonly int[,] _cntAroundBomb; // 周囲の爆弾の数
    private int _cntClosedCell; // 閉じてるセルの数

    private readonly int[] _dx = {1, 1, 0, -1, -1, -1, 0, 1};
    private readonly int[] _dy = {0, 1, 1, 1, 0, -1, -1, -1};

    private readonly System.Random _r = new System.Random();

    public MinesweeperBoard(int2 size, int bombCnt)
    {
        _boardSize = size;
        _bombCount = bombCnt;

        // 一箇所でも開けられるか 本当はassert以外がいい？
        Debug.Assert(_boardSize.x * _boardSize.y > _bombCount);

        // 変数の初期化
        _isOpen = new bool[_boardSize.y, _boardSize.x];
        _isBomb = new bool[_boardSize.y, _boardSize.x];
        _isFlag = new bool[_boardSize.y, _boardSize.x];
        _cntAroundBomb = new int[_boardSize.y, _boardSize.x];
        _cntClosedCell = _boardSize.x * _boardSize.y;
    }

    // 指定した位置がボード内か
    public bool InBoard(int x, int y)
    {
        return 0 <= x && x < _boardSize.x && 0 <= y && y < _boardSize.y;
    }

    // 指定した位置が爆弾か
    public bool IsBomb(int x, int y)
    {
        Debug.Assert(InBoard(x, y));
        return _isBomb[y, x];
    }

    // 指定した位置は旗か
    public bool IsFlag(int x, int y)
    {
        Debug.Assert(InBoard(x, y));
        return _isFlag[y, x];
    }


    // 指定した位置が空いているか
    public bool IsOpened(int x, int y)
    {
        Debug.Assert(InBoard(x, y));
        return _isOpen[y, x];
    }

    // ゲームオーバーか 重い処理なので最適化したい
    public bool IsGameover()
    {
        for (var i = 0; i < _boardSize.x; i++)
        {
            for (var j = 0; j < _boardSize.y; j++)
            {
                // 爆弾がオープンならゲームオーバー
                if (IsBomb(i, j) && IsOpened(i, j)) return true;
            }
        }

        return false;
    }

    // クリアか
    public bool IsClear()
    {
        return _cntClosedCell == _bombCount;
    }

    // 指定した位置の八方周の爆弾数
    public int CntAroundBomb(int x, int y)
    {
        Debug.Assert(InBoard(x, y));
        return _cntAroundBomb[y, x];
    }

    // 盤面初期化 開幕爆破しないようにする
    private void Start(int x, int y)
    {
        // 設置可能な位置にランダムに置く。爆弾数が増えると計算量が悪そう
        var k = _bombCount;
        while (k > 0)
        {
            var px = _r.Next(0, _boardSize.x - 1);
            var py = _r.Next(0, _boardSize.y - 1);

            // 開けるセルとすでにあるセルには置かない
            if ((px == x && py == y) || IsBomb(px, py)) continue;

            // 爆弾を置く
            _isBomb[py, px] = true;
            --k;
        }

        // セルごとに周囲の爆弾の数を数える
        for (var ny = 0; ny < _boardSize.y; ny++)
        {
            for (var nx = 0; nx < _boardSize.x; nx++)
            {
                // 爆弾がある箇所ならINFにして終了
                if (IsBomb(nx, ny))
                {
                    _cntAroundBomb[ny, nx] = 100000;
                    continue;
                }

                for (var a = 0; a < 8; a++)
                {
                    var cx = nx + _dx[a];
                    var cy = ny + _dy[a];
                    if (!InBoard(cx, cy)) continue; // 範囲外なら処理しない
                    _cntAroundBomb[ny, nx] += IsBomb(cx, cy) ? 1 : 0;
                }
            }
        }
    }

    // 指定したセルを開け、そのセルが0なら再起処理で周囲を開けていく
    public void Open(int x, int y)
    {
        if (!InBoard(x, y)) return; // 範囲外なら処理しない
        if (IsOpened(x, y)) return; // 開放済みなら処理しない

        // 未開なら初期化処理
        if (_cntClosedCell == _boardSize.x * _boardSize.y) Start(x, y);

        // 指定位置を開ける
        _isOpen[y, x] = true;
        --_cntClosedCell;

        // 自分が0なら周囲を連鎖的に開ける
        if (CntAroundBomb(x, y) == 0)
        {
            for (var a = 0; a < 8; a++)
            {
                var cx = x + _dx[a];
                var cy = y + _dy[a];

                if (!InBoard(cx, cy)) continue; // 範囲外なら処理しない

                Open(cx, cy);
            }
        }
    }

    // 指定したセルの旗の状態を反転する
    public void TurnFlag(int x, int y)
    {
        if (!InBoard(x, y)) return; // 範囲外なら処理しない
        _isFlag[y, x] = !_isFlag[y, x];
    }
}
