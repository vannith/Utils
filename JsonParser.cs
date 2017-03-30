using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class JsonParser<T> {

	private static string PATH = Application.persistentDataPath;

	public static List<T> ParseFile(string file,bool isDataFile)
    {
        JsonReaderSettings rsettings = new JsonReaderSettings();
        rsettings.TypeHintName = "__type";

     
		string jsonString = "";
		if(isDataFile){
            TextAsset textAsset = Resources.Load<TextAsset>(file);
            jsonString = textAsset.text;
		}
		else{
			if (!File.Exists(PATH + file + ".json"))
				return new List<T>();
			else {
				jsonString = (string) File.ReadAllText(PATH+file+".json");
			}
		}

        JsonReader reader = new JsonReader(jsonString, rsettings);
        List<T> list = (List<T>)reader.Deserialize(typeof(List<T>));

        return list;
    }

    public static void WriteToJson(List<T> list, string file)
    {
        JsonWriterSettings wsettings = new JsonWriterSettings();
        wsettings.PrettyPrint = true;
        wsettings.TypeHintName = "__type";

        System.Text.StringBuilder output = new System.Text.StringBuilder();
        JsonWriter wr = new JsonWriter(output, wsettings);
        wr.Write(list);

		var streamWriter = new StreamWriter(PATH + file + ".json", false);
        streamWriter.Write(output);
        streamWriter.Close();
    }
}
