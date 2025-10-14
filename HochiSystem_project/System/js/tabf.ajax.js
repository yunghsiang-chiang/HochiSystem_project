
var TABFAjax = {
    fnSetup: function (Path, Data, fnSuccess = null, fnError = null) {
        $.ajaxSetup({
            url: Path,
            type : "post",
            data: Data,
            //dataType: '',
            //contentType: '',
            success: function (res) {
                //console.log('this is response data = '+res);
                if (fnSuccess != null) {
                    fnSuccess(res);
                }
            },
            error: function (xhr, opts, thrownError) {
                console.log(xhr);
                console.log(opts);
                console.log(thrownError);
                if (fnError != null) {
                    fnError(xhr, opts, thrownError);
                }
            }
        })
    },
    fnExec: function () {
        $.ajax();
    },
    fnBeforeSend: function (fnBeforeSend) {
        //console.log("fnBeforeSend inside");
        $(document).ajaxSend(fnBeforeSend);
    },
    fnComplete: function (fnComplete) {
        $(document).ajaxComplete(fnComplete)
    },
    fnOnError: function (fnOnError) {
        $(document).ajaxError(fnOnError(xhr,exMsg, errMsg ));
    },
    fnSuccess: function (fnSuccess) {
        $(document).ajaxSuccess(fnSuccess);
    },
    UploadFile: function (FormData, fnSuccess = null, fnError = null) {
        $.ajax({
            type: "POST",
            url: "/File/Upload",
            data: FormData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (fnSuccess != null) {
                    fnSuccess(res);
                }
            },
            error: function (xhr, opts, thrownError) {
                if (fnError != null) {
                    fnError(xhr, opts, thrownError);
                }
            }
        })
    }
}



