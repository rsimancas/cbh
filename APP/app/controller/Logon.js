Ext.define('CBH.controller.Logon', {
    extend: 'Ext.app.Controller',

    models: [
        'common.Users'
    ],
    stores: [
        'common.Users',
    ],
    views: [
        'common.Logon',
    ],

    init: function() {
        this.control({
            'logon button[text="Submit"]': {
                click: this.onSubmitLogon
            },
            'logon textfield[fieldLabel="Password"]': {
                keypress: this.onLogonTextFieldKeypress
            },
            'logon textfield[name="Captcha"]': {
                change: function(field) {}
            }
        });
    },

    passport: function(application) {
        viewport = Ext.create('Ext.Viewport', {
            cls: 'custom-viewport',
            renderTo: Ext.getBody()
        });
        view = Ext.create('CBH.view.common.Logon', {
            //stateful: true,
            autoRender: true,
            autoShow: true
        });

        //view.down('form').center();
        viewport.add(view);
    },

    onLogonTextFieldKeypress: function(textfield, e, eOpts) {
        var me  = textfield.up('form');
        if (e.getCharCode() == Ext.EventObject.ENTER) {
            //enter is pressed call the next buttons handler function here.
            var but = textfield.up('form').down('toolbar').down('button');
            but.fireEvent("click", but);
        }
    },

    onSubmitLogon: function(button, e, eOpts) {

        // Obtenemos el formulario actual
        var me = button.up('form');
        var currentForm = me.getForm();

        

        // Se obtiene el modelo de datos
        record = Ext.create('CBH.model.common.Users');

        // Actualizamos los campos del Formulario en el modelo
        currentForm.updateRecord(record);

        /*
        var val = record.data.UserName;

        if (val.indexOf(String.fromCharCode(101)) >= 0 ) {
            val = val.replace(String.fromCharCode(101)+String.fromCharCode(101),String.fromCharCode(101));
            record.data.UserName = val;
        }
        */

        // Encriptamos el valor cargado en el formulario
        // var encrypted = CryptoJS.SHA1(record.data.UserPasword);

        // record.data.UserPasword = encrypted.toString();

        //Ext.Msg.wait('Starting Session...', 'Wait');

        me.up("viewport").getEl().mask("Starting Session...");


        Ext.Ajax.request({
            url: CBH.GlobalSettings.webApiPath + '/api/auth',
            jsonData: Ext.JSON.encode(record),
            timeout: 120000,

            // Functions that fire (success or failure) when the server responds. 
            // The one that executes is determined by the 
            // response that comes from login.asp as seen below. The server would 
            // actually respond with valid JSON, 
            // something like: response.write "{ success: true}" or 
            // response.write "{ success: false, errors: { reason: 'Login failed. Try again.' }}" 
            // depending on the logic contained within your server script.
            // If a success occurs, the user is notified with an alert messagebox, 
            // and when they click "OK", they are redirected to whatever page
            // you define as redirect. 

            success: function(response, opts) {
                var d = new Date();
                //var expiry = new Date(now.getTime()+(24*3600*1000)); // Ten minutes
                var expiry = new Date(d.setHours(23, 59, 59, 999)); // at end of day
                var result = Ext.JSON.decode(response.responseText);

                Ext.util.Cookies.set('CBH.CurrentUser', Ext.JSON.encode(result.data), expiry);
                Ext.util.Cookies.set('CBH.UserAuth', result.security, expiry);

                if(record.data.RememberMe) {
                    var userLog = Ext.JSON.encode(record.data);
                    var encrypted = CryptoJS.AES.encrypt(userLog, CBH.GlobalSettings.CipherPass);
                    expiry.setTime(expiry.getTime()+(90*24*60*60*1000));
                    Ext.util.Cookies.set("CBH.Log", encrypted, expiry);
                } else {
                    Ext.util.Cookies.clear('CBH.Log');
                }

                me.up("viewport").getEl().unmask();
                var url = location.href;
                url = url.split('#');
                location.href = url[0];
            },

            // Failure function, see comment above re: success and failure. 
            // You can see here, if login fails, it throws a messagebox
            // at the user telling him / her as much.  

            failure: function(response, opts) {
                me.up("viewport").unmask();
                Ext.Msg.alert('Warning!',
                    'Login Failed. Try again.',
                    function() { 
                        me.down('field[name=UserPassword]').focus(true, 200);
                    },
                    me
                );
                //currentForm.reset(); 
            }
        });
    },
});
