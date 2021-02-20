using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class builder : MonoBehaviour
{

	public chooseButtons buildWhat;
	public static builder currentBuilder;
	public GameObject helper;
	public GameObject draggingTile;
	public RectTransform selected;
	public GameObject moreOptions;
	chooseButtons currentHovering;
	bool displayHelp;
	public bool currentlyDragging;
	public List<GameObject> checkIfClosed = new List<GameObject>();
	public LayerMask masking;
	bool fillButton;
	bool moveButton;
	public bool interactingLock = false;

	public void updateFill(bool b)
	{
		fillButton = b;
	}

	public void updateMove(bool b)
	{
		moveButton = b;
	}

	//Help system
	public GameObject Hint1;
	public GameObject Hint2;
	int hintSteps = 1;
	string helpKey = "helpSeen";

	public itemSlots[] buttonArray = new itemSlots[10];

	public bool canInteract()
	{
		foreach (var item in checkIfClosed)
		{
			if (item.activeInHierarchy) return false;
		}
		if (interactingLock)
		{
			return false;
		}
		return true;
	}

	public bool overInventory()
	{
		PointerEventData pe = new PointerEventData(EventSystem.current);
		pe.position = Input.mousePosition;
		List<RaycastResult> hits = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pe, hits);

		foreach (RaycastResult h in hits)
		{
			if (h.gameObject == moreOptions)
				return true;
		}

		return false;
	}

	public void currentlyHovering(chooseButtons cb)
	{
		if (currentlyDragging) return;
		currentHovering = cb;
		showHelpText(currentHovering.RollOverText);

	}


	public void showHelpText(string txt)
	{
		if (currentlyDragging) return;
		helper.gameObject.SetActive(true);
		helper.GetComponentInChildren<Text>().text = txt;
		LayoutRebuilder.ForceRebuildLayoutImmediate(helper.GetComponent<RectTransform>());
		displayHelp = true;
	}

	public void changeObject(itemSlots item)
	{
		foreach (var item2 in gameObject.GetComponentsInChildren<itemSlots>())
		{
			item2.selected = false;
		}

		item.selected = true;
		buildWhat = item.storedChoose;
		selected.position = item.GetComponent<RectTransform>().position;

	}

	public void EndObject()
	{
		if (currentlyDragging) return;
		displayHelp = false;
		helper.SetActive(false);
	}


	// Use this for initialization
	void Start()
	{
		currentBuilder = this;
		if (PlayerPrefs.HasKey(helpKey))
		{
			if(PlayerPrefs.GetInt(helpKey) == 1)
			{
				hintSteps = 3;
				Hint1.SetActive(false);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		//Delay stuff
		if (displayHelp)
		{
			helper.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
		}
		if (!canInteract()) return;
		//Build Tiles or Objects
		if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && !currentlyDragging)
		{

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 300, masking))
			{
				int x = Mathf.FloorToInt(hit.point.x + .5f);
				int y = Mathf.FloorToInt(hit.point.z + .5f); // REMEMBER that y is remapped to Z!

				if (Input.GetKey(KeyCode.Q))
				{
					var go = GameObject.FindGameObjectWithTag("Player");
					go.transform.position = new Vector3(x, 0f, y);
					var gos = go.GetComponent<mapObj>();
					gos.storedType.TX = x;
					gos.storedType.TY = -y;

					if (SaveFile.current.levelGenerator)
					{
						SaveFile.current.CameraX = x * 3;
						SaveFile.current.CameraZ = y * 3;
					}

					SaveFile.current.makePlayerVisible = true;
					

					return;
				}

				if (buildWhat.isObject)
				{
					//First, let's make the proper X and Y
					

					if (tileManager.currentTileManager.isTileLocked(x, y)) return;

					GameObject newObj = GameObject.Instantiate(buildWhat.model, ObjectBuilder.current.transform);
					mapObj newObjST = newObj.GetComponent<mapObj>();


					newObj.transform.position = new Vector3(x, 0f, y);
					newObjST.storedType.TX = (int)newObj.transform.position.x;

					//Remember, Z is y.
					newObjST.storedType.TY = (int)-newObj.transform.position.z;

					newObjST.storedType.UID = SaveFile.GetNewUID();


					if (buildWhat.changeTile)
					{
						tileManager.currentTileManager.UpdateTile(Mathf.CeilToInt(hit.point.x + 0.5f),
						Mathf.CeilToInt(-hit.point.z + 0.5f), buildWhat.matType);
					}

				}
				else
				{
					//Just a tile, so let us check for fill tool.
					if (Input.GetButton("fill") || fillButton)
					{
						if (Input.GetMouseButtonDown(0))
						{
							//This is for filling, simple I know.
							//Now to get the current tile at hit point.
							tileManager.currentTileManager.FillTile(Mathf.CeilToInt(hit.point.x + 0.5f),
						Mathf.CeilToInt(-hit.point.z + 0.5f), buildWhat.matType);
						}
					}
					else
					{
						tileManager.currentTileManager.UpdateTile(Mathf.CeilToInt(hit.point.x + 0.5f),
					Mathf.CeilToInt(-hit.point.z + 0.5f), buildWhat.matType);
					}

				}

			}


		}

		

		if (Input.GetKeyDown(KeyCode.E) && !currentlyDragging)
		{
			switch (hintSteps)
			{
				case 1:
					Hint1.SetActive(false);
					Hint2.SetActive(true);
					break;
				case 2:
					Hint2.SetActive(false);
					if (!PlayerPrefs.HasKey(helpKey))
					{
						PlayerPrefs.SetInt(helpKey, 1);
						PlayerPrefs.Save();
					}
					
					break;
				default:
					break;
			}

			hintSteps += 1;
			moreOptions.SetActive(!moreOptions.activeInHierarchy);
		}

		if(Input.GetKeyDown(KeyCode.Escape) && moreOptions.activeInHierarchy)
		{
			moreOptions.SetActive(false);
		}

		//Now, let's check if they input 0-9, and then map to that array!
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			changeObject(buttonArray[0]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			changeObject(buttonArray[1]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			changeObject(buttonArray[2]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			changeObject(buttonArray[3]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			changeObject(buttonArray[4]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			changeObject(buttonArray[5]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			changeObject(buttonArray[6]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			changeObject(buttonArray[7]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			changeObject(buttonArray[8]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			changeObject(buttonArray[9]);
		}
	}
}
