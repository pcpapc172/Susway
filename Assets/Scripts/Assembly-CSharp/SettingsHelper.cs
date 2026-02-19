using UnityEngine;

public class SettingsHelper : MonoBehaviour
{
	private const int SCALEINCREASE = 75;

	[SerializeField]
	private Transform fillet;

	[SerializeField]
	private Transform outline;

	[SerializeField]
	private Transform grid;

	[SerializeField]
	private Transform versionNr;

	[SerializeField]
	private GameObject seasonButton;

	[SerializeField]
	private GameObject gameCenterButton;

	private GameObject seasonButtonInstance;

	private int originalFilletScale;

	private int originalOutlineScale;

	private int originalVersionNrHeight;

	private void Awake()
	{
		originalFilletScale = (int)fillet.localScale.y;
		originalOutlineScale = (int)outline.localScale.y;
		originalVersionNrHeight = (int)versionNr.localPosition.y;
	}

	private void OnEnable()
	{
		if (PlayerInfo.Instance.currentSeasonAvailable == PlayerInfo.Season.halloween)
		{
			if (seasonButtonInstance == null)
			{
				seasonButtonInstance = NGUITools.AddChild(grid.gameObject, seasonButton);
				seasonButtonInstance.name = "3 Season";
			}
			if (gameCenterButton != null)
			{
				fillet.localScale = new Vector3(fillet.localScale.x, originalFilletScale, 1f);
				outline.localScale = new Vector3(outline.localScale.x, originalOutlineScale, 1f);
				versionNr.localPosition = new Vector3(versionNr.localPosition.x, originalVersionNrHeight, -1f);
				NGUITools.Destroy(gameCenterButton);
			}
		}
		else if (gameCenterButton != null)
		{
			fillet.localScale = new Vector3(fillet.localScale.x, originalFilletScale - 75, 1f);
			outline.localScale = new Vector3(outline.localScale.x, originalOutlineScale - 75, 1f);
			versionNr.localPosition = new Vector3(versionNr.localPosition.x, originalVersionNrHeight + 75, -1f);
			NGUITools.Destroy(gameCenterButton);
		}
		grid.GetComponent<UIGrid>().repositionNow = true;
	}
}
