using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace Suprifattus.Util.IO
{
	public class FileManager
	{
		private readonly DirectoryInfo rootDir;

		public FileManager(string rootDir)
		{
			this.rootDir = new DirectoryInfo(ExpandDirectory(rootDir));

			if (!this.rootDir.Exists)
				this.rootDir.Create();
		}

		private static string ExpandDirectory(string directory)
		{
			if (String.IsNullOrEmpty(directory))
				return directory;

			if (directory.StartsWith("~/"))
			{
				HttpContext webContext = HttpContext.Current;
				if (webContext != null && webContext.Server != null)
					directory = webContext.Server.MapPath("~") + directory.Substring(1);
				else
					directory = AppDomain.CurrentDomain.BaseDirectory + directory.Substring(1);
			}

			return new DirectoryInfo(directory).FullName;
		}

		public DirectoryInfo RootDir
		{
			get { return rootDir; }
		}

		public void ExcluiTemporario(string fileName)
		{
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentException("O nome do arquivo deve ser especificado");

			FileInfo file = GetTempFile(fileName);

			try
			{
				file.Delete();
			}
			catch (IOException)
			{
				ThreadPool.QueueUserWorkItem(
					delegate
						{
							int tries = 20;
							do
							{
								try
								{
									file.Delete();
									return;
								}
								catch (IOException)
								{
								}
							} while (--tries > 0);
						});
			}
		}

		public void SalvaArquivo(string espaco, object pasta, string nome, Stream s)
		{
			using (Stream output = CriaArquivo(espaco, pasta, nome))
				SaveStream(s, output);
		}

		public Stream CriaArquivo(string espaco, object pasta, string nome)
		{
			FileInfo file = GetFileInfo(espaco, pasta, nome);

			return file.Create();
		}

		public bool ExcluiArquivo(string espaco, object pasta, string nome)
		{
			FileInfo file = GetFileInfo(espaco, pasta, nome);
			DirectoryInfo dir = file.Directory;

			bool existia = file.Exists;
			file.Delete();

			try
			{
				while (dir != null && dir.GetFileSystemInfos().Length == 0)
				{
					DirectoryInfo parentDir = dir.Parent;
					dir.Delete();
					dir = parentDir;
				}
			}
			catch (IOException)
			{
			}

			return existia;
		}

		/// <summary>
		/// L� um stream e salva seu conte�do em um arquivo tempor�rio.
		/// </summary>
		/// <param name="s">O <see cref="Stream"/> de origem</param>
		/// <returns>Um <see cref="ManagedTempFile"/></returns>
		public ManagedTempFile SalvaTemporario(Stream s)
		{
			FileInfo tmp = GetTempFile();
			using (FileStream output = tmp.Create())
				SaveStream(s, output);

			return new ManagedTempFile(this, tmp.Name);
		}

		/// <summary>
		/// Cria um novo arquivo tempor�rio vazio. Para criar um arquivo tempor�rio
		/// j� com conte�do, utilize <see cref="SalvaTemporario"/>.
		/// </summary>
		/// <returns>Um <see cref="ManagedTempFile"/> vazio</returns>
		public ManagedTempFile CriaTemporario()
		{
			return new ManagedTempFile(this);
		}

		public Stream LeArquivo(string espaco, object pasta, string nome)
		{
			if (String.IsNullOrEmpty(espaco))
				throw new ArgumentException("Deve ser especificada um espa�o para o arquivo");
			if (String.IsNullOrEmpty(nome))
				throw new ArgumentException("Deve ser especificado um nome para o arquivo");

			FileInfo file = GetFileInfo(espaco, pasta, nome);

			return file.OpenRead();
		}

		public Stream AbreTemporario(string fileName, FileMode fileMode, FileAccess fileAccess)
		{
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentException("O nome do arquivo deve ser especificado");

			FileInfo file = GetTempFile(fileName);

			return file.Open(fileMode, fileAccess);
		}

		public Stream LeTemporario(string fileName)
		{
			return AbreTemporario(fileName, FileMode.Open, FileAccess.Read);
		}

		#region GetFileInfo
		/// <summary>
		/// Obt�m o objeto <see cref="FileInfo"/> para o arquivo especificado.
		/// </summary>
		public FileInfo GetFileInfo(string espaco, object pasta, string nome)
		{
			if (String.IsNullOrEmpty(nome))
				throw new ArgumentException("Deve ser especificado um nome para o arquivo");

			DirectoryInfo pastaDir = this.GetDirectoryInfo(espaco, pasta);

			return new FileInfo(Path.Combine(pastaDir.FullName, nome));
		}
		#endregion

		#region GetDirectoryInfo
		/// <summary>
		/// Opera��o de baixo n�vel. Retorna o diret�rio de um espa�o.
		/// </summary>
		/// <param name="espaco">O nome do espa�o</param>
		public DirectoryInfo GetDirectoryInfo(string espaco)
		{
			if (String.IsNullOrEmpty(espaco))
				throw new ArgumentException("Deve ser especificado o nome do espa�o");

			return rootDir.CreateSubdirectory(espaco);
		}

		/// <summary>
		/// Opera��o de baixo n�vel. Retorna o diret�rio de uma pasta.
		/// </summary>
		public DirectoryInfo GetDirectoryInfo(string espaco, object pasta)
		{
			var espacoDir = this.GetDirectoryInfo(espaco);
			var pastaDir = espacoDir;

			string[] pastas;

			if (pasta is IEnumerable<string>)
				pastas = ((IEnumerable<string>) pasta).ToArray();
			else if (pasta is IEnumerable<object>)
				pastas = ((IEnumerable<object>) pasta).Select(p => Convert.ToString(p)).ToArray();
			else
				pastas = Convert.ToString(pasta).Split('/', '\\');

			foreach (string pastaString in pastas)
				if (!String.IsNullOrEmpty(pastaString))
					pastaDir = pastaDir.CreateSubdirectory(pastaString);

			if (pastaDir == espacoDir)
				throw new ArgumentException("A pasta especificada deve ser uma pasta v�lida no sistema de arquivos");

			return pastaDir;
		}
		#endregion

		/// <summary>
		/// Abre um arquivo tempor�rio, retornando o <see cref="Stream"/> que pode 
		/// ser utilizado para efetuar a grava��o, e o nome do arquivo tempor�rio,
		/// para posterior leitura pelo m�todo <see cref="LeTemporario"/>.
		/// </summary>
		/// <param name="newFileName">Nome do novo arquivo tempor�rio</param>
		/// <returns>Um <see cref="Stream"/> de grava��o, apontando para o arquivo tempor�rio</returns>
		internal Stream CriaTemporarioParaGravacao(out string newFileName)
		{
			FileInfo tmp = GetTempFile();
			newFileName = tmp.Name;
			return tmp.Create();
		}

		protected FileInfo GetTempFile()
		{
			const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
			var rnd = new Random();
			DirectoryInfo tmpDir = rootDir.CreateSubdirectory("temp");
			FileInfo f;
			do
			{
				string fn = "";
				for (int i = 0; i < 8; i++)
					fn += chars[rnd.Next(chars.Length)];
				fn += ".tmp";
				f = new FileInfo(Path.Combine(tmpDir.FullName, fn));
			} while (f.Exists);

			return f;
		}

		private FileInfo GetTempFile(string fileName)
		{
			DirectoryInfo tmpDir = rootDir.CreateSubdirectory("temp");
			return new FileInfo(Path.Combine(tmpDir.FullName, fileName));
		}

		protected static int SaveStream(Stream from, Stream to)
		{
			return Streams.SaveStream(1024 * 20, from, to);
		}
	}
}