function compat() 
{
	this.getElementsByTagNameNS = function(ctx, ns, tagName)
	{
		if (ctx.getElementsByTagNameNS)
			return ctx.getElementsByTagNameNS(ns, tagName);
		else
		{
			var l = ctx.getElementsByTagName(tagName);
			var r = [];
			for (var i=0; i < l.length; i++)
				if (this.getNamespaceUri(l[i]) == ns)
					r.push(l[i]);
			return r;
		}
	}
	
	this.getNamespaceUri = function(obj)
	{
		return obj.namespaceUri ? obj.namespaceUri : obj.tagUrn;
	}
	
	this.getLocalName = function(obj)
	{
		return obj.localName ? obj.localName : obj.tagName;
	}
	
	this.hasAttribute = function(obj, attr)
	{
		return obj.hasAttribute ? obj.hasAttribute(attr) : !!obj[attr];
	}
	
	this.getAttribute = function(obj, attr)
	{
		return obj.getAttribute ? obj.getAttribute(attr) : obj[attr];
	}

	this.getAttributeNS = function(obj, ns, attr)
	{
		if (obj.getAttributeNS)
			return obj.getAttributeNS(ns, attr);
		else
			return this.getAttribute(obj, attr);
	}
	
	this.setAttribute = function(obj, attr, val)
	{
		if (obj.setAttribute)
			obj.setAttribute(attr, val);
		else
			obj[attr] = val;
	}
}
compat = new compat();

if (typeof(Error) != 'undefined')
	if (typeof(Error.prototype.toString) == 'undefined')
		Error.prototype.toString = function() { return this.message; };
