if (typeof(explicitBinding) == 'undefined' || !explicitBinding)
	EventUtil.bind(window, "load", "eval('JsValidation').init()");

function JsValidationClass(form) 
{
	this.form = form;

	this.ccs = false;
	this.showBusinessRule = false;
	
	this.JsValidationNS = this.NS = "http://schemas.suprifattus.com.br/util/jsvalidation/";
	this.REGEX_LIB = [
		[ 'email',   /^\S+@\w+(\.\w+)+$/ ],
		[ 'br_date', new customValidation_br_date() ],
		[ 'br_cep',  /^\d\d[. ]?\d\d\d([. -]?\d\d\d)?$/ , /^00000[. -]?(000)?$/ ],
		[ 'br_zip',  /^\d\d[. ]?\d\d\d([. -]?\d\d\d)?$/ , /^00000[. -]?(000)?$/ ],
		[ 'br_cnpj',  /^(\d{3}\.?){3}\/?\d{4}-?\d{2}$/ ],
		[ 'br_cpf',  new customValidation_br_cpf() ],
		[ 'br_ddd',  /^0?[1-9][0-9]$/ ],
		[ 'br_decimal', /^\d+(,\d+)?$/ ],
		[ 'int', /^\d+$/ ],
		[ 'sql_tinyint', /^((2([0-4]\d|5[0-5]))|(1?\d?\d))$/ ],
		[ 'ccs_nrcartaoev', new customValidation_ccs_nrcartaoev() ]
	];

	this.init = init;
	function init()
	{
		this.form = this.form || document.getElementsByTagName("form")[0];
		EventUtil.bind(this.form, "submit", this.handleFormSubmit);
	}
	
	this.handleFormSubmit = handleFormSubmit;
	function handleFormSubmit(e)
	{
		if (typeof(JsDataBind) != 'undefined') 
			if (JsDataBind.editing) 
				if (!representsTrue(compat.getAttributeNS(e.explicitOriginalTarget, JsValidation.NS, "ignore")))
					if (!JsValidation.validate()) 
						EventUtil.cancel(e); 
	}

	this.getRegExps = getRegExps;
	function getRegExps(type) 
	{
		for (var i=0; i < this.REGEX_LIB.length; i++)
			if (this.REGEX_LIB[i][0] == type)
				return this.REGEX_LIB[i];
		return [''];
	}

	this.validate = validate;
	function validate() 
	{
		var eNotFilled = [], eInvalid = [], eExclusive = [], eSkippable = [];
		
		try 
		{
			var rg = document.selectNodes("//jsvalidation:Custom");
			for (var i=0; i < rg.length; i++)
			{
				if (!eval(rg[i].getAttribute("Expr")))
					eInvalid.push(rg[i].getAttribute("Label"));
			}
		}
		catch (e) {alert('falha no algoritmo Custom:\n'+e);}
		
		try 
		{
			var rg = document.selectNodes("//jsvalidation:OneRequiredGroup");
			for (var i=0; i < rg.length; i++)
			{
				var ctls = rg[i].getAttribute("Members").split(',');
				var lbls = [];
				var filled = false;
				var s = "";
				for (var j=0; j < ctls.length; j++) 
				{
					ctls[j] = document.getElementById(ctls[j]);
					lbls[j] = ctls[j].parentNode;
					if (lbls[j].tagName != "div") 
						lbls[j] = lbls[j].parentNode;
					lbls[j] = lbls[j].getElementsByTagName("label")[0];
					if (/\S+/.test(ctls[j].value))
					{
						filled = true;
						continue;
					}
					else
						s += (s==""?"":" ou ") + lbls[j].childNodes[0].nodeValue;
				}
				
				if (!filled) 
					eNotFilled.push(s);

				for (var j=0; j < ctls.length; j++)
					eExclusive.push(ctls[j]);
			}
		}
		catch (e) {alert('falha no algoritmo OneRequiredGroup:\n'+e);}
		
		try 
		{
			var ri = document.selectNodes("//jsvalidation:RequiredIf");
			for (var i=0; i < ri.length; i++)
			{
				var ctls = [ ri[i].getAttribute("RequiredControl"), ri[i].getAttribute("ConditionControl") ];
				var lbls = [];
				var cond = ri[i].getAttribute("ConditionValue");
				var s = "";
				for (var j=0; j < ctls.length; j++) 
				{
					ctls[j] = document.getElementById(ctls[j]);
					lbls[j] = ctls[j].parentNode;
					if (this.ccs && lbls[j].parentNode.tagName != "li") 
						lbls[j] = lbls[j].parentNode;
					lbls[j] = lbls[j].getElementsByTagName("label")[0];
				}
				
				if (getValue(ctls[1]) == cond)
					if (!/\S+/.test(getValue(ctls[0]))) 
					{
						eNotFilled.push(lbls[0].childNodes[0].nodeValue +
							(!this.showBusinessRule ? "" : " (obrigatório se " + lbls[1].childNodes[0].nodeValue + " = '" + getValue2(ctls[1]) + "')"));
					}

				eExclusive.push(ctls[0]);
			}
		}
		catch (e) {alert('falha no algoritmo RequiredIf:\n'+e);}
		
		var l = document.selectNodes("//html:div[contains(@class, 'requested')]|//html:div[contains(@class, 'required')]");
		for (var i=0; i < l.length; i++) 
		{
			if (!isVisible(l[i]))
				continue;
			
			var lbl = l[i].getElementsByTagName("label")[0];
			var ctl = l[i].getElementsByTagName("input");
			var ctl2 = [];
			for (var j=0; j < ctl.length; j++)
				ctl2[j] = ctl[j];
			ctl = ctl2;

			while (ctl.length != 0 && /(submit|button)/.test(ctl[0].type))
				ctl = ctl.slice(1);
			if (ctl.length == 0)
				ctl = l[i].getElementsByTagName("select");
			if (ctl.length == 0)
				ctl = l[i].getElementsByTagName("textarea");
			if (ctl.length == 0)
				continue;
			
			var ignoreMandatoryCheck = false;
			for (var j=0; j < eExclusive.length; j++)
				if (ctl[0] == eExclusive[j])
					ignoreMandatoryCheck = true;
				
			if (ignoreMandatoryCheck) 
				continue;
			
			if (ctl[0].tagName == "select") 
			{
				if (ctl[0].selectedIndex == 0)
					eNotFilled.push(lbl.childNodes[0].nodeValue);
			}
			else if (ctl[0].tagName == "textarea")
			{
				if (!/\S+/.test(ctl[0].value))
					eNotFilled.push(lbl.childNodes[0].nodeValue);
			}
			else 
			{
				switch (ctl[0].type) 
				{
					case "text": 
					case "password":
						if (!/\S+/.test(ctl[0].value))
							eNotFilled.push(lbl.childNodes[0].nodeValue);
						else if (ctl[1] && ctl[1].type == ctl[0].type && !/\S+/.test(ctl[1].value))
							eNotFilled.push(lbl.childNodes[0].nodeValue);
						break;
					case "checkbox":
					case "radio":
						var ok = false;
						for (var j=0; j < ctl.length; j++)
							if (ctl[j].checked)
								ok = true;
						if (!ok)
							eNotFilled.push(lbl.childNodes[0].nodeValue);
						break;
				}
			}
		}
		
		var eVals = document.selectNodes("//jsvalidation:RegexValidation");
		for (var i=0; i < eVals.length; i++)
		{	
			var eVal = eVals[i];
			var ctl = eVal.selectNodes("following-sibling::html:input[@type='text']")[0];
			var lbl = eVal.selectNodes("preceding-sibling::html:label|../../html:label")[0];
			
			if (!isVisible(ctl))
				continue;
			
			try 
			{
				if (ctl && /\S+/.test(ctl.value)) 
				{
					if (eVal.hasAttribute("Type"))
					{
						var rxs = this.getRegExps(eVal.getAttribute("Type"));
						if (rxs.length >= 2)
							if (!rxs[1].test(ctl.value))
								eInvalid.push(lbl.textContent);
						if (rxs.length >= 3)
							if (rxs[2].test(ctl.value))
								eInvalid.push(lbl.textContent);
						if (eVal.getAttribute("Critical") == "false")
							eSkippable.push(lbl.textContent);
					}
					if (eVal.hasAttribute("Valid")) 
					{
						var rx = new RegExp(eVal.getAttribute("Valid"));
						if (!rx.test(ctl.value))
							eInvalid.push(lbl.textContent);
						if (eVal.getAttribute("Critical") == "false")
							eSkippable.push(lbl.textContent);
					}
					if (eVal.hasAttribute("Invalid")) 
					{
						var rx = new RegExp(eVal.getAttribute("Invalid"));
						if (rx.test(ctl.value))
							eInvalid.push(lbl.textContent);
						if (eVal.getAttribute("Critical") == "false")
							eSkippable.push(lbl.textContent);
					}
				}
			}
			catch (e) {alert(e);}
		}
		
		if (eNotFilled.length > 0) 
		{
			alert("Favor verificar o preenchimento dos seguintes campos OBRIGATÓRIOS:\n\n- " + eNotFilled.join("\n- "));
			return false;
		}
		else if (eInvalid.length > 0)
		{
			alert("Favor verificar o CORRETO PREENCHIMENTO dos seguintes campos:\n\n- " + eInvalid.join("\n- "));
			return false;
		}
		else
			return true;
	}

	this.customValidation_br_cpf = customValidation_br_cpf;
	function customValidation_br_cpf()
	{
		this.test = test;
		function test(cpf) 
		{
			cpf = cpf.replace(/\D/g, '');
			
			if (cpf.length == 11) 
			{
				if (new RegExp(cpf[0]+"{11}").test(cpf))
					return false;

				var soma = 0;
				for (var i = 0; i < 9; i++) 
					soma += (10 - i) * parseInt(cpf[i]);
				soma = 11 - (soma % 11);
				if (soma > 9) 
					soma = 0;
				if (soma == parseInt(cpf[9])) 
				{
					soma = 0;
					for (var i = 0; i < 10; i++) 
						soma += (11 - i) * parseInt(cpf[i]);
					soma = 11 - (soma % 11);
					if (soma > 9) 
						soma = 0;
					if (soma == parseInt(cpf[10])) 
						return true;
				}
			}
			return false;
		}
	}
	
	this.customValidation_br_date = customValidation_br_date;
	function customValidation_br_date()
	{
		this.test = test;
		function test(date) 
		{
			var parts = date.split('/');
			if (parts.length != 3)
				return false;
			
			for (var i=0; i < parts.length; i++)
				parts[i] = Number(parts[i]);
			
			if (parts[2] < 10)
				parts[2] += 1900;
			
			var days = [ 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 ];
			
			if (this.isLeapYear(parts[2]))
				days[1] += 1;
			
			return this.isInRange(parts[1], 1, 12) && this.isInRange(parts[0], 1, days[parts[1]-1]);
		}
		
		this.isInRange = isInRange;
		function isInRange(n, min, max)
		{
			return (n >= min && n <= max);
		}
		
		this.isLeapYear = isLeapYear;
		function isLeapYear(year)
		{
			return (year % 4 == 0) && (year % 100 != 0 || year % 400 == 0);
		}
	}

	this.customValidation_ccs_nrcartaoev = customValidation_ccs_nrcartaoev;
	function customValidation_ccs_nrcartaoev()
	{
		this.test = test;
		function test(nro) 
		{
			nro = nro.replace(/\D/g, '');
			if (nro.length != 16)
				return false;
			
			return nro == adjust(nro);
		}
		
		this.adjust = adjust;
		function adjust(nro)
		{
			nro = nro.replace(/\D/g, '');
			
			if (nro.length != 16)
				alert("Número do cartão deve ter 16 dígitos.");

			var entrada = nro.substring(0, nro.length-1);

			var digito = 0;
			var soma = 0;
			for (var i=0; i < entrada.length; i++) 
			{
				var multiplicador = (i % 3) + 1;
				digito = Number(entrada[i]);
				soma += digito * multiplicador;
			}
			r = ((soma % 11) % 10);
			
			return entrada + r;
		}
	}
}
JsValidation = new JsValidationClass();