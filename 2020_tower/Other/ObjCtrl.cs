using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ObjCtrl : MonoBehaviour
{
    [NonSerialized] bool isOK;
    [NonSerialized]  Vector3 lastMousePosition;
    [NonSerialized]  Vector3 newAngle = new Vector3(0, 0, 0);

    [SerializeField] public EventTrigger eventTrigger;
    public GameObject obj;


    void Start()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((eventDate) => { isOK = true; });
        eventTrigger.triggers.Add(entry);

        EventTrigger.Entry end = new EventTrigger.Entry();
        end.eventID = EventTriggerType.EndDrag;
        end.callback.AddListener((eventDate) => { isOK = false; });
        eventTrigger.triggers.Add(end);

    }

    // Update is called once per frame
    void Update()
	{
        if (Input.GetMouseButtonDown(0))
        {
            //if (!isOK) return;

            // マウスクリック開始(マウスダウン)時にカメラの角度を保持(Z軸には回転させないため).
            newAngle = obj.transform.localEulerAngles;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!isOK) return;

            // マウスの移動量分カメラを回転させる.
            newAngle.y += (Input.mousePosition.x - lastMousePosition.x) * -0.1f;
            //newAngle.x -= (Input.mousePosition.y - lastMousePosition.y) * 0.1f;
            obj.gameObject.transform.localEulerAngles = newAngle;

            lastMousePosition = Input.mousePosition;
        }

    }
}