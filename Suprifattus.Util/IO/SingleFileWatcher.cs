using System;
using System.IO;

using P = System.IO.Path;

namespace Suprifattus.Util.IO
{
	/// <summary>
	/// Listens to the file system change notifications and raises events when
	/// a specific file changes.
	/// </summary>
	public class SingleFileWatcher : FileSystemWatcher
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SingleFileWatcher"/> class,
		/// given the specific file to monitor.
		/// </summary>
		/// <param name="filename">The file to monitor, in standard or UNC notation.</param>
		public SingleFileWatcher(string filename)
			: base(P.GetDirectoryName(filename), P.GetFileName(filename)) { }
	}
}
