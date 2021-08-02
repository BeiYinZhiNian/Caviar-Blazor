window.AntDesign.Prism = {};
window.AntDesign.Prism.highlight = function (code, language) {
	return Prism.highlight(code, Prism.languages[language], language);
}

window.AntDesign.Prism.highlightAll = function () {
	Prism.highlightAll();
}

/*
name:cookie 名
value:cookie 值
*/
//写入cookie
function setCookie(name, value, Days) {
	var exp = new Date();
	exp.setTime(exp.getTime() + Days * 60 * 1000);//60*1000=1分钟
	document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
}

///删除cookie
function delCookie(name) {
	var exp = new Date();
	exp.setTime(exp.getTime() - 1);
	var cval = getCookie(name);
	if (cval != null) document.cookie = name + "=" + cval + ";path=/;expires=" + exp.toGMTString();
}

// 获取指定名称的cookie
function getCookie(name) {
	var strcookie = document.cookie;//获取cookie字符串
	var arrcookie = strcookie.split("; ");//分割
	//遍历匹配
	for (var i = 0; i < arrcookie.length; i++) {
		var arr = arrcookie[i].split("=");
		if (arr[0] == name) {
			return arr[1];
		}
	}
	return "";
}

function getClientWidth() {
	return document.body.clientWidth
}

function loadCss(oldFile,newFile) {
	replacejscssfile(oldFile, newFile, "css");
}

function replacejscssfile(oldfilename, newfilename, filetype) {
	var targetelement = (filetype == "js") ? "script" : (filetype == "css") ? "link" : "none"
	var targetattr = (filetype == "js") ? "src" : (filetype == "css") ? "href" : "none"
	var allsuspects = document.getElementsByTagName(targetelement)
	for (var i = allsuspects.length; i >= 0; i--) {
		if (allsuspects[i] && allsuspects[i].getAttribute(targetattr) != null && allsuspects[i].getAttribute(targetattr).indexOf(oldfilename) != -1) {
			var newelement = createjscssfile(newfilename, filetype)
			allsuspects[i].parentNode.replaceChild(newelement, allsuspects[i])
		}
	}
}

function createjscssfile(filename, filetype) {
	if (filetype == "js") {
		var fileref = document.createElement('script')
		fileref.setAttribute("type", "text/javascript")
		fileref.setAttribute("src", filename)
	}
	else if (filetype == "css") {
		var fileref = document.createElement("link")
		fileref.setAttribute("rel", "stylesheet")
		fileref.setAttribute("type", "text/css")
		fileref.setAttribute("href", "_content/AntDesign/css/"+filename)
	}
	return fileref
}
