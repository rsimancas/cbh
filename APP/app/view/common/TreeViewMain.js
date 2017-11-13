Ext.define('CBH.view.common.TreeViewMain', {
    extend: 'Ext.tree.Panel',

    xtype: 'app_treeview',
    rootVisible: false,
    lines: false,
    useArrows: true,
    /*title: 'Navigation Panel',*/

    //bodyStyle:{"background-color":"#EDEDED"}, 

    initComponent: function() {

        var me = this;

        Ext.applyIf(me, {
            listeners: {
                itemclick: {
                    fn: me.onTreepanelItemClick,
                    scope: me
                },
                afterrender: function() {
                    var usr = CBH.GlobalSettings.getCurrentUser();
                        al = usr.EmployeeAccessLevel;

                    if( al !== 1 && al !== 2) {
                        this.setNodeVisible('manager-menu', false);
                        //this.setNodeVisible('job-menu', false);
                    }
                }
            }
        });

        me.callParent(arguments);
    },

    onTreepanelItemClick: function(dataview, record, item, index, e, eOpts) {
        var pageframe = dataview.up('viewport').down('app_pageframe');
        var tab = null;

        if(record.internalId === 'find-cus') {

            var customersgrid =  Ext.widget("customerslist");

            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                autoScroll: true,
                title: record.data.text.trim(),
                items: [customersgrid],
                listeners: {
                    activate: function() {
                        var grid = this.down('form').down('gridpanel');
                            grid.store.reload();
                    }
                }
            });

            tab.show();
        }

        if(record.internalId === 'find-ven') {

            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                autoScroll: true,
                title: record.data.text.trim(),
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
        }

        if(record.internalId === 'find-item') {

            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                autoScroll: true,
                title: record.data.text.trim(),
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
        }


        if(record.internalId === 'find-currate') {

            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                autoScroll: true,
                title: record.data.text.trim(),
                items: [{
                    xtype: 'container',
                    layout: {
                        type: 'anchor'
                    },
                    items: [Ext.widget("currencylist")]
                }],
                listeners: {
                    activate: function() {
                        var grid = this.down('form').down('gridpanel');
                        grid.store.reload();
                    }
                }
            });

            tab.show();
        }

        if(record.internalId === 'find-payterms') {
            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                autoScroll: true,
                title: record.data.text.trim(),
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
        }

        if(record.internalId === 'find-sched-b') {
            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                autoScroll: true,
                title: record.data.text.trim(),
                items: [{
                    xtype: 'container',
                    layout: {
                        type: 'anchor'
                    },
                    items: [Ext.widget("ScheduleBList")]
                }],
                listeners: {
                    activate: function() {
                        var grid = this.down('form').down('gridpanel');
                        grid.store.reload();
                    }
                }
            });

            tab.show();
        }

        if(record.data.text === 'Job Menu') {
            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                autoScroll: true,
                title: record.data.text.trim(),
                items: [{
                    xtype: 'container',
                    layout: {
                        type: 'anchor'
                    },
                    items: [Ext.widget("jobmenuform")]
                }],
                listeners: {
                    activate: function() {
                        var grid = this.down('form').down('gridpanel');
                        grid.store.reload();
                    }
                }
            });

            tab.show();
        }

        if(record.data.text === 'Sales Menu') {
            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                title: record.data.text.trim(),
                items: [{
                    xtype: 'container',
                    layout: {
                        type: 'anchor'
                    },
                    items: [Ext.widget("salesmenu")]
                }],
                listeners: {
                    activate: function() {
                        var grid = this.down('form').down('gridpanel');
                        grid.store.reload();
                    }
                }
            });

            tab.show();
        }

        if(record.internalId === 'manager-menu') {
            tab = pageframe.add({
                closable: true,
                iconCls: 'tabs',
                autoScroll: true,
                title: record.data.text.trim(),
                items: [{
                    xtype: 'container',
                    layout: {
                        type: 'anchor'
                    },
                    items: [Ext.widget("ManagerMenu")]
                }],
                listeners: {
                    activate: function() {
                        //var grid = this.down('form').down('gridpanel');
                        //grid.store.reload();
                    }
                }
            });

            tab.show();
        }
    },

    setNodeVisible: function(nodeId, visible) {
        var store = this.getStore(),
            view = this.getView(),
            node = view.getNode(store.getNodeById(nodeId)),
            el = Ext.fly(node);

        el.setVisibilityMode(Ext.Element.DISPLAY);
        el.setVisible(visible);
    },

    changeManagerIcon: function() {
        var store = this.getStore(),
            view = this.getView(),
            node = view.getNode(store.getNodeById(nodeId));
        node.set('iconCls', 'fa fa-user');
    }
});