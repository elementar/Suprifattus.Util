using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Suprifattus.Util.IO
{
	public class NoAccentsTextWriter : TextWriter
	{
		#region Static Members
		private static readonly Hashtable map;

		static NoAccentsTextWriter()
		{
			map = new Hashtable();
			AddToMap('a', 'á', 'à', 'â', 'ä', 'ã');
			AddToMap('A', 'Á', 'À', 'Â', 'Ä', 'Ã');
			AddToMap('e', 'é', 'è', 'ê', 'ë');
			AddToMap('E', 'É', 'È', 'Ê', 'Ë');
			AddToMap('i', 'í', 'ì', 'î', 'ï');
			AddToMap('I', 'Í', 'Ì', 'Î', 'Ï');
			AddToMap('o', 'ó', 'ò', 'ô', 'ö', 'õ');
			AddToMap('O', 'Ó', 'Ò', 'Ô', 'Ö', 'Õ');
			AddToMap('u', 'ú', 'ù', 'û', 'ü');
			AddToMap('U', 'Ú', 'Ù', 'Û', 'Ü');
			AddToMap('n', 'ñ');
			AddToMap('N', 'Ñ');
			AddToMap('c', 'ç');
			AddToMap('C', 'Ç');
		}

		private static void AddToMap(char to, params char[] from)
		{
			foreach (char c in from)
				map.Add(c, to);
		}
		#endregion

		private readonly TextWriter tw;

		public NoAccentsTextWriter(TextWriter tw)
		{
			this.tw = tw;
		}

		public NoAccentsTextWriter(Stream stream, Encoding encoding)
		{
			this.tw = new StreamWriter(stream, encoding);
		}

		public override Encoding Encoding
		{
			get { return this.tw.Encoding; }
		}

		public override void Write(char value)
		{
			object newVal = map[value];
			if (newVal != null)
				value = (char) newVal;

			tw.Write(value);
		}

		public override void Write(char[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				object newVal = map[buffer[i]];
				if (newVal != null)
					buffer[i] = (char) newVal;
			}

			tw.Write(buffer);
		}
	}
}