function MaskEditClass() 
{
	this.JsMaskEditNS = this.NS = "http://schemas.suprifattus.com.br/util/jsmaskedit/";
	
	this.MASK_LIB = 
	{
		'br_zip'  : new customMask('00000-000'),
		'br_cpf'  : new customMask('000.000.000-00'),
		'br_cnpj' : new customMask('000.000.000/0000-00'),
		'br_date' : new customMask('00/00/0000'),
		'br_money': new numericMask(32, 2, ','),
		'br_decimal': new numericMask(32, -1, ','),
		'int32'   : new numericMask(32, 0),
		'int64'   : new numericMask(64, 0)
	};
	
	this.bindAll = bindAll;
	function bindAll()
	{
		var l;
		l = compat.getElementsByTagNameNS(document, this.NS, "Bind");
		for (var i=0; i < l.length; i++)
		{
			var ctl = compat.getAttribute(l[i], "Control");
			var mask = null;
			
			if (compat.hasAttribute(l[i], "Mask"))
				mask = this.getMaskFromLib(compat.getAttribute(l[i], "Mask"));
			else if (compat.hasAttribute(l[i], "CustomMask"))
				mask = new customMask(compat.getAttribute(l[i], "CustomMask"));
			else
			{
				alert('Mask tag for control ' + ctl + ' does not have a mask specified.');
				continue;
			}
				
			if (mask != null)
				mask.bind(getById(ctl));
		}

		l = compat.getElementsByTagNameNS(document, this.NS, "FocusOnStart");
		for (var i=0; i < l.length && i < 1; i++)
			EventUtil.bind(window, "load", "MaskEdit.setFocus('" + compat.getAttribute(l[i], "Control") + "')");
	}
	
	this.registerMask = function(name, mask)
	{
		this.MASK_LIB[name] = mask;
	}
	
	this.getMaskFromLib = getMaskFromLib;
	function getMaskFromLib(mask)
	{
		var maskEntry = this.MASK_LIB[mask];
		if (!maskEntry)
		{
			alert("Mask not found: " + mask);
			return null;
		}
		else 
			return maskEntry;
	}
	
	this.bind = bind;
	function bind(ctl, mask)
	{
		mask = getMaskFromLib(mask);
		var orCtl = ctl;
		if (typeof(ctl) == "string")
			ctl = document.getElementById(ctl);
		if (!ctl)
			alert("Control not found: " + orCtl);
		else 
			maskEntry[1].bind(ctl);
	}

	this.setFocus = setFocus;
	function setFocus(ctl)
	{
		if (typeof(ctl) == 'string')
			ctl = document.getElementById(ctl);
			
		if (ctl && ctl.nodeName != 'input' && ctl.nodeName != 'select' && ctl.nodeName != 'button')
		{
			var l = document.getElementsByTagName('input');
			for (var i=0; i < l.length; i++)
				if (l[i].type == 'text' && l[i].id) 
				{
					ctl = l[i];
					break;
				}
		}
		
		if (ctl && ctl.focus)
			ctl.focus();
	}
	
	function customMask(mask)
	{
		this.mask = mask;
		
		this.bind = bind;
		function bind(ctl)
		{
			bindCustomMask(ctl, this.mask);
		}

		function bindCustomMask(ctl, mask)
		{
			ctl.maxLength = mask.length;
			EventUtil.bind(ctl, 'keyup', function(e) { handle(e, ctl, mask); });
			EventUtil.bind(ctl, 'keypress', function(e) { handle(e, ctl, mask); });
			handle(null, ctl, mask);
		
			function handle(e, ctl, mask)
			{
				if (typeof(ctl) == 'string')
					ctl = document.getElementById(ctl);

				var s = ctl.value;
				var n;
				ctl.maxLength = mask.length;

				var repeat, loopProtection = 100;
				do
				{
					repeat = false;
					n = Math.min(s.length, mask.length);
					for (var i=0; i < n; i++)
					{
						if (s != (s = test(i, mask, s)))
						{
							repeat = true;
							break;
						}
					}
				} while (repeat && loopProtection--);
				
				if (e)
				{
					if (e.keyCode == 8) // backspace. aproveita pra retirar caracteres fixos
					{
						if (n > 0)
							if (mask[n-1] != '0')
								s = s.substring(0, s.length-1);
					}
					else // qualquer outra tecla
					{
						if (n < mask.length)
							if (mask[n] != '0')
								s += mask[n];
					}
				}
				
				ctl.value = s;
				
				/*
				if (s.length == mask.length && /true/i.test(ctl.getAttribute('autoblur')))
					ctl.blur();
				*/
				
				function removeChar(s, pos)
				{
					return s.substring(0, pos) + s.substring(pos+1);
				}
				
				function insertChar(s, pos, c)
				{
					return s.substring(0, pos) + c + s.substring(pos);
				}
				
				function test(i, mask, s)
				{
					if (mask[i] == '0')
					{
						if (!/[0-9]/.test(s[i]))
							return removeChar(s, i);
					}
					else if (s[i] != mask[i])
						return insertChar(s, i, mask[i]);
					
					return s;
				}
			}
		}
	}
	
	this.numericMask = numericMask
	function numericMask(bits, decimalPlaces, decimalSeparator)
	{
		this.bits = bits;
		this.decimalPlaces = decimalPlaces;
		this.decimalSeparator = decimalSeparator || ',';
	
		this.bind = bind;
		function bind(ctl)
		{
			var obj = this;
			ctl.onkeyup = function(e) { obj.onKeyUp(e, ctl); };
		}
		
		this.onKeyUp = onKeyUp;
		function onKeyUp(event, obj)
		{
			if (!event) event = window.event;
			var k = event.which;

			var val;
			
			// com decimais == -1, temos um número de casas decimais variável
			if (this.decimalPlaces == -1) 
			{
				if (!this.rx)
					this.rx = new RegExp('^\\d+(['+this.decimalSeparator+']\\d*)?');
				var a = this.rx.exec(obj.value);
				val = a && a[0] || '';
			}
			else
			{
				val = obj.value.replace(/\D/g, '').replace(/^0+/g, '');
				if (val.length != 0 && this.decimalPlaces > 0)
				{
					while (val.length < this.decimalPlaces+1)
						val = '0' + val;

					val = val.substring(0,val.length-this.decimalPlaces) + this.decimalSeparator + val.substring(val.length-this.decimalPlaces, val.length);
				}
			}

			if (obj.value != val)
				obj.value = val;
		}
	}
}
var MaskEdit = new MaskEditClass();

// o evento DOMContentLoaded resolve alguns problemas com o bind 
// no evento 'load', como a perda de partes do campo com máscara 
// devido a diferenças no atributo 'maxlength'.
EventUtil.bind(document, "DOMContentLoaded", function() { MaskEdit.bindAll() });
