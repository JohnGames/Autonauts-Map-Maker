using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapObj : MonoBehaviour {

	public SimpleType storedType;
	public bool unDeletable;
	public bool rotateThis;
	public bool randomRotate;

	[Header("Set below if you objects only support 4 directions instead of 8")]
	public bool fourwayrotate = false;


	// Use this for initialization
	void Start () {

		name = storedType.ID;

		if (rotateThis)
		{
			int Rotation;
			if (fourwayrotate)
			{
				Rotation = 90;
			}
			else
			{
				Rotation = 45;
			}
			transform.Rotate(0, Rotation * storedType.Rotation, 0, Space.World);
		}
		if(randomRotate) transform.Rotate(0, Random.value*365, 0, Space.World);
		tileManager.currentTileManager.lockTile((int)transform.position.x, -(int)transform.position.z);
	}

	private void OnMouseOver()
	{
		if(!unDeletable)
		{
			if (Input.GetMouseButtonDown(1))
			{
				tileManager.currentTileManager.unLockTile((int)transform.position.x, (int)-transform.position.z);
				GameObject.Destroy(gameObject);
			}
			else if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift))
			{
				tileManager.currentTileManager.unLockTile((int)transform.position.x, (int)-transform.position.z);
				GameObject.Destroy(gameObject);
			}
		}
		
	}

	// Update is called once per frame
	void Update () {
		
	}
}
