using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FB_Score
{
	public string UserId;

	public string UserName;

	public string AppId;

	public string AppName;

	public int value;

	private Dictionary<FB_ProfileImageSize, Texture2D> profileImages = new Dictionary<FB_ProfileImageSize, Texture2D>();

	[method: MethodImpl(32)]
	public event Action<FB_Score> OnProfileImageLoaded = delegate
	{
	};

	public string GetProfileUrl(FB_ProfileImageSize size)
	{
		return "https://graph.facebook.com/" + UserId + "/picture?type=" + size;
	}

	public Texture2D GetProfileImage(FB_ProfileImageSize size)
	{
		if (profileImages.ContainsKey(size))
		{
			return profileImages[size];
		}
		return null;
	}

	public void LoadProfileImage(FB_ProfileImageSize size)
	{
		if (GetProfileImage(size) != null)
		{
			Debug.LogWarning("Profile image already loaded, size: " + size);
			this.OnProfileImageLoaded(this);
		}
		SA_WWWTextureLoader sA_WWWTextureLoader = SA_WWWTextureLoader.Create();
		switch (size)
		{
		case FB_ProfileImageSize.large:
			sA_WWWTextureLoader.OnLoad += OnLargeImageLoaded;
			break;
		case FB_ProfileImageSize.normal:
			sA_WWWTextureLoader.OnLoad += OnNormalImageLoaded;
			break;
		case FB_ProfileImageSize.small:
			sA_WWWTextureLoader.OnLoad += OnSmallImageLoaded;
			break;
		case FB_ProfileImageSize.square:
			sA_WWWTextureLoader.OnLoad += OnSquareImageLoaded;
			break;
		}
		Debug.Log("LOAD IMAGE URL: " + GetProfileUrl(size));
		sA_WWWTextureLoader.LoadTexture(GetProfileUrl(size));
	}

	private void OnSquareImageLoaded(Texture2D image)
	{
		if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.square))
		{
			profileImages.Add(FB_ProfileImageSize.square, image);
		}
		this.OnProfileImageLoaded(this);
	}

	private void OnLargeImageLoaded(Texture2D image)
	{
		if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.large))
		{
			profileImages.Add(FB_ProfileImageSize.large, image);
		}
		this.OnProfileImageLoaded(this);
	}

	private void OnNormalImageLoaded(Texture2D image)
	{
		if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.normal))
		{
			profileImages.Add(FB_ProfileImageSize.normal, image);
		}
		this.OnProfileImageLoaded(this);
	}

	private void OnSmallImageLoaded(Texture2D image)
	{
		if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.small))
		{
			profileImages.Add(FB_ProfileImageSize.small, image);
		}
		this.OnProfileImageLoaded(this);
	}
}
