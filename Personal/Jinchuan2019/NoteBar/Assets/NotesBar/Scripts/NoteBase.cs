using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class NoteBase : MonoBehaviour
{
    public abstract void SetNextDuration(int duration);
    public abstract void DoNextNote();
    public abstract void SetId(int id);
    protected int duration;
    protected int id;
    protected int maxId;
}
