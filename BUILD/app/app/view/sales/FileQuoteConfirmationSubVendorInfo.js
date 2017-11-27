Ext.define('CBH.view.sales.FileQuoteConfirmationSubVendorInfo', {
    extend: 'Ext.form.Panel',
    alias: 'widget.filequoteconfirmationsvinfo',
    columnWidth: 1,
    layout: {
        type: 'column'
    },
    //bodyPadding: 5,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,
    itemId: 'formsvinfo',
    currentRecord: null,

    fieldDefaults: {
        labelAlign: 'top',
        labelWidth: 60,
        msgTarget: 'side',
        fieldStyle: 'font-size:11px',
        labelStyle: 'font-size:11px'
    },

    defaults: {
        listeners: {
            blur: function(field) {

            }
        }
    },

    items: [{
        columnWidth: 0.5,
        xtype: 'numericfield',
        name: 'FVProfitMargin',
        fieldLabel: 'Mark Up Used',
        fieldStyle: 'text-align: right;',
        hideTrigger: true,
        useThousandSeparator: true,
        decimalPrecision: 2,
        alwaysDisplayDecimals: true,
        allowNegative: false,
        alwaysDecimals: true,
        thousandSeparator: ',',
        listeners: {
            blur: function(field, The, eOpts) {}
        }
    }, {
        xtype: 'container',
        columnWidth: 0.5,
        layout: 'hbox',
        items: [{
            xtype: 'component',
            flex: 1
        }, {
            flex: 1,
            margin: '15 0 0 8',
            xtype: 'checkbox',
            name: 'FVQuoteInfoConfirmed',
            labelSeparator: '',
            hideLabel: true,
            boxLabel: 'Vendor Confirmed',
            _ProgramaticChange: false,
            handler: function() {
                var me = this.up('form');
                if(!this._ProgramaticChange) {
                    me.onCheckVendorConfirmed(this);
                } else {
                    this._ProgramaticChange = false;
                }
            }
        }]
    }, {
        columnWidth: 1,
        xtype: 'combo',
        name: 'FVVendorContactKey',
        fieldLabel: '- Contact',
        valueField: 'ContactKey',
        displayField: 'x_ContactFullName',
        store: null,
        queryMode: 'local',
        typeAhead: false,
        minChars: 2,
        forceSelection: false,
        anyMatch: true
    }, {
        xtype: 'textfield',
        name: 'ContactPhone',
        fieldLabel: '     Phone',
        columnWidth: 1
    }, {
        xtype: 'textfield',
        name: 'VendorFax',
        fieldLabel: '     Fax',
        columnWidth: 1
    }, {
        xtype: 'textfield',
        name: 'ContactEmail',
        fieldLabel: '     Email',
        columnWidth: 1
    }, {
        xtype: 'combo',
        columnWidth: 1,
        name: 'FVPaymentTerms',
        fieldLabel: 'Payment Terms',
        valueField: 'TermKey',
        displayField: 'x_Description',
        store: null,
        queryMode: 'local',
        typeAhead: false,
        minChars: 2,
        forceSelection: true,
        anyMatch: true
    }, {
        xtype: 'numberfield',
        columnWidth: 0.5,
        name: 'FVTotalWeight',
        fieldLabel: 'Total Weight',
        fieldStyle: 'text-align: right;',
        hideTrigger: true,
    }, {
        margin: '0 0 0 5',
        xtype: 'numberfield',
        columnWidth: 0.5,
        name: 'FVTotalVolume',
        fieldLabel: 'Total Vol.',
        fieldStyle: 'text-align: right;',
        hideTrigger: true,
    }, {
        xtype: 'numericfield',
        columnWidth: 0.5,
        name: 'FVFreightCost',
        hideTrigger: true,
        useThousandSeparator: true,
        alwaysDisplayDecimals: true,
        allowNegative: false,
        currencySymbol: '$',
        alwaysDecimals: true,
        thousandSeparator: ',',
        fieldLabel: 'Freight Cost',
        labelAlign: 'top',
        fieldStyle: 'text-align: right;',
        allowBlank: true
    }, {
        margin: '0 0 0 5',
        xtype: 'numericfield',
        columnWidth: 0.5,
        name: 'FVFreightCost',
        hideTrigger: true,
        useThousandSeparator: true,
        alwaysDisplayDecimals: true,
        allowNegative: false,
        currencySymbol: '$',
        alwaysDecimals: true,
        thousandSeparator: ',',
        fieldLabel: 'Freight Cost',
        labelAlign: 'top',
        fieldStyle: 'text-align: right;',
        allowBlank: true
    }, {
        xtype: 'textfield',
        name: 'FVLeadTime',
        fieldLabel: 'Lead Time',
        columnWidth: 1
    }, {
        xtype: 'textareafield',
        fieldLabel: 'Instructions',
        columnWidth: 1,
        name: 'FVQuotePONotes',
    }, {
        margin: '15 0 0 0',
        xtype: 'gridpanel',
        itemId: 'gridsummaryvendorinfo',
        store: null,
        scrollable: true,
        columnWidth: 1,
        maxHeight: 150,
        minHeight: 50,
        columns: [{
            xtype: 'numbercolumn',
            flex: 1,
            dataIndex: 'Cost',
            text: 'Cost',
            //format: '0,000.00',
            align: 'right',
            renderer: function(value, metaData, record) {
                if (record.data.Currency ==="EUR") {
                    //metaData.style = "color:red;";
                    return Ext.util.Format.eurMoney(value);
                } else {
                    return Ext.util.Format.usMoney(value);
                }
            }
        }, {
            xtype: 'numbercolumn',
            flex: 1,
            dataIndex: 'DiscountAmount',
            text: 'Discount',
            //format: '0,000.00',
            align: 'right',
            renderer: function(value, metaData, record) {
                if (record.data.Currency ==="EUR") {
                    //metaData.style = "color:red;";
                    return Ext.util.Format.eurMoney(value);
                } else {
                    return Ext.util.Format.usMoney(value);
                }
            }
        }, {
            xtype: 'numbercolumn',
            flex: 1,
            dataIndex: 'CostAfterDiscount',
            text: 'Cost - Discount',
            //format: '0,000.00',
            align: 'right',
            renderer: function(value, metaData, record) {
                if (record.data.Currency ==="EUR") {
                    //metaData.style = "color:red;";
                    return Ext.util.Format.eurMoney(value);
                } else {
                    return Ext.util.Format.usMoney(value);
                }
            }
        }, {
            xtype: 'numbercolumn',
            flex: 1,
            dataIndex: 'Price',
            text: 'Price',
            //format: '0,000.00',
            align: 'right',
            renderer: function(value, metaData, record) {
                if (record.data.Currency ==="EUR") {
                    //metaData.style = "color:red;";
                    return Ext.util.Format.eurMoney(value);
                } else {
                    return Ext.util.Format.usMoney(value);
                }
            }
        }]
    }],

    onAfterLoadRecord: function(record) {
        var me = this,
            grid = me.down('gridpanel');

        me.currentRecord = record;
        me.loadRecord(record);

        var storeSumDiscount = new CBH.store.sales.qryFileQuoteVendorSummaryWithDiscount().load({
            params: {
                QHdrKey: record.data.FVQHdrKey,
                VendorKey: record.data.FVVendorKey
            },
            callback: function(records, operation, success) {
                grid.reconfigure(storeSumDiscount);

                var storeVendorContacts = new CBH.store.vendors.VendorContacts().load({
                    params: {
                        vendorkey: record.data.FVVendorKey
                    },
                    callback: function(records, operation, success) {
                        me.down('field[name=FVVendorContactKey]').bindStore(this);
                        me.down('field[name=FVVendorContactKey]').setValue(record.data.FVVendorContactKey);
                        me.getEl().unmask();
                    }
                });
            }
        });

    },

    onCheckVendorConfirmed: function(field) {
        var me = this,
            record = me.currentRecord,
            PaymentTerms = me.down('field[name=FVPaymentTerms]').getRawValue(),
            FVTotalWeight = me.down('field[name=FVTotalWeight]').getValue(),
            FVTotalVolume = me.down('field[name=FVTotalVolume]').getValue();

        // Verify payment terms
        if (String.isNullOrEmpty(PaymentTerms) && field.checked) {
            Ext.Msg.show({
                title: 'Warning',
                msg: 'You must specify the payment terms before you can confirm this vendor',
                buttons: Ext.Msg.OK,
                icon: Ext.Msg.WARNING,
                fn: function() {
                    field._ProgramaticChange = true;
                    field.setValue(false);
                    me.down('field[name=FVPaymentTerms]').focus(true, 200);
                }
            });
            return;
        }

        FVTotalWeight = (!FVTotalWeight) ? 0: parseFloat(FVTotalWeight);
        FVTotalVolume = (!FVTotalVolume) ? 0: parseFloat(FVTotalVolume);

        if (FVTotalWeight === 0 && FVTotalVolume === 0) {
            if (record.data.FVTotalWeightTag === 0 && record.data.FVTotalVolumeTag === 0) {
                Ext.Msg.show({
                    title: 'Ignore Weight/Volume?',
                    msg: 'There have been no weights or volumes entered into this file.  Continue anyway?',
                    buttons: Ext.Msg.YESNO,
                    icon: Ext.Msg.QUESTION,
                    fn: function(btn) {
                        if (btn === "no") {
                            field._ProgramaticChange = true;
                            field.setValue(false);
                            me.down('field[name=FVPaymentTerms]').focus(true, 200);
                        } else {
                            me.checkVendorConfirmed();
                        }
                    }
                });
            } else {
                Ext.Msg.show({
                    title: 'Calculate Weight/Volume?',
                    msg: 'There is no total weight or volume entered.  Do you want to copy the total from the calculated value of the individual items',
                    buttons: Ext.Msg.YESNO,
                    icon: Ext.Msg.QUESTION,
                    fn: function(btn) {
                        if (btn === "no") {
                            field._ProgramaticChange = true;
                            field.setValue(false);
                            me.down('field[name=FVPaymentTerms]').focus(true, 200);
                        } else {
                            me.checkVendorConfirmed();
                        }
                    }
                });
            }
            return;
        }

        me.checkVendorConfirmed();
    },

    checkVendorConfirmed: function() {
        var me = this,
            record = me.currentRecord;

        Ext.Msg.wait('Please wait....', 'Wait');

        Ext.Ajax.request({
            method: 'GET',
            type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/CheckConfirmedVendor',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                FileKey: record.data.FVFileKey,
                VendorKey: record.data.FVVendorKey,
                QHdrKey: record.data.FVQHdrKey
            },
            success: function(response) {
                var data = Ext.JSON.decode(response.responseText);
                Ext.Msg.hide();
            },
            failure: function(response, opts) {
                Ext.Msg.hide();
                console.info('server-side failure with status code ' + response.status);
            }

        });
    }
});