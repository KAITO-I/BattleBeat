//======================================//
/*******作成者:金川　2019-05-23**********/
//======================================//
/*ノーツの表示 */
//======================================//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotesManager : MonoBehaviour
{
    //ノーツの移動の向き
    [SerializeField]
    bool fromRightToLeft;
    //次の判定タイミング来るまでの秒数
    [SerializeField]
    float nextDuration;
    //ノートの個数
    [SerializeField]
    int totalNotes;
    //表示させたいノートのプリハブ
    [SerializeField]
    GameObject notePrefab;
}
