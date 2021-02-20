using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileManager : MonoBehaviour
{

	int tileType;
	Material tileMaterial;
	public GameObject plotPre;
	Vector2 startingPosition = Vector2.zero;
	GameObject plotHolder;
	Transform plotTransform;
	static public tileManager currentTileManager;

	const int plotWidth = 21;
	const int plotHeight = 12;
	const int imageSize = 32;

	public Material[] matTypes = new Material[20];
	public enum materialType
	{
		Grass						= 0,
		Weed						= 1,
		Soil						= 2,
		SoilFertilized				= 3,
		SoilTilled					= 4,
		SoilGrowing					= 5,
		SoilFertlizedGrowing		= 7,
		SoilTilledGrowing			= 8,
		UnderGrass					= 9,
		TreeSoil					= 10,
		TreeHole					= 11,
		Tree						= 12,
		Water						= 13,
		WaterDeep					= 14,
		WaterSea					= 15,
		WaterSeaDeep				= 16,
		Sand						= 17,
		MetalDeposits				= 18,
		Clay						= 19
	};


	public Color[][] colorsPreCalc;

	// Use this for initialization
	void Start()
	{
		currentTileManager = this;

		colorsPreCalc = new Color[matTypes.Length][];

		for (int i = 0; i < matTypes.Length; i++)
		{
			if (matTypes[i] == null)
			{
				Debug.Log("Material was null at " + i);
				colorsPreCalc[i] = null;
				break;
			}
			Texture2D tileTex = matTypes[i].mainTexture as Texture2D;
			colorsPreCalc[i] = tileTex.GetPixels();
		}

		LoadEvent();
	}

	public void UpdateTile(int x, int y, materialType mat, bool lazy = false)
	{
		GameObject plot = getPlotTransform(x, y);
		if (!plot) return;

		Texture2D tex = plot.GetComponent<Renderer>().material.mainTexture as Texture2D;

		int tx = x % plotWidth;
		int ty = y % plotHeight;

		if (tx == 0) tx = plotWidth;
		if (ty == 0) ty = plotHeight;

		//Division and mod don't work with 0 index, so we were sent 1 index, now let's fix that.

		//We have to invert y for visual looks. the formula for ty below also properly moves it to 0 index.
		ty = -ty + plotHeight;
		tx--;

		//Texturesize info, getting starting points
		tx *= imageSize;
		ty *= imageSize;
		tex.SetPixels(tx, ty, imageSize, imageSize, colorsPreCalc[(int)mat]);

		if (!lazy)
		{
			tex.Apply();
		}

		//Again, working from 1 index back to 0 index.
		y--;
		x--;
		
		SaveFile.current.Tiles.TileTypes[(y)*SaveFile.current.Tiles.TilesWide+x] = (int)mat;
	}

	public void FillTile(int x, int y, materialType toMat)
	{
		int fromMat = GetTileType(x, y);

		if (fromMat == (int)toMat) return;

		UpdateTile(x, y, toMat);

		StartCoroutine(FillLoop(x, y, (int)toMat, fromMat));


	}

	bool isInvalidPosition(int x, int y)
	{
		return (x > SaveFile.current.Tiles.TilesWide || y > SaveFile.current.Tiles.TilesHigh || x <= 0 || y <= 0) ;
	}

	int GetTileType(int x, int y)
	{
		x--;
		y--;
		return SaveFile.current.Tiles.TileTypes[(y) * SaveFile.current.Tiles.TilesWide + x];
	}

	IEnumerator FillLoop(int x, int y, int toMat, int fromMat)
	{
		Queue<Vector2> Q = new Queue<Vector2>();
		Q.Enqueue(new Vector2(x, y));

		int RunNumber = 1; //Adds some pretty draw
		int curNumber = 0;
		bool actLazy = false;

		while (Q.Count != 0)
		{
			curNumber++;

			if (curNumber >= RunNumber)
			{
				curNumber = 0;
				RunNumber += 2;
				if (RunNumber > 50) {
					RunNumber = 50000;
					actLazy = true;
				} 
				yield return null;
			}
			Vector2 curPosition = Q.Dequeue();

			Vector2[] checkPositions = new Vector2[4];

			checkPositions[0] = new Vector2(curPosition.x + 1, curPosition.y);
			checkPositions[1] = new Vector2(curPosition.x - 1, curPosition.y);
			checkPositions[2] = new Vector2(curPosition.x, curPosition.y + 1);
			checkPositions[3] = new Vector2(curPosition.x, curPosition.y - 1);

			foreach (var position in checkPositions)
			{
				if (isInvalidPosition((int)position.x, (int)position.y)) break;

				int foundTile = GetTileType((int)position.x, (int)position.y);

				if(foundTile == fromMat)
				{
					UpdateTile((int)position.x, (int)position.y, (materialType)toMat, actLazy);
					Q.Enqueue(position);
				}
			}
		}

		if (actLazy)
		{
			UpdateAllPlots();
		}

	}


	GameObject getPlotTransform(int x, int y)
	{
		//Is this an impossible tile location?
		if (x > SaveFile.current.Tiles.TilesWide || y > SaveFile.current.Tiles.TilesHigh || x <= 0 || y <= 0) return null;

		int px = x / plotWidth;
		int py = y / plotHeight;

		////This handles fringe cases due to integer division reporting 1 when we actually want 0.
		////Subtracting from x caused the left edges to fail.
		if (x % plotWidth == 0) px--;
		if (y % plotHeight == 0) py--;

		int pw = SaveFile.current.Tiles.TilesWide / plotWidth;
		int pt = px + (py * pw);
		//pt is the proper plot transform.
		return plotTransform.GetChild(pt).gameObject;
	}

	public bool isTileLocked(int x, int y)
	{
		y *= -1;
		return SaveFile.current.Tiles.TileLocked[(y) * SaveFile.current.Tiles.TilesWide + x];
	}

	public void lockTile(int x, int y)
	{
		SaveFile.current.Tiles.TileLocked[(y) * SaveFile.current.Tiles.TilesWide + x] = true;
	}

	public void unLockTile(int x, int y)
	{
		SaveFile.current.Tiles.TileLocked[(y) * SaveFile.current.Tiles.TilesWide + x] = false;
	}

	void LoadEvent()
	{
		startingPosition = Vector2.zero;
		int plots = SaveFile.current.Plots.PlotsVisible.Length;
		//plotArray = new tile[tileLength];
		//We are going to build plots!

		//Plot holder creation

		plotHolder = new GameObject("plotHolder");
		plotTransform = plotHolder.transform;
		plotTransform.parent = transform;

		for (int i = 0; i < plots; i++)
		{
			var curTile = GameObject.Instantiate(plotPre);
			curTile.transform.parent = plotTransform;
			curTile.transform.position = new Vector3(startingPosition.x, 0f, startingPosition.y);

			//This crazy code just makes the material and texture unique. Without it we would update all plots when we draw on one.
			curTile.GetComponent<Renderer>().material = Instantiate<Material>(curTile.GetComponent<Renderer>().material);
			curTile.GetComponent<Renderer>().material.mainTexture = Instantiate<Texture2D>(curTile.GetComponent<Renderer>().material.mainTexture as Texture2D);
			curTile.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;

			startingPosition.x += plotWidth;

			if (startingPosition.x == SaveFile.current.Tiles.TilesWide)
			{
				startingPosition.x = 0;
				startingPosition.y -= plotHeight;
			}
		}

		//for (int i = 0; i < plotTransform.childCount; i++)
		//{
		//	plotTransform.GetChild(i).gameObject.name = "Plot:" + i;
		//}

		//Now that we have the plots, we need to set them to the proper tiles.


		for (int i = 1; i <= SaveFile.current.Tiles.TileTypes.Length; i++)
		{
			//First, let's get the coordinates
			int x = i % (SaveFile.current.Tiles.TilesWide);
			if (x == 0) x = SaveFile.current.Tiles.TilesWide;

			int y = Mathf.CeilToInt((float)i / SaveFile.current.Tiles.TilesWide);
			//We now have a coordinate.  We can get the proper plot by parsing this coordinate.


			//Find material id.
			int mId = SaveFile.current.Tiles.TileTypes[i-1];

			UpdateTile(x, y, (materialType)mId,true);
		}

		UpdateAllPlots();
	}

	void UpdateAllPlots()
	{
		foreach (var plot in plotHolder.GetComponentsInChildren<Renderer>())

		{
			Texture2D tex = plot.material.mainTexture as Texture2D;
			tex.Apply();
		}
	}

	//private void OnEnable()
	//{
	//	SaveLoad.LoadEvent += LoadEvent;
	//}

	//private void OnDestroy()
	//{
	//	SaveLoad.LoadEvent -= LoadEvent;
	//}

	// Update is called once per frame
	void Update()
	{
	}

}
