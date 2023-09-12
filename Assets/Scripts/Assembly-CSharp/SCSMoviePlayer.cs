using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class SCSMoviePlayer : MonoBehaviour
{
	public delegate void OnEndDelegate();

	private VideoPlayer _videoPlayer;

	public VideoClip _clip;

	public VideoRenderMode _renderMode;

	public VideoSource _sourceType;

	private AudioSource _audioSource;

	public RenderTexture _renderTexture;

	public Renderer _renderTheMovieOnThisObject;

	public Camera _targetCamera;

	public CameraClearFlags _clearFlags = CameraClearFlags.Color;

	public Color _color = Color.black;

	public string _url;

	public string MoviePath;

	private int AllocatedWidth;

	private int AllocatedHeight;

	private Renderer quad;

	public Material FMVTexture;

	private Camera SwitchCamera;

	private bool mustDestroyCamera;

	private bool skipped;

	public bool _skipable;

	public KeyCode _skipButton;

	private bool finished;

	public event OnEndDelegate onEndEvent;

	public void PlayVideo()
	{
		_PlayVideo();
	}

	private void _PlayVideo()
	{
		finished = false;
		_videoPlayer = base.gameObject.AddComponent<VideoPlayer>();
		_audioSource = base.gameObject.AddComponent<AudioSource>();
		_videoPlayer.renderMode = _renderMode;
		_videoPlayer.source = _sourceType;
		_videoPlayer.playOnAwake = false;
		_audioSource.playOnAwake = false;
		_videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
		_videoPlayer.EnableAudioTrack(0, enabled: true);
		_videoPlayer.SetTargetAudioSource(0, _audioSource);
		mustDestroyCamera = false;
		if (_renderMode == VideoRenderMode.RenderTexture)
		{
			_renderTheMovieOnThisObject.material.mainTexture = _renderTexture;
			_renderTheMovieOnThisObject.enabled = true;
			_videoPlayer.targetTexture = _renderTexture;
		}
		else if (_renderMode == VideoRenderMode.CameraFarPlane || _renderMode == VideoRenderMode.CameraNearPlane)
		{
			if (_targetCamera != null)
			{
				_videoPlayer.targetCamera = _targetCamera;
			}
			else
			{
				_targetCamera = base.gameObject.AddComponent<Camera>();
				_targetCamera.clearFlags = _clearFlags;
				_targetCamera.backgroundColor = _color;
				_videoPlayer.targetCamera = _targetCamera;
				mustDestroyCamera = true;
			}
		}
		if (_sourceType == VideoSource.VideoClip)
		{
			_videoPlayer.clip = _clip;
		}
		else
		{
			if (_url[0] != '/')
			{
				_url = "/" + _url;
			}
			_videoPlayer.url = Application.streamingAssetsPath + _url;
		}
		_videoPlayer.prepareCompleted -= OnVideoPrepared;
		_videoPlayer.prepareCompleted += OnVideoPrepared;
		_videoPlayer.Prepare();
	}

	protected void OnVideoPrepared(VideoPlayer videoPlayer)
	{
		_videoPlayer.Play();
		_audioSource.Play();
		StartCoroutine(DestroyAfterSecs((float)_videoPlayer.frameCount / _videoPlayer.frameRate));
	}

	private void Update()
	{
		if (_videoPlayer != null && _videoPlayer.isPlaying && _skipable && Input.GetKeyDown(_skipButton))
		{
			Skipped();
		}
	}

	private IEnumerator DestroyAfterSecs(float secs)
	{
		yield return new WaitForSecondsRealtime(secs);
		if (!skipped)
		{
			Object.Destroy(_audioSource);
			Object.Destroy(_videoPlayer);
			if (mustDestroyCamera)
			{
				Object.Destroy(_targetCamera);
			}
			finished = true;
			if (this.onEndEvent != null)
			{
				this.onEndEvent();
			}
		}
	}

	public void Skipped()
	{
		if (!skipped)
		{
			_videoPlayer.Stop();
			skipped = true;
			Object.Destroy(_audioSource);
			Object.Destroy(_videoPlayer);
			if (mustDestroyCamera)
			{
				Object.Destroy(_targetCamera);
			}
			finished = true;
			if (this.onEndEvent != null)
			{
				this.onEndEvent();
			}
		}
	}

	public bool isPlaying()
	{
		if (_videoPlayer != null)
		{
			return _videoPlayer.isPlaying;
		}
		return false;
	}

	public void Stop()
	{
		if (_videoPlayer != null)
		{
			_videoPlayer.Stop();
		}
	}

	public bool HasFinished()
	{
		return finished;
	}

	public void Destroy()
	{
		if (_videoPlayer != null)
		{
			_videoPlayer.Stop();
		}
	}
}
