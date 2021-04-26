using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager : ScenePrefab {

    public Action action;

    public Dictionary<string, Sprite> charaSpriteList = new Dictionary<string, Sprite>();

    public IEnumerator SetInfo( Action action )
    {
        
        AddPopup("Popup_Loding");

        Caching.ClearCache();

        this.action = action;
        yield return null;
    }

    //public IEnumerator DownloadAndCache(string url, int version = 1 )
    //{
    //    // キャッシュシステムの準備が完了するのを待ちます
    //    while (!Caching.ready)
    //        yield return null;

    //    // 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードするか、
    //    //  またはダウンロードしてキャッシュに格納します。
    //    using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
    //    {
    //        yield return www;
    //        if (www.error != null)
    //        {
    //            Debug.Log("WWWダウンロードにエラーがありました:" + www.error);
    //        }
    //        else
    //        {
    //            Debug.Log("not error");
    //        }


    //        AssetBundle bundle = www.assetBundle;


    //        foreach (var Value in CharaSetting.AllCharaList())
    //        {
    //            string assetName = Value.charaID;
    //            if (assetName == "")
    //            {
    //                Instantiate(bundle.mainAsset);
    //            }
    //            else
    //            {
    //                Texture2D tex = bundle.LoadAsset<Texture2D>(assetName);
    //                Sprite texture_sprite = SpriteFromTexture2D(tex);
    //                //charaSpriteList[Value.charaID] = texture_sprite;

    //            }
    //        }


    //        Loading.Close();
    //        action();
    //        // メモリ節約のため圧縮されたアセットバンドルのコンテンツをアンロード
    //        bundle.Unload(false);

    //    } // memory is freed from the web stream (www.Dispose() gets called implicitly)
    //}


    public Sprite SpriteFromTexture2D(Texture2D texture)
    {
        Sprite sprite = null;
        if (texture)
        {
            //Texture2DからSprite作成
            sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), new Vector2( 0.5f,0.5f));
        }
        return sprite;
    }

}
