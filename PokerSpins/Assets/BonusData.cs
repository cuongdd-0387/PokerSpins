using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusData", menuName = "ScriptableObjects/BonusData", order = 1)]
public class BonusData : ScriptableObject
{
    public List<PayoutData> listPayout;
}