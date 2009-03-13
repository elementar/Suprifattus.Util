using System;
using System.IO;

namespace Suprifattus.Util.IO
{
	/// <summary>
	/// Um arquivo tempor�rio, gerenciado por um <see cref="FileManager"/>.
	/// </summary>
	public class ManagedTempFile : IDisposable
	{
		private readonly FileManager fileManager;
		private string fileName;
		private bool disposed;

		/// <summary>
		/// Cria um novo arquivo tempor�rio, gerenciado pelo <paramref name="fileManager"/>
		/// especificado.
		/// </summary>
		public ManagedTempFile(FileManager fileManager)
		{
			this.fileManager = fileManager;
		}

		/// <summary>
		/// Cria um novo arquivo tempor�rio, gerenciado pelo <paramref name="fileManager"/>
		/// especificado.
		/// </summary>
		/// <param name="fileManager">O gerenciador</param>
		/// <param name="fileName">O nome do arquivo tempor�rio</param>
		public ManagedTempFile(FileManager fileManager, string fileName)
		{
			this.fileManager = fileManager;
			this.fileName = fileName;
		}

		/// <summary>
		/// Destrutor, elimina o arquivo tempor�rio.
		/// </summary>
		~ManagedTempFile()
		{
			Dispose();
		}

		/// <summary>
		/// O gerenciador de arquivos que � respons�vel por gerenciar 
		/// este arquivo tempor�rio.
		/// </summary>
		public FileManager FileManager
		{
			get { return fileManager; }
		}

		/// <summary>
		/// O nome deste arquivo tempor�rio. N�o cont�m o caminho completo do
		/// arquivo, pois este � gerenciado pelo <see cref="FileManager"/>.
		/// </summary>
		public string FileName
		{
			get { return fileName; }
		}

		/// <summary>
		/// Abre um <see cref="Stream"/> com o conte�do deste arquivo tempor�rio.
		/// </summary>
		public Stream OpenRead()
		{
			if (disposed)
				throw new ObjectDisposedException("N�o � poss�vel ler o arquivo tempor�rio, pois o mesmo j� foi descartado (Disposed).");

			if (String.IsNullOrEmpty(fileName))
				throw new InvalidOperationException("N�o � poss�vel ler o arquivo tempor�rio, pois ele ainda n�o foi escrito.");
			return fileManager.LeTemporario(fileName);
		}

		/// <summary>
		/// Abre um <see cref="Stream"/> pronto para grava��o de novos dados,
		/// em um novo local, apontado por este arquivo tempor�rio.
		/// </summary>
		public Stream OpenWrite(bool append)
		{
			if (disposed)
				throw new ObjectDisposedException("N�o � poss�vel abrir o arquivo tempor�rio para grava��o, pois o mesmo j� foi descartado (Disposed).");

			if (append)
				return fileManager.AbreTemporario(fileName, FileMode.Append, FileAccess.Write);

			if (String.IsNullOrEmpty(fileName))
				return fileManager.CriaTemporarioParaGravacao(out fileName);

			return fileManager.AbreTemporario(fileName, FileMode.Create, FileAccess.Write);
		}

		/// <summary>
		/// Abre um <see cref="Stream"/> pronto para grava��o de novos dados,
		/// em um novo local, apontado por este arquivo tempor�rio.
		/// </summary>
		public Stream OpenWrite()
		{
			return OpenWrite(false);
		}

		/// <summary>
		/// Salva o conte�do deste arquivo tempor�rio em local definitivo,
		/// no espa�o e pasta definidos e gerenciados pelo <see cref="FileManager"/>.
		/// </summary>
		/// <param name="espaco">O espa�o</param>
		/// <param name="pasta">A pasta</param>
		/// <param name="nome">O nome do arquivo</param>
		public void SaveTo(string espaco, object pasta, string nome)
		{
			using (Stream s = this.OpenRead())
				fileManager.SalvaArquivo(espaco, pasta, nome, s);
		}

		/// <summary>
		/// Descarta o arquivo tempor�rio, realizando a exclus�o do arquivo no disco.
		/// </summary>
		public void Dispose()
		{
			if (!disposed)
			{
				if (!String.IsNullOrEmpty(fileName))
					fileManager.ExcluiTemporario(fileName);

				disposed = true;
				GC.SuppressFinalize(this);
			}
		}
	}
}