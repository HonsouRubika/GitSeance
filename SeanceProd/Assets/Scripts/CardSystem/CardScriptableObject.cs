/// Author: Haigron Julien
/// Last modified by: Haigron Julien

using UnityEngine;
using System.Collections.Generic;

namespace Seance.CardSystem
{
    [CreateAssetMenu(fileName = "New Action Card", menuName = "Scriptable Objects/Card System/Action Card", order = 51)]
    public class CardScriptableObject : ScriptableObject
    {
        public string _name;
        public Sprite _picture;
        public string _description;
        public List<CardTarget> _targets;
        public List<CardEffect> _effects;
        public List<int> _effectsValues;
    }

    public enum CardEffect
    {
        CLASSIQUE_DAMAGE,
        AMRMOR_DAMAGE,
        TRUE_DAMAGE,
        CONDITION_DAMAGE, //TODO : handle conditions
        DRAW,
        GAIN_ARMOR,
        DISCARD,
        TANK,
        HEAL,
        BUFF //TODO : handle buffs
    }

    public enum CardTarget
    {
        SEFL,
        TARGET_ALLY,
        EVERY_ALLY,
        TARGET_ENEMY,
        EVERY_ENEMY,
        NONE
    }
}