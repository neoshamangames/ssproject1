using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class unityIRC : EditorWindow
{
	private static unityIRC window = null;
	
	private static string tempName = string.Empty;
	
	private static float scrollPosition = 0;
	private static float userScrollPosition = 0;
	private string msgField = string.Empty;
	private static GUIStyle chatStyle = new GUIStyle();
	private static GUIStyle privateChatStyle = new GUIStyle();
	private static GUIStyle myChatStyle = new GUIStyle();
	private static GUIStyle serverStyle = new GUIStyle();
		
	public static DateTime lastMsgRead;
	public static DateTime lastUsrRead;
	public static bool logged = false;
	
	private static List<string> messageBuffer = new List<string>();
	private static List<string> users = new List<string>();
	
	private static string ircLog = Application.dataPath + "/../irc.log";
	private static string usrLog = Application.dataPath + "/../userList.log";
	private static string msgLog = Application.dataPath + "/../sendMsg.log";
	private static string runLock = Application.dataPath + "/../running.lock";
	
	private static Process ircCmd = null;
	private static bool isConnecting = false;
	
	public static void Connect(string displayName, string realName)
	{
		if (!File.Exists(ircLog))
			File.Create(ircLog);
		if (!File.Exists(msgLog))
			File.Create(msgLog);
		if (!File.Exists(usrLog))
			File.Create(usrLog);
		
		if (!logged)
		{
			string location = Application.dataPath + "/unityIRC/Editor/unityIRCclient.exe";
			ircCmd = Process.Start(location, displayName + " " + realName);
			
			logged = true;
			isConnecting = false;
		}
	}
	
	private void Update()
	{
		if (!logged && !isConnecting && File.Exists(runLock))
		{
			unityIRC.lastMsgRead = DateTime.MinValue;
			unityIRC.lastUsrRead = DateTime.MinValue;
			GetMessages();
			GetUserList();
			
			chatStyle.wordWrap = true;
			chatStyle.padding.bottom = 3;
			chatStyle.normal.textColor = Color.white;
			chatStyle.padding.top = 3;
			
			privateChatStyle.wordWrap = true;
			privateChatStyle.padding.bottom = 3;
			privateChatStyle.normal.textColor = Color.yellow;
			privateChatStyle.padding.top = 3;
			
			myChatStyle.wordWrap = true;
			myChatStyle.padding.bottom = 3;
			myChatStyle.normal.textColor = Color.green;
			myChatStyle.padding.top = 3;
			
			serverStyle.wordWrap = true;
			serverStyle.padding.bottom = 3;
			serverStyle.normal.textColor = Color.blue;
			serverStyle.padding.top = 3;
			
			logged = true;
		}
		
		if (logged)
		{
			if (File.GetLastWriteTime(ircLog) != unityIRC.lastMsgRead)
				GetMessages();
			
			if (File.GetLastWriteTime(usrLog) != unityIRC.lastUsrRead)
				GetUserList();
		}
	}
	
	private static void GetUserList()
	{
		try
		{
			string[] contents = File.ReadAllText(usrLog).Replace("\r", "").Split('\n');
			users.Clear();
			
			foreach (string c in contents)
			{
				if (!string.IsNullOrEmpty(c.Trim()))
					users.Add(c);
			}
			
			lastUsrRead = File.GetLastWriteTime(usrLog);
		}
		catch { }
	}
	
	private static void GetMessages()
	{
		try
		{
			string[] contents = File.ReadAllText(ircLog).Replace("\r", "").Split('\n');
			messageBuffer.Clear();
			
			foreach (string c in contents)
			{
				if (!string.IsNullOrEmpty(c.Trim()))
					messageBuffer.Add(c);
			}
			
			lastMsgRead = File.GetLastWriteTime(ircLog);
		}
		catch { }
	}
	
	public static void Disconnect()
	{
		logged = false;
		if (File.Exists(runLock))
			File.Delete(runLock);
		
		if (ircCmd != null)
		{
			ircCmd.Close();
			ircCmd = null;
		}
		
		SendMessage(">>> Disconnect <<<");
	}
	
	[MenuItem ("Window/unityIRC")]
	static void OpenWindow()
	{
		chatStyle.wordWrap = true;
		chatStyle.padding.bottom = 3;
		chatStyle.normal.textColor = Color.white;
		chatStyle.padding.top = 3;
		
		privateChatStyle.wordWrap = true;
		privateChatStyle.padding.bottom = 3;
		privateChatStyle.normal.textColor = Color.yellow;
		privateChatStyle.padding.top = 3;
		
		myChatStyle.wordWrap = true;
		myChatStyle.padding.bottom = 3;
		myChatStyle.normal.textColor = Color.green;
		myChatStyle.padding.top = 3;
		
		serverStyle.wordWrap = true;
		serverStyle.padding.bottom = 3;
		serverStyle.normal.textColor = Color.blue;
		serverStyle.padding.top = 3;
		
		unityIRC.lastMsgRead = DateTime.MinValue;
		unityIRC.lastUsrRead = DateTime.MinValue;
		
		window = (unityIRC)EditorWindow.GetWindow(typeof(unityIRC));
		
		window.Focus();
    }
	
	private static void SendMessage(string msg)//, SendType sendType)
	{
		File.WriteAllText(msgLog, msg);
		
		scrollPosition = float.MaxValue;
	}
	
	public static void RecieveMessage(string msg)
	{
		scrollPosition = float.MaxValue;
	}
	
	Texture2D bgColor = null;
	private void OnGUI()
	{
		try
		{
			if (bgColor == null)
			{
				bgColor = new Texture2D(1, 1);
				bgColor.SetPixel(0, 0, Color.black);// new Color(0.219607f, 0.219607f, 0.219607f));
				bgColor.Apply();
			}
			
			Repaint();
			
			if (logged)
			{
				GUI.DrawTexture(new Rect(0, 0, position.width, position.height), bgColor);
				
				Event e = Event.current;
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.BeginVertical();
						scrollPosition = EditorGUILayout.BeginScrollView(new Vector2(0, scrollPosition), GUILayout.Width(position.width - 150)).y;
							foreach (string m in messageBuffer)
							{
								if (m.StartsWith("MYMSG: "))
									GUILayout.Label(m.Remove(0, 7), myChatStyle);
								else if (m.StartsWith("PRIVATE: "))
									GUILayout.Label(m.Remove(0, 9), privateChatStyle);
								else if (m.StartsWith("PUBLIC: "))
									GUILayout.Label(m.Remove(0, 8), chatStyle);
								else if (m.StartsWith("SERVER: "))
									GUILayout.Label(m.Remove(0, 8), serverStyle);
								else
									GUILayout.Label(m, chatStyle);
							}
						EditorGUILayout.EndScrollView();
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
						userScrollPosition = EditorGUILayout.BeginScrollView(new Vector2(0, userScrollPosition), GUILayout.Width(150)).y;
							GUI.SetNextControlName("null");
							GUI.Button(new Rect(0, 0, 0, 0), string.Empty);
							
							foreach (string u in users)
							{
								if (GUILayout.Button(u))
								{
									GUI.FocusControl("null");
									msgField = "!pvt " + u + "!";
								}
							}
						EditorGUILayout.EndScrollView();
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					if (e.keyCode == KeyCode.Return && e.type == EventType.keyDown)
					{
						SendMessage(msgField);
						
						if (msgField.StartsWith("!pvt "))
							msgField = "!" + msgField.Remove(0, 1).Split('!')[0] + "! ";
						else
							msgField = string.Empty;
					}
					else
					{
						GUI.SetNextControlName("Chat_Input");
						msgField = EditorGUILayout.TextField(msgField);
						
						if (e.keyCode == KeyCode.Return && e.type == EventType.keyUp)
							GUI.FocusControl("Chat_Input");
					}
				
					if (GUILayout.Button("Disconnect", GUILayout.Width(100)))
						Disconnect();
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.Space();
				
				EditorGUILayout.BeginHorizontal();
					tempName = EditorGUILayout.TextField("Display Name:", tempName);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				
				EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Connect"))
					{
						if (!string.IsNullOrEmpty(tempName.Trim()))
						{
							isConnecting = true;
							Connect(tempName, "brent");
						}
					}
				EditorGUILayout.EndHorizontal();
			}
		}
		catch (Exception e)
		{
			if (e != null)
			{
				if (!e.Message.ToLower().Contains("repaint") && !e.Message.ToLower().Contains("popping") && !e.Message.ToLower().Contains("collection was modified"))
					UnityEngine.Debug.LogError(e.Message);
			}
		}
	}
	
	private void OnDestroy()
	{
		Disconnect();
	}
}