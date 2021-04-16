/*
name:cookie 名
value:cookie 值
*/
//写入cookie
function setCookie(name, value, Days) {
	var exp = new Date();
	exp.setTime(exp.getTime() + Days * 60 * 1000);//60*1000=1分钟
	document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

///删除cookie
function delCookie(name) {
	var exp = new Date();
	exp.setTime(exp.getTime() - 1);
	var cval = getCookie(name);
	if (cval != null) document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
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