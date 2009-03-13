using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Suprifattus.Util.AccessControl.Impl;
using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Web
{
	using AccessControl;
	using Config;
	using Reflection;
	using Data;

	/// <summary>
	/// Classe que contém utilitários para fluxo de navegação
	/// em uma aplicação Web.
	/// </summary>
	[CLSCompliant(false)]
	public class WebUtil : HttpContextBound
	{
		#region Permission Management
		/// <summary>
		/// Verifica se o usuário atualmente conectado tem uma permissão específica.
		/// </summary>
		/// <param name="perm">A permissão</param>
		[Obsolete("Framework obsoleto", true)]
		public static void CheckUserPermission(IPermission perm)
		{
			PermissionChecker.CheckPermission(CurrentUser, perm, null);
		}

		/// <summary>
		/// Verifica se o usuário atualmente conectado tem uma permissão específica.
		/// </summary>
		/// <param name="perm">O ID da permissão</param>
		[Obsolete("Framework obsoleto", true)]
		public static void CheckUserPermission(string perm)
		{
			CheckUserPermission(Permission.GetPermission(perm));
		}

		/// <summary>
		/// Verifica se o usuário atualmente conectado tem permissão para
		/// executar o bloco de código atual.
		/// </summary>
		[Obsolete("Framework obsoleto", true)]
		public static void CheckUserPermissions() 
		{
			PermissionChecker.CheckPermissions(CurrentUser);
		}

		/// <summary>
		/// Verifica se o usuário atualmente conectado tem permissão para
		/// executar o bloco de código atual.
		/// </summary>
		[Obsolete("Framework obsoleto", true)]
		public static void CheckUserPermissions(Type t) 
		{
			PermissionChecker.CheckPermissions(t, CurrentUser, null);
		}

		/// <summary>
		/// Verifica se o usuário atualmente conectado tem permissão para
		/// executar o bloco de código atual.
		/// </summary>
		/// <param name="redirect">Página para redirecionar, caso ocorra a falha de permissão.</param>
		[Obsolete("Framework obsoleto", true)]
		public static void CheckUserPermissions(string redirect) 
		{
			try 
			{
				CheckUserPermissions();
			}
			catch (Exception ex)
			{
				JavascriptRedirect(redirect, "ERRO\n\n{0}", ex.Message);
			}
		}

		/// <summary>
		/// Verifica se o usuário atualmente conectado tem uma permissão específica.
		/// </summary>
		/// <param name="perm">O ID da permissão</param>
		[Obsolete("Framework obsoleto", true)]
		public static bool HasPermission(string perm)
		{
			return HasPermission(Permission.GetPermission(perm));
		}

		/// <summary>
		/// Verifica se o usuário atualmente conectado tem uma permissão específica.
		/// </summary>
		/// <param name="perm">A permissão</param>
		[Obsolete("Framework obsoleto", true)]
		public static bool HasPermission(IPermission perm)
		{
			return PermissionChecker.HasPermission(CurrentUser, perm);
		}
		#endregion

		#region User Management
		[Obsolete("Framework obsoleto", true)]
		public static IExtendedPrincipal CurrentUser 
		{
			get 
			{
				IExtendedPrincipal user = Context.User as IExtendedPrincipal;
				
				if (user == null && Session != null)
					user = Session["__user__"] as IExtendedPrincipal;

				return user;
			}
			set 
			{
				Context.User = value;
				if (Session != null)
					Session["__user__"] = value;
			}
		}
		#endregion

		#region Execution Environment Management
		[Obsolete("Framework obsoleto", true)]
		public static AppEnvironment CurrentAppEnvironment
		{
			get 
			{
				switch (Server.MachineName)
				{
					case "CARNEIRO": return AppEnvironment.Development;
					case "CACHORRO": return AppEnvironment.Development;
					case "TIGRE": return AppEnvironment.Test;
					default: return AppEnvironment.Production;
				}
			}
		}
		#endregion

		#region AppRoot Management
		public static string RelativeAppRoot
		{
			get
			{
				const string appRootKey = "_appRoot";
				if (Context.Items[appRootKey] == null) 
				{
					Uri appUri = new Uri(Request.Url, Request.ApplicationPath + "/");
					Uri reqUri = Request.Url;

#if GENERICS
					Context.Items[appRootKey] = reqUri.MakeRelativeUri(appUri).ToString();
#else
					Context.Items[appRootKey] = reqUri.MakeRelative(appUri);
#endif
				}
				return (string) Context.Items[appRootKey];
			}
		}

		public static string ExpandTilde(string s)
		{
			return s.Replace("~/", RelativeAppRoot);
		}

		public static string ExpandTilde(string s, bool physicalPath)
		{
			if (!physicalPath)
				return ExpandTilde(s);
			else
				return Regex.Replace(s, @"~[/\\]", AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/");
		}
		#endregion

		#region XHTML Support
		/// <summary>
		/// Habilita o ContentType <c>application/xhtml+xml</c> para browsers que o suportam,
		/// ativando o modo "standards compliance" para a página.
		/// </summary>
		/// <remarks>
		/// ContentType para o XUL: "application/vnd.mozilla.xul+xml"
		/// </remarks>
		public static void EnableXHtml() 
		{
			if (Request.ServerVariables["HTTP_ACCEPT"].IndexOf("application/xhtml+xml") > 0)
				Response.ContentType = "application/xhtml+xml";
		}
		#endregion

		#region HTTP Utility
		/// <summary>
		/// Adiciona o cabeçalho <c>Content-Disposition</c> à resposta HTTP, forçando
		/// o browser a realizar o download do arquivo.
		/// </summary>
		/// <param name="fn">O nome do arquivo sugerido para download</param>
		public static void ForceHttpDownload(string fn)
		{
			Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", fn));
		}
		#endregion

		#region SetMaxLengthOnTextBoxes Overloads
		/// <summary>
		/// Atribui a propriedade MaxLength de todos os <see cref="TextBox"/>es filhos de um controle.
		/// </summary>
		/// <param name="parent">O controle a partir de onde serão atribuidas as propriedades <c>MaxLength</c>.</param>
		/// <param name="mdCache">O objeto <see cref="DbMetadataCache"/> utilizado para buscar as informações sobre as tabelas</param>
		/// <param name="tableName">O nome da tabela</param>
		public static void SetMaxLengthOnTextBoxes(Control parent, DbMetadataCache mdCache, string tableName) 
		{
			SetMaxLengthOnTextBoxes(parent, null, mdCache, tableName);
		}

		/// <summary>
		/// Atribui a propriedade MaxLength de todos os <see cref="TextBox"/>es filhos de um controle.
		/// </summary>
		/// <param name="parent">O controle a partir de onde serão atribuidas as propriedades <c>MaxLength</c>.</param>
		/// <param name="connector">O <see cref="IConnector"/></param>
		/// <param name="mdCache">O objeto <see cref="DbMetadataCache"/> utilizado para buscar as informações sobre as tabelas</param>
		/// <param name="defaultTableName">O nome da tabela</param>
		public static void SetMaxLengthOnTextBoxes(Control parent, IConnector connector, DbMetadataCache mdCache, string defaultTableName) 
		{
			Regex rx = new Regex(@"(?<t>[^.]+)\.(?<f>[^.]+)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
			Match m;

			if (connector != null)
				defaultTableName = connector.GetPhysicalTableName(defaultTableName);
			
			foreach (TextBox txt in SelectControls(parent, new TypeCondition(typeof(TextBox))))
				if (txt.Enabled && !txt.ReadOnly && txt.ID != null && txt.ID.Length > 3)
				{
					IDatabaseFieldRelated dbRelated = txt as IDatabaseFieldRelated;
					if (dbRelated != null && dbRelated.DatabaseField != null && (m = rx.Match(dbRelated.DatabaseField)).Success) 
					{
						string table = m.Groups["t"].Value;
						string field = m.Groups["f"].Value;
						if (connector != null)
							table = connector.GetPhysicalTableName(table);
						txt.MaxLength = mdCache.MaxLength(table, field);
						continue;
					}
					txt.MaxLength = mdCache.MaxLength(defaultTableName, txt.ID.Substring(3));
				}
		}

		/// <summary>
		/// Atribui a propriedade MaxLength de todos os <see cref="TextBox"/>es da página.
		/// </summary>
		/// <param name="mdCache">O objeto <see cref="DbMetadataCache"/> utilizado para buscar as informações sobre as tabelas</param>
		/// <param name="tableName">O nome da tabela</param>
		public static void SetMaxLengthOnTextBoxes(DbMetadataCache mdCache, string tableName) 
		{
			SetMaxLengthOnTextBoxes(Page, mdCache, tableName);
		}
		#endregion

		#region AddEmptyItemOnDropDownLists Overloads
		/// <summary>
		/// Adiciona itens em branco como primeiros itens de todas as <see cref="DropDownList"/>s
		/// da página.
		/// </summary>
		[Obsolete("Use an empty supri:FixedListItem instead")]
		public static void AddEmptyItemOnDropDownLists()
		{
			AddEmptyItemOnListControls(typeof(DropDownList));
		}

		/// <summary>
		/// Adiciona itens em branco como primeiros itens de todas as <see cref="DropDownList"/>s
		/// da página.
		/// </summary>
		[Obsolete("Use an empty supri:FixedListItem instead")]
		public static void AddEmptyItemOnListControls()
		{
			AddEmptyItemOnListControls(typeof(ListControl));
		}

		/// <summary>
		/// Adiciona itens em branco como primeiros itens de todas as <see cref="DropDownList"/>s
		/// da página.
		/// </summary>
		[Obsolete("Use an empty supri:FixedListItem instead")]
		public static void AddEmptyItemOnListControls(Type controlType)
		{
			ListControl[] ll = (ListControl[]) SelectControls(typeof(ListControl), new TypeCondition(controlType));
			AddEmptyItemOnListControls(ll);
		}
		
		/// <summary>
		/// Adiciona itens em branco como primeiros itens de todas as <see cref="DropDownList"/>s
		/// da página.
		/// </summary>
		[Obsolete("Use an empty supri:FixedListItem instead")]
		public static void AddEmptyItemOnListControls(params ListControl[] ddls)
		{
			ListItem empty = new ListItem();
			ListItem np = new ListItem("NI", "");
			np.Attributes["title"] = "Não preenchido";

			foreach (ListControl ctl in ddls) 
			{
				if (ctl is RadioButtonList || ctl is CheckBoxList)
					ctl.Items.Insert(0, np);
				else
					ctl.Items.Insert(0, empty);
			}
		}
		
		/// <summary>
		/// Adiciona itens em branco como primeiros itens de todas as <see cref="DropDownList"/>s
		/// da página.
		/// </summary>
		[Obsolete("Use an empty supri:FixedListItem instead")]
		public static void AddEmptyItemOnListControls(params string[] ids)
		{
			ArrayList ctls = new ArrayList(ids.Length);
			Array.Sort(ids);
			foreach (ListControl ctl in SelectControls(new TypeCondition(typeof(ListControl))))
				if (Array.BinarySearch(ids, ctl.ID) >= 0)
					ctls.Add(ctl);
			
			AddEmptyItemOnListControls((ListControl[]) CollectionUtils.ToArray(typeof(ListControl), ctls));
		}
		#endregion

		#region SelectControls Overloads
		public static Control[] SelectControls(Type resultArrayType, Control parent, Condition cond)
		{
			return (Control[]) ObjectQuery.SelectRecursive(resultArrayType, parent.Controls, "Controls", cond);
		}

		public static Control[] SelectControls(Control parent, Condition cond)
		{
			return SelectControls(typeof(Control), parent, cond);
		}

		public static Control[] SelectControls(Type resultArrayType, Condition cond)
		{
			return SelectControls(resultArrayType, Page, cond);
		}

		public static Control[] SelectControls(Condition cond)
		{
			return SelectControls(Page, cond);
		}
		#endregion

		#region ClearControls
		public static void ClearControls(Control parent)
		{
			foreach (Control ctl in parent.Controls)
			{
				if (ctl is TextBox)
					((TextBox) ctl).Text = null;
				else if (ctl is CheckBox)
					((CheckBox) ctl).Checked = false;
				else if (ctl is ListControl)
					((ListControl) ctl).ClearSelection();
				else if (ctl.Controls.Count > 0)
					ClearControls(ctl);
			}
		}
		#endregion

		#region JavascriptRedirect Overloads
		public static void JavascriptRedirect(string url) 
		{
			JavascriptRedirect(url, null);
		}
			
		public static void JavascriptRedirect(string url, string message, params object[] messageParams) 
		{
			TextWriter w = Response.Output;
			
			Response.Clear();
			
			w.WriteLine("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>");
			w.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
			w.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"pt-br\" lang=\"pt-br\">");
			w.WriteLine("\t<head>");
			w.WriteLine("\t\t<script type=\"text/javascript\" defer=\"true\">");
			w.WriteLine("\t\t\t// <![CDATA[");
			if (message != null)
				w.WriteLine("\t\t\talert('{0}');", String.Format(message, messageParams).Replace("'", "\\'").Replace("\n", "\\n"));

			url = url.Replace("'", "\\'").Replace("~", Request.ApplicationPath);
			
			//w.Write("\t\t\talert('Redirecting to \\'{0}\\'');", url);
			w.WriteLine("\t\t\tlocation.href = '{0}';", url);

			w.WriteLine("\t\t\t// ]]>");
			w.WriteLine("\t\t</script>");
			w.WriteLine("\t</head>");
			w.WriteLine("</html>");
			
			Response.End();
		}
		#endregion

		#region RegisterStartupFocus Overloads
		public static void RegisterStartupFocus(Control ctl)
		{
			string js = @"EventUtil.bind(window, 'load', 'document.getElementById(\'" + ctl.ClientID + "\').focus()');";

#if GENERICS
			Page.ClientScript.RegisterClientScriptBlock(typeof(WebUtil), "focus", js, true);
#else
			Page.RegisterClientScriptBlock("focus", "<script type='text/javascript'>" + js + "</script>");
#endif
		}
		#endregion
	}
	
	public interface IDatabaseFieldRelated
	{
		string DatabaseField { get; }
	}
}
