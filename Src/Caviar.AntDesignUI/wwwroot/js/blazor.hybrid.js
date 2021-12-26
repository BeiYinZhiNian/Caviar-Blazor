﻿(function () {

    var getElementByTagName = function (name) {
        return document.getElementsByTagName(name)[0];
    }

    var srvrApp = getElementByTagName("srvr-app");
    var wasmApp = getElementByTagName("wasm-app");

    var windowAddEventActual = window.addEventListener;
    var documentAddEventActual = document.addEventListener;

    var wasmNavigateTo = null;
    var captureListeners = false;

    var addServerEvent = function (type, listener, options) {
        srvrApp.addEventListener(type, listener, options);
    }

    var wasmListeners = [];
    var addWasmEvent = function (type, listener, options) {
        if (type !== 'click' && type !== 'message' && type !== 'popstate') {
            addServerEvent(type, listener, options);
        }
    
        if (captureListeners) {
            wasmListeners.push({ type, listener, options });
        }
    }

    var loadScript = function (name, callback) {
        var script = document.createElement('script');
        script.onload = callback;
        if (name === "webassembly") {
            script.setAttribute("autostart", "false");
        }
        script.src = '_framework/blazor.' + name + '.js';
        document.head.appendChild(script);
    }

    var blazorInfo = function (message) {
        console.info('[' + new Date().toISOString() + '] Information: ' + message);
    }

    var loadWasmFunction = function () {

        if (getElementByTagName('srvr-app').innerHTML.indexOf('<!--Blazor:{') !== -1) {
            setTimeout(loadWasmFunction, 100);
            return;
        }

        window.addEventListener = addWasmEvent;
        document.addEventListener = addWasmEvent;

        captureListeners = true;

        loadScript('webassembly', function () {

            wasmNavigateTo = window.Blazor._internal.navigationManager.navigateTo;
            window.Blazor._internal.navigationManager.navigateTo = window.BlazorServer._internal.navigationManager.navigateTo;

            window.Blazor.start();
        });
    };

    var init = function () {
        var wasmReadyToSwitch = false;

        window.addEventListener = addServerEvent;
        document.addEventListener = addServerEvent;

        loadScript('server', function () {
            window.BlazorServer = window.Blazor;
        });

        setTimeout(loadWasmFunction, 100);

        window.wasmReady = function () {
            blazorInfo('Wasm ready');

            wasmReadyToSwitch = true;

            if (window.hybridType === 'HybridOnReady') {
                window.switchToWasm(window.location.href);
            }
        }

        window.switchToWasm = function (location, manual) {
            if (window.hybridType === 'HybridManual' && !manual) {
                return true;
            }

            if (manual) {
                location = window.location.href;
            }

            if (!wasmReadyToSwitch) return false;
            wasmReadyToSwitch = false;

            blazorInfo('Switch to wasm');

            setTimeout(function () {

                srvrApp.parentNode.removeChild(srvrApp);

                for (var l of wasmListeners) {
                    wasmApp.addEventListener(l.type, l.listener, l.options);
                }

                window.wasmListeners = wasmListeners

                window.addEventListener = windowAddEventActual;
                document.addEventListener = documentAddEventActual;
                
                window.BlazorServer.defaultReconnectionHandler.onConnectionDown = () => { };
                window.BlazorServer._internal.forceCloseConnection();

                window.Blazor._internal.navigationManager.navigateTo = wasmNavigateTo;

                wasmNavigateTo(location, false, false);

                wasmApp.style.display = "block";
                srvrApp.style.display = "none";
            }, 0);

            return true;
        }
    }

    init();

})();