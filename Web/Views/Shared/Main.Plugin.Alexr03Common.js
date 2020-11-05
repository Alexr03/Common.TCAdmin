var Alexr03 = {};

Alexr03.Common = function Alexr03$Common() {

    function _handleAjaxSuccess(e, okFunction) {
        if (!isFunction(okFunction)) {
            if (e.Message) {
                toastr["success"]("Success", e.Message)
            }
        } else {
            if (e.Message) {
                TCAdmin.Ajax.ShowBasicDialog("Success!", e.Message, okFunction);
            }
        }

        _handleAjaxComplete();
    }

    function _handleAjaxError(e, okFunction) {
        if (!isFunction(okFunction)) {
            if (e.responseJSON && e.responseJSON.Message) {
                toastr["error"]("Error", e.responseJSON.Message)
            } else {
                toastr["error"]("Error", "An error has occured! Please try again later.")
            }
        } else {
            if (e.responseJSON && e.responseJSON.Message) {
                TCAdmin.Ajax.ShowBasicDialog("Error!", e.responseJSON.Message, okFunction);
            } else {
                TCAdmin.Ajax.ShowBasicDialog("Error!", "An error has occured! Please try again later.", okFunction)
            }
        }

        _handleAjaxComplete();
    }

    function _handleAjaxBegin() {
        kendo.ui.progress($(document.body), true)
    }

    function _handleAjaxComplete() {
        kendo.ui.progress($(document.body), false)
    }

    function _warningDialog(title, message) {
        return new Promise(function (ok, cancel) {
            $("<div id='warningDialog'></div>").kendoDialog({
                width: "500px",
                title: title,
                closable: true,
                modal: true,
                content: `
                <div class="row">
                    <div class="col-3 d-flex justify-content-center">
                        <span class="k-icon k-i-warning" style="font-size: 64px;color:orange"></span>
                    </div>
                    <div class="col">
                        ` + message + `
                    </div>
                </div>`,
                actions: [
                    {
                        text: 'Close', action: function (e) {
                            cancel(e)
                        }
                    },
                    {
                        text: 'Ok', primary: true, action: function (e) {
                            ok(e)
                        }
                    }
                ],
                close: function (e) {
                    e.sender.destroy();
                }
            });
            $("#warningDialog").data("kendoDialog").open();
        })
    }

    function _dangerDialog(title, message) {
        return new Promise(function (ok, cancel) {
            $("<div id='dangerDialog'></div>").kendoDialog({
                width: "500px",
                title: title,
                closable: true,
                modal: true,
                content: `
                <div class="row">
                    <div class="col-3 d-flex justify-content-center">
                        <span class="k-icon k-i-warning" style="font-size: 64px;color:red"></span>
                    </div>
                    <div class="col">
                        ` + message + `
                    </div>
                </div>`,
                actions: [
                    {
                        text: 'Close', action: function (e) {
                            cancel(e)
                        }
                    },
                    {
                        text: 'Ok', primary: true, action: function (e) {
                            ok(e)
                        }
                    }
                ],
                close: function (e) {
                    e.sender.destroy();
                }
            });
            $("#dangerDialog").data("kendoDialog").open();
        })
    }

    function isFunction(functionToCheck) {
        return functionToCheck && {}.toString.call(functionToCheck) === '[object Function]';
    }

    function _executePopupScript(url, extraArgs, title, closeText, onClose) {
        if (onClose == null) {
            onClose = function () {
            };
        }
        console.log(extraArgs)
        let success = false;
        let outputwindow = $("<div></div>");
        let loader = $('<div class="popup-script-loading">&nbsp;</div>');
        let output = $('<div class="popup-script-output"></div>');
        let outputlog = $('<div><span class="k-icon k-i-more-horizontal" style="float:right;cursor:pointer;"></span><div style="clear:both"></div><ul class="popup-script-log-entries"></ul></div>');
        let outputlogentries = outputlog.find(".popup-script-log-entries");
        outputlogentries.hide();
        let popupwindow = outputwindow.kendoAlert({
            title: title,
            close: onClose,
            content: output.html("&nbsp;")
        }).data("kendoAlert");
        popupwindow.open();

        outputwindow.prepend(loader);
        outputwindow.append(outputlog);
        kendo.ui.progress(loader, true);
        outputwindow.parent().find(".k-button").text(closeText);

        //expand log
        outputlog.find("span.k-icon").click(function () {
            outputlogentries.toggle();
        });

        let dofetch = function (data) {
            fetch(data.Url, {
                headers: {
                    'Content-Type': 'application/json'
                },
                method: 'POST', // or 'PUT'
                body: JSON.stringify(data.ExtraArgs.body),
            }).then(function (response) {
                const reader = response.body.getReader();
                const go = function () {
                    reader.read().then(function (result) {
                        let tmp;
                        if (!result.done) {
                            let line = Utf8ArrayToStr(result.value).trim();
                            if (line) {
                                //fix for android chrome
                                tmp = line.split("}{");
                                if (tmp.length > 1) {
                                    for (var i = 0; i < tmp.length; i++) {
                                        if (tmp[i].indexOf("{") !== 0) {
                                            tmp[i] = "{" + tmp[i]
                                        }

                                        if (tmp[i].lastIndexOf("}") !== tmp[i].length - 1) {
                                            tmp[i] = tmp[i] + "}"
                                        }
                                    }
                                } else {
                                    tmp = line.split("\n");
                                }

                                for (var i = 0; i < tmp.length; i++) {
                                    var message = JSON.parse(tmp[i]);
                                    if (message.Message) {
                                        if (message.Message.indexOf("Process exited with code") === -1) {
                                            output.html(message.Message);
                                        }
                                        console.log(message.Message)
                                        outputlogentries.append(`<li>${output.clone().find('script').remove().end().html()}</li>`);
                                        if (outputlogentries.is(":visible")) {

                                        }
                                    } else
                                        success = message.Success;
                                }
                            }
                            go();
                        } else {
                            kendo.ui.progress(loader, false);
                            if (typeof success !== 'undefined') {
                                if (success) {
                                    loader.addClass("k-icon");
                                    loader.addClass("k-i-success");
                                    loader.addClass("popup-script-loading-success")
                                } else {
                                    loader.addClass("k-icon");
                                    loader.addClass("k-i-warning");
                                    loader.addClass("popup-script-loading-failure")
                                }
                            }
                        }
                    })
                };

                go();
            })
        }

        data = {};
        data.Url = url;
        data.ExtraArgs = extraArgs;
        dofetch(data);
    }

    return {
        HandleAjaxSuccess: _handleAjaxSuccess,
        HandleAjaxFailure: _handleAjaxError,
        HandleAjaxBegin: _handleAjaxBegin,
        HandleAjaxComplete: _handleAjaxComplete,
        ExecutePopupScript: _executePopupScript,
        WarningDialog: _warningDialog,
        DangerDialog: _dangerDialog
    }
}();