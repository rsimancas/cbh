Ext.define('CBH.view.sales.FileQuoteOrderEntryQuickAdd', {
    extend: 'Ext.form.Panel',
    alias: 'widget.FileQuoteOrderEntryQuickAdd',
    modal: true,
    width: 960,
    layout: 'fit',
    title: 'Quick Add to File...',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",
    VendorKey: 0,

    initComponent: function() {
        var me = this;

        var storeFiles = new CBH.store.sales.FileList();
        var storeQuotes = new CBH.store.sales.FileQuoteHeader();

        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [
                // Tabs
                {
                    xtype: 'tabpanel',
                    itemId: 'mainPanel',
                    columnWidth: 1,
                    bodyPadding: 10,
                    margin: '0 0 0 0',
                    activeTab: 0,
                    items: [
                        // Import Previous File
                        {
                            xtype: 'panel',
                            itemId: 'panelFile',
                            minHeight: 300,
                            title: 'Import from Previous File',
                            forceFit: true,
                            layout: {
                                type: 'vbox',
                                align: 'center',
                                pack: 'center'
                            },
                            padding: '5 10 5 10',
                            items: [
                                // Files
                                {
                                    xtype: 'fieldcontainer',
                                    layout: 'fit',
                                    items: [
                                        // Combo
                                        {
                                            xtype: 'combo',
                                            name: 'FileKey',
                                            fieldLabel: 'File',
                                            valueField: 'FileKey',
                                            displayField: 'FileNum',
                                            store: storeFiles,
                                            queryMode: 'remote',
                                            pageSize: 11,
                                            minChars: 2,
                                            allowBlank: true,
                                            triggerAction: '',
                                            forceSelection: false,
                                            queryCaching: false,
                                            emptyText: 'Choose File',
                                            autoSelect: false,
                                            selectOnFocus: true,
                                            matchFieldWidth: false,
                                            listConfig: {
                                                width: 400
                                            },
                                            tpl: Ext.create('Ext.XTemplate',
                                                '<tpl for=".">',
                                                '<div class="x-boundlist-item" >{FileNum} {Customer} {Reference}</div>',
                                                '</tpl>')
                                        }
                                    ]
                                }
                            ]
                        },
                        // Import Previous Quote
                        {
                            xtype: 'panel',
                            title: 'Import from Previous Quote',
                            itemId: 'panelQuote',
                            minHeight: 300,
                            forceFit: true,
                            layout: {
                                type: 'vbox',
                                align: 'center',
                                pack: 'center'
                            },
                            padding: '5 10 5 10',
                            items: [
                                // Quotes
                                {
                                    xtype: 'fieldcontainer',
                                    layout: 'fit',
                                    items:[
                                        // Combo
                                        {
                                            xtype: 'combo',
                                            name: 'QHdrKey',
                                            fieldLabel: 'Quote Number',
                                            valueField: 'QHdrKey',
                                            displayField: 'QuoteNum',
                                            store: storeQuotes,
                                            queryMode: 'remote',
                                            pageSize: 11,
                                            minChars: 2,
                                            allowBlank: true,
                                            triggerAction: '',
                                            forceSelection: false,
                                            queryCaching: false,
                                            emptyText: 'Choose File',
                                            autoSelect: false,
                                            selectOnFocus: true,
                                            matchFieldWidth: false,
                                            listConfig: {
                                                width: 400
                                            },
                                            tpl: Ext.create('Ext.XTemplate',
                                                '<tpl for=".">',
                                                '<div class="x-boundlist-item" >{QuoteNum} {QHdrDate:date("m/d/Y")} {x_CustName}</div>',
                                                '</tpl>')
                                        },
                                        {
                                            xtype:'fieldset',
                                            margin: '10 0 0 0',
                                            padding: '10 10 10 10',
                                            items:[
                                                // radio container
                                                {
                                                    xtype: 'radiogroup',
                                                    disabled: true,
                                                    //xtype      : 'fieldcontainer',
                                                    //fieldLabel : 'Size',
                                                    //defaultType: 'radiofield',
                                                    fixed: true,
                                                    defaults: {
                                                        flex: 1
                                                    },
                                                    layout: 'vbox',
                                                    items: [
                                                        {
                                                            boxLabel  : 'Create a new quote with the same information and charges from this quote',
                                                            name      : 'tipo',
                                                            inputValue: '0',
                                                            id        : 'tipo1'
                                                        }, {
                                                            boxLabel  : 'Import only the items from this quote, do not create a new quote',
                                                            name      : 'tipo',
                                                            inputValue: 'l',
                                                            id        : 'tipo2',
                                                            checked   : true
                                                        }
                                                    ]
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ],

            dockedItems: [{
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    itemId: 'acceptbutton',
                    text: 'Import',
                    //formBind: true,
                    listeners: {
                        click: {
                            fn: me.onSaveChanges,
                            scope: me
                        }
                    }
                }]
            }],
            // RadioFieldset
            listeners: {
                show: {
                    fn: me.onShowWindow,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onShowWindow: function() {
        var me = this;
        //me.down('#mainPanel').tabBar.setVisible(false);
    },

    onSaveChanges: function() {
        var me = this,
            tabs = me.down('#mainPanel'),
            activeTab = tabs.getActiveTab();

        if (activeTab.itemId === "panelFile") {

            var FileKey = me.down('field[name=FileKey]').getValue();

            if(!FileKey) {
                Ext.Msg.alert('Warning', 'You must select a file to import from!');
                return;
            }
 
            Ext.Msg.show({
                title: 'Confirme Import',
                msg: 'Are you sure you want to continue?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(btn) {
                    if (btn === "yes") {
                        me.ImportFile(FileKey);
                    }
                }
            });
        } else {
            var QHdrKey = me.down('field[name=QHdrKey]').getValue();

            if(!QHdrKey) {
                Ext.Msg.alert('Warning', 'You must select a quote number to import from!');
                return;
            }

            Ext.Msg.show({
                title: 'Confirme Import',
                msg: 'Are you sure you want to continue?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(btn) {
                    if (btn === "yes") {
                        me.ImportQuote(QHdrKey);
                    }
                }
            });
        }
    },

    ImportFile: function(FileKey) {
        var me = this,
            callerForm = me.callerForm;

        me.getEl().mask('Please wait....');

        Ext.Ajax.request({
            method: 'GET',
            //type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/ImportFile',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                FileKeySource: FileKey,
                FileKeyTarget: me.FileKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName
            },
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                me.getEl().unmask();
                Ext.Msg.alert('Success','Import Complete.  This file now has all records from the selected file.  Duplicates may have been made.  All costs, prices, and notes have been imported as well.');
                me.close();
                callerForm.refreshItemDetails();
            },
            failure: function(response, opts) {
                me.getEl().unmask();
                alert('server-side failure with status code ' + response.status);
            }
        });
    },

    ImportQuote: function(QHdrKey) {
        var me = this,
            callerForm = me.callerForm;

        me.getEl().mask('Please wait....');

        Ext.Ajax.request({
            method: 'GET',
            //type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/ImportQuote',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                QHdrKeySource: QHdrKey,
                FileKeyTarget: me.FileKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName
            },
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                me.getEl().unmask();
                Ext.Msg.alert('Success','Import Complete.  This file now has all records from the selected file.  Duplicates may have been made.  All costs, prices, and notes have been imported as well.');
                me.close();
                callerForm.refreshItemDetails();
            },
            failure: function(response, opts) {
                me.getEl().unmask();
                alert('server-side failure with status code ' + response.status);
            }
        });
    }
});