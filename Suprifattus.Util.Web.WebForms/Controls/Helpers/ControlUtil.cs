using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Web.Controls.Helpers
{
	/// <summary>
	/// Classe utilitária, com métodos auxiliares para lidar com <see cref="Control"/>s.
	/// </summary>
	public class ControlUtil
	{
		private ControlUtil() { throw new InvalidOperationException(); }
		
		/// <summary>
		/// Verifica se um controle está vazio.
		/// </summary>
		/// <param name="ctl">O controle</param>
		/// <returns>Verdadeiro se o controle está vazio, falso caso contrário</returns>
		/// <remarks>
		/// Um controle pode estar vazio pelos motivos abaixo:
		/// <list type="bullet">
		///		<item>Seu valor em <see cref="String"/> é igual a nulo ou a <see cref="String.Empty"/></item>
		///		<item>É um <see cref="ListControl"/> e não tem nenhum item selecionado</item>
		/// </list>
		/// </remarks>
		public static bool IsEmpty(Control ctl)
		{
			object val = GetValue(ctl);
			if (val == null) return true;
			if (val is Array) return ((Array) val).Length == 0;
			if (val is string) return ((string) val).Length == 0;

			throw new Exception("Control Type not supported: " + ctl.GetType().FullName);
		}

		/// <summary>
		/// Verifica se pelo menos um dos controles especificados está vazio.
		/// </summary>
		/// <param name="ctls">A lista dos controles</param>
		/// <returns>Verdadeiro se há um controle vazio, falso caso contrário</returns>
		/// <remarks>
		/// A verificação se o controle está vazio ou não é realizada pelo método <see cref="IsEmpty(Control)"/>.
		/// </remarks>
		public static bool IsOneEmpty(params Control[] ctls)
		{
			foreach (Control ctl in ctls)
				if (IsEmpty(ctl))
					return true;
			return false;
		}

		/// <summary>
		/// Verifica se todos os controles especificados estão vazios.
		/// </summary>
		/// <param name="ctls">A lista dos controles</param>
		/// <returns>Verdadeiro se todos os controles especificados forem vazios, falso caso contrário</returns>
		/// <remarks>
		/// A verificação se o controle está vazio ou não é realizada pelo método <see cref="IsEmpty(Control)"/>.
		/// </remarks>
		public static bool IsAllEmpty(params Control[] ctls)
		{
			foreach (Control ctl in ctls)
				if (!IsEmpty(ctl))
					return false;
			return true;
		}

		#region Type-safe Getters
		public static Int32 GetInt32(Control ctl)
		{
			try 
			{
				return Convert.ToInt32(GetValue(ctl));
			}
			catch (Exception ex)
			{
				if (!Logic.StringEmpty(ctl.ID))
					throw new FormatException(String.Format("Error while trying to convert the {0} control value '{1}' to {2}", ctl.ID, GetValue(ctl), typeof(Int32).FullName), ex);
				else
					throw;
			}
		}

		public static Int32[] GetInt32Array(ListControl ctl)
		{
			return (int[]) GetArray(ctl, typeof(int));
		}

		public static Int16 GetInt16(Control ctl)
		{
			return Convert.ToInt16(GetValue(ctl));
		}

		public static Int16[] GetInt16Array(ListControl ctl)
		{
			return (Int16[]) GetArray(ctl, typeof(Int16));
		}

		public static Decimal GetDecimal(Control ctl)
		{
			return Convert.ToDecimal(GetValue(ctl));
		}

		public static string GetString(Control ctl)
		{
			return Convert.ToString(GetValue(ctl));
		}

		public static object GetStringArray(ListControl ctl)
		{
			return (string[]) GetArray(ctl, typeof(string));
		}

		public static DateTime GetDate(Control ctl)
		{
			try
			{
				return Convert.ToDateTime(GetValue(ctl), CultureInfo.CurrentCulture);
			}
			catch (Exception ex)
			{
				if (!Logic.StringEmpty(ctl.ID))
					throw new FormatException(String.Format("Error while trying to convert the {0} control value '{1}' to {2}", ctl.ID, GetValue(ctl), typeof(DateTime).FullName), ex);
				else
					throw;
			}
		}

		public static byte GetByte(Control  ctl)
		{
			return Convert.ToByte(GetValue(ctl));
		}

		public static bool GetBoolean(Control ctl)
		{
			return Logic.RepresentsTrue(GetString(ctl));
		}

		public static Enum GetEnum(Control ctl, Type enumType)
		{
			return (Enum) Enum.Parse(enumType, GetString(ctl), true);
		}
		#endregion

		public static object GetValue(Control ctl)
		{
			if (ctl is TextBox) return ((TextBox) ctl).Text;
			if (ctl is ListControl) return ((ListControl) ctl).SelectedValue;
			if (ctl is CheckBox) return ((CheckBox) ctl).Checked;
			if (ctl is HtmlInputHidden) return ((HtmlInputHidden) ctl).Value;
			if (ctl is HtmlInputText) return ((HtmlInputText) ctl).Value;
			if (ctl is HtmlSelect) return ((HtmlSelect) ctl).Value;

			throw new Exception("Control type not supported: " + ctl.GetType().FullName);
		}

		private static IList GetArray(ListControl ctl, Type itemType)
		{
			ArrayList al = new ArrayList(ctl.Items.Count);
	
			foreach (ListItem li in ctl.Items)
				if (li.Selected)
					al.Add(Convert.ChangeType(li.Value, itemType));
			
			return CollectionUtils.ToArray(itemType, al);
		}

		/// <summary>
		/// Atribui um valor a um controle.
		/// </summary>
		/// <param name="ctl">Um controle</param>
		/// <param name="val">O valor a ser atribuído.</param>
		/// <remarks>
		/// Os controles atualmente suportados são:
		/// <list type="bullet">
		///		<item><see cref="TextBox"/> e derivados</item>
		///		<item><see cref="ListControl"/> e derivados (como <see cref="DropDownList"/> e <see cref="CheckBoxList"/>)</item>
		///		<item><see cref="CheckBox"/> e derivados (como <see cref="RadioButton"/>)</item>
		/// </list>
		/// </remarks>
		public static void SetValue(Control ctl, object val)
		{
			Debug.WriteLine(String.Format("Setting Control '{0}' to '{1}'", ctl.ID, val));
			
			if (ctl is TextBox) 
				((TextBox) ctl).Text = Convert.ToString(val);
			else if (ctl is ListControl) 
				TrySetValue((ListControl) ctl, Convert.ToString(val, CultureInfo.CurrentCulture), false);
			else if (ctl is CheckBox)
				((CheckBox) ctl).Checked = Convert.ToBoolean(val);
			else
				throw new Exception("Control type not supported: " + ctl.GetType().FullName);
		}

		/// <summary>
		/// Tenta selecionar o valor especificado no controle de lista.
		/// </summary>
		/// <param name="ctl"></param>
		/// <param name="val">O valor.</param>
		/// <returns>Verdadeiro se o valor pôde ser atribuído, falso caso contrário</returns>
		/// <remarks>
		/// O valor é convertido para <see cref="String"/> com o seguinte código:
		/// <code>
		/// Convert.ToString(val, System.Globalization.CultureInfo.CurrentCulture)
		/// </code>
		/// </remarks>
		public static bool TrySetValue(ListControl ctl, object val)
		{
			return TrySetValue(ctl, Convert.ToString(val, CultureInfo.CurrentCulture));
		}
		
		/// <summary>
		/// Tenta selecionar o valor especificado no controle de lista.
		/// </summary>
		/// <param name="ctl"></param>
		/// <param name="val">O valor.</param>
		/// <returns>Verdadeiro se o valor pôde ser atribuído, falso caso contrário</returns>
		public static bool TrySetValue(ListControl ctl, string val)
		{
			return TrySetValue(ctl, val, false);
		}
		
		/// <summary>
		/// Tenta selecionar o valor especificado no controle de lista.
		/// </summary>
		/// <param name="ctl">O <see cref="ListControl"/></param>
		/// <param name="val">O valor.</param>
		/// <param name="throwException">Se verdadeiro, uma exceção é lançada, ao invés de simplesmente returnar <c>false</c>.</param>
		/// <returns>Verdadeiro se o valor pôde ser atribuído, falso caso contrário</returns>
		/// <exception cref="IndexOutOfRangeException">Se o valor não for encontrado no <see cref="ListControl"/> especificado em <paramref name="ctl"/>.</exception>
		public static bool TrySetValue(ListControl ctl, string val, bool throwException)
		{
			ListItem li = ctl.Items.FindByValue(val);
			if (li == null) 
			{
				if (throwException)
					throw new IndexOutOfRangeException(String.Format("O valor '{0}' não foi encontrado no {1} '{2}'.", val, ctl.GetType().Name, ctl.ID));
				else if (!IsSingleSelectionControl(ctl))
					ctl.ClearSelection();
			}
			else
			{
				if (IsSingleSelectionControl(ctl))
					ctl.SelectedValue = li.Value;
				else
					li.Selected = true;
			}

			return li != null;
		}

		private static bool IsSingleSelectionControl(ListControl ctl)
		{
			return 
				(ctl is DropDownList) || 
				(ctl is RadioButtonList) || 
				(ctl is ListBox && ((ListBox) ctl).SelectionMode == ListSelectionMode.Single);
		}

		/// <summary>
		/// Verifica se o controle especificado é editável.
		/// <see cref="WebControl"/>s são editáveis se a propriedade <see cref="WebControl.Enabled"/>
		/// for verdadeira, e <see cref="TextBox"/>es são editáveis se a propriedade
		/// <see cref="TextBox.ReadOnly"/> for falsa.
		/// </summary>
		/// <param name="ctl">O controle</param>
		/// <returns>Verdadeiro se o controle é editável, falso caso contrário.</returns>
		public static bool IsEditable(Control ctl)
		{
			return 
				(ctl is WebControl && ((WebControl) ctl).Enabled) &&
				(!(ctl is TextBox) || ((TextBox) ctl).ReadOnly == false);
			}
	}
}
