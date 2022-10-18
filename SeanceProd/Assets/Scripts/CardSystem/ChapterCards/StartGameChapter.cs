/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.CardSystem
{
    [CreateAssetMenu(fileName = "New Start Game Chapter", menuName = "Scriptable Objects/Card System/Chapter Cards/Start Game Chapter", order = 50)]
    public class StartGameChapter : ChapterCard
    {
        public override bool CheckChapterCompletion()
        {
            throw new System.NotImplementedException();
        }

        protected override void BeginChapter()
        {
            throw new System.NotImplementedException();
        }
    }
}
