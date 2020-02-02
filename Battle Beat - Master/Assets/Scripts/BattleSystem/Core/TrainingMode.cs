using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingMode : BattleManager
{
    // Start is called before the first frame update
    void Start()
    {
        onGame = true;
        rythmManager.Init();
        SoundManager.Instance.PlayBGM(BGMID.InGame0);
   
        p1 = TurnManager._instance.GetPlayer(1);
        p2 = TurnManager._instance.GetPlayer(2);
        gameState = new TrainingModeMainState(this);
        TurnManager._instance.GetPlayer(1).onGame = true;
        TurnManager._instance.GetPlayer(2).onGame = true;
        rythmManager.StartRythm();
    }

    // Update is called once per frame
    void Update()
    {
        gameState = gameState?.Run();
    }
    protected class TrainingModeMainState : GameState
    {
        public TrainingModeMainState(BattleManager manager) : base(manager)
        {
            
        }
        public override GameState Run()
        {
            if (ControllerManager.Instance.GetButtonUp_Menu(ControllerManager.Button.Select))
            {
                SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
            }
            return this;
        }
    }
    Player p1;
    Player p2;
    float p1Hp;
    float p2Hp;
    int p1HpNoChangeTurn=0;
    int p2HpNoChangeTurn=0;
    [SerializeField]
    int RecoverTurn = 3;
    public override void TurnProcess()
    {
        if (p1.GetHp() != p1Hp)
        {
            p1Hp = p1.GetHp();
            p1HpNoChangeTurn = 0;
        }
        else
        {
            p1HpNoChangeTurn++;
        }
        if (p2.GetHp() != p2Hp)
        {
            p2Hp = p2.GetHp();
            p2HpNoChangeTurn = 0;
        }
        else
        {
            p2HpNoChangeTurn++;
        }
        if (p1HpNoChangeTurn> RecoverTurn)
        {
            p1HpNoChangeTurn = 0;
            p1Hp = p1.HpMax;
            p1.SetHp(p1.HpMax);
        }
        if (p2HpNoChangeTurn > RecoverTurn)
        {
            p2HpNoChangeTurn = 0;
            p2Hp = p2.HpMax;
            p2.SetHp(p2.HpMax);
        }
    }
}
