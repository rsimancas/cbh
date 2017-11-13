Ext.define('CBH.view.sales.FileQuoteVendorSelection', {
    extend: 'Ext.form.Panel',
    itemId:'FileQuoteVendorSelection',
    alias: 'widget.filequotevendorselection',
    width: 600,
    title: 'Vendor Selection',
    bodyPadding: 10,
    enableKeyEvents: true,
    callerForm: null,
    storeVendors: null,
    selectedQuote: null,
    storeFileQuoteVendorInfo: null,

    initComponent: function() {

        var me = this;

        Ext.applyIf(me, 
        {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items:[
            {
                xtype:'fieldcontainer',
                layout: {
                    type:'column'
                },
                items:[
                {
                    xtype: 'displayfield',
                    value: 'Highlight which vendors you want to include in this quote',
                    columnWidth: 1,
                    margin:'0 0 10 0',
                },
                {
                    columnWidth: 0.5,
                    margin: '0 0 0 8',
                    xtype: 'checkbox',
                    itemId: 'CheckAllchk',
                    labelSeparator: '',
                    hideLabel: true,
                    boxLabel: 'Select All',
                    handler: me.onCheckedAll
                },
                {
                    xtype:'component',
                    columnWidth: 0.5
                },
                {
                    margin:'0 0 5 0',
                    xtype: 'gridpanel',
                    itemId: 'gridvendors',
                    //title: 'Vendors',
                    // selModel: {
                    //     mode: 'MULTI',
                    //     toggleOnClick: false
                    // },
                    columnWidth: 1,
                    minHeight: 150,
                    //margin: '0 5 5 0',
                    store: me.storeVendors,
                    columns: [
                    {
                        xtype: 'checkcolumn',
                        dataIndex: 'x_Selected',
                        text: 'Sel.',
                        width: 35,
                        listeners: {
                            'checkchange' : {
                                fn: me.onCheckChange,
                                scope: me
                            }
                        }

                    },
                    {
                        xtype: 'gridcolumn',
                        flex:1,
                        dataIndex: 'Vendor',
                        text: 'Vendor'  
                    },
                    {
                        xtype: 'gridcolumn',
                        flex:1,
                        dataIndex: 'Qty',
                        text: 'Qty',
                        align: 'right',
                        format: '00,000.00',
                    },
                    {
                        xtype: 'gridcolumn',
                        flex:1,
                        dataIndex: 'Cost',
                        text: 'Cost',
                        align: 'right',
                        renderer: Ext.util.Format.usMoney  
                    },
                    {
                        xtype: 'gridcolumn',
                        flex:1,
                        dataIndex: 'Price',
                        text: 'Price',
                        align: 'right',
                        renderer: Ext.util.Format.usMoney
                    }
                    ]
                },
                {
                    margin: '10 0 5 0',
                    xtype: 'displayfield',
                    value: 'Note: If the vendor is selected on another quote, it will be removed from that quote and added to this one.',
                    columnWidth: 1
                }
                ]
            }
            ],
            dockedItems: [
            {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [
                {
                    xtype: 'component',
                    flex: 1
                },
                {
                    xtype: 'button',
                    itemId: 'acceptbtn',
                    text: 'Save',
                    disabled: true,
                    listeners: {
                        click: {
                            fn: me.onOkClick,
                            scope: me
                        }
                    }
                },
                {
                    xtype: 'button',
                    itemId: 'cancelbtn',
                    text: 'Cancel',
                    listeners: {
                        click: {
                            fn: me.onCancelClick,
                            scope: me
                        }
                    }
                }
                ]
            }
            ],
            listeners:{
                show: {
                    fn: me.onShowWindow,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onShowWindow: function() {
        //Ext.getCmp('profitmargenvalue').focus(true, 200);
        var me = this;

        Ext.each(me.storeFileQuoteVendorInfo.data.items, function(item, index, allItems) {
            r = item.data;
            if(r.FVQHdrKey === me.selectedQuote.QHdrKey) {
                var i = me.storeVendors.find('VendorKey', r.FVVendorKey);
                if(i > -1) {
                    me.storeVendors.getAt(i).set('x_Selected', true);
                }
            }
        });
    },

    onOkClick: function() {
        var me = this,
            callerForm = Ext.getCmp(me.callerForm),
            store = me.down('#gridvendors').store,
            record = null;

        Ext.each(store.data.items, function(item, index, allItems) {
            var selected = item.data,
                fvindex = -1;

            if(selected.x_Selected) {
                fvindex = me.storeFileQuoteVendorInfo.find('FVVendorKey', selected.VendorKey);
                if(fvindex !== -1) {
                    record = me.storeFileQuoteVendorInfo.getAt(fvindex);
                    record.set('FVQHdrKey', me.selectedQuote.QHdrKey);
                } else {
                    r = Ext.create('CBH.model.sales.FileQuoteVendorInfo', {
                        FVFileKey: me.callerForm.FileKey,
                        FVQHdrKey: me.selectedQuote.QHdrKey,
                        FVVendorKey: selected.VendorKey
                    });
                    me.storeFileQuoteVendorInfo.add(r);
                }
            } else {
                fvindex = me.storeFileQuoteVendorInfo.find('FVVendorKey', selected.VendorKey);
                if(fvindex !== -1) {
                    record = me.storeFileQuoteVendorInfo.getAt(fvindex);
                    if(record.data.FVQHdrKey === me.selectedQuote.QHdrKey)
                        record.set('FVQHdrKey', null);
                }
            }
        });

        var recordsForSave = me.storeFileQuoteVendorInfo.getCount(),
            recordsSaved = [];

        if(recordsForSave) Ext.Msg.wait('Saving Changes...', 'Wait');

        Ext.each(me.storeFileQuoteVendorInfo.data.items, function(item, index, allItems) {
            item.save({
                callback: function(records, operations, success) {
                    recordsSaved.push(item);
                    if(recordsSaved.length === recordsForSave) {
                        var storeQuoteSummary = me.callerForm.modelFileOverView.Quotes().load({
                            //scope: me.callerForm,
                            callback: function() {
                                var grid = me.callerForm.down('#gridquotes');

                                if(grid.getSelectionModel().selected.length === 0) {
                                    grid.getSelectionModel().select(0);
                                }

                                if(grid.store.getCount() === 0) {
                                    me.setDisabledQuoteButtons(true);
                                }

                                me.destroy();
                                Ext.Msg.hide();
                            }
                        });
                    }
                }
            });
        });
    },

    onCancelClick: function() {
        var me = this;
        me.close();
    },

    onCheckedAll: function(checkbox, checked) {
        var me = this.up('panel'),
            store = me.down('#gridvendors').store;

        Ext.each(store.data.items, function(item, index, allItems) {
            item.set('x_Selected', checked);
        });

        me.onCheckChange();
    },

    onCheckChange: function() {
        var me = this,
            store = me.down('#gridvendors').store;

        var isSelectedOne = false;

        Ext.each(store.data.items, function(item, index, allItems) {
            if(item.get('x_Selected')) {
                isSelectedOne = true;
                return false;
            } 
        });

        // if not vendor selected then set disabled ok button
        me.down('#acceptbtn').setDisabled(!isSelectedOne);  
    }
});