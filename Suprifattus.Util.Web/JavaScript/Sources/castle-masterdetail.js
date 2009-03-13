var MasterDetailDropDown = 
{
	register: function(master, detail, url, paramsEval)
	{
		master = $(master);
		detail = $(detail);
	
		var f;
		Event.observe(master, "change", f = function(e) { 
			var oldDetailVal;
			try { oldDetailVal = detail.selectedValue || detail.getAttribute('selectedValue') || (typeof(getValue) == 'function' && getValue(detail) || null); } catch (ex) {alert(ex.message);}
			var up;
			up = new Ajax.Updater(detail, url, {
				asynchronous: true,
				parameters: eval(paramsEval),
				insertion: function(t, res) {
					try {
					var xml = up.transport.responseXML;
					if (!xml)
					{
						alert('Erro ao atualizar:\n\n' + up.transport.responseText);
						return;
					}
					
					if (document.createRange)
					{
						var r = document.createRange();
						r.selectNodeContents(t);
						r.deleteContents();
					}
					else
						t.innerHTML = '';
					
					var l = xml.documentElement;
					if (l.nodeName == 'items') {
						t.options.add(document.createElement('option'));
						for (var i=0; i < l.childNodes.length; i++) {
							var item = l.childNodes[i];
							if (item.nodeName != 'item')
								continue;
							var op = document.createElement('option');
							op.value = item.getAttribute('id');
							op.appendChild( document.createTextNode( item.textContent ) );
							t.options.add(op);
						}
					}
					else {
						for (var i=0; i < l.childNodes.length; i++)
						{
							var node = l.childNodes[i];
							var op;
							if (document.all)
							{
								op = document.createElement('option');
								op.value = node.value || node.getAttribute('value') || '';
								if (node.firstChild)
									op.innerText = node.firstChild.nodeValue;
							}
							else
							{
								op = node;
								i--;
							}
							
							t.appendChild(op);
						}
					}
					detail.selectedIndex = 0;
					detail.selectedValue = oldDetailVal;
					if (detail.selectedIndex == 0)
					{
						for (var i=0; i < detail.options.length; i++)
						{
							if (detail.options[i].value == oldDetailVal) 
							{
								if (document.all)
									setTimeout(function() { detail.selectedIndex = i; }, 100);
								else
									detail.selectedIndex = i;
								break;
							}
						}
					}
					} catch (ex) { alert(ex.message); }
				}
			});
		});
		
		// chama a função, 
		// para buscar as informações adicionais ainda no carregamento.
		Event.observe(window, 'load', f);
	}
}
