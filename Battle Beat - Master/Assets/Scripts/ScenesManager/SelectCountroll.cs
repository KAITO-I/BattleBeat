using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectCountroll : MonoBehaviour
{
    [SerializeField]
    GameObject FlameObj;
    [SerializeField,Header("ボスコマンド設定")]
    public ControllerManager.Button[] Commands;//Boss
    [SerializeField]
    Sprite _boss_Sprite;//Boss立ち絵
    [SerializeField]
    Sprite _void;//ボスの時にひつよう
    [SerializeField]
    GameObject _Ready;
    [SerializeField]
    Text text;

    public List<CharaSelectObj> CharaObj;//中央を動かしている

    ControllerManager.Controller _1Pcontroller = ControllerManager.Instance.Player1;
    ControllerManager.Controller _2Pcontroller = ControllerManager.Instance.Player2;
    SceneLoader loader = SceneLoader.Instance;
    SoundManager _soundManager=SoundManager.Instance;

    //テキストの透明度変更
    float time;
    public float interval;
    float _changeTransparency = 1;

    //Boss変数
    bool[] _boss=new bool[2] {false,false};
    void BossSelect(int playerid)
    {
        if( playerid==0) {
            _boss[0] = true;
            Setting.p1c = (Setting.Chara)4;
        }
        else if(playerid == 1)
        {
            _boss[1] = true;
            Setting.p2c = (Setting.Chara)4;
        }
    }

    //スクリプト改変用
    BaseSelect _1P;
    BaseSelect _2P;

    void Start()
    {
        CharaObj = new List<CharaSelectObj>();
        foreach (Transform v in FlameObj.transform)
        {
            var CObj = v.GetComponent<CharaSelectObj>();
            CObj.Init();
            CharaObj.Add(CObj);
        }
        _Ready.SetActive(false);

        string commandStr = string.Empty;
        foreach(var v in Commands)
        {
            commandStr += v.ToString();
        }
        CommandManager.instance.registCommand(commandStr, BossSelect);
        _soundManager.PlayBGM(BGMID.CharacterSelect);

        Color color = text.color;
        color.a = 0;
        text.color = color;

        //スクリプト改変用
        _1P = GetComponent<Select1P>();
        _2P = GetComponent<Select2P>();
        _1P.Inctance(this, _soundManager);
        _2P.Inctance(this, _soundManager);
    }
    void Update()
    {
        if (loader.isLoading) return;
        _1P.playerUpdate();
        _2P.playerUpdate();
        //Charaが二人とも選択されたとき
        if (_1P.GetItem<bool>("player")&& _2P.GetItem<bool>("player"))
        {
            //決定ボタンを入力したら
            if (ControllerManager.Instance.GetButtonDown_Menu(ControllerManager.Button.Start))
            {
                if (!_boss[0])//ボスではない
                {
                    Setting.p1c = (Setting.Chara)_1P.GetItem<int>();
                }
                if (!_boss[1])//ボスではない
                {
                    Setting.p2c = (Setting.Chara)_2P.GetItem<int>();
                }
                _soundManager.PlaySE(SEID.CharacterSelect_GameStart);
                SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainGame);
            }
            _Ready.SetActive(true);
            TextColorChange();//「Startボタンで開始」テキスト表示
            return;
        }
        _Ready.SetActive(false);
    }
    //==============テキストのフェードイン、アウト============
    void TextColorChange()
    {
        //一定期間で表示・非表示
        time += Time.deltaTime * _changeTransparency;
        if (time >= interval) { _changeTransparency = -1; time = interval; }
        if (time <= 0) { _changeTransparency = 1; time = 0; }
        Color color = text.color;
        color.a = time / interval;
        text.color = color;
    }
    //==========================================
}
