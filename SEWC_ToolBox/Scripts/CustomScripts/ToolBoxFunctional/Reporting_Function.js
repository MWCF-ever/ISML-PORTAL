$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    // 初始化表单监听
    layui.use('form', function () {
        var form = layui.form;

        // 监听创建提交
        form.on('submit(formCreate)', function (data) {

            var loading_index;
            var prurlstr = $("#PreUrltxt").val();

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Add_Report",
                type: "POST",
                ContentType: "application/json",
                dataType: "text",
                data: data.field,
                async: true,
                beforeSend: function () {
                    loading_index = customLoading();
                },
                success: function (result) {
                    var resultJSON = result;
                    if (typeof result == 'string') {
                        resultJSON = JSON.parse(result);
                    }

                    if (resultJSON.OpResult == true)
                        customAlert(1, resultJSON.OpMsg, "refreshParent");
                    else
                        customAlert(2, resultJSON.OpMsg);

                },
                complete: function () {
                    layer.close(loading_index);
                }
            });

            return false;

        });

        // 监听修改提交
        form.on('submit(formUpdate)', function (data) {

            var loading_index;

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Update_Report",
                type: "POST",
                ContentType: "application/json",
                dataType: "text",
                data: data.field,
                async: true,
                beforeSend: function () {
                    loading_index = customLoading();
                },
                success: function (result) {
                    var resultJSON = result;
                    if (typeof result == 'string') {
                        resultJSON = JSON.parse(result);
                    }

                    if (resultJSON.OpResult == true)
                        customAlert(1, resultJSON.OpMsg, "refreshParent");
                    else
                        customAlert(2, resultJSON.OpMsg);

                },
                complete: function () {
                    layer.close(loading_index);
                }
            });

            return false;

        });

        // Monitor select onchange events
        form.on('select(select_SubCategory)', function (data) {

            var catagoryId = data.value;

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Get_Catagory_BySubCatagoryId",
                type: "POST",
                ContentType: "application/json",
                dataType: "text",
                data: { "": catagoryId },
                async: true,
                beforeSend: function () {
                    loading_index = customLoading();
                },
                success: function (result) {
                    var resultJSON = result;
                    if (typeof result == 'string') {
                        resultJSON = JSON.parse(result);
                    }
                    var menuList = resultJSON.MenuList;

                    if (resultJSON.OpResult == true) {
                        $("#select_Category").html("");
                        $("#select_UpdateFrequency").html("");

                        $.each(menuList, function (key, value) {

                            if (value.ml_Third_MenuName == "Function Category") {
                                var option = $("<option>").val(value.ml_Fourth_ID).text(value.ml_Fourth_MenuName);
                                $("#select_Category").append(option);
                            }

                            if (value.ml_Third_MenuName == "Update Frequency") {
                                var option2 = $("<option>").val(value.ml_Fourth_ID).text(value.ml_Fourth_MenuName);
                                $("#select_UpdateFrequency").append(option2);
                            }
                        });
                        form.render("select");
                        //  $("#select_Category").get(0).selectedIndex = 0;
                    }
                },
                complete: function () {
                    layer.close(loading_index);
                }
            });

            return false;
        });

    });

    // 初始上传模块
    layui.use('upload', function () {

        var upload = layui.upload;
        var reportGUID = $("#input_reportGUID").val();
        //执行实例
        var uploadInst = upload.render({
            elem: '#btn_upload' //绑定元素
            , url: window.preURL+ "/api/ToolBox_Api/Upload_ItemImage"
            , method: "POST"
            , data: { "picType": "ReportImg", "itemGUID": reportGUID }
            , done: function (resultJSON) {

                //上传完毕回调
                if (resultJSON.OpResult == true) {

                    customMsg(1, resultJSON.OpMsg);

                    $("#input_picPath").val(resultJSON.EXT02);
                    $("#img_reportPic").attr("src", resultJSON.EXT03 + "?t=" + Math.random());

                    $("#img_reportPic").css("display", "block");

                }
                else {

                    customAlert(2, resultJSON.OpMsg);

                    $("#input_picPath").val("");
                    $("#img_reportPic").attr("src", "");

                    $("#img_reportPic").css("display", "none");

                }

            }
        });

    });

    // 关闭窗口按钮
    $("#btn_close").click(function () {

        closeFrame();

    });

    // 初始化表单内控件样式
    $("#btn_reset").click();

});