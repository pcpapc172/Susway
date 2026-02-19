using System;
using System.Collections.Generic;
using UnityEngine;

public class ThemeAssets : MonoBehaviour
{
	[Serializable]
	public class TextureMaterialPair
	{
		public Texture2D texture;

		public Material material;
	}

	[Serializable]
	public class Assets
	{
		public string theme;

		public GameObject[] environmentModels;

		public GameObject[] characterModels;

		public TextureMaterialPair[] textureAndMaterials;

		public AudioClip[] sounds;

		public Color fogColor;

		public Dictionary<Material, Texture> textureAndMaterialLinks = new Dictionary<Material, Texture>();

		public void CreateMaterialsAndTextureLink()
		{
			TextureMaterialPair[] array = textureAndMaterials;
			foreach (TextureMaterialPair textureMaterialPair in array)
			{
				textureAndMaterialLinks.Add(textureMaterialPair.material, textureMaterialPair.texture);
			}
		}
	}

	private static ThemeAssets instance;

	public Assets[] assets;

	public Material[] dynamicChangingMaterials;

	public HashSet<Material> dynamicChangingMaterialsSet = new HashSet<Material>();

	[HideInInspector]
	public Dictionary<string, Mesh> environmentModelMeshes = new Dictionary<string, Mesh>();

	[HideInInspector]
	public Dictionary<string, Mesh> characterModelMeshes = new Dictionary<string, Mesh>();

	[HideInInspector]
	public Dictionary<Material, Material> original2CloneMaterials = new Dictionary<Material, Material>();

	private Dictionary<Theme, Assets> theme2assets;

	private HashSet<Texture> allTextures;

	public static ThemeAssets Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(ThemeAssets)) as ThemeAssets;
				if (instance == null)
				{
					Debug.LogError("Could not find ThemeAssets instance.");
				}
			}
			return instance;
		}
	}

	private void GetMeshes()
	{
		Assets[] array = this.assets;
		foreach (Assets assets in array)
		{
			GameObject[] environmentModels = assets.environmentModels;
			foreach (GameObject gameObject in environmentModels)
			{
				MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>(true);
				MeshFilter[] array2 = componentsInChildren;
				foreach (MeshFilter meshFilter in array2)
				{
					if (environmentModelMeshes.ContainsKey(meshFilter.sharedMesh.name))
					{
						Debug.Log(meshFilter.sharedMesh.name + " allready exits");
					}
					else
					{
						environmentModelMeshes.Add(meshFilter.sharedMesh.name, meshFilter.sharedMesh);
					}
				}
			}
			GameObject[] characterModels = assets.characterModels;
			foreach (GameObject gameObject2 in characterModels)
			{
				SkinnedMeshRenderer[] componentsInChildren2 = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				SkinnedMeshRenderer[] array3 = componentsInChildren2;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array3)
				{
					characterModelMeshes.Add(skinnedMeshRenderer.sharedMesh.name, skinnedMeshRenderer.sharedMesh);
				}
			}
		}
	}

	public Material ReplaceMaterial(Material originalMaterial)
	{
		Material result = null;
		Material value;
		if (dynamicChangingMaterialsSet.Contains(originalMaterial))
		{
			result = RegisterDynmaicMaterial(originalMaterial);
		}
		else if (original2CloneMaterials.TryGetValue(originalMaterial, out value))
		{
			result = value;
		}
		return result;
	}

	private void Awake()
	{
		theme2assets = new Dictionary<Theme, Assets>();
		allTextures = new HashSet<Texture>();
		Material[] array = dynamicChangingMaterials;
		foreach (Material item in array)
		{
			dynamicChangingMaterialsSet.Add(item);
		}
		Assets[] array2 = this.assets;
		foreach (Assets assets in array2)
		{
			theme2assets.Add(Theme.FindByName(assets.theme), assets);
			assets.CreateMaterialsAndTextureLink();
			TextureMaterialPair[] textureAndMaterials = assets.textureAndMaterials;
			foreach (TextureMaterialPair textureMaterialPair in textureAndMaterials)
			{
				if (!allTextures.Contains(textureMaterialPair.texture))
				{
					allTextures.Add(textureMaterialPair.texture);
				}
				if (!original2CloneMaterials.ContainsKey(textureMaterialPair.material))
				{
					Material value = new Material(textureMaterialPair.material);
					original2CloneMaterials.Add(textureMaterialPair.material, value);
				}
			}
		}
		GetMeshes();
	}

	public Material RegisterDynmaicMaterial(Material sharedMaterial)
	{
		Material material = null;
		Material material2 = new Material(sharedMaterial);
		material2.name = "replaced_" + sharedMaterial.name;
		Assets[] array = this.assets;
		foreach (Assets assets in array)
		{
			TextureMaterialPair[] textureAndMaterials = assets.textureAndMaterials;
			foreach (TextureMaterialPair textureMaterialPair in textureAndMaterials)
			{
				if (textureMaterialPair.material == sharedMaterial)
				{
					assets.textureAndMaterialLinks.Add(material2, textureMaterialPair.texture);
					if (!original2CloneMaterials.ContainsKey(material2))
					{
						material = new Material(material2);
						original2CloneMaterials.Add(material2, material);
					}
				}
			}
		}
		return material;
	}

	private void Start()
	{
		ThemeManager.Instance.OnChangeTheme += OnChangeTheme;
	}

	private void OnChangeTheme(Theme theme)
	{
		if (theme == null)
		{
			Debug.LogWarning("cannot change theme to null");
			return;
		}
		Assets assets = theme2assets[theme];
		foreach (KeyValuePair<Material, Material> original2CloneMaterial in original2CloneMaterials)
		{
			Texture value;
			if (assets.textureAndMaterialLinks.TryGetValue(original2CloneMaterial.Key, out value))
			{
				original2CloneMaterial.Value.SetTexture("_MainTex", value);
			}
		}
		RenderSettings.fogColor = assets.fogColor;
		Resources.UnloadUnusedAssets();
	}
}
