window.AntDesign.Prism = {};
window.AntDesign.Prism.highlight = function (code, language) {
	return Prism.highlight(code, Prism.languages[language], language);
}

window.AntDesign.Prism.highlightAll = function () {
	Prism.highlightAll();
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

//iframe内发送消息
function iframeMessage(message) {
	window.parent.postMessage(message, '*');
}





function switch_wasm() {
	wasm_app = document.getElementById("wasm_app");
	iframe_Server = document.getElementById("iframe_div");
	wasm_app.style.display = "block";
	iframe_Server.style.display = "none";
	var iframe = document.getElementById("iframe_Server");
	iframe.parentNode.removeChild(iframe);
}


//iframe外监听
window.addEventListener('message', function (e) { 
	console.log(e.data);
	DotNet.invokeMethod("Caviar.AntDesignUI", "JsNavigation", e.data)
})
