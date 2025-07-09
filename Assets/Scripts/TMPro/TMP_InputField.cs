using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro
{
	[AddComponentMenu("UI/TextMeshPro - Input Field", 11)]
	public class TMP_InputField : Selectable, ICanvasElement, IEventSystemHandler, IPointerClickHandler, IUpdateSelectedHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, ISubmitHandler
	{
		public enum ContentType
		{
			Standard,
			Autocorrected,
			IntegerNumber,
			DecimalNumber,
			Alphanumeric,
			Name,
			EmailAddress,
			Password,
			Pin,
			Custom
		}

		public enum InputType
		{
			Standard,
			AutoCorrect,
			Password
		}

		public enum CharacterValidation
		{
			None,
			Integer,
			Decimal,
			Alphanumeric,
			Name,
			EmailAddress
		}

		public enum LineType
		{
			SingleLine,
			MultiLineSubmit,
			MultiLineNewline
		}

		[Serializable]
		public class SubmitEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class OnChangeEvent : UnityEvent<string>
		{
		}

		protected enum EditState
		{
			Continue,
			Finish
		}

		public delegate char OnValidateInput(string text, int charIndex, char addedChar);

		private const float kHScrollSpeed = 0.05f;

		private const float kVScrollSpeed = 0.1f;

		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		protected TouchScreenKeyboard m_Keyboard;

		private static readonly char[] kSeparators = new char[3]
		{
			' ',
			'.',
			','
		};

		[SerializeField]
		protected RectTransform m_TextViewport;

		[SerializeField]
		protected TMP_Text m_TextComponent;

		protected RectTransform m_TextComponentRectTransform;

		[SerializeField]
		protected Graphic m_Placeholder;

		[SerializeField]
		private ContentType m_ContentType;

		[SerializeField]
		private InputType m_InputType;

		[SerializeField]
		private char m_AsteriskChar = '*';

		[SerializeField]
		private TouchScreenKeyboardType m_KeyboardType;

		[SerializeField]
		private LineType m_LineType;

		[SerializeField]
		private bool m_HideMobileInput;

		[SerializeField]
		private CharacterValidation m_CharacterValidation;

		[SerializeField]
		private int m_CharacterLimit;

		[SerializeField]
		private SubmitEvent m_OnEndEdit = new SubmitEvent();

		[SerializeField]
		private OnChangeEvent m_OnValueChanged = new OnChangeEvent();

		[SerializeField]
		private OnValidateInput m_OnValidateInput;

		[SerializeField]
		private Color m_CaretColor = new Color(10f / 51f, 10f / 51f, 10f / 51f, 1f);

		[SerializeField]
		private bool m_CustomCaretColor;

		[SerializeField]
		private Color m_SelectionColor = new Color(56f / 85f, 206f / 255f, 1f, 64f / 85f);

		[SerializeField]
		protected string m_Text = string.Empty;

		[Range(0f, 4f)]
		[SerializeField]
		private float m_CaretBlinkRate = 0.85f;

		[Range(1f, 5f)]
		[SerializeField]
		private int m_CaretWidth = 1;

		[SerializeField]
		private bool m_ReadOnly;

		protected int m_CaretPosition;

		protected int m_CaretSelectPosition;

		private RectTransform caretRectTrans;

		protected UIVertex[] m_CursorVerts;

		private CanvasRenderer m_CachedInputRenderer;

		[NonSerialized]
		protected Mesh m_Mesh;

		private bool m_AllowInput;

		private bool m_ShouldActivateNextUpdate;

		private bool m_UpdateDrag;

		private bool m_DragPositionOutOfBounds;

		protected bool m_CaretVisible;

		private Coroutine m_BlinkCoroutine;

		private float m_BlinkStartTime;

		protected int m_DrawStart;

		protected int m_DrawEnd;

		private Coroutine m_DragCoroutine;

		private string m_OriginalText = string.Empty;

		private bool m_WasCanceled;

		private bool m_HasDoneFocusTransition;

		private Event m_ProcessingEvent = new Event();

		protected Mesh mesh
		{
			get
			{
				if (m_Mesh == null)
				{
					m_Mesh = new Mesh();
				}
				return m_Mesh;
			}
		}

		public bool shouldHideMobileInput
		{
			get
			{
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
				case RuntimePlatform.Android:
				case RuntimePlatform.BlackBerryPlayer:
				case RuntimePlatform.TizenPlayer:
				case RuntimePlatform.tvOS:
					return m_HideMobileInput;
				default:
					return true;
				}
			}
			set
			{
				SetPropertyUtility.SetStruct(ref m_HideMobileInput, value);
			}
		}

		private bool shouldActivateOnSelect
		{
			get
			{
				return Application.platform != RuntimePlatform.tvOS;
			}
		}

		public string text
		{
			get
			{
				if (m_Keyboard != null && m_Keyboard.active && !InPlaceEditing() && EventSystem.current.currentSelectedGameObject == base.gameObject)
				{
					return m_Keyboard.text;
				}
				return m_Text;
			}
			set
			{
				if (!(text == value))
				{
					m_Text = value;
					if (m_Keyboard != null)
					{
						m_Keyboard.text = m_Text;
					}
					if (m_CaretPosition > m_Text.Length)
					{
						m_CaretPosition = (m_CaretSelectPosition = m_Text.Length);
					}
					SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		public bool isFocused
		{
			get
			{
				return m_AllowInput;
			}
		}

		public float caretBlinkRate
		{
			get
			{
				return m_CaretBlinkRate;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_CaretBlinkRate, value) && m_AllowInput)
				{
					SetCaretActive();
				}
			}
		}

		public int caretWidth
		{
			get
			{
				return m_CaretWidth;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_CaretWidth, value))
				{
					MarkGeometryAsDirty();
				}
			}
		}

		public RectTransform textViewport
		{
			get
			{
				return m_TextViewport;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_TextViewport, value);
			}
		}

		public TMP_Text textComponent
		{
			get
			{
				return m_TextComponent;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_TextComponent, value);
			}
		}

		public Graphic placeholder
		{
			get
			{
				return m_Placeholder;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_Placeholder, value);
			}
		}

		public Color caretColor
		{
			get
			{
				return (!customCaretColor) ? textComponent.color : m_CaretColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref m_CaretColor, value))
				{
					MarkGeometryAsDirty();
				}
			}
		}

		public bool customCaretColor
		{
			get
			{
				return m_CustomCaretColor;
			}
			set
			{
				if (m_CustomCaretColor != value)
				{
					m_CustomCaretColor = value;
					MarkGeometryAsDirty();
				}
			}
		}

		public Color selectionColor
		{
			get
			{
				return m_SelectionColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref m_SelectionColor, value))
				{
					MarkGeometryAsDirty();
				}
			}
		}

		public SubmitEvent onEndEdit
		{
			get
			{
				return m_OnEndEdit;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnEndEdit, value);
			}
		}

		[Obsolete("onValueChange has been renamed to onValueChanged")]
		public OnChangeEvent onValueChange
		{
			get
			{
				return onValueChanged;
			}
			set
			{
				onValueChanged = value;
			}
		}

		public OnChangeEvent onValueChanged
		{
			get
			{
				return m_OnValueChanged;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnValueChanged, value);
			}
		}

		public OnValidateInput onValidateInput
		{
			get
			{
				return m_OnValidateInput;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnValidateInput, value);
			}
		}

		public int characterLimit
		{
			get
			{
				return m_CharacterLimit;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_CharacterLimit, Math.Max(0, value)))
				{
					UpdateLabel();
				}
			}
		}

		public ContentType contentType
		{
			get
			{
				return m_ContentType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_ContentType, value))
				{
					EnforceContentType();
				}
			}
		}

		public LineType lineType
		{
			get
			{
				return m_LineType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_LineType, value))
				{
					SetTextComponentWrapMode();
				}
				SetToCustomIfContentTypeIsNot(ContentType.Standard, ContentType.Autocorrected);
			}
		}

		public InputType inputType
		{
			get
			{
				return m_InputType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_InputType, value))
				{
					SetToCustom();
				}
			}
		}

		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return m_KeyboardType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_KeyboardType, value))
				{
					SetToCustom();
				}
			}
		}

		public CharacterValidation characterValidation
		{
			get
			{
				return m_CharacterValidation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_CharacterValidation, value))
				{
					SetToCustom();
				}
			}
		}

		public bool readOnly
		{
			get
			{
				return m_ReadOnly;
			}
			set
			{
				m_ReadOnly = value;
			}
		}

		public bool multiLine
		{
			get
			{
				return m_LineType == LineType.MultiLineNewline || lineType == LineType.MultiLineSubmit;
			}
		}

		public char asteriskChar
		{
			get
			{
				return m_AsteriskChar;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_AsteriskChar, value))
				{
					UpdateLabel();
				}
			}
		}

		public bool wasCanceled
		{
			get
			{
				return m_WasCanceled;
			}
		}

		protected int caretPositionInternal
		{
			get
			{
				return m_CaretPosition + Input.compositionString.Length;
			}
			set
			{
				m_CaretPosition = value;
				ClampPos(ref m_CaretPosition);
			}
		}

		protected int caretSelectPositionInternal
		{
			get
			{
				return m_CaretSelectPosition + Input.compositionString.Length;
			}
			set
			{
				m_CaretSelectPosition = value;
				ClampPos(ref m_CaretSelectPosition);
			}
		}

		private bool hasSelection
		{
			get
			{
				return caretPositionInternal != caretSelectPositionInternal;
			}
		}

		public int caretPosition
		{
			get
			{
				return m_CaretSelectPosition + Input.compositionString.Length;
			}
			set
			{
				selectionAnchorPosition = value;
				selectionFocusPosition = value;
			}
		}

		public int selectionAnchorPosition
		{
			get
			{
				return m_CaretPosition + Input.compositionString.Length;
			}
			set
			{
				if (Input.compositionString.Length == 0)
				{
					m_CaretPosition = value;
					ClampPos(ref m_CaretPosition);
				}
			}
		}

		public int selectionFocusPosition
		{
			get
			{
				return m_CaretSelectPosition + Input.compositionString.Length;
			}
			set
			{
				if (Input.compositionString.Length == 0)
				{
					m_CaretSelectPosition = value;
					ClampPos(ref m_CaretSelectPosition);
				}
			}
		}

		private static string clipboard
		{
			get
			{
				return GUIUtility.systemCopyBuffer;
			}
			set
			{
				GUIUtility.systemCopyBuffer = value;
			}
		}

		protected TMP_InputField()
		{
		}

		protected void ClampPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
			}
			else if (pos > text.Length)
			{
				pos = text.Length;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (m_Text == null)
			{
				m_Text = string.Empty;
			}
			m_DrawStart = 0;
			m_DrawEnd = m_Text.Length;
			if (m_CachedInputRenderer != null)
			{
				m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
			}
			if (m_TextComponent != null)
			{
				m_TextComponent.RegisterDirtyVerticesCallback(MarkGeometryAsDirty);
				m_TextComponent.RegisterDirtyVerticesCallback(UpdateLabel);
				UpdateLabel();
			}
		}

		protected override void OnDisable()
		{
			m_BlinkCoroutine = null;
			DeactivateInputField();
			if (m_TextComponent != null)
			{
				m_TextComponent.UnregisterDirtyVerticesCallback(MarkGeometryAsDirty);
				m_TextComponent.UnregisterDirtyVerticesCallback(UpdateLabel);
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (m_CachedInputRenderer != null)
			{
				m_CachedInputRenderer.Clear();
			}
			if (m_Mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(m_Mesh);
			}
			m_Mesh = null;
			base.OnDisable();
		}

		private IEnumerator CaretBlink()
		{
			m_CaretVisible = true;
			yield return null;
			while (isFocused && m_CaretBlinkRate > 0f)
			{
				float blinkPeriod = 1f / m_CaretBlinkRate;
				bool blinkState = (Time.unscaledTime - m_BlinkStartTime) % blinkPeriod < blinkPeriod / 2f;
				if (m_CaretVisible != blinkState)
				{
					m_CaretVisible = blinkState;
					if (!hasSelection)
					{
						MarkGeometryAsDirty();
					}
				}
				yield return null;
			}
			m_BlinkCoroutine = null;
		}

		private void SetCaretVisible()
		{
			if (m_AllowInput)
			{
				m_CaretVisible = true;
				m_BlinkStartTime = Time.unscaledTime;
				SetCaretActive();
			}
		}

		private void SetCaretActive()
		{
			if (!m_AllowInput)
			{
				return;
			}
			if (m_CaretBlinkRate > 0f)
			{
				if (m_BlinkCoroutine == null)
				{
					m_BlinkCoroutine = StartCoroutine(CaretBlink());
				}
			}
			else
			{
				m_CaretVisible = true;
			}
		}

		protected void OnFocus()
		{
			SelectAll();
		}

		protected void SelectAll()
		{
			caretPositionInternal = text.Length;
			caretSelectPositionInternal = 0;
		}

		public void MoveTextEnd(bool shift)
		{
			int length = text.Length;
			if (shift)
			{
				caretSelectPositionInternal = length;
			}
			else
			{
				caretPositionInternal = length;
				caretSelectPositionInternal = caretPositionInternal;
			}
			UpdateLabel();
		}

		public void MoveTextStart(bool shift)
		{
			int num = 0;
			if (shift)
			{
				caretSelectPositionInternal = num;
			}
			else
			{
				caretPositionInternal = num;
				caretSelectPositionInternal = caretPositionInternal;
			}
			UpdateLabel();
		}

		private bool InPlaceEditing()
		{
			return !TouchScreenKeyboard.isSupported;
		}

		protected virtual void LateUpdate()
		{
			if (m_ShouldActivateNextUpdate)
			{
				if (!isFocused)
				{
					ActivateInputFieldInternal();
					m_ShouldActivateNextUpdate = false;
					return;
				}
				m_ShouldActivateNextUpdate = false;
			}
			if (InPlaceEditing() || !isFocused)
			{
				return;
			}
			AssignPositioningIfNeeded();
			if (m_Keyboard == null || !m_Keyboard.active)
			{
				if (m_Keyboard != null)
				{
					if (!m_ReadOnly)
					{
						this.text = m_Keyboard.text;
					}
					if (m_Keyboard.wasCanceled)
					{
						m_WasCanceled = true;
					}
				}
				OnDeselect(null);
				return;
			}
			string text = m_Keyboard.text;
			if (m_Text != text)
			{
				if (m_ReadOnly)
				{
					m_Keyboard.text = m_Text;
				}
				else
				{
					m_Text = string.Empty;
					for (int i = 0; i < text.Length; i++)
					{
						char c = text[i];
						if (c == '\r' || c == '\u0003')
						{
							c = '\n';
						}
						if (onValidateInput != null)
						{
							c = onValidateInput(m_Text, m_Text.Length, c);
						}
						else if (characterValidation != 0)
						{
							c = Validate(m_Text, m_Text.Length, c);
						}
						if (lineType == LineType.MultiLineSubmit && c == '\n')
						{
							m_Keyboard.text = m_Text;
							OnDeselect(null);
							return;
						}
						if (c != 0)
						{
							m_Text += c;
						}
					}
					if (characterLimit > 0 && m_Text.Length > characterLimit)
					{
						m_Text = m_Text.Substring(0, characterLimit);
					}
					int num2 = (caretPositionInternal = (caretSelectPositionInternal = m_Text.Length));
					if (m_Text != text)
					{
						m_Keyboard.text = m_Text;
					}
					SendOnValueChangedAndUpdateLabel();
				}
			}
			if (m_Keyboard.done)
			{
				if (m_Keyboard.wasCanceled)
				{
					m_WasCanceled = true;
				}
				OnDeselect(null);
			}
		}

		protected int GetCharacterIndexFromPosition(Vector2 pos)
		{
			return 0;
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left && m_TextComponent != null && m_Keyboard == null;
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (MayDrag(eventData))
			{
				m_UpdateDrag = true;
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (MayDrag(eventData))
			{
				caretSelectPositionInternal = TMP_TextUtilities.GetCursorIndexFromPosition(m_TextComponent, eventData.position, eventData.pressEventCamera);
				MarkGeometryAsDirty();
				m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(textViewport, eventData.position, eventData.pressEventCamera);
				if (m_DragPositionOutOfBounds && m_DragCoroutine == null)
				{
					m_DragCoroutine = StartCoroutine(MouseDragOutsideRect(eventData));
				}
				eventData.Use();
			}
		}

		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			while (m_UpdateDrag && m_DragPositionOutOfBounds)
			{
				Vector2 localMousePos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(textViewport, eventData.position, eventData.pressEventCamera, out localMousePos);
				Rect rect = textViewport.rect;
				if (multiLine)
				{
					if (localMousePos.y > rect.yMax)
					{
						MoveUp(true, true);
					}
					else if (localMousePos.y < rect.yMin)
					{
						MoveDown(true, true);
					}
				}
				else if (localMousePos.x < rect.xMin)
				{
					MoveLeft(true, false);
				}
				else if (localMousePos.x > rect.xMax)
				{
					MoveRight(true, false);
				}
				UpdateLabel();
				float delay = ((!multiLine) ? 0.05f : 0.1f);
				yield return new WaitForSeconds(delay);
			}
			m_DragCoroutine = null;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (MayDrag(eventData))
			{
				m_UpdateDrag = false;
			}
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
			bool allowInput = m_AllowInput;
			base.OnPointerDown(eventData);
			if (!InPlaceEditing() && (m_Keyboard == null || !m_Keyboard.active))
			{
				OnSelect(eventData);
				return;
			}
			if (allowInput)
			{
				int num2 = (caretSelectPositionInternal = (caretPositionInternal = TMP_TextUtilities.GetCursorIndexFromPosition(m_TextComponent, eventData.position, eventData.pressEventCamera)));
			}
			UpdateLabel();
			eventData.Use();
		}

		protected EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.modifiers;
			RuntimePlatform platform = Application.platform;
			//TODO 更改
			//bool flag = ((platform != 0 && platform != RuntimePlatform.OSXPlayer && platform != RuntimePlatform.OSXWebPlayer) ? ((modifiers & EventModifiers.Control) != 0) : ((modifiers & EventModifiers.Command) != 0));
			bool flag = ((platform != 0 && platform != RuntimePlatform.OSXPlayer) ? ((modifiers & EventModifiers.Control) != 0) : ((modifiers & EventModifiers.Command) != 0));
			bool flag2 = (modifiers & EventModifiers.Shift) != 0;
			bool flag3 = (modifiers & EventModifiers.Alt) != 0;
			bool flag4 = flag && !flag3 && !flag2;
			switch (evt.keyCode)
			{
			case KeyCode.Backspace:
				Backspace();
				return EditState.Continue;
			case KeyCode.Delete:
				ForwardSpace();
				return EditState.Continue;
			case KeyCode.Home:
				MoveTextStart(flag2);
				return EditState.Continue;
			case KeyCode.End:
				MoveTextEnd(flag2);
				return EditState.Continue;
			case KeyCode.A:
				if (flag4)
				{
					SelectAll();
					return EditState.Continue;
				}
				break;
			case KeyCode.C:
				if (flag4)
				{
					if (inputType != InputType.Password)
					{
						clipboard = GetSelectedString();
					}
					else
					{
						clipboard = string.Empty;
					}
					return EditState.Continue;
				}
				break;
			case KeyCode.V:
				if (flag4)
				{
					Append(clipboard);
					return EditState.Continue;
				}
				break;
			case KeyCode.X:
				if (flag4)
				{
					if (inputType != InputType.Password)
					{
						clipboard = GetSelectedString();
					}
					else
					{
						clipboard = string.Empty;
					}
					Delete();
					SendOnValueChangedAndUpdateLabel();
					return EditState.Continue;
				}
				break;
			case KeyCode.LeftArrow:
				MoveLeft(flag2, flag);
				return EditState.Continue;
			case KeyCode.RightArrow:
				MoveRight(flag2, flag);
				return EditState.Continue;
			case KeyCode.UpArrow:
				MoveUp(flag2);
				return EditState.Continue;
			case KeyCode.DownArrow:
				MoveDown(flag2);
				return EditState.Continue;
			case KeyCode.Return:
			case KeyCode.KeypadEnter:
				if (lineType != LineType.MultiLineNewline)
				{
					return EditState.Finish;
				}
				break;
			case KeyCode.Escape:
				m_WasCanceled = true;
				return EditState.Finish;
			}
			char c = evt.character;
			if (!multiLine && (c == '\t' || c == '\r' || c == '\n'))
			{
				return EditState.Continue;
			}
			if (c == '\r' || c == '\u0003')
			{
				c = '\n';
			}
			if (IsValidChar(c))
			{
				Append(c);
			}
			if (c == '\0' && Input.compositionString.Length > 0)
			{
				UpdateLabel();
			}
			return EditState.Continue;
		}

		private bool IsValidChar(char c)
		{
			switch (c)
			{
			case '\u007f':
				return false;
			case '\t':
			case '\n':
				return true;
			default:
				return m_TextComponent.font.HasCharacter(c);
			}
		}

		public void ProcessEvent(Event e)
		{
			KeyPressed(e);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (!isFocused)
			{
				return;
			}
			bool flag = false;
			while (Event.PopEvent(m_ProcessingEvent))
			{
				if (m_ProcessingEvent.rawType == EventType.KeyDown)
				{
					flag = true;
					EditState editState = KeyPressed(m_ProcessingEvent);
					if (editState == EditState.Finish)
					{
						DeactivateInputField();
						break;
					}
				}
				EventType type = m_ProcessingEvent.type;
				if (type == EventType.ValidateCommand || type == EventType.ExecuteCommand)
				{
					switch (m_ProcessingEvent.commandName)
					{
					case "SelectAll":
						SelectAll();
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				UpdateLabel();
			}
			eventData.Use();
		}

		private string GetSelectedString()
		{
			if (!hasSelection)
			{
				return string.Empty;
			}
			int num = caretPositionInternal;
			int num2 = caretSelectPositionInternal;
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return text.Substring(num, num2 - num);
		}

		private int FindtNextWordBegin()
		{
			if (caretSelectPositionInternal + 1 >= text.Length)
			{
				return text.Length;
			}
			int num = text.IndexOfAny(kSeparators, caretSelectPositionInternal + 1);
			if (num == -1)
			{
				return text.Length;
			}
			return num + 1;
		}

		private void MoveRight(bool shift, bool ctrl)
		{
			if (hasSelection && !shift)
			{
				int num3 = (caretPositionInternal = (caretSelectPositionInternal = Mathf.Max(caretPositionInternal, caretSelectPositionInternal)));
				return;
			}
			int num4 = ((!ctrl) ? (caretSelectPositionInternal + 1) : FindtNextWordBegin());
			if (shift)
			{
				caretSelectPositionInternal = num4;
			}
			else
			{
				int num3 = (caretSelectPositionInternal = (caretPositionInternal = num4));
			}
		}

		private int FindtPrevWordBegin()
		{
			if (caretSelectPositionInternal - 2 < 0)
			{
				return 0;
			}
			int num = text.LastIndexOfAny(kSeparators, caretSelectPositionInternal - 2);
			if (num == -1)
			{
				return 0;
			}
			return num + 1;
		}

		private void MoveLeft(bool shift, bool ctrl)
		{
			if (hasSelection && !shift)
			{
				int num3 = (caretPositionInternal = (caretSelectPositionInternal = Mathf.Min(caretPositionInternal, caretSelectPositionInternal)));
				return;
			}
			int num4 = ((!ctrl) ? (caretSelectPositionInternal - 1) : FindtPrevWordBegin());
			if (shift)
			{
				caretSelectPositionInternal = num4;
			}
			else
			{
				int num3 = (caretSelectPositionInternal = (caretPositionInternal = num4));
			}
		}

		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= m_TextComponent.textInfo.characterCount)
			{
				originalPos--;
			}
			TMP_CharacterInfo tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = tMP_CharacterInfo.lineNumber;
			if (lineNumber - 1 < 0)
			{
				return (!goToFirstChar) ? originalPos : 0;
			}
			int num = m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex - 1;
			for (int i = m_TextComponent.textInfo.lineInfo[lineNumber - 1].firstCharacterIndex; i < num; i++)
			{
				if (m_TextComponent.textInfo.characterInfo[i].origin >= tMP_CharacterInfo.origin)
				{
					return i;
				}
			}
			return num;
		}

		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos >= m_TextComponent.textInfo.characterCount)
			{
				return text.Length;
			}
			TMP_CharacterInfo tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = tMP_CharacterInfo.lineNumber;
			if (lineNumber + 1 >= m_TextComponent.textInfo.lineCount)
			{
				return (!goToLastChar) ? originalPos : text.Length;
			}
			int lastCharacterIndex = m_TextComponent.textInfo.lineInfo[lineNumber + 1].lastCharacterIndex;
			for (int i = m_TextComponent.textInfo.lineInfo[lineNumber + 1].firstCharacterIndex; i < lastCharacterIndex; i++)
			{
				if (m_TextComponent.textInfo.characterInfo[i].origin >= tMP_CharacterInfo.origin)
				{
					return i;
				}
			}
			return lastCharacterIndex + 1;
		}

		private void MoveDown(bool shift)
		{
			MoveDown(shift, true);
		}

		private void MoveDown(bool shift, bool goToLastChar)
		{
			if (hasSelection && !shift)
			{
				int num3 = (caretPositionInternal = (caretSelectPositionInternal = Mathf.Max(caretPositionInternal, caretSelectPositionInternal)));
			}
			int num4 = ((!multiLine) ? text.Length : LineDownCharacterPosition(caretSelectPositionInternal, goToLastChar));
			if (shift)
			{
				caretSelectPositionInternal = num4;
			}
			else
			{
				int num3 = (caretPositionInternal = (caretSelectPositionInternal = num4));
			}
		}

		private void MoveUp(bool shift)
		{
			MoveUp(shift, true);
		}

		private void MoveUp(bool shift, bool goToFirstChar)
		{
			if (hasSelection && !shift)
			{
				int num3 = (caretPositionInternal = (caretSelectPositionInternal = Mathf.Min(caretPositionInternal, caretSelectPositionInternal)));
			}
			int num4 = (multiLine ? LineUpCharacterPosition(caretSelectPositionInternal, goToFirstChar) : 0);
			if (shift)
			{
				caretSelectPositionInternal = num4;
			}
			else
			{
				int num3 = (caretSelectPositionInternal = (caretPositionInternal = num4));
			}
		}

		private void Delete()
		{
			if (!m_ReadOnly && caretPositionInternal != caretSelectPositionInternal)
			{
				if (caretPositionInternal < caretSelectPositionInternal)
				{
					m_Text = text.Substring(0, caretPositionInternal) + text.Substring(caretSelectPositionInternal, text.Length - caretSelectPositionInternal);
					caretSelectPositionInternal = caretPositionInternal;
				}
				else
				{
					m_Text = text.Substring(0, caretSelectPositionInternal) + text.Substring(caretPositionInternal, text.Length - caretPositionInternal);
					caretPositionInternal = caretSelectPositionInternal;
				}
			}
		}

		private void ForwardSpace()
		{
			if (!m_ReadOnly)
			{
				if (hasSelection)
				{
					Delete();
					SendOnValueChangedAndUpdateLabel();
				}
				else if (caretPositionInternal < text.Length)
				{
					m_Text = text.Remove(caretPositionInternal, 1);
					SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		private void Backspace()
		{
			if (!m_ReadOnly)
			{
				if (hasSelection)
				{
					Delete();
					SendOnValueChangedAndUpdateLabel();
				}
				else if (caretPositionInternal > 0)
				{
					m_Text = text.Remove(caretPositionInternal - 1, 1);
					caretSelectPositionInternal = --caretPositionInternal;
					SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		private void Insert(char c)
		{
			if (!m_ReadOnly)
			{
				string text = c.ToString();
				Delete();
				if (characterLimit <= 0 || this.text.Length < characterLimit)
				{
					m_Text = this.text.Insert(m_CaretPosition, text);
					caretSelectPositionInternal = (caretPositionInternal += text.Length);
					SendOnValueChanged();
				}
			}
		}

		private void SendOnValueChangedAndUpdateLabel()
		{
			SendOnValueChanged();
			UpdateLabel();
		}

		private void SendOnValueChanged()
		{
			if (onValueChanged != null)
			{
				onValueChanged.Invoke(text);
			}
		}

		protected void SendOnSubmit()
		{
			if (onEndEdit != null)
			{
				onEndEdit.Invoke(m_Text);
			}
		}

		protected virtual void Append(string input)
		{
			if (m_ReadOnly || !InPlaceEditing())
			{
				return;
			}
			int i = 0;
			for (int length = input.Length; i < length; i++)
			{
				char c = input[i];
				if (c >= ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\n')
				{
					Append(c);
				}
			}
		}

		protected virtual void Append(char input)
		{
			if (!m_ReadOnly && InPlaceEditing())
			{
				if (onValidateInput != null)
				{
					input = onValidateInput(text, caretPositionInternal, input);
				}
				else if (characterValidation != 0)
				{
					input = Validate(text, caretPositionInternal, input);
				}
				if (input != 0)
				{
					Insert(input);
				}
			}
		}

		protected void UpdateLabel()
		{
			if (m_TextComponent != null && m_TextComponent.font != null)
			{
				string text = ((Input.compositionString.Length <= 0) ? this.text : (this.text.Substring(0, m_CaretPosition) + Input.compositionString + this.text.Substring(m_CaretPosition)));
				string str = ((inputType != InputType.Password) ? text : new string(asteriskChar, text.Length));
				bool flag = string.IsNullOrEmpty(text);
				if (m_Placeholder != null)
				{
					m_Placeholder.enabled = flag && !isFocused;
				}
				if (!m_AllowInput)
				{
					m_DrawStart = 0;
					m_DrawEnd = m_Text.Length;
				}
				if (!flag)
				{
					SetCaretVisible();
				}
				m_TextComponent.text = str + "\u200b";
				MarkGeometryAsDirty();
			}
		}

		private bool IsSelectionVisible()
		{
			if (m_DrawStart > caretPositionInternal || m_DrawStart > caretSelectPositionInternal)
			{
				return false;
			}
			if (m_DrawEnd < caretPositionInternal || m_DrawEnd < caretSelectPositionInternal)
			{
				return false;
			}
			return true;
		}

		private static int GetLineStartPosition(TextGenerator gen, int line)
		{
			line = Mathf.Clamp(line, 0, gen.lines.Count - 1);
			return gen.lines[line].startCharIdx;
		}

		private static int GetLineEndPosition(TextGenerator gen, int line)
		{
			line = Mathf.Max(line, 0);
			if (line + 1 < gen.lines.Count)
			{
				return gen.lines[line + 1].startCharIdx;
			}
			return gen.characterCountVisible;
		}

		private int GetCaretPositionFromInternalStringIndex(int stringIndex)
		{
			int characterCount = m_TextComponent.textInfo.characterCount;
			for (int i = 0; i < characterCount; i++)
			{
				if (m_TextComponent.textInfo.characterInfo[i].index >= stringIndex)
				{
					return i;
				}
			}
			return characterCount;
		}

		private void SetDrawRangeToContainCaretPosition(int caretPos)
		{
		}

		public void ForceLabelUpdate()
		{
			UpdateLabel();
		}

		private void MarkGeometryAsDirty()
		{
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		public virtual void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.LatePreRender)
			{
				UpdateGeometry();
			}
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		private void UpdateGeometry()
		{
			if (shouldHideMobileInput)
			{
				if (m_CachedInputRenderer == null && m_TextComponent != null)
				{
					GameObject gameObject = new GameObject(base.transform.name + " Input Caret");
					gameObject.hideFlags = HideFlags.DontSave;
					gameObject.transform.SetParent(m_TextComponent.transform.parent);
					gameObject.transform.SetAsFirstSibling();
					gameObject.layer = base.gameObject.layer;
					caretRectTrans = gameObject.AddComponent<RectTransform>();
					m_CachedInputRenderer = gameObject.AddComponent<CanvasRenderer>();
					m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
					gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
					AssignPositioningIfNeeded();
				}
				if (!(m_CachedInputRenderer == null))
				{
					OnFillVBO(mesh);
					m_CachedInputRenderer.SetMesh(mesh);
				}
			}
		}

		private void AssignPositioningIfNeeded()
		{
			if (m_TextComponent != null && caretRectTrans != null && (caretRectTrans.localPosition != m_TextComponent.rectTransform.localPosition || caretRectTrans.localRotation != m_TextComponent.rectTransform.localRotation || caretRectTrans.localScale != m_TextComponent.rectTransform.localScale || caretRectTrans.anchorMin != m_TextComponent.rectTransform.anchorMin || caretRectTrans.anchorMax != m_TextComponent.rectTransform.anchorMax || caretRectTrans.anchoredPosition != m_TextComponent.rectTransform.anchoredPosition || caretRectTrans.sizeDelta != m_TextComponent.rectTransform.sizeDelta || caretRectTrans.pivot != m_TextComponent.rectTransform.pivot))
			{
				caretRectTrans.localPosition = m_TextComponent.rectTransform.localPosition;
				caretRectTrans.localRotation = m_TextComponent.rectTransform.localRotation;
				caretRectTrans.localScale = m_TextComponent.rectTransform.localScale;
				caretRectTrans.anchorMin = m_TextComponent.rectTransform.anchorMin;
				caretRectTrans.anchorMax = m_TextComponent.rectTransform.anchorMax;
				caretRectTrans.anchoredPosition = m_TextComponent.rectTransform.anchoredPosition;
				caretRectTrans.sizeDelta = m_TextComponent.rectTransform.sizeDelta;
				caretRectTrans.pivot = m_TextComponent.rectTransform.pivot;
			}
		}

		private void OnFillVBO(Mesh vbo)
		{
			using (VertexHelper vertexHelper = new VertexHelper())
			{
				if (!isFocused)
				{
					vertexHelper.FillMesh(vbo);
					return;
				}
				if (!hasSelection)
				{
					GenerateCaret(vertexHelper, Vector2.zero);
				}
				else
				{
					GenerateHightlight(vertexHelper, Vector2.zero);
				}
				vertexHelper.FillMesh(vbo);
			}
		}

		private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
		{
			if (!m_CaretVisible)
			{
				return;
			}
			if (m_CursorVerts == null)
			{
				CreateCursorVerts();
			}
			float num = m_CaretWidth;
			int characterCount = m_TextComponent.textInfo.characterCount;
			Vector2 vector = Vector2.zero;
			float num2 = 0f;
			TMP_CharacterInfo tMP_CharacterInfo;
			if (caretPositionInternal == 0)
			{
				tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[0];
				vector = new Vector2(tMP_CharacterInfo.origin, tMP_CharacterInfo.descender);
				num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
			}
			else if (caretPositionInternal < characterCount)
			{
				tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[caretPositionInternal];
				vector = new Vector2(tMP_CharacterInfo.origin, tMP_CharacterInfo.descender);
				num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
			}
			else
			{
				tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[characterCount - 1];
				vector = new Vector2(tMP_CharacterInfo.xAdvance, tMP_CharacterInfo.descender);
				num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
			}
			if (!(num2 > m_TextComponent.rectTransform.rect.height))
			{
				AdjustRectTransformRelativeToViewport(vector, num2, tMP_CharacterInfo.isVisible);
				vector.x = Mathf.Min(m_TextComponent.rectTransform.position.x + vector.x, m_TextViewport.position.x + m_TextViewport.rect.xMax) - m_TextComponent.rectTransform.position.x;
				for (int i = 0; i < m_CursorVerts.Length; i++)
				{
					m_CursorVerts[i].color = caretColor;
				}
				m_CursorVerts[0].position = new Vector3(vector.x, vector.y, 0f);
				m_CursorVerts[1].position = new Vector3(vector.x, vector.y + num2, 0f);
				m_CursorVerts[2].position = new Vector3(vector.x + num, vector.y + num2, 0f);
				m_CursorVerts[3].position = new Vector3(vector.x + num, vector.y, 0f);
				vbo.AddUIVertexQuad(m_CursorVerts);
				int height = Screen.height;
				vector.y = (float)height - vector.y;
				Input.compositionCursorPos = vector;
			}
		}

		private void CreateCursorVerts()
		{
			m_CursorVerts = new UIVertex[4];
			for (int i = 0; i < m_CursorVerts.Length; i++)
			{
				m_CursorVerts[i] = UIVertex.simpleVert;
				m_CursorVerts[i].uv0 = Vector2.zero;
			}
		}

		private void AdjustRectTransformRelativeToViewport(Vector2 startPosition, float height, bool isCharVisible)
		{
			float num = m_TextViewport.position.x + m_TextViewport.rect.xMax - (m_TextComponent.rectTransform.position.x + startPosition.x);
			if (num < 0f && (!multiLine || (multiLine && isCharVisible)))
			{
				m_TextComponent.rectTransform.position += new Vector3(num, 0f, 0f);
				AssignPositioningIfNeeded();
			}
			float num2 = m_TextComponent.rectTransform.position.x + startPosition.x - (m_TextViewport.position.x + m_TextViewport.rect.xMin);
			if (num2 < 0f)
			{
				m_TextComponent.rectTransform.position += new Vector3(0f - num2, 0f, 0f);
				AssignPositioningIfNeeded();
			}
			float num3 = m_TextViewport.position.y + m_TextViewport.rect.yMax - (m_TextComponent.rectTransform.position.y + startPosition.y + height);
			if (num3 < 0f)
			{
				m_TextComponent.rectTransform.position += new Vector3(0f, num3, 0f);
				AssignPositioningIfNeeded();
			}
			float num4 = m_TextComponent.rectTransform.position.y + startPosition.y - (m_TextViewport.position.y + m_TextViewport.rect.yMin);
			if (num4 < 0f)
			{
				int lineNumber = m_TextComponent.textInfo.characterInfo[Mathf.Max(0, caretPositionInternal - 1)].lineNumber;
				num4 = m_TextComponent.textInfo.lineInfo[lineNumber].lineHeight;
				m_TextComponent.rectTransform.position += new Vector3(0f, num4, 0f);
				AssignPositioningIfNeeded();
			}
		}

		private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
		{
			TMP_TextInfo textInfo = m_TextComponent.textInfo;
			float num = 0f;
			Vector2 startPosition;
			if (caretSelectPositionInternal < textInfo.characterCount)
			{
				startPosition = new Vector2(textInfo.characterInfo[caretSelectPositionInternal].origin, textInfo.characterInfo[caretSelectPositionInternal].descender);
				num = textInfo.characterInfo[caretSelectPositionInternal].ascender - textInfo.characterInfo[caretSelectPositionInternal].descender;
			}
			else
			{
				startPosition = new Vector2(textInfo.characterInfo[caretSelectPositionInternal - 1].xAdvance, textInfo.characterInfo[caretSelectPositionInternal - 1].descender);
				num = textInfo.characterInfo[caretSelectPositionInternal - 1].ascender - textInfo.characterInfo[caretSelectPositionInternal - 1].descender;
			}
			AdjustRectTransformRelativeToViewport(startPosition, num, true);
			int num2 = Mathf.Max(0, caretPositionInternal);
			int num3 = Mathf.Max(0, caretSelectPositionInternal);
			if (num2 > num3)
			{
				int num4 = num2;
				num2 = num3;
				num3 = num4;
			}
			num3--;
			int num5 = textInfo.characterInfo[num2].lineNumber;
			int lastCharacterIndex = textInfo.lineInfo[num5].lastCharacterIndex;
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.uv0 = Vector2.zero;
			simpleVert.color = selectionColor;
			for (int i = num2; i <= num3 && i < textInfo.characterCount; i++)
			{
				if (i == lastCharacterIndex || i == num3)
				{
					TMP_CharacterInfo tMP_CharacterInfo = textInfo.characterInfo[num2];
					TMP_CharacterInfo tMP_CharacterInfo2 = textInfo.characterInfo[i];
					Vector2 vector = new Vector2(tMP_CharacterInfo.origin, tMP_CharacterInfo.ascender);
					Vector2 vector2 = new Vector2(tMP_CharacterInfo2.xAdvance, tMP_CharacterInfo2.descender);
					Vector2 vector3 = (Vector2)m_TextViewport.position + m_TextViewport.rect.min;
					Vector2 vector4 = (Vector2)m_TextViewport.position + m_TextViewport.rect.max;
					float num6 = m_TextComponent.rectTransform.position.x + vector.x - vector3.x;
					if (num6 < 0f)
					{
						vector.x -= num6;
					}
					float num7 = m_TextComponent.rectTransform.position.y + vector2.y - vector3.y;
					if (num7 < 0f)
					{
						vector2.y -= num7;
					}
					float num8 = vector4.x - (m_TextComponent.rectTransform.position.x + vector2.x);
					if (num8 < 0f)
					{
						vector2.x += num8;
					}
					float num9 = vector4.y - (m_TextComponent.rectTransform.position.y + vector.y);
					if (num9 < 0f)
					{
						vector.y += num9;
					}
					if (!(m_TextComponent.rectTransform.position.y + vector.y < vector3.y) && !(m_TextComponent.rectTransform.position.y + vector2.y > vector4.y))
					{
						int currentVertCount = vbo.currentVertCount;
						simpleVert.position = new Vector3(vector.x, vector2.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector2.x, vector2.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector2.x, vector.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector.x, vector.y, 0f);
						vbo.AddVert(simpleVert);
						vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
						vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
					}
					num2 = i + 1;
					num5++;
					if (num5 < textInfo.lineCount)
					{
						lastCharacterIndex = textInfo.lineInfo[num5].lastCharacterIndex;
					}
				}
			}
		}

		protected char Validate(string text, int pos, char ch)
		{
			if (characterValidation == CharacterValidation.None || !base.enabled)
			{
				return ch;
			}
			if (characterValidation == CharacterValidation.Integer || characterValidation == CharacterValidation.Decimal)
			{
				bool flag = pos == 0 && text.Length > 0 && text[0] == '-';
				bool flag2 = caretPositionInternal == 0 || caretSelectPositionInternal == 0;
				if (!flag)
				{
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
					if (ch == '-' && (pos == 0 || flag2))
					{
						return ch;
					}
					if (ch == '.' && characterValidation == CharacterValidation.Decimal && !text.Contains("."))
					{
						return ch;
					}
				}
			}
			else if (characterValidation == CharacterValidation.Alphanumeric)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (characterValidation == CharacterValidation.Name)
			{
				char c = ((text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)]);
				char c2 = ((text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)]);
				if (char.IsLetter(ch))
				{
					if (char.IsLower(ch) && c == ' ')
					{
						return char.ToUpper(ch);
					}
					if (char.IsUpper(ch) && c != ' ' && c != '\'')
					{
						return char.ToLower(ch);
					}
					return ch;
				}
				switch (ch)
				{
				case '\'':
					if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
					{
						return ch;
					}
					break;
				case ' ':
					if (c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
					{
						return ch;
					}
					break;
				}
			}
			else if (characterValidation == CharacterValidation.EmailAddress)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '@' && text.IndexOf('@') == -1)
				{
					return ch;
				}
				if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
				{
					return ch;
				}
				if (ch == '.')
				{
					char c3 = ((text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)]);
					char c4 = ((text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)]);
					if (c3 != '.' && c4 != '.')
					{
						return ch;
					}
				}
			}
			return '\0';
		}

		public void ActivateInputField()
		{
			if (!(m_TextComponent == null) && !(m_TextComponent.font == null) && IsActive() && IsInteractable())
			{
				if (isFocused && m_Keyboard != null && !m_Keyboard.active)
				{
					m_Keyboard.active = true;
					m_Keyboard.text = m_Text;
				}
				m_ShouldActivateNextUpdate = true;
			}
		}

		private void ActivateInputFieldInternal()
		{
			if (EventSystem.current.currentSelectedGameObject != base.gameObject)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
			if (TouchScreenKeyboard.isSupported)
			{
				if (Input.touchSupported)
				{
					TouchScreenKeyboard.hideInput = shouldHideMobileInput;
				}
				m_Keyboard = ((inputType != InputType.Password) ? TouchScreenKeyboard.Open(m_Text, keyboardType, inputType == InputType.AutoCorrect, multiLine) : TouchScreenKeyboard.Open(m_Text, keyboardType, false, multiLine, true));
				MoveTextEnd(false);
			}
			else
			{
				Input.imeCompositionMode = IMECompositionMode.On;
				OnFocus();
			}
			m_AllowInput = true;
			m_OriginalText = text;
			m_WasCanceled = false;
			SetCaretVisible();
			UpdateLabel();
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (shouldActivateOnSelect)
			{
				ActivateInputField();
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				ActivateInputField();
			}
		}

		public void DeactivateInputField()
		{
			if (!m_AllowInput)
			{
				return;
			}
			m_HasDoneFocusTransition = false;
			m_AllowInput = false;
			if (m_Placeholder != null)
			{
				m_Placeholder.enabled = string.IsNullOrEmpty(m_Text);
			}
			if (m_TextComponent != null && IsInteractable())
			{
				if (m_WasCanceled)
				{
					text = m_OriginalText;
				}
				if (m_Keyboard != null)
				{
					m_Keyboard.active = false;
					m_Keyboard = null;
				}
				m_CaretPosition = (m_CaretSelectPosition = 0);
				m_TextComponent.rectTransform.localPosition = Vector3.zero;
				caretRectTrans.localPosition = Vector3.zero;
				SendOnSubmit();
				Input.imeCompositionMode = IMECompositionMode.Auto;
			}
			MarkGeometryAsDirty();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			DeactivateInputField();
			base.OnDeselect(eventData);
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (IsActive() && IsInteractable() && !isFocused)
			{
				m_ShouldActivateNextUpdate = true;
			}
		}

		private void EnforceContentType()
		{
			switch (contentType)
			{
			case ContentType.Standard:
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.Default;
				m_CharacterValidation = CharacterValidation.None;
				break;
			case ContentType.Autocorrected:
				m_InputType = InputType.AutoCorrect;
				m_KeyboardType = TouchScreenKeyboardType.Default;
				m_CharacterValidation = CharacterValidation.None;
				break;
			case ContentType.IntegerNumber:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				m_CharacterValidation = CharacterValidation.Integer;
				break;
			case ContentType.DecimalNumber:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				m_CharacterValidation = CharacterValidation.Decimal;
				break;
			case ContentType.Alphanumeric:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
				m_CharacterValidation = CharacterValidation.Alphanumeric;
				break;
			case ContentType.Name:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.Default;
				m_CharacterValidation = CharacterValidation.Name;
				break;
			case ContentType.EmailAddress:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
				m_CharacterValidation = CharacterValidation.EmailAddress;
				break;
			case ContentType.Password:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Password;
				m_KeyboardType = TouchScreenKeyboardType.Default;
				m_CharacterValidation = CharacterValidation.None;
				break;
			case ContentType.Pin:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Password;
				m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				m_CharacterValidation = CharacterValidation.Integer;
				break;
			}
		}

		private void SetTextComponentWrapMode()
		{
			if (!(m_TextComponent == null))
			{
				if (m_LineType == LineType.SingleLine)
				{
					m_TextComponent.enableWordWrapping = false;
				}
				else
				{
					m_TextComponent.enableWordWrapping = true;
				}
			}
		}

		private void SetToCustomIfContentTypeIsNot(params ContentType[] allowedContentTypes)
		{
			if (contentType == ContentType.Custom)
			{
				return;
			}
			for (int i = 0; i < allowedContentTypes.Length; i++)
			{
				if (contentType == allowedContentTypes[i])
				{
					return;
				}
			}
			contentType = ContentType.Custom;
		}

		private void SetToCustom()
		{
			if (contentType != ContentType.Custom)
			{
				contentType = ContentType.Custom;
			}
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			if (m_HasDoneFocusTransition)
			{
				state = SelectionState.Highlighted;
			}
			else if (state == SelectionState.Pressed)
			{
				m_HasDoneFocusTransition = true;
			}
			base.DoStateTransition(state, instant);
		}

		//TODO 注释
		// virtual Transform ICanvasElement.get_transform()
		// {
		// 	return base.transform;
		// }

		// virtual bool ICanvasElement.IsDestroyed()
		// {
		// 	return IsDestroyed();
		// }
	}
}
