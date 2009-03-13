EventUtil.bind(window, "load", "eval('JsDataBind').init()");


function JsDataBindClass()
{
	this.debug = 0;
	this.JsDataBindNS = this.NS = "http://schemas.suprifattus.com.br/util/jsdatabind/";
	this.confirms = new Table2D();
	this.editing = false;
	this.editingRecord = null;
	this.editControl = null;
	this.disableWhenEditingControls = [];
	
	this.init = init;
	function init()
	{
		this.editControl = document.getElementById('pnlNewRecord'); // HARD-CODED
		if (this.editControl == null) this.editControl = document.getElementById('tab');
		this.disableWhenEditingControls = [ 
			document.getElementById('btnDelete'), 
			document.getElementById('list') 
		]; // HARD-CODED
	
		var again = false;
		var currentRecord;
		do
		{
			var l = compat.getElementsByTagNameNS(document, this.NS, "*");
			again = false;
			for (var i=0; i < l.length; i++)
			{
				switch (compat.getLocalName(l[i]))
				{
					case "Record": 
						currentRecord = l[i]; 
						break;
					case "Output": 
						var fld = l[i].getAttribute("Field");
						var val = compat.getAttribute(currentRecord.getElementsByTagName(fld)[0], "Value");
						var newNode = document.createTextNode(val);
						l[i].parentNode.replaceChild(newNode, l[i]); 
						again = true;
						break;
					case "ChangeConfirmation": 
						this.bindConfirm(l[i]); 
						break;
				}
				if (again) 
					break;
			}
		} while (again);
	}

	this.bindConfirm = bindConfirm;
	function bindConfirm(el)
	{
		if (!el.bound)
		{
			var msg = compat.getAttribute(el, "Message");
			var fld = compat.getAttribute(el, "Field");
			var ctl = this.editControl.selectNodes("//html:*[substring(@id, 4)='"+fld+"']");
			ctl = (ctl.length > 0 ? ctl[0] : null);
			Debug.assert(ctl, "Control for field " + fld + " not found");
			this.confirms.add( ctl, [ fld, msg ] );
			el.bound = true;
		}
	}
	
	this.confirm = confirm;
	function confirm() 
	{
		var ok = true;
	
		for (var i=0; ok && i < this.confirms.data.length; i++)
		{
			var c = this.confirms.data[i];
			
			var msg = c[2];
			var fld = c[1];
			var ctl = c[0];
			var newVal = getValue(ctl);
			var xpath = "jsdatabind:"+fld+"/@Value";
			var oldVal = this.editingRecord.selectNodes(xpath);
			oldVal = oldVal.length > 0 ? oldVal[0].value : null;
			//Debug.assert(oldVal, "Old value not found for field '"+fld+"' using xpath "+xpath);
			
			if (oldVal && oldVal != newVal)
				ok = ok && window.confirm(eval("'"+msg.replace(/(\$[{])([^}]+)([}])/g, function(m) { return "'+" + m.substring(2,m.length-1) + "+'"; })+"'"));
		}
		
		return ok;
	}
	
	this.findRecord = findRecord;
	function findRecord(el)
	{
		while (el && el.localName != "Record")
		{
			var oldEl = el;
			el = el.previousSibling;
			if (!el)
				el = oldEl.parentNode;
		}
		return el;
	}
	
	this.ensureEditing = ensureEditing;
	function ensureEditing()
	{
		if (!this.editing)
		{
			this.editing = true;
			document.forms[0].reset();
			for (var i=0; i < this.disableWhenEditingControls.length; i++)
				if (this.disableWhenEditingControls[i])
				{
					switch (this.disableWhenEditingControls[i].nodeName)
					{
						case 'input': this.disableWhenEditingControls[i].disabled = true;
						default     : this.disableWhenEditingControls[i].style.opacity = 0.3;
					}
				}
						
			_cssshow(this.editControl);

			var l = this.editControl.getElementsByTagName('input');
			var submitBtn = l[l.length-2];
			bindReturnToSubmit(this.editControl, submitBtn);
			
			if (MaskEdit) MaskEdit.setFocus(this.editControl);
		}
	}
	
	this.bindReturnToSubmit = bindReturnToSubmit;
	function bindReturnToSubmit(container, submitBtn)
	{
			if (typeof(submitBtn) == 'string')
				submitBtn = document.getElementById(submitBtn);
			
			if (typeof(container) == 'string')
				container = document.getElementById(container);
			
			var l = container.getElementsByTagName('input');
			if (typeof(submitBtn) == 'undefined')
				alert('submit button not found');
			else
			{
				// alert('binding submit to ' + submitBtn.id);
				EventUtil.bind(this.editControl, 'keypress', function(e)
				{
					var ctl = e.target;
					if (ctl && ctl.nodeName == 'input' && (ctl.type == 'text' || ctl.type == 'password'))
						if (e.keyCode == 13) {
							// alert('submitting using RETURN');
							ctl.blur();
							EventUtil.cancel(e);
							submitBtn.click();
						}
				});
			}
	}
	
	this.editNew = editNew;
	function editNew()
	{
		this.edit(null);
	}
	
	this.edit = edit;
	function edit(rec, allowClean)
	{
		if (!this.editControl)
			return;
		
		this.ensureEditing();
		this.editingRecord = rec;
		
		if (typeof(JsMasterDetail) != 'undefined') JsMasterDetail.asyncMode = false;
		
		var l = [
			this.editControl.getElementsByTagName("input"), 
			this.editControl.getElementsByTagName("textarea"), 
			this.editControl.getElementsByTagName("select"),
			this.editControl.getElementsByTagName("span")
		];
		for (var i=0; i < l.length; i++)
		{
			for (var j=0; j < l[i].length; j++)
			{
				var ctl = l[i][j];
				if (ctl.nodeName == 'input' && ctl.type == 'submit')
				{
					if (!representsTrue(compat.getAttributeNS(ctl, JsValidation.NS, "ignore")))
						EventUtil.bind(ctl, "click", function(e) { if (!JsDataBind.confirm()) EventUtil.cancel(e); });
				}
				
				if (ctl.nodeName == 'input' && ctl.type == 'password')
					continue;
				
				if (rec != null || allowClean)
				{
					if (ctl.id)
						try {
							var val = getEditRecordValue(rec, ctl) || "";
							
							if (ctl.nodeName == 'select' && ctl.wsbindinprogress)
								ctl.onwsbindcomplete = function() { setValue(ctl, val); };
							else
								setValue(ctl, val);
						} catch (e) {}
				}
			}
		}
		
		if (typeof(JsMasterDetail) != 'undefined') JsMasterDetail.asyncMode = true;
		if (typeof(MaskEdit) != 'undefined') MaskEdit.bindAll();
		if (typeof(aguarde) != 'undefined' && aguarde.hide) aguarde.hide();
	}
	
	function getEditRecordValue(rec, ctl)
	{
		var id = ctl.id.substring(3);
		var l = rec.childNodes;
		var fld = null;
		for (var i=0; i < l.length && !fld; i++)
			if (l[i].localName == id)
				fld = l[i];
		//if (debug >= 5) alert(fld.nodeName + '\n' + fld.textContent);
		return (fld && (fld.getAttribute("Value") || fld.textContent));
	}
	
	this.onEditClick = onEditClick;
	function onEditClick(obj, e)
	{
		if (typeof(aguarde) != 'undefined' && aguarde.show) aguarde.show();

		this.edit(findRecord(obj));
	}
	
	this.onAjaxEditClick = onAjaxEditClick;
	function onAjaxEditClick(obj, e, url)
	{
		if (typeof(aguarde) != 'undefined' && aguarde.show) aguarde.show();
		
		var this_ = this;
		var ajax = createXmlHttp();
		ajax.open("POST", url, true);
		ajax.onreadystatechange = function() {
			if (ajax.readyState == 4) {
				this_.edit(ajax.responseXML.documentElement.firstChild);
			}
		};
		ajax.send('');
	}
	
	this.onCheckClick = onCheckClick;
	function onCheckClick(obj, e)
	{
		var tds = obj.parentNode.getElementsByTagName('td');
		var chks;
		for (var i=0; i < tds.length; i++)
		{
			chks = tds[i].getElementsByTagName('input');
			if (chks.length > 0 && chks[0].type == 'checkbox')
				break;
		}
		
		var chk = chks[0];
		chk.checked = !chk.checked;
	}
	
	this.cancelEdit = cancelEdit;
	function cancelEdit()
	{
		this.editing = false;
		for (var i=0; i < this.disableWhenEditingControls.length; i++)
			if (this.disableWhenEditingControls[i])
			{
				switch (this.disableWhenEditingControls[i].nodeName)
				{
					case 'input': this.disableWhenEditingControls[i].disabled = false;
					default     : this.disableWhenEditingControls[i].style.opacity = 1;
				}
			}
		_cssshow(this.editControl);
	}
}
JsDataBind = new JsDataBindClass();
