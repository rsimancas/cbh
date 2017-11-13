Ext.define('CBH.view.sales.FileOrderEntry', {
    extend: 'Ext.form.Panel',
    alias: 'widget.fileorderentry',

    autoShow: true,
    autoRender: true,
    //autoScroll: true,
    header: false,
    title: 'Order Entry',
    forceFit: true,
    layout: 'auto',
    FileKey: 0,
    FileNum: "",
    Customer: "",
    CustKey: 0,
    bodyPadding: 20,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        var storeQuotes = new CBH.store.sales.FileQuoteDetail({
            remoteSort: false
        }).load({
            params: {
                filekey: me.FileKey
            }
        });
        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            synchronous: true
        });
        var storeItems = null;
        var vendormodel = null;
        var storeLastQuoteMargin = null;
        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
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
            items: [{
                xtype: 'fieldset',
                //columnWidth: 1,
                layout: {
                    type: 'hbox'
                },
                items: [{
                    flex: 1,
                    xtype: 'textfield',
                    itemId: 'customer',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'Customer',
                    readOnly: true,
                    margin: '0 0 10 0',
                    value: me.Customer
                }, {
                    xtype: 'textfield',
                    itemId: 'custcode',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'Customer Code',
                    readOnly: true,
                    margin: '0 0 10 5',
                    value: me.CustKey
                }, {
                    xtype: 'textfield',
                    itemId: 'filenum',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'Reference Num',
                    readOnly: true,
                    margin: '0 0 10 5',
                    value: me.FileNum
                }, {
                    xtype: 'hiddenfield',
                    name: 'FileDefaultCurrencySymbol'
                }]
            }, {
                xtype: 'gridpanel',
                itemId: 'gridorderentry',
                //columnWidth: 1,
                storeLastQuoteMargin: null,
                store: storeQuotes,
                scrollable: false,
                maxHeight: parseInt(screen.height * 0.6),
                minHeight: parseInt(screen.height * 0.6),
                features: [{
                    ftype: 'summary'
                }],
                columns: [{
                    xtype: 'numbercolumn',
                    width: 65,
                    dataIndex: 'QuoteSort',
                    text: 'Sort',
                    format: '00,000',
                    sortable: true
                }, {
                    xtype: 'numbercolumn',
                    width: 65,
                    dataIndex: 'QuoteQty',
                    text: 'Qty.',
                    align: 'right',
                    format: '00,000',
                    sortable: true
                }, {
                    xtype: 'gridcolumn',
                    flex: 1,
                    dataIndex: 'x_VendorName',
                    text: 'Vendor',
                    sortable: true
                }, {
                    xtype: 'gridcolumn',
                    width: 80,
                    dataIndex: 'x_ItemNum',
                    text: 'Item Num.',
                    sortable: true
                }, {
                    xtype: 'gridcolumn',
                    flex: 1,
                    dataIndex: 'x_ItemName',
                    text: 'Description',
                    totalsText: 'First column is sum, second max',
                    summaryType: 'count',
                    summaryRenderer: function(value, summaryData, dataIndex) {
                        return Ext.String.format('Total for {0} items', value, value !== 1 ? value : 0);
                    },
                    sortable: true
                }, {
                    xtype: 'numbercolumn',
                    width: 70,
                    text: 'Profit',
                    align: 'right',
                    dataIndex: 'x_ProfitMargin',
                    format: '00,000.00',
                    sortable: true
                }, {
                    xtype: 'gridcolumn',
                    width: 120,
                    text: 'Total Cost',
                    align: 'right',
                    dataIndex: 'QuoteItemLineCost',
                    renderer: function(value, metaData, record) {
                        var returnValue = Ext.util.Format.usMoney(value),
                            currencySymbol = record.data.QuoteItemCurrencySymbol;
                        return returnValue.replace('$', currencySymbol);
                    },
                    summaryType: 'sum',
                    summaryRenderer: function(value, metaData, dataIndex) {
                        var currencySymbol = me.down('field[name=FileDefaultCurrencySymbol]').getValue();
                        currencySymbol = !String.isNullOrEmpty(currencySymbol) ? currencySymbol : "$";
                        var returnValue = Ext.util.Format.usMoney(value);
                        return returnValue.replace('$', currencySymbol);
                    },
                    sortable: true
                }, {
                    xtype: 'gridcolumn',
                    width: 120,
                    text: 'Total Price',
                    align: 'right',
                    dataIndex: 'QuoteItemLinePrice',
                    renderer: function(value, metaData, record) {
                        var returnValue = Ext.util.Format.usMoney(value),
                            currencySymbol = record.data.QuoteItemCurrencySymbol;
                        return returnValue.replace('$', currencySymbol);
                    },
                    summaryType: 'sum',
                    summaryRenderer: function(value, metaData, dataIndex) {
                        var currencySymbol = me.down('field[name=FileDefaultCurrencySymbol]').getValue();
                        currencySymbol = !String.isNullOrEmpty(currencySymbol) ? currencySymbol : "$";
                        var returnValue = Ext.util.Format.usMoney(value);
                        return returnValue.replace('$', currencySymbol);
                    },
                    sortable: true
                }, {
                    xtype: 'gridcolumn',
                    width: 70,
                    text: 'Currency',
                    align: 'right',
                    dataIndex: 'QuoteItemCurrencyCode',
                    sortable: true
                }, {
                    xtype: 'actioncolumn',
                    draggable: false,
                    width: 25,
                    resizable: false,
                    hideable: false,
                    stopSelection: false,
                    items: [{
                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                            var tabs = this.up('app_pageframe');

                            var storeToNavigate = new CBH.store.sales.FileQuoteDetail().load({
                                //params: {filekey: record.data.QuoteFileKey},
                                params: {
                                    id: record.data.QuoteKey
                                },
                                callback: function() {
                                    var form = Ext.widget('filelineentry', {
                                        storeNavigator: this,
                                        VendorKey: record.data.QuoteVendorKey,
                                        ItemKey: record.data.QuoteItemKey,
                                        FileKey: me.FileKey,
                                        FileNum: me.FileNum
                                    });

                                    var tab = tabs.add({
                                        closable: true,
                                        iconCls: 'tabs',
                                        autoScroll: true,
                                        title: 'Line Entry',
                                        items: [form]
                                    });

                                    form.down('#FormToolbar').gotoAt(1);

                                    tab.show();
                                }
                            });

                        },
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                        tootip: 'view line detail'
                    }]
                }],
                tbar: [{
                    text: 'Add',
                    handler: function() {
                        var me = this.up('form'),
                            tabs = me.up('app_pageframe'),
                            grid = me.down('gridpanel'),
                            rowCount = grid.store.max("QuoteSort");

                        if(!rowCount)
                            rowCount = 0;

                        var model = Ext.create('CBH.model.sales.FileQuoteDetail', {
                            QuoteFileKey: me.FileKey,
                            x_FileNum: me.FileNum,
                            QuoteSort: parseFloat(rowCount) + 100
                        });

                        var storeToNavigate = new CBH.store.sales.FileQuoteDetail();

                        var form = Ext.widget('filelineentry', {
                            storeNavigator: storeToNavigate,
                            FileKey: me.FileKey,
                            FileNum: me.FileNum
                        });

                        var tab = tabs.add({
                            closable: true,
                            iconCls: 'tabs',
                            autoScroll: true,
                            title: 'New Line Entry',
                            items: [form]
                        });

                        tab.show();

                        var btn = form.down('#FormToolbar').down('#add');
                        btn.fireEvent('click', btn, null, null, model);
                    }
                }, {
                    itemId: 'deleteline',
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
                                        selection[0].destroy();
                                    }
                                }
                            }).defaultButton = 2;
                        }
                    },
                    disabled: true
                }],
                selType: 'rowmodel',
                listeners: {
                    selectionchange: function(view, records) {
                        this.down('#deleteline').setDisabled(!records.length);
                    },
                    validateedit: {
                        fn: me.onValidateEdit,
                        scope: me
                    },
                    viewready: {
                        fn: me.onGridViewReady,
                        scope: me
                    },
                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                        var me = this.up('panel'),
                            tabs = this.up('app_pageframe'),
                            grid = view.up('gridpanel');

                        var header = grid.headerCt.getGridColumns()[cellIndex];

                        if(header.text === "Vendor") {
                            me.onViewVendor(record.data.QuoteVendorKey);
                        } else if(header.text === "Item Num.") {
                            me.onViewItem(record.data.QuoteItemKey);
                        } else {
                            var storeToNavigate = new CBH.store.sales.FileQuoteDetail().load({
                                params: {
                                    id: record.data.QuoteKey
                                },
                                callback: function() {
                                    var form = Ext.widget('filelineentry', {
                                        storeNavigator: this,
                                        VendorKey: record.data.QuoteVendorKey,
                                        ItemKey: record.data.QuoteItemKey
                                    });

                                    var tab = tabs.add({
                                        closable: true,
                                        iconCls: 'tabs',
                                        autoScroll: true,
                                        title: 'Line Entry',
                                        items: [form]
                                    });

                                    form.down('#FormToolbar').gotoAt(1);

                                    tab.show();
                                }
                            });
                        }
                    }
                }
            }, {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'textfield',
                    name: 'FileCreatedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created By',
                    width: 120
                }, {
                    xtype: 'datefield',
                    name: 'FileCreatedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created Date',
                    hideTrigger: true,
                    width: 80
                }, {
                    xtype: 'textfield',
                    name: 'FileModifiedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified By',
                    width: 120
                }, {
                    xtype: 'datefield',
                    name: 'FileModifiedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true,
                    width: 80,                }, {
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    text: 'Customer Information',
                    handler: function() {
                        var me = this.up("form");
                        me.onClickCustomerInformation();
                    }
                }, {
                    margin:'0 10 0 5',
                    xtype: 'button',
                    text: 'Update Currency Rates',
                    handler: function() {
                        var me = this.up("form");
                        me.onClickUpdateCurrencyRates();
                    }
                }, {
                    margin:'0 10 0 5',
                    xtype: 'button',
                    text: 'Import / Quick Add',
                    handler: function() {
                        var me = this.up('form');
                        me.onClickImportQuickAdd();
                    }
                }]
            }],
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                }
            }
        });

        //me.on('validateedit', );

        me.callParent(arguments);
    },

    refreshData: function() {
        var me = this;
        me.down("#gridorderentry").getStore().reload();
    },

    onValidateEdit: function(e) {
        var myTargetRow = 6;

        if (e.rowIdx == myTargetRow) {
            e.cancel = true;
            e.record.data[e.field] = e.value;
        }
    },

    onRenderForm: function() {
        var me = this;

        var storeFile = new CBH.store.sales.FileHeader().load({
            params: {
                id: me.FileKey
            },
            callback: function(records, operation, success) {
                if(success && records[0]) {
                    me.down('field[name=FileCreatedBy]').setValue(records[0].data.FileCreatedBy);
                    me.down('field[name=FileCreatedDate]').setValue(records[0].data.FileCreatedDate);
                    me.down('field[name=FileModifiedBy]').setValue(records[0].data.FileModifiedBy);
                    me.down('field[name=FileModifiedDate]').setValue(records[0].data.FileModifiedDate);
                    me.down('field[name=FileDefaultCurrencySymbol]').setValue(records[0].data.FileDefaultCurrencySymbol);
                }
            }
        });
    },

    onGridViewReady: function(grid) {

        // get reference to the summary row
        var summaryRow = grid.view.el.down('tr.x-grid-row-summary');

        // this will apply a css class to the row, in this example,
        // I am applying the extjs grid header style to my summary row
        summaryRow.addCls('x-grid-header-ct');

        // or, to do it all in javascript as you mentioned in the comment
        // first you would create a style object, I took these style
        // properties from the .x-grid-header-ct
        aStyleObject = {
            cursor: 'default',
            zoom: 1,
            padding: 0,
            border: '1px solid #d0d0d0',
            'border-bottom-color': '#c5c5c5',
            'background-image': 'none',
            'background-color': '#c5c5c5',
            'font-weight': 'bold'
        };

        // then you pass the style object using setStyle
        //summaryRow.setStyle(aStyleObject);

        // // or you could set the style for each cell individually using 
        // // addCls or setStyle:
        Ext.Array.each(summaryRow.query('td'), function(td) {
            var cell = Ext.get(td);
            //cell.addCls('some-class');
            // or
            cell.setStyle({
                'font-weight': 'bold',
                'color': 'green',
                'font-size': '12px'
            });
        });
    },

    onClickCustomerInformation: function() {
        var me = this;

        var tabs = this.up('app_pageframe');

        var storeToNavigate = new CBH.store.sales.FileHeader().load({
            params: {
                id: me.FileKey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function() {
                var form = Ext.widget('fileform', {
                    storeNavigator: storeToNavigate
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'File: ' + this.getAt(0).data.x_FileNumFormatted,
                    items: [form]
                });

                form.down('#FormToolbar').gotoAt(1);

                tab.show();
            }
        });
    },

    onClickUpdateCurrencyRates: function() {
        var me = this,
            grid = me.down("gridpanel");

        me.getEl().mask('Please wait...');

        Ext.Ajax.request({
            method: 'GET',
            type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/UpdateFileQuoteCurrencyRates',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                FileKey: me.FileKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName
            },
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                me.getEl().unmask();
                grid.store.reload();
                Ext.Msg.alert('Status', 'Updated successfully.');
            },
            failure: function(response, opts) {
                console.info('server-side failure with status code ' + response.status);
                me.getEl().unmask();
            }
        });
    },

    onClickImportQuickAdd: function() {
        var me = this;

        var form = Ext.widget('FileQuoteOrderEntryQuickAdd', {
            modal: true,
            width: 700,
            frameHeader: true,
            header: true,
            /*layout: {
                type: 'column'
            },*/
            title: 'Quick Add to File',
            bodyPadding: 10,
            closable: true,
            //constrain: true,
            stateful: false,
            floating: true,
            callerForm: me,
            forceFit: true,
            FileKey: me.FileKey
        });

        form.show();
    },

    refreshItemDetails: function() {
        var me = this;

        me.down('gridpanel').getStore().reload();
    },

    onViewVendor: function(VendorKey) {
        var me = this,
            tabs = me.up('app_pageframe');

        me.getEl().mask("Loading Vendor...");
        storeToNavigate = new CBH.store.vendors.Vendors().load({
            params: {
                id: VendorKey
            },
            callback: function(records) {
                var form = Ext.widget('vendors', {
                    storeNavigator: storeToNavigate,
                    callerForm: me
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Vendor Maintenance',
                    items: [form],
                });

                //This javascript replaces all 3 types of line breaks with a space
                if(records[0].data.VendorAddress1) 
                    records[0].data.VendorAddress1 = records[0].data.VendorAddress1.replace(/(\r\n|\n|\r)/gm," ");
                if(records[0].data.VendorAddress2)
                    records[0].data.VendorAddress2 = records[0].data.VendorAddress2.replace(/(\r\n|\n|\r)/gm," ");
                
                form.down('#FormToolbar').gotoAt(1);
                tab.show();
                me.getEl().unmask();
            }
        });
    },

    onViewItem: function(ItemKey) {
        var me = this;
        var tabs = this.up('app_pageframe');

        me.getEl().mask("Loading Item...");
        storeToNavigate = new CBH.store.vendors.Items().load({
            params: {
                id: ItemKey
            },
            callback: function() {
                var form = Ext.widget('items', {
                    storeNavigator: storeToNavigate,
                    ItemKey: ItemKey
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Item Maintenance',
                    items: [form]
                });

                form.down('#FormToolbar').gotoAt(1);
                tab.show();
                me.getEl().unmask();
            }
        });
    }
});