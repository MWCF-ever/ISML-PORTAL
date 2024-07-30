$(document).ready(function () {

    $.ajaxSetup({
        async: false
    });

    // 鼠标进入报表图片区域时，显示工具条
    $('.div_report_ImgView').mouseenter(function () {

        var ctl = $(this).find(".div_report_Btn");

        // 0.5秒后若仍在图片区域内，显示工具条
        trigger = setTimeout(function () {

            if (ctl.is(":hidden"))
                ctl.slideDown();

        }, 500);

    });

    // 鼠标离开报表图片区域时，隐藏工具条
    $(".div_report_ImgView").mouseleave(function () {

        if (trigger != undefined)
            clearTimeout(trigger);

        var ctl = $(this).find(".div_report_Btn");

        if (!ctl.is(":hidden"))
            ctl.slideUp();

    });

    // 打开编辑报表模块
    $("[name='a_report_edit']").click(function (event) {

        event.preventDefault();
        var ctl = $(this).parent().parent().parent().find("[name='input_reportID']");

        if (ctl.length == 0)
            ctl = $(this).parent().parent().parent().parent().find("[name='input_reportID']");

        var reportID = ctl.val();
        var frameWidth = $(window).width() * 0.9;
        var frameHeight = $(window).height() * 0.9;

        customIframe(2, 'Edit Report', window.preURL + "/ToolBoxFunctional/Reporting_Edit?reportID=" + reportID, frameWidth, frameHeight, 3);

    });

    // 报表添加到收藏
    $("[name='a_report_addToFavorites']").click(function () {

        event.preventDefault();

        var isAdd = true;
        var type_ID = 0;

        var ctl = $(this).parent().parent().parent().find("[name='input_reportID']");

        if (ctl.length == 0)
            ctl = $(this).parent().parent().parent().parent().find("[name='input_reportID']");

        var object_ID = ctl.val();

        var control_Hide = $(this);
        var control_Show = $(this).parent().find("[name='a_report_removeFromFavorites']");

        update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show);

    });

    // 报表移除收藏
    $("[name='a_report_removeFromFavorites']").click(function () {

        event.preventDefault();

        var isAdd = false;
        var type_ID = 0;

        var ctl = $(this).parent().parent().parent().find("[name='input_reportID']");

        if (ctl.length == 0)
            ctl = $(this).parent().parent().parent().parent().find("[name='input_reportID']");

        var object_ID = ctl.val();

        var control_Hide = $(this);
        var control_Show = $(this).parent().find("[name='a_report_addToFavorites']");

        update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show);

    });

    // 鼠标进入快速链接图片区域时，显示工具条
    $('.div_quickLink_ImgView').mouseenter(function () {

        var ctl = $(this).find(".div_quickLink_Btn");

        // 0.5秒后若仍在图片区域内，显示工具条
        trigger = setTimeout(function () {

            if (ctl.is(":hidden"))
                ctl.slideDown();

        }, 500);

    });

    // 鼠标离开快速链接图片区域时，隐藏工具条
    $(".div_quickLink_ImgView").mouseleave(function () {

        if (trigger != undefined)
            clearTimeout(trigger);

        var ctl = $(this).find(".div_quickLink_Btn");

        if (!ctl.is(":hidden"))
            ctl.slideUp();

    });

    // 打开编辑快速链接模块
    $("[name='a_quickLink_edit']").click(function (event) {
        event.preventDefault();

        var ctl = $(this).parent().parent().parent().find("[name='input_quickLinkID']");

        if (ctl.length == 0)
            ctl = $(this).parent().parent().parent().parent().find("[name='input_quickLinkID']");

        var quickLinkID = ctl.val();
        var frameWidth = $(window).width() * 0.9;
        var frameHeight = $(window).height() * 0.9;

        customIframe(2, 'Edit QuickLink', window.preURL + "/ToolBoxFunctional/QuickLink_Edit?quickLinkID=" + quickLinkID, frameWidth, frameHeight, 3);

    });

    // 快速链接添加到收藏
    $("[name='a_quickLink_addToFavorites']").click(function () {

        event.preventDefault();

        var isAdd = true;
        var type_ID = 1;

        var ctl = $(this).parent().parent().parent().find("[name='input_quickLinkID']");

        if (ctl.length == 0)
            ctl = $(this).parent().parent().parent().parent().find("[name='input_quickLinkID']");

        var object_ID = ctl.val();

        var control_Hide = $(this);
        var control_Show = $(this).parent().find("[name='a_quickLink_removeFromFavorites']");

        update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show);

    });

    // 快速链接移除收藏
    $("[name='a_quickLink_removeFromFavorites']").click(function () {

        event.preventDefault();

        var isAdd = false;
        var type_ID = 1;

        var ctl = $(this).parent().parent().parent().find("[name='input_quickLinkID']");

        if (ctl.length == 0)
            ctl = $(this).parent().parent().parent().parent().find("[name='input_quickLinkID']");

        var object_ID = ctl.val();

        var control_Hide = $(this);
        var control_Show = $(this).parent().find("[name='a_quickLink_addToFavorites']");

        update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show);

    });



    // 打开编辑自定义内容模块
    $("[name='a_customization_edit']").click(function (event) {

        event.preventDefault();

        var customizationID = $(this).parent().parent().parent().find("[name='input_customizationID']").val();
        var frameWidth = $(window).width() * 0.9;
        var frameHeight = $(window).height() * 0.9;

        customIframe(2, 'Edit Customization', window.preURL + "/ToolBoxFunctional/Customization_Edit?customizationID=" + customizationID, frameWidth, frameHeight, 3);

    });

});

var trigger;


function updateClickStatistics(rid) {
    $.ajax({
        url: window.preURL + "/api/ToolBox_Api/Update_ClickStatistics",
        type: "POST",
        ContentType: "application/json",
        dataType: "json",
        data: { "": rid },
        async: true,
        success: function (result) {
        }
    });
}







// 更新报表点击次数统计
function update_reportClickStatistics(r_ID) {
    var statisticsCtl = $("#span_clickStatistics_" + r_ID);

    $.ajax({
        url: window.preURL + "/api/ToolBox_Api/Update_ClickStatistics",
        type: "POST",
        ContentType: "application/json",
        dataType: "text",
        data: { "": r_ID },
        async: true,
        success: function (result) {
            var resultJSON = result
            if (typeof result === "string") {
                resultJSON = JSON.parse(result);
            }
            if (resultJSON.OpResult == true) {
                var rex = /^[0-9]*[1-9][0-9]*$/;
                // 验证返回信息，如果是正整数表示更新成功，更新前台显示

                // 验证返回信息，如果是正整数表示更新成功，更新前台显示
                if (rex.test(resultJSON.OpMsg)) {
                    statisticsCtl.text(Number(statisticsCtl.text()) + Number(resultJSON.OpMsg));
                }

            }
            else
                customMsg(2, resultJSON.OpMsg);

        }
    });

}

// Report Issues
function report_issues(r_ID) {
    // 阻止元素发生默认的行为
    var layedit = layui.layedit;
    var index = 0;

    // 弹出框 
    layer.open({
        type: 2,
        title: "pls describe the report errors:",
        fix: false,
        shadeClose: false,
        maxmin: true,
        area: ['780px', '545px'],
        content: window.preURL + '/SSML/layeditor',
        btn: ['Submit', 'Cancle'],
        anim: 3,
        yes: function (index, layro) {
            var remark = $(layro).find('iframe')[0].contentWindow.getContent(); //获得弹出页面;

            if (remark.length > 0) {

                var pageInfo = r_ID + "*" + remark;
                $.ajax({
                    url: window.preURL + "/api/ToolBox_Api/Report_Issues",
                    type: "POST",
                    ContentType: "application/json",
                    dataType: "json",
                    data: { "": pageInfo },
                    async: true,
                    beforeSend: function () {
                        loading_index = customLoading();
                    },
                    success: function (result) {

                        if (result.OpResult == true)
                            customAlert(1, result.OpMsg);
                        else
                            customAlert(2, result.OpMsg);

                    },
                    complete: function () {
                        layer.close(loading_index);
                        layer.close(index);
                    }
                });
            } else {
                customAlert(2, "Please input the errors!");
            }
        }

    });
}


// 申请单个报表权限
function get_reportAccess_single(r_ID, r_Name, user_Gid) {

    // 阻止元素发生默认的行为
    event.preventDefault();

    $.ajax({
        url: window.preURL + "/api/ToolBox_Api/CheckUser",
        type: "POST",
        ContentType: "application/json",
        dataType: "json",
        async: true,
        success: function (result) {

            if (result == true) {
                layer.open({
                    type: 1,
                    title: "Please input reason for access aplication :",
                    fix: false,
                    shadeClose: false,
                    maxmin: true,
                    area: ['530px', '380px'],
                    content: '<div class="container"> <div style="margin-top:15px" > <textarea style="height:230px"  class="layui-textarea" placeholder="Reason..."  id="remarkId"></textarea></div></div>',
                    btn: ['Submit', 'Cancle'],
                    yes: function (index, layro) {
                        var remark = $("#remarkId").val();
                        var valid = false;

                        if (remark.length > 0) {
                            valid = true;
                        }

                        if (valid) {
                            var pageInfo = r_ID + "*" + remark;
                            //var pageInfo = r_ID + "*" + remark;
                            $.ajax({
                                url: window.preURL + "/api/ToolBox_Api/Get_ReportAcess_Single",
                                type: "POST",
                                ContentType: "application/json",
                                dataType: "json",
                                data: { "": pageInfo },
                                async: true,
                                beforeSend: function () {
                                    loading_index = customLoading();
                                },
                                success: function (result) {
                                    if (result.OpResult == true)
                                        customAlert(1, result.OpMsg);
                                    else
                                        customAlert(2, result.OpMsg);
                                },
                                complete: function () {
                                    layer.close(loading_index);
                                    layer.close(index);
                                }
                            });
                        } else {
                            customAlert(2, "Please input the reason!");
                        }
                    }

                });
            }
            else {
                layer.open({
                    type: 1,
                    title: "Please input reason for access aplication :",
                    fix: false,
                    shadeClose: false,
                    maxmin: true,
                    area: ['530px', '350px'],
                    content: '<div class="container"> <div style="margin-top:15px" class="layui-form-item"><label class="layui-form-label">User_Gid</label><div class="layui-input-block"> <input type="text" id="userGid" name="userGid" required lay-verify="required" placeholder="UserGid" autocomplete="off" class="layui-input" /> </div></div> <div class="layui-form-item"><label class="layui-form-label">User_Email</label><div class="layui-input-block"> <input type="text" id="userEmail" name="UserEmail" required lay-verify="required" placeholder="UserEmail" autocomplete="off" class="layui-input" /> </div></div><div class="layui-form-item"><label class="layui-form-label">Reason</label><div class="layui-input-block"><textarea  class="layui-textarea" placeholder="Reason..."  id="remarkId"></textarea></div></div></div> ',
                    btn: ['Submit', 'Cancle'],
                    yes: function (index, layro) {
                        var remark = $("#remarkId").val();
                        var gid = $("#userGid").val();
                        var uEmail = $("#userEmail").val();
                        var valid = false;

                        if (remark.length > 0 && gid.length > 0 && uEmail.length > 0) {
                            valid = true;
                        }

                        if (valid) {
                            var pageInfo = r_ID + "*" + remark + "*" + gid + "*" + uEmail;
                            $.ajax({
                                url: window.preURL + "/api/ToolBox_Api/Get_ReportAcess_Single",
                                type: "POST",
                                ContentType: "application/json",
                                dataType: "json",
                                data: { "": pageInfo },
                                async: true,
                                beforeSend: function () {
                                    loading_index = customLoading();
                                },
                                success: function (result) {
                                    if (result.OpResult == true)
                                        customAlert(1, result.OpMsg);
                                    else
                                        customAlert(2, result.OpMsg);
                                },
                                complete: function () {
                                    layer.close(loading_index);
                                    layer.close(index);
                                }
                            });
                        } else {
                            customAlert(2, "Please complete the information!");
                        }
                    }

                });

            }
        }

    });



}

function get_Single_Access(r_Name, r_Link, currentUserId) {

    // 阻止元素发生默认的行为
    event.preventDefault();

    $.ajax({
        url: window.preURL + "/api/ToolBox_Api/CheckUser",
        type: "POST",
        ContentType: "application/json",
        dataType: "json",
        async: true,
        success: function (result) {
            if (result == true) {
                layer.open({
                    type: 1,
                    title: "Please input reason for access aplication :",
                    fix: false,
                    shadeClose: false,
                    maxmin: true,
                    area: ['530px', '380px'],
                    content: '<div class="container"> <div style="margin-top:15px" > <textarea style="height:230px"  class="layui-textarea" placeholder="Reason..."  id="remarkId"></textarea></div></div>',
                    btn: ['Submit', 'Cancle'],
                    yes: function (index, layro) {
                        var remark = $("#remarkId").val();
                        var valid = false;

                        if (remark.length > 0) {
                            valid = true;
                        }

                        if (valid) {
                            var pageInfo = r_Name + "*" + remark + "*" + r_Link;
                            $.ajax({
                                url: window.preURL + "/api/ToolBox_Api/Get_Single_Access",
                                type: "POST",
                                ContentType: "application/json",
                                dataType: "json",
                                data: { "": pageInfo },
                                async: true,
                                beforeSend: function () {
                                    loading_index = customLoading();
                                },
                                success: function (result) {
                                    if (result.OpResult == true)
                                        customAlert(1, result.OpMsg);
                                    else
                                        customAlert(2, result.OpMsg);
                                },
                                complete: function () {
                                    layer.close(loading_index);
                                    layer.close(index);
                                }
                            });
                        } else {
                            customAlert(2, "Please input the reason!");
                        }
                    }

                });
            }
            else {
                layer.open({
                    type: 1,
                    title: "Please input reason for access aplication :",
                    fix: false,
                    shadeClose: false,
                    maxmin: true,
                    area: ['530px', '350px'],
                    content: '<div class="container"> <div style="margin-top:15px" class="layui-form-item"><label class="layui-form-label">User_Gid</label><div class="layui-input-block"> <input type="text" id="userGid" name="userGid" required lay-verify="required" placeholder="UserGid" autocomplete="off" class="layui-input" /> </div></div> <div class="layui-form-item"><label class="layui-form-label">User_Email</label><div class="layui-input-block"> <input type="text" id="userEmail" name="UserEmail" required lay-verify="required" placeholder="UserEmail" autocomplete="off" class="layui-input" /> </div></div><div class="layui-form-item"><label class="layui-form-label">Reason</label><div class="layui-input-block"><textarea  class="layui-textarea" placeholder="Reason..."  id="remarkId"></textarea></div></div></div> ',
                    btn: ['Submit', 'Cancle'],
                    yes: function (index, layro) {
                        var remark = $("#remarkId").val();
                        var gid = $("#userGid").val();
                        var uEmail = $("#userEmail").val();
                        var valid = false;

                        if (remark.length > 0 && gid.length > 0 && uEmail.length > 0) {
                            valid = true;
                        }

                        if (valid) {
                            var pageInfo = r_Name + "*" + remark + "*" + gid + "*" + uEmail;
                            $.ajax({
                                url: window.preURL + "/api/ToolBox_Api/Get_Single_Access",
                                type: "POST",
                                ContentType: "application/json",
                                dataType: "json",
                                data: { "": pageInfo },
                                async: true,
                                beforeSend: function () {
                                    loading_index = customLoading();
                                },
                                success: function (result) {
                                    if (result.OpResult == true)
                                        customAlert(1, result.OpMsg);
                                    else
                                        customAlert(2, result.OpMsg);
                                },
                                complete: function () {
                                    layer.close(loading_index);
                                    layer.close(index);
                                }
                            });
                        } else {
                            customAlert(2, "Please complete the information!");
                        }
                    }

                });

            }
        }

    });



}


// 删除指定Item
function delete_Item(itemType, itemID, itemName, createUser, currentUserGID) {
    event.preventDefault();
    debugger
    if (createUser == currentUserGID) {
        layer.confirm(
            'Delete below item: <br /> ' + itemName,
            {
                icon: 3,
                title: 'Attention',
                btn: ['Yes', 'No']
            },
            function (index) {
                var loading_index;

                $.ajax({
                    url: window.preURL + "/api/ToolBox_Api/Delete_Item",
                    type: "POST",
                    ContentType: "application/json",
                    dataType: "json",
                    data: { "itemType": itemType, "itemID": itemID },
                    async: true,
                    beforeSend: function () {
                        loading_index = customLoading();
                    },
                    success: function (result) {
                        var resultJSON = result;
                        if (typeof result == 'string') {
                            resultJSON = JSON.parse(data)
                        }

                        if (resultJSON.OpResult == true)
                            customAlert(1, resultJSON.OpMsg, "refresh");
                        else
                            customAlert(2, resultJSON.OpMsg);

                    },
                    complete: function () {
                        layer.close(loading_index);
                    }
                });

                layer.close(index);

            });
    } else {
        layer.alert("您没有权限进行删除操作！");
    }

}

// 打开批量获取权限模块
function getAccess_multiple(deptID, subactionname, thirdactionname, fourthactionname) {
    var frameWidth = $(window).width() * 0.9;
    var frameHeight = $(window).height() * 0.9;

    customIframe(2, 'Get Multiple Access', window.preURL + "/ToolBoxFunctional/Report_GetAccess_Multiple?deptID=" + deptID + "&ReportID_List=&Sub_Action=" + subactionname + "&Third_Action=" + thirdactionname + "&Fourth_Action=" + fourthactionname, frameWidth, frameHeight, 3);

}

// 打开添加报表模块
function addReport(deptID, subID, thirdID, fourthID) {
    var frameWidth = $(window).width() * 0.9;
    var frameHeight = $(window).height() * 0.9;
    //var frameHeight = $(window).height();

    customIframe(2, 'Add Report', window.preURL + "/ToolBoxFunctional/Reporting_Add?deptID=" + deptID + "&subID=" + subID + "&thirdID=" + thirdID + "&fourthID=" + fourthID, frameWidth, frameHeight, 3);

}

// 打开添加工具模块
function addQuickLink(deptID, subID) {

    var frameWidth = $(window).width() * 0.9;
    var frameHeight = $(window).height() * 0.9;
    customIframe(2, 'Add QuickLink', window.preURL + "/ToolBoxFunctional/QuickLink_Add?deptID=" + deptID + "&subID=" + subID, frameWidth, frameHeight, 3);

}

// 打开添加自定义内容
function addCustomization(deptID) {

    var frameWidth = $(window).width() * 0.9;
    var frameHeight = $(window).height() * 0.9;

    customIframe(2, 'Add Customization', window.preURL + "/ToolBoxFunctional/Customization_Add?deptID=" + deptID, frameWidth, frameHeight, 3);

}

// 添加或移除收藏
function update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show) {
    var loading_index;

    $.ajax({
        url: window.preURL + "/api/ToolBox_Api/Update_Favorite",
        type: "POST",
        ContentType: "application/json",
        dataType: "json",
        data: { "isAdd": isAdd, "f_TypeID": type_ID, "f_ObjectID": object_ID },
        async: true,
        beforeSend: function () {
            loading_index = customLoading();
        },
        success: function (data) {
            var resultJSON = data;
            if (typeof data == 'string') {
                resultJSON = JSON.parse(data)
            }

            if (resultJSON.OpResult == true) {

                var thirdAction = $("#input_thirdAction").val();

                if (thirdAction == undefined) {

                    customMsg(1, resultJSON.OpMsg);
                    control_Hide.css("display", "none");
                    control_Show.css("display", "inline");

                }
                else {

                    if (thirdAction == "Reporting" || thirdAction == "QuickLink")
                        customAlert(1, resultJSON.OpMsg, "refresh");

                }

            }
            else
                customAlert(2, resultJSON.OpMsg);

        },
        complete: function () {
            layer.close(loading_index);
        }
    });

}

// 收藏夹下分类切换
function categorySwitch(targetPath) {

    window.location.href = targetPath;

}