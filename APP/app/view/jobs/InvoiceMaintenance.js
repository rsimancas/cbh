Ext.define('CBH.view.jobs.InvoiceMaintenance', {
    extend: 'Ext.form.Panel',
    alias: 'widget.invoicemaintenance',
    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    title: 'Invoice Maintenance',
    JobKey: 0,
    storeNavigator: null,
    storesLoaded: null,
    storeToLoad: 0,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        var rowEditingItems = me.loadPluginItems();

        //var rowEditingCharges = me.loadPluginCharges();

        var rowEditingChargesST = me.loadPluginChargesST();

        Ext.Msg.wait('Loading data...', 'Wait');
        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeCurRatesCharges = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                Ext.Msg.hide();
            }
        });

        var storeChargesST = null,
            storetlkpCategories = null,
            storetlkpGL = null,
            storeCharges = null,
            storeVendors = null,
            storeCustomers = null,
            storeVendorContacts = null,
            storeCustomerShipAddress = null,
            storeEmployeeRoles = null,
            storeInvoiceItems = null;

        var storeShipmentTypes = new CBH.store.common.ShipmentTypes().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storePaymentTerms = new CBH.store.common.PaymentTerms().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeCountries = new CBH.store.common.Countries().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeStates = new CBH.store.common.States().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeChargesKeys = new CBH.store.sales.ChargeCategories().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });


        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [
                // Cust Key and Reference Num
                {
                    xtype: 'fieldset',
                    columnWidth: 1,
                    defaults: {
                        anchor: '100%',

                    },
                    collapsible: false,
                    items: [{
                        xtype: 'fieldcontainer',
                        layout: {
                            type: 'hbox'
                        },
                        items: [{
                            xtype: 'component',
                            flex: 1
                        }, {
                            xtype: 'displayfield',
                            name: 'x_InvoiceCustKey',
                            hideTrigger: true,
                            labelAlign: 'left',
                            labelWidth: 80,
                            fieldLabel: 'Customer Code',
                            editable: false,
                            fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                        }, {
                            margin: '0 0 0 10',
                            xtype: 'displayfield',
                            name: 'FullInvoiceNum',
                            fieldLabel: 'Reference Num',
                            labelAlign: 'left',
                            labelWidth: 80,
                            editable: false,
                            fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                        }]
                    }]
                }, {
                    xtype: 'tabpanel',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    activeTab: 0,
                    items: [
                        //General Information
                        {
                            xtype: 'panel',
                            title: 'General Information',
                            layout: 'column',
                            minHeight: 350,
                            items: [
                                //Purchase Order Information
                                {
                                    margin: '10 0 0 0',
                                    padding: '0 10 10 10',
                                    columnWidth: 0.5,
                                    xtype: 'fieldset',
                                    title: 'Invoice Info',
                                    layout: {
                                        type: 'column'
                                    },
                                    items: [{
                                        xtype:'hiddenfield',
                                        name:'InvoiceKey'
                                    },
                                    {
                                        xtype: 'textfield',
                                        columnWidth: 0.5,
                                        name: 'InvoiceNum',
                                        fieldLabel: 'Invoice Num'

                                    }, {
                                        margin: '0 0 0 5',
                                        columnWidth: 0.5,
                                        xtype: 'combo',
                                        displayField: 'name',
                                        valueField: 'id',
                                        fieldLabel: 'Revision Num',
                                        name: 'InvoiceRevisionNum',
                                        queryMode: 'local',
                                        typeAhead: true,
                                        minChars: 1,
                                        forceSelection: true,
                                        /*emptyText: 'choose',*/
                                        enableKeyEvents: true,
                                        autoSelect: true,
                                        selectOnFocus: true,
                                        defaultValue: 0,
                                        allowBlank: false,
                                        store: {
                                            fields: ['id', 'name'],
                                            data: [{
                                                'id': 0,
                                                'name': '0'
                                            }, {
                                                'id': 1,
                                                'name': 'A'
                                            }, {
                                                'id': 2,
                                                'name': 'B'
                                            }, {
                                                'id': 3,
                                                'name': 'C'
                                            }, {
                                                'id': 4,
                                                'name': 'D'
                                            }, {
                                                'id': 5,
                                                'name': 'E'
                                            }, {
                                                'id': 6,
                                                'name': 'F'
                                            }, {
                                                'id': 7,
                                                'name': 'G'
                                            }, {
                                                'id': 8,
                                                'name': 'H'
                                            }, {
                                                'id': 9,
                                                'name': 'I'
                                            }, {
                                                'id': 10,
                                                'name': 'J'
                                            }, {
                                                'id': 11,
                                                'name': 'K'
                                            }, {
                                                'id': 12,
                                                'name': 'L'
                                            }, {
                                                'id': 13,
                                                'name': 'M'
                                            }, {
                                                'id': 14,
                                                'name': 'N'
                                            }, {
                                                'id': 15,
                                                'name': 'O'
                                            }, {
                                                'id': 16,
                                                'name': 'P'
                                            }, {
                                                'id': 17,
                                                'name': 'Q'
                                            }, {
                                                'id': 18,
                                                'name': 'R'
                                            }, {
                                                'id': 19,
                                                'name': 'S'
                                            }, {
                                                'id': 20,
                                                'name': 'T'
                                            }, {
                                                'id': 21,
                                                'name': 'U'
                                            }, {
                                                'id': 22,
                                                'name': 'V'
                                            }, {
                                                'id': 23,
                                                'name': 'W'
                                            }, {
                                                'id': 24,
                                                'name': 'X'
                                            }, {
                                                'id': 25,
                                                'name': 'Y'
                                            }, {
                                                'id': 26,
                                                'name': 'Z'
                                            }]
                                        }
                                    }, {
                                        xtype: 'datefield',
                                        name: 'InvoiceDate',
                                        fieldLabel: 'Invoice Date',
                                        columnWidth: 0.5
                                    }, {
                                        margin: '0 0 0 5',
                                        xtype: 'datefield',
                                        name: 'JobShipDate',
                                        fieldLabel: 'Ship Date',
                                        columnWidth: 0.5
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        name: 'JobShipType',
                                        fieldLabel: 'Shipment Type',
                                        listConfig: {
                                            minWidth: null
                                        },
                                        valueField: 'ShipTypeExpression',
                                        displayField: 'ShipTypeText',
                                        store: storeShipmentTypes,
                                        queryMode: 'local',
                                        typeAhead: false,
                                        minChars: 2,
                                        //allowBlank: false,
                                        forceSelection: true,
                                        anyMatch: true
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        name: 'InvoiceEmployeeKey',
                                        fieldLabel: 'Sales Rep',
                                        displayField: 'x_EmployeeFullName',
                                        valueField: 'EmployeeKey',
                                        enableKeyEvents: true,
                                        forceSelection: true,
                                        queryMode: 'local',
                                        selectOnFocus: true,
                                        allowBlank: false,
                                        anyMatch: true,
                                        store: new CBH.store.common.Employees().load({
                                            params: {
                                                fieldFilters: JSON.stringify({fields: [{name:'EmployeeStatusCode',value:8,type:'int'}]}),
                                                page: 0,
                                                start: 0,
                                                limit: 0
                                            }
                                        })
                                    }, {
                                        xtype: 'combo',
                                        name: 'InvoiceCurrencyCode',
                                        fieldLabel: 'Currency Code',
                                        store: storeCurrencyRates,
                                        listConfig: {
                                            minWidth: null
                                        },
                                        columnWidth: 0.5,
                                        valueField: 'CurrencyCode',
                                        displayField: 'CurrencyCodeDesc',
                                        queryMode: 'local',
                                        autoSelect: false,
                                        minChars: 2,
                                        allowBlank: false,
                                        forceSelection: true
                                    }, {
                                        margin: '0 0 0 5',
                                        xtype: 'numericfield',
                                        name: 'InvoiceCurrencyRate',
                                        columnWidth: 0.5,
                                        hideTrigger: false,
                                        useThousandSeparator: true,
                                        decimalPrecision: 2,
                                        alwaysDisplayDecimals: true,
                                        allowNegative: false,
                                        alwaysDecimals: true,
                                        thousandSeparator: ',',
                                        fieldLabel: 'Rate',
                                        labelAlign: 'top',
                                        fieldStyle: 'font-size:11px;text-align:right;',
                                        allowBlank: false
                                    }, {
                                        xtype: 'combo',
                                        columnWidth: 1,
                                        name: 'InvoicePaymentTerms',
                                        fieldLabel: 'Payment Terms',
                                        valueField: 'TermKey',
                                        displayField: 'x_Description',
                                        store: storePaymentTerms,
                                        queryMode: 'local',
                                        typeAhead: false,
                                        minChars: 2,
                                        forceSelection: true,
                                        anyMatch: true
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        name: 'InvoiceCustReference',
                                        fieldLabel: 'Customer Ref. Num.'

                                    }]
                                }, {
                                    margin: '10 0 0 5',
                                    padding: '0 10 10 10',
                                    columnWidth: 0.5,
                                    xtype: 'fieldset',
                                    title: 'Customer Info.',
                                    layout: {
                                        type: 'column'
                                    },
                                    items: [{
                                        xtype: 'radiogroup',
                                        name: 'InvoiceRecipient',
                                        columnWidth: 1,
                                        defaults: {
                                            flex: 1
                                        },
                                        layout: 'hbox',
                                        items: [{
                                            boxLabel: 'Customer',
                                            name: 'recipient',
                                            inputValue: 0,
                                            checked: true
                                        }, {
                                            boxLabel: 'Vendor',
                                            name: 'recipient',
                                            inputValue: 1
                                        }],
                                        listeners: {
                                            change: function(that, newValue, oldValue, eOpts) {
                                                var me = this.up('form');
                                                if(newValue.recipient === 0) {
                                                    me.down("#custContainer").setVisible(true);
                                                    me.down("#vendorContainer").setVisible(false);
                                                } else {
                                                    me.down("#custContainer").setVisible(false);
                                                    me.down("#vendorContainer").setVisible(true);
                                                }
                                            }
                                        }
                                    }, {
                                        xtype: 'container',
                                        itemId: 'custContainer',
                                        columnWidth: 1,
                                        layout: 'column',
                                        items: [{
                                            xtype: 'combo',
                                            name: 'InvoiceCustKey',
                                            fieldLabel: 'Customer',
                                            columnWidth: 1,
                                            valueField: 'CustKey',
                                            displayField: 'CustName',
                                            store: storeCustomers,
                                            queryMode: 'remote',
                                            pageSize: 11,
                                            minChars: 2,
                                            allowBlank: false,
                                            triggerAction: '',
                                            forceSelection: false,
                                            queryCaching: false,
                                            emptyText: 'Choose Customer',
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
                                                    fn: me.onCustomerBlur,
                                                    scope: me
                                                }
                                            }
                                        }, {
                                            xtype: 'combo',
                                            name: 'InvoiceCustContactKey',
                                            fieldLabel: 'Contact',
                                            columnWidth: 1,
                                            queryMode: 'local',
                                            store: null,
                                            valueField: 'ContactKey',
                                            displayField: 'x_ContactFullName',
                                            triggerAction: '',
                                            anyMatch: true
                                        }]
                                    }, {
                                        xtype: 'container',
                                        itemId: 'vendorContainer',
                                        columnWidth: 1,
                                        hidden: true,
                                        layout: 'column',
                                        items: [{
                                            xtype: 'combo',
                                            name: 'InvoiceVendorKey',
                                            fieldLabel: 'Vendor',
                                            columnWidth: 1,
                                            valueField: 'VendorKey',
                                            displayField: 'VendorName',
                                            store: storeVendors,
                                            queryMode: 'remote',
                                            pageSize: 11,
                                            minChars: 2,
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
                                            name: 'InvoiceVendorContactKey',
                                            fieldLabel: 'Contact',
                                            columnWidth: 1,
                                            queryMode: 'local',
                                            store: null,
                                            valueField: 'ContactKey',
                                            displayField: 'x_ContactFullName',
                                            triggerAction: '',
                                            anyMatch: true
                                        }]
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        name: 'InvoiceBillingName',
                                        fieldLabel: 'Name'

                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        name: 'InvoiceBillingAddress1',
                                        fieldLabel: 'Address'

                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        name: 'InvoiceBillingAddress2'
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 0.3,
                                        fieldLabel: 'City',
                                        name: 'InvoiceBillingCity'
                                    }, {
                                        xtype: 'combo',
                                        margin: '0 0 0 5',
                                        columnWidth: 0.4,
                                        fieldLabel: 'State',
                                        name: 'InvoiceBillingState',
                                        displayField: 'StateName',
                                        queryMode: 'local',
                                        typeAhead: true,
                                        minChars: 2,
                                        forceSelection: true,
                                        store: storeStates,
                                        valueField: 'StateCode',
                                        emptyText: 'Choose State',
                                        anyMatch: true
                                    }, {
                                        xtype: 'textfield',
                                        margin: '0 0 0 5',
                                        columnWidth: 0.3,
                                        fieldLabel: 'Zip',
                                        name: 'CustZip',
                                    }, {
                                        xtype: 'combo',
                                        columnWidth: 1,
                                        fieldLabel: 'Country',
                                        name: 'InvoiceBillingCountryKey',
                                        queryMode: 'local',
                                        typeAhead: true,
                                        minChars: 2,
                                        forceSelection: true,
                                        emptyText: 'Choose Country',
                                        displayField: 'CountryName',
                                        store: storeCountries,
                                        valueField: 'CountryKey',
                                        anyMatch: true
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        name: 'InvoiceCustShipKey',
                                        fieldLabel: 'Ship Address',
                                        anchor: '100%',
                                        valueField: 'ShipKey',
                                        displayField: 'ShipName',
                                        store: null,
                                        queryMode: 'local',
                                        minChars: 2,
                                        allowBlank: false,
                                        forceSelection: false,
                                        queryCaching: false,
                                        anyMatch: true,
                                        listeners: {
                                            select: function(field, records, eOpts) {
                                                if(records[0])
                                                    field.next().setValue(records[0].data.x_FullShipAddress);
                                            }
                                        }
                                    }, {
                                        columnWidth: 1,
                                        margin:'5 0 0 0',
                                        xtype: 'textarea',
                                        name: 'x_FullShipAddress',
                                        editable: false,
                                        readOnly: true
                                    }]
                                }
                            ]
                        },
                        // Charges Panel
                        {
                            xtype: 'panel',
                            title: 'Charges',
                            margin: '0 0 10 0',
                            layout: {
                                type: 'column'
                            },
                            items: [
                                // Charges SubTotals
                                {
                                    xtype: 'fieldset',
                                    title: 'Charges Sub Totals',
                                    columnWidth: 1,
                                    padding: '5 5 15 5',
                                    layout: 'column',
                                    items: [
                                        // Grid Charges SubTotals
                                        {
                                            xtype: 'grid',
                                            itemId: 'gridchargesst',
                                            autoScroll: true,
                                            viewConfig: {
                                                stripeRows: true
                                            },
                                            columnWidth: 1,
                                            minHeight: 120,
                                            margin: '0 5 5 0',
                                            store: storeChargesST,
                                            columns: [{
                                                xtype: 'gridcolumn',
                                                flex: 1,
                                                dataIndex: 'x_Category',
                                                text: 'Category',
                                                editor: {
                                                    xtype: 'combo',
                                                    displayField: 'STDescriptionText',
                                                    valueField: 'SubTotalKey',
                                                    name: 'ISTSubTotalKey',
                                                    fieldStyle: 'font-size:11px',
                                                    queryMode: 'local',
                                                    typeAhead: true,
                                                    minChars: 2,
                                                    allowBlank: false,
                                                    forceSelection: true,
                                                    emptyText: 'Choose Category',
                                                    autoSelect: false,
                                                    matchFieldWidth: false,
                                                    listConfig: {
                                                        width: 400
                                                    },
                                                    anyMatch: true,
                                                    listeners: {
                                                        change: function(field) {
                                                            var form = field.up('panel');
                                                            form.onFieldChange();
                                                        },
                                                        select: function(field, records, eOpts) {
                                                            var form = field.up('panel'),
                                                                record = form.context.record;

                                                            if (records.length > 0) {
                                                                record.set('x_Category', field.getRawValue());
                                                            }
                                                        }
                                                    },
                                                    store: storetlkpCategories
                                                }
                                            }, {
                                                xtype: 'gridcolumn',
                                                flex: 1,
                                                dataIndex: 'x_Location',
                                                text: 'Location',
                                                editor: {
                                                    xtype: 'combo',
                                                    displayField: 'ListText',
                                                    valueField: 'ListKey',
                                                    name: 'ISTLocation',
                                                    fieldStyle: 'font-size:11px',
                                                    queryMode: 'local',
                                                    minChars: 2,
                                                    allowBlank: false,
                                                    forceSelection: true,
                                                    emptyText: 'Choose Location',
                                                    autoSelect: false,
                                                    matchFieldWidth: false,
                                                    listConfig: {
                                                        width: 400
                                                    },
                                                    anyMatch: true,
                                                    listeners: {
                                                        change: function(field) {
                                                            var form = field.up('panel');
                                                            form.onFieldChange();
                                                        },
                                                        select: function(field, records, eOpts) {
                                                            var form = field.up('panel'),
                                                                record = form.context.record;

                                                            if (records.length > 0) {
                                                                record.set('x_Location', field.getRawValue());
                                                            }
                                                        }
                                                    },
                                                    selectOnFocus: true,
                                                    store: storetlkpGL
                                                }
                                            }],
                                            tbar: [{
                                                    xtype: 'component',
                                                    flex: 1
                                                }, {
                                                text: 'Add',
                                                itemId: 'addchargest',
                                                handler: function() {
                                                    var me = this.up('gridpanel').up('form'),
                                                        grid = this.up('gridpanel'),
                                                        count = grid.getStore().count(),
                                                        invoiceKey = me.down('#FormToolbar').getCurrentRecord().data.InvoiceKey;

                                                    rowEditingChargesST.cancelEdit();

                                                    var r = Ext.create('CBH.model.jobs.InvoiceChargesSubTotal', {
                                                        ISTInvoiceKey: invoiceKey
                                                    });

                                                    grid.store.insert(count, r);
                                                    rowEditingChargesST.startEdit(count, 1);
                                                },
                                                disabled: true
                                            }, {
                                                itemId: 'removechargest',
                                                text: 'Delete',
                                                hidden: accLevel === 3,
                                                handler: function() {
                                                    var grid = this.up('gridpanel');
                                                    var sm = grid.getSelectionModel();

                                                    rowEditingChargesST.cancelEdit();

                                                    selection = sm.getSelection();

                                                    if (selection) {
                                                        Ext.Msg.show({
                                                            title: 'Delete',
                                                            msg: 'Do you want to delete?',
                                                            buttons: Ext.Msg.YESNO,
                                                            icon: Ext.Msg.QUESTION,
                                                            fn: function(btn) {
                                                                if (btn === "yes") {
                                                                    selection[0].destroy({
                                                                        success: function() {
                                                                            grid.store.reload({
                                                                                callback: function() {
                                                                                    sm.select(0);
                                                                                }
                                                                            });
                                                                        }
                                                                    });
                                                                }
                                                            }
                                                        }).defaultButton = 2;
                                                    }
                                                },
                                                disabled: true
                                            }],
                                            selType: 'rowmodel',
                                            plugins: [rowEditingChargesST],
                                            listeners: {
                                                selectionchange: function(view, records, eOpts) {
                                                    var me = this.up('form'),
                                                        toolbar = me.down('toolbar');

                                                    if (toolbar.isEditing === true) {
                                                        this.down('#removechargest').setDisabled(!records.length);
                                                    }
                                                }
                                            }
                                        }
                                    ]
                                },
                                // Grid Charges
                                {
                                    xtype: 'grid',
                                    itemId: 'gridcharges',
                                    autoScroll: true,
                                    viewConfig: {
                                        stripeRows: true
                                    },
                                    columnWidth: 1,
                                    minHeight: 340,
                                    margin: '0 5 5 0',
                                    store: storeCharges,
                                    features: [{
                                        ftype: 'summary'
                                    }],
                                    columns: [{
                                        xtype: 'numbercolumn',
                                        flex: 0.25,
                                        dataIndex: 'IChargeSort',
                                        text: 'Sort',
                                        format: '00,000'
                                    }, {
                                        xtype: 'numbercolumn',
                                        flex: 0.6,
                                        dataIndex: 'IChargeQty',
                                        text: 'Qty.',
                                        align: 'right',
                                        format: '00,000'
                                    }, {
                                        xtype: 'gridcolumn',
                                        flex: 1,
                                        dataIndex: 'x_ChargeDescription',
                                        text: 'Description'
                                    }, {
                                        xtype: 'gridcolumn',
                                        flex: 1.5,
                                        dataIndex: 'IChargeMemo',
                                        text: 'Notes & Comments',
                                        summaryType: 'count',
                                        summaryRenderer: function(value, summaryData, dataIndex) {
                                            return Ext.String.format('Total for {0} items', value, value !== 1 ? value : 0);
                                        }
                                    }, {
                                        xtype: 'numbercolumn',
                                        flex: 0.6,
                                        dataIndex: 'IChargeCost',
                                        text: 'Cost',
                                        align: 'right',
                                        renderer: Ext.util.Format.usMoney,
                                        summaryType: 'sum'
                                    }, {
                                        xtype: 'numbercolumn',
                                        flex: 0.6,
                                        dataIndex: 'IChargePrice',
                                        text: 'Price',
                                        align: 'right',
                                        renderer: Ext.util.Format.usMoney,
                                        summaryType: 'sum'
                                    }, {
                                        xtype: 'gridcolumn',
                                        flex: 0.5,
                                        dataIndex: 'IChargeCurrencyCode',
                                        text: 'Currency'
                                    }, {
                                        xtype: 'actioncolumn',
                                        draggable: false,
                                        width: 35,
                                        resizable: false,
                                        hideable: false,
                                        stopSelection: false,
                                        items: [{
                                            handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                                var form = Ext.widget('InvoiceEditCharges');
                                                form.modal = true;
                                                form.loadRecord(record);
                                                form.callerForm = this.up('form');
                                                form.show();
                                            },
                                            getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                            tootip: 'view details'
                                        }]
                                    }],
                                    tbar: [{
                                        xtype: 'component',
                                        flex: 1
                                    }, {
                                        text: 'Add',
                                        itemId: 'addcharge',
                                        handler: function() {
                                            var me = this.up('form'),
                                            toolbar = me.down('#FormToolbar'),
                                            currentIN = toolbar.getCurrentRecord().data,
                                            totalCharges = toolbar.store.getCount();

                                            var form = Ext.widget('InvoiceEditCharges');
                                            record = Ext.create('CBH.model.jobs.InvoiceCharges', {
                                                IChargeInvoiceKey: currentIN.InvoiceKey,
                                                IChargeCurrencyCode: 'USD',
                                                IChargeCurrencyRate: 1,
                                                IChargeSort: (totalCharges) * 100
                                            });

                                            form.loadRecord(record);
                                            form.center();
                                            form.callerForm = this.up('form');
                                            form.show();
                                        },
                                        disabled: true
                                    }, {
                                        itemId: 'removecharge',
                                        text: 'Delete',
                                        handler: function() {
                                            var grid = this.up('gridpanel');
                                            var sm = grid.getSelectionModel();

                                            //rowEditingCharges.cancelEdit();

                                            selection = sm.getSelection();

                                            if (selection) {
                                                selection[0].destroy({
                                                    success: function() {
                                                        grid.store.reload({
                                                            callback: function() {
                                                                sm.select(0);
                                                            }
                                                        });
                                                    }
                                                });
                                            }
                                        },
                                        disabled: true
                                    }],
                                    selType: 'rowmodel',
                                    /*plugins: [rowEditingCharges],*/
                                    listeners: {
                                        selectionchange: function(view, records, eOpts) {
                                            this.down('#removecharge').setDisabled(!records.length);
                                        }
                                    }
                                }
                            ]
                        },
                        // Panel Items
                        {
                            xtype: 'panel',
                            title: 'Items',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridInvoiceItems',
                                minHeight: 350,
                                store: storeInvoiceItems,
                                maxHeight: 400,
                                features: [{
                                    ftype: 'summary'
                                }],
                                columns: [{
                                    xtype: 'gridcolumn',
                                    text: 'Sort',
                                    dataIndex: 'ISummarySort',
                                    format: '00,000'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Qty.',
                                    dataIndex: 'ISummaryQty',
                                    format: '00,000.00'
                                }, {
                                    xtype:'gridcolumn',
                                    text:'Vendor',
                                    dataIndex:'x_ItemVendorName',
                                    flex:1
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Item Num.',
                                    dataIndex: 'ISummaryItemNum',
                                    flex: 1
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Description',
                                    dataIndex: 'ISummaryDescription',
                                    flex: 1,
                                    summaryType: 'count',
                                    summaryRenderer: function(value, summaryData, dataIndex) {
                                        return Ext.String.format('Total for {0} items', value, value !== 1 ? value : 0);
                                    }
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Cost',
                                    dataIndex: 'ISummaryPrice',
                                    align: 'right',
                                    format: '00,000.00'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Price',
                                    dataIndex: 'ISummaryLinePrice',
                                    align: 'right',
                                    format: '00,000.00',
                                    summaryType: 'sum'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'CUR',
                                    dataIndex: 'ISummaryCurrencyCode'
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    items: [{
                                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                            var me = this.up('form');

                                            var form = Ext.widget('InvoiceEditItems', {
                                                currentRecord: record,
                                                VendorKey: record.data.ISummaryVendorKey
                                            });
                                            form.modal = true;
                                            form.loadRecord(record);
                                            form.callerForm = this.up('form');
                                            form.show();
                                        },
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                        tootip: 'view details'
                                    }]
                                }],
                                tbar: [
                                    // Search Field
                                    {
                                        xtype: 'searchfield',
                                        width: 400,
                                        itemId: 'itemsearchfield',
                                        listeners: {
                                            'triggerclick': function(field) {
                                                me.onItemSearchFieldChange();
                                            }
                                        }
                                    }, {
                                        xtype: 'component',
                                        flex: 1
                                    }, {
                                        text: 'Add',
                                        itemId: 'additem',
                                        handler: function() {
                                            var me = this.up('form'),
                                                toolbar = me.down('#FormToolbar'),
                                                currentIN = toolbar.getCurrentRecord().data,
                                                totalItems = toolbar.store.getCount();

                                            var record = Ext.create('CBH.model.jobs.InvoiceItemsSummary', {
                                                ISummaryInvoiceKey: currentIN.InvoiceKey,
                                                ISummaryVendorKey: currentIN.InvoiceVendorKey,
                                                ISummaryCurrencyCode: currentIN.InvoiceCurrencyCode,
                                                ISummaryCurrencyRate: currentIN.InvoiceCurrencyRate,
                                                ISummarySort: (totalItems) * 100
                                            });


                                            var form = Ext.widget('InvoiceEditItems', {
                                                currentRecord: record,
                                                VendorKey: currentIN.InvoiceVendorKey
                                            });
                                            form.modal = true;
                                            form.loadRecord(record);
                                            form.callerForm = this.up('form');
                                            form.show();
                                        },
                                        disabled: true
                                    }, {
                                        itemId: 'deleteitem',
                                        text: 'Delete',
                                        hidden: accLevel === 3,
                                        handler: function() {
                                            var grid = this.up('gridpanel');
                                            var sm = grid.getSelectionModel();

                                            selection = sm.getSelection();

                                            if (selection) {
                                                Ext.Msg.show({
                                                    title: 'Delete',
                                                    msg: 'Do you want to delete?',
                                                    buttons: Ext.Msg.YESNO,
                                                    icon: Ext.Msg.QUESTION,
                                                    fn: function(btn) {
                                                        if (btn === "yes") {
                                                            selection[0].destroy({
                                                                success: function() {
                                                                    grid.store.remove(sm.getSelection());
                                                                    if (grid.store.getCount() > 0) {
                                                                        sm.select(0);
                                                                    }
                                                                }
                                                            });
                                                        }
                                                    }
                                                }).defaultButton = 2;
                                            }
                                        },
                                        disabled: true
                                    }
                                ],
                                bbar: new Ext.PagingToolbar({
                                    itemId: 'itemspagingtoolbar',
                                    store: storeInvoiceItems,
                                    displayInfo: true,
                                    displayMsg: 'Displaying records {0} - {1} of {2}',
                                    emptyMsg: "No records to display"
                                }),
                                selType: 'rowmodel',
                                plugins: [rowEditingItems],
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deleteitem').setDisabled(!records.length);
                                    }
                                }
                            }]
                        },
                        // Panel Memo
                        {
                            xtype: 'panel',
                            title: 'Notes',
                            layout: 'column',
                            minHeight: 350,
                            items: [{
                                margin: '0 0 0 0',
                                columnWidth: 1,
                                xtype: 'combo',
                                displayField: 'name',
                                valueField: 'id',
                                fieldLabel: 'Notes Font Color',
                                name: 'InvoiceMemoFont',
                                queryMode: 'local',
                                typeAhead: true,
                                minChars: 1,
                                forceSelection: true,
                                emptyText: 'choose color',
                                enableKeyEvents: true,
                                autoSelect: true,
                                selectOnFocus: true,
                                defaultValue: 0,
                                store: {
                                    fields: ['id', 'name'],
                                    data: [{
                                        'id': 0,
                                        'name': 'Black'
                                    }, {
                                        'id': 1,
                                        'name': 'Green'
                                    }, {
                                        'id': 2,
                                        'name': 'Blue'
                                    }, {
                                        'id': 3,
                                        'name': 'Red'
                                    }]
                                }
                            }, {
                                xtype: 'textareafield',
                                margin: '5 0 0 0',
                                selectOnFocus: true,
                                columnWidth: 1,
                                rows: 6,
                                name: 'InvoiceMemo'
                            }]
                        }
                    ]
                }
            ],
            dockedItems: [{
                xtype: 'formtoolbar',
                itemId: 'FormToolbar',
                dock: 'top',
                store: me.storeNavigator,
                printEnabled: true,
                listeners: {
                    addrecord: {
                        fn: me.onAddClick,
                        scope: me
                    },
                    savechanges: {
                        fn: me.onSaveClick,
                        scope: me
                    },
                    deleterecord: {
                        fn: me.onDeleteClick,
                        scope: me
                    },
                    afterloadrecord: {
                        fn: me.onAfterLoadRecord,
                        scope: me
                    },
                    beginedit: {
                        fn: me.onBeginEdit,
                        scope: me
                    },
                    printrecord: {
                        fn: me.onPrintRecord,
                        scope: me
                    }
                }
            }, {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'textfield',
                    name: 'InvoiceCreatedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created By'
                }, {
                    xtype: 'datetimefield',
                    name: 'InvoiceCreatedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created Date',
                    hideTrigger: true
                }, {
                    xtype: 'textfield',
                    name: 'InvoiceModifiedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified By'
                }, {
                    xtype: 'datetimefield',
                    name: 'InvoiceModifiedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true
                }, {
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    itemId: 'btnPrintInvoiceWSch',
                    text: 'Print Invoice w/ SchB',
                    listeners: {
                        click: {
                            fn: me.onClickPrintInvoiceWSch,
                            scope: me
                        }
                    }
                }, {
                    xtype: 'button',
                    itemId: 'btnPrintPackingList',
                    text: 'Print Packing List',
                    listeners: {
                        click: {
                            fn: me.onClickPrintPackingList,
                            scope: me
                        }
                    }
                }, {
                    xtype: 'button',
                    itemId: 'btnExportInvToPeach',
                    text: 'Export Invoice To Peachtree',
                    handler: me.onClickExportInvToPeach
                }]
            }],
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                },
                afterrender: {
                    fn: me.registerKeyBindings,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    registerKeyBindings: function(view, options) {
        var me = this;
        Ext.EventManager.on(view.getEl(), 'keyup', function(evt, t, o) {
                if (evt.ctrlKey && evt.keyCode === Ext.EventObject.F8) { 
                    evt.stopEvent();
                    var toolbar = me.down('#FormToolbar');
                    if(toolbar.isEditing) {
                        var btn = toolbar.down('#save');
                        btn.fireEvent('click');
                    }
                }
            },
            this);
    },

    onRenderForm: function() {
        var me = this;
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this;

        if (record.phantom) {
            me.down('#addchargest').setDisabled(true);
            me.down('#addcharge').setDisabled(true);
            me.down('#additem').setDisabled(true);
            return;
        }

        me.down('#addchargest').setDisabled(false);
        me.down('#addcharge').setDisabled(false);
        me.down('#additem').setDisabled(false);


        var jobkey = record.data.InvoiceJobKey,
            custkey = record.data.InvoiceCustKey;
        vendorkey = record.data.InvoiceVendorKey;

        curRec = record.data;
        me.storeToLoad = 8;
        me.storesLoaded = [];

        Ext.Msg.wait('Loading data...', 'Wait');

        var storeCustomers = new CBH.store.customers.Customers().load({
            params: {
                id: custkey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function() {
                var fieldCust = me.down("field[name=InvoiceCustKey]");
                fieldCust.getStore().removeAll();
                fieldCust.bindStore(storeCustomers);
                me.down('field[name=x_InvoiceCustKey]').setValue(custkey);
                me.down('field[name=InvoiceCustKey]').setValue(custkey);
                me.storesLoaded.push("Customers");
                me.checkLoaded();
            }
        });

        var storeCustomerContacts = new CBH.store.customers.CustomerContacts().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                custkey: custkey
            },
            callback: function() {
                var fieldContact = me.down("field[name=InvoiceCustContactKey]");
                fieldContact.getStore().removeAll();
                fieldContact.bindStore(storeCustomerContacts);
                fieldContact.setValue(curRec.InvoiceCustContactKey);

                /*if (this.totalCount > 0 && String.isNullOrEmpty(fieldContact.getValue())) {
                    var record = this.getAt(0);
                    fieldContact.setValue(record.data.ContactKey);
                }*/

                me.storesLoaded.push("CustomerContacts");
                me.checkLoaded();
            }
        });

        if (vendorkey === null)
            vendorkey = 0;

        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                id: vendorkey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function() {
                var fieldVendor = me.down("field[name=InvoiceVendorKey]");
                fieldVendor.getStore().removeAll();
                fieldVendor.bindStore(storeVendors);

                if (this.totalCount > 0 && String.isNullOrEmpty(fieldVendor.getValue())) {
                    var record = this.getAt(0);
                    fieldVendor.setValue(record.data.VendorKey);
                    //me.down('field[name=x_VendorAddress]').setValue(record.data.VendorAddress1);                                        
                }

                me.storesLoaded.push("Vendors");
                me.checkLoaded();
            }
        });

        var storeVendorContacts = new CBH.store.vendors.VendorContacts().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                vendorkey: vendorkey
            },
            callback: function() {
                var fieldContact = me.down("field[name=InvoiceVendorContactKey]");
                fieldContact.getStore().removeAll();
                fieldContact.bindStore(storeVendorContacts);

                if (this.totalCount > 0 && String.isNullOrEmpty(fieldContact.getValue())) {
                    var record = this.getAt(0);
                    fieldContact.setValue(record.data.ContactKey);
                }

                me.storesLoaded.push("VendorContacts");
                me.checkLoaded();
            }
        });

        var storeCustomerShipAddress = new CBH.store.customers.CustomerShipAddress().load({
            params: {
                custkey: custkey,
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function(records, success, eOpts) {
                me.down('field[name=InvoiceCustShipKey]').bindStore(this);
                me.down('field[name=InvoiceCustShipKey]').setValue(curRec.InvoiceCustShipKey);
                
                var index = this.find('ShipKey',curRec.InvoiceCustShipKey);
                if(index > -1) {
                    me.down('field[name=x_FullShipAddress]').setValue(records[index].data.x_FullShipAddress);
                }

                me.storesLoaded.push("CustomerShipAddress");
                me.checkLoaded();
            }
        });

        var storeInvoiceItems = new CBH.store.jobs.InvoiceItemsSummary().load({
            params: {
                InvoiceKey: curRec.InvoiceKey
            },
            callback: function() {
                var grid = me.down("#gridInvoiceItems");
                grid.reconfigure(storeInvoiceItems);
                me.down('#itemspagingtoolbar').bindStore(storeInvoiceItems);

                me.storesLoaded.push("CBH.store.jobs.InvoiceItemsSummary");
                me.checkLoaded();
            }
        });

        var storeInvoiceCharges = new CBH.store.jobs.InvoiceCharges().load({
            scope: storeInvoiceCharges,
            params: {
                InvoiceKey: curRec.InvoiceKey
            },
            callback: function() {
                var grid = me.down("#gridcharges");
                grid.reconfigure(storeInvoiceCharges);
                me.storesLoaded.push("InvoiceCharges");
                me.checkLoaded();
            }
        });

        var storeChargesST = new CBH.store.jobs.InvoiceChargesSubTotal().load({
            params: {
                InvoiceKey: curRec.InvoiceKey
            },
            callback: function() {
                var grid = me.down('#gridchargesst');
                grid.reconfigure(storeChargesST);
                me.storesLoaded.push("InvoiceCharges");
                me.checkLoaded();
            }
        });
    },

    checkLoaded: function() {
        var me = this,
            stores = me.storesLoaded;

        if (stores.length < me.storeToLoad) {
            return;
        }

        Ext.Msg.hide();
    },

    onAddClick: function(toolbar, record) {
        var me = this,
            grid = me.down('#gridroles');

        grid.store.removeAll();
    },

    onBeginEdit: function(toolbar, record) {
        var me = toolbar.up('form');
    },

    onSaveClick: function(toolbar, record) {
        var me = this,
            form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        var savedRecord = form.getRecord();

        if (!savedRecord.phantom) {
            savedRecord.set('InvoiceModifiedBy', CBH.GlobalSettings.getCurrentUserName());
        }

        Ext.Msg.wait('Saving Record...', 'Wait');

        savedRecord.save({
            callback: function(records, operation, success) {
                if (success) {
                    Ext.Msg.hide();
                    toolbar.doRefresh();
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    },

    onDeleteClick: function(toolbar, record) {
        if (record) {
            var curRec = record.index - 1;
            curPage = toolbar.store.currentPage;
            prevRec = (curRec <= 0) ? 1 : curRec;

            Ext.Msg.wait('Deleting Record...', 'Wait');
            record.destroy({
                success: function() {
                    var lastOpt = toolbar.store.lastOptions,
                        params = (lastOpt) ? lastOpt.params : null;

                    toolbar.store.reload({
                        params: params,
                        callback: function() {}
                    });
                    if (toolbar.store.getCount() > 0) {
                        toolbar.gotoAt(prevRec);
                    } else {
                        toolbar.up('form').up('panel').destroy();
                    }
                    Ext.Msg.hide();
                },
                failure: function() {
                    Ext.Msg.hide();
                }
            });
        }
    },

    onDestroy: function(e, eOpts) {
        //...
    },

    // Event Blur CustKey Field
    onCustomerBlur: function(field, The, eOpts) {
        var form = field.up('panel');

        if (field.value !== null && field.valueModels[0]) {

            var currentCust = field.value,
                fieldContact = form.down('field[name=JobContactKey]');

            var contacts = new CBH.store.customers.CustomerContacts().load({
                params: {
                    page: 0,
                    start: 0,
                    limit: 0,
                    custkey: currentCust
                },
                callback: function() {
                    fieldContact.getStore().removeAll();
                    fieldContact.bindStore(contacts);

                    if (this.totalCount > 0 && String.isNullOrEmpty(fieldContact.getValue())) {
                        var record = this.getAt(0);
                        fieldContact.setValue(record.data.ContactKey);
                    }
                }
            });

            var fieldShip = form.down('field[name=JobCustShipKey]');
            var address = new CBH.store.customers.CustomerShipAddress().load({
                params: {
                    page: 0,
                    start: 0,
                    limit: 0,
                    custkey: currentCust
                },
                callback: function() {
                    fieldShip.getStore().removeAll();
                    fieldShip.bindStore(address);

                    if (this.totalCount > 0 && String.isNullOrEmpty(fieldShip.getValue())) {
                        var index = this.find('ShipDefault', true);
                        index = (index < 0) ? 0 : index;
                        var record = this.getAt(index);
                        fieldShip.setValue(record.data.ShipKey);
                    }
                }
            });
        } else {
            var rawvalue = field.getRawValue();
            Ext.Msg.show({
                title: 'Question',
                msg: 'The customer doesn\'t exists, Do you want to add to database?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(e) {
                    if (e === "yes") {
                        form.addCustomer(rawvalue);
                    } else {
                        field.setValue(null);
                        field.focus(true, 200);
                    }
                }
            });
        }
    },

    // Event Blur CustKey Field
    onVendorBlur: function(field, The, eOpts) {
        var me = field.up('form');

        if (field.value !== null && field.valueModels[0]) {
            var currentVendor = field.value,
                fieldContact = me.down('field[name=InvoiceVendorContactKey]'); // fieldOriginAddress = me.down('field[name=POVendorOriginAddress]')

            var contacts = new CBH.store.vendors.VendorContacts().load({
                params: {
                    page: 0,
                    start: 0,
                    limit: 0,
                    vendorkey: currentVendor
                },
                callback: function() {
                    fieldContact.getStore().removeAll();
                    fieldContact.bindStore(contacts);

                    if (this.totalCount > 0 && String.isNullOrEmpty(fieldContact.getValue())) {
                        var record = this.getAt(0);
                        fieldContact.setValue(record.data.ContactKey);
                    }
                }
            });
        }
    },

    addCustomer: function(value) {
        var me = this;

        var storeToNavigate = new CBH.store.customers.Customers({
            autoLoad: false
        });
        model = Ext.create('CBH.model.customers.Customers', {
            CustName: value,
            CustStatus: 1
        });
        storeToNavigate.add(model);
        var form = Ext.widget('customers', {
            storeNavigator: storeToNavigate,
            modal: true,
            width: 700,
            frameHeader: true,
            header: true,
            layout: {
                type: 'column'
            },
            title: 'New Customer',
            bodyPadding: 10,
            closable: true,
            //constrain: true,
            stateful: false,
            floating: true,
            callerForm: me,
            forceFit: true
        });

        form.show();

        var btn = form.down('#FormToolbar').down('#add');
        btn.fireEvent('click', btn, null, null, model); // aditional param model for send model data to click event handler
    },

    onContactBlur: function(field, The, eOpts) {
        var form = field.up('panel');

        if (field.value !== null && field.valueModels[0]) {

        } else if (form.down('field[name=JobCustKey]').value !== null) {
            Ext.Msg.show({
                title: 'Question',
                msg: 'The contact doesn\'t exists, Do you want to add to database?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(e) {
                    if (e === "yes") {
                        form.addContact(field);
                    } else {
                        field.setValue(null);
                        field.focus(true, 200);
                    }
                },
                scope: field
            });
        }
    },

    addContact: function(field) {
        var me = this,
            rawvalue = field.getRawValue(),
            res = rawvalue.split(' '),
            firstName = (res[0]) ? res[0] : '',
            lastName = (res[1]) ? res[1] : '',
            custkey = me.down('field[name=JobCustKey]').getValue();

        record = Ext.create('CBH.model.customers.CustomerContacts', {
            ContactCustKey: custkey,
            ContactFirstName: firstName,
            ContactLastName: lastName
        });

        var form = Ext.create('CBH.view.customers.CustomerContacts');
        form.loadRecord(record);
        form.center();
        form.callerForm = me;
        form.show();
    },

    onShipAddressBlur: function(field, The, eOpts) {
        var form = field.up('panel');

        if (field.value !== null && field.valueModels[0]) {

        } else if (form.down('field[name=JobCustKey]').value !== null) {
            Ext.Msg.show({
                title: 'Question',
                msg: 'The ship address doesn\'t exists, Do you want to add to database?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(e) {
                    if (e === "yes") {
                        form.addShipAddress(field);
                    } else {
                        field.setValue(null);
                        field.focus(true, 200);
                    }
                },
                scope: field
            });
        }
    },

    addShipAddress: function(field) {
        var me = this,
            rawvalue = field.getRawValue();

        var custkey = me.down('field[name=JobCustKey]').getValue();
        record = Ext.create('CBH.model.customers.CustomerShipAddress', {
            ShipCustKey: custkey
        });

        var form = new CBH.view.customers.CustomerShipAddress();
        form.loadRecord(record);
        form.center();
        form.callerForm = me;
        form.show();
    },

    onSplitBackorderClick: function() {
        var me = this;

        Ext.Msg.show({
            title: 'Split Purchase Order',
            msg: 'Are you sure you want to split this Purchase Order?',
            buttons: Ext.Msg.YESNO,
            icon: Ext.Msg.QUESTION,
            fn: function(btn) {
                if (btn === "yes") {
                    me.splitBackOrder();
                }
            }
        });
    },

    splitBackOrder: function() {
        var me = this,
            curRec = me.down('formtoolbar').getCurrentRecord(),
            storeItems = me.down('#gridInvoiceItems').getStore();

        // Check if items has backorder split
        var totalQty = storeItems.sum('ISummaryBackorderQty');

        if (totalQty === null || totalQty === 0) {
            Ext.Msg.alert('Warning', "Purchase Order don't have items with Backorder Qty.");
            return;
        }

        Ext.Msg.wait('Please wait....', 'Wait');

        Ext.Ajax.request({
            method: 'GET',
            type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/SplitBackOrder',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                POKey: curRec.data.POKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName
            },
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                Ext.Msg.hide();
            }
        });
    },

    // plugins
    loadPluginItems: function() {
        var me = this;

        return new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 100,
                    fn: function(item, e) {
                        var editor = this.getEditor();
                        editor.down('field[name=ISummaryBackorderQty]').focus(true, 200);
                        this.getEditor().onFieldChange();
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('#gridInvoiceItems');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('#gridInvoiceItems'),
                            record = context.record,
                            fromEdit = true,
                            isPhantom = record.phantom;

                        record.save({
                            callback: function() {
                                grid.store.reload();
                            }
                        });
                    }
                }
            }
        });
    },

    loadPluginCharges: function() {
        return new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 100,
                    fn: function(item, e) {
                        this.getEditor().onFieldChange();
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel'),
                            record = context.record,
                            fromEdit = true,
                            isPhantom = record.phantom;

                        record.save({
                            callback: function() {
                                grid.store.reload();
                            }
                        });
                    }
                }
            }
        });
    },

    loadPluginChargesST: function() {
        return new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 100,
                    fn: function(item, context) {
                        var me = this.getEditor().up('form'),
                            formEditing = this.getEditor(),
                            toolbar = me.down('#FormToolbar'),
                            masterRecord = toolbar.getCurrentRecord();

                        var record = context.record;

                        formEditing.onFieldChange();

                        var filterQHdr = new Ext.util.Filter({
                            property: 'ListCategory',
                            value: 1
                        });

                        Ext.Msg.wait('Loading', 'Loading Data...');

                        var storetlkpGL = new CBH.store.sales.tlkpGenericLists().load({
                            params: {
                                page: 0,
                                start: 0,
                                limit: 0
                            },
                            filters: [filterQHdr],
                            callback: function() {
                                field = formEditing.down('field[name=ISTLocation]').bindStore(storetlkpGL);
                                field.setValue(record.data.ISTLocation);

                                var filterQHdr = new Ext.util.Filter({
                                    property: 'STDescriptionLanguageCode',
                                    value: masterRecord.data.CustLanguageCode
                                });

                                storetlkpCategories = new CBH.store.sales.tlkpInvoiceSubTotalCategories().load({
                                    params: {
                                        page: 0,
                                        start: 0,
                                        limit: 0
                                    },
                                    filters: [filterQHdr],
                                    callback: function() {
                                        field = formEditing.down('field[name=ISTSubTotalKey]').bindStore(storetlkpCategories);
                                        field.setValue(record.data.ISTSubTotalKey);

                                        Ext.Msg.hide();
                                    }
                                });
                            }
                        });
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel'),
                            record = context.record,
                            fromEdit = true,
                            isPhantom = record.phantom;

                        record.save({
                            callback: function() {
                                grid.store.reload();
                            }
                        });
                    }
                }
            }
        });
    },

    onClickExportInvToPeach: function() {
        var me = this.up('form'),
            today = new Date(),
            invoiceKey = me.down("field[name=InvoiceKey]").getValue(),
            t = Ext.util.Cookies.get('CBH.UserAuth');

        var url = "{0}/ExportInvoiceToPeachTree?_dc={1}&InvoiceKey={2}&t={3}".format(CBH.GlobalSettings.webApiPath, today.getTime(), invoiceKey, t);
        window.open(url, '_blank');
    },

    onClickPrintPackingList: function() {
        var me = this,
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey(),
            invoiceKey = me.down("field[name=InvoiceKey]").getValue(),
            today = new Date();


        var formData = {
            id: invoiceKey,
            employeeKey: userKey
        };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptJobInvoicePackingList", params);
        window.open(pathReport, 'CBH - Packing List', false);
    },

    onPrintRecord: function(toolbar, record) {
        var userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey();

        var formData = {
                id: record.get('InvoiceKey'),
                employeeKey: userKey
            };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptJobInvoice", params);
        window.open(pathReport, 'CBH - Invoice', false);
    },

    onClickPrintInvoiceWSch: function() {
        var me = this,
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey(),
            invoiceKey = me.down("field[name=InvoiceKey]").getValue(),
            today = new Date();


        var formData = {
            id: invoiceKey,
            employeeKey: userKey,
            wSch: 1
        };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptJobInvoice", params);
        window.open(pathReport, 'CBH - Packing List', false);
    }
});