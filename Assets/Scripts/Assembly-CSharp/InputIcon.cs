using System;
using UnityEngine;

[Serializable]
public class InputIcon
{
	public string Name;

	public Sprite Image;

	public string Text = "";

	public Sprite AlternateImage;

	public string AlternateText = "";

	public Sprite PS4ControllerImage;

	public Sprite XBoxControllerImage;

	public Sprite NintendoProControllerImage;

	public Sprite StadiaControllerImage;

	public bool SpecialAllowedForKeyboard = true;

	public bool PrioritizeOverActualMapping;
}
