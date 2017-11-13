Ext.define('CBH.view.common.ToolBar', {
	extend: 'Ext.toolbar.Toolbar',
	xtype: 'app_toolbar',

	initComponent: function() {

        var me = this;

		var auth = Ext.util.Cookies.get("CBH.CurrentUser");
        
		currentUser = Ext.JSON.decode(auth);
        tipo = typeof currentUser.UserFullName;
        
        fullName = ("string" == tipo)  ? currentUser.UserFullName : '';

		Ext.apply(me, {
            items: [
            {
            	xtype:'splitbutton',
                //glyph: 'xf007@FontAwesome',
                /*iconCls: 'app-user',*/
                iconCls: 'fa fa-user',
            	text: fullName,
            	menu:[{
                    glyph: 'xf08b@FontAwesome',
                    //iconCls: 'app-logout',
                    text: 'Logout',
        			handler: function() {
                        Ext.Msg.show({
                            title: 'Question',
                            msg: 'Do you want to close session?',
                            buttons: Ext.Msg.YESNO,
                            icon: Ext.Msg.QUESTION,
                            fn: function(btn) {
                                if (btn === "yes") {
                                    CBH.GlobalSettings.closeSession();
                                }
                            }
                        }).defaultButton = 2;
            		}
                }]
            }]
        });

		this.callParent(arguments);
	}
});