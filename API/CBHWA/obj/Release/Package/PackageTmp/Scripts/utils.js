var idleTime = 0;

String.isNullOrEmpty = function(value) {
    if (value === null) return true;
    return !(typeof value === "string" && value.trim().length > 0);
};

String.prototype.trim = function() {
    return this.replace(/^\s+|\s+$/g, "");
};


String.prototype.format = function() {
    var s = this,
        i = arguments.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};

if (!Array.indexOf) {
    Array.prototype.indexOf = function(obj) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == obj) {
                return i;
            }
        }
        return -1;
    };
}

String.prototype.replaceAll = function(search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function deepCloneStore(source) {
    var target = Ext.create('Ext.data.Store', {
        model: source.model
    });

    Ext.each(source.getRange(), function(record) {
        var newRecordData = Ext.clone(record.copy().data);
        var model = new source.model(newRecordData, newRecordData.id);

        target.add(model);
    });

    return target;
}


function createUUID() {
    // http://www.ietf.org/rfc/rfc4122.txt
    var s = [];
    var hexDigits = "0123456789abcdef";
    for (var i = 0; i < 36; i++) {
        s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    s[14] = "4"; // bits 12-15 of the time_hi_and_version field to 0010
    s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1); // bits 6-7 of the clock_seq_hi_and_reserved to 01
    s[8] = s[13] = s[18] = s[23] = "-";

    var uuid = s.join("");
    return uuid;
}

function GetMarginPrice(cost, margin) {
    var price = 0;
    if (margin === 1) {
        price = cost;
    } else {
        price = (cost / (1 - margin)).toFixed(4);
    }

    return price;
}

function GetProfitMargin(cost, price) {
    var profit = 0;

    profit = (((price - cost) / price) * 1000) / 1000;

    return (profit * 100).toFixed(2);
}

function Kilograms(pounds) {
    pounds = (!pounds) ? 0 : pounds;

    return pounds * 0.45359237;
}

function Pounds(kilograms) {
    kilograms = (!kilograms) ? 0 : kilograms;

    return kilograms * 2.2046;
}

function CubicMeters(cubicFeet) {
    cubicFeet = (!cubicFeet) ? 0 : cubicFeet;

    return cubicFeet / 35.314;
}

function CubicFeet(cubicMeter) {
    cubicMeter = (!cubicMeter) ? 0 : cubicMeter;

    return cubicMeter * 35.314;
}

function serialize(obj) {
    return '?' + Object.keys(obj).reduce(function(a, k) { a.push(k + '=' + encodeURIComponent(obj[k])); return a; }, []).join('&');
}

bootstrap_alert = function () { }
bootstrap_alert.warning = function (message, alert, timeout) {
    $('<div id="floating_alert" class="alert alert-' + alert + ' fade in"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>' + message + '&nbsp;&nbsp;</div>').appendTo('body');


    setTimeout(function () {
        $(".alert").alert('close');
    }, timeout);

}