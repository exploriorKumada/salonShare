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

        SetUnityStage(0, 0);

        for (int i = 1; i <= amount; i++)
        {
            SetUnityStage(0, i);
            SetUnityStage(i, 0);
            SetUnityStage(0, -i);
            SetUnityStage(-i, 0);

            for (int j = 1; j <= amount; j++)
            {
                SetUnityStage(i, j);
                SetUnityStage(i, -j);
                SetUnityStage(-i, j);
                SetUnityStage(-i, -j);
            }
        }

        //本当はfor文の中に入れて、処理不可軽減を入れたいのに、何でかrootオブジェクト消え去るので調査中
        yield return null;

    }

    int recentryId = 0;
    /// <summary>
    /// ブロック一個分の生成
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetUnityStage(int x, int y, BlockType blockType = BlockType.Grass)
    {
        recentryId++;
        var stage = Instantiate(rootObject, rootObject.transform.parent);
        float ajust = rootObject.transform.localScale.x;
        stage.transform.localPosition = new Vector3(x * ajust, 0, y * ajust);
        stage.SetActive(true);
        stage.name = "(" + x + "," + y + ")";

        blockControllers.Add(new BlockController(x, y, recentryId, blockType));
    }


    public BlockController GetBlockController(int x, int y)
    {
        return blockControllers.FirstOrDefault(b => b.x == x && b.y == y);
    }

    public BlockController GetBlockController(int id)
    {
        return blockControllers.FirstOrDefault(b => b.id == id);
    }

    public void BlockDestroy(int x, int y)
    {
        var block = GetBlockController(x, y);
        Destroy(block.gameObject);
        blockControllers.Remove(block);
    }

}
