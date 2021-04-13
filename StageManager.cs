using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Explorior;
using System.Linq;
using TW.GameSetting;


public class StageManager : MonoBehaviour
{
    /// <summary>
    /// プロックオリジナルオブジェクト
    /// </summary>
    [SerializeField] GameObject rootObject;

    /// <summary>
    /// ブロック一つ一つの管理変数
    /// </summary>
    List<BlockController> blockControllers = new List<BlockController>();

    /// <summary>
    /// ブロック半径
    /// </summary>
    int amount = 10;

    /// <summary>
    /// ゲームマネージャー　上位クラスだから引き継いで置くのは悪いけど、結局便利なので
    /// </summary>
    private GameManager gameManager;


    BlockController beforeBlockController;


    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="_gameManager"></param>
    public void Initialized(GameManager _gameManager)
    {
        blockControllers = new List<BlockController>();
        rootObject.transform.ParentTransInitialize();
        rootObject.SetActive(false);
        gameManager = _gameManager;

        //初期ステージ生成
        StartCoroutine(BloackInit());

    }

    /// <summary>
    /// 初期ステージ生成
    /// </summary>
    /// <returns></returns>
    IEnumerator BloackInit()
    {

        SetBlock(0, 0);

        for (int i = 1; i <= amount; i++)
        {
            SetBlock(0, i);
            SetBlock(i, 0);
            SetBlock(0, -i);
            SetBlock(-i, 0);

            for (int j = 1; j <= amount; j++)
            {
                SetBlock(i, j);
                SetBlock(i, -j);
                SetBlock(-i, j);
                SetBlock(-i, -j);
            }
        }

        //本当はfor文の中に入れて、処理不可軽減を入れたいのに、何でかrootオブジェクト消え去るので調査中
        yield return null;

    }


    /// <summary>
    /// ブロック一個分の生成
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetBlock(int x, int y, BlockType blockType = BlockType.Grass)
    {
        var stage = Instantiate(rootObject, rootObject.transform.parent);
        float ajust = rootObject.transform.localScale.x;
        stage.transform.localPosition = new Vector3(x * ajust, 0, y * ajust);
        stage.SetActive(true);
        stage.name = "(" + x + "," + y + ")";

        var bc = stage.GetComponent<BlockController>();
        bc.Init(x, y, blockType, SetNewLine);
        blockControllers.Add(bc);
    }

    public void SetNewLine(BlockController blockController)
    {
        if (beforeBlockController == null)
        {
            beforeBlockController = blockController;
        }

        int moveX = blockController.x - beforeBlockController.x;
        int moveY = blockController.y - beforeBlockController.y;

        beforeBlockController = blockController;

        Debug.Log("x:" + moveX + "y:" + moveY);
        //進む方向
        Direction direction = GetDirection(moveX, moveY);
        //進む逆方向
        Direction oppoDirection = GetDirection(-moveX, -moveY);

        //進む方向のブロックの一番先っちょの座標
        int directionMax = GetAddBlockNum(direction);
        //進む逆方向のブロックの一番先っちょの座標
        int directionMin = GetAddBlockNum(oppoDirection);

        Debug.Log("direction:" + direction);
        Debug.Log("oppoDirection" + oppoDirection + " directionMax:" + directionMax + " directionMin:" + directionMin);

        bool vertical = direction == Direction.UP || direction == Direction.DOWN;
        bool upperRight = direction == Direction.UP || direction == Direction.RIGHT;

        //進む方向の先っちょのブロック配列
        List<BlockController> directionTipBlocks = blockControllers.Where(bc =>
            (vertical ? bc.y : bc.x) == (upperRight ? directionMax : directionMin)).ToList();
        //進む逆方向の先っちょのブロック配列
        List<BlockController> oppodirectionTipBlocks = blockControllers.Where(bc =>
            (!vertical ? bc.y : bc.x) == (upperRight ? directionMin : directionMax) ).ToList();


        //増やす量
        int _amount = amount * 2;

        directionTipBlocks.ForEach(bc => Debug.Log("directionTipBlocks:" + bc.x + ":" + bc.y));
        //増やす基準点
        int first = directionTipBlocks.Min(block => (vertical ? block.x : block.y));

        Debug.Log("first:" + first);

        //指定の方向に、指定の数に1列
        for (int i = 0; i < _amount; i++)
        {
            int x = (vertical ? first + i : direction == Direction.RIGHT ? directionMax + 1: directionMax - 1);
            int y = (!vertical ? first + i : direction == Direction.UP ? directionMax + 1 : directionMax - 1);

            Debug.Log("adds:" + x +":" +y);
            SetBlock(x, y);
        }

        //ブロック削除
        oppodirectionTipBlocks.ForEach(bc => Debug.Log("subs:" + bc.x + ":" + bc.y));
        oppodirectionTipBlocks.ForEach(bc => DestroyBlock(bc));
    }

    public Direction GetDirection(int x, int y)
    {
        if (x == 0 && y == 1) return Direction.UP;
        if (x == 0 && y == -1) return Direction.DOWN;
        if (x == 1 && y == 0) return Direction.RIGHT;
        return Direction.LEFT;
    }

    public int GetAddBlockNum(Direction direction)
    {
        if (direction == Direction.UP) return blockControllers.Max(bc => bc.y);
        if (direction == Direction.DOWN) return blockControllers.Min(bc => bc.y);
        if (direction == Direction.RIGHT) return blockControllers.Max(bc => bc.x);
        return blockControllers.Min(bc => bc.x);
    }


    public BlockController GetBlockController(int x, int y)
    {
        return blockControllers.FirstOrDefault(b => b.x == x && b.y == y);
    }

    public void DestroyBlock(BlockController blockController) { DestroyBlock(blockController.x, blockController.y); }
    public void DestroyBlock(int x, int y)
    {
        var block = GetBlockController(x, y);
        Destroy(block.gameObject);
        blockControllers.Remove(block);
    }

}
