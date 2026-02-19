using System;
using UnityEngine;

public class UIModelController : MonoBehaviour
{
	public enum ModelScreen
	{
		Character = 0,
		GameOver = 1,
		TutorialPopup = 2
	}

	public GameObject CharacterAnchor;

	public GameObject GameOverAnchor;

	public GameObject MysteryBoxAnchor;

	public GameObject TutorialPopupAnchor;

	public GameObject ModelPrefab;

	private CharacterModel _cachedActiveModel;

	private Action _onChangedCurrentlyShown;

	private int _currentlyShownModel;

	private static UIModelController _instance;

	public int currentlyShownModel
	{
		get
		{
			return _currentlyShownModel;
		}
	}

	public static UIModelController Instance
	{
		get
		{
			return _instance ?? (_instance = UnityEngine.Object.FindObjectOfType(typeof(UIModelController)) as UIModelController);
		}
	}

	public void ActivateGameOverModel()
	{
		_ActivateModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, ModelScreen.GameOver);
	}

	public void AddOnChangedCurrentlyShownHandler(Action handler)
	{
		_onChangedCurrentlyShown = (Action)Delegate.Combine(_onChangedCurrentlyShown, handler);
	}

	public void RemoveOnChangedCurrentlyShownHandler(Action handler)
	{
		_onChangedCurrentlyShown = (Action)Delegate.Remove(_onChangedCurrentlyShown, handler);
	}

	public void SelectCurrentModel()
	{
		PlayerInfo.Instance.currentCharacter = _currentlyShownModel;
		PlayerInfo.Instance.SaveIfDirty();
		if (Game.Instance != null)
		{
			Game.Instance.Character.characterModel.ChangeCharacterModel(((Characters.CharacterType)currentlyShownModel).ToString());
		}
		Action onChangedCurrentlyShown = _onChangedCurrentlyShown;
		if (onChangedCurrentlyShown != null)
		{
			onChangedCurrentlyShown();
		}
	}

	public void ActivateCharacterModel()
	{
		_currentlyShownModel = PlayerInfo.Instance.currentCharacter;
		_ActivateModel((Characters.CharacterType)_currentlyShownModel, ModelScreen.Character);
		Action onChangedCurrentlyShown = _onChangedCurrentlyShown;
		if (onChangedCurrentlyShown != null)
		{
			onChangedCurrentlyShown();
		}
	}

	public void ShowMenuModel(Characters.CharacterType charType)
	{
		_currentlyShownModel = (int)charType;
		_SwitchModel(charType);
		Action onChangedCurrentlyShown = _onChangedCurrentlyShown;
		if (onChangedCurrentlyShown != null)
		{
			onChangedCurrentlyShown();
		}
	}

	private void _ActivateModel(Characters.CharacterType characterName, ModelScreen screen)
	{
		if (_cachedActiveModel != null)
		{
			ClearModels();
		}
		switch (screen)
		{
		case ModelScreen.Character:
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(ModelPrefab) as GameObject;
			gameObject2.transform.parent = CharacterAnchor.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
			Utility.SetLayerRecursively(gameObject2.transform, CharacterAnchor.layer);
			gameObject2.transform.localScale = new Vector3(21f, 21f, 21f);
			gameObject2.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			CharacterModel component2 = gameObject2.GetComponent<CharacterModel>();
			component2.ChangeCharacterModel(characterName.ToString());
			component2.HideAllPowerups();
			component2.StartIdleAnimations();
			_cachedActiveModel = component2;
			break;
		}
		case ModelScreen.GameOver:
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ModelPrefab) as GameObject;
			gameObject.transform.parent = GameOverAnchor.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			Utility.SetLayerRecursively(gameObject.transform, GameOverAnchor.layer);
			gameObject.transform.localScale = new Vector3(18f, 18f, 18f);
			gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			CharacterModel component = gameObject.GetComponent<CharacterModel>();
			component.ChangeCharacterModel(characterName.ToString());
			component.HideAllPowerups();
			component.StartIdleAnimations();
			_cachedActiveModel = component;
			break;
		}
		}
	}

	private void _SwitchModel(Characters.CharacterType characterName)
	{
		if (_cachedActiveModel != null)
		{
			_cachedActiveModel.ChangeCharacterModel(characterName.ToString());
			_cachedActiveModel.HideAllPowerups();
			_cachedActiveModel.StartIdleAnimations();
		}
		else
		{
			_ActivateModel(characterName, ModelScreen.Character);
		}
	}

	public void ClearModels()
	{
		foreach (Transform item in CharacterAnchor.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in GameOverAnchor.transform)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		foreach (Transform item3 in TutorialPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item3.gameObject);
		}
	}

	public void ClearTutorialPopup()
	{
		foreach (Transform item in TutorialPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}
}
