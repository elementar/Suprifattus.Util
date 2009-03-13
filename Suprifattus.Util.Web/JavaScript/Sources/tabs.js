function toggleTab(strip, id)
{
	var scrollY = window.pageYOffset;
	
	var tabs = document.getElementById(strip);
	l = tabs.getElementsByTagName('li');
	for (var i=0; i < l.length; i++)
		removeCSSClass(l[i], 'selected');
	
	var newTab = document.getElementById('tab' + id);
	var newTabLink = document.getElementById('tablink' + id);
	
	appendCSSClass(newTabLink, 'selected');
	//newTab.style.display = 'block';
	appendCSSClass(newTab, 'tab-selected');
	
	l = tabs.parentNode.getElementsByTagName('ul');
	for (var i=0; i < l.length; i++)
		if (l[i] != newTab) 
			removeCSSClass(l[i], 'tab-selected');

	l = newTab.getElementsByTagName('input');
	if (l.length == 0) l = newTab.getElementsByTagName('textarea');
	if (l.length == 0) l = newTab.getElementsByTagName('select');
	if (l.length > 0)
		l[0].focus();
	
	window.scrollTo(0, scrollY);
}
