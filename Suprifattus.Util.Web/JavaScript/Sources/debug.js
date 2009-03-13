function Debug()
{
	function dumpObject(obj)
	{
		var o, s = o + " (" + typeof(obj) + ")\n\n";
		for (o in obj)
			s += o + " = " + obj[o] + "\n";
		return s;
	}
	
	function dumpObjectToWindow(obj)
	{
		var doc = window.open().document;
		doc.write("<table border='1'>");
		doc.write("<thead>");
		doc.write("<tr><th>property</th><th>value</th></tr>");
		doc.write("</thead>");
		doc.write("<tbody>");
		for (var o in obj)
			doc.write("<tr><td>" + o + "</td><td><pre>" + obj[o] + "</pre></td></tr>");
		doc.write("</tbody>");
		doc.write("</table>");
		doc.close();
	}
	
	function assert(cond, msg)
	{
		if (!cond)
			alert(msg?msg:"Assertion failed");
	}
}