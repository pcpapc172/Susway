using UnityEngine;

public class CharacterPopup : MonoBehaviour
{
	private CharacterModel characterModel;

	private UIModelController uimodelController;

	private void Awake()
	{
		uimodelController = Object.FindObjectOfType(typeof(UIModelController)) as UIModelController;
	}

	protected void SetCharacter(Characters.CharacterType character)
	{
		uimodelController.ClearTutorialPopup();
		if (characterModel == null)
		{
			characterModel = NGUITools.AddChild(uimodelController.TutorialPopupAnchor, uimodelController.ModelPrefab).GetComponent<CharacterModel>();
			Utility.SetLayerRecursively(characterModel.transform, uimodelController.TutorialPopupAnchor.layer);
			Transform transform = characterModel.transform;
			transform.localPosition = new Vector3(35f, 23f, 50f);
			transform.localScale = Vector3.one * 19f;
			transform.localEulerAngles = new Vector3(50f, 200f, 0f);
			characterModel.HideBlobShadow();
		}
		characterModel.ChangeCharacterModel(character.ToString());
		characterModel.HideAllPowerups();
		characterModel.StartIdlePopupAnimations();
	}

	protected virtual void OnDisable()
	{
		if (!(UIScreenController.Instance == null) && !UIScreenController.Instance.stoppingFromEditor)
		{
			uimodelController.ClearTutorialPopup();
		}
	}
}
