if (typeof(HTMLElement) != 'undefined' && HTMLElement.prototype)
{
	HTMLElement.prototype.__defineSetter__("innerXHTML", function(content)
	{
		try 
		{
			this.innerHTML = content;
		}
		catch (e)
		{
			var doc = document.implementation.createDocument("", "doc", null);
			var r = doc.createRange();
			r.selectNode(doc.documentElement);
			var f = r.createContextualFragment(content);
			
			while (this.childNodes.length > 0)
				this.removeChild(this.childNodes[0]);
			
			var l = f.childNodes;
			while (l.length > 0)
				this.appendChild(l[0]);
		}
	});	
}