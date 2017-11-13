Ext.define('CBH.view.vendors.ItemsPriceHistory', {
    extend: 'Ext.form.Panel',
    alias: 'widget.itemspricehistory',

    bodyPadding: 10,
    layout: { type: 'column'},

    requires: [
    ],
    storeNavigator: null,

    initComponent: function() {
        var me = this,
            storeLangs = null,
            storeHistory = null;


        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [
                {// Item Information
                    xtype: 'fieldset',
                    id: 'PHEncFieldSet',
                    columnWidth: 0.5,
                    layout: {
                        type: 'column'
                    },
                    padding: '0 10 10 10',
                    collapsible: true,
                    title: 'Item Information',
                    items: [
                        {
                            xtype: 'textfield',
                            name: 'x_VendorName',
                            fieldLabel: 'Vendor',
                            id: 'PHItemVendorKey',
                            columnWidth: 1,
                            readOnly: true,
                            allowBlank: false
                        },
                        {
                            xtype: 'textfield',
                            name: 'ItemNum',
                            fieldLabel: 'Item Number',
                            id: 'PHItemNum',
                            columnWidth:1,
                            readOnly: true,
                            allowBlank: false
                        },
                    ]
                },
                {// Descriptions Panel
                    columnWidth: 0.5,
                    xtype: 'panel',
                    title: 'Descriptions',
                    margin: '0 0 0 10',
                    items: [
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridhistory',
                            minHeight: 150,
                            store: storeDesc,
                            maxHeight: 300,
                            columns: [
                                {
                                    xtype: 'rownumberer',
                                    format: '00,000'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    flex: 2,
                                    text: 'Languages',
                                    dataIndex: 'x_Language'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    flex: 8,
                                    dataIndex: 'DescriptionText',
                                    text: 'Description',
                                    format: '0' 
                                }
                            ]
                        }
                    ]
                },
                {// Descriptions History
                    columnWidth: 1,
                    xtype: 'panel',
                    title: 'Descriptions',
                    items: [
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridhistory',
                            minHeight: 350,
                            store: storeHistory,
                            maxHeight: 600,
                            columns: [
                                {
                                    xtype: 'rownumberer',
                                    format: '00,000'
                                },
                                {
                                    xtype: 'numbercolumn',
                                    width:65,
                                    dataIndex: 'PONum',
                                    text: 'PO Num',
                                    format: '0'
                                },
                                {
                                    xtype: 'numbercolumn',
                                    width:125,
                                    text: 'Cost From Supplier',
                                    dataIndex: 'CostFromSupplier',
                                    format: '00,000.00',
                                    align: 'right'
                                },
                                {
                                    xtype: 'numbercolumn',
                                    width:150,
                                    text: 'Price paid by Customer',
                                    dataIndex: 'PricePaidByCust',
                                    format: '00,000.00',
                                    align: 'right'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    width:100,
                                    flex:3,
                                    dataIndex: 'CustName',
                                    text: 'Name'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    width:100,
                                    flex: 1,
                                    dataIndex: 'Date',
                                    text: 'Date',
                                    renderer: Ext.util.Format.dateRenderer('m/d/Y')
                                }
                            ]
                        }
                    ]
                }
            ],
            /*dockedItems: [
                {
                    xtype: 'formtoolbar',
                    itemId: 'PHFormToolbar',
                    dock: 'top',
                    store: me.storeNavigator,
                    listeners: {
                        afterloadrecord: {
                            fn: me.onAfterLoadRecord,
                            scope: me
                        }
                    }
                },
                {
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    items: [
                        {
                            xtype: 'textfield',
                            itemId: 'FileCreatedBy',
                            name: 'FileCreatedBy',
                            readOnly: true,
                            editable: false,
                            fieldLabel: 'Created By'
                        },
                        {
                            xtype: 'datefield',
                            name: 'FileCreatedDate',
                            itemId: 'FileCreatedDate',
                            readOnly: true,
                            editable: false,
                            fieldLabel: 'Created Date',
                            hideTrigger: true
                        },
                        {
                            xtype: 'textfield',
                            name: 'FileModifiedBy',
                            itemId: 'FileModifiedBy',
                            readOnly: true,
                            editable: false,
                            fieldLabel: 'Modified By'
                        },
                        {
                            xtype: 'datefield',
                            name: 'FileModifiedDate',
                            itemId: 'FileModifiedDate',
                            readOnly: true,
                            editable: false,
                            fieldLabel: 'Modified Date',
                            hideTrigger: true
                        },
                        {
                            xtype: 'component',
                            flex: 1
                        }
                    ],                
                }
            ],*/
            listeners:{
                render: {
                    fn: me.onRenderForm,
                    scope: me
                },
            }
        });
        me.callParent(arguments);
    },
    onRenderForm: function() {
        var me = this;
    },
});