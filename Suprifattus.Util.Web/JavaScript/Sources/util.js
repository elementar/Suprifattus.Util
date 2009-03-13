debug = 0;

var EventUtil = 
{
	bind: function(ctl, ev, fn)
	{
		if (typeof(fn) == "string")
			fn = new Function(/*"alert('"+fn.replace(/'/g, "\\'")+"');"+*/fn);
		if (typeof(ctl) == "string")
		{
			if (ctl[0] == '*')
			{
				var l = document.getElementsByTagName(ctl.length == 1 ? "*" : ctl.substring(1));
				for (var i=0; i < l.length; i++)
					bind(l[i], ev, fn);
				return;
			}
			else
			{
				var id = ctl;
				ctl = document.getElementById(ctl);
				if (!ctl)
					alert('EventUtil.bind(): control not found: ' + id + '\n\nfunction:\n' + fn);
			}
		}
		
		if (ctl && ctl.addEventListener) ctl.addEventListener(ev, fn, false);
		if (ctl && ctl.attachEvent) ctl.attachEvent(this.toIECompat(ev), fn);
	},
	
	bindOnLoad: function(ctl, ev, fn)
	{
		this.bind(window, "load", function() { EventUtil.bind(ctl, ev, fn); });
	},
	
	cancel: function(e)
	{
		if (e.cancelable && e.preventDefault)
			e.preventDefault();
	},
	
	toIECompat: function(ev)
	{
		var comp = ev;
		switch (ev)
		{
			case 'DOMContentLoaded': comp = 'load';
		}
		
		return "on" + comp;
	}
}

String.prototype.trimLeft = function()
{
	return this.replace(/^\s+/, '');
}
String.prototype.trimRight = function()
{
	return this.replace(/\s+$/, '');
}
String.prototype.trim = function()
{
	return this.trimLeft().trimRight();
}

function DefaultButtons(acceptCtl, cancelCtl)
{
	this.acceptCtl = acceptCtl;
	this.cancelCtl = cancelCtl;
	this.handle = handle;
	
	function handle(ev) 
	{
		switch (ev.keyCode)
		{
			case 13:
				EventUtil.cancel(ev);
				acceptCtl.click();
				break;
			case 27:
				EventUtil.cancel(ev);
				cancelCtl.click();
				break;
		}
	}
}

function instantSetFormTarget(target)
{
	document.forms[0].target = target; 
	setTimeout('document.forms[0].target=null;', 500);
}

function coalesce(a)
{
	for (var i=0; i < a.length; i++)
		if (a[i] != null)
			return a[i];
	return null;
}

function getById(id)
{
	var ctl = id;
	if (typeof(id) == 'string')
		ctl = document.getElementById(id);
	if (typeof(ctl) == 'undefined')
		alert('element with id "' + id + '" not found');
	return ctl;
}

function isVisible(el)
{
	return !el || !el.style || el.style.display != 'none' && el.style.visibility != 'hidden' && isVisible(el.parentNode);
}

function representsTrue(s)
{
	// true t yes y 12 30 4 sim s on x
	return /^(t(rue)?|y(es)?|-?[1-9]\d*|s(im)?|on|x)$/i.test(s);
}

function setValue(ctl, val)
{
	var debug = 0;
	if (debug >= 2) alert("Setting '" + val + "' to '" + ctl.id + "' (" + ctl.nodeName + ")");

	switch (ctl.nodeName)
	{
		case 'span':
		{
			var ok = false;
			val = (val ? String(val).toLowerCase() : val);
			for (var i=0; i < ctl.childNodes.length; i++)
			{
				var nodeval = ctl.childNodes[i].value;
				nodeval = (nodeval ? String(nodeval).toLowerCase() : nodeval);
				if (nodeval == val)
				{
					ctl.childNodes[i].checked = true;
					ok = true;
					break;
				}
			}
			if (!ok)
				if (debug >= 1) alert("Could not set value '" +val + "' for SPAN id = '" + ctl.id + "'. Value not found.");
			break;
		}
		case 'select':
		{
			var ok = false;
			for (var i=0; i < ctl.options.length; i++)
				if (ctl.options[i].value == val) 
				{
					ctl.selectedIndex = i;
					if (ctl.onchange) ctl.onchange();
					ok = true;
					break;
				}
			if (!ok)
				if (debug >= 1) alert("Could not set value '" +val + "' for SELECT id = '" + ctl.id + "'. Value not found.");
			break;
		}
		case 'input': 
			if (ctl.type == 'text' || ctl.type == 'hidden' || ctl.type == 'password')
				ctl.value = val;
			else if (ctl.type == 'checkbox')
				ctl.checked = /true|yes|on|1/i.test(val);
			else
				if (debug >= 2) alert("There's no support for INPUT type='" + ctl.type + "' control.");
			break;
		case 'textarea':
			ctl.value = val;
			break;
	}
}

function getValue(ctl)
{
	switch (ctl.nodeName)
	{
		case 'input':
			if (ctl.type == 'text' || ctl.type == 'hidden' || ctl.type == 'password') 
				return ctl.value;
			alert('getValue(input[type='+ctl.type+'])');
			break;
		case 'span':
		case 'div':
			var l = ctl.getElementsByTagName('input');
			for (var i=0; i < l.length; i++)
				if (l[i].checked)
					return l[i].value;
			break;
		case 'select':
		case 'SELECT':
			return ctl.options[ctl.selectedIndex].value;
		default:
			alert('getValue('+ctl.nodeName+')');
	}
}

function getValue2(ctl)
{
	switch (ctl.nodeName)
	{
		case 'input':
			return getValue(ctl);
		case 'span':
		case 'div':
			var l = ctl.getElementsByTagName('input');
			for (var i=0; i < l.length; i++)
				if (l[i].checked)
					return l[i].textContent;
			break;
		case 'select':
		case 'SELECT':
			return ctl.options[ctl.selectedIndex].name;
		default:
			alert('getValue2('+ctl.nodeName+')');
	}
}

function setAutoFillData(ddl, txt)
{
	var debug = 0;
	var ns = "http://schemas.suprifattus.com.br/util/jsmasterdetail/";
	var opt = ddl.options[ddl.selectedIndex];
	var data;
	if (opt.hasAttributeNS && opt.hasAttributeNS(ns, "autofill-data"))
		data = opt.getAttributeNS(ns, "autofill-data");
	else if (opt.hasAttribute && opt.hasAttribute("jsmasterdetail:autofill-data"))
		data = opt.getAttribute("jsmasterdetail:autofill-data");
	else if (opt["jsmasterdetail:autofill-data"])
		data = opt["jsmasterdetail:autofill-data"];
	else
		data = "";
	
	data = data.split(',');
	for (var i=0; i < data.length; i++)
		data[i] = data[i].split('=');
	
	var txt2 = txt;
	while ((txt2 = txt2.nextSibling) && txt2.nodeName != 'input')
		;
	
	if (txt.hasAttribute("readonly") || txt.value == '' || (txt2 && txt2.value == ''))
	{
		var r = findInArray(data, txt.getAttributeNS(ns, "autofill-expr"));
		if (!r || r.length < 1 || typeof(r[1]) == 'undefined')
		{
			if (debug >= 1) alert('autofill-expr not found in autofill-data');
			txt.value = '';
			return false;
		}
		else
		{
			txt.value = r[1];
			return true;
		}
	}
}

function findInArray(a, key)
{
	for (var i=0; i < a.length; i++)
		if (a[i][0] == key)
			return a[i];
}

function Table2D(source, delim1, delim2)
{
	this.delim1 = (typeof(delim1) != 'undefined' ? delim1 : ',');
	this.delim2 = (typeof(delim2) != 'undefined' ? delim2 : '=');
	this.data = source ? source.split(this.delim1) : [];
	
	for (var i=0; i < this.data.length; i++)
		this.data[i] = this.data[i].split(this.delim2);
		
	this.findRow = function(key) { return findInArray(this.data, key); }
	
	this.add = function(key, values) {
		var newRow = [key];
		for (var i=0; i < values.length; i++)
			newRow.push(values[i]);
		this.data.push(newRow);
		return this.data.length-1;
	}
}

// Original:  Dev Pragad (devpragad@yahoo.com)
// Web Site:  http://www.geocities.com/devpragad
// This script and many more are available free online at
// The JavaScript Source!! http://javascript.internet.com
function calcAge(typed) {
	typed = typed.split('/');

	var dd = typed[0];
	var mm = typed[1];
	var yy = typed[2];

	var main = "valid";
	if ((mm < 1) || (mm > 12) || (dd < 1) || (dd > 31) || (yy < 1) ||(mm == "") || (dd == "") || (yy == ""))
		main = "Invalid";
	else if (((mm == 4) || (mm == 6) || (mm == 9) || (mm == 11)) && (dd > 30))
		main = "Invalid";
	else if (mm == 2) {
		if (dd > 29)
			main = "Invalid";
		else if((dd > 28) && (!lyear(yy)))
			main="Invalid";
	}
	else if ((yy > 9999)||(yy < 0))
		main = "Invalid";
	else
		main = main;

	if (main == "valid") {
		function leapyear(a) {
			return (((a % 4 == 0) && (a % 100 != 0)) || (a % 400 == 0));
		}
		
		days = new Date();
		gdate = days.getUTCDate();
		gmonth = days.getUTCMonth();
		gyear = days.getUTCFullYear();
		age = gyear - yy;
		if((mm == (gmonth + 1)) && (dd <= parseInt(gdate))) {
			age = age;
		}
		else {
			if(mm <= (gmonth)) {
				age = age;
			}
			else {
				age = age - 1; 
			}
		}
		if (age == 0)
			age = age;
		
		return age;
	}
	
	return Number.NaN;
}
