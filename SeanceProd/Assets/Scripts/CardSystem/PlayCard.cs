/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using UnityEngine;

namespace Seance.CardSystem
{
    public abstract class PlayCard : ScriptableObject
    {
        public string _title;
        public Sprite _picture;
        public string _description;

		public void ApplyCardEffects()
		{
			Use();
		}

		protected abstract void Use();
	}
}