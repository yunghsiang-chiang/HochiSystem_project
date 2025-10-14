<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewFriend.aspx.cs" Inherits="CRM_NewFriend" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>新朋友CRM</title>
    <style>
        .btn-lg {
            font-size: 1.05rem;
        }

        .form-control {
            font-size: 1.05rem;
        }

        .section {
            border: 1px solid #eee;
            border-radius: .75rem;
            padding: 1rem;
            margin-bottom: 1rem;
        }

        .hint {
            background: #fff3cd;
            padding: .5rem .75rem;
            border-radius: .5rem;
        }
    </style>
    <!-- head 內：CSS -->
    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css">

    <!-- body 底部、你自己的 <script> 之前：JS 載入順序非常重要！ -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/js/bootstrap.bundle.min.js"></script>

</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container py-3">
        <asp:ScriptManager runat="server" EnablePageMethods="true" />
        <h4 class="mb-3">新朋友 CRM（搜尋 → 新增 → 接觸紀錄）</h4>

        <!-- 1) 搜尋 -->
        <div class="section bg-white">
            <div class="form-group">
                <label>手機號碼</label>
                <input id="txtMobile" class="form-control" maxlength="20" placeholder="請輸入手機（09xxxxxxxx）">
            </div>
            <div class="form-group">
                <label>或 姓名</label>
                <input id="txtName" class="form-control" maxlength="50" placeholder="可選">
            </div>
            <button id="btnSearch" type="button" class="btn btn-primary btn-lg">搜尋</button>
            <div id="searchHint" class="mt-2 small text-muted"></div>
        </div>

        <!-- 2) 結果/新增卡 -->
        <div id="cardArea" class="section bg-white" style="display: none;"></div>

        <!-- 3) 接觸紀錄 -->
        <div id="interactionArea" class="section bg-white" style="display: none;">
            <h5 class="mb-2">新增接觸紀錄</h5>
            <div class="form-row">
                <div class="form-group col-6">
                    <label>方式</label>
                    <select id="selMethod" class="form-control">
                        <option>電話</option>
                        <option>面談</option>
                        <option>LINE</option>
                        <option>活動</option>
                    </select>
                </div>
                <div class="form-group col-6">
                    <label>意向</label>
                    <select id="selIntent" class="form-control">
                        <option>關注</option>
                        <option>想體驗</option>
                        <option>已參加</option>
                        <option>待追蹤</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label>下一步</label>
                <input id="txtNext" class="form-control" maxlength="100" placeholder="例如：邀約體驗課 / 回電">
            </div>
            <div class="form-group">
                <label>提醒日期（可選）</label>
                <input id="txtNextDate" type="date" class="form-control">
            </div>
            <div class="form-group">
                <label>備註</label>
                <textarea id="txtMemo" class="form-control" rows="3"></textarea>
            </div>
            <button id="btnSaveInteraction" type="button" class="btn btn-success btn-lg">儲存接觸</button>
            <div id="interactionHint" class="mt-2 small text-muted"></div>
        </div>

        <asp:HiddenField ID="hidNewFriendId" runat="server" />
        <asp:HiddenField ID="hidMyHID" runat="server" />
        <asp:HiddenField ID="hidChannel" runat="server" />

        <div class="modal fade" id="savedModal" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-body text-center">
                        <div class="mb-2 font-weight-bold">已儲存</div>
                        <button type="button" class="btn btn-primary btn-block mb-2" id="btnNewInteraction">繼續為此人新增接觸</button>
                        <button type="button" class="btn btn-outline-primary btn-block mb-2" id="btnResetForNext">填寫下一位新朋友</button>
                        <button type="button" class="btn btn-light btn-block" data-dismiss="modal">關閉</button>
                    </div>
                </div>
            </div>
        </div>

    </form>




    <script>
        // 取同修 HID（你現有：cookie person_id）
        $(function () {
            // 這裡示範從伺服器注入；也可由 cookie 注入
            if (!$('#<%=hidMyHID.ClientID%>').val()) {
                // 預留防呆
            }
            // 來源渠道（可從 URL ?channel=活動）
            const url = new URL(window.location.href);
            $('#<%=hidChannel.ClientID%>').val(url.searchParams.get('channel') || '道場');
        });

        $('#btnSearch').on('click', function () {
            const mobile = $('#txtMobile').val().trim();
            const name = $('#txtName').val().trim();
            $('#searchHint').text('查詢中...');
            PageMethods.SearchPerson(mobile, name, function (res) {
                if (res.Found) {
                    $('#searchHint').html('<span class="hint">找到相符資料，請確認/補充後可新增接觸紀錄。</span>');
                    renderCard(res.Data, true);
                    $('#<%=hidNewFriendId.ClientID%>').val(res.Data.NewFriendId);
                    $('#interactionArea').show();
                } else {
                    $('#searchHint').html('<span class="hint">找不到，請建立新朋友（至少填姓名與手機）。</span>');
                    renderCard({ FullName: name, MobilePhone: mobile }, false);
                    $('#interactionArea').hide();
                }
                $('#cardArea').show();
            }, function (err) { $('#searchHint').text('查詢失敗'); });
        });

        function renderCard(d, isExisting) {
            var html = '';
            html += '<div class="form-row">';
            html += '  <div class="form-group col-6"><label>姓名</label><input id="nfName" class="form-control" value="' + (d.FullName || '') + '"></div>';
            html += '  <div class="form-group col-6"><label>手機</label><input id="nfMobile" class="form-control" value="' + (d.MobilePhone || '') + '"></div>';
            html += '</div>';
            html += '<div class="form-row">';
            html += '  <div class="form-group col-6"><label>城市</label><input id="nfCity" class="form-control" value="' + (d.City || '') + '"></div>';
            html += '  <div class="form-group col-6"><label>區</label><input id="nfDistrict" class="form-control" value="' + (d.District || '') + '"></div>';
            html += '</div>';
            html += '<div class="form-group"><label>地址</label><input id="nfAddress" class="form-control" value="' + (d.Address || '') + '"></div>';
            if (isExisting) {
                html += '<button id="btnUpdateNF" type="button" class="btn btn-outline-primary btn-lg mr-2">更新資料</button>';
            } else {
                html += '<button id="btnCreateNF" type="button" class="btn btn-primary btn-lg mr-2">建立新朋友</button>';
            }
            $('#cardArea').html(html);
            gateNoHid(); // ★ 讓未帶 myHid 的情況下，建立/更新按鈕一律擋住

            $('#btnCreateNF').off().on('click', function () {
                const payload = collectNF();
                const myHID = $('#<%=hidMyHID.ClientID%>').val();
                const channel = $('#<%=hidChannel.ClientID%>').val();
                PageMethods.CreateNewFriend(payload, parseInt(myHID || '0'), channel, function (r) {
                    if (r.Ok) {
                        $('#<%=hidNewFriendId.ClientID%>').val(r.NewFriendId);
                        $('#searchHint').html('<span class="hint">已建立，現在可新增接觸紀錄。</span>');
                        $('#interactionArea').show();
                        resetInteractionForm();
                        document.getElementById('interactionArea').scrollIntoView({ behavior: 'smooth' });
                    } else {
                        alert(r.Msg || '建立失敗');
                    }
                }, function () { alert('建立失敗'); });
            });

            $('#btnUpdateNF').off().on('click', function () {
                const payload = collectNF();
                const id = $('#<%=hidNewFriendId.ClientID%>').val();
                const myHID = $('#<%=hidMyHID.ClientID%>').val();
                PageMethods.UpdateNewFriend(parseInt(id || '0'), payload, parseInt(myHID || '0'), function (r) {
                    if (r.Ok) { alert('已更新'); } else { alert(r.Msg || '更新失敗'); }
                }, function () { alert('更新失敗'); });
            });
        }

        function collectNF() {
            return {
                FullName: $('#nfName').val().trim(),
                MobilePhone: $('#nfMobile').val().trim(),
                City: $('#nfCity').val().trim(),
                District: $('#nfDistrict').val().trim(),
                Address: $('#nfAddress').val().trim()
            };
        }

        $('#btnSaveInteraction').on('click', function () {
            const id = parseInt($('#<%=hidNewFriendId.ClientID%>').val() || '0');
            const myHID = parseInt($('#<%=hidMyHID.ClientID%>').val() || '0');

            if (!id) { alert('請先建立/選擇新朋友，再儲存接觸'); return; }
            if (!myHID) { alert('未取得同修識別 myHid，請用專屬 QR 開啟本頁'); return; }

            const dto = {
                NewFriendId: id,
                ContactHID: myHID,
                Method: $('#selMethod').val(),
                IntentLevel: $('#selIntent').val(),
                NextAction: $('#txtNext').val().trim(),
                NextActionDate: $('#txtNextDate').val(),
                Memo: $('#txtMemo').val().trim(),
                // 這五個是同步主檔用
                FullName: $('#nfName').val().trim(),
                MobilePhone: $('#nfMobile').val().trim(),
                City: $('#nfCity').val().trim(),
                District: $('#nfDistrict').val().trim(),
                Address: $('#nfAddress').val().trim()
            };

            $('#interactionHint').text('儲存中...');
            PageMethods.AddInteraction(dto, function (r) {
                if (r.Ok) {
                    $('#interactionHint').text('已儲存');
                    $('#savedModal').modal('show');   // ★ 顯示成功提醒 + 下一步選項
                } else {
                    $('#interactionHint').text(r.Msg || '失敗');
                }
            }, function () { $('#interactionHint').text('失敗'); });

        });

        $(function () {
            const myHID = parseInt($('#<%=hidMyHID.ClientID%>').val() || '0', 10);
            const channel = $('#<%=hidChannel.ClientID%>').val() || '道場';

            // 若未帶 myHid，直接鎖住互動與儲存，給提示
            if (!myHID) {
                $('#btnSaveInteraction').prop('disabled', true);
                $('#searchHint').html('<span class="hint">未取得同修識別 (myHid)。請使用專屬QR或在網址後加上 ?myHid=你的HID</span>');
            }

            // 讓你能在 UI 角落顯示目前同修身分與來源（可選）
            console.log('ACTIVE ContactHID=', myHID, 'channel=', channel);
        });

        function resetForNextPerson() {
            // 清搜尋區與卡片、隱藏互動區
            $('#txtMobile, #txtName').val('');
            $('#cardArea').hide().empty();
            $('#interactionArea').hide();
            $('#<%=hidNewFriendId.ClientID%>').val(''); // 下一位
            $('#searchHint').text('');
            // 捲回頂端，方便手機繼續掃描/輸入
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        $('#btnNewInteraction').on('click', function () { $('#savedModal').modal('hide'); });
        $('#btnResetForNext').on('click', function () { $('#savedModal').modal('hide'); resetForNextPerson(); });

        PageMethods.AddInteraction(dto, function (r) {
            if (r.Ok) {
                $('#interactionHint').text('已儲存');
                $('#savedModal').modal('show'); // ⬅ 顯示下一步選單
            } else {
                $('#interactionHint').text(r.Msg || '失敗');
            }
        }, function () { $('#interactionHint').text('失敗'); });

        // 清空互動表單
        function resetInteractionForm() {
            $('#selMethod').val('電話');
            $('#selIntent').val('關注');
            $('#txtNext').val('');
            $('#txtNextDate').val('');
            $('#txtMemo').val('');
            $('#interactionHint').text('');
        }

        // 清空整頁，準備下一位
        function resetForNextPerson() {
            $('#txtMobile, #txtName').val('');
            $('#cardArea').hide().empty();
            $('#interactionArea').hide();
            $('#<%=hidNewFriendId.ClientID%>').val('');
            resetInteractionForm();
            $('#searchHint').text('');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        // 綁定 Modal 兩個按鈕
        $(document).on('click', '#btnNewInteraction', function () { $('#savedModal').modal('hide'); });
        $(document).on('click', '#btnResetForNext', function () { $('#savedModal').modal('hide'); resetForNextPerson(); });

        function gateNoHid() {
            const myHID = parseInt($('#<%=hidMyHID.ClientID%>').val() || '0', 10);
            if (!myHID) {
                // 禁用建立/更新/儲存，並提示
                $('#btnSaveInteraction').prop('disabled', true);
                // 之後 renderCard 會重繪按鈕，這裡先監聽
                $(document).off('click.nohid').on('click.nohid', '#btnCreateNF,#btnUpdateNF', function (e) {
                    e.preventDefault();
                    alert('未取得同修識別 (myHid)。請使用專屬 QR 或在網址後加上 ?myHid=你的HID');
                    return false;
                });
                // 搜尋可以，但任何互動區保持隱藏
                $('#interactionArea').hide();
                $('#searchHint').html('<span class="hint">未取得同修識別 (myHid)。可先查詢，但無法建立/更新/儲存接觸。</span>');
            } else {
                $('#btnSaveInteraction').prop('disabled', false);
                $(document).off('click.nohid');
            }
        }

        // 進頁與每次 renderCard 後都執行一次 gate
        $(function () { gateNoHid(); });

    </script>
</body>
</html>
