using System.Collections.Generic;
using TurnBasedStrategy;
using UnityEngine;

public struct SpawnData
{
    public int factionSide;
    public Transform prefab;
    public Transform factionTransform;
    public Quaternion rotation;
    public Transform[] spawnPoints;
    public int count;
    public List<Mech> mechs;
}