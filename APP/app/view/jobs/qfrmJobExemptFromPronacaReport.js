Ext.define('CBH.view.jobs.qfrmJobExemptFromPronacaReport', {
    extend: 'Ext.form.Panel',
    alias: 'widget.qfrmJobExemptFromPronacaReport',

    /*autoShow: true,
    autoRender: true,
    autoScroll: true,
    header: false,
    title: 'Exempt Job from Profit Report',*/
    /*forceFit: true,*/
    layout: {
        type: 'anchor'
    },
    bodyPadding: 10,

    initComponent: function() {

        var me = this;

        storeExempt = new CBH.store.jobs.qfrmJobExemptFromPronacaReport();

        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [
                // Grid JobProfit
                {
                    xtype: 'gridpanel',
                    itemId: 'gridexempt',
                    store: storeExempt,
                    minHeight: 350,
                    forceFit: true,
                    viewConfig: {
                        stripeRows: true
                    },
                    columns: [
                        // Job Num
                        {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'JobNum',
                            text: 'Job Num'
                        },
                        // Check Exempt
                        {
                            width: 140,
                            xtype: 'checkcolumn',
                            dataIndex: 'JobExemptFromPronacaReport',
                            text: 'Exempt From Report',
                            listeners: {
                                'checkchange': function(comp, rowIndex, checked, eOpts) {
                                    var grid = this.up('form').down('gridpanel'),
                                                record = grid.store.getAt(rowIndex);

                                    record.getProxy().setSilentMode(true);
                                    record.save();
                                }
                            }
                        },
                        // Product Description
                        {
                            xtype: 'gridcolumn',
                            flex: 1,
                            dataIndex: 'JobProdDescription',
                            text: 'Product Description'
                        },
                        // Reference
                        {
                            xtype: 'gridcolumn',
                            flex: 1,
                            dataIndex: 'JobReference',
                            text: 'Reference',
                        },
                        // Customer Name
                        {
                            xtype: 'gridcolumn',
                            flex: 1,
                            dataIndex: 'CustName',
                            text: 'Customer Name'
                        },
                        // Customer Contact
                        {
                            xtype: 'gridcolumn',
                            flex: 1,
                            dataIndex: 'CustContact',
                            text: 'Contact'
                        }
                    ],
                    tbar: [
                        // Search Field
                        {
                            xtype: 'searchfield',
                            width: '50%',
                            itemId: 'searchfield',
                            name: 'searchField',
                            listeners: {
                                'triggerclick': function(field) {
                                    me.onSearchFieldChange();
                                }
                            }
                        }, {
                            xtype: 'component',
                            flex: 1
                        }
                    ],
                    bbar: new Ext.PagingToolbar({
                        itemId: 'pagingtoolbar',
                        store: storeExempt,
                        displayInfo: true,
                        displayMsg: 'Displaying records {0} - {1} of {2}',
                        emptyMsg: "No records to display"
                    }),
                    selType: 'rowmodel'
                }
            ],
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

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('#gridexempt');

        grid.store.removeAll();

        if (!String.isNullOrEmpty(fieldValue)) {
            grid.store.loadPage(1, {
                params: {
                    query: fieldValue
                },
                callback: function(records, operation, success) {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                callback: function(records, operation, success) {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        }
    }
});
