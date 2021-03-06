Ext.define('CBH.view.jobs.JobNewInvoice', {
    extend: 'Ext.form.Panel',
    alias: 'widget.jobnewinvoice',
    modal: true,
    width: 800,
    layout: {
        type: 'absolute'
    },
    title: 'Create New Invoice...',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",
    VendorKey: 0,

    initComponent: function() {
        var me = this;

        var selModel = Ext.create('Ext.selection.CheckboxModel', {
            checkOnly: true,
            listeners: {
                select: function(model, record) {
                    record.set('x_Selected', true);
                },
                deselect: function(model, record) {
                    record.set('x_Selected', false);
                }
            }
        });

        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                page: 1,
                start: 0,
                limit: 8
            }
        });
        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeVendorPOInformation = new CBH.store.jobs.qLstJobPurchaseOrders().load({
            params: {
                JobKey: me.JobKey
            },
            callback: function() {
                var grid = this.down('#gridvendors');

                if (grid.getSelectionModel().selected.length === 0) {
                    grid.getSelectionModel().select(0);
                }
            },
            scope: me
        });

        Ext.applyIf(me, {
            fieldDefaults: {
                //labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [{
                xtype: 'fieldcontainer',
                padding: 5,
                layout: {
                    type: 'fit'
                },
                items: [{
                    xtype: 'fieldset',
                    fieldDefaults: {
                        labelWidth: 50,
                        labelAlign: 'top'
                    },
                    padding: '0 10 10 10',
                    layout: 'column',
                    columnWidth: 1,
                    collapsible: false,
                    title: 'This Invoice is',
                    items: [{
                        xtype: 'radiogroup',
                        padding: '0 0 0 20',
                        width: 800,
                        fixed: true,
                        defaults: {
                            flex: 1
                        },
                        layout: 'hbox',
                        items: [{
                            boxLabel: 'Standard invoice to customer (using purchase orders)',
                            name: 'typeinvoice',
                            inputValue: 'standard',
                            checked: true,
                            itemId: 'radio1'
                        }, {
                            boxLabel: 'Commission Only (no purchase orders or items)',
                            name: 'typeinvoice',
                            inputValue: 'commission',
                            itemId: 'radio2'
                        }],
                        listeners: {
                            change: function(that, newValue, oldValue, eOpts) {
                                var me = this.up('form'),
                                    panelpo = me.down('#panelpo'),
                                    panelComission = me.down('#panelComission'),
                                    acceptbutton = me.down('#acceptbutton'),
                                    mainPanel = me.down('#mainPanel');

                                if (newValue.typeinvoice === "commission") {
                                    panelpo.setDisabled(true);
                                    panelComission.setDisabled(false);
                                    mainPanel.setActiveTab(1);
                                    //     acceptbutton.formBind = true;  
                                    //     me.getForm().reset();   
                                } else {
                                    panelpo.setDisabled(false);
                                    panelComission.setDisabled(true);
                                    mainPanel.setActiveTab(0);
                                }
                            }
                        }
                    }]
                }, {
                    xtype: 'tabpanel',
                    itemId: 'mainPanel',
                    columnWidth: 1,
                    bodyPadding: 10,
                    margin: '0 0 0 0',
                    activeTab: 0,
                    items: [
                        // Purchase Order Panel
                        {
                            xtype: 'panel',
                            itemId: 'panelpo',
                            minHeight: 300,
                            disabled: false,
                            header: false,
                            layout: {
                                type: 'column'
                            },
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridvendors',
                                //title: 'Select which Purchase Orders to include on this invoice',
                                columnWidth: 1,
                                height: 200,
                                margin: '0 5 5 0',
                                store: storeVendorPOInformation,
                                columns: [{
                                    xtype: 'rownumberer',
                                    width: 30
                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'Date',
                                    text: 'Date',
                                    renderer: Ext.util.Format.dateRenderer('m/d/Y')
                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'Vendor',
                                    text: 'Vendor'
                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'PONum',
                                    text: 'PO Num'
                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'Status',
                                    text: 'Status'
                                }, {
                                    xtype: 'numbercolumn',
                                    width: 80,
                                    dataIndex: 'Cost',
                                    text: 'Cost',
                                    align: 'right',
                                    format: '00,000.00'
                                }, {
                                    xtype: 'gridcolumn',
                                    width: 60,
                                    dataIndex: 'CurrencyCode',
                                    text: 'CUR'
                                }],
                                selModel: selModel
                            }],
                            tbar: [
                                '<h3>Select which Purchase Orders to include on this invoice</h3>'
                            ]
                        },
                        // Commission Panel
                        {
                            xtype: 'panel',
                            header: false,
                            itemId: 'panelComission',
                            layout: 'fit',
                            padding: '5 10 5 10',
                            disabled: true,
                            items: [{
                                xtype: 'fieldset',
                                title: 'Commission',
                                flex: 1,
                                layout: 'column',
                                fieldDefaults: {
                                    labelWidth: 50,
                                    labelAlign: 'top'
                                },
                                items: [{
                                    xtype: 'combo',
                                    name: 'VendorKey',
                                    fieldLabel: 'Vendor',
                                    columnWidth: 0.5,
                                    valueField: 'VendorKey',
                                    displayField: 'VendorName',
                                    store: storeVendors,
                                    queryMode: 'remote',
                                    pageSize: 11,
                                    minChars: 2,
                                    allowBlank: false,
                                    triggerAction: '',
                                    forceSelection: false,
                                    queryCaching: false,
                                    emptyText: 'Choose Vendor',
                                    autoSelect: false,
                                    selectOnFocus: true,
                                    listeners: {
                                        buffer: 100,
                                        change: function(field, newValue, oldValue) {
                                            if (field.readOnly) return;
                                            var me = field.up('panel');
                                            field.next().clearValue();
                                        },
                                        blur: {
                                            fn: me.onVendorBlur,
                                            scope: me
                                        }
                                    }
                                }, {
                                    xtype: 'combo',
                                    name: 'VendorContactKey',
                                    fieldLabel: 'Contact',
                                    margin: '0 0 0 5',
                                    columnWidth: 0.5,
                                    queryMode: 'local',
                                    store: null,
                                    valueField: 'ContactKey',
                                    displayField: 'x_ContactFullName',
                                    triggerAction: '',
                                    anyMatch: true
                                }, {
                                    xtype: 'numericfield',
                                    name: 'TotalSale',
                                    fieldLabel: 'Total Sale',
                                    columnWidth: 0.5,
                                    hideTrigger: true,
                                    useThousandSeparator: true,
                                    decimalPrecision: 2,
                                    alwaysDisplayDecimals: true,
                                    allowNegative: false,
                                    alwaysDecimals: true,
                                    thousandSeparator: ',',
                                    fieldStyle: 'text-align: right;',
                                    allowBlank: false,
                                    listeners: {
                                        change: function(field, newValue, oldValue, eOpts) {
                                            if (document.activeElement.name !== field.name) return;

                                            var form = field.up('form'),
                                                pct = form.down('field[name=CommissionPCT]').getValue();

                                            form.down('field[name=CommissionTotal]').setValue(newValue * pct);
                                        }
                                    }
                                }, {
                                    margin: '0 0 0 5',
                                    xtype: 'combo',
                                    columnWidth: 0.5,
                                    name: 'TotalSaleCurrencyCode',
                                    width: 50,
                                    fieldLabel: 'Currency Total Sale',
                                    store: storeCurrencyRates,
                                    // matchFieldWidth: false,
                                    // listConfig: {
                                    //   width: 'auto'
                                    // },                                                                             
                                    valueField: 'CurrencyCode',
                                    displayField: 'CurrencyCode',
                                    queryMode: 'local',
                                    typeAhead: true,
                                    minChars: 2,
                                    allowBlank: false,
                                    forceSelection: true,
                                    triggerAction: '',
                                    anyMatch: true,
                                    listeners: {
                                        blur: function(field, The, eOpts) {
                                            if (field.value !== null) {
                                                //me.onSaveClick();
                                            }
                                        }
                                    },
                                    tpl: Ext.create('Ext.XTemplate',
                                        '<tpl for=".">',
                                        '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                                        '</tpl>')
                                }, {
                                    xtype: 'numberfield',
                                    name: 'TotalSaleCurrencyRate',
                                    fieldLabel: 'Total Sale',
                                    columnWidth: 0.5,
                                    minValue: 0,
                                    value: 0.00,
                                    allowDecimals: true,
                                    decimalPrecision: 2,
                                    hidden: true
                                }, {
                                    xtype: 'container',
                                    columnWidth: 1,
                                    layout: 'hbox',
                                    items: [{
                                        xtype: 'numberfield',
                                        name: 'CommissionPCT',
                                        fieldLabel: 'Commission %',
                                        value: 0,
                                        maxValue: 100,
                                        minValue: 0,
                                        allowDecimals: true,
                                        decimalPrecision: 2,
                                        fieldStyle: 'text-align: right;',
                                        width: '50%'
                                    }, {
                                        xtype: 'component',
                                        flex: 1
                                    }]
                                }, {
                                    xtype: 'numericfield',
                                    name: 'CommissionTotal',
                                    fieldLabel: 'Commission Total',
                                    columnWidth: 0.5,
                                    hideTrigger: true,
                                    useThousandSeparator: true,
                                    decimalPrecision: 2,
                                    alwaysDisplayDecimals: true,
                                    allowNegative: false,
                                    alwaysDecimals: true,
                                    thousandSeparator: ',',
                                    fieldStyle: 'text-align: right;',
                                    allowBlank: false
                                }, {
                                    margin: '0 0 25 5',
                                    xtype: 'combo',
                                    name: 'CommissionTotalCurrencyCode',
                                    columnWidth: 0.5,
                                    fieldLabel: 'Currency Commission',
                                    store: storeCurrencyRates,
                                    //matchFieldWidth: false,
                                    // listConfig: {
                                    //   width: 'auto'
                                    // },                                                                             
                                    valueField: 'CurrencyCode',
                                    displayField: 'CurrencyCode',
                                    queryMode: 'local',
                                    typeAhead: true,
                                    minChars: 2,
                                    allowBlank: false,
                                    forceSelection: true,
                                    triggerAction: '',
                                    anyMatch: true,
                                    listeners: {
                                        blur: function(field, The, eOpts) {
                                            if (field.value !== null) {
                                                //me.onSaveClick();
                                            }
                                        }
                                    },
                                    tpl: Ext.create('Ext.XTemplate',
                                        '<tpl for=".">',
                                        '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                                        '</tpl>')
                                }, {
                                    xtype: 'numberfield',
                                    name: 'CommissionTotalCurrencyRate',
                                    fieldLabel: 'Total Sale',
                                    columnWidth: 0.5,
                                    minValue: 0,
                                    value: 0.00,
                                    allowDecimals: true,
                                    decimalPrecision: 2,
                                    hidden: true
                                }]
                            }]
                        }
                    ]
                }, ]
            }],

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
                    text: 'Next > > >',
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
        me.down('#mainPanel').tabBar.setVisible(false);
    },

    onSaveChanges: function() {
        var me = this,
            invoiceType = me.down('radiogroup').getValue().typeinvoice;

        if (invoiceType === "standard") {
            var grid = me.down('gridpanel'),
                sm = grid.getSelectionModel(),
                selected = sm.getSelection();

            if (selected.length === 0) {
                Ext.Msg.show({
                    title: 'Continue?',
                    msg: 'Are you sure you want to continue without any Purchase Orders selected?',
                    buttons: Ext.Msg.YESNO,
                    icon: Ext.Msg.QUESTION,
                    fn: function(btn) {
                        if (btn === "yes") {
                            me.CreateNewJobInvoice();
                        }
                    }
                });
            } else {
                me.CreateNewJobInvoice();
            }
        } else {
            var vendorKey = me.down("field[name=VendorKey]").getValue();

            if (vendorKey === null || vendorKey === 0) {
                Ext.Msg.alert('Vendor Not Specified', 'You must select a vendor to receive the commission from!');
                me.down("field[name=VendorKey]").focus(true, 200);
                return;
            }

            var msgHtml = '<h3>Review</h3><div><ul>',
                showMsg = false,
                totalSale = me.down('field[name=TotalSale]').getValue(),
                totalCommission = me.down('field[name=CommissionTotal]').getValue();

            if (totalSale === null || totalSale <= 0) {
                showMsg = true;
                msgHtml = msgHtml + '<li><h4 style="color:red;">You have not specified a valid sale price (for volume tracking)</h4></li>';
            }

            if (totalCommission === null || totalCommission <= 0) {
                showMsg = true;
                msgHtml = msgHtml + '<li><h4 style="color:red;">You have no specified a valid commission amount</h4></li>';
            }

            msgHtml = msgHtml + '</ul></div>';

            if (showMsg) {
                var panelAccept = new CBH.view.jobs.JobNewInvoiceContinue({
                    callerForm: me,
                    msgHtml: msgHtml
                });

                panelAccept.show();
                return;
            }

            me.CreateNewCommissionInvoice();
        }
    },

    onVendorBlur: function(field, The, eOpts) {
        if (field.readOnly) return;

        var me = field.up('panel'),
            rawvalue = field.getRawValue();

        if (field && field.valueModels !== null) {
            vendormodel = new CBH.model.vendors.Vendors({
                VendorKey: field.value
            });
            storeLastQuoteMargin = vendormodel.LastQuoteMargin().load({
                params: {
                    page: 0,
                    start: 0,
                    limit: 0
                },
                callback: function() {
                    me.down('field[name=CommissionPCT]').setValue(storeLastQuoteMargin.getAt(0).data.FVProfitMargin);

                    var storeContacts = new CBH.store.vendors.VendorContacts().load({
                        params: {
                            page: 1,
                            start: 0,
                            limit: 8,
                            vendorkey: field.value
                        },
                        callback: function() {
                            field.next().bindStore(storeContacts);
                        }
                    });
                }
            });
        }
    },

    CreateNewJobInvoice: function() {
        var me = this,
            callerForm = me.callerForm,
            records = me.down('gridpanel').getSelectionModel().getSelection(),
            selectedId = [];

        Ext.Array.each(records, function(record, index) {
            selectedId.push(record.data.POKey);
        });

        Ext.Msg.wait('Please wait....', 'Wait');

        Ext.Ajax.request({
            method: 'POST',
            type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/CreateNewJobInvoice',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                JobKey: me.JobKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName
            },
            jsonData: selectedId,
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                Ext.Msg.hide();
                me.close();
                callerForm.down('#gridInvoices').store.reload();
            }
        });
    },

    CreateNewCommissionInvoice: function() {
        var me = this,
            callerForm = me.callerForm,
            form = me.getForm(),
            record = me.commissionModel;

        form.updateRecord(record);

        Ext.Msg.wait('Please wait....', 'Wait');

        Ext.Ajax.request({
            method: 'POST',
            type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/CreateNewCommissionInvoice',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                JobKey: me.JobKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName
            },
            jsonData: record.data,
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                Ext.Msg.hide();
                me.close();
                callerForm.down('#gridInvoices').store.reload();
            }
        });
    }
});
