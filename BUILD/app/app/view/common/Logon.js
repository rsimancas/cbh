Ext.define('CBH.view.common.Logon', {
    extend: 'Ext.container.Container',
    alias: 'widget.logon',
    //renderTo: Ext.GetBody(),
    autoRender: true,
    autoShow: true,
    //width: 300,  
    //id: 'logon',

    initComponent: function() {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'container',
                    frame: false,
                    html: '<div><p align="center"><img src="images/logo_cbh_logon.png" style="width:240px;"/></p></div>'
                },
                {
                    xtype: 'form',
                    autoRender: true,
                    autoShow: true,
                    frame: true,
                    layout: { type: 'fit'},
                    style: {
                        marginLeft: 'auto',
                        marginRight: 'auto',
                        marginTop: 10
                    },
                    width: 300,
                    //autoScroll: true,
                    bodyPadding: 0,
                    //manageHeight: false,
                    title: 'Logon',
                    jsonSubmit: true,
                    items: [
                        {
                            xtype: 'fieldcontainer',
                            height: 120,
                            margin: '0 5 10 5',
                            layout: {
                                type: 'anchor'
                            },
                            fieldDefaults: {
                                labelAlign: 'top',
                                labelWidth: 90,
                                msgTarget: 'qtip'
                            },
                            labelAlign: 'top',
                            items: [
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    margin: '0 5 0 5',
                                    fieldLabel: 'User Name:',
                                    msgTarget: 'side',
                                    name: 'UserName',
                                    allowBlank: false,
                                    emptyText: 'UserName',
                                    value: CBH.GlobalSettings.getUserRemember().UserName
                                },
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    margin: '0 5 0 5',
                                    fieldLabel: 'Password',
                                    emptyText: 'Password',
                                    msgTarget: 'side',
                                    name: 'UserPassword',
                                    inputType: 'password',
                                    allowBlank: false,
                                    enableKeyEvents: true,
                                    value: CBH.GlobalSettings.getUserRemember().UserPassword
                                },
                                {
                                    margin: '10 10 10 10',
                                    xtype: 'checkbox',
                                    name: 'RememberMe',
                                    labelSeparator: '',
                                    hideLabel: true,
                                    boxLabel: 'Remember me',
                                    checked: !String.isNullOrEmpty(CBH.GlobalSettings.getUserRemember().UserName)
                                }
                            ]
                        }
                    ],
                    dockedItems: [
                        {
                            xtype: 'toolbar',
                            dock: 'bottom',
                            stateful: true,
                            autoRender: true,
                            autoShow: true,
                            margin: '',
                            ui: 'footer',
                            layout: {
                                align: 'stretch',
                                pack: 'end',
                                type: 'hbox'
                            },
                            items: [
                                {
                                    xtype: 'button',
                                    formBind: false,
                                    width: 100,
                                    text: 'Submit'
                                }
                            ]
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }

});