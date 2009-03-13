function WebServices() 
{
	function call(xmlhttp, command, ns, callback, async)
	{
		if (xmlhttp == null)
			xmlhttp = createXmlHttp();
		
		var debug = 0;
		
		var paramsRx = /\((.*)\)/;
		var params = paramsRx.exec(command)[1].split(';');
		var url = command.replace(paramsRx, '');
		var action = url.substring(url.lastIndexOf('/')+1);
		
		url = url.substring(0, url.lastIndexOf('/'));
		
		if (debug >= 3) alert('command = ' + command + '\nurl = ' + url);
		
		params = "'" + params.join('&') + "'";
		params = params.replace(/\$\{/g, "' + getValue(document.getElementById('");
		params = params.replace(/\}/g, "')) + '");
		
		if (debug >= 3) alert(params + '\n' + eval(params));
		params = eval(params);
		params = envelope(action, ns, params);
		
		if (debug >= 2) alert('calling web service "' + url + '", with params "' + params + '"');
		
		xmlhttp.open("POST", url, async);
		xmlhttp.setRequestHeader("SOAPAction", ns + '/' + action);
		xmlhttp.setRequestHeader("Content-Type", "text/xml");
		
		if (async)
			xmlhttp.onreadystatechange = callback;
		xmlhttp.send(params);
		if (!async)
			callback();
		
		return xmlhttp;
	}
	
	function envelope(action, ns, params)
	{
		var xml =
			'<?xml version="1.0" encoding="utf-8"?>\n'+
			'<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\n'+
			'	<soap:Body>\n'+
			'		<' + action + ' xmlns="' + ns + '">\n';
		
		var params = params.split('&');
		for (var i=0; i < params.length; i++)
		{
			var p = params[i].split('=');
			xml += '			<' + p[0] + '>' + p[1] + '</' + p[0] + '>\n';
		}
		
		xml +=
			'		</' + action + '>\n'+
			'	</soap:Body>\n'+
			'</soap:Envelope>\n';
		
		return xml;
	}
}
