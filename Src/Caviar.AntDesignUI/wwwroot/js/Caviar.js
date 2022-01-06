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





function switch_wasm() {
	wasm_app = document.getElementById("wasm_app");
	iframe_div = document.getElementById("iframe_div");
	wasm_app.style.display = "block";
	iframe_div.style.display = "none";
	var iframe = document.getElementById("iframe_Server");
	iframe.parentNode.removeChild(iframe);
}

function switch_server(url) {
	wasm_app = document.getElementById("wasm_app");
	wasm_app.style.display = "none";
	iframe_div = document.getElementById("iframe_div");
	iframe_div.innerHTML = '<iframe id="iframe_Server" src="' + url + '?IsAutomaticSwitchWasm=false&server=true" style="width:100%; height:100%;border:medium none"></iframe>'
	iframe_div.style.display = "block"

}



//iframe内发送消息
function iframeMessage(message) {
	window.parent.postMessage(message, '*');
}


//iframe外监听
window.addEventListener('message', function (e) { 
	console.log(e.data);
	DotNet.invokeMethod("Caviar.AntDesignUI", "JsNavigation", e.data)
})
