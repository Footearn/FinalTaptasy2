using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GP_Participant
{
	private string _id;

	private string _playerid;

	private string _HiResImageUrl;

	private string _IconImageUrl;

	private string _DisplayName;

	private GP_ParticipantResult _result;

	private GP_RTM_ParticipantStatus _status = GP_RTM_ParticipantStatus.STATUS_UNRESPONSIVE;

	private Texture2D _SmallPhoto;

	private Texture2D _BigPhoto;

	public Texture2D SmallPhoto
	{
		get
		{
			return _SmallPhoto;
		}
	}

	public Texture2D BigPhoto
	{
		get
		{
			return _BigPhoto;
		}
	}

	public string id
	{
		get
		{
			return _id;
		}
	}

	public string playerId
	{
		get
		{
			return _playerid;
		}
	}

	public string HiResImageUrl
	{
		get
		{
			return _HiResImageUrl;
		}
	}

	public string IconImageUrl
	{
		get
		{
			return _IconImageUrl;
		}
	}

	public string DisplayName
	{
		get
		{
			return _DisplayName;
		}
	}

	public GP_RTM_ParticipantStatus Status
	{
		get
		{
			return _status;
		}
	}

	public GP_ParticipantResult Result
	{
		get
		{
			return _result;
		}
	}

	[method: MethodImpl(32)]
	public event Action<Texture2D> BigPhotoLoaded = delegate
	{
	};

	[method: MethodImpl(32)]
	public event Action<Texture2D> SmallPhotoLoaded = delegate
	{
	};

	public GP_Participant(string uid, string playerUid, string stat, string hiResImg, string IconImg, string Name)
	{
		_id = uid;
		_playerid = playerUid;
		_status = (GP_RTM_ParticipantStatus)Convert.ToInt32(stat);
		_HiResImageUrl = hiResImg;
		_IconImageUrl = IconImg;
		_DisplayName = Name;
	}

	public void SetResult(GP_ParticipantResult r)
	{
		_result = r;
	}

	public void LoadBigPhoto()
	{
		SA_WWWTextureLoader sA_WWWTextureLoader = SA_WWWTextureLoader.Create();
		sA_WWWTextureLoader.OnLoad += HandheBigPhotoLoaed;
		sA_WWWTextureLoader.LoadTexture(_HiResImageUrl);
	}

	public void LoadSmallPhoto()
	{
		SA_WWWTextureLoader sA_WWWTextureLoader = SA_WWWTextureLoader.Create();
		sA_WWWTextureLoader.OnLoad += HandheSmallPhotoLoaed;
		sA_WWWTextureLoader.LoadTexture(_IconImageUrl);
	}

	private void HandheBigPhotoLoaed(Texture2D tex)
	{
		_BigPhoto = tex;
		this.BigPhotoLoaded(_BigPhoto);
	}

	private void HandheSmallPhotoLoaed(Texture2D tex)
	{
		_SmallPhoto = tex;
		this.SmallPhotoLoaded(_SmallPhoto);
	}
}
