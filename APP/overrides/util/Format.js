// Override number to fix negative number
Ext.define('Overrides.util.Format', {
    override: 'Ext.util.Format',
    originalNumberFormatter: Ext.util.Format.number,
    number: function(v, formatString) {
        if (v < 0) {
            //negative number: flip the sign, format then prepend '-' onto output
            return '-' + this.originalNumberFormatter(v * -1, formatString);
        } else {
            //positive number: as you were
            return this.originalNumberFormatter(v, formatString);
        }
    },
    bsMoney: function(v) {
        var nStr = v.toFixed(2);

        nStr += '';
        x = nStr.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';

        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
           x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }

        return ('Bs.' + x1 + x2);
    },
    eurMoney: function(v) {
        var nStr = v.toFixed(2);

        nStr += '';
        x = nStr.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';

        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
           x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }

        return ('€' + x1 + x2);
    }
});