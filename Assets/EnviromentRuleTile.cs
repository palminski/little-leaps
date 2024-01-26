using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
[Serializable]
public class EnviromentRuleTile : RuleTile
{

    public enum SibingGroup
    {
        Enviroment
    }
    public SibingGroup sibingGroup;

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor)
        {
            case TilingRule.Neighbor.This:
                {
                    return other is EnviromentRuleTile
                        && (other as EnviromentRuleTile).sibingGroup == this.sibingGroup;
                }
            case TilingRule.Neighbor.NotThis:
                {
                    return !(other is EnviromentRuleTile
                        && (other as EnviromentRuleTile).sibingGroup == this.sibingGroup);
                }
        }

        return base.RuleMatch(neighbor, other);
    }
}

