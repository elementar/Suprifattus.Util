using System;
using System.IO;
using System.Collections;

namespace Suprifattus.Util.IO.HoloFS.Impl
{
	using Collections;

	/// <summary>
	/// Representa uma lista de diretórios que pode ser pesquisada.
	/// </summary>
	public class FallThroughDirectory : HoloDirectory
	{
		readonly HoloDirectory[] dirs;

		/// <summary>
		/// Cria um novo diretório fall-through.
		/// </summary>
		/// <param name="dirs">Os diretórios a serem utilizados, na ordem</param>
		public FallThroughDirectory(params HoloDirectory[] dirs)
		{
			this.dirs = dirs;
		}

		/// <summary>
		/// Cria um novo diretório fall-through.
		/// </summary>
		/// <param name="dirs">Os diretórios a serem utilizados, na ordem</param>
		public FallThroughDirectory(params string[] dirs)
		{
			this.dirs = new HoloDirectory[dirs.Length];
			for (int i = 0; i < dirs.Length; i++)
				this.dirs[i] = HoloDirectory.Get(dirs[i]);
		}

		/// <summary>
		/// Not implemented for <see cref="FallThroughDirectory"/>.
		/// </summary>
		/// <exception cref="NotImplementedException">Always.</exception>
		public override string Name
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Not implemented for <see cref="FallThroughDirectory"/>.
		/// </summary>
		/// <exception cref="NotImplementedException">Always.</exception>
		public override string FullName
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Retorna a lista de todos os arquivos de todos os diretórios.
		/// </summary>
		public override HoloFile[] GetFiles()
		{
			ArrayList files = new ArrayList();
			foreach (HoloDirectory dir in dirs)
				files.AddRange(dir.GetFiles());
			return (HoloFile[]) CollectionUtils.ToArray(typeof(HoloFile), files);
		}

		/// <summary>
		/// Retorna uma lista de arquivos, pesquisando em todos os diretórios.
		/// </summary>
		/// <param name="filter">O filtro</param>
		public override HoloFile[] GetFiles(string filter)
		{
			ArrayList files = new ArrayList();
			foreach (HoloDirectory dir in dirs)
				files.AddRange(dir.GetFiles(filter));
			return (HoloFile[]) CollectionUtils.ToArray(typeof(HoloFile), files);
		}

		/// <summary>
		/// Retorna o primeiro arquivo encontrado nos diretórios.
		/// </summary>
		/// <param name="filename">O nome do arquivo</param>
		/// <returns>O arquivo</returns>
		/// <exception cref="FileNotFoundException">Caso o arquivo não seja encontrado</exception>
		public override HoloFile GetFile(string filename)
		{
			HoloFile file = GetFileSilently(filename);
			if (file == null)
				throw new FileNotFoundException("File not found", filename);
			return file;
		}

		/// <summary>
		/// Retorna o primeiro arquivo encontrado nos diretórios.
		/// </summary>
		/// <param name="filename">O nome do arquivo</param>
		/// <returns>O arquivo, ou null caso o arquivo não seja encontrado</returns>
		public override HoloFile GetFileSilently(string filename)
		{
			HoloFile file = null;
			foreach (HoloDirectory dir in dirs)
			{
				file = dir.GetFileSilently(filename);
				if (file != null)
					break;
			}
			return file;
		}

		/// <summary>
		/// Not implemented for <see cref="FallThroughDirectory"/>.
		/// </summary>
		/// <exception cref="NotImplementedException">Always.</exception>
		public override bool Exists
		{
			get { throw new NotImplementedException(); }
		}

	}
}
