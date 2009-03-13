using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Suprifattus.Util.IO
{
	/// <summary>
	/// Cria um arquivo temporário na construção,
	/// deletando-o no Dispose.
	/// </summary>
	public class TempFile : IDisposable
	{
		/// <summary>
		/// Número máximo de tentativas de obter arquivos temporários com uma
		/// extensão específica.
		/// </summary>
		public static int MaxTries = 100;

		private FileInfo file;
		private volatile Timer timer;

		/// <summary>
		/// Cria um novo arquivo temporário com a extensão especificada.
		/// </summary>
		/// <param name="extension">A extensão necessária, iniciando com ponto.</param>
		/// <param name="source">O stream com dados a serem inseridos no arquivo temporário, quando este for criado.</param>
		public TempFile(string extension, Stream source)
		{
			int n = 0;
			string filename;
			do 
			{
				if (++n > MaxTries)
					throw new ApplicationException("Não foi possível gerar um arquivo temporário com a extensão solicitada.\n\nExclua alguns arquivos temporários.");

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
		/// Cria um novo arquivo temporário com a extensão especificada.
		/// </summary>
		/// <param name="extension">A extensão necessária, iniciando com ponto.</param>
		public TempFile(string extension) : this(extension, null) {}
		
		/// <summary>
		/// Cria um novo arquivo temporário com a extensão padrão (.tmp).
		/// </summary>
		public TempFile() 
		{
			file = new FileInfo(Path.GetTempFileName());
		}

		/// <summary>
		/// Deleta o arquivo temporário.
		/// </summary>
		~TempFile() 
		{
			Dispose();
		}

		/// <summary>
		/// Retorna o objeto <c>FileInfo</c> referente a este arquivo temporário.
		/// Lança uma exceção <see cref="ObjectDisposedException"/> caso o
		/// arquivo já tenha sido liberado (chamando o método <see cref="Dispose"/>).
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Caso o arquivo temporário já tenha sido liberado 
		/// (chamando o método <see cref="Dispose"/>).
		/// </exception>
		public FileInfo File 
		{
			get { CheckDisposed(); return file; }
		}
		
		/// <summary>
		/// Verifica se o arquivo ainda é válido (ainda existe).
		/// </summary>
		public bool IsValid 
		{
			get { return file.Exists; }
		}

		/// <summary>
		/// Verifica se o objeto já foi liberado, e lança uma exceção.
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Caso o arquivo temporário já tenha sido liberado 
		/// (chamando o método <see cref="Dispose"/>).
		/// </exception>
		private void CheckDisposed() 
		{
			if (file == null)
				throw new ObjectDisposedException("TempFile", "O arquivo temporário já foi liberado.");
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
					Debug.WriteLine("Não foi deletado " + f.FullName + ", devido a " + ex);
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
		/// Deleta o arquivo temporário.
		/// </summary>
		public void Dispose()
		{
			if (file != null)
				timer = new Timer(new TimerCallback(TryDelete), file, 0, 300);
			
			// marca que o objeto TempFile não deve mais ser utilizado.
			file = null;
		}
	}
}
