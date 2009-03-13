using System;
using System.IO;

namespace Suprifattus.Util.IO
{
	/// <summary>
	/// Um arquivo temporário, gerenciado por um <see cref="FileManager"/>.
	/// </summary>
	public class ManagedTempFile : IDisposable
	{
		private readonly FileManager fileManager;
		private string fileName;
		private bool disposed;

		/// <summary>
		/// Cria um novo arquivo temporário, gerenciado pelo <paramref name="fileManager"/>
		/// especificado.
		/// </summary>
		public ManagedTempFile(FileManager fileManager)
		{
			this.fileManager = fileManager;
		}

		/// <summary>
		/// Cria um novo arquivo temporário, gerenciado pelo <paramref name="fileManager"/>
		/// especificado.
		/// </summary>
		/// <param name="fileManager">O gerenciador</param>
		/// <param name="fileName">O nome do arquivo temporário</param>
		public ManagedTempFile(FileManager fileManager, string fileName)
		{
			this.fileManager = fileManager;
			this.fileName = fileName;
		}

		/// <summary>
		/// Destrutor, elimina o arquivo temporário.
		/// </summary>
		~ManagedTempFile()
		{
			Dispose();
		}

		/// <summary>
		/// O gerenciador de arquivos que é responsável por gerenciar 
		/// este arquivo temporário.
		/// </summary>
		public FileManager FileManager
		{
			get { return fileManager; }
		}

		/// <summary>
		/// O nome deste arquivo temporário. Não contém o caminho completo do
		/// arquivo, pois este é gerenciado pelo <see cref="FileManager"/>.
		/// </summary>
		public string FileName
		{
			get { return fileName; }
		}

		/// <summary>
		/// Abre um <see cref="Stream"/> com o conteúdo deste arquivo temporário.
		/// </summary>
		public Stream OpenRead()
		{
			if (disposed)
				throw new ObjectDisposedException("Não é possível ler o arquivo temporário, pois o mesmo já foi descartado (Disposed).");

			if (String.IsNullOrEmpty(fileName))
				throw new InvalidOperationException("Não é possível ler o arquivo temporário, pois ele ainda não foi escrito.");
			return fileManager.LeTemporario(fileName);
		}

		/// <summary>
		/// Abre um <see cref="Stream"/> pronto para gravação de novos dados,
		/// em um novo local, apontado por este arquivo temporário.
		/// </summary>
		public Stream OpenWrite(bool append)
		{
			if (disposed)
				throw new ObjectDisposedException("Não é possível abrir o arquivo temporário para gravação, pois o mesmo já foi descartado (Disposed).");

			if (append)
				return fileManager.AbreTemporario(fileName, FileMode.Append, FileAccess.Write);

			if (String.IsNullOrEmpty(fileName))
				return fileManager.CriaTemporarioParaGravacao(out fileName);

			return fileManager.AbreTemporario(fileName, FileMode.Create, FileAccess.Write);
		}

		/// <summary>
		/// Abre um <see cref="Stream"/> pronto para gravação de novos dados,
		/// em um novo local, apontado por este arquivo temporário.
		/// </summary>
		public Stream OpenWrite()
		{
			return OpenWrite(false);
		}

		/// <summary>
		/// Salva o conteúdo deste arquivo temporário em local definitivo,
		/// no espaço e pasta definidos e gerenciados pelo <see cref="FileManager"/>.
		/// </summary>
		/// <param name="espaco">O espaço</param>
		/// <param name="pasta">A pasta</param>
		/// <param name="nome">O nome do arquivo</param>
		public void SaveTo(string espaco, object pasta, string nome)
		{
			using (Stream s = this.OpenRead())
				fileManager.SalvaArquivo(espaco, pasta, nome, s);
		}

		/// <summary>
		/// Descarta o arquivo temporário, realizando a exclusão do arquivo no disco.
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