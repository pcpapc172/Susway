using System;
using System.Collections;
using UnityEngine;

public class CoinScreenSetup : MonoBehaviour
{
	public GameObject coinPrefab;

	public GameObject coinEarnerPrefab;

	public UIFont headerFont;

	private UITable _table;

	private UIDraggablePanel _parentDragPanel;

	private bool firstRun = true;

	[NonSerialized]
	public int counterForElementsOnDisplay;

	private GameObject go;

	private void Awake()
	{
		_table = GetComponent<UITable>();
		_parentDragPanel = NGUITools.FindInParents<UIDraggablePanel>(base.transform.parent.gameObject);
	}

	private void Start()
	{
		FillTable();
	}

	public void RefreshCurrencyEarners()
	{
		FillTable();
	}

	private void FillTable()
	{
		counterForElementsOnDisplay = 0;
		base.transform.parent.GetComponent<UIPanel>().widgetsAreStatic = false;
		foreach (Transform item in base.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		int num = 0;
		GameObject gameObject = NGUITools.AddChild(base.gameObject);
		gameObject.name = string.Format("{0:000}", num);
		UILabel uILabel = NGUITools.AddWidget<UILabel>(base.gameObject);
		uILabel.cachedTransform.parent = gameObject.transform;
		uILabel.font = headerFont;
		uILabel.text = "Coin Shop";
		uILabel.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
		uILabel.MakePixelPerfect();
		if (DeviceInfo.isHighres)
		{
			uILabel.gameObject.transform.localScale = new Vector3(uILabel.gameObject.transform.localScale.x / 2f, uILabel.gameObject.transform.localScale.y / 2f, uILabel.gameObject.transform.localScale.z);
		}
		num++;
		for (int i = 0; i < InAppData.inAppTiersAndInAppTiersDiscount.Length / 2; i++)
		{
			go = NGUITools.AddChild(base.gameObject, coinPrefab);
			go.name = string.Format("{0:000}", num);
			go.GetComponent<CoinButtonHelper>().Init(i);
			go.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
			NGUITools.AddWidgetCollider(go);
			num++;
		}
		bool flag = false;
		for (int j = 1; j < EarnCurrencyInfo.profiles.Length; j++)
		{
			if (EarnCurrencyInfo.ShouldShowInGUI(j))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			UILabel uILabel2 = NGUITools.AddWidget<UILabel>(base.gameObject);
			uILabel2.font = headerFont;
			uILabel2.text = "Earn Coins";
			uILabel2.color = new Color(0f, 0.2901961f, 0.5019608f, 1f);
			uILabel2.name = string.Format("{0:000}", num);
			uILabel2.MakePixelPerfect();
			if (DeviceInfo.isHighres)
			{
				uILabel2.gameObject.transform.localScale = new Vector3(uILabel2.gameObject.transform.localScale.x / 2f, uILabel2.gameObject.transform.localScale.y / 2f, uILabel2.gameObject.transform.localScale.z);
			}
			num++;
			for (int k = 1; k < EarnCurrencyInfo.profiles.Length; k++)
			{
				if (EarnCurrencyInfo.ShouldShowInGUI(k))
				{
					EarnCurrencyInfo.EarnCurrencyProfile earnCurrencyProfile = EarnCurrencyInfo.profiles[k];
					go = NGUITools.AddChild(base.gameObject, coinEarnerPrefab);
					go.name = string.Format("{0:000}", num);
					string desc = string.Format(earnCurrencyProfile.desc, earnCurrencyProfile.GetAmountOfCoins());
					go.GetComponent<CoinEarnerButtonHelper>().Init(k, earnCurrencyProfile.title, desc, earnCurrencyProfile.iconName);
					go.GetComponent<UIDragPanelContents>().draggablePanel = _parentDragPanel;
					NGUITools.AddWidgetCollider(go);
					num++;
				}
			}
		}
		base.gameObject.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
		if (base.gameObject.active)
		{
			_table.Reposition();
			if (!firstRun)
			{
				_parentDragPanel.RestrictWithinBounds(true);
			}
			else
			{
				firstRun = false;
			}
		}
	}

	private IEnumerator SetStatic()
	{
		yield return null;
		base.transform.parent.GetComponent<UIPanel>().widgetsAreStatic = true;
	}
}
