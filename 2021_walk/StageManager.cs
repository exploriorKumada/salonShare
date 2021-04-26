using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Explorior;
using System.Linq;
using TW.GameSetting;
using static StageUtlity;
using TMPro;


public class StageManager : MonoBehaviour
{
    /// <summary>
    /// プロックオリジナルオブジェクト
    /// </summary>
    [SerializeField] GameObject rootObject;

    [SerializeField] TextMeshProUGUI pointText;

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
        pointText.text = string.Empty;
        //初期ステージ生成
        StartCoroutine(BloackInit());

    }

    /// <summary>
    /// 初期ステージ生成
    /// </summary>
    /// <returns></returns>
    IEnumerator BloackInit()
    {
        SetBlock(Vector3.zero);

        for (int i = 1; i <= amount; i++)
        {
            SetBlock(new Vector3(0,0,i));
            SetBlock(new Vector3(i, 0, 0));
            SetBlock(new Vector3(0, 0, -i));
            SetBlock(new Vector3(-i,0, 0));
      
            for (int j = 1; j <= amount; j++)
            {
                SetBlock(new Vector3(i, 0, j));
                SetBlock(new Vector3(i, 0, -j));
                SetBlock(new Vector3(-i, 0, j));
                SetBlock(new Vector3(-i, 0, -j));
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
    public void SetBlock(Vector3 vector3, BlockType blockType = BlockType.Grass)
    {
        if (blockControllers.FirstOrDefault(b => b.vector3 == vector3) != null)
        {
            Debug.LogError("重なって表示されています" + vector3.ToString());
            //return;原因調査中
        }

        var stage = Instantiate(rootObject, rootObject.transform.parent);
        float ajust = rootObject.transform.localScale.x;
        stage.transform.localPosition = vector3 * ajust;
        stage.SetActive(true);
        stage.name = vector3.ToString();

        var bc = stage.GetComponent<BlockController>();
        bc.Init(vector3, blockType, SetNewLineInit);
 

        for (int i = 1; i <= vector3.y; i++)
        {
            var stage_bottom = Instantiate(rootObject, rootObject.transform.parent);
            stage_bottom.transform.localPosition = new Vector3(vector3.x * ajust, i * ajust, vector3.z * ajust);
            stage_bottom.SetActive(true);
            stage_bottom.name = stage_bottom.transform.localPosition.ToString();
            var bcbt = stage_bottom.GetComponent<BlockController>();
            bcbt.Init(vector3, BlockType.Soil);

            bc.downBlocks.Add(bcbt);
        }

        blockControllers.Add(bc);
    }




    public void SetNewLineInit(BlockController blockController)
    {

        if (beforeBlockController == null)
        {
            beforeBlockController = blockController;
            return;
        }

        pointText.text = blockController.vector3.ToString();

        int moveX = (int)(blockController.vector3.x - beforeBlockController.vector3.x);
        int moveZ = (int)(blockController.vector3.z - beforeBlockController.vector3.z);

        Debug.Log(beforeBlockController.vector3 + ":" + blockController.vector3);
        //履歴保存
        beforeBlockController = blockController;    
        //進む方向
        Direction direction = GetDirection(moveX, moveZ);
        if(IsCrossDirection(direction))
        {
            SetNewLine(direction, IsVerticalDirection(direction)? moveZ : moveX);
        }
        else
        {
            switch (direction)
            {
                case Direction.UPRIGHT:
                    SetNewLine(Direction.UP, moveZ);
                    SetNewLine(Direction.RIGHT, moveX);
                    break;
                case Direction.DOWNRIGHT:
                    SetNewLine(Direction.DOWN, moveZ);
                    SetNewLine(Direction.RIGHT, moveX);
                    break;
                case Direction.UPLEFT:
                    SetNewLine(Direction.UP, moveZ);
                    SetNewLine(Direction.LEFT, moveX);
                    break;
                case Direction.DOWNLEFT:
                    SetNewLine(Direction.DOWN, moveZ);
                    SetNewLine(Direction.LEFT, moveX);
                    break;
                default:
                    Debug.LogError("DirectionError:" + moveX + ":" + moveZ + ":" + direction);
                    SetNewLine(Direction.UP, moveZ);
                    break;
            }

        }
    }


    /// <summary>
    /// その方向に一列生成する 方向
    /// </summary>
    /// <param name="blockController"></param>
    public void SetNewLine(Direction direction, int addValue = 1 )
    {
        addValue = Mathf.Abs(addValue);

        //進む逆方向
        Direction oppoDirection = GetOppoDirection(direction);

        string log = "direction:" + direction + " oppoDirection:" + oppoDirection;
        log += "\naddValue:"+ addValue;

        if (addValue > 1)
            Debug.Log("２列以上生成");

        for(int i = 0;i< addValue; i++)
        {
            log += "\nadd:";

            //進む方向の先っちょのブロック配列
            List<BlockController> directionTipBlocks = GetDirectionTipBlocks(direction);

            foreach (var Value in directionTipBlocks)
            {
                Vector3 moveVector = GetUnitXZ(direction, Value, i);
                moveVector.y = GetYPosition(Value);

                log += moveVector;
                SetBlock(moveVector);
            }


            log += "\nsub:";
            //ブロック削除
            //進む逆方向の先っちょのブロック配列
            List<BlockController> oppodirectionTipBlocks = GetDirectionTipBlocks(oppoDirection);

            foreach(var Value in oppodirectionTipBlocks)
            {
                log += Value.vector3;
                DestroyBlock(Value);
            }
        }

        Debug.Log(log);
    }


    public int GetYPosition(BlockController blockController)
    {
        List<BlockController> aroundBC = GetAroundBC(blockController);

        //高地の数
        List<int> zs = new List<int>();
        aroundBC.ForEach(bc => zs.Add((int)bc.vector3.z));

        int yMax = zs.Count == 0 ? 0 : zs.GetAtRandom();
        //Debug.Log("zMax" + zMax);

        int number = Random.Range(0, 100);

        int returnY = 0;

        if (number < 70)
        {
            returnY = (int)blockController.vector3.y;
        }
        else if (number < 95)
        {
            returnY = yMax - 1;

            if (returnY < 0)
                returnY = 0;

        }
        else
        {
            returnY = yMax + 1;
        }

        return returnY;

    }

    /// <summary>
    /// その方向の先っちょのブロック配列
    /// </summary>
    List<BlockController> GetDirectionTipBlocks(Direction direction)
    {
        //その方向の最高値
        int num = GetAddBlockNum(direction);

        //Debug.Log("direction:" + direction + " num:" + num);
        //※進行方向 初期値の場合 UP:yが10 DOWN:yが-10 RIGHT:xが10 LEFT:xが-10 のブロック配列
        //※進行逆方向　初期値の場合 UP:yが-10 DOWN:yが10 RIGHT:xが-10 LEFT:xが10 のブロック配列
        if (IsVerticalDirection(direction))
            return blockControllers.Where(bc => bc.vector3.z == num).ToList();
        else
            return blockControllers.Where(bc => bc.vector3.x == num).ToList();
    }

    /// <summary>
    /// その方向の最高値
    /// </summary>
    public int GetAddBlockNum(Direction direction)
    {
        if (direction == Direction.UP) return (int)blockControllers.Max(bc => bc.vector3.z);
        if (direction == Direction.DOWN) return (int)blockControllers.Min(bc => bc.vector3.z);
        if (direction == Direction.RIGHT) return (int)blockControllers.Max(bc => bc.vector3.x);
        if (direction == Direction.LEFT) return (int)blockControllers.Min(bc => bc.vector3.x);

        Debug.LogError("想定外:" + direction);

        return (int)blockControllers.Max(bc => bc.vector3.y);
    }


    public BlockController GetBlockController(int x, int z) => blockControllers.FirstOrDefault(b => b.vector3.x == x && b.vector3.z == z);

    public void DestroyBlock(BlockController blockController)
    {
        foreach(var Value in blockController.downBlocks)
        {
            Destroy(Value.gameObject);
        }

        DestroyBlock((int)blockController.vector3.x, (int)blockController.vector3.z);
    }
    public void DestroyBlock(int x, int z)
    {
        var block = GetBlockController(x, z);
        Destroy(block.gameObject);
        blockControllers.Remove(block);
    }

    public List<BlockController> GetAroundBC(BlockController blockController) => GetAroundBC((int)blockController.vector3.x, (int)blockController.vector3.y);
    public List<BlockController> GetAroundBC(int bc_x, int bc_y)
    {
        var returnValue = new List<BlockController>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                var bc = GetBlockController(bc_x + x, bc_y + y);

                if (bc != null)
                    returnValue.Add(bc);
            }
        }

        return returnValue;
    }
}
