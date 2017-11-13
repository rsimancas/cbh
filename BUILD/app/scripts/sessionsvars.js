var tok = 'WORKING-PC\\rony_simancas@hotmail.com:2theinfinity';
var hash = Base64.encode(tok);
var auth = "User1";
var idleTime = 0;

// write cookie
//Ext.util.Cookies.set('UserAuth', 'User1');


var urlweb = '../wa/api/customer';

function timerIncrement() {
    idleTime = idleTime + 1;
    if (idleTime > 19) { // 20 minutes
		Ext.MessageBox.wait('Detected Inactivity...','Warning');
        setTimeout(function() { Ext.MessageBox.hide() }, 5000)
        Ext.util.Cookies.clear('UserAuth');
        window.location.reload();
    }
}