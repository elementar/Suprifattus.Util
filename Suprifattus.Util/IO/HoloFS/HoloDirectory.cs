using System;

namespace Suprifattus.Util.IO.HoloFS
{
	/// <summary>
	/// Representa um diretório em qualquer sistema de arquivos.
	/// </summary>
	public abstract class HoloDirectory : HoloFSItem
	{
		/// <summary>
		/// Retorna um vetor com todos os arquivos contidos no diretório.
		/// </summary>
		/// <returns>Um vetor com todos os arquivos contidos no diretório.</returns>
		public abstract HoloFile[] GetFiles();

		/// <summary>
		/// Retorna um vetor com todos os arquivos contidos no diretório.
		/// </summary>
		/// <param name="filter">Um filtro para selecionar os arquivos que serão retornados.</param>
		/// <returns>Um vetor com todos os arquivos contidos no diretório.</returns>
		public abstract HoloFile[] GetFiles(string filter);

		/// <summary>
		/// Retorna um arquivo específico contido no diretório. Lança uma exceção caso
		/// o arquivo não seja encontrado, ou mais de um arquivo com o mesmo nome seja
		/// encontrado (caso seja fornecida uma máscara ao invés de um nome de arquivo
		/// no parâmetro).
		/// </summary>
		/// <param name="filename">O nome do arquivo a ser retornado</param>
		/// <returns>O arquivo solicitado.</returns>
		public abstract HoloFile GetFile(string filename);

		/// <summary>
		/// Retorna um arquivo específico contido no diretório. Retorna <c>null</c> 
		/// (<c>Nothing</c> no Visual Basic) caso o arquivo não seja encontrado, ou 
		/// mais de um arquivo com o mesmo nome seja encontrado (caso seja fornecida 
		/// uma máscara ao invés de um nome de arquivo no parâmetro).
		/// </summary>
		/// <param name="filename">O nome do arquivo a ser retornado</param>
		/// <returns>O arquivo solicitado.</returns>
		public abstract HoloFile GetFileSilently(string filename);

		/// <summary>
		/// Retorna um novo objeto <see cref="HoloDirectory"/>, representando o
		/// diretório especificado. Atualmente suporta apenas diretórios locais.
		/// </summary>
		/// <param name="path">O caminho do diretório</param>
		/// <returns>Um novo <see cref="HoloDirectory"/>.</returns>
		public static HoloDirectory Get(string path)
		{
			return new Impl.LocalDirectory(path);
		}

		/// <summary>
		/// Retorna um novo objeto <see cref="HoloDirectory"/>, representando os
		/// diretórios especificados. Atualmente suporta apenas diretórios locais.
		/// </summary>
		/// <param name="paths">O caminho dos diretórios</param>
		/// <returns>Um novo <see cref="HoloDirectory"/>.</returns>
		public static HoloDirectory GetFallThrough(params string[] paths)
		{
			return new Impl.FallThroughDirectory(paths);
		}
	}
}
