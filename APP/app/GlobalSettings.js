Ext.define('CBH.GlobalSettings', {
    singleton: true,
    //webApiPath: 'http://localhost:4831',
    CipherPass: "CBH@Intl..",
    webApiPath: '../wa',
    currentUser: Ext.JSON.decode(Ext.util.Cookies.get("CBH.CurrentUser")),
    getCurrentUser: function() {
        if (this.currentUser) {
            if(this.currentUser.EmployeeAccessLevel === null)
                this.currentUser.EmployeeAccessLevel = 3;
            return this.currentUser;
        } else {
            return null;
        }
    },
    getCurrentUserName: function() {
        if (this.currentUser) {
            //return this.currentUser.UserName;
            return this.currentUser.UserFullName;
        } else {
            return null;
        }
    },
    getCurrentUserEmployeeKey: function() {
        if (this.currentUser) {
            return this.currentUser.EmployeeKey;
        } else {
            return null;
        }
    },
    getUserRemember: function() {
        var encrypted = Ext.util.Cookies.get("CBH.Log"),
            str = "",
            remember = {};

        if(encrypted) {
            decrypted = CryptoJS.AES.decrypt(encrypted, this.CipherPass);
            str = decrypted.toString(CryptoJS.enc.Utf8);
            remember = Ext.JSON.decode(str);
        }

        if(!remember.UserName) remember.UserName = "";
        if(!remember.UserPassword) remember.UserPassword = "";

        return remember;
    },

    unloadListener: function(e) { // For >=IE7, Chrome, Firefox
        var confirmationMessage = 'Are you sure to leave the page?';  // a space
        (e || window.event).returnValue = confirmationMessage;
        return confirmationMessage;
    },

    closeSession: function() {
        var myEvent = window.removeEventListener;
        var chkevent = 'beforeunload'; /// make IE7, IE8 compitable
        myEvent(chkevent, this.unloadListener, false);

        Ext.util.Cookies.clear("CBH.UserAuth");
        Ext.util.Cookies.clear("CBH.CurrentUser");
        Ext.MessageBox.wait('Closing','Closing Session!!!');
        var url = location.href;
        url = url.split('#');
        location.href = url[0];
    }
});