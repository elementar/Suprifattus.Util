EventUtil.bind(window, "load", function() {
	var recs = compat.getElementsByTagNameNS(document, JsDataBind.NS, 'Record');

	var objs = [ document.getElementById('pnlNewRecord'), document.getElementById('tab') ];
	
	for (var i=0; i < objs.length; i++)
		if (objs[i]) 
			_cssshow(objs[i]);

	JsDataBind.edit(recs[0], (recs.length > 0));
});
