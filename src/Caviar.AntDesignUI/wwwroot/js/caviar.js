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




//移除iframe，显示wasm
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

function open_blank(url) {
	window.open(url, "_blank")
}


//iframe内发送消息
function iframeMessage(message) {
	window.parent.postMessage(message, '*');
}


//iframe外监听
window.addEventListener('message', function (e) { 
	console.log(e.data);
	if (e.data.pattern == 1) {
		window.location.href = e.data.url;
	}
	else {
		try {
			DotNet.invokeMethodAsync("Caviar.AntDesignUI", "JsNavigation", e.data)
				.then(data => {
					console.log(data);
				});
		}
		catch {
			//wasm尚未加载完毕
        }
    }
	
})


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

window.onbeforeunload = onbeforeunload_handler;
function onbeforeunload_handler() {
	console.log(document.activeElement.href);
}

function loadJS(url, callback) {
	var script = document.createElement('script'),
        fn = callback || function () {
            console.log("wasm加载完毕");
        };
    script.type = 'text/javascript';
    script.autostart = false;
	if (script.readyState) {
        script.onreadystatechange = function () {
            Blazor.start({
                loadBootResource: function (type, name, defaultUri, integrity) {
                    if (type !== 'dotnetjs' && location.hostname !== 'localhost') {
                        return (async function () {
                            const response = await fetch(defaultUri + '.br', { cache: 'no-cache' });
                            if (!response.ok) {
                                throw new Error(response.statusText);
                            }
                            const originalResponseBuffer = await response.arrayBuffer();
                            const originalResponseArray = new Int8Array(originalResponseBuffer);
                            const decompressedResponseArray = BrotliDecode(originalResponseArray);
                            const contentType = type ===
                                'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
                            return new Response(decompressedResponseArray,
                                { headers: { 'content-type': contentType } });
                        })();
                    }
                }
            });
			if (script.readyState == 'loaded' || script.readyState == 'complete') {
				script.onreadystatechange = null;
				fn();
			}
		};
	} else {
		//其他浏览器
		script.onload = function () {
			fn();
		};
	}
	script.src = url;
	script.async = 'async';
    document.getElementsByTagName('head')[0].appendChild(script);
}
