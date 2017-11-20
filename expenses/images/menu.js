var ns4;
var op5;
var op6;
var agt;
var mac;
var ie; 
var mac_ie;

function sniffBrowsers() {
	ns4 = document.layers;
	op5 = (navigator.userAgent.indexOf("Opera 5")!=-1) ||(navigator.userAgent.indexOf("Opera/5")!=-1);
	op6 = (navigator.userAgent.indexOf("Opera 6")!=-1) ||(navigator.userAgent.indexOf("Opera/6")!=-1);
	agt=navigator.userAgent.toLowerCase();
	mac = (agt.indexOf("mac")!=-1);
	ie = (agt.indexOf("msie") != -1); 
	mac_ie = mac && ie;
}


function getStyleObject(objectId, doc) {
    if(document.getElementById && document.getElementById(objectId)) {
	return document.getElementById(objectId).style;
    } else if (document.all && document.all(objectId)) {
	return document.all(objectId).style;
    } else if (document.layers && document.layers[objectId]) {
		return getObjNN4(document,objectId);
    } else {
	return false;
    }
} 

function changeObjectVisibility(objectId, newVisibility) {
    var styleObject = getStyleObject(objectId, document);
    if(styleObject) {
	styleObject.visibility = newVisibility;
	return true;
    } else {
	return false;
    }
} 

function findImage(name, doc) {
	var i, img;
	for (i = 0; i < doc.images.length; i++) {
    	if (doc.images[i].name == name) {
			return doc.images[i];
		}
	}
	for (i = 0; i < doc.layers.length; i++) {
    	if ((img = findImage(name, doc.layers[i].document)) != null) {
			img.container = doc.layers[i];
			return img;
    	}
	}
	return null;
}

function getImage(name) {
	if (document.layers) {
    	return findImage(name, document);
	}
	return null;
}

function getObjNN4(obj,name)
{
	var x = obj.layers;
	var foundLayer;
	for (var i=0;i<x.length;i++)
	{
		if (x[i].id == name)
		 	foundLayer = x[i];
		else if (x[i].layers.length)
			var tmp = getObjNN4(x[i],name);
		if (tmp) foundLayer = tmp;
	}
	return foundLayer;
}

function getElementHeight(Elem) {
	if (ns4) {
		var elem = getObjNN4(document, Elem);
		return elem.clip.height;
	} else {
		var elem;
		if(document.getElementById) {
			var elem = document.getElementById(Elem);
		} else if (document.all){
			var elem = document.all[Elem];
		}
		if (op5) {
			xPos = elem.style.pixelHeight;
		} else {
			xPos = elem.offsetHeight;
		}
		return xPos;
	} 
}

function getElementWidth(Elem) {
	if (ns4) {
		var elem = getObjNN4(document, Elem);
		return elem.clip.width;
	} else {
		var elem;
		if(document.getElementById) {
			var elem = document.getElementById(Elem);
		} else if (document.all){
			var elem = document.all[Elem];
		}
		if (op5) {
			xPos = elem.style.pixelWidth;
		} else {
			xPos = elem.offsetWidth;
		}
		return xPos;
	}
}

function getElementLeft(Elem) {
	if (ns4) {
		var elem = getObjNN4(document, Elem);
		return elem.pageX;
	} else {
		var elem;
		if(document.getElementById) {
			var elem = document.getElementById(Elem);
		} else if (document.all){
			var elem = document.all[Elem];
		}
		xPos = elem.offsetLeft;
		tempEl = elem.offsetParent;
  		while (tempEl != null) {
  			xPos += tempEl.offsetLeft;
	  		tempEl = tempEl.offsetParent;
  		}
		return xPos;
	}
}


function getElementTop(Elem) {
	if (ns4) {
		var elem = getObjNN4(document, Elem);
		return elem.pageY;
	} else {
		if(document.getElementById) {	
			var elem = document.getElementById(Elem);
		} else if (document.all) {
			var elem = document.all[Elem];
		}
		yPos = elem.offsetTop;
		tempEl = elem.offsetParent;
		while (tempEl != null) {
  			yPos += tempEl.offsetTop;
	  		tempEl = tempEl.offsetParent;
  		}
		return yPos;
	}
}


function getImageLeft(myImage) {
	var x, obj;
	if (document.layers) {
		var img = getImage(myImage);
    	if (img.container != null)
			return img.container.pageX + img.x;
		else
			return img.x;
  	} else {
		return getElementLeft(myImage);
	}
	return -1;
}

function getImageTop(myImage) {
	var y, obj;
	if (document.layers) {
		var img = getImage(myImage);
		if (img.container != null)
			return img.container.pageY + img.y;
		else
			return img.y;
	} else {
		return getElementTop(myImage);
	}
	return -1;
}

function getImageWidth(myImage) {
	var x, obj;
	if (document.layers) {
		var img = getImage(myImage);
		return img.width;
	} else {
		return getElementWidth(myImage);
	}
	return -1;
}



function getImageHeight(myImage) {
	var y, obj;
	if (document.layers) {
		var img = getImage(myImage);
		return img.height;
	} else {
		return getElementHeight(myImage);
	}
	return -1;
}

function moveXY(myObject, x, y) {
	obj = getStyleObject(myObject)
	if (ns4) {
		obj.top = y;
 		obj.left = x;
	} else {
		if (op5) {
			obj.pixelTop = y;
 			obj.pixelLeft = x;
		} else {
			obj.top = y + 'px';
 			obj.left = x + 'px';
		}	
	}
}

function changeClass(Elem, myClass) {
	var elem;
	if(document.getElementById) {
		var elem = document.getElementById(Elem);
	} else if (document.all){
		var elem = document.all[Elem];
	}
	if (op5 || op6) elem.style.className = myClass;
	else elem.className = myClass;
}

function changeBGColour(myObject, colour) {
	if (ns4) {
		var obj = getObjNN4(document, myObject);
		obj.bgColor=colour;
	} else {
		var obj = getStyleObject(myObject);
		if (op5) {
			obj.background = colour;	
		} else {
			obj.backgroundColor = colour;
		}	
	}
}

function changeImage(target, source) {
	var imageObj;
	
	if (ns4) {
		imageObj = getImage(target);
		if (imageObj) imageObj.src = eval(source).src; 
	} else {
		imageObj = eval('document.images.' + target);
		if (imageObj) imageObj.src = eval(source).src; 
	}
}


var timeOn = null;

var numMenus = 50;

var currentMenuNo = 0;
var menuActive = new Array(numMenus);
var tier = new Array(numMenus);
var borderMod = new Array(numMenus);
var offClass = new Array(numMenus);
var onClass = new Array(numMenus);
var offColours = new Array(numMenus);
var onColours = new Array(numMenus);
//var labelBulletName = new Array(numMenus);
var menuType = new Array(numMenus);
var menus = new Array(numMenus);


//function bulletPoint(offURL, onURL) {	
//	this.offImage = new Image();
//	this.offImage.src = offURL;
//	this.onImage = new Image();
//	this.onImage.src = onURL;
//	this.URL = String(offURL);
//}

function openMe(newin) {
        flyout=window.open(newin,"flyout","")
}

function borderCell(B) {
	return '<td width="1" bgcolor="' + B + '"><img src="images/shim.gif" width="1" height="1" border="0"></td>';
}

function borderRow(B, C) {
	return '<tr><td height="1" colspan="' + C + '" bgcolor="' + B + '"><img src="images/shim.gif" width="1" height="1" border="0"></td></tr>';
}

function menuOver() {
	
	clearTimeout(timeOn);
}

function changeStatus(txt)
{
	window.status = 'Goto ' + txt;
}

function menuOut() {
	timeOn = setTimeout("hideAllMenus()", 1000);
}


function showMenu(m_No, eventObj) {
	hideAllMenusTier(tier[m_No]-1);
	var borderModSize = borderMod[m_No];
	if (ns4) {
		changeBGColour('menuLabel' + m_No, onColours[m_No]);
	} else {
		changeBGColour('labelCell' + m_No, onColours[m_No]);
		changeClass('menuLink' + m_No, onClass[m_No]);
	}
	//if (labelBulletName[m_No] != null){
	//	changeImage('menuBullet' + m_No, labelBulletName[m_No] + '.onImage');
	//}
	menuActive[m_No] = true;
	if (menuType[m_No] != 'blank') {
		if (ns4) labelObj = 'menuLabel'+m_No;
		else labelObj = 'labelCell'+m_No;
		
		x = getElementLeft(labelObj)-borderModSize;
		y = getElementTop(labelObj) + getElementHeight(labelObj);

		if (menus[m_No].align == 'center')  x = x + ((getElementWidth(labelObj)-getElementWidth('menu'+m_No))/2);
		if (menus[m_No].align == 'right') x = x + ((getElementWidth(labelObj)-getElementWidth('menu'+m_No))) + (borderModSize*2);

		moveXY('menu' + m_No, x, y);

		if(changeObjectVisibility('menu' + m_No, 'visible')) return true;
    	else return false;
	}
}

function showMenuSide(m_No, eventObj, myTier) {
	hideAllMenusTier(tier[m_No]-1);
	var borderModSize = borderMod[m_No];
	if (ns4) {
		changeBGColour('menuLabel' + m_No, onColours[m_No]);
	} else {
		changeBGColour('labelCell' + m_No, onColours[m_No]);
		changeClass('menuLink' + m_No, onClass[m_No]);
	}
	//if (labelBulletName[m_No] != null) changeImage('menuBullet' + m_No, labelBulletName[m_No] + '.onImage');
	menuActive[m_No] = true;
	if (menuType[m_No] != 'blank') {
		if (ns4) {
			labelObj = 'menuLabel'+m_No;
		} else {
			labelObj = 'labelCell'+m_No;
			if (mac_ie) labelObj = 'labelRow'+m_No;
		}
		x = getElementLeft(labelObj);
		y = getElementTop(labelObj) - borderModSize;

		if (menus[m_No].align=='right') x = x + getElementWidth(labelObj);
		else x = x - getElementWidth('menu'+m_No);

		moveXY('menu' + m_No, x, y);
	
		if(changeObjectVisibility('menu' + m_No, 'visible')) return true;
	    else return false;
	}
}

function hideAllMenus() {
	for (var i = 1; i < (currentMenuNo+1); i++) {
		if(menuActive[i] == true) hideMenu(i);
	}
}

function hideAllMenusTier(myTier) {
	for (var i = 1; i < (currentMenuNo+1); i++) {
		if( tier[i] > myTier && menuActive[i] == true) hideMenu(i);
	}
}

function hideMenu(m_No) {
	if (ns4) {
		changeBGColour('menuLabel' + m_No, offColours[m_No]);
	} else {
		changeBGColour('labelCell' + m_No, offColours[m_No]);
		changeClass('menuLink' + m_No, offClass[m_No]);
	}
	//if (labelBulletName[m_No] != null){
	//	changeImage('menuBullet' + m_No, labelBulletName[m_No] + '.offImage');
	//}
	menuActive[m_No] = false;
	
	if(changeObjectVisibility('menu' + m_No, 'hidden'))  return true;
    else return false;

}



function menuBar(barName, barWidth, orientation, i_Bor, o_Bor) {
	this.numLabels = 0;
	this.i_Bor = i_Bor;	
	this.o_Bor = o_Bor;	
	this.height = 15;
	this.orientation = orientation;
	this.labelText = new Array();
	this.rowText = new Array();
	this.offClass = 'MenuLabelLink';
	this.onClass = 'MenuLabelLinkOn';
	//this.bulletAlign = 'left';
	this.targetType = 'self'; // self, iframe, frame, new
	this.targetFrame = '_self'; // _self, _blank or (i)frame name
	
	
	this.addLabel = function(bullet, labelText, menuNo, labelWidth, offColour, onColour, labelURL, align) {
		this.numLabels += 1;
		tier[menuNo] = 0;		
		if (this.o_Bor != null) borderMod[menuNo] = 1;	
		else borderMod[menuNo] = 0;	
		if (menuNo != null) {
			onColours[menuNo] = onColour;
			offColours[menuNo] = offColour;
			onClass[menuNo] = this.onClass;
			offClass[menuNo] = this.offClass;
			//labelBulletName[menuNo] = bullet;
		}
		
		temp = new String('');
		this.rowText[this.numLabels] = new String('');
		if (this.orientation == 'vertical') this.rowText[this.numLabels] += '<tr id="labelRow'+ menuNo + '">';

		temp += '<td id="labelCell' + menuNo + '" width="'+ labelWidth + '" bgcolor="' + offColour + '" valign="middle" height="' + this.height + '" ';
		
		if (!ns4) {
			temp += ' onmouseout="menuOut(); "onmouseover="menuOver(); ';
			if (this.orientation == 'vertical') temp += 'return !showMenuSide(' + menuNo + ', event, tier[' + menuNo + ']);" ';
			else temp += 'return !showMenu(' + menuNo + ', event);" ';
			if (this.targetType=='self') temp += ' onclick="document.location.href=\'' + labelURL + '\';" ';
			if (this.targetType=='new') temp += ' onclick="openMe(\'' + labelURL + '\'); return false;" ';
			if (this.targetType=='frame') temp += ' onclick="parent.' + this.targetFrame + '.document.location.href=\'' + labelURL + '\';" ';
			if (this.targetType=='iframe') temp += ' onclick="' + this.targetFrame + '.location.href=\'' + labelURL + '\';" ';
		} 		
		temp +='>';
		
		if (ns4) {
			temp +='<ilayer><layer onmouseout="menuOut();" onmouseover="menuOver(); ';
			if (this.orientation == 'vertical') temp +='return !showMenuSide(' + menuNo + ', event, tier[' + menuNo + ']);" ';
			else temp +='return !showMenu(' + menuNo + ', event);" ';
		} else {
			temp +='<div ';
		}
		
		temp += ' class="toplevelmenu" width="' + labelWidth + '"  id="menuLabel' + menuNo +'">';
		
		
			temp += '<a runat="server" href="' + labelURL +'" target="' + this.targetFrame + '"';
		
		
		temp += ' class="' + this.offClass + '" id="menuLink' + menuNo +'">';
		
		
		//if (bullet != null) temp += '<img src="' + eval(bullet + ".URL") + '" border="0" align="' + this.bulletAlign + '" id="menuBullet' + menuNo + '" name="menuBullet' + menuNo + '">';
		
		temp += labelText;
		
		temp += '</a>';
		
		temp += '</span>';
		
		
		if (ns4) temp += '</layer></ilayer>';
		else temp += '</div>';
		
		temp += '</td>';		
		this.labelText[this.numLabels] = new String(temp);

	}
	
	this.addButton = function(bullet, labelText, menuNo, labelWidth, offColour, onColour, labelURL, align) {
		this.numLabels += 1;
		//tier[menuNo] = 0;		
		//if (this.o_Bor != null) borderMod[menuNo] = 1;	
		//else borderMod[menuNo] = 0;	
		//if (menuNo != null) {
		//	onColours[menuNo] = onColour;
		//	offColours[menuNo] = offColour;
		//	onClass[menuNo] = this.onClass;
		//	offClass[menuNo] = this.offClass;
		//	labelBulletName[menuNo] = bullet;
		//}
		
		temp = new String('');
		//this.rowText[this.numLabels] = new String('');
		//if (this.orientation == 'vertical') this.rowText[this.numLabels] += '<tr id="labelRow'+ menuNo + '">';

		temp += '<td id="labelCell' + menuNo + '" width="'+ labelWidth + '" bgcolor="' + offColour + '" valign="middle" height="' + this.height + '" ';
		
		if (!ns4) {
			temp += ' onmouseout=menuOut(); onmouseover=menuOver(); ';
			//if (this.orientation == 'vertical') temp += 'return !showMenuSide(' + menuNo + ', event, tier[' + menuNo + ']);" ';
			//else temp += 'return !showMenu(' + menuNo + ', event);" ';
			if (this.targetType=='self') temp += ' onclick="document.location.href=\'' + labelURL + '\';" ';
			if (this.targetType=='new') temp += ' onclick="openMe(\'' + labelURL + '\'); return false;" ';
			if (this.targetType=='frame') temp += ' onclick="parent.' + this.targetFrame + '.document.location.href=\'' + labelURL + '\';" ';
			if (this.targetType=='iframe') temp += ' onclick="' + this.targetFrame + '.location.href=\'' + labelURL + '\';" ';
		} 		
		temp +='>';
		
		
		
		temp += '<a runat=server href="' + labelURL +'" target="' + this.targetFrame + '" class="' + this.offClass + '" id="menuLink' + menuNo +'">';
		
		//if (bullet != null) temp += '<img src="' + eval(bullet + ".URL") + '" border="0" align="' + this.bulletAlign + '" id="menuBullet' + menuNo + '" name="menuBullet' + menuNo + '">';
			
		temp += labelText + '</a>';
		
		
		
		temp += '</td>';		
		this.labelText[this.numLabels] = new String(temp);

	}
	
	this.writeMenuBar = function() {
		o_Bor = this.o_Bor;
		i_Bor = this.i_Bor;
		menuBarStr = new String();
		menuBarStr += '<div id="' + barName + '"><table border="0" cellpadding="0" cellspacing="0">';
		if (this.orientation == 'vertical') {
			if (o_Bor != null) menuBarStr += borderRow(o_Bor, 3);
			for (var count = 0; count < this.numLabels; count++) {		
				menuBarStr += this.rowText[count+1];
				if (o_Bor != null) menuBarStr += borderCell(o_Bor);
				menuBarStr += this.labelText[count+1];
				if (o_Bor != null) menuBarStr += borderCell(o_Bor);				
				menuBarStr += '</tr>';
				if (i_Bor != null && count < (this.numLabels-1)) {
					if (o_Bor != null) menuBarStr += '<tr>' + borderCell(o_Bor) + borderCell(i_Bor) + borderCell(o_Bor) + '</tr>';
					else menuBarStr += borderRow(i_Bor, 1);
				}
			}
			if (o_Bor != null) menuBarStr += borderRow(o_Bor, 3);
		} else {
			if (o_Bor != null) {
				if (i_Bor != null) menuBarStr += borderRow(o_Bor, ((this.numLabels*2)+1));
				else menuBarStr += borderRow(o_Bor, (this.numLabels+2));
			}
			menuBarStr += '<tr>';
			if (o_Bor != null) menuBarStr += borderCell(o_Bor); 
			for (var count = 0; count < this.numLabels; count++) {		
				menuBarStr += this.rowText[count+1];			
				menuBarStr += this.labelText[count+1];
				if (i_Bor != null && count < (this.numLabels-1)) {
					menuBarStr += borderCell(i_Bor);
				}
			}
			if (o_Bor != null) {
				menuBarStr += borderCell(o_Bor);
				if (i_Bor != null) menuBarStr += borderRow(o_Bor, ((this.numLabels*2)+1));
				else menuBarStr += borderRow(o_Bor, (this.numLabels+2)); 
			}
			menuBarStr +=  '</tr>';
		}
		menuBarStr += '</table></div>';
		document.write(menuBarStr);
	}
}


function menu(menuWidth, orientation, i_Bor, o_Bor) {
	currentMenuNo += 1;
	this.numItems = 0;
	this.i_Bor = i_Bor;	
	this.o_Bor = o_Bor;	
	this.height = 15;
	this.itemText = new Array();
	this.rowText = new Array();
	this.align = 'left';
	this.offClass = 'MenuItemLink';
	this.onClass = 'MenuItemLinkOn';
	this.orientation = orientation;
	//this.bulletAlign = 'left';
	this.targetType = 'self'; // self, iframe, frame, new
	this.targetFrame = '_self'; // _self, _blank or (i)frame name

	this.addItem = function(bullet, itemText, menuNo, itemWidth, offColour, onColour, itemURL, align) {
		this.numItems += 1;
		
		var tempId = currentMenuNo + '_' + this.numItems;
		var statustxt = itemText;
		if (menuNo != null) {
			tier[menuNo] = tier[currentMenuNo] + 1;
			onColours[menuNo] = onColour;
			offColours[menuNo] = offColour;
			onClass[menuNo] = this.onClass;
			offClass[menuNo] = this.offClass;
			//labelBulletName[menuNo] = bullet;
			if (this.o_Bor != null) borderMod[menuNo] = 1;	
			else borderMod[menuNo] = 0;	
		}
		
		temp = new String('');
		this.rowText[this.numItems] = new String('');
		
		if (menuNo != null) {
			if (this.orientation =='vertical') this.rowText[this.numItems] += '<tr id="labelRow'+ menuNo + '">';
			temp += '<td id="labelCell'+ menuNo + '" width="'+ itemWidth + '" bgcolor="' + offColour + '" valign="middle" height="' + this.height + '" ';
		} else {
			if (this.orientation =='vertical') this.rowText[this.numItems] += '<tr>';
			temp += '<td id="itemCell' + tempId + '" width="'+ itemWidth + '" bgcolor="' + offColour + '" valign="middle" height="' + this.height + '" class="' + align + 'Menu"';
		}		
		if (!ns4) {
			if (this.targetType=='self') temp += ' onclick="document.location.href=\'' + itemURL + '\';" ';
			if (this.targetType=='new') temp += ' onclick="openMe(\'' + itemURL + '\'); return false;" ';
			if (this.targetType=='frame') temp += ' onclick="parent.' + this.targetFrame + '.document.location.href=\'' + itemURL + '\';" ';
			if (this.targetType=='iframe') temp += ' onclick="' + this.targetFrame + '.location.href=\'' + itemURL + '\';" ';
			if (menuNo != null) {			
				if (this.orientation =='vertical') temp += ' onmouseover="menuOver(); return !showMenuSide(' + menuNo + ', event, tier[' + menuNo + ']); " onmouseout=" menuOut(); "';
				else temp += ' onmouseover="menuOver(); return !showMenu(' + menuNo + ', event); " onmouseout=" menuOut(); "';
			} else {
				temp += ' onmouseover="changeClass(\'menuLink' + tempId + '\',\'' + this.onClass + '\'); hideAllMenusTier(tier[' + currentMenuNo + ']); menuOver();  changeBGColour(\'itemCell' + tempId + '\', \'' + onColour + '\'); " onmouseout="menuOut(); changeClass(\'menuLink' + tempId + '\',\'' + this.offClass + '\'); changeBGColour(\'itemCell' + tempId + '\', \'' + offColour + '\'); "';
			}
		} 
		temp += '>';
				
		if (ns4) {
			temp += '<ilayer><layer ';
			if(menuNo != null) { 
				if (this.orientation == 'vertical') temp += 'onmouseover="menuOver(); return !showMenuSide(' + menuNo + ', event, tier[' + menuNo + ']);" onmouseout="menuOut();"';
				else temp += 'onmouseover="menuOver(); return !showMenu(' + menuNo + ', event);" onmouseout="menuOut();"';
			} else {
				temp += 'onmouseover="hideAllMenusTier(tier[' + currentMenuNo + ']); menuOver(); changeBGColour(\'menuItem' + tempId + '\', \'' + onColour + '\'); " onmouseout="menuOut(); changeBGColour(\'menuItem' + tempId + '\', \'' + offColour + '\'); "';
			}
		} else {
			temp +='<div onmouseover="changeStatus(\'' + statustxt + '\');"';
		}
		temp += ' class="menuitem" width="' + itemWidth + '"';
	
		if (menuNo != null) temp += ' id="menuLabel' + menuNo +'"';
		else temp += ' id="menuItem' + tempId +'"';
		
		temp += '><a runat="server" ';
		if (itemURL != '')
		{
			temp += 'onmouseover="changeStatus(\'' + statustxt + '\');" ';
		}
		temp += 'href="' + itemURL +'" target="' + this.targetFrame + '" class="' + this.offClass + '"';
		if (menuNo != null) temp += 'id="menuLink' + menuNo +'"';
		else temp += 'id="menuLink' + tempId +'"';
		temp +='>';
		
		//if (bullet != null) {
		//	if (menuNo != null) temp += '<img src="' + eval(bullet + ".URL") + '" border="0" align="' + this.bulletAlign + '" id="menuBullet' + menuNo + '" name="menuBullet' + menuNo + '">';
		//	else temp += '<img src="' + eval(bullet + ".URL") + '" border="0" align="' + this.bulletAlign + '" id="menuItemBullet' + tempId + '" name="menuItemBullet' + tempId + '">';
		//}
			
		temp += itemText + '</a>';
		
		if (ns4) temp += '</layer></ilayer>';
		else temp += '</div>';

		temp += '</td>';	
		this.itemText[this.numItems] = new String(temp);
	}
	
	this.writeMenu = function() {
		var menuStr = new String();
		o_Bor = this.o_Bor;
		i_Bor = this.i_Bor;
		if (this.numItems == 0) menuType[currentMenuNo] = 'blank';
		else menuType[currentMenuNo] = 'default';
		menuStr += '<div id="menu' + currentMenuNo + '" name="menu' + currentMenuNo + '" class="myMenu" width="' + menuWidth + '"';
		if (!ns4) menuStr += ' style="width:' + menuWidth + ';"';
		menuStr+= '><table border="0" cellpadding="0" cellspacing="0" width="' + menuWidth + '">';
		if (this.orientation == 'vertical') {
			if (o_Bor != null) menuStr += borderRow(this.o_Bor, 3);
			for (var count = 0; count < this.numItems; count++) {
				menuStr += this.rowText[count+1];
				if (o_Bor != null) menuStr += borderCell(o_Bor);
				menuStr += this.itemText[count+1];
				if (o_Bor != null) menuStr += borderCell(o_Bor);
				menuStr += '</tr>';
				if (i_Bor != null && count < (this.numItems-1)) {
					if (o_Bor != null) menuStr += '<tr>' + borderCell(o_Bor) + borderCell(i_Bor) + borderCell(o_Bor) + '</tr>';
					else menuStr += borderRow(i_Bor, 1);
				}
			}
			if (o_Bor != null) menuStr += borderRow(o_Bor, 3);
		} else {
			if (o_Bor != null) {
				if (i_Bor != null) menuStr += borderRow(o_Bor, ((this.numItems*2)+1));
				else menuStr += borderRow(o_Bor, (this.numItems+2));
			}
			menuStr += '<tr>';
			if (o_Bor != null) menuStr += borderCell(o_Bor); 
			for (var count = 0; count < this.numItems; count++) {		
				menuStr += this.rowText[count+1];			
				menuStr += this.itemText[count+1];
				if (i_Bor != null && count < (this.numItems-1)) {
					menuStr += borderCell(i_Bor);
				}
			}
			if (o_Bor != null) {
				menuStr += borderCell(o_Bor);
				if (i_Bor != null) menuStr += borderRow(o_Bor, ((this.numItems*2)+1));
				else menuStr += borderRow(o_Bor, (this.numItems+2)); 
			}
			menuStr +=  '</tr>';
		}
		menuStr += '</table></div>';
		document.write(menuStr);
	}
}

