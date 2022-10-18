/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.CardSystem
{
    public abstract class ChapterCard : ScriptableObject
    {
		public string _title;
		public Sprite _picture;
		[TextArea(5, 50)]
		public string _description;

		public void ApplyChapterBeginEffects()
		{
			BeginChapter();
		}

		protected abstract void BeginChapter();

		public abstract bool CheckChapterCompletion();
	}
}
