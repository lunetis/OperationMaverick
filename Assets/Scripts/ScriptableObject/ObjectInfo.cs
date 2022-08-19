using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectInfo", menuName = "Scriptable Object Asset/ObjectInfo")]
public class ObjectInfo : ScriptableObject
{
    [SerializeField]
    string objectName;
    [SerializeField]
    string objectNickname;
    [SerializeField]
    int score;
    [SerializeField]
    int hp;
    [SerializeField]
    bool mainTarget;

    [SerializeField]
    float warningDistance;

    [SerializeField]
    bool isGroundObject;

    [SerializeField]
    bool variesHpByDifficulty = false;

    [SerializeField]
    int easyHp;
    [SerializeField]
    int normalHp;
    [SerializeField]
    int hardHp;
    [SerializeField]
    int aceHp;

    public string ObjectName
    {
        get { return objectName; }
    }
    public string ObjectNickname
    {
        get { return objectNickname; }
    }
    public int Score
    {
        get { return score; }
    }
    public int HP
    {
        get { return hp; }
    }
    public bool MainTarget
    {
        get { return mainTarget; }
    }

    public float WarningDistance
    {
        get { return warningDistance; }
    }

    public bool IsGroundObject
    {
        get { return isGroundObject; }
    }

    public bool VariesHpByDifficulty
    {
        get { return variesHpByDifficulty; }
    }

    public int EasyHp
    {
        get { return easyHp; }
    }
    public int NormalHp
    {
        get { return normalHp; }
    }
    public int HardHp
    {
        get { return hardHp; }
    }
    public int AceHp
    {
        get { return aceHp; }
    }
}
