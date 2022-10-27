/// Author: Julien Haigron
/// Last modified by: Julien Haigron

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Seance.Interactions
{
	/// <summary>
	/// Edouard
	/// </summary>
	public class Dice20 : MonoBehaviour
	{
		[Header("Dice")]
		[SerializeField] private int _diceValue = 20;
		[SerializeField] private int _expectedValue = 20;

		[Header("Cheat values")]
		[SerializeField] private float _cheatScore = 0f;
		[SerializeField] private float _cheatScoreDecay = 0.666f;
		[SerializeField] private float _cheatScorePunish = 1f;
		[SerializeField] private float _cheatTolerance = 2f;

		[Header("Components")]
		private Animator _animator;
		//private DiceClick _diceClick;
		//private CameraShake _mainCameraShake;
		private Vignette _vignetteEffect;
		[SerializeField] GameObject _loseUI;

		public int ExpectedValue { get => _expectedValue; set => _expectedValue = value; }

		public int DiceValue { get => _diceValue; set => _diceValue = Mathf.Clamp(value, 0, 20); }

		#region Unity events

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			//_diceClick = GetComponentInChildren<DiceClick>();
			//_mainCameraShake = Camera.main.GetComponent<CameraShake>();

			//_diceClick.onLeftClick += IncreaseDiceValue;
			//_diceClick.onRightClick += DecreaseDiceValue;
		}

		public void Init(Volume volumePostprocess)
		{
			_vignetteEffect = (Vignette)volumePostprocess.profile.components[0];
		}

		private void Start()
		{
			DiceValue = 10;

			UpdateAnimator();
		}

		#endregion

		#region Public methods
		public void IncreaseDiceValue()
		{
			//CheckCheat(1);
			DiceValue++;
			UpdateAnimator();

			//Play dice roll sound effect
			//AudioManager.Instance.PlayEffectOnTmpSource();
		}

		public void DecreaseDiceValue()
		{
			DecreaseDiceValue(1);
		}

		public void DecreaseDiceValue(int value)
		{
			DiceValue -= value;

			if (DiceValue < 1)
			{
				_loseUI.SetActive(true);
			}
			else
			{
				UpdateAnimator();
			}
		}

		#endregion

		#region Private methods

		private void CheckCheat(int valueDirection)
		{
			if (valueDirection == Sign(_expectedValue - _diceValue))
			{
				_cheatScore = Mathf.Max(_cheatScore - _cheatScoreDecay, 0f);
			}
			else
			{
				_cheatScore += _cheatScorePunish;
				if (_cheatScore >= _cheatTolerance)
				{
					CheatFeedback();
				}
			}
		}

		private void CheatFeedback()
		{
			// Don't work: Cinemachine freeze camera position
			//StartCoroutine(_mainCameraShake.Shake(0.2f, 1f, 0.1f, 0.1f));

			StartCoroutine(VignetteEffect(0.2f, 0.1f));
		}

		// Mathf.Sign() sucks...
		private int Sign(float number)
		{
			return number < 0 ? -1 : (number > 0 ? 1 : 0);
		}

		private IEnumerator VignetteEffect(float duration, float baseIntensity)
		{
			float elapsed = 0;
			while (elapsed < duration)
			{
				float intensity = baseIntensity * (1 + ((_cheatScore - _cheatTolerance) / 20));
				if (_vignetteEffect != null)
				{
					_vignetteEffect.intensity.overrideState = true;
					_vignetteEffect.intensity.value = Mathf.Lerp(0, intensity, Mathf.Sin((elapsed / duration) * Mathf.PI));
				}

				elapsed += Time.deltaTime;
				yield return 0;
			}
		}

		private void UpdateAnimator()
		{
			_animator.SetInteger("diceValue", _diceValue);
		}

		#endregion
	}
}
