using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
//using FullSerializer;
using Newtonsoft.Json;
using System.Text;

public class SaveLoad : MonoBehaviour
{

	public delegate void LoadAction();

	public static event LoadAction LoadEvent;

	static bool onlyOne;

	static public Animator saveNotifier;

	static public StringBuilder sb = new StringBuilder();
	static public int errorCounter = 0;


	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public static void NewSave(int x, int y)
	{
		SaveFile.current = new SaveFile();
		SaveFile.UID = 0;
		SaveFile.current.makeTiles(x, y);
		SaveFile.current.makeCamera();
		SaveFile.current.makePlayer();
	}




	//private static readonly fsSerializer _serializer = new fsSerializer();

	public static void Save(string fileName)
	{
		testSave.current.PlayNotification();
		int i = 0;
		foreach (var item in ObjectBuilder.current.GetComponentsInChildren<mapObj>())
		{
			SaveFile.current.Objects.Add(item.storedType);
			i++;
		}

		if (SaveFile.current.makePlayerVisible)
		{
			GameObject go = GameObject.FindGameObjectWithTag("Player");
			int plotWidth = 21;
			int plotHeight = 12;
			int x = (int)go.transform.position.x;
			int y = (int)-go.transform.position.z;


			int px = x / plotWidth;
			int py = y / plotHeight;

			////This handles fringe cases due to integer division reporting 1 when we actually want 0.
			////Subtracting from x caused the left edges to fail.
			if (x % plotWidth == 0) px--;
			if (y % plotHeight == 0) py--;

			int pw = SaveFile.current.Tiles.TilesWide / plotWidth;
			int pt = px + (py * pw);

			SaveFile.current.Plots.PlotsVisible[pt] = 1;
		}

		if (File.Exists(fileName))
		{
			string newFileName = Path.GetFileNameWithoutExtension(fileName) + "BACKUP.txt";
			string newFilePlace = Path.GetDirectoryName(fileName);

			File.Copy(fileName, newFilePlace + "\\" + newFileName,true);
		}
		JsonSerializer serializer = new JsonSerializer();
		StreamWriter sw = new StreamWriter(fileName);
		JsonWriter writer = new JsonTextWriter(sw);
		serializer.NullValueHandling = NullValueHandling.Ignore;
		serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
		serializer.ContractResolver = new SkipEmptyCollectionsContractResolver();



		serializer.Serialize(writer, SaveFile.current);

		SaveFile.current.Objects.Clear();


		//fsData data;
		//_serializer.TrySerialize<SaveFile>(SaveFile.current, out data);

		//fsJsonPrinter.CompressedJson(data, sw);
		sw.Close();
		//JSONNode J = JSON.Parse(JsonUtility.ToJson(SaveFile.current));

		//File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".txt", J.ToString());

	}

	private void OnEnable()
	{
		if (onlyOne)
		{
			GameObject.Destroy(gameObject);
			return;
		}
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		onlyOne = true;
	}

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		if (arg0.buildIndex == 1)
		{
			LoadEvent();
		}

	}

	public static void StartFile()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			SceneManager.LoadSceneAsync("generativeload", LoadSceneMode.Single);
			//We will do loadevent after scene change.
		}
		else
		{
			LoadEvent();
		}
	}

	private static void ErrorHandler(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
	{
		string error = args.ErrorContext.Error.Message;
		error = error.Replace("Could not find member", "Editor doesn't expect");

		sb.AppendLine(error);
		errorCounter++;
		args.ErrorContext.Handled = true;
	}

	public static void Load(string fileName)
	{

		StreamReader sr = new StreamReader(fileName);
		SaveFile.UID = 0;
		SaveFile newSave = JsonConvert.DeserializeObject<SaveFile>(sr.ReadToEnd(),
		new JsonSerializerSettings
		{
			MissingMemberHandling = MissingMemberHandling.Error,
			Error = ErrorHandler
		}
		);

		newSave.External = 1;
		newSave.Tiles.TileLocked = new bool[newSave.Tiles.TileTypes.Length];
		SaveFile.current = newSave;

		StartFile();


	}
}
