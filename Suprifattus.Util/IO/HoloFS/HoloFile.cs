using System;
using System.IO;

namespace Suprifattus.Util.IO.HoloFS
{
	/// <summary>
	/// Representa um arquivo em qualquer sistema de arquivos.
	/// </summary>
	public abstract class HoloFile : HoloFSItem
	{
		/// <summary>
		/// Abre o arquivo para leitura.
		/// </summary>
		/// <returns>Um <see cref="Stream"/> já aberto com o conteúdo do arquivo.</returns>
		public abstract Stream OpenRead();
		/// <summary>
		/// Abre o arquivo para gravação.
		/// </summary>
		/// <returns>Um <see cref="Stream"/> já aberto pronto para gravação no arquivo.</returns>
		public abstract Stream OpenWrite();

		/// <summary>
		/// Retorna um novo objeto <see cref="HoloFile"/>, representando o
		/// arquivo especificado. Atualmente suporta apenas arquivos locais.
		/// </summary>
		/// <param name="path">O caminho do arquivo</param>
		/// <returns>Um novo <see cref="HoloFile"/>.</returns>
		public static HoloFile Get(string path)
		{
			return new Impl.LocalFile(path);
		}
	}
}
