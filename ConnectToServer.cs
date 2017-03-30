using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;

[Serializable()]
public class ConnectToServer {

	public string url ="http://happs-production.herokuapp.com";

	private string key;
	private WebRequest request;
	private string json;

	public ConnectToServer(){
		if (Application.platform == RuntimePlatform.OSXEditor)
			url = "http://happs-staging.herokuapp.com";
	}

	public string CreatePlayer(string username){
		return Request ("/api/players","POST","{ \"username\": \"" +username +"\"}","");
	}

	public string GetUser(string code){
		return Request ("/api/players/me","GET","",code);
	}

	public string GetUserById(string id,string code){
		return Request ("/api/players/"+id,"GET","",code);

	}

	public string GetChallenges(string code){
		return Request ("/api/challenges/me","GET","",code);
	}

	public string GetActiveChallenges(string code){
		return Request ("/api/challenges/me/active","GET","",code);
	}
		
	public string GetActiveChallengeById(string code,string id){
		return Request ("/api/challenges/"+code,"GET","",id);
	}

	public string UpdateChallenge(string id){
		return Request ("/api/challenges/"+id,"PUT","","completed");

	}

	public string UpdateChallenge(string id,string code,string jsonString){
		return Request ("/api/challenges/"+id,"PUT",jsonString,code);
	}

	public string SaveUser(string id,string connectionCode,string jsonString){
		return Request ("/api/players/"+id,"PUT",jsonString,connectionCode);
	}

	public void CreateResults(string id,string connectionCode,string jsonString){
		Request ("/api/results","POST",jsonString,connectionCode);
	}

	public void SaveResults(string id,string connectionCode,string jsonString){
		Request ("/api/results","PUT",jsonString,connectionCode);
	}
		
	public string GetWordLists(string code,int gameIndex){
		return Request ("/api/settings/game/"+gameIndex,"GET","",code);
	}

	public string Request(string urlSuffix,string requestTypeString,string postString,string connectionCode){
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create (@url+urlSuffix);
		//WebRequest request=WebRequest.Create(url);

		request.UserAgent = 
			"User-Agent:Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.94 Safari/537.36;"+
			"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; WOW64; " +
			"Trident/4.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; " +
			".NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET CLR 3.0.30618; " +
			"InfoPath.2; OfficeLiveConnector.1.3; OfficeLivePatch.0.0)";
	
		request.Headers.Add ("Connection-Code" , connectionCode);

		Stream dataStream;
		string responseFromServer = null;
		//request.Proxy = null;
		HttpWebRequest.DefaultWebProxy=null;

		try{
		request.Method = requestTypeString;
		request.ContentType= "application/json; charset=utf-8";
		if(!requestTypeString.Equals("GET")){
		byte[] byteArray = Encoding.UTF8.GetBytes (postString);

		request.ContentLength = byteArray.Length;

		dataStream = request.GetRequestStream ();

		dataStream.Write (byteArray, 0, byteArray.Length);
		dataStream.Close ();

		}
		WebResponse response = request.GetResponse ();
		//Debug.Log (((HttpWebResponse)response).StatusDescription);
		dataStream = response.GetResponseStream ();

		StreamReader reader = new StreamReader (dataStream);
		responseFromServer = reader.ReadToEnd ();

		reader.Close ();
		dataStream.Close ();
			response.Close ();
		}
		catch(WebException e){
			try{
			Debug.Log (e+"\n");
			dataStream=e.Response.GetResponseStream ();
			StreamReader reader = new StreamReader (dataStream);
			string errorMessage = reader.ReadToEnd ();
			Debug.Log (errorMessage);
			reader.Close ();
				dataStream.Close ();}
			catch(Exception ex){
				Debug.Log("connection failed");
			}
		}
		return responseFromServer;

	}
}
