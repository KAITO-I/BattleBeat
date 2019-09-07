using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackItemBase : MonoBehaviour
{
    [SerializeField]
    //攻撃するプレイヤーのID
    protected int RootID;
    //攻撃発生した時プレイヤーの座標
    protected int Row;
    protected int Col;
    //範囲がy軸対称で反転するか（プレイヤー１と２を区別するため）
    protected bool Reverse;
    //攻撃範囲
    [SerializeField]
    [HideInInspector]
    public List<Vector2Int> Area; //座標
    //リングの大きさ
    public Vector2Int BoardSize = new Vector2Int(3, 3);


    protected Player RootPlayer;
    protected Player Opponent;
    protected bool isCancel;

    public int CoolDown;
    public float SpCost;

    [SerializeField]
    protected Effekseer.EffekseerEffectAsset EffekseerEffectAsset;

    //初期化関数
    public virtual void Init(int row, int col, bool reverse, int root)
    {
        Row = row;
        Col = col;
        Reverse = reverse;
        RootID = root;
        isCancel = false;
        RootPlayer = AttackManager._instance.GetPlayer(RootID);

        int OpponentID = 3 - RootID;

        Opponent = AttackManager._instance.GetPlayer(OpponentID);

        transform.SetParent(RootPlayer.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    //ターンの処理（使い方はAttackManagerクラスに記載しています）
    public virtual void TurnPreprocess() { }
    public virtual void TurnProcessPhase0_Main() { }
    public virtual void TurnProcessPhase1_Main() { }
    public virtual void TurnProcessPhase2_Main() { }
    public virtual void TurnProcessPhase0_Prediction_Request() { }
    public virtual void TurnProcessPhase1_Prediction_Request() { }
    public virtual void TurnProcessPhase2_Prediction_Request() { }
    public virtual void DamegePhase() {
        if (CheckDamage())
        {
            foreach (var p in Area)
            {
                Vector2Int pos = AreaProcess(p);

                if (pos == Opponent.Pos)
                {
                    PassDamage(Opponent);
                }
            }
        }
    }
    public virtual void TurnPostprocess() { }

    //攻撃が終わってるか
    public virtual bool isEnd() { return isCancel; }
    //プレイヤーにダメージを与える
    public virtual void PassDamage(Player player) { player.TakeDamage(0); }
    //このターンにダメージ判定があるか
    public virtual bool CheckDamage() { return false; }
    //posの場所にidがrootIdのプレイヤーがダメージを受けるべきか
    public virtual bool CheckArea(Vector2Int pos, int rootId,List<Vector2Int> Area = null)
    {
        if (Area == null)
        {
            Area = this.Area;
        }
        Vector2Int _Area;
        for (int i = 0; i < Area.Count; i++)
        {
            _Area = AreaProcess(Area[i]);
            if (pos == _Area)
            {
                if (rootId != RootID)
                {
                    return true;
                }
            }
        }
        return false;
    }
    //このスキルが中断された時の処理
    public virtual void OnInterruption() { }
    public virtual void Cancel() { isCancel = true; }
    protected virtual Vector2Int AreaProcess(Vector2Int Grid)
    {
        Vector2Int pos;
        if (Reverse)
        {
            pos = new Vector2Int(Col - Grid.x, Row + Grid.y - 1);
        }
        else
        {
            pos = new Vector2Int(Col + Grid.x, Row + Grid.y - 1);
        }

        return pos;
    }
    protected void ChangeFloorColor(Floor.Colors color, int mode = 0, List<Vector2Int> Area = null)
    {
        if(Area == null)
        {
            Area = this.Area;
        }
        foreach (var Grid in Area)
        {
            Vector2Int pos;
            pos = AreaProcess(Grid);
            var floor = BoardManager._instance.GetGameObjectAt(pos, RootID);
            if (floor != null)
            {
                var floorObj = floor.GetComponent<Floor>();
                if (mode == 0)
                {
                    floorObj.AddColor(color);
                }
                else
                {
                    floorObj.SubColor(color);
                }
            }
        }
    }
}
