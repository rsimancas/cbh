<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportJobSummary.aspx.cs" Inherits="CBHWA.Areas.Reports.Views.rptJobSummary.ReportJobSummary" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body {
            font: 13px 'Segoe UI', Tahoma, Arial, Helvetica, sans-serif;
            background: #ddd;
            color: #333;
            margin: 0;
        }

        h1 {
            background: #333;
            color: #fff;
            padding: 10px;
            font: 29px 'Segoe UI Light', 'Tahoma Light', 'Arial Light', 'Helvetica Light', sans-serif;
        }

        .myRow {
            width: auto;
            padding: 0 5px 0 5px;
            height: auto;
        }
    </style>
</head>
<body>
    <%-- Store User's SessionId --%>
    <input type="hidden" id="sid" name="sid" value="<%=Session.SessionID%>" />
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div class="myRow" style="clear: both" id="pnlReport">
            <rsweb:ReportViewer ID="reportViewer" runat="server" Width="100%" Height="100%" ShowBackButton="false" ShowPrintButton="false"
                Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="12pt" ShowZoomControl="False">
            </rsweb:ReportViewer>
        </div>
    </form>

    <%-- Add Reference to jQuery at Google CDN --%>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.2/jquery.min.js"></script>
    <script src="<%=ResolveUrl("~/Scripts/arrive.js") %>"></script>

    <%-- Register the WebClientPrint script code --%>
    <%=Neodynamic.SDK.Web.WebClientPrint.CreateScript()%>


    <%--<script>
        // Print function (require the reportviewer client ID)
        function printReport(report_ID) {
            var rv1 = $('#' + report_ID);
            var iDoc = rv1.parents('html');

            // Reading the report styles
            var styles = iDoc.find("head style[id$='ReportControl_styles']").html();
            if ((styles == undefined) || (styles == '')) {
                iDoc.find('head script').each(function () {
                    var cnt = $(this).html();
                    var p1 = cnt.indexOf('ReportStyles":"');
                    if (p1 > 0) {
                        p1 += 15;
                        var p2 = cnt.indexOf('"', p1);
                        styles = cnt.substr(p1, p2 - p1);
                    }
                });
            }
            if (styles == '') { alert("Cannot generate styles, Displaying without styles.."); }
            styles = '<style type="text/css">' + styles + "</style>";

            // Reading the report html
            var table = rv1.find("div[id$='_oReportDiv']");
            if (table == undefined) {
                alert("Report source not found.");
                return;
            }

            // Generating a copy of the report in a new window
            var docType = '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/loose.dtd">';
            var docCnt = styles + table.parent().html();
            var docHead = '<head><title>Printing ...</title><style>body{margin:5;padding:0;}</style></head>';
            var winAttr = "location=yes, statusbar=no, directories=no, menubar=no, titlebar=no, toolbar=no, dependent=no, width=720, height=600, resizable=yes, screenX=200, screenY=200, personalbar=no, scrollbars=yes";;
            var newWin = window.open("", "_blank", winAttr);
            writeDoc = newWin.document;
            writeDoc.open();
            writeDoc.write(docType + '<html>' + docHead + '<body onload="window.print();">' + docCnt + '</body></html>');
            writeDoc.close();

            // The print event will fire as soon as the window loads
            newWin.focus();
            // uncomment to autoclose the preview window when printing is confirmed or canceled.
            // newWin.close();
        };

        $(document).ready(function () {
            $(".reportViewer").css({ 'height': ($(document).height() - ($(document).height() * 0.05).toFixed(0)) + 'px' });
        });
    </script>--%>

    <script>
        <%-- hide built-in print button from ReportViewer toolbar --%>
        $('table[title="Print"]').hide();

        <%-- embed custom DropDownList for listing installed client printers and print image button --%>
        $('#<%=reportViewer.ClientID%>_ctl05:last-child').children().append('<div class=" " style="display:inline-block;font-family:Verdana;font-size:8pt;vertical-align:top;"><table cellpadding="0" cellspacing="0" style="display:inline;"><tbody><tr><td height="28px"><select name="ddlClientPrinters" id="ddlClientPrinters" style="font-family:Verdana;font-size:8pt;" title="Select Printer"><option>Loading printers...</option></select></td><td width="4px"></td><td height="28px"><div><div id="<%=reportViewer.ClientID%>_Custom_Print_Button" style="border: 1px solid transparent; background-color: transparent; cursor: default;"><table title="Print"><tbody><tr><td><input type="image" title="Print" src="<%=ResolveUrl("~/Images/print.png")%>" alt="Print" style="border-style:None;height:16px;width:16px;border-width:0px;"></td></tr></tbody></table></div></div></td></tr></tbody></table></div>');

        <%-- embed custom Enqueue button --%>
        $('#<%=reportViewer.ClientID%>_ctl05:last-child').children().append('<div class=" " style="display:inline-block;font-family:Verdana;font-size:8pt;vertical-align:top;"><table cellpadding="0" cellspacing="0" style="display:inline;"><tbody><tr><td height="28px" style="padding-right:5px;">|</td></tr></tbody></table><table cellpadding="0" cellspacing="0" style="display:inline;"><tbody><tr><td height="28px"><button name="btnEnqueue" id="btnEnqueue" style="font-family:Verdana;font-size:8pt;" title="Enqueue">Enqueue</button></td></tr></tbody></table></div>');

        <%-- mouse hover effect for our new print image button --%>
        $('#<%=reportViewer.ClientID%>_Custom_Print_Button').hover(function () { //hover style
            $(this).css({ 'border': '1px solid #336699', 'background-color': '#DDEEF7', 'cursor': 'pointer' });

        }, function () { //normal style
            $(this).css({ 'border': '1px solid transparent', 'background-color': 'transparent', 'cursor': 'default' })

        });

        $("#<%=reportViewer.ClientID%>").css({ 'height': ($(document).height() - ($(document).height() * 0.05).toFixed(0)) + 'px' });

        <%-- Time delay we'll wait for getting client printer names --%>
        var wcppGetPrintersDelay_ms = 5000; //5 sec
        function wcpGetPrintersOnSuccess() {
            <%-- Display client installed printers --%>
            if (arguments[0].length > 0) {
                var p = arguments[0].split("|");
                var options = '<option>Default Printer</option>';
                for (var i = 0; i < p.length; i++) {
                    options += '<option>' + p[i] + '</option>';
                }
                $('#ddlClientPrinters').html(options);
            } else {
                alert("No printers are installed in your system.");
            }
        }

        function wcpGetPrintersOnFailure() {
            <%-- Do something if printers cannot be got from the client --%>
            alert("No printers are installed in your system.");
        }

        $(document).ready(function () {
            document.arrive("div[dir]", function () {
                $('div[dir]').css({ 'background-color': 'white' });
                
                <%-- load client printers through WebClientPrint --%>
                jsWebClientPrint.getPrinters();

                <%-- add click handler for print button --%>
                $('#<%=reportViewer.ClientID%>_Custom_Print_Button').click(function () {
                    jsWebClientPrint.print('printerName=' + $('#ddlClientPrinters').val());
                });
            })
        });
    </script>
</body>
</html>
