/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Seance.Player
{
	public class PlayerInputHandler : MonoBehaviour
	{
		[Header("Params")]
		[SerializeField] float _clickRaycastLength = 50f;
		[SerializeField] bool _debugRaycasts = true;

		[Header("References")]
		[SerializeField] LayerMask _interactableLayer;

		public void OnClick(InputAction.CallbackContext context)
		{
			if (!context.started)
				return;

			RaycastHit hit;

			Ray ray = GameManager.Lobby._ownedPlayerInstance.CameraController.Camera.ScreenPointToRay(Input.mousePosition);

			if (_debugRaycasts)
				Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, .8f);

			if (Physics.Raycast(ray, out hit, _clickRaycastLength, _interactableLayer))
			{
				Interactor interactor;

				if (hit.transform.TryGetComponent(out interactor))
				{
					interactor.Interact();
				}
			}
		}
	}
}
