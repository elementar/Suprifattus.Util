EventUtil.bind(window, "load", "JsMasterDetail.bindAll()");


function JsMasterDetail() 
{

	function initVars()
	{
		if (!this.initialized)
		{
			this.debug = 0;
			this.asyncMode = true;
			this.comboWSBase = "../Resources/WebServices/DropDownCompletion.asmx/";
			this.comboWSNS = "http://schemas.ccs.com.br/CartaoEvangelico/WebServices/DropDownCompletion";
			this.JsMasterDetailNS = this.NS = "http://schemas.suprifattus.com.br/util/jsmasterdetail/";
			this.controlsBound = [];
			this.controlsMasters = [];
			this.initialized = true;
		}
	}

	this.bindAll = bindAll;
	this.handleEvent = handleEvent;
	
	function bindAll()
	{
		if (debug >= 1) alert('bindAll');
		initVars();
		
		var l;
		l = compat.getElementsByTagNameNS(document, this.NS, "Bind");
		if (debug >= 4) alert('found ' + l.length + ' Bind\'s');
		for (var i=0; i < l.length; i++)
			bind(compat.getAttribute(l[i], "DetailControl"), compat.getAttribute(l[i], "MasterControls").split(','));
		
		for (var i=0; i < controlsMasters.length; i++)
			handleEvent(controlsMasters[i][0]);
	}
	
	function bind(ctlDetail, ctlMasters) 
	{
		if (debug >= 1) alert('bind');
		
		if (typeof(ctlDetail) == "string")
			ctlDetail = document.getElementById(ctlDetail);
	
		Debug.assert(ctlDetail != null, "Detail control not found: " + arguments[1]);
		
		for (var i=0; i < ctlMasters.length; i++)
		{
			if (typeof(ctlMasters[i]) == "string")
				ctlMasters[i] = document.getElementById(ctlMasters[i]);
		
			var r = findInArray(controlsMasters, ctlMasters[i]);
			if (!r)
			{
				r = [ctlMasters[i], []];
				controlsMasters.push(r);
			}
			
			r[1].push(ctlDetail);
		}

		var wsBound = ctlDetail.hasAttributeNS(JsDataBind.NS, "source-ws");
		controlsBound.push([ctlDetail, (wsBound ? null : ctlDetail.cloneNode(true)), ctlMasters]);

		if (!wsBound)
			removeAllItemsFromSelect(ctlDetail);
	}
	
	function removeAllItemsFromSelect(ctl)
	{
		if (ctl.options)
			ctl.options.length = 1;
	}
	
	function getDetailData(ctl, key)
	{
		if (ctl.nodeName == 'select')
			ctl = ctl.options[ctl.selectedIndex];
		
		Debug.assert(ctl.nodeName == 'option', 'Not an option');
		
		var val = ctl.getAttributeNS(JsMasterDetailNS, "detail-data");
		var det = new Table2D(val);
		return det.findRow(key)[1];
	}
	
	function callComboWebService(serviceName, masterCtl, detailCtl)
	{
		var xmlhttp = createXmlHttp();
		var fc = new FillCombo(detailCtl, xmlhttp);
		WebServices.call(xmlhttp, comboWSBase + serviceName, comboWSNS, function() { fc.handle() }, asyncMode);
	}

	function handleEvent(masterCtl)
	{
		if (masterCtl.wsbindinprogress)
		{
			if (debug >= 5) alert('bind in progress for ctl ' + masterCtl.id);
			return;
		}
		
		if (debug >= 1) alert('handleEvent(' + masterCtl.id + ')');
		
		var r = findInArray(controlsMasters, masterCtl);

		if (!r)
		{
			if (debug >= 1) alert('this control (' + masterCtl.id + ') is NOT a master control.');
			return false;
		}
		
		if (debug >= 5) alert('this control (' + masterCtl.id + ') is a master control.');
		
		var val = getValue(masterCtl);
		if (debug >= 5) alert('value of master control = ' + val);
		
		if (debug >= 3) alert('iterating among detail controls: ' + r[1]);
		
		for (var i=0; i < r[1].length; i++)
		{
			var ctlDetail = r[1][i];
			if (debug > 5) alert('entering detail control: ' + ctlDetail.id);
			
			if (ctlDetail.nodeName == 'input' && ctlDetail.type == 'text')
			{
				if (debug >= 3) alert('detail control ('+ctlDetail.id+') is text.\nautofilling.');
				setAutoFillData(masterCtl, ctlDetail);
				continue;
			}
			
			if (ctlDetail.hasAttributeNS(JsDataBind.NS, "source-ws"))
			{
				var ws = ctlDetail.getAttributeNS(JsDataBind.JsDataBindNS, "source-ws");
				if (debug >= 3) alert('detail control ('+ctlDetail.id+') is web service bound.\ncalling web service:\n' + ws);
				callComboWebService(ws, masterCtl, ctlDetail);
				continue;
			}
			
			if (debug >= 3) if (!confirm('detail control ('+ctlDetail.id+') is a simple drop down.')) continue;

			if (debug >= 5) alert('removing all items from select');
			removeAllItemsFromSelect(ctlDetail);
			
			var ctlRow = findInArray(controlsBound, ctlDetail);
			var clones = ctlRow[1];
			if (debug >= 5) alert('found clone: ' + clones);
		
			var l = clones.length;
			if (debug >= 5) alert('length of clone: ' + l);
			
			var masterData = [];
			
			for (var j=1 ; j<l; j++)
			{
				var cloneVals = clones[j].getAttributeNS(JsMasterDetailNS, "detail-data").split(',');
				if (debug >= 3) alert('clone detail data: ' + cloneVals);
				var otherMasters = ctlRow[2];
				if (debug >= 5) alert('other masters for detail ' + ctlDetail.id +  ': ' + otherMasters[0].id + ', ' + otherMasters[1].id + ' (' + (otherMasters.length-2) + ' more)');
				
				if (masterData.length == 0)
				{
					if (debug >= 3) alert('initializing master data');
					for (var m=0; m < otherMasters.length; m++)
						for (var k=0; k < cloneVals.length; k++)
							if (new RegExp(cloneVals[k].split('=')[0]+'$').test(otherMasters[m].id))
								masterData[k] = getValue(otherMasters[m]);
					if (debug >= 3) alert('master data: ' + masterData);
				}

				var canAdd = true;
				var atLeastOneCriteria = false;
				for (var k=0; k < cloneVals.length; k++)
				{
					if (!masterData[k] || masterData[k].length == 0)
						continue;
						
					atLeastOneCriteria = true;
				
					var cloneVal = cloneVals[k].split('=')[1];
					if (cloneVal != masterData[k])
						canAdd = false;
				}
				if (canAdd && atLeastOneCriteria)
					ctlDetail.appendChild(clones[j].cloneNode(true));
			}
			
			if (ctlDetail.onchange)
				ctlDetail.onchange();
		}
		
		return true;
	}

}
if (document.all)
	JsMasterDetail = new JsMasterDetail();