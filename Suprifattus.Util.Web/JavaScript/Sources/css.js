EventUtil.bind(window, "load", setEvents);

function setEvents() 
{
	var focus = function() { highlightRow(this, 'li', 'focus', true); };
	var blur = function() { highlightRow(this, 'li', 'focus', false); };
	var l = [ document.getElementsByTagName("input"), document.getElementsByTagName("textarea"), document.getElementsByTagName("select") ];
	for (var i=0; i < l.length; i++)
		for (var j=0; j < l[i].length; j++)
		{
			l[i][j].onfocus = focus;
			l[i][j].onblur = blur;
		}
}

function toggleClass(obj, clazz)
{
	if (typeof(obj) == 'string')
		obj = document.getElementById(obj);
	
	var rx = new RegExp("\\b"+clazz+"\\b", "g");
	if (!rx.test(obj.className))
		obj.className += ' ' + clazz;
	else
		obj.className = obj.className.replace(rx, '');
}

function removeCSSClass(obj, clazz)
{
	if (typeof(obj) == 'string')
		obj = document.getElementById(obj);
	var rx = new RegExp("\\b"+clazz+"\\b", "g");
	obj.className = obj.className.replace(rx, '');
}

function appendCSSClass(obj, clazz)
{
	if (typeof(obj) == 'string')
		obj = document.getElementById(obj);
	obj.className += ' ' + clazz;
}

function toggleStyle(obj, styleKey, values)
{
	if (typeof(obj) == 'string')
		obj = document.getElementById(obj);
	
	var currStyle = obj.style[styleKey];
	var newStylePos = 0;
	
	for (var i=0; i < values.length; i++)
		if (currStyle == values[i])
		{
			newStylePos = i+1;
			if (newStylePos >= values.length)
				newStylePos = 0;
			break;
		}

	obj.style[styleKey] = values[newStylePos];
}


function highlightRow(o, rowTagName, cssClass, add)
{
	if (!o) return;
	
	var div = o.parentNode;
	if (!div) return;
	
	while (div.parentNode && div.parentNode.tagName != rowTagName)
		div = div.parentNode;
	if (!div || !div.parentNode) return;
	var row = div.parentNode;
	
	if (add)
	{
		appendCSSClass(div, cssClass);
		appendCSSClass(row, cssClass);
	}
	else
	{
		removeCSSClass(div, cssClass);
		removeCSSClass(row, cssClass);
	}
}

function _viewkey(n)
{
	var n = n || 13;
	if (event.keyCode == n)
	{
		event.cancelBubble = true;
		event.returnValue = false;
   }
}
		
function _cssshow(o)
{
	if (typeof(o) == "string")
		o = document.getElementById(o);

	toggleStyle(o, 'display', ['none', 'block']);
}