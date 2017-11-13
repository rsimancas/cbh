Ext.define('CBH.view.jobs.JobStatusHistoryList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.jobstatushistorylist',

    autoShow: true,
    autoRender: true,
    autoScroll: true,
    header: false,
    title: 'Job Status History',
    forceFit: true,
    layout: {
        type: 'anchor'
    },
    JobKey: 0,
    JobNum: "",
    Customer: "",
    JobStatus: "",
    bodyPadding: 10,

    initComponent: function() {

        var me = this;

        storeStatus = new CBH.store.jobs.JobStatusHistorySubDetails({
            autoLoad: false
        }).load({
            params: {
                JobKey: me.JobKey,
                page: 0,
                limit: 0,
                start: 0
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
                    itemId: 'customer',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'Customer',
                    readOnly: true,
                    margin: '0 0 10 0',
                    value: me.Customer
                }, {
                    xtype: 'textfield',
                    itemId: 'jobnum',
                    fieldStyle: 'text-align=right; color: #157fcc;font-weight:bold;',
                    fieldLabel: 'Reference Num',
                    readOnly: true,
                    margin: '0 0 10 0',
                    value: me.JobNum
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
                    dataIndex: 'StatusDate',
                    text: 'Date',
                    renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                }, {
                    xtype: 'gridcolumn',
                    width: 120,
                    dataIndex: 'x_Status',
                    text: 'Status'
                }, {
                    xtype: 'gridcolumn',
                    width: 80,
                    dataIndex: 'StatusPONum',
                    text: 'PO Num.'
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
                    width: 110,
                    dataIndex: 'StatusModifiedBy',
                    text: 'Modified By'
                }, {
                    xtype: 'actioncolumn',
                    width: 25,
                    getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                    tooltip: 'view details',
                    listeners: {
                        click: function(view, rowIndex, colIndex, item, e, record) {
                            var me = this.up("form");
                            me.onClickViewDetails(record);
                        }
                    }
                }],
                tbar: [{
                    text: 'Add',
                    handler: function() {
                        var form = new CBH.view.jobs.JobStatusHistory({
                            JobKey: me.JobKey,
                            JobNum: me.JobNum
                        });

                        record = new CBH.model.jobs.JobStatusHistory({
                            JobStatusJobKey: me.JobKey,
                            JobStatusDate: new Date()

                        });
                        form.loadRecord(record);
                        form.center();
                        form.callerForm = this.up('form');
                        form.show();
                    }
                }],
                selType: 'rowmodel',
                listeners: {
                    celldblclick: function( gridPanel, td, cellIndex, record, tr, rowIndex, e, eOpts ) {
                        var me = gridPanel.up("form");
                        me.onClickViewDetails(record);
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

    },

    onClickViewDetails: function(record) {
        var me = this;

        var storeHistory = new CBH.store.jobs.JobStatusHistory().load({
            params: {
                id: record.data.StatusKey
            },
            callback: function(records, operation, success) {
                var form = new CBH.view.jobs.JobStatusHistory({
                    JobKey: me.JobKey,
                    JobNum: me.JobNum
                });

                if(records && records.length) {
                    form.loadRecord(records[0]);
                    form.center();
                    form.callerForm = me;
                    form.show();
                    if (!record.phantom) form.down("#acceptbutton").setVisible(false);
                } else {
                    Ext.Msg.alert('Status', 'This record must be changed from its origin');
                }
            }
        });
    }
});