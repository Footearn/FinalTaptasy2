using System.IO;
using System.Text;

namespace SimpleJSON
{
	public class JSONBool : JSONNode
	{
		private bool m_Data;

		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Boolean;
			}
		}

		public override bool IsBoolean
		{
			get
			{
				return true;
			}
		}

		public override string Value
		{
			get
			{
				return m_Data.ToString();
			}
			set
			{
				bool result;
				if (bool.TryParse(value, out result))
				{
					m_Data = result;
				}
			}
		}

		public override bool AsBool
		{
			get
			{
				return m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public JSONBool(bool aData)
		{
			m_Data = aData;
		}

		public JSONBool(string aData)
		{
			Value = aData;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write((byte)6);
			aWriter.Write(m_Data);
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append((!m_Data) ? "false" : "true");
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is bool)
			{
				return m_Data == (bool)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_Data.GetHashCode();
		}
	}
}
