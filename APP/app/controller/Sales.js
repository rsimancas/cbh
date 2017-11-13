Ext.define('CBH.controller.Sales', {
    extend: 'Ext.app.Controller',

    stores: [
        'CBH.store.sales.FileStatusHistorySubDetails'
    ],

    init: function(application) {
        this.control({
            'salesmenu gridview': {
                expandbody: this.onExpandRow,
                collapsebody: this.onCollapseRow
            },
            // 'salesmenu gridpanel': {
            //     selectionchange: this.onSelectChange
            // }
        });
    },

    onExpandRow: function(node, record, eNode) {
        var status = new CBH.store.sales.FileStatusHistorySubDetails().load({
            params: {
                filekey: record.data.FileKey
            }
        });

        var element = Ext.get(eNode).down('.ux-row-expander-box');

        var grid = Ext.create('Ext.grid.Panel', {
            cls: 'custom-grid',
            store: status,
            hideHeaders: false,
            border: false,
            minHeight: 120,
            margin: '0 0 0 20',
            viewConfig: {
                stripeRow: true
            },
            columns: [{
                xtype: 'rownumberer'
            }, {
                xtype: 'gridcolumn',
                width: 90,
                dataIndex: 'StatusDate',
                text: 'Date',
                renderer: Ext.util.Format.dateRenderer('m/d/Y')
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
                dataIndex: 'StatusMemo',
                text: 'Description',
                flex: 1
            }]
        });

        Ext.apply(grid, this.expandable);
        grid.on('itemclick', function(view) {
            this.getView().getSelectionModel().deselectAll();
        });

        element.swallowEvent(['click', 'mousedown', 'mouseup', 'dblclick'], true);
        grid.render(element);
    },

    onCollapseRow: function(node, record, eNode) {
        Ext.get(eNode).down('.ux-row-expander-box').down('div').destroy();
    },
});
