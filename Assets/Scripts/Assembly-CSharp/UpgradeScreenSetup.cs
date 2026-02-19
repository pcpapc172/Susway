using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreenSetup : MonoBehaviour
{
	public GameObject WatchVideoPrefab;

	public GameObject DoubleCoinsPrefab;

	public GameObject ConsumablePrefab;

	public GameObject PermanentPrefab;

	public UIFont headerFont;

	private UITable _table;

	private UIDraggablePanel _parentDragPanel;

	private GameObject[] skipMissions = new GameObject[3];

	private string[] skipMissionNames = new string[3];

	public List<UpgradeHelper> cachedUpgradeHelpers = new List<UpgradeHelper>(11);

	private bool _hasStartedToFillTable;

	private bool _hasFilledTableCompletely;

	[NonSerialized]
	public PowerupType[] powerupSingleUse = new PowerupType[4]
	{
		PowerupType.hoverboard,
		PowerupType.mysterybox,
		PowerupType.headstart500,
		PowerupType.headstart2000
	};

	[NonSerialized]
	public PowerupType[] powerupSkipMission = new PowerupType[3]
	{
		PowerupType.skipmission1,
		PowerupType.skipmission2,
		PowerupType.skipmission3
	};

	[NonSerialized]
	public PowerupType[] powerupPermanent = new PowerupType[4]
	{
		PowerupType.jetpack,
		PowerupType.supersneakers,
		PowerupType.coinmagnet,
		PowerupType.doubleMultiplier
	};

	private int numberOfObjects;

	private GameObject go;

	private void Awake()
	{
		_table = GetComponent<UITable>();
		_parentDragPanel = NGUITools.FindInParents<UIDraggablePanel>(base.transform.parent.gameObject);
		Missions instance = Missions.Instance;
		instance.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Combine(instance.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionComplete));
		Missions instance2 = Missions.Instance;
		instance2.onMissionSetComplete = (Missions.MissionSetCompleteHandler)Delegate.Combine(instance2.onMissionSetComplete, new Missions.MissionSetCompleteHandler(OnMissionSetComplete));
	}

	private void OnDestroy()
	{
		if (!UIScreenController.Instance.stoppingFromEditor)
		{
			Missions instance = Missions.Instance;
			instance.onMissionComplete = (Missions.MissionCompleteHandler)Delegate.Remove(instance.onMissionComplete, new Missions.MissionCompleteHandler(OnMissionComplete));
			Missions instance2 = Missions.Instance;
			instance2.onMissionSetComplete = (Missions.MissionSetCompleteHandler)Delegate.Remove(instance2.onMissionSetComplete, new Missions.MissionSetCompleteHandler(OnMissionSetComplete));
		}
	}

	private void OnEnable()
	{
		_table.repositionNow = true;
		if (_hasStartedToFillTable && !_hasFilledTableCompletely)
		{
			FillRemainingTableNow();
		}
	}

	private void Start()
	{
		numberOfObjects = 0;
		FillTopTable();
		VideoAdsManager instance = VideoAdsManager.instance;
		if (!instance.isInitialized)
		{
			instance.Init();
		}
	}

	private void FillTopTable()
	{
		_hasStartedToFillTable = true;
		UILabel uILabel = NGUITools.AddWidget<UILabel>(base.gameObject);
		uILabel.font = headerFont;
		uILabel.text = "Single Use";
		uILabel.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
		uILabel.name = string.Format("{0:000}", numberOfObjects);
		uILabel.supportEncoding = false;
		uILabel.multiLine = false;
		uILabel.MakePixelPerfect();
		if (DeviceInfo.isHighres)
		{
			uILabel.gameObject.transform.localScale = new Vector3(uILabel.gameObject.transform.localScale.x / 2f, uILabel.gameObject.transform.localScale.y / 2f, uILabel.gameObject.transform.localScale.z);
		}
		numberOfObjects++;
		PowerupType[] array = powerupSingleUse;
		foreach (PowerupType powerupType in array)
		{
			MakeBuyable(powerupType, false);
			numberOfObjects++;
		}
		base.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		StartCoroutine(FillRemaingTableDelayed());
	}

	private IEnumerator FillRemaingTableDelayed()
	{
		yield return null;
		FillRemainingTableNow();
		_table.Reposition();
		base.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
	}

	private GameObject MakeBuyable(PowerupType powerupType, bool permanent)
	{
		if (permanent)
		{
			go = NGUITools.AddChild(base.gameObject, PermanentPrefab);
			go.GetComponent<UpgradeHelper>().InitPermanent(powerupType);
		}
		else
		{
			go = NGUITools.AddChild(base.gameObject, ConsumablePrefab);
			go.GetComponent<UpgradeHelper>().InitSingle(powerupType);
		}
		go.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
		go.name = string.Format("{0:000}", numberOfObjects);
		NGUITools.AddWidgetCollider(go);
		cachedUpgradeHelpers.Add(go.GetComponent<UpgradeHelper>());
		return go;
	}

	private void FillRemainingTableNow()
	{
		for (int i = 0; i < powerupSkipMission.Length; i++)
		{
			string text = string.Format("{0:000}", numberOfObjects);
			skipMissionNames[i] = text;
			if (!Missions.Instance.GetMissionInfo(i).complete)
			{
				skipMissions[i] = MakeBuyable(powerupSkipMission[i], false);
			}
			numberOfObjects++;
		}
		UILabel uILabel = NGUITools.AddWidget<UILabel>(base.gameObject);
		uILabel.font = headerFont;
		uILabel.text = "Upgrades";
		uILabel.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
		uILabel.name = string.Format("{0:000}", numberOfObjects);
		uILabel.supportEncoding = false;
		uILabel.multiLine = false;
		uILabel.MakePixelPerfect();
		if (DeviceInfo.isHighres)
		{
			uILabel.gameObject.transform.localScale = new Vector3(uILabel.gameObject.transform.localScale.x / 2f, uILabel.gameObject.transform.localScale.y / 2f, uILabel.gameObject.transform.localScale.z);
		}
		numberOfObjects++;
		PowerupType[] array = powerupPermanent;
		foreach (PowerupType powerupType in array)
		{
			MakeBuyable(powerupType, true);
			numberOfObjects++;
		}
		_hasFilledTableCompletely = true;
	}

	private void OnMissionComplete(string payload)
	{
		for (int i = 0; i < skipMissions.Length; i++)
		{
			if (skipMissions[i] != null && Missions.Instance.GetMissionInfo(i).complete)
			{
				cachedUpgradeHelpers.Remove(skipMissions[i].GetComponent<UpgradeHelper>());
				NGUITools.SetActive(skipMissions[i], false);
				UnityEngine.Object.Destroy(skipMissions[i]);
			}
		}
		_table.repositionNow = true;
	}

	private void OnMissionSetComplete()
	{
		for (int i = 0; i < skipMissions.Length; i++)
		{
			if (skipMissions[i] != null)
			{
				cachedUpgradeHelpers.Remove(skipMissions[i].GetComponent<UpgradeHelper>());
				UnityEngine.Object.Destroy(skipMissions[i]);
			}
		}
		bool active = base.gameObject.active;
		for (int j = 0; j < powerupSkipMission.Length; j++)
		{
			if (!Missions.Instance.GetMissionInfo(j).complete)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject, ConsumablePrefab);
				gameObject.name = skipMissionNames[j];
				gameObject.GetComponent<UpgradeHelper>().InitSingle(powerupSkipMission[j]);
				gameObject.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
				NGUITools.AddWidgetCollider(gameObject);
				skipMissions[j] = gameObject;
				NGUITools.SetActive(gameObject, active);
				cachedUpgradeHelpers.Add(gameObject.GetComponent<UpgradeHelper>());
			}
		}
		if (base.gameObject.active)
		{
			_table.repositionNow = true;
		}
	}

	private IEnumerator SetStatic()
	{
		yield return null;
		base.transform.parent.GetComponent<UIPanel>().widgetsAreStatic = true;
	}
}
