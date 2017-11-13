Ext.define('CBH.view.sales.FileStatusHistoryList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.filestatushistorylist',

    autoShow: true,
    autoRender: true,
    autoScroll: true,
    header: false,
    title: 'Status History',
    forceFit: true,
    layout: {
        type: 'anchor'
    },
    FileKey: 0,
    FileNum: "",
    Customer: "",
    FileStatus: "",
    bodyPadding: 10,

    initComponent: function() {

        var me = this;

        var storeStatus = new CBH.store.sales.FileStatusHistorySubDetails({
            autoLoad: false
        }).load({
            params: {
                filekey: me.FileKey,
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
                layout: {
                    type: 'hbox'
                },
                items: [
                {
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
                    itemId: 'filenum',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'Reference Num',
                    readOnly: true,
                    margin: '0 0 10 0',
                    value: me.FileNum
                }]
            }, {
                xtype: 'gridpanel',
                itemId: 'gridstatus',
                store: storeStatus,
                minHeight: 150,
                height: 400,
                columns: [
                    {
                        xtype: 'rownumberer'
                    }, 
                    {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'StatusDate',
                        text: 'Date',
                        renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'x_Status',
                        text: 'Status'
                    }, {
                        xtype: 'gridcolumn',
                        width: 80,
                        dataIndex: 'StatusQuoteNum',
                        text: 'Quote Num.'
                    }, {
                        xtype: 'gridcolumn',
                        flex: 1,
                        dataIndex: 'StatusMemo',
                        text: 'Description',
                    }, {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'StatusModifiedDate',
                        text: 'Modified Date',
                        renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                    }, {
                        xtype: 'gridcolumn',
                        width: 100,
                        dataIndex: 'StatusModifiedBy',
                        text: 'Modified By'
                    }
                    
                ],
                tbar: [{
                        text: 'Update',
                        handler: function() {
                            var form = new CBH.view.sales.FileStatusHistory({
                                FileKey: me.FileKey,
                                FileNum: me.FileNum
                            });
                            record = new CBH.model.sales.FileStatusHistory({
                                FileStatusFileKey: me.FileKey
                            });
                            form.loadRecord(record);
                            form.center();
                            form.callerForm = this.up('form');
                            form.show();
                        },
                        //disabled: (me.FileStatus == "File Closed") ? true : false
                    }
                ],
                selType: 'rowmodel',
                listeners: {
                    selectionchange: function(view, records) {
                        if (me.FileStatus == "File Closed") return;
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
                                        tip.update(grid.getStore().getAt(grid.recordIndex).get(header.dataIndex));
                                    }
                                }
                            }
                        });
                    }
                }
            }],
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onRenderForm: function() {

    }
});
