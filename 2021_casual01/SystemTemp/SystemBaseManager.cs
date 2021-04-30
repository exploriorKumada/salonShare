using UnityEngine;

public class SystemBaseManager : MonoBehaviour
{
    /// <summary>
    /// キャンバスに対してのタップ位置を取得する
    /// </summary>
    /// <param name="canvas"></param>
    /// <returns></returns>
    public Vector2 GetMousePosition(Canvas canvas)
    {
        Vector2 localpos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), Input.mousePosition, canvas.worldCamera, out localpos);
        return localpos;
    }

    public void DebugLog(bool isActive = true, string value = "")
    {
        if (isActive)
            Debug.Log(value);
    }

}
