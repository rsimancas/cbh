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

function Serialize(obj) {
    return '?' + Object.keys(obj).reduce(function(a, k) { a.push(k + '=' + encodeURIComponent(obj[k])); return a; }, []).join('&') + '&_=' + (new Date()).getTime();
}

function next_id(input, stringLen) {
    stringLen = stringLen || 2;
    var output = parseInt(input, 10) + 1; // parse and increment
    output += ""; // convert to string
    while (output.length < stringLen) output = "0" + output; // prepend leading zeros
    return output;
}

/**
 * Pads the left side of a string with a specified character.  This is especially useful
 * for normalizing number and date strings.  Example usage:
 *
 *     var s = Ext.String.leftPad('123', 5, '0');
 *     // s now contains the string: '00123'
 *
 * @param {String} string The original string.
 * @param {Number} size The total length of the output string.
 * @param {String} [character=' '] (optional) The character with which to pad the original string.
 * @return {String} The padded string.
 */
function leftPad(string, size, character) {
    var result = String(string);
    character = character || " ";
    while (result.length < size) {
        result = character + result;
    }
    return result;
}

function GetNewRevisionNum(strRevision) {
    var strPart = StrVal(strRevision),
        intValue = 0,
        output = "";

    strRevision += "";

    if(strPart !== "") {
        intValue = parseInt(strPart);
    }

    if(intValue === 0) { // '*** Revision is all alpha characters
        strPart = strRevision.substring(strRevision.length - 1);
        intValue = strPart.charCodeAt(0);
        if(intValue === 90 && strRevision.length === 1) {
            output = "AA";
        }  else if(intValue === 122 && strRevision.length === 1) {
            output = "aa";
        } else {
            strPart = (intValue + 1).toString();
            output = strRevision.substring(0,strRevision.length-1) + strPart;
        }

    } else { // '*** Revision is alpha-numeric
        intValue += 1;
        strPart = strRevision.substring(0, strRevision.length - strPart.length);
        output = strPart + leftPad(intValue, 2, "0");
    }

    output = String.fromCharCode(parseInt(output));
    return output;
}

function StrVal(expression) {
    var counter = 0,
        strVal = "";

    if(!expression.length) return "";

    for(var i = 0; i < expression.length; i++) {
        chrCode = expression.charCodeAt(i);
        if((chrCode >= 48 && chrCode <= 57) || expression[i] === "." || expression[i] === "-" || expression[i] === "(" || expression[i] === ")") {
            strVal += expression[i];
        }
    }

    return strVal;
}