// Ext.Loader.setConfig({enabled:true,paths:{"Ext.ux":"ux",Overrides:"overrides"}});
// Ext.grid.RowEditor.prototype.cancelBtnText="Cancel";
// Ext.grid.RowEditor.prototype.saveBtnText="Save";

// Ext.JSON.encodeDate = function(b) {
//     return'"' + Ext.Date.format(b,"c") + '"'
// };

// Ext.popupMsg = function() {
//     var c;function d(b,a){if(b=="Warning"){return'<div class="msgError"><p align="center"><h3>'+a+"</h3></p></div>"}else{return'<div class="msgSuccess"><p align="center"><h3>'+a+"</h3></p></div>"}}return{msg:function(a,b){if(!c){if(a=="Warning"){c=Ext.DomHelper.insertFirst(document.body,{id:"app-popup-error-div"},true)}else{c=Ext.DomHelper.insertFirst(document.body,{id:"app-popup-success-div"},true)}}var g=Ext.String.format.apply(String,Array.prototype.slice.call(arguments,1));var h=Ext.DomHelper.append(c,d(a,g),true);h.hide();h.slideIn("t").ghost("t",{delay:1200,remove:true})},init:function(){if(!c){c=Ext.DomHelper.insertFirst(document.body,{id:"app-popup-success-div"},true)}}}}();

//     Ext.onReady(function() {
//         Ext.popupMsg.init;function d(){idleTime=idleTime+1;if(idleTime>=30){function a(b){if(b!=="yes"){Ext.util.Cookies.clear("CBHUserAuth");
//         Ext.util.Cookies.clear("CurrentUser");window.location.reload()}}Ext.Msg.show({title:"Inactivity Detected",msg:"Do you want keep this session?",buttons:Ext.Msg.YESNO,icon:Ext.Msg.QUESTION,closable:false,fn:a,icon:Ext.Msg.Info})}}var c=setInterval(d,60*1000);document.onmousemove=function(a){idleTime=0};document.onkeypress=function(a){idleTime=0}});Ext.application({requires:["Ext.ux.Router","Ext.ux.form.Toolbar","Ext.ux.form.NumericField","Ext.ux.window.NumberPrompt","Overrides.form.field.Base","Overrides.form.ComboBox","Overrides.view.Table","Overrides.data.Store","Overrides.toolbar.Paging"],routes:{"/":"home#init",logon:"logon#passport","users/:id/edit":"users#edit"},controllers:["Home","Logon","Sales"],name:"CBH",launch:function(){Ext.ux.Router.on({routemissed:function(b){Ext.Msg.show({title:"Error 404",msg:"Route not found: "+b,buttons:Ext.Msg.OK,icon:Ext.Msg.ERROR})},beforedispatch:function(d,e,f){},dispatch:function(h,e,g,f){if(f.id=="Home"){Ext.create("CBH.view.Viewport")}}})}});

Ext.Loader.setConfig({
    enabled: true,
    disableCaching: false,
    paths: {
        'Ext.ux': 'ux',
        'Ext.ux.DataView': 'ux/DataView/',
        'Overrides': 'overrides'
    }
});

Ext.grid.RowEditor.prototype.cancelBtnText = "Cancel";
Ext.grid.RowEditor.prototype.saveBtnText = "Save";


// Format date to UTC
Ext.JSON.encodeDate = function(o) {
    return '"' + Ext.Date.format(o, 'c') + '"';
};

Ext.popupMsg = function() {
    var msgCt;

    function createBox(t, s) {
        if (t == "Warning") {
            return '<div class="msgError"><p align="center"><h3>' + s + '</h3></p></div>';
        } else {
            //return '<div class="msgSuccess"><div class="app-check"/><h3>' + s + '</h3></div>';
            return '<div class="msgSuccess"><p align="center"><h3>' + s + '</h3></p></div>';
        }
    }
    return {
        msg: function(title, format) {
            //if(!msgCt){
            if (title == "Warning") {
                msgCt = Ext.DomHelper.insertFirst(document.body, {
                    id: 'app-popup-error-div'
                }, true);
            } else {
                msgCt = Ext.DomHelper.insertFirst(document.body, {
                    id: 'app-popup-success-div'
                }, true);
            }
            //};
            var s = Ext.String.format.apply(String, Array.prototype.slice.call(arguments, 1));
            var m = Ext.DomHelper.append(msgCt, createBox(title, s), true);
            m.hide();
            m.slideIn('t').ghost("t", {
                delay: 1200,
                remove: true
            });
        },

        init: function() {
            //if(!msgCt){
            // It's better to create the msg-div here in order to avoid re-layouts 
            // later that could interfere with the HtmlEditor and reset its iFrame.
            //msgCt = Ext.DomHelper.insertFirst(document.body, {id:'app-popup-success-div'}, true);
            //}
        }
    };
}();


// custom Vtype for vtype:'arancel'
var arancelTest = /^\d{4}?[\.]?\d{2}[\.]?\d{4}$/i;
Ext.apply(Ext.form.field.VTypes, {
    //  vtype validation function
    arancel: function(val, field) {
        return arancelTest.test(val);
    },
    // vtype Text property: The error text to display when the validation function returns false
    arancelText: 'Not a valid.  Must be in the format "9999.99.9999".',
    // vtype Mask property: The keystroke filter mask
    arancelMask: /^\d{4}?[\.]?\d{2}[\.]?\d{4}$/i
});

Ext.ux.LoaderX = Ext.apply({}, {
    load: function(fileList, callback, scope, preserveOrder) {
        var scope = scope || this,
            head = document.getElementsByTagName("head")[0],
            fragment = document.createDocumentFragment(),
            numFiles = fileList.length,
            loadedFiles = 0,
            me = this;

        // Loads a particular file from the fileList by index. This is used when preserving order
        var loadFileIndex = function(index) {
            head.appendChild(
                me.buildScriptTag(fileList[index], onFileLoaded)
            );
        };

        /**
         * Callback function which is called after each file has been loaded. This calls the callback
         * passed to load once the final file in the fileList has been loaded
         */
        var onFileLoaded = function() {
            loadedFiles++;

            //if this was the last file, call the callback, otherwise load the next file
            if (numFiles == loadedFiles && typeof callback == 'function') {
                callback.call(scope);
            } else {
                if (preserveOrder === true) {
                    loadFileIndex(loadedFiles);
                }
            }
        };

        if (preserveOrder === true) {
            loadFileIndex.call(this, 0);
        } else {
            //load each file (most browsers will do this in parallel)
            Ext.each(fileList, function(file, index) {
                fragment.appendChild(
                    this.buildScriptTag(file, onFileLoaded)
                );
            }, this);

            head.appendChild(fragment);
        }
    },

    buildScriptTag: function(filename, callback) {
        var exten = filename.substr(filename.lastIndexOf('.') + 1);
        var today = new Date(),
            href = '?_DC=' + today.getTime();

        if (exten == 'js') {
            var script = document.createElement('script');
            script.type = "text/javascript";
            script.src = filename + href;

            //IE has a different way of handling <script> loads, so we need to check for it here
            if (script.readyState) {
                script.onreadystatechange = function() {
                    if (script.readyState == "loaded" || script.readyState == "complete") {
                        script.onreadystatechange = null;
                        callback();
                    }
                };
            } else {
                script.onload = callback;
            }
            return script;
        }
        if (exten == 'css') {
            var style = document.createElement('link');
            style.rel = 'stylesheet';
            style.type = 'text/css';
            style.href = filename + href;
            callback();
            return style;
        }
    }
});

Ext.onReady(function() {
    Ext.tip.QuickTipManager.init();


    function timerIncrement() {
        idleTime = idleTime + 1;

        if (idleTime >= 60) { // 60 minutes
            var out = function(btn) {
                if (btn !== "yes") {
                    Ext.util.Cookies.clear("CBHUserAuth");
                    Ext.util.Cookies.clear("CurrentUser");
                    window.location.reload();
                }
            };

            Ext.Msg.show({
                title: 'Inactivity Detected',
                msg: "Do you want keep this session?",
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                closable: false,
                fn: out
            });

        }
    }

    // Load some style resources
    Ext.ux.LoaderX.load(
        ['app/resources/css/app.css', 'app/resources/css/data-view.css', 'app/resources/css/fonts.css'],
        function() {
            document.getElementById("loading").style.display = 'none';
        }
    );

    //Increment the idle time counter every minute.
    var idleInterval = setInterval(timerIncrement, 60 * 1000); // 1 minute

    //Zero the idle timer on mouse movement.
    document.onmousemove = function(e) {
        idleTime = 0;
    };
    document.onkeypress = function(e) {
        idleTime = 0;
    };
    document.onmousedown = function(e) {
        idleTime = 0;
    };
    document.onmouseup = function(e) {
        idleTime = 0;
    };
});

Ext.setGlyphFontFamily('FontAwesome');

Ext.application({

    requires: [
        'CBH.GlobalSettings',
        'CBH.AppEvents',
        'CBH.view.Viewport',
        'Ext.ux.Router',
        'Ext.ux.form.Toolbar',
        'Ext.ux.form.NumericField',
        'Ext.ux.CheckColumn',
        'Ext.ux.CheckColumnPatch',
        'Ext.ux.form.SearchField',
        'Ext.ux.form.field.TimePickerField',
        'Ext.ux.form.DateTimePicker',
        'Ext.ux.form.field.DateTimeField',
        'Ext.ux.form.DateTimeMenu',
        'Ext.ux.InputTextMask',
        'Overrides.form.field.Date',
        'Overrides.form.field.Base',
        'Overrides.form.ComboBox',
        'Overrides.view.Table',
        'Overrides.data.Store',
        'Overrides.data.proxy.Proxy',
        'Overrides.toolbar.Paging',
        'Overrides.util.Format',
        'Overrides.grid.column.Action'
    ],

    routes: {
        '/': 'home#init',
        'logon': 'logon#passport',
        'users/:id/edit': 'users#edit'
    },

    controllers: [
        'Home',
        'Logon'
    ],

    //autoCreateViewport: true,

    name: 'CBH',

    launch: function() {
        /* 
         * Ext.ux.Router provides some events for better controlling
         * dispatch flow
         */
        Ext.ux.Router.on({

            routemissed: function(token) {
                Ext.Msg.show({
                    title: 'Error 404',
                    msg: 'Route not found: ' + token,
                    buttons: Ext.Msg.OK,
                    icon: Ext.Msg.ERROR
                });
            },

            beforedispatch: function(token, match, params) {
                //consolex.log('beforedispatch ' + token);
            },

            /**
             * For this example I'm using the dispatch event to render the view
             * based on the token. Each route points to a controller and action. 
             * Here I'm using these 2 information to get the view and render.
             */
            dispatch: function(token, match, params, controller) {

                //document.getElementById("loading").style.display = 'none';

                if (controller.id == "Home") {
                    Ext.create("CBH.view.Viewport");
                }
            }
        });
    }
});
