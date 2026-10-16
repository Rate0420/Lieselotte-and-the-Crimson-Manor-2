using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotTable", menuName = "SlotTable")]
public class SlotTable : ScriptableObject
{
    public List<SlotsTableDataBase> SlotsTableData;
}
[Serializable]
public class SlotsTableDataBase
{
    [Header("スロット1")] public int Slot1;
    [Header("スロット2")] public int Slot2;
    [Header("スロット3")] public int Slot3;
}
