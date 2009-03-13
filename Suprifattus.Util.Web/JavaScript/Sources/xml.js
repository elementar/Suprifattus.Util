function createXmlHttp()
{
	var xmlhttp;
	
/*@cc_on @*/
/*@if (@_jscript_version >= 5)
	try 
	{
		xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
	} 
	catch (e) 
	{
		try 
		{
			xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		} 
		catch (E) 
		{
			xmlhttp = false;
		}
	}
@else
	xmlhttp = false;
@end @*/
	if (!xmlhttp && typeof XMLHttpRequest!='undefined') 
	{
		try 
		{
			xmlhttp = new XMLHttpRequest();
		} 
		catch (e) 
		{
			xmlhttp = false;
		}
	}
	
	return xmlhttp;
}

try 
{
	if (typeof(XMLDocument) != 'undefined' && typeof(XMLDocument.prototype.selectNodes) == 'undefined')
		XMLDocument.prototype.selectNodes = function(xpath) { return selectNodes(this, xpath); }
	if (typeof(Document) != 'undefined' && typeof(Document.prototype.selectNodes) == 'undefined')
		Document.prototype.selectNodes = function(xpath) { return selectNodes(this, xpath); }
	if (typeof(Node) != 'undefined' && typeof(Node.prototype.selectNodes) == 'undefined')
		Node.prototype.selectNodes = function(xpath) { return selectNodes(this, xpath); }
}
catch (e) { alert(e); }

function selectNodes(el, xpath) 
{
	var arr = [];

	var nsr = null;
	var doc = (el.ownerDocument ? el.ownerDocument : el);
	//if (doc.createNSResolver)
	//	nsr = doc.createNSResolver(doc.documentElement);
	nsr = NSResolver;
	var res = doc.evaluate(xpath, el, nsr, XPathResult.ORDERED_NODE_ITERATOR_TYPE, null);
	if (res) 
	{
		var node;
		while (node = res.iterateNext())
			arr.push(node);
	} 
	return arr;
}

function NSResolver(prefix)
{
	switch (prefix) {
		case 'html': return 'http://www.w3.org/1999/xhtml';
		case 'xsi': return 'http://www.w3.org/2001/XMLSchema-instance';
		case 'xsd': return 'http://www.w3.org/2001/XMLSchema';
		case 'jsvalidation': return 'http://schemas.suprifattus.com.br/util/jsvalidation/';
		case 'jsmaskedit': return 'http://schemas.suprifattus.com.br/util/jsmaskedit/';
		case 'jsmasterdetail': return 'http://schemas.suprifattus.com.br/util/jsmasterdetail/';
		case 'jsdatabind': return 'http://schemas.suprifattus.com.br/util/jsdatabind/';
		default: return "";
	}
}

// Objeto utilizado para preencher combos
// com informações retornadas de um XMLHttpRequest.
function FillCombo(pctl, pxmlhttp, loadingMsg)
{
	this.debug = 0;
	this.ctl = pctl;
	this.xmlhttp = pxmlhttp;
	this.msg = (typeof(loadingMsg) != 'undefined' ? loadingMsg : "(aguarde... carregando...)");
	this.selectedValues = [];

	this.init = init;
	function init()
	{
		var debug = this.debug;
		this.ctl = getById(this.ctl);
		
		this.ctl.wsbindinprogress = true;

		this.ctl.options[0].text = this.msg;
		this.ctl.disabled = true;
		
		var l = this.ctl.options;
		if (this.ctl.oldSelectedValues != null)
		{
			this.selectedValues = this.ctl.oldSelectedValues;
			this.ctl.oldSelectedValues = null;
			l.length = 1;
		}
		else
		{
			while (l.length > 1)
			{
				if (l[1].selected)
					this.selectedValues.push(l[1].value);
				l[1] = null;
			}
		}
		
		if (debug >= 1) alert('FillCombo.init\nControl: ' + this.ctl.id + '\nSelected values: \n - ' + this.selectedValues.join('\n - '));
	}

	this.handle = handle;
	function handle()
	{
		var debug = this.debug;
		if (this.xmlhttp.readyState == 4) 
		{
			if (debug >= 1) alert('selected values: ' + this.selectedValues + '\n\nFillCombo.handle("' + this.ctl.id + '")\n' + this.xmlhttp.responseText);
			var xml = this.xmlhttp.responseXML;
			
			if (this.xmlhttp.responseXML)
			{
				//var els = xml.selectNodes("//*[@Value]");
				var els = xml.documentElement.getElementsByTagName("DropDownItem");
				
				if (debug >= 4) alert(els.length + ' DropDownItem found');
				var oneSelected = false;
				for (var i=0; i < els.length; i++)
				{
					var op = document.createElement("option");
					var val = els[i].getAttribute("Value");
					op.setAttribute("value", val);
					op.appendChild(document.createTextNode(els[i].getAttribute("Text")));
					
					var afd = els[i].getAttribute("AutoFillData");
					if (afd)
						op.setAttributeNS(JsMasterDetail.NS, "autofill-data", afd);
					
					for (var x=0; x < this.selectedValues.length; x++)
					{
						if (this.selectedValues[x] == val)
							op.selected = true;
						oneSelected = true;
					}
					
					this.ctl.appendChild(op);
					if (debug >= 5) alert('new child added: ' + op.value);
				}
				this.ctl.oldSelectedValues = (oneSelected ? null : this.selectedValues);
				
				if (debug >= 1) alert('FillCombo.finished\nControl: ' + this.ctl.id);
			}
			
			this.ctl.options[0].text = "";
			this.ctl.disabled = false;
			this.ctl.wsbindinprogress = false;
			if (this.ctl.onwsbindcomplete)
			{
				if (debug >= 3) alert('onwsbindcomplete for ' + this.ctl.id + ' = ' + this.ctl.onwsbindcomplete);
				this.ctl.onwsbindcomplete();
				this.ctl.onwsbindcomplete = null;
			}

			if (typeof(JsMasterDetail) != 'undefined')
				JsMasterDetail.handleEvent(this.ctl);
		}
	}
	
	this.init();
}
