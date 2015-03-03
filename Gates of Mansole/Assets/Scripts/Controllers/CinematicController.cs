﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CinematicController : MonoBehaviour {
	
	public TextMesh dialogueText;
	public SpriteRenderer leftImage;
	public SpriteRenderer rightImage;
	public GameObject dialogueWindow;
	public int dialogueLineSize;

	public GameObject[] BackgroundImages;
	public Sprite[] CharacterImages;

	private string cinFile;
	private string type;
	private bool isDone;

	public class _cinematic_entry {
		public int Music;
		public int Background;
		public string ImageLeft;
		public string ImageRight;
		public string Speaker;
		public string Dialogue;
		public float ShowTime;
	};
	public List<_cinematic_entry> Cinematic;

	private int entryIndex;
	private float entryChangeTime;

	// Use this for initialization
	void Start () {
		Debug.Log (Player.currentChapter);
		if (Player.isWatchingIntro) {
			cinFile = Player.chapterIntroCinematicFiles [Player.currentChapter];
			type = "Intro";
			Player.watchedIntroCinematic();
		} else {
			cinFile = Player.chapterExitCinematicFiles [Player.currentChapter];
			type = "Exit";
			Player.watchedExitCinematic();
		}
		Debug.Log ("Chapter : " + Player.currentChapter.ToString() + " " + type +  " : " + Player.chapterIntroCinematicFiles [Player.currentChapter]);

		isDone = false;
		entryIndex = 0;
		entryChangeTime = 0;
		StartCoroutine (Init (cinFile));
	}

	public IEnumerator Init(string filePath) {
		string[] levelData;
		string[] seps = {"\n"};
		string[] sepsLine = {":"};
		
		if (filePath.Contains("://"))
		{
			WWW www = new WWW (filePath);
			yield return www;
			levelData = www.text.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);
		} else {
			levelData = System.IO.File.ReadAllLines(filePath);
		}

		Cinematic = new List<_cinematic_entry> ();
		foreach(string ln in levelData) {
			switch(ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[0].ToLower().TrimStart()) {
			case "entry":
				Cinematic.Add(new _cinematic_entry());
				break;
			case "music":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Music = int.Parse(ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart());
				}
				break;
			case "background":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Background = int.Parse(ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart());
				}
				break;
			case "imageleft":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].ImageLeft = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart();
				}
				break;
			case "imageright":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].ImageRight = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart();
				}
				break;
			case "speaker":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Speaker = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].TrimStart();
				}
				break;
			case "dialogue":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Dialogue = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].TrimStart();
				}
				break;
			case "showtime":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].ShowTime = float.Parse(ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart());
				}
				break;
			}
		}

		isDone = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (isDone == false) {
			return;
		}

		if (Time.time > entryChangeTime) {
			if (entryIndex < Cinematic.Count) {
				entryChangeTime = Time.time + Cinematic[entryIndex].ShowTime;

				if (Cinematic[entryIndex].Dialogue != "None") {
					dialogueWindow.SetActive(true);
					dialogueText.text = processDialogue(Cinematic[entryIndex].Speaker, Cinematic[entryIndex].Dialogue);
				} else {
					dialogueWindow.SetActive(false);
				}

				if (Cinematic[entryIndex].ImageLeft == "none") {
					leftImage.sprite = null;
				} else if (int.Parse(Cinematic[entryIndex].ImageLeft) < CharacterImages.Length) {
					leftImage.sprite = CharacterImages[int.Parse(Cinematic[entryIndex].ImageLeft)];
				}
				
				if (Cinematic[entryIndex].ImageRight == "none") {
					rightImage.sprite = null;
				}
				else if (int.Parse(Cinematic[entryIndex].ImageRight) < CharacterImages.Length) {
					rightImage.sprite = CharacterImages[int.Parse(Cinematic[entryIndex].ImageRight)];
				}

				entryIndex++;
			} else {
				Application.LoadLevel ("LevelSelect");
			}
		}
	}
	
	string processDialogue(string speaker, string text) {
		string newText = "";
		int lineLenth = 0;
		char[] seps = { ' ' };

		if (speaker != "None") {
			newText += speaker;
			newText += ":\n\n";
		}
		
		if (text == null) {
			return newText;
		}
		
		foreach (string word in text.Split(seps)) {
			if ((lineLenth + word.Length) >= dialogueLineSize) {
				newText += "\n";
				lineLenth = word.Length + 1;
			} else {
				lineLenth += word.Length + 1;
			}
			
			newText += word;
			newText += " ";
		}
		
		return newText;
	}
}