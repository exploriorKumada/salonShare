using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

    //public void SetEnemyAcount(Transform parentTF)
    //{
    //    Instantiate(ResourceLoader.GetEffectObject(31),parentTF);       
    //}

    //public void SetEnemyDead(Transform parentTF)
    //{
    //    Instantiate(ResourceLoader.GetEffectObject(1), parentTF);
    //}
	
    //public void SetBossEnemyAcount(Transform parentTF)
    //{
    //    Instantiate(ResourceLoader.GetEffectObject(1), parentTF);
    //}

    //public void SetDamagedAcount(Transform parentTF)
    //{
    //    Instantiate(ResourceLoader.GetEffectObject(1), parentTF);
    //}

    //public void SetDiceEffect(Transform parentTF)
    //{
    //    Instantiate(ResourceLoader.GetEffectObject(1), parentTF);
    //}

    public void SetEffect(Transform parentTF, int id, bool autoDestroyFlag = false)
    {

        if (UserData.GetUserName() == "")
        {

        }else if (parentTF.gameObject != null && gameObject != null)
        {
            var targetGO = Instantiate(ResourceLoaderOrigin.GetEffectObject(id), parentTF);
            if (autoDestroyFlag)
                StartCoroutine( AutoDestroy(targetGO));
        }
        
    }

    private IEnumerator AutoDestroy( GameObject targetGO)
    {
        yield return new WaitForSeconds(8f);

        Destroy(targetGO);
    }
}
