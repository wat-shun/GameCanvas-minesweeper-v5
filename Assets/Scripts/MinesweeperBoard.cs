using UnityEngine;

public class MinesweeperBoard
{
    public Vector2Int BoardSize; // 盤面の大きさ
    public int BombCount; // 爆弾の数

    private bool[,] _isOpen; // 空いているか
    private bool[,] _isBomb; // 爆弾があるか
    private bool[,] _isFlag; // 旗があるか

    private int[,] _cntAroundBomb; // 周囲の爆弾の数
    public int cntClosedCell; // 閉じてるセルの数

    private int[] dx = {1, 1, 0, -1, -1, -1, 0, 1};
    private int[] dy = {0, 1, 1, 1, 0, -1, -1, -1};

    System.Random r = new System.Random();

    public MinesweeperBoard(Vector2Int size, int bombCnt)
    {
        BoardSize = size;
        BombCount = bombCnt;

        // 一箇所でも開けられるか 本当はassert以外がいい？
        Debug.Assert(BoardSize.x * BoardSize.y > BombCount);

        // 変数を初期化する
        _isOpen = new bool[BoardSize.y, BoardSize.x];
        _isBomb = new bool[BoardSize.y, BoardSize.x];
        _isFlag = new bool[BoardSize.y, BoardSize.x];
        _cntAroundBomb = new int[BoardSize.y, BoardSize.x];
        cntClosedCell = BoardSize.x * BoardSize.y;
    }

    // 指定した位置がボード内か
    public bool inBoard(int x, int y)
    {
        return 0 <= x && x < BoardSize.x && 0 <= y && y < BoardSize.y;
    }

    // 指定した位置が爆弾か
    public bool isBomb(int x, int y)
    {
        Debug.Assert(inBoard(x, y));
        return _isBomb[y, x];
    }

    // 指定した位置は旗か
    public bool isFlag(int x, int y)
    {
        Debug.Assert(inBoard(x, y));
        return _isFlag[y, x];
    }


    // 指定した位置が空いているか
    public bool isOpen(int x, int y)
    {
        Debug.Assert(inBoard(x, y));
        return _isOpen[y, x];
    }

    // ゲームオーバーか 重い処理なので最適化したい
    public bool isGameover()
    {
        for (int i = 0; i < BoardSize.x; i++)
        {
            for (int j = 0; j < BoardSize.y; j++)
            {
                // 爆弾がオープンならゲームオーバー
                if (isBomb(i, j) && isOpen(i, j)) return true;
            }
        }

        return false;
    }

    // クリアか
    public bool isClear()
    {
        return cntClosedCell == BombCount;
    }

    // 指定した位置の八方周の爆弾数
    public int cntAroundBomb(int x, int y)
    {
        Debug.Assert(inBoard(x, y));
        return _cntAroundBomb[y, x];
    }

    // 盤面初期化 開幕爆破しないようにする
    public void start(int x, int y)
    {
        // 設置可能な位置にランダムに置く。爆弾数が増えると計算量が悪そう
        int k = BombCount;
        while (k > 0)
        {
            int px = r.Next(0, BoardSize.x - 1);
            int py = r.Next(0, BoardSize.y - 1);

            // 開けるセルとすでにあるセルには置かない
            if ((px == x && py == y) || isBomb(px, py)) continue;

            // 爆弾を置く
            _isBomb[py, px] = true;
            --k;
        }

        // セルごとに周囲の爆弾の数を数える
        for (int ny = 0; ny < BoardSize.y; ny++)
        {
            for (int nx = 0; nx < BoardSize.x; nx++)
            {
                // 爆弾がある箇所ならINFにして終了
                if (isBomb(nx, ny))
                {
                    _cntAroundBomb[ny, nx] = 100000;
                    continue;
                }

                for (int a = 0; a < 8; a++)
                {
                    int cx = nx + dx[a];
                    int cy = ny + dy[a];
                    if (!inBoard(cx, cy)) continue; // 範囲外なら処理しない
                    _cntAroundBomb[ny, nx] += isBomb(cx, cy) ? 1 : 0;
                }
            }
        }
    }

    // 指定したセルを開け、そのセルが0なら再起処理で周囲を開けていく
    public void open(int x, int y)
    {
        if (!inBoard(x, y)) return; // 範囲外なら処理しない
        if (isOpen(x, y)) return; // 開放済みなら処理しない

        // 未開なら初期化処理
        if (cntClosedCell == BoardSize.x * BoardSize.y) start(x, y);

        // 指定位置を開ける
        _isOpen[y, x] = true;
        --cntClosedCell;

        // 自分が0なら周囲を連鎖的に開ける
        if (cntAroundBomb(x, y) == 0)
        {
            for (int a = 0; a < 8; a++)
            {
                int cx = x + dx[a];
                int cy = y + dy[a];

                if (!inBoard(cx, cy)) continue; // 範囲外なら処理しない

                open(cx, cy);
            }
        }
    }

    // 指定したセルの旗の状態を反転する
    public void turnFlag(int x, int y)
    {
        if (!inBoard(x, y)) return; // 範囲外なら処理しない
        _isFlag[y, x] = !_isFlag[y, x];
    }
}