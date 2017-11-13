Ext.define('CBH.view.sales.FilesGrid', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.filesgrid',
    xtype: 'filesgrid',

    autoShow: true,
    autoRender: true,
    height: 420,
    width: 743,
    autoScroll: true,
    header: false,
    forceFit: true,

    requires:[
        'CBH.store.sales.FileList'
    ],

    initComponent: function() {

        var me = this;

        var pluginExpanded = true;

        var storeFileList = new CBH.store.sales.FileList({
            autoLoad: true
        });

        Ext.applyIf(me, {
            store: storeFileList,
            columns: [{
                xtype: 'rownumberer'
            }, {
                xtype: 'gridcolumn',
                width: 85,
                dataIndex: 'Date',
                text: 'Date',
                renderer: Ext.util.Format.dateRenderer('m/d/Y')
            }, {
                xtype: 'gridcolumn',
                width: 80,
                dataIndex: 'FileNum',
                text: 'File Num'
            }, {
                xtype: 'gridcolumn',
                width: 280,
                dataIndex: 'Customer',
                text: 'Customer'
            }, {
                xtype: 'gridcolumn',
                width: 250,
                dataIndex: 'Reference',
                text: 'Reference'
            }, {
                xtype: 'gridcolumn',
                dataIndex: 'Status',
                text: 'Status'
            }, {
                xtype: 'actioncolumn',
                width: 35,
                items: [{
                    iconCls: 'cbh-find',
                    tooltip: 'view details'
                }]
            }],
            plugins: [{
                ptype: 'rowexpander',
                rowBodyTpl: [
                    '<div class="ux-row-expander-comment"><p><b>Modified By:</b> {ModifiedBy}</p><p><b> Modified Date:</b> {ModifiedDate:date("m/d/Y")}</p></div>',
                    '<div class="ux-row-expander-box"></div>'
                ],
                expandOnRender: true,
                expandOnDblClick: false
            }],
            bbar: new Ext.PagingToolbar({
                store: storeFileList,
                displayInfo: true,
                displayMsg: 'Displaying records {0} - {1} of {2}',
                emptyMsg: "No records to display"
            })
        });

        // trigger the data store load
        //Ext.data.StoreManager.lookup('fileliststore').loadPage(1);
        storeFileList.loadPage(1);

        me.callParent(arguments);
    }
});
