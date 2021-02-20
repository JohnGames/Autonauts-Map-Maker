using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectBuilder : MonoBehaviour
{

	public static ObjectBuilder current;

	public UnityEvent loadComplete;

	public GameObject tallGrass;
	public GameObject[] animals;
	public GameObject[] standardGrassObjects;
	public GameObject[] treespots;
	public GameObject[] metalDeposits;
	public GameObject[] wheat;
	public GameObject[] flowers;

	void LoadEvent()
	{

		//SaveFile.current.stackedObjectsArray = new List<GameObject>[SaveFile.current.Tiles.TilesWide, SaveFile.current.Tiles.TilesHigh];

		foreach (var item in gameObject.GetComponentsInChildren<Transform>())
		{
			if (item.transform != transform)
			{
				GameObject.Destroy(item.gameObject);
			}
		}

		//Ok, all cleared. Now Build, BUILD!

		//Why not make the fence array now, yes? Yes!

		SaveFile.current.fenceArray = new bool[SaveFile.current.Tiles.TilesWide, SaveFile.current.Tiles.TilesHigh];

		var numberOfUnknownItems = new Dictionary<string, int>();

		foreach (var item in SaveFile.current.Objects)
		{

			GameObject newObject;
			if (Resources.Load(item.ID))
			{
				newObject = Instantiate(Resources.Load(item.ID), transform) as GameObject;
			}
			else
			{
				if (numberOfUnknownItems.ContainsKey(item.ID))
				{
					int count;
					numberOfUnknownItems.TryGetValue(item.ID, out count);

					count++;

					numberOfUnknownItems.Remove(item.ID);
					numberOfUnknownItems.Add(item.ID, count);
				}
				else
				{
					numberOfUnknownItems.Add(item.ID, 1);
				}
				newObject = Instantiate(Resources.Load("Unknown"), transform) as GameObject;
			}

				newObject.GetComponent<mapObj>().storedType = item;


			

			//List<GameObject> stackedObjectList = SaveFile.current[item.TX, item.TY];

			//stackedObjectList.Add(newObject);
			//int newObjectHeight = stackedObjectList.Count;
			//newObjectHeight--;


			//newObject.transform.position = new Vector3(item.TX,0.5f*newObjectHeight,-item.TY);

			newObject.transform.position = new Vector3(item.TX, 0f, -item.TY);




		}

		foreach (KeyValuePair<string, int> pair in numberOfUnknownItems)
		{
			Debug.Log(pair.Key + " : " + pair.Value);
		}

		SaveFile.current.Objects.Clear();

		//Now, am I actually done, do I need, to generate extra crap?
		if (SaveFile.current.levelGenerator)
		{
			
			int x = 0;
			int y = 0;
			//I do?! What a joy.
			foreach (tileManager.materialType item in SaveFile.current.Tiles.TileTypes)
			{
				switch (item)
				{
					case tileManager.materialType.Grass:
						if (Random.value > 0.97f)
						{
							if(Random.value > 0.8f)
							{
								buildObject(flowers[(int)Random.Range(0, flowers.Length)], x, y);
							}
							else
							{
								buildObject(standardGrassObjects[(int)Random.Range(0, standardGrassObjects.Length)], x, y);
							}
							
						}
						break;
					case tileManager.materialType.Weed:
						break;
					case tileManager.materialType.Soil:
						buildObject(wheat[(int)Random.Range(0, wheat.Length)], x, y);
						break;
					case tileManager.materialType.SoilFertilized:
						break;
					case tileManager.materialType.SoilTilled:
						break;
					case tileManager.materialType.SoilGrowing:
						break;
					case tileManager.materialType.SoilFertlizedGrowing:
						break;
					case tileManager.materialType.SoilTilledGrowing:
						break;
					case tileManager.materialType.UnderGrass:
						buildObject(tallGrass, x, y);
						if (Random.value > 0.96f)
						{
							buildObject(animals[(int)Random.Range(0, animals.Length)], x, y);
						}
						break;
					case tileManager.materialType.TreeSoil:
						if (Random.value > 0.90f)
						{
							buildObject(treespots[(int)Random.Range(0, treespots.Length)], x, y);
						}
						break;
					case tileManager.materialType.TreeHole:
						break;
					case tileManager.materialType.Tree:
						break;
					case tileManager.materialType.Water:
						break;
					case tileManager.materialType.WaterDeep:
						break;
					case tileManager.materialType.WaterSea:
						break;
					case tileManager.materialType.WaterSeaDeep:
						break;
					case tileManager.materialType.Sand:
						break;
					case tileManager.materialType.MetalDeposits:
						if (Random.value > 0.97f)
						{
							buildObject(metalDeposits[(int)Random.Range(0, metalDeposits.Length)], x, y);
						}
						break;
					case tileManager.materialType.Clay:
						break;
					default:
						break;
				}

				x++;
				if (x == SaveFile.current.Tiles.TilesWide)
				{
					x = 0;
					y++;
				}
			}
		}

		//Done, now I can call save complete if anything needs it.
		loadComplete.Invoke();

		//Debug.Log(SaveFile.UID);


	}

	void buildObject(GameObject go,int x, int y)
	{
		GameObject newObj = GameObject.Instantiate(go, transform);
		mapObj newObjST = newObj.GetComponent<mapObj>();


		newObj.transform.position = new Vector3(x, 0f, -y);
		newObjST.storedType.TX = x;

		//Remember, Z is y.
		newObjST.storedType.TY = y;

		newObjST.storedType.UID = SaveFile.GetNewUID();
	}

	private void Start()
	{
		current = this;

		if(loadComplete == null)
		{
			loadComplete = new UnityEvent();
		}
	}



	private void OnEnable()
	{
		SaveLoad.LoadEvent += LoadEvent;
	}

	private void OnDestroy()
	{
		SaveLoad.LoadEvent -= LoadEvent;
	}
}
