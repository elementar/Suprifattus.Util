using System;
using System.IO;

namespace Suprifattus.Util.IO.HoloFS.Impl
{
	/// <summary>
	/// Implementação de um arquivo local.
	/// </summary>
	public class LocalFile : HoloFile
	{
		FileInfo fi;

		public LocalFile(FileInfo fi)
		{
			this.fi = fi;
		}

		public LocalFile(string filename)
			: this(new FileInfo(filename)) { }


		public override Stream OpenRead()
		{
			return fi.OpenRead();
		}

		public override Stream OpenWrite()
		{
			return fi.OpenWrite();
		}

		public override bool Exists
		{
			get { return fi.Exists; }
		}

		public override string Name
		{
			get { return fi.Name; }
		}

		public override string FullName
		{
			get { return fi.FullName; }
		}

	}
}
