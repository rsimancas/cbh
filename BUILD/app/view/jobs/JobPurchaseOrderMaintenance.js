Ext.define('CBH.view.jobs.JobPurchaseOrderMaintenance', {
    extend: 'Ext.form.Panel',
    alias: 'widget.jobpurchaseordermaintenance',

    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    title: 'Purchase Order Information',

    requires: [
        'CBH.view.jobs.JobPOEditInstruction',
        'CBH.view.jobs.JobPOEditCharges',
        'CBH.view.jobs.JobPOEditItems',
        'CBH.view.jobs.JobNewInvoiceContinue'
    ],

    POJobKey: 0,
    storeNavigator: null,

    storesLoaded: null,
    storeToLoad: 0,
    currentJob: null,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        var rowEditingItems = me.loadPluginItems();

        Ext.Msg.wait('Loading data...', 'Wait');
        storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        storeCurrencyRatesHeader = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                Ext.Msg.hide();
            }
        });

        var storeVendorContacts = null;
        var storeCustomerShipAddress = new CBH.store.customers.CustomerShipAddress().load({
            params: {
                custkey: me.currentJob.data.JobCustKey,
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function(records, operation, success) {
                if(success && records && records.length) {
                    me.down("field[name=POCustShipKey]").setValue(records[0].get("ShipKey"));
                } else {
                    me.down("field[name=POCustShipKey]").setValue(null);
                }
            }
        });

        var storeEmployeeRoles = null;
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
        var storeInspectionCompanies = new CBH.store.common.InspectionCompanies().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeFreightDestinations = new CBH.store.common.FreightDestinations().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeWarehouseType = new CBH.store.vendors.WarehouseTypes().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storePOItems = null;
        var storePOCharges = null;
        var storeInstructions = null;

        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [
                // Job Key and Reference Num
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
                            name: 'POJobKey',
                            hideTrigger: true,
                            labelAlign: 'left',
                            labelWidth: 60,
                            fieldLabel: 'Job Key',
                            editable: false,
                            fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                        }, {
                            margin: '0 0 0 10',
                            xtype: 'displayfield',
                            name: 'JobNumFormatted',
                            // name: 'x_JobNumFormatted',
                            fieldLabel: 'Reference Num',
                            labelAlign: 'left',
                            labelWidth: 80,
                            editable: false,
                            fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right',
                            value: me.currentJob.data.JobNum
                        }, {
                            margin: '0 0 0 10',
                            xtype: 'displayfield',
                            name: 'x_PONumFormatted',
                            fieldLabel: 'PO Num',
                            labelAlign: 'left',
                            labelWidth: 80,
                            editable: false,
                            fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                        }]
                    }]
                },
                // Page Frame
                {
                    xtype: 'tabpanel',
                    itemId: 'tabFeatures',
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
                                //Purchase Order Information Tab Left
                                {
                                    margin: '10 0 0 0',
                                    padding: '0 10 10 10',
                                    columnWidth: 0.5,
                                    xtype: 'fieldset',
                                    //title:'Purchase Order Information',
                                    layout: {
                                        type: 'column'
                                    },
                                    items: [{
                                        xtype: 'textfield',
                                        columnWidth: 0.5,
                                        name: 'PONum',
                                        fieldLabel: 'Purchase Order',
                                        editable: false,
                                        readOnly: true

                                    }, {
                                        margin: '0 0 0 5',
                                        columnWidth: 0.5,
                                        xtype: 'combo',
                                        displayField: 'name',
                                        valueField: 'id',
                                        fieldLabel: 'Revision Num',
                                        name: 'PORevisionNum',
                                        queryMode: 'local',
                                        typeAhead: true,
                                        minChars: 1,
                                        forceSelection: true,
                                        emptyText: 'choose',
                                        enableKeyEvents: true,
                                        autoSelect: true,
                                        selectOnFocus: true,
                                        defaultValue: 0,
                                        store: {
                                            fields: ['id', 'name'],
                                            data: [{
                                                'id': 0,
                                                'name': ''
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
                                        name: 'PODate',
                                        fieldLabel: 'PO Date',
                                        columnWidth: 0.5,
                                        allowBlank: false,
                                    }, {
                                        margin: '0 0 0 5',
                                        xtype: 'datefield',
                                        name: 'POGoodThruDate',
                                        fieldLabel: 'Good Thru Date',
                                        allowBlank: false,
                                        columnWidth: 0.5
                                    }, {
                                        xtype: 'combo',
                                        name: 'POVendorKey',
                                        fieldLabel: 'Vendor',
                                        columnWidth: 1,
                                        valueField: 'VendorKey',
                                        displayField: 'VendorName',
                                        queryMode: 'remote',
                                        autoSelect: false,
                                        minChars: 2,
                                        allowBlank: false,
                                        triggerAction: '',
                                        forceSelection: false,
                                        queryCaching: false, // set for after add a new customer, this control recognize the new customer added
                                        emptyText: 'Choose Vendors',
                                        pageSize: 11,
                                        //queryBy: 'CustName', //Custom property for optimize remote query
                                        listeners: {
                                            buffer: 100,
                                            blur: {
                                                fn: me.onVendorBlur,
                                                scope: this
                                            }
                                        },
                                        store: new CBH.store.vendors.Vendors({
                                            params: {
                                                page: 1,
                                                start: 0,
                                                limit: 8
                                            }
                                        })
                                    }, {
                                        xtype: 'combo',
                                        name: 'POVendorContactKey',
                                        fieldLabel: '- Contact',
                                        columnWidth: 1,
                                        valueField: 'ContactKey',
                                        displayField: 'x_ContactFullName',
                                        store: storeVendorContacts,
                                        queryMode: 'local',
                                        //autoSelect: false,
                                        typeAhead: false,
                                        minChars: 2,
                                        allowBlank: false,
                                        forceSelection: false,
                                        //triggerAction: '',
                                        queryCaching: false, // set for after add a new contact, this control recognize the new contact added
                                        listeners: {
                                            beforequery: function(record) {
                                                record.query = new RegExp(record.query, 'i');
                                                record.forceAll = true;
                                            },
                                            blur: {
                                                fn: me.onContactBlur,
                                                scope: this
                                            }
                                        }
                                    }, {
                                        xtype: 'textarea',
                                        columnWidth: 1,
                                        name: 'x_VendorAddress',
                                        fieldLabel: ' - Address',
                                        rows: 6,
                                        readOnly: true,
                                        editable: false

                                    }, {
                                        margin: '0 0 30 0',
                                        xtype: 'textfield',
                                        columnWidth: 0.5,
                                        name: 'POVendorReference',
                                        fieldLabel: 'Vendor Reference'

                                    }]
                                },
                                //Purchase Order Information Tab Right
                                {
                                    margin: '10 0 0 5',
                                    padding: '0 10 10 10',
                                    columnWidth: 0.5,
                                    xtype: 'fieldset',
                                    layout: {
                                        type: 'column'
                                    },
                                    items: [{
                                        xtype: 'combo',
                                        //margin: '0 0 0 5',
                                        columnWidth: 1,
                                        name: 'POVendorPaymentTerms',
                                        fieldLabel: 'Payment Terms',
                                        valueField: 'TermKey',
                                        displayField: 'x_Description',
                                        store: storePaymentTerms,
                                        queryMode: 'local',
                                        typeAhead: false,
                                        minChars: 2,
                                        forceSelection: true,
                                        listeners: {
                                            beforequery: function(record) {
                                                record.query = new RegExp(record.query, 'i');
                                                record.forceAll = true;
                                            }
                                        }
                                    }, {
                                        xtype: 'combo',
                                        name: 'POCurrencyCode',
                                        fieldLabel: 'PO Currency',
                                        store: storeCurrencyRates,
                                        labelWidth: 50,
                                        listConfig: {
                                            minWidth: null
                                        },
                                        columnWidth: 0.5,
                                        valueField: 'CurrencyCode',
                                        displayField: 'CurrencyCodeDesc',
                                        queryMode: 'local',
                                        autoSelect: false,
                                        //typeAhead: true,
                                        minChars: 2,
                                        allowBlank: false,
                                        forceSelection: true,
                                        listeners: {
                                            select: function(field, records, eOpts) {
                                                if (records.length > 0) {
                                                    var me = field.up('form');
                                                    var rate = records[0].data.CurrencyRate;
                                                    me.down('field[name=POCurrencyRate]').setValue(rate);
                                                }
                                            },
                                            change: function(field, newValue, oldValue) {
                                                if (newValue === null) {
                                                    var me = field.up('form');
                                                    me.down('field[name=POCurrencyRate]').setValue(0);
                                                }
                                            },
                                            blur: function(field, The, eOpts) {
                                                if (field.value !== null) {
                                                    var me = field.up('form');
                                                    var rate = field.valueModels[0].data.CurrencyRate;
                                                    me.down('field[name=POCurrencyRate]').setValue(rate);
                                                }
                                            },
                                            beforequery: function(record) {
                                                record.query = new RegExp(record.query, 'i');
                                                record.forceAll = true;
                                            }
                                        }
                                    }, {
                                        margin: '0 0 0 5',
                                        xtype: 'numericfield',
                                        columnWidth: 0.5,
                                        name: 'PODefaultProfitMargin',
                                        fieldLabel: 'Default Profit %',
                                        hideTrigger: true,
                                        useThousandSeparator: true,
                                        decimalPrecision: 3,
                                        alwaysDisplayDecimals: true,
                                        allowNegative: false,
                                        //currencySymbol:'$',
                                        alwaysDecimals: true,
                                        thousandSeparator: ',',
                                        labelAlign: 'top',
                                        fieldStyle: 'text-align: right;',
                                        allowBlank: false,
                                        maxValue: 99.99,
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        name: 'POShipmentType',
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
                                        listeners: {
                                            beforequery: function(record) {
                                                record.query = new RegExp(record.query, 'i');
                                                record.forceAll = true;
                                            }
                                        }
                                    }, {
                                        xtype: 'fieldcontainer',
                                        columnWidth: 1,
                                        layout: 'hbox',
                                        items: [{
                                            width: '90%',
                                            xtype: 'combo',
                                            name: 'POVendorOriginAddress',
                                            fieldLabel: 'Vendor Origin',
                                            listConfig: {
                                                minWidth: null
                                            },
                                            valueField: 'OriginKey',
                                            displayField: 'OriginName',
                                            store: null,
                                            queryMode: 'local',
                                            typeAhead: false,
                                            minChars: 2,
                                            //allowBlank: false,
                                            forceSelection: true,
                                            listeners: {
                                                beforequery: function(record) {
                                                    record.query = new RegExp(record.query, 'i');
                                                    record.forceAll = true;
                                                }
                                            },
                                            tpl: Ext.create('Ext.XTemplate',
                                                '<tpl for=".">',
                                                '<div class="x-boundlist-item" >{x_CountryName} ({OriginName}) {x_StateName}</div>',
                                                '</tpl>')
                                        }, {
                                            width: '10%',
                                            margin: '18 0 0 20',
                                            xtype: 'checkbox',
                                            name: 'POUseOriginAddress',
                                            labelSeparator: '',
                                            hideLabel: true,
                                            listeners: {
                                                render: function(field) {
                                                    Ext.tip.QuickTipManager.register({
                                                        target: field.getEl().id,
                                                        text: 'Use Origin Address',
                                                        showDelay: 100
                                                    });
                                                }
                                            }
                                        }]
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        name: 'POFreightDestination',
                                        fieldLabel: 'Freight Destination',
                                        listConfig: {
                                            minWidth: null
                                        },
                                        valueField: 'DestinationKey',
                                        displayField: 'Destination',
                                        store: storeFreightDestinations,
                                        queryMode: 'local',
                                        typeAhead: false,
                                        minChars: 2,
                                        allowBlank: false,
                                        forceSelection: true,
                                        listeners: {
                                            beforequery: function(record) {
                                                record.query = new RegExp(record.query, 'i');
                                                record.forceAll = true;
                                            }
                                        }
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        name: 'POCustShipKey',
                                        fieldLabel: 'Cust Ship Address',
                                        anchor: '100%',
                                        valueField: 'ShipKey',
                                        displayField: 'x_ShipAddress',
                                        store: storeCustomerShipAddress,
                                        queryMode: 'local',
                                        minChars: 2,
                                        allowBlank: false,
                                        forceSelection: false,
                                        queryCaching: false,
                                        listeners: {
                                            beforequery: function(record) {
                                                record.query = new RegExp(record.query, 'i');
                                                record.forceAll = true;
                                            },
                                            select: function(combo, records, eOpts) {
                                                var me = combo.up('form'),
                                                    record = records[0];
                                                me.down('field[name=POFreightDestinationZip]').setValue(record.data.ShipZip);
                                            }
                                        }
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        name: 'POWarehouseKey',
                                        fieldLabel: 'Warehouse Key',
                                        valueField: 'WarehouseKey',
                                        displayField: 'CarrierWarehouse',
                                        store: storeWarehouseType,
                                        queryMode: 'local',
                                        typeAhead: false,
                                        minChars: 2,
                                        forceSelection: false,
                                        beforequery: function(record) {
                                            record.query = new RegExp(record.query, 'i');
                                            record.forceAll = true;
                                        }
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        name: 'POFreightDestinationZip',
                                        fieldLabel: 'Destination Zip'

                                    }]
                                },
                                // Delimited
                                {
                                    xtype: 'container',
                                    columnWidth: 0.5,
                                    layout: 'anchor',
                                    items: [{
                                        xtype: 'component',
                                        flex: 1
                                    }]
                                },
                                // Invoice
                                {
                                    xtype: 'fieldset',
                                    layout: 'column',
                                    columnWidth: 0.5,
                                    margin: '10 0 0 5',
                                    padding: '0 10 10 10',
                                    items: [{
                                        xtype: 'combo',
                                        name: 'POInvoiceKey',
                                        fieldLabel: 'Invoice',
                                        columnWidth: 1,
                                        valueField: 'InvoiceKey',
                                        displayField: 'x_InvoiceNum',
                                        queryMode: 'remote',
                                        autoSelect: false,
                                        minChars: 2,
                                        allowBlank: true,
                                        triggerAction: '',
                                        forceSelection: false,
                                        queryCaching: false, // set for after add a new customer, this control recognize the new customer added
                                        emptyText: 'Choose Invoice',
                                        pageSize: 11,
                                        store: null
                                    }]
                                }
                            ]
                        },
                        // Panel Items
                        {
                            xtype: 'panel',
                            title: 'Items',
                            itemId: 'panelItems',
                            pageSize: 12,
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridPOItems',
                                minHeight: 410,
                                store: storePOItems,
                                maxHeight: 410,
                                features: [{
                                    ftype: 'summary'
                                }],
                                columns: [{
                                    xtype: 'gridcolumn',
                                    text: 'Sort',
                                    dataIndex: 'POItemsSort',
                                    format: '00,000'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Qty.',
                                    dataIndex: 'POItemsQty',
                                    format: '00,000.00'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Item Number',
                                    dataIndex: 'x_ItemNum'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Description',
                                    dataIndex: 'x_ItemName',
                                    flex: 1,
                                    summaryType: 'count',
                                    summaryRenderer: function(value, summaryData, dataIndex) {
                                        return Ext.String.format('Total for {0} items', value, value !== 1 ? value : 0);
                                    }
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Cost',
                                    dataIndex: 'POItemsLineCost',
                                    align: 'right',
                                    format: '00,000.00',
                                    summaryType: 'sum'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Price',
                                    dataIndex: 'POItemsLinePrice',
                                    align: 'right',
                                    format: '00,000.00',
                                    summaryType: 'sum'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'CUR',
                                    dataIndex: 'POItemsCurrencyCode'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Backorder Qty.',
                                    dataIndex: 'POItemsBackorderQty',
                                    format: '00,000.00',
                                    align: 'right',
                                    editor: {
                                        xtype: 'numericfield',
                                        name: 'POItemsBackorderQty',
                                        fieldStyle: 'text-align: right;',
                                        hideTrigger: false,
                                        useThousandSeparator: true,
                                        decimalPrecision: 0,
                                        alwaysDisplayDecimals: false,
                                        allowNegative: false,
                                        alwaysDecimals: false,
                                        thousandSeparator: ','
                                    }
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    items: [{
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) {
                                            return 'xf00e@FontAwesome';
                                        },
                                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                            var me = this.up('form'),
                                                curRec = me.down('formtoolbar').getCurrentRecord();

                                            var form = Ext.widget('jobpoedititems', {
                                                currentRecord: record,
                                                VendorKey: curRec.data.POVendorKey
                                            });
                                            form.modal = true;
                                            form.loadRecord(record);
                                            form.callerForm = this.up('form');
                                            form.show();
                                        },
                                        //iconCls: 'app-grid-edit',
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
                                                currentPO = toolbar.getCurrentRecord().data,
                                                totalItems = toolbar.store.getCount();


                                            var record = Ext.create('CBH.model.jobs.JobPurchaseOrderItems', {
                                                POItemsPOKey: currentPO.POKey,
                                                POItemsVendorKey: currentPO.POVendorKey,
                                                POItemsCurrencyCode: currentPO.POCurrencyCode,
                                                POItemsCurrencyRate: currentPO.POCurrencyRate,
                                                POItemsSort: (totalItems) * 100,
                                                POItemsJobKey: currentPO.POJobKey
                                            });


                                            var form = Ext.widget('jobpoedititems', {
                                                currentRecord: record,
                                                VendorKey: currentPO.POVendorKey
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
                                    }, {
                                        itemId: 'splibackorder',
                                        text: 'Split Backorder',
                                        handler: function() {
                                            var me = this.up('form');
                                            me.onSplitBackorderClick();
                                        }
                                    }
                                ],
                                bbar: new Ext.PagingToolbar({
                                    itemId: 'itemspagingtoolbar',
                                    store: storePOItems,
                                    displayInfo: true,
                                    displayMsg: 'Displaying records {0} - {1} of {2}',
                                    emptyMsg: "No records to display"
                                }),
                                selType: 'cellmodel',
                                plugins: [rowEditingItems],
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deleteitem').setDisabled(!records.length);
                                    },
                                    viewready: function(grid) {
                                        var view = grid.view;

                                        // record the current cellIndex
                                        grid.mon(view, {
                                            uievent: function(type, view, cell, recordIndex, cellIndex, e) {
                                                grid.cellIndex = cellIndex;
                                                grid.recordIndex = recordIndex;
                                            }
                                        });

                                        grid.tip = Ext.create('Ext.tip.ToolTip', {
                                            target: view.el,
                                            delegate: '.x-grid-cell',
                                            trackMouse: true,
                                            renderTo: Ext.getBody(),
                                            listeners: {
                                                beforeshow: function updateTipBody(tip) {
                                                    if (!Ext.isEmpty(grid.cellIndex) && grid.cellIndex !== -1) {
                                                        header = grid.headerCt.getGridColumns()[grid.cellIndex];
                                                        if (header.dataIndex === "POItemsBackorderQty") {
                                                            tip.update("click for split backorder");
                                                        } else {
                                                            tip.update(grid.getStore().getAt(grid.recordIndex).get(header.dataIndex));
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                            }]
                        },
                        // Panel Charges
                        {
                            xtype: 'panel',
                            title: 'Charges',
                            itemId: 'panelCharges',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridPOCharges',
                                minHeight: 350,
                                store: storePOCharges,
                                maxHeight: 400,
                                features: [{
                                    ftype: 'summary'
                                }],
                                columns: [{
                                    xtype: 'gridcolumn',
                                    text: 'Sort',
                                    dataIndex: 'POChargesSort',
                                    format: '00,000'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Qty.',
                                    dataIndex: 'POChargesQty',
                                    format: '00,000.00'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Description',
                                    dataIndex: 'x_DescriptionText',
                                    flex: 1
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Notes & Comments',
                                    dataIndex: 'POChargesMemo',
                                    flex: 1,
                                    summaryType: 'count',
                                    summaryRenderer: function(value, summaryData, dataIndex) {
                                        return Ext.String.format('Total for {0} charges', value, value !== 1 ? value : 0);
                                    }
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Cost Unit',
                                    dataIndex: 'x_UnitCost',
                                    align: 'right',
                                    format: '00,000.00'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Price Unit',
                                    dataIndex: 'x_UnitPrice',
                                    align: 'right',
                                    format: '00,000.00'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Total Cost',
                                    dataIndex: 'POChargesCost',
                                    align: 'right',
                                    format: '00,000.00',
                                    summaryType: 'sum'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Total Price',
                                    dataIndex: 'POChargesPrice',
                                    align: 'right',
                                    format: '00,000.00',
                                    summaryType: 'sum'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'CUR',
                                    dataIndex: 'POChargesCurrencyCode'
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    items: [{
                                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                            var form = Ext.widget('jobpoeditcharges', {
                                                ItemKey: record.data.POItemsItemKey
                                            });
                                            form.modal = true;
                                            form.loadRecord(record);
                                            form.callerForm = this.up('form');
                                            form.show();
                                        },
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) {
                                            return 'xf00e@FontAwesome';
                                        },
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
                                            currentPO = toolbar.getCurrentRecord().data,
                                            totalCharges = toolbar.store.getCount();

                                        var form = Ext.widget('jobpoeditcharges');
                                        record = Ext.create('CBH.model.jobs.JobPurchaseOrderCharges', {
                                            POChargesPOKey: currentPO.POKey,
                                            POChargesJobKey: currentPO.POJobKey,
                                            POChargesCurrencyCode: 'USD',
                                            POChargesCurrencyRate: 1
                                        });

                                        form.loadRecord(record);
                                        form.center();
                                        form.callerForm = this.up('form');
                                        form.show();
                                    },
                                    disabled: true
                                }, {
                                    itemId: 'deletecharge',
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
                                }],
                                bbar: new Ext.PagingToolbar({
                                    itemId: 'pochargespagingtoolbar',
                                    store: storePOCharges,
                                    displayInfo: true,
                                    displayMsg: 'Displaying records {0} - {1} of {2}',
                                    emptyMsg: "No records to display"
                                }),
                                selType: 'rowmodel',
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deletecharge').setDisabled(!records.length);
                                    },
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var form = Ext.widget('jobpoeditcharges');
                                        form.modal = true;
                                        form.loadRecord(record);
                                        form.callerForm = this.up('form');
                                        form.show();
                                    }
                                }
                            }]
                        },
                        // Panel Instructions
                        {
                            xtype: 'panel',
                            title: 'Notes',
                            itemId: 'panelInstructions',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridInstructions',
                                minHeight: 350,
                                store: storeInstructions,
                                maxHeight: 400,
                                columns: [{
                                    xtype: 'gridcolumn',
                                    text: 'Sort',
                                    dataIndex: 'POInstructionsStep',
                                    format: '00,000'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Instructions',
                                    dataIndex: 'x_ITextMemo',
                                    flex: 1
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Notes & Comments',
                                    dataIndex: 'POInstructionsMemo',
                                    flex: 1
                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    text: 'Note Font Color',
                                    dataIndex: 'x_NotesFontColor'
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    items: [{
                                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                            var form = Ext.widget('jobpoeditinstruction');
                                            form.modal = true;
                                            form.loadRecord(record);
                                            form.callerForm = this.up('form');
                                            form.show();
                                        },
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) {
                                            return 'xf00e@FontAwesome';
                                        },
                                        tootip: 'view details'
                                    }]
                                }],
                                tbar: [{
                                    xtype: 'component',
                                    flex: 1
                                }, {
                                    text: 'Add',
                                    itemId: 'addinstruction',
                                    handler: function() {
                                        currentRecord = this.up('form').down("#FormToolbar").getCurrentRecord();
                                        var pokey = currentRecord.data.POKey;
                                        var form = Ext.widget('jobpoeditinstruction');
                                        record = Ext.create('CBH.model.jobs.JobPurchaseOrderInstructions', {
                                            POInstructionsPOKey: pokey
                                        });
                                        form.loadRecord(record);
                                        form.center();
                                        form.callerForm = this.up('form');
                                        form.show();
                                    },
                                    disabled: true
                                }, {
                                    itemId: 'deleteinstruction',
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
                                }],
                                bbar: new Ext.PagingToolbar({
                                    itemId: 'instructionspagingtoolbar',
                                    store: storeInstructions,
                                    displayInfo: true,
                                    displayMsg: 'Displaying records {0} - {1} of {2}',
                                    emptyMsg: "No records to display"
                                }),
                                selType: 'rowmodel',
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deleteinstruction').setDisabled(!records.length);
                                    },
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var form = Ext.widget('jobpoeditinstruction');
                                        form.modal = true;
                                        form.loadRecord(record);
                                        form.callerForm = this.up('form');
                                        form.show();
                                    }
                                }
                            }]
                        }
                    ]
                },
                // Delimited 2
                {
                    xtype: 'container',
                    columnWidth: 0.5,
                    layout: 'anchor',
                    items: [{
                        xtype: 'component',
                        flex: 1
                    }]
                },
                // Currency Rate General
                {
                    xtype: 'fieldset',
                    layout: 'column',
                    columnWidth: 0.5,
                    margin: '10 0 0 5',
                    padding: '0 10 10 10',
                    items: [{
                        xtype: 'combo',
                        name: 'x_POCurrencyCode',
                        fieldLabel: 'Currency',
                        formBind: false,
                        store: storeCurrencyRatesHeader,
                        labelWidth: 50,
                        listConfig: {
                            minWidth: null
                        },
                        columnWidth: 0.5,
                        valueField: 'CurrencyCode',
                        displayField: 'CurrencyCodeDesc',
                        queryMode: 'local',
                        typeAhead: false,
                        autoSelect: false,
                        minChars: 2,
                        allowBlank: true,
                        forceSelection: true,
                        listeners: {
                            select: function(field, records, eOpts) {
                                if (records.length > 0) {
                                    var me = this.up('panel');
                                    field.next().setValue(records[0].data.CurrencyRate);
                                    me.onSelectCurrencyCode(records[0]);
                                }
                            },
                            change: function(field, newValue, oldValue) {
                                if (newValue === null) {
                                    field.next().setValue(0);
                                }
                            },
                            blur: function(field, The, eOpts) {
                                if (field.value !== null) {
                                    var me = this.up('panel');
                                    copyToField = field.valueModels[0].data.CurrencyRate;
                                    copyField = me.down('field[name=POCurrencyRate]');
                                    copyField.setValue(copyToField);
                                }
                            },
                            beforequery: function(record) {
                                record.query = new RegExp(record.query, 'i');
                                record.forceAll = true;
                            }
                        },
                        tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                            '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                            '</tpl>')
                    }, {
                        xtype: 'numericfield',
                        name: 'POCurrencyRate',
                        columnWidth: 0.5,
                        margin: '0 0 0 10',
                        hideTrigger: true,
                        useThousandSeparator: true,
                        decimalPrecision: 3,
                        alwaysDisplayDecimals: true,
                        allowNegative: false,
                        currencySymbol: '$',
                        alwaysDecimals: true,
                        thousandSeparator: ',',
                        fieldLabel: 'Rate',
                        labelAlign: 'top',
                        fieldStyle: 'text-align: right;',
                        allowBlank: false,
                        formBind: false,
                        editable: false,
                        disabled: true
                    }]
                }
            ],
            dockedItems: [{
                xtype: 'formtoolbar',
                itemId: 'FormToolbar',
                dock: 'top',
                store: me.storeNavigator,
                navigationEnabled: true,
                addEnabled: true,
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
                    name: 'POCreatedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created By'
                }, {
                    xtype: 'datefield',
                    name: 'POCreatedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created Date',
                    hideTrigger: true
                }, {
                    xtype: 'textfield',
                    name: 'POModifiedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified By'
                }, {
                    xtype: 'datefield',
                    name: 'POModifiedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true
                }, {
                    xtype: 'datefield',
                    name: 'POSubmittedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Sent Date',
                    hideTrigger: true
                }, {
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    itemId: 'exportPeachtreebutton',
                    text: 'Export PO to Peachtree',
                    handler: me.onClickExportPOToPeachtree
                }, {
                    xtype: 'button',
                    itemId: 'gotHistorybutton',
                    text: 'Go to Status History',
                    handler: me.onClickEditStatusHistory
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
                    if (toolbar.isEditing) {
                        var btn = toolbar.down('#save');
                        btn.fireEvent('click');
                    }
                }
            },
            this);
    },

    onRenderForm: function() {
        /*var currencyId = me.down('field[name=x_POCurrencyCode]').getId();
        var currencyEl = Ext.select("#" + currencyId + "-inputEl").elements[0];
        currencyEl.removeAttribute('readonly');
        //Ext.select("#" + currencyId + "-inputEl").set({readonly:null}, false);*/


    },

    onAfterLoadRecord: function(tool, record) {
        var me = this,
            toolbar = me.down("#FormToolbar"),
            btn = toolbar.down("#save"),
            jobkey = record.data.JobKey,
            custkey = record.data.JobCustKey,
            vendorkey = record.data.POVendorKey,
            curRec = record.data;

        var storeOriginAddress = new CBH.store.vendors.VendorOriginAddress().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function(records, eOpts, success) {
                if (success && me && me.down("field[name=POVendorOriginAddress]")) {
                    me.down("field[name=POVendorOriginAddress]").bindStore(this);
                    me.down("field[name=POVendorOriginAddress]").setValue(curRec.POVendorOriginAddress);
                }

                // me.storesLoaded.push("VendorOriginAddress");
                // me.checkLoaded();
            }
        });

        if (record.phantom) {
            me.down('#panelItems').tab.hide();
            me.down('#panelCharges').tab.hide();
            me.down('#panelInstructions').tab.hide();
            me.down('#exportPeachtreebutton').setDisabled(true);
            me.down('#gotHistorybutton').setDisabled(true);

            btn.setTooltip('Save the general data previously to load the items');

            setTimeout(function() {
                Ext.Msg.alert('Remember', 'Save the general data previously to load the items');
            }, 1500);

            return;
        } else {
            btn.setTooltip('Save');
        }

        me.down('#exportPeachtreebutton').setDisabled(false);
        me.down('#gotHistorybutton').setDisabled(false);

        me.down('#panelItems').tab.show();
        me.down('#panelCharges').tab.show();
        me.down('#panelInstructions').tab.show();

        me.down('#addinstruction').setDisabled(false);
        me.down('#additem').setDisabled(false);
        me.down('#addcharge').setDisabled(false);

        me.down('#panelItems').setDisabled(false);
        me.down('#panelCharges').setDisabled(false);
        me.down('#panelInstructions').setDisabled(false);

        me.storeToLoad = 7;
        me.storesLoaded = [];

        Ext.Msg.wait('Loading data...', 'Wait');

        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                id: vendorkey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function() {
                var fieldVendor = me.down("field[name=POVendorKey]");
                fieldVendor.getStore().removeAll();
                fieldVendor.bindStore(storeVendors);

                if (this.totalCount > 0 && String.isNullOrEmpty(fieldVendor.getValue())) {
                    var record = this.getAt(0);
                    fieldVendor.setValue(record.data.VendorKey);
                    me.down('field[name=x_VendorAddress]').setValue(record.data.x_VendorAddress);
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
                var fieldContact = me.down("field[name=POVendorContactKey]");
                fieldContact.bindStore(storeVendorContacts);
                fieldContact.setValue(curRec.POVendorContactKey);

                me.storesLoaded.push("VendorContacts");
                me.checkLoaded();
            }
        });

        var storeCustomerShipAddress = new CBH.store.customers.CustomerShipAddress().load({
            params: {
                id: curRec.POCustShipKey,
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                me.down('field[name=POCustShipKey]').bindStore(this);
                me.down('field[name=POCustShipKey]').setValue(curRec.POCustShipKey);

                me.storesLoaded.push("CustomerShipAddress");
                me.checkLoaded();
            }
        });

        var storePOItems = new CBH.store.jobs.JobPurchaseOrderItems().load({
            params: {
                POItemsPOKey: curRec.POKey
            },
            callback: function() {
                var grid = me.down("#gridPOItems");
                grid.reconfigure(storePOItems);
                me.down('#itemspagingtoolbar').bindStore(storePOItems);

                me.storesLoaded.push("JobPurchaseOrderItems");
                me.checkLoaded();
            }
        });

        var storePOCharges = new CBH.store.jobs.JobPurchaseOrderCharges().load({
            params: {
                POChargesPOKey: curRec.POKey
            },
            callback: function() {
                var grid = me.down("#gridPOCharges");
                grid.reconfigure(storePOCharges);
                me.down('#pochargespagingtoolbar').bindStore(storePOCharges);

                me.storesLoaded.push("JobPurchaseOrderCharges");
                me.checkLoaded();
            }
        });

        var storeInstructions = new CBH.store.jobs.JobPurchaseOrderInstructions().load({
            params: {
                POInstructionsPOKey: curRec.POKey
            },
            callback: function() {
                var grid = me.down("#gridInstructions");
                grid.reconfigure(storeInstructions);
                me.down("#instructionspagingtoolbar").bindStore(storeInstructions);

                me.storesLoaded.push("JobPurchaseOrderInstructions");
                me.checkLoaded();
            }
        });

        var storeInvoices = new CBH.store.jobs.InvoiceDDL().load({
            params: {
                page: 1,
                start: 0,
                limit: 8,
                JobKey: curRec.POJobKey
            },
            callback: function(records, success, eOpts) {
                me.down('field[name=POInvoiceKey]').bindStore(storeInvoices);
                me.down('field[name=POInvoiceKey]').setValue(curRec.POInvoiceKey);

                me.storesLoaded.push(storeInvoices.getProxy().getModel().modelName);
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
        var me = this;

        record.data.POJobKey = me.JobKey;
        record.data.POCurrencyCode = me.currentJob.data.CustCurrencyCode;
        record.data.POCurrencyRate = me.currentJob.data.CustCurrencyRate;
        record.data.POWarehouseKey = !me.currentJob.data.JobWarehouseKey ? null : me.currentJob.data.JobWarehouseKey;
        record.data.PODefaultProfitMargin = 0.15;
        record.data.POCustShipKey = me.currentJob.data.JobCustShipKey;
        record.data.PODate = new Date();
        record.data.POGoodThruDate = Ext.Date.add(new Date(), Ext.Date.DAY, 1);
        record.data.POVendorPaymentTerms = 5;

        me.down('#panelItems').setDisabled(true);
        me.down('#panelCharges').setDisabled(true);
        me.down('#panelInstructions').setDisabled(true);

        me.down('field[name=x_VendorAddress]').setValue('');

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

        if (savedRecord.data.JobKey !== 0) {
            savedRecord.set('JobModifiedBy', CBH.GlobalSettings.getCurrentUserName());
        }

        Ext.Msg.wait('Saving Record...', 'Wait');

        savedRecord.save({
            callback: function(records, operation, success) {
                if (success) {
                    Ext.Msg.hide();
                    toolbar.doRefresh(records[0]);
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

            Ext.Msg.show({
                title: 'Delete',
                msg: 'Do you want to delete?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(btn) {
                    if (btn === "yes") {
                        Ext.Msg.wait('Deleting Record...', 'Wait');
                        record.destroy({
                            success: function() {
                                var lastOpt = toolbar.store.lastOptions,
                                    params = (lastOpt) ? lastOpt.params : null;

                                toolbar.store.reload({
                                    params: params,
                                    callback: function() {
                                        if (toolbar.store.getCount() > 0) {
                                            toolbar.gotoAt(Math.max(prevRec, 1));
                                        } else {
                                            toolbar.up('form').up('panel').destroy();
                                        }
                                        Ext.Msg.hide();
                                    }
                                });
                            },
                            failure: function() {
                                Ext.Msg.hide();
                            }
                        });
                    }
                }
            }).defaultButton = 2;
        }
    },

    onDestroy: function(e, eOpts) {
        //...
    },

    // Event Blur CustKey Field
    onVendorBlur: function(field, The, eOpts) {
        var me = field.up('form');

        if (field.value !== null && field.valueModels[0]) {
            var currentVendor = field.value,
                fieldContact = me.down('field[name=POVendorContactKey]'),
                fieldOriginAddress = me.down('field[name=POVendorOriginAddress]');

            me.down('field[name=x_VendorAddress]').setValue(field.valueModels[0].data.x_VendorAddress);

            me.getEl().mask('Loading vendors\'s contacts...');
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

                    me.getEl().unmask();
                }
            });
        }
    },

    onContactBlur: function(field, The, eOpts) {
        var form = field.up('form');

        if (field.value !== null && field.valueModels[0]) {

        } else if (field.getValue() && form.down('field[name=POVendorKey]').value !== null) {
            Ext.Msg.show({
                title: 'Question',
                msg: 'The contact doesn\'t exists, Do you want to add to database?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(e) {
                    if (e === "yes") {
                        form.addContact(field);
                    } else {
                        this.setValue(null);
                        this.focus(true, 200);
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
            vendorkey = me.down('field[name=POVendorKey]').getValue();

        record = Ext.create('CBH.model.vendors.VendorContacts', {
            ContactVendorKey: vendorkey,
            ContactFirstName: firstName,
            ContactLastName: lastName
        });

        var form = Ext.create('CBH.view.vendors.VendorContacts');
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
            gridItems = me.down('#gridPOItems'),
            storeItems = gridItems.getStore();


        // Check if items has backorder split
        var totalQty = storeItems.sum('POItemsBackorderQty');

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
                gridItems.store.reload();
                Ext.Msg.hide();
            }
        });
    },

    onSelectCurrencyCode: function(record) {
        var me = this,
            curRec = me.down('formtoolbar').getCurrentRecord();

        Ext.Msg.wait('Please wait....', 'Wait');

        Ext.Ajax.request({
            method: 'GET',
            type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/ChangePOCurrency',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                POKey: curRec.data.POKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName,
                CurrencyCode: record.data.CurrencyCode,
                CurrencyRate: record.data.CurrencyRate
            },
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                me.down("field[name=POCurrencyCode]").setValue(record.data.CurrencyCode);
                me.down("field[name=POCurrencyRate]").setValue(record.data.CurrencyRate);
                Ext.Msg.hide();
            }
        });
    },

    // plugins
    loadPluginItems: function() {
        var me = this;

        /*return new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 100,
                    fn: function(item, e) {
                        var editor = this.getEditor();
                        editor.down('field[name=POItemsBackorderQty]').focus(true, 200);
                        this.getEditor().onFieldChange();
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('#gridPOItems');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('#gridPOItems'),
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
        });*/

        return new Ext.grid.plugin.CellEditing({
            clicksToEdit: 1,
            listeners: {
                edit: {
                    fn: function(editor, context) {
                        var grid = context.grid,
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

    onClickExportPOToPeachtree: function() {
        var me = this.up('form'),
            today = new Date(),
            toolbar = me.down("#FormToolbar"),
            record = toolbar.getCurrentRecord(),
            POKey = record.data.POKey,
            t = Ext.util.Cookies.get('CBH.UserAuth');

        var url = "{0}/ExportPurchaseOrderToPeachTree?_dc={1}&POKey={2}&t={3}".format(CBH.GlobalSettings.webApiPath, today.getTime(), POKey, t);
        window.open(url, '_blank');
    },

    onClickEditStatusHistory: function() {
        var me = this.up("form"),
            toolbar = me.down("#FormToolbar");

        var tabs = this.up('app_pageframe');

        var selection = toolbar.getCurrentRecord();

        var POKey = selection.data.POKey;
        var JobKey = selection.data.POJobKey;
        var JobNum = me.down('field[name=JobNumFormatted]').getValue();
        var customer = me.currentJob.data.Customer;
        var vendor = me.down('field[name=POVendorKey]').getRawValue();
        var PONum = me.down('field[name=x_PONumFormatted]').getValue();

        var storeqfrmJobPurchaseOrderStatusHistory = new CBH.store.jobs.qfrmJobPurchaseOrderStatusHistory().load({
            params: {
                id: POKey
            },
            callback: function(records, success, eOpts) {

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Job Purchase Order Status History',
                    items: [{
                        xtype: 'container',
                        layout: {
                            type: 'anchor'
                        },
                        items: [
                            Ext.widget('JobPurchaseOrderStatusHistoryList', {
                                JobKey: JobKey,
                                JobNum: JobNum,
                                Customer: customer,
                                Vendor: vendor,
                                JobStatus: null,
                                POKey: POKey,
                                PONum: PONum,
                                POShipETA: records[0].data.POShipETA,
                                currentRecord: records[0]
                            })
                        ]
                    }],
                    listeners: {
                        activate: function() {
                            var grid = this.down('form').down('gridpanel'),
                                lastOptions = grid.store.lastOptions,
                                lastParams = (lastOptions) ? lastOptions.params : null;
                            if (lastParams)
                                grid.store.reload({
                                    params: lastParams
                                });
                        }
                    }
                });

                tabs.setActiveTab(tab.getId());
            }
        });
    },

    onPrintRecord: function(toolbar, record) {
        var userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey();

        var formData = {
            id: record.get('POKey'),
            employeeKey: userKey
        };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptJobPurchaseOrder", params);
        window.open(pathReport, 'CBH - Purchase Order', false);
    }
});