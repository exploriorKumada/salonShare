using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacterSytem : CharaControllBaseManager
{
    public void Initialized()
    {
      


        foreach (Transform Value in playerCharaParent)
            Destroy(Value.gameObject);

        CharaInfo charaInfo = DataManager.Instance.userTeamCharaInfos(0).First();
        GameObject playerModel = ResourceManager.Instance.GetCharaModel(charaInfo.charaMaster.id);
        GameObject player = Instantiate(playerModel, playerCharaParent);

        playerCharaController = new PlayerCharaController();
        playerCharaController = player.AddComponent<PlayerCharaController>();
        playerCharaController.charaInfo = charaInfo;
        playerCharaController.camera = camera;
        playerCharaController.Initialize();

        CharaControllBaseManagerInit();

        cameraTaget.transform.parent = playerCharaController.transform;
        cameraTaget.transform.localPosition = new Vector3(0, 1, 0);

   
    }
}
