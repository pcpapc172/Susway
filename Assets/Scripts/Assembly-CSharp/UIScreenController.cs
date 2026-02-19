using System;
using System.Collections;
using System.Collections.Generic;
using Extra;
using UnityEngine;

public class UIScreenController : MonoBehaviour
{
	public enum SlideInType
	{
		Mission = 0,
		MissionSet = 1,
		Letters = 2,
		Character = 3,
		LettersCompleteMysteryBox = 4,
		LettersCompleteCoins = 5,
		DoubleCoins = 6
	}

	private class SlideIn
	{
		public SlideInType type;

		public string payload = string.Empty;
	}

	private const float CHARTBOOST_DELAY_SECONDS = 0.1f;

	private static UIScreenController _instance;

	public AnimationCurve guidelineAnimation;

	public GameObject backgroundAnchor;

	public GameObject overlayAnchor;

	public GameObject popupAnchor;

	public GameObject superPopupAnchor;

	public Camera Camera3D;

	public GameObject MenuElements3D;

	public bool LoadMenuOnStart;

	public UIFont FloatingTextFont;

	private Camera mainCamera;

	public Action<string> OnChangedScreen;

	private static readonly List<string> PAYOUT_DISALLOWED_SCREENS = new List<string> { "IngameUI" };

	private static bool _facebookPayoutPopupQueued = false;

	private static bool _gameCenterPayoutPopupQueued = false;

	private bool _runningChartboostDelayCoroutine;

	private float _chartboostDelaySecondsLeft;

	private Dictionary<string, GameObject> _cachedScreens = new Dictionary<string, GameObject>();

	private Stack<string> _screenStack = new Stack<string>(20);

	private Queue<string> _popupQueue = new Queue<string>(15);

	private bool _popupActive;

	private bool _upgradesWasShown;

	private float _timescaleBeforeUpgradeUI = 1f;

	private List<string> _screenNamesWithoutBackground = new List<string> { "FrontUI", "IngameUI" };

	private List<string> _screenNamesWithOnlineVersion = new List<string> { "LeaderboardUI", "FriendsUI" };

	public bool stoppingFromEditor;

	[SerializeField]
	private UITexture backgroundTexture;

	[SerializeField]
	private UISlideInMissionHelper missionSlideIn;

	[SerializeField]
	private UISlideInMissionSetHelper missionSetSlideIn;

	[SerializeField]
	private UISlideInLettersHelper lettersSlideIn;

	[SerializeField]
	private UISlideInUnlock characterSlideIn;

	public CoinLabelSizer coinReward;

	private Queue<SlideIn> _slideInQueue = new Queue<SlideIn>(15);

	private bool slideInActive;

	[SerializeField]
	private AudioClipInfo slideInSound;

	[SerializeField]
	private AudioClipInfo slideInFanfare;

	public UIMessageHelper messageHelper;

	private Queue<string> _messageQueue = new Queue<string>();

	private bool messageShowing;

	public GameObject inAppPurchaseOverlay;

	public static bool isInstanced
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController;
			}
			return _instance != null;
		}
	}

	public static UIScreenController Instance
	{
		get
		{
			return _instance ?? (_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController);
		}
	}

	public bool isShowingPopup
	{
		get
		{
			return _popupQueue != null && _popupQueue.Count > 0;
		}
	}

	private void Awake()
	{
		Missions instance = Missions.Instance;
		instance.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Combine(instance.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionCompleted));
		Missions instance2 = Missions.Instance;
		instance2.onMissionSetComplete = (Missions.MissionSetCompleteHandler)Delegate.Combine(instance2.onMissionSetComplete, new Missions.MissionSetCompleteHandler(OnMissionSetCompleted));
		PlayerInfo instance3 = PlayerInfo.Instance;
		instance3.OnPickedUpLetter = (Action)Delegate.Combine(instance3.OnPickedUpLetter, new Action(OnLetterPickedUp));
		PlayerInfo instance4 = PlayerInfo.Instance;
		instance4.OnTokenCollected = (Action<Characters.CharacterType>)Delegate.Combine(instance4.OnTokenCollected, new Action<Characters.CharacterType>(OnTokenPickUp));
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void OnDestroy()
	{
		if (!stoppingFromEditor)
		{
			Missions instance = Missions.Instance;
			instance.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Remove(instance.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionCompleted));
			Missions instance2 = Missions.Instance;
			instance2.onMissionSetComplete = (Missions.MissionSetCompleteHandler)Delegate.Remove(instance2.onMissionSetComplete, new Missions.MissionSetCompleteHandler(OnMissionSetCompleted));
			PlayerInfo instance3 = PlayerInfo.Instance;
			instance3.OnPickedUpLetter = (Action)Delegate.Remove(instance3.OnPickedUpLetter, new Action(OnLetterPickedUp));
			PlayerInfo instance4 = PlayerInfo.Instance;
			instance4.OnTokenCollected = (Action<Characters.CharacterType>)Delegate.Remove(instance4.OnTokenCollected, new Action<Characters.CharacterType>(OnTokenPickUp));
		}
	}

	private void Start()
	{
		HideInAppPurchaseOverlay();
		if (LoadMenuOnStart)
		{
			ShowMainMenu();
		}
		PlayerInfo.Instance.BragCompleted();
		if (DeviceInfo.formFactor == DeviceInfo.FormFactor.iPad)
		{
			backgroundTexture.GetComponent<UIStretch>().relativeSize = Vector2.one;
		}
		else
		{
			backgroundTexture.GetComponent<UIStretch>().relativeSize = new Vector2(1.0667f, 1.2f);
		}
		MissionInfo[] missionInfo = Missions.Instance.GetMissionInfo();
		if (missionInfo[0].complete && missionInfo[1].complete && missionInfo[2].complete)
		{
			Missions.Instance.currentMissionSet++;
			Debug.LogWarning("you completed all missions but was not sent to next mission set, this should never happen, but this fixes it");
		}
		Wrapper.DumpGameObject("UIScreenController.Start ()", this);
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			if (_screenStack.Count > 0 && _screenStack.Peek() == "IngameUI")
			{
				PushScreen(null, "PauseUI");
			}
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	public void FacebookLogIn(bool loggedIn)
	{
		if (_screenStack.Count > 0)
		{
			if (_screenStack.Peek() == "FriendsUI_offline")
			{
				if (loggedIn)
				{
					_SwitchScreen("FriendsUI_online");
				}
			}
			else if (_screenStack.Peek() == "FriendsUI_online")
			{
				if (loggedIn)
				{
					_SwitchScreen("FriendsUI_online");
				}
			}
			else if (_screenStack.Peek() == "LeaderboardUI_online")
			{
				if (loggedIn)
				{
					_SwitchScreen("LeaderboardUI_online");
				}
			}
			else if (_screenStack.Peek() == "LeaderboardUI_offline")
			{
				if (loggedIn)
				{
					_SwitchScreen("LeaderboardUI_offline");
				}
			}
			else if (_screenStack.Peek() == "GameoverUI" && loggedIn)
			{
				_cachedScreens["GameoverUI"].GetComponent<UIGameOverHelper>().FacebookLoggedIn();
			}
		}
		if (loggedIn)
		{
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.FacebookLoggedIn);
		}
	}

	public void ShowMainMenu()
	{
		StartCoroutine(ShowMainMenuCoroutine());
	}

	private IEnumerator ShowMainMenuCoroutine()
	{
		while (Time.realtimeSinceStartup < LoadLevelCtrl.continueTime)
		{
			yield return null;
		}
		ThemeManager.Instance.ForceRefresh();
		_ActivateScreen("FrontUI");
	}

	public void GameOverTriggered()
	{
		PlayerInfo.Instance.RunCompleted();
		Missions.Instance.inRun = false;
		_ActivateScreen("GameoverUI");
	}

	public void QueueMessage(string message)
	{
		Debug.Log("Showing message: " + message);
		_QueueMessage(message);
	}

	public void GoToMainMenuFromGame(GameObject sender)
	{
		if (Game.Instance != null)
		{
			Missions.Instance.inRun = false;
			Game.Instance.StartTopMenu();
			Game.Instance.TriggerPause(false);
		}
		_ActivateScreen("FrontUI");
	}

	public string GetTopScreenName()
	{
		if (_screenStack != null && _screenStack.Count > 0)
		{
			return _screenStack.Peek();
		}
		return null;
	}

	public string GetCurrentPopupName()
	{
		if (_popupQueue != null && _popupQueue.Count > 0)
		{
			return _popupQueue.Peek();
		}
		return null;
	}

	public void PushScreen(GameObject sender)
	{
		PushScreen(sender, string.Empty);
	}

	public void PushScreen(GameObject sender, string screenOverride)
	{
		Debug.Log("PushScreen: ");
		string text = string.Empty;
		if (screenOverride != string.Empty)
		{
			text = screenOverride;
		}
		else
		{
			Debug.Log("sender is null: " + sender);
			if (sender.GetComponent<UIButtonChangeScreen>() != null)
			{
				text = sender.GetComponent<UIButtonChangeScreen>().ScreenNameToOpen;
			}
			if (sender.GetComponent<BackBtnBehaviourAndroid>() != null)
			{
				text = sender.GetComponent<BackBtnBehaviourAndroid>().ScreenNameToOpen;
			}
		}
		if (_screenNamesWithOnlineVersion.Contains(text))
		{
			text = ((!SocialManager.instance.consolidatedFriendsCompleted || (!Social.localUser.authenticated && !SocialManager.instance.facebookIsLoggedIn)) ? (text + "_offline") : (text + "_online"));
		}
		_ActivateScreen(text);
		if (text == "IngameUI")
		{
			_cachedScreens[text].GetComponent<UIIngameUpdater>().TriggerInGameUI();
			if (!UIIngameUpdater.isCountingDown())
			{
				Missions.Instance.inRun = true;
			}
		}
		else if (text == "PauseUI" && Game.Instance != null)
		{
			Game.Instance.TriggerPause(true);
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.StayInOneLane, (int)(Time.time - Character.Instance.sameLaneTimeStamp));
		}
	}

	public void SwitchScreen(GameObject sender)
	{
		string empty = string.Empty;
		empty = sender.GetComponent<UIButtonChangeScreen>().ScreenNameToOpen;
		if (_screenNamesWithOnlineVersion.Contains(empty))
		{
			empty = ((!SocialManager.instance.consolidatedFriendsCompleted || (!Social.localUser.authenticated && !SocialManager.instance.facebookIsLoggedIn)) ? (empty + "_offline") : (empty + "_online"));
		}
		_SwitchScreen(empty);
	}

	public void BackToPrevious()
	{
		_BackToPreviousScreen();
	}

	public static void QueueFacebookPayoutPopup()
	{
		_facebookPayoutPopupQueued = true;
		if (isInstanced)
		{
			Instance.TryQueuePayoutPopups();
		}
	}

	public static void QueueGameCenterPayoutPopup()
	{
		_gameCenterPayoutPopupQueued = true;
		if (isInstanced)
		{
			Instance.TryQueuePayoutPopups();
		}
	}

	private void TryQueuePayoutPopups()
	{
		if ((!_facebookPayoutPopupQueued && !_gameCenterPayoutPopupQueued) || _screenStack == null || _screenStack.Count <= 0)
		{
			return;
		}
		string item = _screenStack.Peek();
		if (!PAYOUT_DISALLOWED_SCREENS.Contains(item))
		{
			if (_facebookPayoutPopupQueued)
			{
				QueuePopup("FacebookPayoutPopup");
				_facebookPayoutPopupQueued = false;
			}
			if (_gameCenterPayoutPopupQueued)
			{
				QueuePopup("GameCenterPayoutPopup");
				_gameCenterPayoutPopupQueued = false;
			}
		}
		else
		{
			Debug.Log("Cannot show payout popup on this screen");
		}
	}

	public void QueuePopup(string popupName)
	{
		_QueuePopup(popupName);
	}

	public void QueuePopup(GameObject sender)
	{
		string screenNameToOpen = sender.GetComponent<UIButtonChangeScreen>().ScreenNameToOpen;
		_QueuePopup(screenNameToOpen);
	}

	public void QueueMysteryBox()
	{
		string text = string.Empty;
		if (_popupQueue.Count > 0)
		{
			text = _popupQueue.Peek();
			if (text == "MysteryBoxPopup")
			{
				return;
			}
			_RemovePopup();
		}
		_QueuePopup("MysteryBoxPopup");
		if (text != string.Empty)
		{
			_QueuePopup(text);
		}
	}

	public void ClosePopup(GameObject go = null)
	{
		Debug.Log("ClosePopup");
		_RemovePopup();
	}

	public void SpawnCollectText(Vector3 startPosition, string text)
	{
		UILabel uILabel = NGUITools.AddWidget<UILabel>(superPopupAnchor);
		uILabel.text = text;
		uILabel.transform.position = new Vector3(startPosition.x, startPosition.y, uILabel.cachedTransform.position.z);
		uILabel.font = FloatingTextFont;
		uILabel.color = new Color(50f / 51f, 66f / 85f, 0.23529412f, 0f);
		uILabel.cachedTransform.localScale = new Vector3(17f, 17f, 1f);
		StartCoroutine(AnimateCollectText(uILabel));
	}

	private IEnumerator AnimateCollectText(UILabel collectText)
	{
		Vector3 fromLocalPosition = collectText.transform.localPosition;
		Vector3 toLocalPosition = new Vector3(fromLocalPosition.x, fromLocalPosition.y + 50f, fromLocalPosition.z);
		yield return StartCoroutine(AnimateAlpha(collectText, 0.1f, 1f));
		StartCoroutine(MoveTransform(collectText.cachedTransform, 1f, toLocalPosition));
		yield return new WaitForSeconds(0.8f);
		StartCoroutine(AnimateAlpha(collectText, 0.2f, 0f));
		yield return new WaitForSeconds(0.25f);
		UnityEngine.Object.Destroy(collectText.gameObject);
	}

	private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
	}

	private IEnumerator MoveTransform(Transform trans, float duration, Vector3 toPos)
	{
		Vector3 fromPos = trans.localPosition;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			trans.localPosition = Vector3.Lerp(fromPos, toPos, factor);
			yield return null;
		}
	}

	private void AddBackButtonBehaviorForAndroid(string screenName)
	{
		Debug.Log("screenName : " + screenName);
		BackBtnBehaviourAndroid component = _cachedScreens[screenName].GetComponent<BackBtnBehaviourAndroid>();
		if (!component)
		{
			_cachedScreens[screenName].AddComponent<BackBtnBehaviourAndroid>();
			switch (screenName)
			{
			case "FrontUI":
				_cachedScreens[screenName].GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.ExitGame;
				break;
			case "FriendsUI_offline":
			case "FriendsUI_online":
			case "TokensUI":
			case "LeaderboardUI_offline":
			case "LeaderboardUI_online":
			case "CharacterScreen":
			case "UpgradesUI_shop":
			case "CoinsUI_shop":
			case "GameoverUI":
				_cachedScreens[screenName].GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.PushScreen;
				_cachedScreens[screenName].GetComponent<BackBtnBehaviourAndroid>().ScreenNameToOpen = "FrontUI";
				break;
			case "PauseUI":
				_cachedScreens[screenName].GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.PushScreen;
				_cachedScreens[screenName].GetComponent<BackBtnBehaviourAndroid>().ScreenNameToOpen = "IngameUI";
				break;
			case "IngameUI":
				_cachedScreens[screenName].GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.PushScreen;
				_cachedScreens[screenName].GetComponent<BackBtnBehaviourAndroid>().ScreenNameToOpen = "PauseUI";
				break;
			}
		}
	}

	private void _ActivateScreen(string screenName)
	{
		UIModelController.Instance.ClearModels();
		if (_cachedScreens.ContainsKey(screenName))
		{
			GameObject gameObject = _cachedScreens[screenName];
			if (_screenStack.Contains(screenName))
			{
				while (_screenStack.Contains(screenName) && _screenStack.Peek() != screenName)
				{
					string key = _screenStack.Pop();
					_cachedScreens[key].SetActiveRecursively(false);
				}
			}
			else
			{
				_cachedScreens[_screenStack.Peek()].SetActiveRecursively(false);
				_screenStack.Push(screenName);
			}
			gameObject.SetActiveRecursively(true);
			Wrapper.DumpGameObject(gameObject);
			gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			if (_screenStack.Count > 0)
			{
				_cachedScreens[_screenStack.Peek()].SetActiveRecursively(false);
			}
			_screenStack.Push(screenName);
			GameObject prefab = Resources.Load("Prefabs/Screens/" + screenName, typeof(GameObject)) as GameObject;
			GameObject gameObject2 = NGUITools.AddChild(overlayAnchor, prefab);
			_cachedScreens.Add(screenName, gameObject2);
			Wrapper.DumpGameObject(gameObject2);
			gameObject2.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		}
		AddBackButtonBehaviorForAndroid(screenName);
		switch (screenName)
		{
		case "GameoverUI":
			UIModelController.Instance.ActivateGameOverModel();
			_cachedScreens[screenName].GetComponent<UIGameOverHelper>().SetupBeforeMysteryBox();
			if (PlayerInfo.Instance.mysteryBoxesToUnlockCount > 0)
			{
				_QueuePopup("MysteryBoxPopup");
			}
			else
			{
				_cachedScreens[screenName].GetComponent<UIGameOverHelper>().SetupAfterMysteryBox();
			}
			break;
		case "FriendsUI_online":
		case "LeaderboardUI_online":
			Debug.Log("Getting an online screen!");
			SocialManager.instance.UpdateFriendScores(_cachedScreens[screenName].GetComponent<UISocialScreen>().ReloadFriends);
			_cachedScreens[screenName].GetComponent<UISocialScreen>().ReloadFriends();
			break;
		case "FriendsUI_offline":
		case "LeaderboardUI_offline":
			_cachedScreens[screenName].SendMessage("InitOfflineScreen", SendMessageOptions.DontRequireReceiver);
			break;
		}
		if (screenName == "UpgradesUI_shop")
		{
			_upgradesWasShown = true;
			_timescaleBeforeUpgradeUI = Time.timeScale;
			Time.timeScale = 0f;
		}
		else if (_upgradesWasShown)
		{
			_upgradesWasShown = false;
			Time.timeScale = _timescaleBeforeUpgradeUI;
		}
		SetBackground(!_screenNamesWithoutBackground.Contains(screenName));
		Action<string> onChangedScreen = OnChangedScreen;
		if (onChangedScreen != null)
		{
			onChangedScreen(screenName);
		}
		Wrapper.DumpGameObject(this.gameObject);
		ScreenDidChange(screenName);
	}

	private void _SwitchScreen(string screenName)
	{
		string key = _screenStack.Pop();
		_cachedScreens[key].SetActiveRecursively(false);
		_ActivateScreen(screenName);
	}

	private void _BackToPreviousScreen()
	{
		if (_screenStack.Count > 1)
		{
			string key = _screenStack.Pop();
			_cachedScreens[key].SetActiveRecursively(false);
			key = _screenStack.Peek();
			_cachedScreens[key].SetActiveRecursively(true);
			SetBackground(!_screenNamesWithoutBackground.Contains(key));
			ScreenDidChange(key);
		}
		else
		{
			Debug.LogError("Tried to remove the only screen in the stack. You dun goofed.", this);
		}
		Wrapper.DumpGameObject("UIScreenController._BackToPreviousScreen ()", this);
	}

	private void ScreenDidChange(string newScreenName)
	{
		messageHelper.SetTemporaryHidden(newScreenName != "IngameUI");
		Flurry.LogEvent("UI Screen " + newScreenName);
		InvokeChartboostDelayed(0.1f);
		TryQueuePayoutPopups();
		if (newScreenName == "FrontUI" || newScreenName == "GameoverUI")
		{
			HouseKeeper.RefreshOnlineSettingsAndInappsIfNeeded();
		}
		if (newScreenName == "FrontUI" && _cachedScreens[GetTopScreenName()].GetComponent<FrontScreen>().buttonsHaveTweened && PlayerInfo.Instance.shouldShowMission2Popup && !PlayerInfo.Instance.hasShownMission2Popup)
		{
			ShowUpdateAppPopUp();
			PlayerInfo.Instance.shouldShowMission2Popup = false;
			PlayerInfo.Instance.hasShownMission2Popup = true;
			_QueuePopup("TutorialEndGameMissionsPopup");
		}
	}

	public void ShowUpdateAppPopUp()
	{
		UpdateApp.ShowIfNeeded();
	}

	private void InvokeChartboostDelayed(float delay)
	{
		_chartboostDelaySecondsLeft = delay;
		if (!_runningChartboostDelayCoroutine)
		{
			StartCoroutine(DelayedChartboostNotifyCoroutine());
		}
	}

	private IEnumerator DelayedChartboostNotifyCoroutine()
	{
		if (!_runningChartboostDelayCoroutine)
		{
			_runningChartboostDelayCoroutine = true;
			float lastRealTime = Time.realtimeSinceStartup;
			while (_chartboostDelaySecondsLeft > 0f)
			{
				float realTime = Time.realtimeSinceStartup;
				float deltaRealTime = realTime - lastRealTime;
				lastRealTime = realTime;
				_chartboostDelaySecondsLeft -= deltaRealTime;
				yield return null;
			}
			string screenName = GetTopScreenName();
			if (!string.IsNullOrEmpty(screenName))
			{
				ChartBoostManager.instance.GameScreenChanged(screenName);
			}
			_runningChartboostDelayCoroutine = false;
		}
	}

	private void _QueuePopup(string name)
	{
		if (!name.Equals("CoinsUI_quick"))
		{
			_popupQueue.Enqueue(name);
			if (!_popupActive)
			{
				_ActivateNextPopup();
			}
		}
	}

	private void _ActivateNextPopup()
	{
		if (_popupQueue.Count > 0)
		{
			_PauseAnimations(true, MenuElements3D.transform);
			NGUITools.SetActive(MenuElements3D, false);
			string text = _popupQueue.Peek();
			if (!_cachedScreens.ContainsKey(text))
			{
				GameObject prefab = Resources.Load("Prefabs/Popups/" + text, typeof(GameObject)) as GameObject;
				GameObject value = NGUITools.AddChild(popupAnchor, prefab);
				_cachedScreens.Add(text, value);
			}
			_cachedScreens[text].SetActiveRecursively(true);
			Wrapper.DumpGameObject("UIScreenController.__ActivateNextPopup ()", _cachedScreens[text]);
			if (text == "MysteryBoxPopup")
			{
				mainCamera.enabled = false;
				_cachedScreens[text].GetComponent<MysteryBoxHandler>().SetupMysteryBoxScreen();
			}
			else if (text == "BragPopup")
			{
				_cachedScreens[text].GetComponent<BragPopupHandler>().SetupBragPopup();
			}
			_cachedScreens[text].BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
			_popupActive = true;
			if (text == "UpgradesUI_quick")
			{
				_upgradesWasShown = true;
				_timescaleBeforeUpgradeUI = Time.timeScale;
				Time.timeScale = 0f;
			}
			else if (_upgradesWasShown)
			{
				_upgradesWasShown = false;
				Time.timeScale = _timescaleBeforeUpgradeUI;
			}
			BackBtnBehaviourAndroid component = _cachedScreens[text].GetComponent<BackBtnBehaviourAndroid>();
			if (component == null && text != "MysteryBoxPopup")
			{
				_cachedScreens[text].AddComponent<BackBtnBehaviourAndroid>();
				_cachedScreens[text].GetComponent<BackBtnBehaviourAndroid>().screenChangeType = BackBtnBehaviourAndroid.ScreenChangeType.ClosePopup;
			}
			Action<string> onChangedScreen = OnChangedScreen;
			if (onChangedScreen != null)
			{
				onChangedScreen(text);
			}
		}
		else
		{
			NGUITools.SetActive(MenuElements3D, true);
			_PauseAnimations(false, MenuElements3D.transform);
			if (_upgradesWasShown)
			{
				_upgradesWasShown = false;
				Time.timeScale = _timescaleBeforeUpgradeUI;
			}
		}
	}

	public void PauseAnimations(bool pause, Transform trans)
	{
		Debug.LogWarning("This method is not supposed to be used outside of testing.");
		_PauseAnimations(pause, trans);
	}

	private void _PauseAnimations(bool pause, Transform trans)
	{
		foreach (Transform tran in trans)
		{
			_PauseAnimations(pause, tran);
		}
		if (trans.GetComponent<CharacterModel>() != null)
		{
			if (pause)
			{
				trans.GetComponent<CharacterModel>().StopIdleAnimations();
			}
			else
			{
				trans.GetComponent<CharacterModel>().StartIdleAnimations();
			}
		}
	}

	private void _RemovePopup()
	{
		if (_popupQueue.Count < 1)
		{
			return;
		}
		string text = _popupQueue.Dequeue();
		_cachedScreens[text].SetActiveRecursively(false);
		_popupActive = false;
		if (_popupQueue.Count == 0)
		{
			Action<string> onChangedScreen = OnChangedScreen;
			if (onChangedScreen != null)
			{
				onChangedScreen(GetTopScreenName());
			}
		}
		_ActivateNextPopup();
		if (text == "MysteryBoxPopup")
		{
			if (mainCamera != null)
			{
				mainCamera.enabled = true;
			}
			if (_screenStack.Peek() == "GameoverUI")
			{
				_cachedScreens[_screenStack.Peek()].GetComponent<UIGameOverHelper>().SetupAfterMysteryBox();
			}
		}
		if (_popupQueue.Count == 0)
		{
			ChartBoostManager.instance.LastQueuedPopupsClosed(GetTopScreenName());
		}
	}

	private void SetBackground(bool state)
	{
		string text = "NotebookPanel";
		if (state)
		{
			if (!_cachedScreens.ContainsKey(text))
			{
				GameObject prefab = Resources.Load("Prefabs/Screens/" + text, typeof(GameObject)) as GameObject;
				GameObject value = NGUITools.AddChild(backgroundAnchor, prefab);
				_cachedScreens.Add(text, value);
			}
			mainCamera.enabled = false;
			backgroundTexture.enabled = true;
			backgroundTexture.SendMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
			_cachedScreens[text].SetActiveRecursively(true);
			_cachedScreens[text].BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			if (_cachedScreens.ContainsKey(text))
			{
				_cachedScreens[text].SetActiveRecursively(false);
			}
			backgroundTexture.enabled = false;
			mainCamera.enabled = true;
		}
		Wrapper.DumpGameObject("UIScreenController.SetBackground ()", this);
	}

	private void OnMissionCompleted(string message)
	{
		QueueSlideIn(SlideInType.Mission, message);
	}

	private void OnMissionSetCompleted()
	{
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.ReachMissionSet);
		QueueSlideIn(SlideInType.MissionSet, string.Empty);
	}

	private void OnLetterPickedUp()
	{
		QueueSlideIn(SlideInType.Letters, string.Empty);
	}

	private void OnTokenPickUp(Characters.CharacterType type)
	{
		if (Characters.characterData[type].Price <= PlayerInfo.Instance.GetCollectedTokens(type))
		{
			QueueSlideIn(SlideInType.Character, Characters.characterData[type].modelName);
			Flurry.LogEventWithAParameter("Character unlocked", "Id", Characters.characterData[type].modelName.ToLower());
		}
	}

	public void QueueSlideIn(SlideInType type, string payload = "")
	{
		SlideIn slideIn = new SlideIn();
		slideIn.type = type;
		slideIn.payload = payload;
		_slideInQueue.Enqueue(slideIn);
		if (!slideInActive)
		{
			_ShowSlideIn();
		}
	}

	public void ReadyForNextSlide()
	{
		slideInActive = false;
		if (!slideInActive)
		{
			_ShowSlideIn();
		}
	}

	private void _ShowSlideIn()
	{
		if (_slideInQueue.Count > 0)
		{
			SlideIn slideIn = _slideInQueue.Dequeue();
			if (slideIn.type == SlideInType.Mission)
			{
				So.Instance.playSound(slideInFanfare);
				missionSlideIn.SetupSlideInMission(slideIn.payload);
			}
			else if (slideIn.type == SlideInType.MissionSet)
			{
				So.Instance.playSound(slideInFanfare);
				missionSetSlideIn.SetupSlideInMissionSet(PlayerInfo.Instance.rawMultiplier);
			}
			else if (slideIn.type == SlideInType.Letters)
			{
				So.Instance.playSound(slideInSound);
				lettersSlideIn.SetupLetters();
			}
			else if (slideIn.type == SlideInType.Character)
			{
				So.Instance.playSound(slideInSound);
				characterSlideIn.SetupSlideInUnlock(slideIn.payload);
			}
			else if (slideIn.type == SlideInType.LettersCompleteMysteryBox)
			{
				So.Instance.playSound(slideInFanfare);
				missionSetSlideIn.SetupMysteryBox();
			}
			else if (slideIn.type == SlideInType.LettersCompleteCoins)
			{
				So.Instance.playSound(slideInFanfare);
				missionSetSlideIn.SetupCoin();
			}
			else if (slideIn.type == SlideInType.DoubleCoins)
			{
				So.Instance.playSound(slideInFanfare);
				characterSlideIn.SetupSlideInUnlock("Double Coins");
			}
			slideInActive = true;
		}
	}

	public void ReadyForNextMessage()
	{
		messageShowing = false;
		_ShowNextMessage();
	}

	private void _QueueMessage(string message)
	{
		_messageQueue.Enqueue(message);
		if (!messageShowing)
		{
			_ShowNextMessage();
		}
		if (!slideInActive)
		{
			_ShowSlideIn();
		}
	}

	private void _ShowNextMessage()
	{
		if (_messageQueue.Count > 0)
		{
			string message = _messageQueue.Dequeue();
			messageHelper.ShowMessage(message);
			messageShowing = true;
		}
	}

	public void ShowInAppPurchaseOverlay()
	{
		inAppPurchaseOverlay.SetActiveRecursively(true);
		Camera3D.enabled = false;
	}

	public void HideInAppPurchaseOverlay()
	{
		inAppPurchaseOverlay.SetActiveRecursively(false);
		Camera3D.enabled = true;
	}

	public bool IsInAppPurchaseOverlayVisible()
	{
		return inAppPurchaseOverlay.active;
	}

	public bool isPopupQueueEmpty()
	{
		return _popupQueue.Count <= 0;
	}
}
