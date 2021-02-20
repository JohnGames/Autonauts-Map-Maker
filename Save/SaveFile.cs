using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;



[System.Serializable]
public class SaveFile
{

	public static SaveFile current;
	public string Version = "15.1";
	public int External = 1;
	public float CameraDistance = 20;
	public float CameraX;
	public float CameraZ;
	public SaveTiles Tiles = new SaveTiles();
	public SavePlots Plots = new SavePlots();
	public SaveScripts Scripts = new SaveScripts();
	public SaveResources Resources = new SaveResources();
	public SaveObjectTypes ObjectTypes = new SaveObjectTypes();
	public SaveSpawnAnimations SpawnAnimations = new SaveSpawnAnimations();

	public SaveWorldSettings WorldSettings = new SaveWorldSettings();

	[JsonRequired]
	public Research Research = new Research();

	public List<SimpleType> Objects = new List<SimpleType>();


	[System.NonSerialized]
	public static int UID;

	[System.NonSerialized]
	public List<GameObject>[,] stackedObjectsArray;

	[System.NonSerialized]
	public bool[,] fenceArray;

	[System.NonSerialized]
	public bool levelGenerator;
	[System.NonSerialized]
	public bool makePlayerVisible;

	//public List<GameObject> this[int x, int y]
	//{
	//	get
	//	{
	//		var stuffList = stackedObjectsArray[x, y];
	//		if(stuffList == null)
	//		{
	//			stuffList = new List<GameObject>();
	//			stackedObjectsArray[x, y] = stuffList;
	//		}
	//		return stuffList;
	//	}
	//}

	public static int GetNewUID()
	{
		UID += 1;
		return UID;
	}

	public void makeTiles(int tw = 20, int th = 20)
	{
		//I am called if nothing was set.
		Tiles.TilesWide = tw * 21;
		Tiles.TilesHigh = th * 12;
		Plots.PlotsVisible = new int[tw * th];
		Tiles.TileTypes = new int[tw * 21 * th * 12];
		Tiles.TileLocked = new bool[tw * 21 * th * 12];
	}

	//public void changeTile(int x, int y, tileManager.materialType t = tileManager.materialType.Grass)
	//{
	//	Tiles.TileTypes[y * Tiles.TilesWide + x] = (int)t;

	//	//SaveFile.current.Tiles.TileTypes[(y) * SaveFile.current.Tiles.TilesWide + x] = (int)mat;
	//}

	public void makeCamera()
	{
		CameraX = Tiles.TilesWide / 2 * 3;
		CameraZ = -Tiles.TilesHigh / 2 * 3;
	}

	public void makePlayer()
	{
		SimpleType player = new SimpleType();
		player.ID = "FarmerPlayer";
		player.TX = Mathf.RoundToInt(Tiles.TilesWide / 2);
		player.TY = Mathf.RoundToInt(Tiles.TilesHigh / 2);
		player.UID = 1;
		player.Energy = 100;
		player.Rotation = 2;
		Objects.Add(player);
	}

}

[System.Serializable]
public class Research
{
	public List<Topics> TopicsArray;
}

[System.Serializable]
public class Topics
{
	public string Name;
	public int Completed;
	public bool Started;
}

[System.Serializable]
public class SaveTiles
{
	//Just some default values.
	public int TilesWide;
	public int TilesHigh;
	public int[] TileTypes;
	[System.NonSerialized]
	public bool[] TileLocked;
	public List<SaveTileExtra> TileExtra = new List<SaveTileExtra>();
}

[System.Serializable]
public class SaveWorldSettings
{
	public int[] WorkerSerials;
}

[System.Serializable]
public class SaveTileExtra
{
	public int x;
	public int y;
	public int T;
}


[System.Serializable]
public class SavePlots
{

	public int[] PlotsVisible;
}

[System.Serializable]
public class SaveScripts
{
	public List<string> ScriptArray;
}

[System.Serializable]
public class SaveResources
{
	public List<string> ResourceTypes;
	public List<int> ResourceCount;
	public List<string> ReservedTypes;
	public List<int> ReservedCount;
}

[System.Serializable]
public class SaveObjectTypes
{
	public List<string> ObjectTypes;
}

[System.Serializable]
public class SaveSpawnAnimations
{
	public List<SimpleType> Objects = new List<SimpleType>();
}
