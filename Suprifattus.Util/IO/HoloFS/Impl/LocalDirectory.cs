using System;
using System.IO;

namespace Suprifattus.Util.IO.HoloFS.Impl
{
	/// <summary>
	/// Representa um diretório local.
	/// </summary>
	/// <remarks>
	/// Encapsula um <see cref="System.IO.DirectoryInfo"/>.
	/// </remarks>
	public class LocalDirectory : HoloDirectory
	{
		DirectoryInfo di;

		/// <summary>
		/// Cria um objeto que representa um diretório local.
		/// </summary>
		/// <param name="di">O <see cref="DirectoryInfo"/> que representa o diretório</param>
		public LocalDirectory(DirectoryInfo di)
		{
			this.di = di;
		}

		/// <summary>
		/// Cria um objeto que representa um diretório local.
		/// </summary>
		/// <param name="directoryName">O nome do diretório</param>
		public LocalDirectory(string directoryName)
			: this(new DirectoryInfo(directoryName)) { }
		
		/// <summary>
		/// O nome do diretório.
		/// </summary>
		public override string Name
		{
			get { return di.Name; }
		}

		/// <summary>
		/// O nome completo do diretório.
		/// </summary>
		public override string FullName
		{
			get { return di.FullName; }
		}

		/// <summary>
		/// Verdadeiro se o diretório existe.
		/// </summary>
		public override bool Exists
		{
			get { return di.Exists; }
		}

		private HoloFile[] ConvertFiles(FileInfo[] files)
		{
			HoloFile[] holoFiles = new HoloFile[files.Length];

			for (int i = 0; i < files.Length; i++)
				holoFiles[i] = new LocalFile(files[i]);

			return holoFiles;
		}

		/// <summary>
		/// Retorna todos os arquivos contidos no diretório local.
		/// </summary>
		public override HoloFile[] GetFiles()
		{
			return ConvertFiles(di.GetFiles());
		}

		/// <summary>
		/// Retorna todos os arquivos contidos no diretório local,
		/// de acordo com um filtro.
		/// </summary>
		/// <param name="filter">O filtro</param>
		public override HoloFile[] GetFiles(string filter)
		{
			return ConvertFiles(di.GetFiles(filter));
		}

		/// <summary>
		/// Retorna um arquivo contido no diretório local.
		/// </summary>
		/// <param name="filename">O nome do arquivo</param>
		/// <returns>O arquivo</returns>
		/// <exception cref="FileNotFoundException">Se o arquivo não for encontrado, ou se for encontrado mais de um arquivo com o mesmo nome (caso sejam utilizadas máscaras).</exception>
		public override HoloFile GetFile(string filename)
		{
			FileInfo[] files = di.GetFiles(filename);
			if (files.Length == 0)
				throw new FileNotFoundException("File not found", filename);
			if (files.Length > 1)
				throw new FileNotFoundException("More than one file found using the filename specified.", filename);
			
			return ConvertFiles(files)[0];
		}

		/// <summary>
		/// Retorna um arquivo contido no diretório local.
		/// </summary>
		/// <param name="filename">O nome do arquivo</param>
		/// <returns>O arquivo, ou null caso não seja encontrado.</returns>
		public override HoloFile GetFileSilently(string filename)
		{
			try
			{
				FileInfo[] files = di.GetFiles(filename);
				return (files.Length > 0 ? ConvertFiles(files)[0] : null);
			}
			catch
			{
				return null;
			}
		}

	}
}
