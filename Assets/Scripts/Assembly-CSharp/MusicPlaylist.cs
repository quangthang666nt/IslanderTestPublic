using UnityEngine;

[CreateAssetMenu(menuName = "Islanders/Music Playlist")]
public class MusicPlaylist : ScriptableObject
{
	public AudioClip[] soundtrack;

	public AudioClip[] fanfare;
}
