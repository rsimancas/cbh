Ext.define('CBH.view.jobs.JobPurchaseOrderStatusHistoryList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.JobPurchaseOrderStatusHistoryList',

    autoShow: true,
    autoRender: true,
    autoScroll: true,
    header: false,
    title: 'Purchase Order Status History',
    forceFit: true,
    layout: {
        type: 'anchor'
    },
    JobKey: 0,
    JobNum: "",
    Vendor: "",
    JobStatus: "",
    Customer: "",
    PONum: 0,
    POKey: 0,
    POShipETA: "",
    bodyPadding: 10,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        storeStatus = new CBH.store.jobs.JobPurchaseOrderStatusHistory({
            autoLoad: false
        }).load({
            params: {
                POKey: me.POKey
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
                items: [{
                    flex: 1,
                    xtype: 'textfield',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'PO Vendor',
                    readOnly: true,
                    margin: '0 0 10 0',
                    value: me.Vendor
                }, {
                    xtype: 'textfield',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'Job Num',
                    readOnly: true,
                    margin: '0 0 10 5',
                    value: me.JobNum
                }, {
                    xtype: 'textfield',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'PO Num',
                    readOnly: true,
                    margin: '0 0 10 5',
                    value: me.PONum
                }, {
                    xtype: 'datefield',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'Shipment ETA',
                    readOnly: true,
                    margin: '0 0 10 5',
                    value: me.POShipETA
                }]
            }, {
                xtype: 'gridpanel',
                itemId: 'gridstatus',
                store: storeStatus,
                minHeight: 150,
                height: 400,
                columns: [{
                    xtype: 'gridcolumn',
                    width: 120,
                    dataIndex: 'POStatusDate',
                    text: 'Date',
                    renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                }, {
                    xtype: 'gridcolumn',
                    width: 150,
                    dataIndex: 'x_Status',
                    text: 'Status'
                }, {
                    xtype: 'gridcolumn',
                    flex: 1,
                    dataIndex: 'POStatusMemo',
                    text: 'Description',
                }, {
                    xtype: 'gridcolumn',
                    width: 120,
                    dataIndex: 'POStatusModifiedDate',
                    text: 'Modified Date',
                    renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                }, {
                    xtype: 'gridcolumn',
                    width: 100,
                    dataIndex: 'POStatusModifiedBy',
                    text: 'Modified By'
                }, {
                    xtype: 'actioncolumn',
                    draggable: false,
                    width: 35,
                    resizable: false,
                    hideable: false,
                    stopSelection: false,
                    items: [{
                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                            /*if(me.JobStatus == "Job Closed") return;

                            if(record.data.StatusPONum == "*") {
                                store = Ext.create('CBH.store.jobs.JobStatusHistory',{autoLoad: false});
                                store.load({params:{id:record.data.StatusKey}, callback: function() {
                                    curRecord = this.getAt(0);
                                    var form = Ext.widget('jobstatushistory');
                                    form.loadRecord(curRecord);
                                    form.center();
                                    form.callerForm = me;
                                    form.show();    
                                }});
                            } else {*/
                            store = Ext.create('CBH.store.jobs.JobPurchaseOrderStatusHistory', {
                                autoLoad: false
                            });
                            store.load({
                                params: {
                                    id: record.data.POStatusKey
                                },
                                callback: function(records, success, eOpts) {
                                    curRecord = records[0];
                                    var form = new CBH.view.jobs.JobPurchaseOrderStatusHistory({
                                        EmployeeEmail: me.currentRecord.data.EmployeeEmail,
                                        ForwarderEmail: me.currentRecord.data.ForwarderEmail,
                                        CustEmail: me.currentRecord.data.CustEmail,
                                        JobNum: me.JobNum,
                                        currentRecord: curRecord
                                    });
                                    form.loadRecord(curRecord);
                                    form.center();
                                    form.callerForm = me;
                                    form.show();
                                }
                            });
                            //}
                        },
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                        tootip: 'view line detail'
                    }]
                }],
                tbar: [{
                    text: 'Add',
                    handler: function() {

                        record = new CBH.model.jobs.JobPurchaseOrderStatusHistory({
                            POStatusJobKey: me.JobKey,
                            POStatusPOKey: me.POKey,
                            POStatusDate: new Date()
                        });

                        var form = new CBH.view.jobs.JobPurchaseOrderStatusHistory({
                            EmployeeEmail: me.currentRecord.data.EmployeeEmail,
                            ForwarderEmail: me.currentRecord.data.ForwarderEmail,
                            CustEmail: me.currentRecord.data.CustEmail,
                            JobNum: me.JobNum,
                            currentRecord: record
                        });

                        form.loadRecord(record);
                        form.center();
                        form.callerForm = this.up('form');
                        form.show();
                    },
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
                selType: 'rowmodel',
                listeners: {
                    selectionchange : function(view, records) {
                        this.down('#deleteline').setDisabled(!records.length);
                    },
                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                        store = Ext.create('CBH.store.jobs.JobPurchaseOrderStatusHistory', {
                            autoLoad: false
                        });
                        store.load({
                            params: {
                                id: record.data.StatusKey
                            },
                            callback: function() {
                                curRecord = this.getAt(0);
                                var form = Ext.widget('JobPurchaseOrderStatusHistory');
                                form.loadRecord(curRecord);
                                form.center();
                                form.callerForm = me;
                                form.show();
                            }
                        });
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
