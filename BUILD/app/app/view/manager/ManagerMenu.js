Ext.define('CBH.view.manager.ManagerMenu', {
    extend: 'Ext.form.Panel',
    alias: 'widget.ManagerMenu',
    //width: 790,
    layout: {
        type: 'vbox',
        align: 'center',
        pack: 'center'
    },
    padding: '50 10 10 10',

    initComponent: function() {

        var me = this;



        Ext.applyIf(me, {
            items: [
                // main container
                {
                    xtype: 'container',
                    layout: 'column',
                    items: [
                        // Buttons Left
                        {
                            xtype: 'container',
                            columnWidth: 0.5,
                            minHeight: 324,
                            layout: {
                                align: 'stretch',
                                type: 'vbox'
                            },
                            items: [{
                                xtype: 'button',
                                flex: 1,
                                margin: '0 5 5 5',
                                text: 'Customer Database',
                                listeners: {
                                    click: {
                                        fn: me.onClickCustomerDatabase,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                //glyph: 0xf0c0,
                                minWidth: 165,
                                margin: '0 5 5 5',
                                text: 'Vendor Database',
                                listeners: {
                                    click: {
                                        fn: me.onClickVendorDatabase,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                //glyph: 0xf07a,
                                margin: '0 5 5 5',
                                text: 'Item Database',
                                listeners: {
                                    click: {
                                        fn: me.onClickItemDatabase,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                margin: '0 5 5 5',
                                text: 'Edit Payment Terms',
                                listeners: {
                                    click: {
                                        fn: me.onClickPaymentTerms,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                margin: '0 5 5 5',
                                text: 'Edit Employee List',
                                listeners: {
                                    click: {
                                        fn: me.onClickEditEmployeeList,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                height: 16,
                                margin: '10 5 5 5',
                                text: 'Edit Job Roles List',
                                listeners: {
                                    click: {
                                        fn: me.onClickEditJobRolesList,
                                        scope: me
                                    }
                                }
                            }]
                        },
                        // Buttons Right
                        {
                            xtype: 'container',
                            margin: '0 0 0 5',
                            columnWidth: 0.5,
                            minHeight: 324,
                            layout: {
                                align: 'stretch',
                                type: 'vbox'
                            },
                            items: [{
                                xtype: 'button',
                                flex: 1,
                                margin: '0 5 5 5',
                                text: 'Job Profit Report',
                                listeners: {
                                    click: {
                                        fn: me.onClickJobProfitReport,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                margin: '0 5 5 5',
                                text: 'Job Profit Excel Report',
                                listeners: {
                                    click: {
                                        fn: me.onClickJobProfitExcelReport,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                margin: '0 5 5 5',
                                text: 'Exempt Job from Profit Rpt',
                                listeners: {
                                    click: {
                                        fn: me.onClickqfrmJobExemptFromProfitReportList,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                margin: '0 5 5 5',
                                text: 'Customer Extranet Log',
                                listeners: {
                                    click: {
                                        fn: me.onClickCustomerExtranetLog,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                margin: '0 5 5 5',
                                text: 'Internal Quote Report',
                                listeners: {
                                    click: {
                                        fn: me.onClickInternalQuoteReport,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'splitbutton',
                                flex: 1,
                                height: 16,
                                margin: '10 5 5 5',
                                text: 'Pronaca Report',
                                menu: [
                                    // menu
                                    {
                                        text: 'Closed and Shipped',
                                        listeners: {
                                            click: {
                                                fn: me.onClickPronacaReportClosedShipped,
                                                scope: me
                                            }
                                        }
                                    }, {
                                        text: 'Transit Orders',
                                        listeners: {
                                            click: {
                                                fn: me.onClickPronacaTransitOrders,
                                                scope: me
                                            }
                                        }
                                    }, {
                                        text: 'Open Quotes',
                                        listeners: {
                                            click: {
                                                fn: me.onClickPronacaReportQuotes,
                                                scope: me
                                            }
                                        }
                                    }, {
                                        text: 'Exempt Jobs from Rpt',
                                        listeners: {
                                            click: {
                                                fn: me.onClickqfrmJobExemptFromPronacaReport,
                                                scope: me
                                            }
                                        }
                                    }
                                ],
                                listeners: {
                                    click: {
                                        fn: me.onClickPronacaReport,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                height: 16,
                                margin: '10 5 5 5',
                                text: 'Quote Report',
                                listeners: {
                                    click: {
                                        fn: me.onClickQuoteReport,
                                        scope: me
                                    }
                                }
                            }]
                        }
                    ]
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
        var me = this;
        //me.down('#btnEditQuote').setDisabled(true);
    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('#gridjobs');

        grid.store.removeAll();

        if (!String.isNullOrEmpty(fieldValue)) {
            grid.store.loadPage(1, {
                params: {
                    query: fieldValue
                },
                callback: function() {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                callback: function() {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        }
    },

    onClickCustomerDatabase: function() {
        var pageframe = this.up('app_pageframe');
        var tab = null;

        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Customers',
            items: [Ext.widget("customerslist")],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    grid.store.reload();
                }
            }
        });

        tab.show();
    },

    onClickEditEmployeeList: function() {
        var pageframe = this.up('app_pageframe');
        var tab = null;

        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Employee List',
            items: [Ext.widget("EmployeeList")],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    grid.store.reload();
                }
            }
        });

        tab.show();
    },

    onClickVendorDatabase: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Vendors',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("vendorslist")]
            }],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    grid.store.reload();
                }
            }
        });

        tab.show();
    },

    onClickItemDatabase: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Items',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("itemslist")]
            }],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    grid.store.reload();
                }
            }
        });

        tab.show();
    },

    onClickPaymentTerms: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Payment Terms',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("paymenttermslist")]
            }],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    grid.store.reload();
                }
            }
        });

        tab.show();
    },

    onClickEditJobRolesList: function() {
        var pageframe = this.up('app_pageframe');
        var tab = null;

        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Job Roles Maintenance',
            items: [Ext.widget("JobRolesList")],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    grid.store.reload();
                }
            }
        });

        tab.show();
    },

    onClickPronacaReport: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Form Criteria',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria")]
            }],
            listeners: {
                activate: function() {
                    /*var grid = this.down('form').down('gridpanel');
                    grid.store.reload();*/
                }
            }
        });

        tab.show();
    },

    onClickJobProfitReport: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Quote Report....',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptJobProfit'
                })]
            }],
            listeners: {
                activate: function() {
                    /*var grid = this.down('form').down('gridpanel');
                    grid.store.reload();*/
                }
            }
        });

        tab.show();
    },

    onClickqfrmJobExemptFromProfitReportList: function() {
        var pageframe = this.up('app_pageframe');
        var tab = null;

        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Exempt Job from Profit Report',
            items: [Ext.widget("qfrmJobExemptFromProfitReportList")],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    grid.store.reload();
                }
            }
        });

        tab.show();
    },

    onClickCustomerExtranetLog: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Customer Extranet Log....',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptCustomerWebLogins'
                })]
            }]
        });

        tab.show();
    },

    onClickInternalQuoteReport: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Quote Report',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptPronacaReportQuotes'
                })]
            }]
        });

        tab.show();
    },

    onClickQuoteReport: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Quote Report',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptPronacaReport NoProfit'
                })]
            }]
        });

        tab.show();
    },

    onClickqfrmJobExemptFromPronacaReport: function() {
        var pageframe = this.up('app_pageframe');
        var tab = null;

        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Exempt Job from Profit Report',
            items: [Ext.widget("qfrmJobExemptFromPronacaReport")],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    grid.store.reload();
                }
            }
        });

        tab.show();
    },

    onClickPronacaReportQuotes: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Quote Report',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptPronacaReportQuotes NoProfit'
                })]
            }]
        });

        tab.show();
    },

    onClickPronacaTransitOrders: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Quote Report',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptPronacaTransitOrders'
                })]
            }]
        });

        tab.show();
    },

    onClickPronacaReportClosedShipped: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Closed and Shipped Report',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptPronacaReportClosedShipped'
                })]
            }]
        });

        tab.show();
    }
});
