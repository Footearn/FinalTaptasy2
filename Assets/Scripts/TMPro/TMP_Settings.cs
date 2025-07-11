using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	[ExecuteInEditMode]
	public class TMP_Settings : ScriptableObject
	{
		public class LineBreakingTable
		{
			public Dictionary<int, char> leadingCharacters;

			public Dictionary<int, char> followingCharacters;
		}

		private static TMP_Settings s_Instance;

		[SerializeField]
		private bool m_enableWordWrapping;

		[SerializeField]
		private bool m_enableKerning;

		[SerializeField]
		private bool m_enableExtraPadding;

		[SerializeField]
		private bool m_enableTintAllSprites;

		[SerializeField]
		private bool m_enableParseEscapeCharacters;

		[SerializeField]
		private int m_missingGlyphCharacter;

		[SerializeField]
		private bool m_warningsDisabled;

		[SerializeField]
		private TMP_FontAsset m_defaultFontAsset;

		[SerializeField]
		private List<TMP_FontAsset> m_fallbackFontAssets;

		[SerializeField]
		private TMP_SpriteAsset m_defaultSpriteAsset;

		[SerializeField]
		private TMP_StyleSheet m_defaultStyleSheet;

		[SerializeField]
		private TextAsset m_leadingCharacters;

		[SerializeField]
		private TextAsset m_followingCharacters;

		[SerializeField]
		private LineBreakingTable m_linebreakingRules;

		public static bool enableWordWrapping
		{
			get
			{
				return instance.m_enableWordWrapping;
			}
		}

		public static bool enableKerning
		{
			get
			{
				return instance.m_enableKerning;
			}
		}

		public static bool enableExtraPadding
		{
			get
			{
				return instance.m_enableExtraPadding;
			}
		}

		public static bool enableTintAllSprites
		{
			get
			{
				return instance.m_enableTintAllSprites;
			}
		}

		public static bool enableParseEscapeCharacters
		{
			get
			{
				return instance.m_enableParseEscapeCharacters;
			}
		}

		public static int missingGlyphCharacter
		{
			get
			{
				return instance.m_missingGlyphCharacter;
			}
		}

		public static bool warningsDisabled
		{
			get
			{
				return instance.m_warningsDisabled;
			}
		}

		public static TMP_FontAsset defaultFontAsset
		{
			get
			{
				return instance.m_defaultFontAsset;
			}
		}

		public static List<TMP_FontAsset> fallbackFontAssets
		{
			get
			{
				return instance.m_fallbackFontAssets;
			}
		}

		public static TMP_SpriteAsset defaultSpriteAsset
		{
			get
			{
				return instance.m_defaultSpriteAsset;
			}
		}

		public static TMP_StyleSheet defaultStyleSheet
		{
			get
			{
				return instance.m_defaultStyleSheet;
			}
		}

		public static TextAsset leadingCharacters
		{
			get
			{
				return instance.m_leadingCharacters;
			}
		}

		public static TextAsset followingCharacters
		{
			get
			{
				return instance.m_followingCharacters;
			}
		}

		public static LineBreakingTable linebreakingRules
		{
			get
			{
				if (instance.m_linebreakingRules == null)
				{
					LoadLinebreakingRules();
				}
				return instance.m_linebreakingRules;
			}
		}

		public static TMP_Settings instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = Resources.Load("TMP Settings") as TMP_Settings;
				}
				return s_Instance;
			}
		}

		public static TMP_Settings LoadDefaultSettings()
		{
			if (s_Instance == null)
			{
				TMP_Settings x = Resources.Load("TMP Settings") as TMP_Settings;
				if (x != null)
				{
					s_Instance = x;
				}
			}
			return s_Instance;
		}

		public static TMP_Settings GetSettings()
		{
			if (instance == null)
			{
				return null;
			}
			return instance;
		}

		public static TMP_FontAsset GetFontAsset()
		{
			if (instance == null)
			{
				return null;
			}
			return instance.m_defaultFontAsset;
		}

		public static TMP_SpriteAsset GetSpriteAsset()
		{
			if (instance == null)
			{
				return null;
			}
			return instance.m_defaultSpriteAsset;
		}

		public static TMP_StyleSheet GetStyleSheet()
		{
			if (instance == null)
			{
				return null;
			}
			return instance.m_defaultStyleSheet;
		}

		public static void LoadLinebreakingRules()
		{
			if (!(instance == null))
			{
				if (s_Instance.m_linebreakingRules == null)
				{
					s_Instance.m_linebreakingRules = new LineBreakingTable();
				}
				s_Instance.m_linebreakingRules.leadingCharacters = GetCharacters(s_Instance.m_leadingCharacters);
				s_Instance.m_linebreakingRules.followingCharacters = GetCharacters(s_Instance.m_followingCharacters);
			}
		}

		private static Dictionary<int, char> GetCharacters(TextAsset file)
		{
			Dictionary<int, char> dictionary = new Dictionary<int, char>();
			string text = file.text;
			foreach (char c in text)
			{
				if (!dictionary.ContainsKey(c))
				{
					dictionary.Add(c, c);
				}
			}
			return dictionary;
		}
	}
}
