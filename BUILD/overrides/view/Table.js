/* To Fix className error on model.destroy*/
Ext.define('Overrides.view.Table', {
    override: 'Ext.view.Table',

    doStripeRows: function(startRow, endRow) {
        var me = this,
            rows,
            rowsLn,
            i,
            row;
 
 
        if (me.rendered && me.stripeRows) {
            rows = me.getNodes(startRow, endRow);
 
            for (i = 0, rowsLn = rows.length; i < rowsLn; i++) {
                row = rows[i];
 
                if (row) { // self updating; check for row existence
                    row.className = row.className.replace(me.rowClsRe, ' ');
                    startRow++;
 
                    if (startRow % 2 === 0) {
                        row.className += (' ' + me.altRowCls);
                    }
                }
            }
        }
    }
});