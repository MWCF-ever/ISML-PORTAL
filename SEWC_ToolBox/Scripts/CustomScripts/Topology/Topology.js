$(document).ready(function () {
    $.ajaxSetup({
        async: false
    });

    nodes = new vis.DataSet();
    edges = new vis.DataSet();
    container = document.getElementById('div_network');
    data = {
        nodes: nodes,
        edges: edges
    };
    options = {
        nodes: {
            color: {
                border: '#101010',
                background: '#879baa',
                highlight: {
                    border: '#101010',
                    background: '#555f69'
                },
                hover: {
                    border: '#101010',
                    background: '#555f69'
                }
            },
            font: {
                color: '#ffffff'
            },
            shadow: {
                enabled: false,
                color: 'rgba(0,0,0,0.5)',
                size: 10,
                x: 5,
                y: 5
            },
            shape: 'box',
            shapeProperties: {
                borderRadius: 3 // only for box shape
            }
        },
        edges: {
            arrows: {
                to: { enabled: true, scaleFactor: 1, type: 'arrow' },
                middle: { enabled: false, scaleFactor: 1, type: 'arrow' },
                from: { enabled: false, scaleFactor: 1, type: 'arrow' }
            },
            smooth: {
                enabled: true,
                type: "cubicBezier",
                forceDirection: 'vertical',
                roundness: 0
            }
        },
        layout: {
            randomSeed: undefined,
            improvedLayout: false,
            hierarchical: {
                enabled: false,
                levelSeparation: 150,
                nodeSpacing: 200,
                treeSpacing: 150,
                blockShifting: true,
                edgeMinimization: true,
                parentCentralization: true,
                direction: 'UD',        // UD, DU, LR, RL
                sortMethod: 'hubsize'   // hubsize, directed
            }
        },
        physics: {
            enabled: false,
            barnesHut: {
                gravitationalConstant: -10000,
                centralGravity: 0,
                springLength: 95,
                springConstant: 0.04,
                damping: 0.09,
                avoidOverlap: 0
            },
            forceAtlas2Based: {
                gravitationalConstant: -200,
                centralGravity: 0,
                springConstant: 0.08,
                springLength: 100,
                damping: 0,
                avoidOverlap: 0
            },
            repulsion: {
                centralGravity: 0.2,
                springLength: 200,
                springConstant: 0.05,
                nodeDistance: 200,
                damping: 1
            },
            hierarchicalRepulsion: {
                centralGravity: 0.0,
                springLength: 100,
                springConstant: 0.01,
                nodeDistance: 120,
                damping: 0
            },
            maxVelocity: 0,
            minVelocity: 0,
            solver: 'repulsion',
            stabilization: {
                enabled: true,
                iterations: 1000,
                updateInterval: 100,
                onlyDynamicEdges: false,
                fit: true
            },
            timestep: 0.5,
            adaptiveTimestep: true
        },
        interaction: {
            navigationButtons: true,
            zoomView: true
        }
    };
    network = new vis.Network(container, data, options);

    //选择节点时触发
    network.on("selectNode", function (params) {

        var node_ID = params.nodes[0];

        $("#input_currentNodeID").val(node_ID);
        loadLinkage(node_ID);

    });

    //拖动时触发
    network.on("dragStart", function (params) {

        var node_ID = params.nodes[0];

        if (node_ID != undefined) {

            $("#input_currentNodeID").val(node_ID);
            loadLinkage(node_ID);

        }

    });

    //取消节点时触发
    network.on("deselectNode", function (params) {

        displayLinkage(0);

    });

    // 文件添加到收藏
    $("[name='a_file_addToFavorites']").click(function () {
        event.preventDefault();

        var isAdd = true;
        var type_ID = 2;
        var object_ID = $(this).attr("value");

        var control_Hide = $(this);
        var control_Show = $(this).parent().find("[name='a_file_removeFromFavorites']");

        update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show);

    });

    // 快速链接移除文件收藏
    $("[name='a_file_removeFromFavorites']").click(function () {
        event.preventDefault();

        var isAdd = false;
        var type_ID = 2;
        var object_ID = $(this).attr("value");

        var control_Hide = $(this);
        var control_Show = $(this).parent().find("[name='a_file_addToFavorites']");

        console.log(control_Show);

        update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show);

    });

    initial();

});

var nodes;
var edges;
var container;
var data;
var options;
var network;

var fitNodes = [];

function initial() {
    var dept_ID = $("#input_deptID").val();
    var menu_ID = $("#input_menuID").val();
    var loading_index;

    $.ajax({
        url: window.preURL + "/api/ToolBox_Api/Get_ProcessTopology_ByDeptID",
        type: "POST",
        ContentType: "application/json",
        dataType: "text",
        async: false,
        data: {
            "dept_ID": dept_ID,
            "menu_ID": menu_ID
        },
        beforeSend: function () {
            loading_index = customLoading();
        },
        success: function (result) {
            var resultJSON = result;
            if (typeof result == 'string') {
                resultJSON = JSON.parse(result);
            }
            
            if (resultJSON[0].length > 0 && resultJSON[1].length > 0) {

                for (var i = 0; i < resultJSON[0].length; i++) {

                    var curNode = resultJSON[0][i];

                    if (curNode.pn_m_ID == menu_ID) {
                        fitNodes.push(curNode.pn_ID);
                    }

                    var groupColor;

                    if (curNode.pn_m_ID == 46) {

                        groupColor = {
                            border: '#2d373c',
                            background: '#879baa',
                            highlight: {
                                border: '#2d373c',
                                background: '#555f69'
                            },
                            hover: {
                                border: '#2d373c',
                                background: '#555f69'
                            }
                        };

                    }
                    else if (curNode.pn_m_ID == 47) {

                        groupColor = {
                            border: '#005f87',
                            background: '#50bed7',
                            highlight: {
                                border: '#005f87',
                                background: '#2387aa'
                            },
                            hover: {
                                border: '#005f87',
                                background: '#2387aa'
                            }
                        };

                    }
                    else if (curNode.pn_m_ID == 48) {

                        groupColor = {
                            border: '#c85a1e',
                            background: '#ffb900',
                            highlight: {
                                border: '#c85a1e',
                                background: '#eb780a'
                            },
                            hover: {
                                border: '#c85a1e',
                                background: '#eb780a'
                            }
                        };

                    }
                    else if (curNode.pn_m_ID == 49) {

                        groupColor = {
                            border: '#00646e',
                            background: '#41aaaa',
                            highlight: {
                                border: '#00646e',
                                background: '#0f8287'
                            },
                            hover: {
                                border: '#00646e',
                                background: '#0f8287'
                            }
                        };

                    }

                    nodes.add({
                        id: curNode.pn_ID,
                        label: curNode.pn_NodeName,
                        title: "Owner: " + curNode.User_Owner,
                        color: groupColor,
                        x: curNode.pn_Position_X,
                        y: curNode.pn_Position_Y
                    });

                }

                for (var i = 0; i < resultJSON[1].length; i++) {

                    var curConnection = resultJSON[1][i];

                    var isDashes = false;

                    if (curConnection.pc_ConnectionType == 2)
                        isDashes = true;
                    edges.add({
                        from: curConnection.pc_Node_ID,
                        to: curConnection.pc_NextNode_ID,
                        dashes: isDashes,
                        label: curConnection.pc_ConnectionLabel
                    });

                }

                var tempOption = {
                    nodes: fitNodes
                };

                network.fit(tempOption);
                $("#div_topology").css("display", "block");
                $("#div_noData").css("display", "none");

            }
            else {
                $("#div_topology").css("display", "none");
                $("#div_noData").css("display", "block");
            }

        },
        complete: function () {
            displayLinkage(0);
            layer.close(loading_index);
        }
    });

}

function addToFavorites(fileId, nodeId) {
    event.preventDefault();

    var isAdd = true;
    var type_ID = 2;
    var object_ID = fileId;
    var control_Hide = $(this);
    var control_Show = $(this).parent().find("[name='a_file_removeFromFavorites']");

    update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show, nodeId);
}

function removeToFavorites(fileId, nodeId) {
    event.preventDefault();

    var isAdd = false;
    var type_ID = 2;
    var object_ID = fileId;
    var control_Hide = $(this);
    var control_Show = $(this).parent().find("[name='a_file_removeFromFavorites']");

    update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show, nodeId);
}

// 添加或移除收藏
function update_favorite(isAdd, type_ID, object_ID, control_Hide, control_Show, nodeId) {
    var loading_index;

    $.ajax({
        url: window.preURL + "/api/ToolBox_Api/Update_Favorite",
        type: "POST",
        ContentType: "application/json",
        dataType: "text",
        data: { "isAdd": isAdd, "f_TypeID": type_ID, "f_ObjectID": object_ID },
        async: true,
        beforeSend: function () {
            loading_index = customLoading();
        },
        success: function (data) {
            var resultJSON = data;
            if (typeof data == 'string') {
                resultJSON = JSON.parse(data);
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
                loadLinkage(nodeId);
            }
            else
                customAlert(2, resultJSON.OpMsg);

        },
        complete: function () {
            layer.close(loading_index);
        }
    });

}

function loadLinkage(node_ID) {

    var loading_index;
    var currentUserGID = $("#input_currentUserGID").val();
    var a_type_U = $("#input_a_type_U").val();
    var a_type_D = $("#input_a_Type_D").val();

    $.ajax({
        url: window.preURL + "/api/ToolBox_Api/Get_ProcessLinkage_ByNodeID",
        type: "POST",
        ContentType: "application/json",
        dataType: "text",
        data: { "": node_ID },
        async: true,
        beforeSend: function () {
            loading_index = customLoading();
        },
        success: function (result) {
            var resultJSON = result;
            if (typeof result == 'string') {
                resultJSON = JSON.parse(result);
            }

            $("#table_document tbody").empty();
            $("#table_report tbody").empty();
            $("#table_tool tbody").empty();

            var nodeOwner = resultJSON[0].pn_Owner;

            if (resultJSON[1].length > 0) {

                var temp_document = new Array();
                var temp_report = new Array();
                var temp_tool = new Array();

                var rownum_document = 0
                var rownum_report = 0;
                var rownum_tool = 0;

                for (var i = 0; i < resultJSON[1].length; i++) {

                    var curLinkage = resultJSON[1][i];
                    var temp = new Array();
                    var rownum;

                    if (curLinkage.pl_Type == 1) {
                        rownum_document = rownum_document + 1;
                        rownum = rownum_document;
                    }
                    else if (curLinkage.pl_Type == 2) {
                        rownum_report = rownum_report + 1;
                        rownum = rownum_report;
                    }
                    else if (curLinkage.pl_Type == 3) {
                        rownum_tool = rownum_tool + 1;
                        rownum = rownum_tool;
                    }

                    temp.push("<tr>");

                    temp.push("<td>" + rownum + "</td>");
                    temp.push("<td>" + curLinkage.pl_Name + "</td>");

                    temp.push("<td>");
                    console.log(curLinkage.Favorite);
                    // 判断是否收藏夹的显示
                    if (curLinkage.Favorite != null && curLinkage.Favorite == currentUserGID) {
                        temp.push("<a class='btn btn-primary  btn_rowOperation' value='" + curLinkage.pl_ID + "' name='a_file_addToFavorites'  onclick='addToFavorites(" + curLinkage.pl_ID + "," + node_ID + ")'   title='AddToFavorites' style='display: none;'><i class='layui-icon' style='font-size: 18px; color: white;'>&#xe600;</i></a>");
                        temp.push("<a class='btn btn-primary layui-btn btn_rowOperation' value='" + curLinkage.pl_ID + "' name='a_file_removeFromFavorites' onclick='removeToFavorites(" + curLinkage.pl_ID + "," + node_ID + ")' title='RemoveFromFavorites'><i class='layui-icon' style='font-size: 18px; color: white;'>&#xe658;</i></a>");
                    }
                    else {
                        temp.push("<a class='btn btn-primary layui-btn btn_rowOperation' value='" + curLinkage.pl_ID + "' name='a_file_addToFavorites' onclick='addToFavorites(" + curLinkage.pl_ID + "," + node_ID + ")' title='AddToFavorites'><i class='layui-icon' style='font-size: 18px; color: white;'>&#xe600;</i></a>");
                        temp.push("<a class='btn btn-primary layui-btn btn_rowOperation' value='" + curLinkage.pl_ID + "' name='a_file_removeFromFavorites' onclick='removeToFavorites(" + curLinkage.pl_ID + "," + node_ID + ")' title='RemoveFromFavorites' style='display: none;'><i class='layui-icon' style='font-size: 18px; color: white;'>&#xe658;</i></a>");

                    }
                    if (curLinkage.pl_Type == 2) {
                        // 权限申请
                        temp.push("<a class='btn btn-primary layui-btn btn_rowOperation' target='_blank' name='a_report_getAccess_single' title='Get Access' style='margin-left:5px;margin-right:5px;' onclick='get_Single_Access(\"" + curLinkage.pl_Name + "\",\"" + curLinkage.pl_Linkage + "\",\"" + currentUserGID + "\");'><i class='layui-icon' style='font-size: 18px; color: white;'>&#xe857;</i></a>");
                    }

                    // visit
                    if (/^\\\\+/.test(curLinkage.pl_Linkage)) {
                        temp.push("<a class='btn btn-primary layui-btn btn_rowOperation btn_copy_report' data-clipboard-text='" + curLinkage.pl_Linkage + "' title='Visit' href='javascript:void(0)'><i class='layui-icon' style='font-size:18px;'>&#xe602;</i></a>");
                    }
                    else {
                        temp.push("<a class='btn btn-primary layui-btn btn_rowOperation' title='Visit' target='_blank' href='" + curLinkage.pl_Linkage + "'><i class='layui-icon' style='font-size:18px;'>&#xe602;</i></a>");
                    }
                    if (a_type_U >= 2 || (a_type_U != 0 && currentUserGID == nodeOwner))
                        temp.push("<a class='btn btn-primary layui-btn btn_rowOperation' title='Edit' name='a_processLinkage_edit' onclick='editLinkage(" + curLinkage.pl_ID + "," + curLinkage.pl_Type + ")'><i class='layui-icon' style='font-size: 18px;'>&#xe642;</i></a>");

                    if (a_type_D >= 2 || (a_type_D != 0 && currentUserGID == nodeOwner))
                        temp.push("<a class='btn btn-primary layui-btn btn_rowOperation' title='Delete' name='a_processLinkage_delete' onclick='deleteLinkage(" + node_ID + "," + curLinkage.pl_ID + ",\"" + curLinkage.pl_Name + "\"," + rownum + ");'><i class='layui-icon' style='font-size: 18px;'>&#xe640;</i></a>");

                    temp.push("</td>");

                    temp.push("</tr>");

                    if (curLinkage.pl_Type == 1)
                        temp_document.push(temp.join(''));
                    else if (curLinkage.pl_Type == 2)
                        temp_report.push(temp.join(''));
                    else if (curLinkage.pl_Type == 3)
                        temp_tool.push(temp.join(''));

                }

                $("#table_document tbody").append(temp_document.join(''));
                $("#table_report tbody").append(temp_report.join(''));
                $("#table_tool tbody").append(temp_tool.join(''));

                displayLinkage(2);
            }
            else
                displayLinkage(1);

        },
        complete: function () {
            layer.close(loading_index);
        }
    });

}

function addLinkage(pl_Type) {

    var node_ID = $("#input_currentNodeID").val();
    var frameWidth = $(window).width() * 0.7;
    var frameHeight = $(window).height() * 0.7;

    var title;

    if (pl_Type == 1)
        title = "Add Document Linkage";
    else if (pl_Type == 2)
        title = "Add Report Linkage";
    else if (pl_Type == 3)
        title = "Add Tool Linkage";

    customIframe(2, title, window.preURL + "/ToolBoxFunctional/ProcessLinkage_Add?node_ID=" + node_ID + "&pl_Type=" + pl_Type, frameWidth, frameHeight, 3);

}

function editLinkage(pl_ID, pl_Type) {

    var node_ID = $("#input_currentNodeID").val();
    var frameWidth = $(window).width() * 0.7;
    var frameHeight = $(window).height() * 0.7;

    var title;

    if (pl_Type == 1)
        title = "Edit Document Linkage";
    else if (pl_Type == 2)
        title = "Edit Report Linkage";
    else if (pl_Type == 3)
        title = "Edit Tool Linkage";

    customIframe(2, title, window.preURL + "/ToolBoxFunctional/ProcessLinkage_Edit?node_ID=" + node_ID + "&pl_ID=" + pl_ID + "&pl_Type=" + pl_Type, frameWidth, frameHeight, 3);

}

function deleteLinkage(node_ID, pl_ID, Name, rowNum) {
    layer.confirm(
        'Delete ' + Name + ' at row ' + rowNum + ' ?',
        {
            icon: 3,
            title: 'Attention',
            btn: ['Yes', 'No']
        },
        function (index) {

            $.ajax({
                url: window.preURL + "/api/ToolBox_Api/Delete_ProcessLinkage_ByID",
                type: "POST",
                ContentType: "application/json",
                dataType: "text",
                data: { "": pl_ID },
                async: false,
                success: function (result) {
                    var resultJSON = result;
                    if (typeof result == 'string') {
                        resultJSON = JSON.parse(result);
                    }
                    // 上传完毕回调
                    if (resultJSON.OpResult == true) {

                        loadLinkage(node_ID);
                        customMsg(1, resultJSON.OpMsg);

                    }
                    else
                        customAlert(2, resultJSON.OpMsg);

                }
            })

        });

}

// 0: no node selected; 1: no linkages for selected node; 2: display linkages
function displayLinkage(type) {

    if (type == 0) {

        $("#div_linkage").css("display", "none");

    }
    else if (type == 1) {

        $("#div_linkage").css("display", "block");

        $("#h4_noDocument").css("display", "block");
        $("#h4_noReport").css("display", "block");
        $("#h4_noTool").css("display", "block");

        $("#table_document").css("display", "none");
        $("#table_report").css("display", "none");
        $("#table_tool").css("display", "none");

    }
    else if (type == 2) {

        $("#div_linkage").css("display", "block");

        if ($("#table_document tbody").html() != '') {
            $("#h4_noDocument").css("display", "none");
            $("#table_document").css("display", "");
        }
        else {
            $("#h4_noDocument").css("display", "block");
            $("#table_document").css("display", "none");
        }

        if ($("#table_report tbody").html() != '') {
            $("#h4_noReport").css("display", "none");
            $("#table_report").css("display", "");
        }
        else {
            $("#h4_noReport").css("display", "block");
            $("#table_report").css("display", "none");
        }

        if ($("#table_tool tbody").html() != '') {
            $("#h4_noTool").css("display", "none");
            $("#table_tool").css("display", "");
        }
        else {
            $("#h4_noTool").css("display", "block");
            $("#table_tool").css("display", "none");
        }

    }

}