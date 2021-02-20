using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;

[System.Serializable]
public class IngredientType
{
	[JsonRequired]
	public string ID;
	public int UID
	{
		get
		{
			return uID;
		}

		set
		{
			uID = value;
			if (value > SaveFile.UID) SaveFile.UID = value;
		}
	}
	int uID;
	public int TX;
	public int TY;
}

[System.Serializable]
public class SimpleType {

	[JsonRequired]
	public string ID;

	public int UID
	{
		get
		{
			return uID;
		}

		set
		{
			uID = value;
			if (value > SaveFile.UID) SaveFile.UID = value;
		}
	}

	int uID;
	public int TX;
	public int TY;
	public int Rotation = 0;
	public SaveCarry Carry;
	public SaveInv Inv;
	//Used by anything that can wear clothes
	public SaveClothes Clothes;
	//Upgrades
	public SaveUp Up;

	public float Energy = 0;

	public int State = 0;
	public string ToCreateItem;
	public int NumCreated = 0;
	public List<IngredientType> IngredientsItems;
	public string New;
	public int NewID = 0;
	public int ST = 0;
	public int STT = 0;
	public int HutID = 0;
	public int Used = 0; //Counts number of times tool is used.

	//Used by buckets
	public string Held;

	//Used by cows
	public int EC = 0;
	public int FC = 0;

	//Used by native
	public string NM;
	public int EN;
	public SimpleType Hat;
	public SimpleType Top;

	//Used by rock, manure
	public bool TD;


	//Used for spawn animations
	public int SX;
	public int SY;
	public int EX;
	public int EY;
	public int JumpHeight;
	public int StartHeight;
	public int EndHeight;

	public string ObjectType;
	public int SM = 0;
	public string WN;
	public WorkerInterpreter Interpreter;

	//Used by flower pot
	public int Type;

	//Used by data storage things
	public List<WorkerHighInstructionArray> HighInstructionsArray;

	[HideInInspector]
	public string Name;

	public bool ShouldSerializeName()
	{
		return Name != null;
	}

	

	//Used by storageseedlings
	public int Seeds;
	public int Fertiliser;

	//Cropwhear
	public int SD;


	

}

public class SaveCarry
{
	[JsonRequired]
	public List<SimpleType> CarryObjects;
	//Carryobjects
}

public class SaveInv
{
	[JsonRequired]
	public List<SimpleType> InvObjects;
	//InvObjects
}

public class SaveUp
{
	public SimpleType[] UpgradeObjects;
}

public class SaveClothes
{
	[JsonRequired]
	public List<SimpleType> ClothesObjects;
}

public class WorkerInterpreter
{
	[JsonRequired]
	public List<WorkerScriptLocalArray> ScriptLocalArray;
	[JsonRequired]
	public List<string> GlobalArray;
	public List<WorkerHighInstructionArray> HighInstructionsArray;
}

//Belongs to ScriptLocalArray

public class WorkerScriptLocalArray
{
	public WorkerScript Script;
	public int Instruction;
	public List<string> LocalArray;
}

public class WorkerScript
{
	public string Name;
	public List<WorkerInstructions> Instructions;


}

public class WorkerInstructions
{
	[JsonRequired]
	public string Ins;
	[JsonRequired]
	public string Var1;
	[JsonRequired]
	public string Var2;
	[JsonRequired]
	public string Var3;
	[JsonRequired]
	public string Var4;
}

//Below is high instruction
public class WorkerHighInstructionArray
{
	[JsonProperty(Required = Required.Always)]
	public string Type;
	[JsonProperty(Required = Required.Always)]
	public string ArgName;
	public int Line;
	public List<WorkerHighInstructionArray> Children;
	[JsonProperty(Required = Required.Always)]
	public string OT;
	public int UID;
	public int X;
	public int y;
	[JsonProperty(Required = Required.Always)]
	public string V1;
	[JsonProperty(Required = Required.Always)]
	public string V2;

}