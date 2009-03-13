EventUtil.bind(window, "load", "eval('FormsEx').init()");

function FormsExClass()
{
	this.FormsExNS = this.NS = "http://schemas.suprifattus.com.br/util/formsex/";
	
	this.init = init;
	function init()
	{
		var l = compat.getElementsByTagNameNS(document, this.NS, "subform");
		for (var i=0; i < l.length; i++)
			bindReturnToSubmit(l[i], l[i].getAttribute("submitControl"));
	}

	function bindReturnToSubmit(container, submitBtn)
	{
		if (typeof(submitBtn) == 'string')
			submitBtn = document.getElementById(submitBtn);
		
		if (typeof(container) == 'string')
			container = document.getElementById(container);
		
		if (typeof(submitBtn) == 'undefined')
			alert('submit button not found');
		else
		{
			var h = new SubmitSubFormClass(submitBtn);
			
			var l = container.getElementsByTagName('input');
			for (var i=0; i < l.length; i++)
				EventUtil.bind(l[i], 'keypress', function(e) { h.handleKeyPress(e); });
		}
	}
	
	function SubmitSubFormClass(submitBtn)
	{
		this.submitBtn = submitBtn;
	
		this.handleKeyPress = handleKeyPress;
		function handleKeyPress(e)
		{
			var ctl = e.target;
			if (ctl && ctl.nodeName == 'input' && (ctl.type == 'text' || ctl.type == 'password'))
			{
				if (e.keyCode == 13) 
				{
					ctl.blur();
					EventUtil.cancel(e);
					this.submitBtn.click();
				}
			}
		}
	}
}

var FormsEx = new FormsExClass();