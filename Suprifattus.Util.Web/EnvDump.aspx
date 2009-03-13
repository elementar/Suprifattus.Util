<%@ Page language="c#" AutoEventWireup="false" %>
<script runat="server">
		class NameValue {
			private string name, val;
			
			public NameValue(string name, string val) {
				this.name = name;
				this.val = val;
			}
			
			public string Name { get { return name; } }
			public string Value { get { return val; } }
		}

		protected object envDmp;

		void Button1_Click(object sender, System.EventArgs e) {
			if (TextBox1.Text == "envdmp")
			{
				envDmp = BuildEnvDump();
				dgDump.Visible = true;
				dgDump.DataBind();
			}
		}

		object BuildEnvDump() {
			ArrayList ds = new ArrayList();

			ds.Add(new NameValue("MapPath('/')", Server.MapPath("/")));
			ds.Add(new NameValue("MapPath('.')", Server.MapPath(".")));
			ds.Add(new NameValue("AppDomain.BaseDir", AppDomain.CurrentDomain.BaseDirectory));

			foreach (string key in Request.ServerVariables.Keys)
				ds.Add(new NameValue(key, Request.ServerVariables[key].ToString().Replace("\n", "<br>")));
			
			return ds;
		}
</script>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>EnvDump</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<style type="text/css">
			BODY, TBODY, INPUT { font: 8pt Verdana, Arial, sans-serif; }
		</style>
	</HEAD>
	<body MS_POSITIONING="FlowLayout">
		<form id="Form1" method="post" runat="server">
			<P>
				<asp:TextBox ID="TextBox1" Runat="server" TextMode="Password" OnTextChanged="Button1_Click"/>
				<asp:Button ID="Button1" Runat="server" Text="EnvDump" OnClick="Button1_Click"/>
			</P>
			<P>
				<asp:DataGrid id=dgDump runat="server" DataSource="<%# envDmp %>" AutoGenerateColumns="False" BorderColor="Tan" BorderWidth="1px" BackColor="LightGoldenrodYellow" CellPadding="2" GridLines="None" ForeColor="Black" Width="100%" Visible="False">
					<SelectedItemStyle ForeColor="GhostWhite" BackColor="DarkSlateBlue"></SelectedItemStyle>
					<AlternatingItemStyle BackColor="PaleGoldenrod"></AlternatingItemStyle>
					<HeaderStyle Font-Bold="True" BackColor="Tan"></HeaderStyle>
					<FooterStyle BackColor="Tan"></FooterStyle>
					<PagerStyle HorizontalAlign="Center" ForeColor="DarkSlateBlue" BackColor="PaleGoldenrod"></PagerStyle>
					<Columns>
						<asp:BoundColumn HeaderText="Name" DataField="Name"/>
						<asp:BoundColumn HeaderText="Value" DataField="Value"/>
					</Columns>
				</asp:DataGrid>
			</P>
		</form>
	</body>
</HTML>
