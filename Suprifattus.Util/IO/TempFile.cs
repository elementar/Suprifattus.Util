using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Suprifattus.Util.IO
{
	/// <summary>
	/// Cria um arquivo tempor�rio na constru��o,
	/// deletando-o no Dispose.
	/// </summary>
	public class TempFile : IDisposable
	{
		/// <summary>
		/// N�mero m�ximo de tentativas de obter arquivos tempor�rios com uma
		/// extens�o espec�fica.
		/// </summary>
		public static int MaxTries = 100;

		private FileInfo file;
		private volatile Timer timer;

		/// <summary>
		/// Cria um novo arquivo tempor�rio com a extens�o especificada.
		/// </summary>
		/// <param name="extension">A extens�o necess�ria, iniciando com ponto.</param>
		/// <param name="source">O stream com dados a serem inseridos no arquivo tempor�rio, quando este for criado.</param>
		public TempFile(string extension, Stream source)
		{
			int n = 0;
			string filename;
			do 
			{
				if (++n > MaxTries)
					throw new ApplicationException("N�o foi poss�vel gerar um arquivo tempor�rio com a extens�o solicitada.\n\nExclua alguns arquivos tempor�rios.");

				filename = Path.GetTempFileName();
				System.IO.File.Delete(filename); // apaga o arquivo 0-bytes que o Windows cria.
				
				filename = Path.ChangeExtension(filename, extension);
			} while (System.IO.File.Exists(filename));

			file = new FileInfo(filename);

			if (source != null) 
			{
				using (FileStream fs = file.OpenWrite())
					Streams.SaveStream(source, fs);
			}
		}

		/// <summary>
		/// Cria um novo arquivo tempor�rio com a extens�o especificada.
		/// </summary>
		/// <param name="extension">A extens�o necess�ria, iniciando com ponto.</param>
		public TempFile(string extension) : this(extension, null) {}
		
		/// <summary>
		/// Cria um novo arquivo tempor�rio com a extens�o padr�o (.tmp).
		/// </summary>
		public TempFile() 
		{
			file = new FileInfo(Path.GetTempFileName());
		}

		/// <summary>
		/// Deleta o arquivo tempor�rio.
		/// </summary>
		~TempFile() 
		{
			Dispose();
		}

		/// <summary>
		/// Retorna o objeto <c>FileInfo</c> referente a este arquivo tempor�rio.
		/// Lan�a uma exce��o <see cref="ObjectDisposedException"/> caso o
		/// arquivo j� tenha sido liberado (chamando o m�todo <see cref="Dispose"/>).
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Caso o arquivo tempor�rio j� tenha sido liberado 
		/// (chamando o m�todo <see cref="Dispose"/>).
		/// </exception>
		public FileInfo File 
		{
			get { CheckDisposed(); return file; }
		}
		
		/// <summary>
		/// Verifica se o arquivo ainda � v�lido (ainda existe).
		/// </summary>
		public bool IsValid 
		{
			get { return file.Exists; }
		}

		/// <summary>
		/// Verifica se o objeto j� foi liberado, e lan�a uma exce��o.
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Caso o arquivo tempor�rio j� tenha sido liberado 
		/// (chamando o m�todo <see cref="Dispose"/>).
		/// </exception>
		private void CheckDisposed() 
		{
			if (file == null)
				throw new ObjectDisposedException("TempFile", "O arquivo tempor�rio j� foi liberado.");
		}
		
		private void TryDelete(object file) 
		{
			if (timer != null) 
			{
				FileInfo f = file as FileInfo;

				Debug.Assert(f != null);
				
				Debug.WriteLine("Tentando deletar o arquivo " + f.FullName);
				Debug.IndentLevel++;

				try 
				{
					f.Delete();
					Debug.WriteLine("Deletado " + f.FullName);
					timer.Dispose();
					timer = null;
				}
				catch (IOException ex) 
				{
					Debug.WriteLine("N�o foi deletado " + f.FullName + ", devido a " + ex);
				}
				finally 
				{
					Debug.IndentLevel--;
				}
			}
			else 
			{
				Debug.WriteLine("TryDelete chamado, mas timer == null");
			}
		}
		
		/// <summary>
		/// Deleta o arquivo tempor�rio.
		/// </summary>
		public void Dispose()
		{
			if (file != null)
				timer = new Timer(new TimerCallback(TryDelete), file, 0, 300);
			
			// marca que o objeto TempFile n�o deve mais ser utilizado.
			file = null;
		}
	}
}
