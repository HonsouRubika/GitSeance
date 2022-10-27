/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Seance.PostProcess
{
    public class PostProcessManager : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] [Range(0f, 5f)] float _transitionDuration = 1f;

        [Header("References")]
        [SerializeField] Volume _baseVolume;
        [SerializeField] Volume _watchedVolume;
        [SerializeField] Volume _spottedVolume;

        Dictionary<PostProcessType, Volume> _volumes = new();
        bool _isLerping = false;

        private void Start()
        {
            _volumes.Add(PostProcessType.Base, _baseVolume);
            _volumes.Add(PostProcessType.Watched, _watchedVolume);
            _volumes.Add(PostProcessType.Spotted, _spottedVolume);
		}

        //Needs an overload to force transition
        public void SetPostProcess(PostProcessType type)
        {
            if (_isLerping)
                return;

            _isLerping = true;

            foreach(KeyValuePair<PostProcessType, Volume> kvp in _volumes)
            {
                if(kvp.Key != type && kvp.Value.weight > 0)
                {
                    FloatLerper lerper = new();
                    lerper.Lerp(1f, 0f, _transitionDuration, value => kvp.Value.weight = value);
                }
            }

            FloatLerper _lerper = new();
            _lerper.Lerp(0f, 1f, _transitionDuration, value => _volumes[type].weight = value, () => _isLerping = false);
        }
    }

    public enum PostProcessType
    {
        Base,
        Watched,
        Spotted
    }
}
